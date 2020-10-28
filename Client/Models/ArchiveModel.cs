using System;
using System.IO;
using System.Security.Cryptography;
using Netsphere.Shared.Models;

namespace Netsphere.Client.Models
{
    [Serializable]
    public class ArchiveModel : FileModel
    {
        public ArchiveModel(string name, byte[] data)
        {
            Name = Path.GetFileNameWithoutExtension(name);

            using (var hash = SHA256.Create())
            {
                var fHash = hash.ComputeHash(data);
                Hash = BitConverter.ToString(fHash).Replace("-", string.Empty);
            }

            Data = data;
        }

        public byte[] Data { get; set; }
    }
}
