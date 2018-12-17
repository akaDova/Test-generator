using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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



            var testGenerator = new TestGenerator(args.Take(args.Length - 4), args[args.Length - 3 - 1], limits);

            try
            {
                testGenerator.Generate().Wait();
            }
            catch (AggregateException ex)
            {
                Console.WriteLine("Уважаемый, ошибка");
                
            }
            

        }
    }
}
