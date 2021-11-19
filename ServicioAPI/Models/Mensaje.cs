using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicioAPI.Models
{
    public class Mensaje
    {
        public string emisor { get; set; }
        public string receptor { get; set; }
        public string cadena { get; set; }
        public IFormFile archivo { get; set; }
    }
}
