﻿using Filestransformer.StateMachines.FileTransformers.Events;
using System;
using System.IO;
using System.Threading;

namespace Filestransformer.StateMachines.FileTransformers
{
    public class LowercaseFileTransformer : FileTransformer
    {
        private FileStream inputFileStream;
        private FileStream outputFileStream;

        protected override void InitializeFileTransformer()
        {
            base.InitializeFileTransformer();

            bool init = false;
            int retryAttempt = 0;
            Exception lastException = null;

            do
            {
                try
                {
                    inputFileStream = File.OpenRead(Path.Combine(inputDirectory, fileName));
                    outputFileStream = File.OpenWrite(Path.Combine(outputDirectory, fileName));
                    init = true;
                    break;
                }
                catch (Exception ex)
                {
                    // todo: weird code for retry, hitting INPUT file stream being used by another process in win 11 :(, proc explorer doesnt show up anything
                    // retrying after sometime seems to work
                    lastException = ex;
                    status = FileTransformationStatus.Failed;
                    Thread.Sleep(1000);
                }
            } while (retryAttempt < 3);

            if (!init && retryAttempt >= 3)
            {
                status = FileTransformationStatus.Failed;
                SendFileTranformationResponse(FileTransformationStatus.Failed, fileName, lastException?.Message);

                // send completion status to self and proceed to halt this machine
                this.Send(Id, new eFileTranformationCompletionEvent());
            }

            //try
            //{
            //    //inputFileStream = File.OpenRead(Path.Combine(inputDirectory, fileName));
            //    inputFileStream = File.Open(Path.Combine(inputDirectory, fileName), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            //    outputFileStream = File.OpenWrite(Path.Combine(outputDirectory, fileName));
            //}
            //catch (Exception ex) 
            //{
            //    status = FileTransformationStatus.Failed;
            //    SendFileTranformationResponse(FileTransformationStatus.Failed, fileName, ex.Message);

            //    // send completion status to self and proceed to halt this machine
            //    this.Send(Id, new eFileTranformationCompletionEvent());
            //}

            status = FileTransformationStatus.InProgress;
        }

        protected override void SendFileTransformationRequest()
        {
            var machine = this.CreateMachine(typeof(LowercaseFileChunkTransformer));
            this.Send(machine, new eFileChunkTransformRequestEvent(this.Id, inputFileStream, fileChunkSizeToReadInBytes, fileEncoding));
        }

        protected override void HandleFileChunkResponse()
        {
            var response = ReceivedEvent as eFileChunkTransformResponseEvent;
            if (response.Status == FileTransformationStatus.Success)
            {
                status = FileTransformationStatus.Success;

                outputFileStream.Write(response.TransformedBytes, 0, response.TransformedBytes.Length);
                outputFileStream.Flush();
            }
            else if (response.Status == FileTransformationStatus.Failed)
            {
                status = FileTransformationStatus.Failed;
                failureReason = response.FailureReason;

                // stop processing file transformation
                // send completion status to self and proceed to halt this machine
                this.Send(Id, new eFileTranformationCompletionEvent());
            }
        }

        public override void HandleFileTransformationRequestCompleted()
        {
            SendFileTranformationResponse(status, fileName, failureReason);
            DisposeFileStreams();
        }

        protected override bool CompletedReadingInputFile() => 
            inputFileStream?.Position >= inputFileStream?.Length;
            
        private void DisposeFileStreams()
        {
            inputFileStream?.Close();
            outputFileStream?.Close();
        }

        private void SendFileTranformationResponse(FileTransformationStatus status, string fileName, string failureReason = "")
        {
            if (status == FileTransformationStatus.Success)
            {
                var completionTime = DateTime.UtcNow;
                var timeToComplete = completionTime.Subtract(timeOfRequest);
                this.Send(sender, new eFileTranformationResponseEvent(status, fileName, completionTime, timeToComplete, failureReason));
            }
            else if (status == FileTransformationStatus.Failed)
            {
                this.Send(sender, new eFileTranformationResponseEvent(status, fileName, null, null, failureReason));
            }
            else if (status == FileTransformationStatus.InProgress)
            {
                this.Send(sender, new eFileTranformationResponseEvent(status, fileName, null, null, string.Empty));
            }
            else
            {
                throw new InvalidOperationException($"Unexpected enum value for {nameof(FileTransformationStatus)}: {status}");
            }
        }
    }
}
