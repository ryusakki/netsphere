using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Netsphere.Client.Models
{
    public class PackageModel : FileModel
    {
        public PackageModel(string name, byte[] data)
        {
            Name = name;

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
