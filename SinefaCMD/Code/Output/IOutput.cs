using System;
using System.Collections.Generic;

namespace SinefaCMD.Output
{
    interface IOutput
    {
        void Top(List<Host> hosts);

        void TopUp(List<Host> hosts);

        void TopDown(List<Host> hosts);

        void ListProbes(List<Probe> probes);

        void ListHosts(List<string> hosts);

        void Monitor(IEnumerable<Host> hostData);

        void Error(Exception ex);
    }
}
