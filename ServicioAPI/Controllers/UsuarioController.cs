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
        public void aceptarsolicitud(SolicitudContacto solicitud)
        {
            Contacto insertar = new Contacto();
            insertar.miusuario = solicitud.receptor;
            insertar.micontacto = solicitud.emisor; 
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("CHAT");
            var eliminarsolicitud = database.GetCollection<SolicitudContacto>("Solicitudes");

            eliminarsolicitud.DeleteOne(eliminar => eliminar.emisor == solicitud.emisor);

            if (solicitud.status==2)
            {
                var usuariodb = database.GetCollection<Contacto>("Contactos");
                usuariodb.InsertOne(insertar);
                Contacto reverso = new Contacto();
                reverso.miusuario = insertar.micontacto;
                reverso.micontacto = insertar.miusuario;
                usuariodb.InsertOne(reverso);
            }
            
        }

        [Route("vercontactos")]
        [HttpPost]
        public List<Contacto> vercontactos([FromBody]string idusuario)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("CHAT");
            var usuariodb = database.GetCollection<Contacto>("Contactos");
            List<Contacto> resultado = usuariodb.Find(contacto =>contacto.miusuario  == idusuario).ToList();
            return resultado;
        }
    }
}
