﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicioAPI.Models
{
    public class SubirArchivo
    {
        public IFormFile File { get; set; }   
    }
}
