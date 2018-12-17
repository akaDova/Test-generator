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
            set;
        }
        public int WriteCount
        {
            get;
            set;
        }
        public int ProcessCount
        {
            get;
            set;
        }

        public Limits() { }

        public Limits(int readCount = 1, int processCount = 1, int writeCount = 1)
        {
            ReadCount = readCount;
            ProcessCount = processCount;
            WriteCount = writeCount;
        }
    }
}
