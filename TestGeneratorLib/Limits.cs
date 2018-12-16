using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGeneratorLib
{
    public class Limits
    {
        public int ReadCount
        {
            get;
        }
        public int WriteCount
        {
            get;
        }
        public int ProcessCount
        {
            get;
        }

        public Limits() { }

        public Limits(int readCount, int processCount, int writeCount)
        {
            ReadCount = readCount;
            ProcessCount = processCount;
            WriteCount = writeCount;
        }
    }
}
