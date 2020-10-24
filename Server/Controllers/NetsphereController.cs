using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Timers;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

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
            Timer.Elapsed += OnRemoveInvalidPeers;
            Timer.AutoReset = true;
            Timer.Enabled = true;
        }

        private static void OnRemoveInvalidPeers(object source, ElapsedEventArgs e)
        {
            lock(Locker)
            {
                var currentTime = DateTime.Now;                
                Peers.RemoveWhere(p => Math.Abs((currentTime - p.Timestamp).Seconds) > 5);
            }
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

        [HttpPost("Register/{peer}")]
        public ActionResult Register([DisallowNull] Peer peer)
        {
            lock(Locker)
            {
                return Peers.Add(peer) ? Ok() : BadRequest() as ActionResult;
            }
        }

        [HttpGet("QueryCatalog")]
        public ActionResult<List<string>> QueryCatalog()
        {
            lock(Locker)
            {
                return Ok(Peers.SelectMany(p => p.AvailableContent));
            }
        }
    }
}
