using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementation.Exceptions
{
    public class PValidationException : Exception
    {
        public string Property { get; protected set; }
        public PValidationException(string message, string prop) : base(message)
        {
            Property = prop;
        }
    }
}