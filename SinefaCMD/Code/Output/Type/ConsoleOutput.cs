using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinefaCMD.Output.Type
{
    class ConsoleOutput : StandardOutput, IOutput
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
            var top = hosts.OrderByDescending(host => host.Down).First();

            var hostname = GetHostname(top.IPAddress);
            var username = GetUsername(hostname);

            Console.WriteLine(" {0,-10}: {1}", "IP Address", top.IPAddress);
            Console.WriteLine(" {0,-10}: {1}", "Hostname", hostname);
            Console.WriteLine(" {0,-10}: {1}", "Username", username);
            Console.WriteLine(" {0,-10}: {1}mbps", "Bandwidth", top.Down);
        }

        public void ListProbes(List<Probe> probes)
        {
            var sorted = probes.OrderBy(probe => probe.Index);

            Console.WriteLine();

            Console.WriteLine(" {0,-3}{1}", "#", "Name");
            Console.WriteLine(" {0,-3}{1}", "-", "----");

            foreach (var probe in sorted)
            {
                Console.WriteLine(" {0,-3:D}{1}", probe.Index, probe.Name);
            }
        }

        public void ListHosts(List<string> hosts)
        {
            Console.WriteLine();

            Console.WriteLine(" {0,-17}{1,-26}{2,-16}", "IP Address", "Hostname", "Username");
            Console.WriteLine(" {0,-17}{1,-26}{2,-16}", "----------------", "-------------------------", "---------------");

            //var ipv6 = hosts.Where(host => host.Contains(":"));

            var sortedIps = hosts
                .Where(host => !host.Contains(":"))
                .Select(Version.Parse)
                .OrderBy(arg => arg)
                .Select(arg => arg.ToString())
                //.Concat(ipv6)
                .ToList();

            foreach (var host in sortedIps)
            {
                var hostname = GetHostname(host);
                var username = GetUsername(hostname);

                Console.WriteLine(" {0,-17}{1,-16}{2,-26}", host, hostname, username);

                //when the hostname is too long that causes us to get a blank newline
            }
        }

        public void Monitor(IEnumerable<Host> hostData)
        {
            var showHeader = true;
            
            //this needs to be indented one

            foreach (var result in hostData)
            {
                if (showHeader)
                {
                    Console.WriteLine("IP Address Down Up");
                    showHeader = false;
                }

                Console.WriteLine($"{result.IPAddress} {result.Down} {result.Up}");

                
            }
            //maybe we could take an argument that specifies whether this is the first logging attempt and therefore whether to show the header?

            //OR, alternatively, we could have this method take the ienumerable<host> and do the iteration in here
            //we need to only show the header the first time somehow
        }

        public void Error(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
