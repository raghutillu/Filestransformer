﻿using Filestransformer.Support.CommandLine;

namespace Filestransformer.Settings
{
    public class ConsoleSettingsProvider : ISettingsProvider
    {
        private readonly Setting settings;

        public ConsoleSettingsProvider(Options op)
        {
            int fileGroups = op?.FileGroups ?? 0;
            int maximumParallelFileTransformations = op?.MaximumParallelFileTransformations ?? 0;
            string inputDirectoryPath = op?.InputDirectoryPath ?? "";
            string outputDirectoryPath = op?.OutputDirectoryPath ?? "";
            int fileChunkSizeToReadInBytes = op?.FileChunkSizeToReadInBytes ?? 0;

            settings = new Setting(fileGroups, maximumParallelFileTransformations, 
                inputDirectoryPath, outputDirectoryPath, fileChunkSizeToReadInBytes);
        }

        public Setting GetSettings()
        {
            return settings;
        }
    }
}
