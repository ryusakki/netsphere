using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Netsphere.Shared.Models
{
    [Serializable]
    public class FileModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Hash { get; set; }
    }
}
