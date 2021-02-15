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

        public async Task<string[]> ReadDeliveryFileAsync(string inputFilePath)
        {
            if(!File.Exists(inputFilePath)){
                throw new FileNotFoundException("Input file not found.");
            }

            return await File.ReadAllLinesAsync(inputFilePath);
        }
    }
}