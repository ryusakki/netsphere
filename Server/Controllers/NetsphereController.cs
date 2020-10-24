using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Timers;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using Netsphere.Shared.Models;

namespace Netsphere.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NetsphereController : ControllerBase
    {
        private static object Locker = new object();
        private static HashSet<PeerModel> Peers = new HashSet<PeerModel>();
        private static Timer Timer = new Timer(5000);

        static NetsphereController()
        {
            Timer.Elapsed += (source, e) =>
            {
                lock (Locker)
                {
                    var currentTime = DateTime.Now;
                    Peers.RemoveWhere(p => Math.Abs((currentTime - p.Timestamp).Seconds) > 5);
                }
            };

            Timer.AutoReset = true;
            Timer.Enabled = true;
        }

        [HttpPost("Ping/{ipEndPoint}")]
        public ActionResult<string> Ping([DisallowNull] string ipEndPoint)
        {
            lock(Locker)
            {
                var peer = Peers.Where(p => p.IPEndPoint == ipEndPoint).FirstOrDefault();
                if (peer != null)
                {
                    peer.Timestamp = DateTime.Now;
                }
                return Ok("Pong");
            }
        }

        [HttpPost("Register")]
        public ActionResult Register([DisallowNull] PeerModel peer)
        {
            lock(Locker)
            {
                Console.WriteLine("Register {0}", peer.IPEndPoint);
                return Peers.Add(peer) ? Ok() : BadRequest() as ActionResult;
            }
        }

        [HttpGet("Catalog")]
        public ActionResult<HashSet<PeerModel>> Catalog()
        {
            lock(Locker)
            {
                return Ok(Peers);
            }
        }
    }
}
