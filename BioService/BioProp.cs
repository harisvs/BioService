using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioService
{
    class BioProp
    {
        public class Errorlog
        {
            public string Message { get; set; }
            public int ErrorType { get; set; }
            public int ErrorCode { get; set; }
            public string MethodName { get; set; }
        }

    }
}
