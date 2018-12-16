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

        public TestGenerator(IEnumerable<string> sourceFiles, string targetDir, Limits limits)
        {
            this.sourceFiles = sourceFiles;
            this.targetDir = targetDir;
            this.limits = limits;
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
                    readWriter.ReadFileAsync(filePath), readBlockOptions);


            var processBlock = new TransformManyBlock<Task<string>, Task<TestTemplate>>((sourceCode) =>
            {
                // TODO process
                return new IEnumerable<Task<TestTemplate>>();// (() => new TestTemplate());
            }, processBlockOptions);

            var writeBlock = new ActionBlock<Task<TestTemplate>>((testTemplate) =>
            {
                // TODO write
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
