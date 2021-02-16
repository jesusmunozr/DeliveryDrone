using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Infrastructure;

namespace Infrastructure
{
    public class FileManager : IFileManager
    {
        public async Task<string> CreateOutputFileAsync(string name, string content)
        {
            if (!Directory.Exists("Output"))
                Directory.CreateDirectory("Output");

            await File.WriteAllTextAsync($"Output/{name}", content);

            return $"{Environment.CurrentDirectory}/{name}";
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