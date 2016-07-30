using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SinefaCMD.Output.Type
{
    class PrtgOutput : StandardOutput, IOutput
    {
        public void Top(List<Host> hosts)
        {
            throw new OutputNotImplementedException();
        }

        public void TopUp(List<Host> hosts)
        {
            throw new OutputNotImplementedException();
        }

        public void TopDown(List<Host> hosts)
        {
            var sorted = hosts.OrderByDescending(host => host.Down);

            var first = sorted.First();

            var xml = new XElement("prtg",
                new XElement("text", GetMessage(first)),
                new XElement("result",
                    new XElement("channel", "Bandwidth"),
                    new XElement("value", first.Down),
                    new XElement("float", 1),
                    new XElement("Unit", "SpeedNet"),
                    new XElement("VolumeSize", "MegaBit")
                )
            );

            var str = xml.ToString();

            Console.WriteLine(str);
        }

        public void ListProbes(List<Probe> probes)
        {
            throw new OutputNotImplementedException();
        }

        public void ListHosts(List<string> hosts)
        {
            throw new OutputNotImplementedException();
        }

        public void Monitor(IEnumerable<Host> hostData)
        {
            throw new OutputNotImplementedException();
        }

        public void Monitor(Host host)
        {
            throw new OutputNotImplementedException();
        }

        public void Error(Exception ex)
        {
            var xml = new XElement("prtg",
                new XElement("error", 1),
                new XElement("text", ex.Message)
            );

            Console.WriteLine(xml.ToString());
        }
    }
}
