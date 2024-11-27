using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUFramework
{
    public class CPUDevException :Exception
    {
        public CPUDevException(string? message, Exception? innerecxeption) : base(message, innerecxeption)
        {

        }
    }
}
