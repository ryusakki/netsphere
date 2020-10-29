using Netsphere.Client.Enums;
using Netsphere.Shared.Models;
using System;

namespace Netsphere.Client.Models
{
    [Serializable]
    public class RequestPacketModel : PacketModel
    {
        public RequestPacketModel()
        {
            Type = PacketType.FileRequest;
        }

        /// <summary>
        /// Informações mínimas sobre o arquivo que deseja-se obter
        /// </summary>
        public FileModel File { get; set; }
    }
}
