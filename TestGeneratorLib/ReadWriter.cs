using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TestGeneratorLib
{
    class ReadWriter
    {
        public async Task<string> ReadFileAsync(string fileName)
        {
            string fileContent;
            using (StreamReader fileStream = new StreamReader(fileName))
            {
                fileContent = await fileStream.ReadToEndAsync();
            }

            return fileContent;
            
        }
        public async Task WriteFileAsync(string fileContent, string destinationFile)
        {
            using (StreamWriter fileStream = File.CreateText(destinationFile))
            {
                await fileStream.WriteAsync(fileContent);
            }
        }
        
    }
}
