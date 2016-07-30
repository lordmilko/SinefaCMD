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
