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
        private readonly HttpClient client = new HttpClient();
        private readonly Timer timer = new Timer(1000);
        private readonly object locker = new object();
        private readonly string server = "http://localhost:5001/Netsphere";
        private List<PeerModel> peers;

        public PeerService()
        {
            Connection = new Connection();
            IsRegistered = false;

            timer.AutoReset = true;
            timer.Elapsed += (source, args) =>
            {
                lock(locker)
                {
                    if(IsRegistered)
                    {
                        var apiEndPoint = string.Concat(server, "/Ping");
                        var peerConnection = Connection.Listener.LocalEndpoint.ToString();

                        var request = client.PostAsJsonAsync(apiEndPoint, peerConnection).GetAwaiter().GetResult();
                        IsRegistered = request.IsSuccessStatusCode;
                    }
                    else
                    {
                        RegisterRequest().Wait();
                        if(!IsRegistered)
                        {
                            timer.Enabled = false;
                        }
                    }
                }
            };
        }

        public bool IsRegistered { get; private set; }
        public Connection Connection { get; private set; }

        public async Task<bool> RegisterRequest()
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

                IsRegistered = request.IsSuccessStatusCode;

                lock (locker)
                {
                    if (IsRegistered && !timer.Enabled)
                    {
                        timer.Enabled = true;
                    }
                }

                return IsRegistered;
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
                return await Connection.Request(peer, file);
            }
            catch(Exception e)
            {
                throw e;
            }

        }
    }
}
