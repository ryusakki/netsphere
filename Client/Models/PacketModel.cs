using Netsphere.Client.Enums;
using System;

namespace Netsphere.Client.Models
{
    [Serializable]
    public class PacketModel
    {
        /// <summary>
        /// Tipo do pacote
        /// </summary>
        public PacketType Type { get; set; }
    }
}
