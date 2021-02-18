using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

            // Files without correct name format will be ignored
            return Directory.GetFiles(inputFolder).Where(s => Regex.IsMatch(Path.GetFileName(s), "in[0-9]+.txt")).ToArray();
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