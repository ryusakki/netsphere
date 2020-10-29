using Client;
using Netsphere.Client.Enums;
using Netsphere.Client.Extensions;
using Netsphere.Client.Logic;
using Netsphere.Client.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Netsphere.Client
{
    public class Program
    {
        private static UIContext Interface;
        private static PeerService Service;

        static async Task Init()
        {
            var registeredTask = Interface.Loading("Synchronizing your repository to NetsphereServer", Service.RegisterRequest());
            var registered = await registeredTask;

            if (registered)
            {
                var catalog = await Interface.Loading("Loading catalog of files", Service.CatalogRequest());

                if(!catalog.IsEmpty())
                {

                    int selected = 0;
                    while(selected != -1)
                    {
                        selected = Interface.Menu(true, catalog.ToArray());
                        var file = catalog.ElementAt(selected);
                        var package = await Interface.Loading("Kindly asking for a file to another peer", Service.FileRequest(file));
                    }

                }
                else
                {
                    Console.WriteLine("Sem itens");
                }
            }
            else
            {
                Interface.DisplayMessage("Registration failed. NetsphereServer may be offline.", ConsoleColor.Red);
            }
        }

        static async Task Main(string[] args)
        {
            var config = File.ReadAllText(string.Concat(Directory.GetParent(Repository.Path), "/config.json"));
            var jConfig = JsonSerializer.Deserialize<Dictionary<string, string>>(config);

            Interface = new UIContext("Netsphere");
            Service = new PeerService(jConfig["server"]);
            await Init();
            Console.ReadKey();
        }
    }
}
