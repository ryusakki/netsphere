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
            var registeredTask = Interface.Loading("Synchronizing your repository with NetsphereServer", Service.SynchronizeRequest());
            var registered = await registeredTask;

            if (registered)
            {
                var catalog = await Interface.Loading("Loading catalog of files", Service.CatalogRequest());

                while(catalog.IsEmpty())
                {
                    Interface.ShowMessage("There are no other peers connected to NetsphereServer, but your files were synchronized.", ConsoleColor.Yellow);
                    Interface.ShowMessage("Refreshing catalog in 2s...", ConsoleColor.Magenta);
                    Thread.Sleep(2000);
                    
                    catalog = await Interface.Loading("Loading catalog of files", Service.CatalogRequest());
                }

                int selected = 0;
                while (selected != -1)
                {
                    selected = Interface.Menu(true, catalog.ToArray());
                    var file = catalog.ElementAt(selected);
                    var message = string.Format("Requesting {0}", file.Name);
                    await Interface.Loading(message, Service.FileRequest(file));
                }
            }
            else
            {
                Interface.ShowMessage("Registration failed. NetsphereServer may be offline.", ConsoleColor.Red);
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
