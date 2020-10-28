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

        public ArchiveModel Archive { get; set; }
    }
}
