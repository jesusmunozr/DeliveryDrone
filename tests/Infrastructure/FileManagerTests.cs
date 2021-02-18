using Infrastructure;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace tests.Infrastructure
{
    public class FileManagerTests
    {
        [Fact]
        public async Task GivenFilePathShouldReturnPathsArray()
        {
            // Arrange
            string filePath = "Files/in01.txt";
            string[] expectedData = new string[] { "AAAAIAA", "DDDAIAD", "AAIADAD" };
            IFileManager fileManager = new FileManager();

            // Act
            var fileLines = await fileManager.ReadDeliveryFileAsync(filePath);

            // Assert
            Assert.Equal<int>(3, fileLines.Length);
            Assert.Collection<string>(fileLines,
                line => Assert.Equal(line, expectedData[0]),
                line => Assert.Equal(line, expectedData[1]),
                line => Assert.Equal(line, expectedData[2]));
        }

        [Fact]
        public async Task GivenWrongFilePathShouldThrowException()
        {
            // Arrange
            string wrongFilePath = "Files/wrong.txt";
            IFileManager fileManager = new FileManager();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<FileNotFoundException>(async () => await fileManager.ReadDeliveryFileAsync(wrongFilePath));
            Assert.Equal("Input file not found.", exception.Message);
        }
    }
}
