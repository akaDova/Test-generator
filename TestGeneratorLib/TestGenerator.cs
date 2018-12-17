using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TestGeneratorLib
{
    public class TestGenerator
    {
        IEnumerable<string> sourceFiles;
        string targetDir;
        Limits limits;
        ReadWriter readWriter = new ReadWriter();
        CSParser parser = new CSParser();

        public TestGenerator(IEnumerable<string> sourceFiles, string targetDir, Limits limits)
        {
            this.sourceFiles = sourceFiles;
            this.targetDir = targetDir;
            this.limits = limits;
        }

        public (string, string) __Generate(string src)
        {
            var res = parser.GetTestCode(src).Single();
            return res;
        }

        public async Task Generate()
        {
            var linkOptions = new DataflowLinkOptions
            {
                PropagateCompletion = true
            };

            var readBlockOptions = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = limits.ReadCount
            };

            var processBlockOptions = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = limits.ProcessCount
            };

            var writeBlockOptions = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = limits.WriteCount
            };

            var readBlock = new TransformBlock<string, Task<string>>(filePath => 
            {
                var res = readWriter.ReadFileAsync(filePath);
                Console.WriteLine("read: збс"); 
                return res;
            }         
                    , readBlockOptions);


            var processBlock = new TransformManyBlock<Task<string>, (string, string)>(async (sourceCode) =>
            {
                // TODO process
                Console.WriteLine("process: start!");
                //awaisourceCode.Wait();
                string resText = await sourceCode ;
                Console.WriteLine(resText);
                IEnumerable<(string, string)> res = null;
                try
                {
                     res = parser.GetTestCode(resText);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("process: Ошибка!");
                }
                Console.WriteLine("process: збс!");
                return res;


            }, processBlockOptions);

            var writeBlock = new ActionBlock<(string, string)>(async (testTuple) =>
            {
                Console.WriteLine("write  start");
                string testCodeText, testFileName;
                (testFileName, testCodeText) = testTuple;
                await readWriter.WriteFileAsync(testCodeText, $@"{targetDir}/{testFileName}");
                Console.WriteLine("збс!");
            }, writeBlockOptions);

            readBlock.LinkTo(processBlock, linkOptions);
            processBlock.LinkTo(writeBlock, linkOptions);

            foreach (string sourceFile in sourceFiles)
            {
                await readBlock.SendAsync(sourceFile);
            }

            readBlock.Complete();
            await readBlock.Completion;
        }
    }
}
