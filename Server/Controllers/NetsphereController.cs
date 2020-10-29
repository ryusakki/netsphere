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
        private static Timer Timer = new Timer(4500);

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

        /// <summary>
        /// Ping em um peer específico. Quando chamado, este endpoint atualiza o DateTime do peer cujo ip foi especificado.
        /// </summary>
        /// <param name="ipEndPoint">String com o ip:port do peer que deseja-se realizar o ping.</param>
        /// <returns>Tamanho da lista de peers</returns>
        [HttpPost("Ping")]
        public ActionResult<int> Ping([FromBody] string ipEndPoint)
        {
            lock (Locker)
            {
                var peer = Peers.Where(p => p.IPEndPoint == ipEndPoint).FirstOrDefault();
                if (peer != null)
                {
                    peer.Timestamp = DateTime.Now;
                }
                return Ok(Peers.Where(p => p != peer).Count());
            }
        }

        /// <summary>
        /// Registra um peer em uma coleção de peers.
        /// </summary>
        /// <param name="peer">Peer que deseja-se registrar.</param>
        /// <returns>Status 'Ok' caso o registro tenha sido efetuado com sucesso, 'BadRequest' caso contrário.</returns>
        [HttpPost("Register")]
        public ActionResult Register([DisallowNull] PeerModel peer)
        {
            lock(Locker)
            {
                Console.WriteLine("Register {0}", peer.IPEndPoint);
                return Peers.Add(peer) ? Ok() : BadRequest() as ActionResult;
            }
        }

        /// <summary>
        /// Retorna a lista de peer e conteúdos que cada um disponibiliza.
        /// </summary>
        /// <param name="ipEndPoint">O ip do peer que faz a requisição para este endpoint deve ser especificado, para que o mesmo não seja retornado junto a lista.</param>
        /// <returns>Coleção de peers</returns>
        [HttpPost("Catalog")]
        public ActionResult<HashSet<PeerModel>> Catalog([FromBody] string ipEndPoint)
        {
            lock(Locker)
            {
                return Ok(Peers.Where(p => p.IPEndPoint != ipEndPoint));
            }
        }
    }
}
