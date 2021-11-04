using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServicioAPI.Models;
using MongoDB.Driver;


namespace ServicioAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        [Route("enviarsolicitud")]
        public bool EnviarSolicitud(SolicitudContacto nuevaSolicitud)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("CHAT");
            var usuariodb = database.GetCollection<Usuario>("Usuario");
           
            List<Usuario> resultado = usuariodb.Find(usuarioBuscar => usuarioBuscar.usuario == nuevaSolicitud.receptor).ToList();

            var solicitud = database.GetCollection<SolicitudContacto>("Solicitudes");
            if (resultado.Count>0)
            {
                solicitud.InsertOne(nuevaSolicitud);
                return true;
            }
            else
            {
                return false;
            }

           
        }
        [Route("versolicitudes")]
        [HttpPost]
        public List<SolicitudContacto> DevolverSolicitudes([FromBody]string usuario)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("CHAT");
            var usuariodb = database.GetCollection<SolicitudContacto>("Solicitudes");
            List<SolicitudContacto> resultado = usuariodb.Find(Solicitudes => Solicitudes.receptor == usuario).ToList();
            return resultado;
        }

        [Route("aceptarsolicitud")]
        [HttpPost]
        public void aceptarsolicitud(Contacto insertar)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("CHAT");
            var usuariodb = database.GetCollection<Contacto>("Contactos");
            usuariodb.InsertOne(insertar);
            Contacto reverso = new Contacto();
            reverso.miusuario = insertar.micontacto;
            reverso.micontacto = insertar.miusuario;
            usuariodb.InsertOne(reverso);
        }
    }
}
