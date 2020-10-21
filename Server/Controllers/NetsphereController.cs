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
        private static HashSet<Peer> Peers = new HashSet<Peer>();
        private static Timer Timer = new Timer(4000);

        static Netsphere()
        {
            Timer.Elapsed += OnRemoveInvalidPeers;
            Timer.AutoReset = true;
            Timer.Enabled = true;
        }

        private static void OnRemoveInvalidPeers(object source, ElapsedEventArgs e)
        {
            lock(Peers)
            {
                var currentTime = DateTime.Now;                
                Peers.RemoveWhere(p => (p.Timestamp- currentTime).TotalSeconds > 5);
            }
        }

        [HttpPost("Ping")]
        public ActionResult<string> Ping() => Ok("Pong");

        [HttpPost("Register")]
        public ActionResult Register(Peer peer)
        {
            lock(Peers)
            {
                return Peers.Add(peer) ? Ok() : Forbid() as ActionResult;
            }
        }

        [HttpGet("QueryCatalog")]
        public ActionResult<List<string>> QueryCatalog()
        {
            lock(Peers)
            {
                return Ok(Peers.SelectMany(p => p.AvailableContent));
            }
        }
    }
}
