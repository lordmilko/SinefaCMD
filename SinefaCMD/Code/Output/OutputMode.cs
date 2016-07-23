using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SinefaCMD.Output.Type;

namespace SinefaCMD.Output
{
    enum OutputMode
    {
        [OutputType(typeof(ConsoleOutput))]
        Console,

        [OutputType(typeof(PrtgOutput))]
        Prtg
    }
}
