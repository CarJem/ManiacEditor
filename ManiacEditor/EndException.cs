using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor
{
    public class ValidException : Exception
    {
        public ValidException()
        {
        }

        public ValidException(string message)
            : base(message)
        {
        }

        public ValidException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
