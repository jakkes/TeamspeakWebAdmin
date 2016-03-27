using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var s = Dns.GetHostAddresses("82.12.123.123");
            Console.WriteLine(s.Length);
            Console.Read();
        }
    }
}
