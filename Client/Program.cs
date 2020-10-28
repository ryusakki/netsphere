using Client;
using Netsphere.Client.Enums;
using Netsphere.Client.Extensions;
using Netsphere.Client.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Netsphere.Client
{
    public class Program
    {
        private static UIContext Interface = new UIContext("Netsphere");
        private static PeerService Service = new PeerService();

        static async Task Init()
        {
            var registered = await Interface.Loading("Registering to NetsphereServer", Service.RegisterRequest());

            if(registered)
            {
                var catalog = await Interface.Loading("Loading catalog of files", Service.CatalogRequest());

                if(!catalog.IsEmpty())
                {
                    var selected =  Interface.Menu(true, catalog.ToArray());
                    var file = catalog.ElementAt(selected);

                    var package = await Interface.Loading("Kindly asking for a file to another peer", Service.FileRequest(file));
                    
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
            //int selectedClass = UIContext.Menu(true, "Warrior", "Bard", "Mage", "Archer", "Thief", "Assassin", "Cleric", "Paladin", "etc.");
            //var service = new PeerService();
            //await service.RegisterRequest();


            //service.CatalogRequest();
            //var register = await pService.RegisterRequest();

            await Init();
            Console.ReadKey();
        }
    }
}
