﻿namespace Filestransformer.Settings
{
    public class Setting
    {
        public int FileGroups { get; private set; }

        public int MaxParallelFileTransformations { get; private set; }

        public string InputFolderPath { get; private set; }

        public string OutputFolderPath { get; private set; }

        public Setting(int fileGroups, int maxParallelFileTransformations, string inputFolderPath, string outputFolderPath)
        {
            this.FileGroups = fileGroups;
            this.MaxParallelFileTransformations = maxParallelFileTransformations;
            this.InputFolderPath = inputFolderPath;
            this.OutputFolderPath = outputFolderPath;
        }

        /// <summary>
        /// Merges settings from <seealso cref="Settings"/> if current instance members are not set
        /// </summary>
        /// <param name="otherSettings"></param>
        public Setting MergeFrom(Setting otherSettings)
        {
            if (this.FileGroups == 0)
            { 
                this.FileGroups = otherSettings.FileGroups; 
            }

            if (this.MaxParallelFileTransformations == 0)
            { 
                this.MaxParallelFileTransformations = otherSettings.MaxParallelFileTransformations; 
            }

            if (string.IsNullOrWhiteSpace(this.InputFolderPath))
            { 
                this.InputFolderPath = otherSettings.InputFolderPath; 
            }

            if (string.IsNullOrWhiteSpace(this.OutputFolderPath))
            { 
                this.OutputFolderPath = otherSettings.OutputFolderPath; 
            }
            return this;
        }
    }
}
