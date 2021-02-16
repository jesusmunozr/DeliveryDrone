using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Infrastructure;

namespace Infrastructure
{
    public class FileManager : IFileManager
    {
        public Task<string> CreateOutputFileAsync(string path)
        {
            throw new NotImplementedException();
        }

        public string[] ListInputFiles(string inputFolder)
        {
            if (!Directory.Exists(inputFolder))
            {
                throw new DirectoryNotFoundException("Input directory not found.");
            }

            return Directory.GetFiles(inputFolder);
        }

        public async Task<string[]> ReadDeliveryFileAsync(string inputFilePath)
        {
            if (!File.Exists(inputFilePath))
            {
                throw new FileNotFoundException("Input file not found.");
            }

            return await File.ReadAllLinesAsync(inputFilePath);
        }
    }
}