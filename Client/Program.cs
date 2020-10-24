using System;
using System.IO;
using System.Security.Cryptography;
using System.Linq;
using System.Collections.Generic;
using System.Timers;
using System.Net.Http;

namespace Client
{
    class Program
    {


        static Program()
        {

        }

        public enum PacketType
        {
            FileRequest = 0x5C,
            FileResponse
        }

        public class Packet
        {
            public PacketType Type { get; set; }
        }

       

        static void Main(string[] args)
        {
            var path = string.Concat(AppContext.BaseDirectory.Split("bin").First(), "Repository");

            var directory = new DirectoryInfo(path);
            var files = new Dictionary<string, byte[]>();

            using (var hash = SHA256.Create())
            {
                foreach(var file in directory.GetFiles())
                {
                    using (var fStream = file.Open(FileMode.Open))
                    {
                        fStream.Position = 0;
                        var fHash = hash.ComputeHash(fStream);
                        var fHashStr = BitConverter.ToString(fHash).Replace("-", string.Empty);
                        var fBytes = new byte[file.Length];

                        fStream.Read(fBytes, 0, Convert.ToInt32(file.Length));
                        files.Add(fHashStr, fBytes);
                    }
                };
            }
        }
    }
}
