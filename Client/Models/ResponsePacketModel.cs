using Netsphere.Client.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netsphere.Client.Models
{
    [Serializable]
    public class ResponsePacketModel : PacketModel
    {
        public ResponsePacketModel()
        {
            Type = PacketType.FileResponse;
        }

        /// <summary>
        /// Arquivo solicitado por outro peer
        /// </summary>
        public ArchiveModel Archive { get; set; }
    }
}
