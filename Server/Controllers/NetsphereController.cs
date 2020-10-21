using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Timers;
using System.Linq;

namespace Netsphere.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Netsphere : ControllerBase
    {
        private static object Locker = new object();
        private static HashSet<Peer> Peers = new HashSet<Peer>();
        private static Timer Timer = new Timer(5000);

        static Netsphere()
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
        public ActionResult<string> Ping(string ipEndPoint)
        {
            lock(Locker)
            {
                var peer = Peers.Where(p => p.IPEndPoint == ipEndPoint).FirstOrDefault();
                if(peer != null)
                {
                    peer.Timestamp = DateTime.Now;
                }

                return Ok("Pong");
            }
        }

        [HttpPost("Register/{peer}")]
        public ActionResult Register(Peer peer)
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
