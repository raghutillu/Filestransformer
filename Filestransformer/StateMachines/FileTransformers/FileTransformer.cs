﻿using Filestransformer.StateMachines.CommonEvents;
using Filestransformer.StateMachines.FileTransformers.Events;
using Filestransformer.Support.Logger;
using Microsoft.PSharp;
using System.IO;


namespace Filestransformer.StateMachines.FileTransformers
{
    public abstract partial class FileTransformer : Machine
    {
        protected ILogger logger;
        protected string fileName;
        protected string inputDirectory;
        protected string outputDirectory;

        protected virtual void InitializeFileTransformer()
        {
            var config = ReceivedEvent as eFileTransformerEvent;
            logger = config.Logger;
            fileName = config.FileName;
            inputDirectory = config.InputDirectory;
            outputDirectory = config.OutputDirectory;
        }
    }
}
