using Netsphere.Client.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netsphere.Client.Logic
{
    public class Repository
    {
        static Repository()
        {
            Path = string.Concat(AppContext.BaseDirectory.Split("bin").First(), "Repository");
            var directory = new DirectoryInfo(Path);

            Files = directory.GetFiles().ToList().ConvertAll(f =>
            {
                var fBytes = new byte[f.Length];
                using (var fStream = f.Open(FileMode.Open))
                {
                    fStream.Position = 0;
                    fStream.Read(fBytes);
                }
                return new ArchiveModel(string.Concat(f.Name, f.Extension), fBytes);
            });
        }

        public static string Path { get; private set; }
        public static List<ArchiveModel> Files { get; private set; }

        public static async Task Save(ArchiveModel archive)
        {
            Files.Add(archive);
            var downloadsFolder = string.Format("{0}\\Downloads\\{1}", Path, archive.Name);
            await File.WriteAllBytesAsync(downloadsFolder, archive.Data);
        }
    }
}
