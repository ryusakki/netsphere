using System;
using System.IO;
using System.Security.Cryptography;
using Netsphere.Shared.Models;

namespace Netsphere.Client.Models
{
    [Serializable]
    public class ArchiveModel : FileModel
    {
        /// <summary>
        /// Inicializa as propriedades da super classe
        /// </summary>
        /// <param name="name">Nome do arquivo(extensão inclusa)</param>
        /// <param name="data">ByteArray do arquivo</param>
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

        /// <summary>
        /// ByteArray do arquivo
        /// </summary>
        public byte[] Data { get; set; }
    }
}
