using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SinefaCMD
{
    [Serializable]
    public class ProbeNotFoundException : Exception
    {
        public ProbeNotFoundException()
        {
        }

        public ProbeNotFoundException(string message) : base(message)
        {
        }

        public ProbeNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ProbeNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
