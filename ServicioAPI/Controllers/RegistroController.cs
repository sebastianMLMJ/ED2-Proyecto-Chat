using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServicioAPI.Models;
using MongoDB.Driver;
using Libreria_ED2;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace ServicioAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistroController : ControllerBase
    {
        private IWebHostEnvironment rootpath;

        public RegistroController(IWebHostEnvironment appEnvironment)
        {
            rootpath = appEnvironment;
        }


        [Route("login")]
        [HttpPost]
        public bool LogearUsuario(Usuario usuarioLogear)
        {

            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("CHAT");
            var usuariodb = database.GetCollection<Usuario>("Usuario");

            List<Usuario> resultado = usuariodb.Find(usuarioBuscar => usuarioBuscar.usuario==usuarioLogear.usuario).ToList();

            if (resultado.Count>0)
            {
                
                if (resultado[0].contrasenia==usuarioLogear.contrasenia)
                {
                    return true;
                }
            }
            return false;

        }

        [Route("registrar")]
        [HttpPost]
        public bool RegistrarUsuario(Usuario nuevoUsuario) 
        {

            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("CHAT");
            var usuariodb = database.GetCollection<Usuario>("Usuario");
            
            List<Usuario> resultado = usuariodb.Find(usuarioBuscar => usuarioBuscar.usuario == nuevoUsuario.usuario || usuarioBuscar.correo==nuevoUsuario.correo).ToList();

            if (resultado.Count>0)
            {
                return false;
            }
            else
            {
                usuariodb.InsertOne(nuevoUsuario);
                return true;
            }



        }
    }
}
