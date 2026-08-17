using System;
using Microsoft.Owin.Hosting;

namespace MeuProxySsl
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = "http://localhost:5010";
            Console.WriteLine($"Starting OWIN host at {url}...");
            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine("OWIN host running. Press 'Q' to stop.");
                while (true)
                {
                    var key = Console.ReadLine();
                    if (key?.ToUpper() == "Q") break;
                }
            }
            Console.WriteLine("OWIN host stopped.");
        }
    }
}
