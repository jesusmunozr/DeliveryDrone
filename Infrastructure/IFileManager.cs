using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure
{
    public interface IFileManager {
        Task<string[]> ReadDeliveryFileAsync(string inputDirectory);

        Task<string> CreateOutputFileAsync(string path);
    }
}