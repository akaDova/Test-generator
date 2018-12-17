using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestGeneratorLib;

namespace ConsoleApp
{
    class Program
    {
        const int MIN_ARGS_COUNT = 5;

        static void Main(string[] args)
        {
            if (args.Length < MIN_ARGS_COUNT)
            {
                Console.WriteLine("Уважаемый, неправильно");
                return;
            }

            Limits limits = new Limits
            {
                ReadCount = int.Parse(args[args.Length - 2 - 1]),
                ProcessCount = int.Parse(args[args.Length - 1 - 1]),
                WriteCount = int.Parse(args[args.Length - 1])
            };
            string fileContent = "";
            if (File.Exists(args[0]))
            {
                using (StreamReader fileStream = File.OpenText(args[0]))
                {
                    fileContent = fileStream.ReadToEnd();
                }
            }
                
            


            var testGenerator = new TestGenerator(args.Take(args.Length - 4), args[args.Length - 3 - 1], limits);

            //var res = testGenerator.__Generate(fileContent);

            try
            {
                var task = testGenerator.Generate();
                task.Wait();
                Thread.Sleep(2000);
                Console.WriteLine("збс");
                
            }
            catch (AggregateException ex)
            {
                Console.WriteLine("Уважаемый, ошибка");
                
            }
            

        }
    }
}
