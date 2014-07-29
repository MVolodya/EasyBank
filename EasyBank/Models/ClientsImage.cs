using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EasyBank.Models
{
    public class ClientsImage
    {
        [Key]
        public int ImageID { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public byte[] ImageContent { get; set; }
        public int PhotoType { get; set; }

        public int ClientId { get; set; }

        public virtual Client Client { get; set; }
    }
}