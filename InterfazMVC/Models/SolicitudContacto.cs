using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterfazMVC.Models
{
    public class SolicitudContacto
    {
        public string Emisor { get; set; }
        public string Receptor { get; set; }
        public int Status { get; set; }
    }
}
