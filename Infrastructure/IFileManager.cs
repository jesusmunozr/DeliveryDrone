using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure
{
    public interface IFileManager {

        Task<string> CreateOutputFileAsync(string path);

        string[] ListInputFiles(string inputFolder);

        Task<string[]> ReadDeliveryFileAsync(string inputFilePath);
    }
}