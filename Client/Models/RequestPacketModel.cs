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

        public FileModel File { get; set; }
    }
}
