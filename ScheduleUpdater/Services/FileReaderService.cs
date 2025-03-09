using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleUpdater.Services
{
    internal class FileReaderService : IFileReaderService
    {
        public FileReaderService() { }

        public async Task FileReaderManager(string file)
        {
            GetFileHeaders(file);
            await GetFileContent(file);
        }
        private string[] GetFileHeaders(string fileName) 
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(fs))
            {
                string line;
                if((line = sr.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                    line = sr.ReadLine()!;
                    return line.Split(",");
                }

                return new string[0];
                
            }

        }

        private async Task<List<string>> GetFileContent(string fileName)
        {
            var fileObjects= new List<string>();
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
            {
                await sr.ReadLineAsync();
                string line;
                while ((line = await sr.ReadLineAsync()) != null)
                {
                    Console.WriteLine(line);
                    fileObjects.Add(line);
                }


            }
            return fileObjects;

        }

       
    }
}
