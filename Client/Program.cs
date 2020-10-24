using Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Netsphere.Client
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var pService = new PeerService();
            var catalog = await pService.CatalogRequest();
            // _ = pService.RegisterRequest();

            foreach(var c in catalog)
            {
                Console.WriteLine(c.Name + " - " + c.Hash);
            }
        }
    }
}
