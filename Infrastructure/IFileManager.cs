using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure
{
    public interface IFileManager {

        Task<string> CreateOutputFileAsync(string name, string content);

        string[] ListInputFiles(string inputFolder);

        Task<string[]> ReadDeliveryFileAsync(string inputFilePath);
    }
}