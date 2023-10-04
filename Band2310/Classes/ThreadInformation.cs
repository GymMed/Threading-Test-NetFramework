using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Band2310.Classes
{
    public class ThreadInformation
    {
        private int threadID;
        private string generatedLine;

        public ThreadInformation(int threadID, string generatedLine)
        {
            this.threadID = threadID;
            this.generatedLine = generatedLine;
        }

        public int ThreadID 
        { 
            get => threadID;
            set => threadID = value; 
        }

        public string GeneratedLine 
        {
            get => generatedLine;
            set => generatedLine = value; 
        }
    }
}
