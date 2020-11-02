using System;
using System.IO;
using System.Security.Cryptography;
using System.Linq;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Timers;
using System.Net.Http;
using System.Net;
using System.Text.Json;
using System.Text;
using Netsphere.Client.Models;
using Netsphere.Shared.Models;
using Netsphere.Client.Extensions;
using System.Threading.Tasks;
using Netsphere.Client.Services;
using Netsphere.Client.Logic;

namespace Client
{
    class PeerService
    {
        private HttpClient client;
        private Timer timer;
        private string server;
        private List<PeerModel> peers;
        private readonly object locker = new object();

        public PeerService(string server)
        {
            this.server = server;
            client = new HttpClient();
            timer = new Timer(1000);
            Connection = new Connection();
            IsSynchronized = false;

            timer.AutoReset = true;
            timer.Elapsed += (source, args) =>
            {
                lock(locker)
                {
                    if(IsSynchronized)
                    {
                        var apiEndPoint = string.Concat(server, "/Ping");
                        var peerConnection = Connection.Listener.LocalEndpoint.ToString();

                        var request = client.PostAsJsonAsync(apiEndPoint, peerConnection).GetAwaiter().GetResult();
                        IsSynchronized = request.IsSuccessStatusCode;
                    }
                    else
                    {
                        SynchronizeRequest().Wait();
                        if(!IsSynchronized)
                        {
                            timer.Enabled = false;
                        }
                    }
                }
            };
        }

        public bool IsSynchronized { get; private set; }
        public Connection Connection { get; private set; }
        public async Task<bool> SynchronizeRequest()
        {
            try
            {
                var apiEndPoint = string.Concat(server, "/Register");
                var request = await client.PostAsJsonAsync(apiEndPoint, new PeerModel
                {
                    IPEndPoint = Connection.Listener.LocalEndpoint.ToString(),
                    Timestamp = DateTime.Now,
                    Files = Repository.Files.Cast<FileModel>().ToList()
                });


                lock (locker)
                {
                    IsSynchronized = request.IsSuccessStatusCode;
                    if (IsSynchronized && !timer.Enabled)
                    {
                        timer.Enabled = true;
                    }
                }

                return IsSynchronized;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public async Task<List<FileModel>> CatalogRequest()
        {
            var apiEndPoint = string.Concat(server, "/Catalog");
            var peerConnection = Connection.Listener.LocalEndpoint.ToString();
            var request = await client.PostAsJsonAsync(apiEndPoint, peerConnection);
            var json = await request.Content.ReadAsStringAsync();

            peers = JsonSerializer.Deserialize<List<PeerModel>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return peers.SelectMany(p => p.Files).ToList();
        }

        public async Task<ArchiveModel> FileRequest(FileModel file)
        {
            if(peers is null || peers.IsEmpty())
            {
                throw new NullReferenceException("Peer list is undefined.");
            }

            try
            {
                var peer = peers.Where(p => p.Files.Contains(file)).FirstOrDefault();
                var requestedFile = await Connection.Request(peer, file);
                await Repository.Save(requestedFile);

                return requestedFile;
            }
            catch(Exception e)
            {
                throw e;
            }

        }
    }
}
