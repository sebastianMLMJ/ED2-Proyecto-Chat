using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InterfazMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Net.Http;

namespace InterfazMVC.Controllers
{
    public class ChatController : Controller
    {
        private static readonly HttpClient client = new HttpClient();
        private IWebHostEnvironment rootpath;

        public ChatController(IWebHostEnvironment appEnvironment)
        {
            rootpath = appEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Chat(string miusuario, string micontacto, string mensaje)
        {

            if (micontacto != null && micontacto != null)
            {
                HttpContext.Session.SetString("Usuario", miusuario);
                HttpContext.Session.SetString("Contacto", micontacto);
            }
            else
            {
                miusuario = HttpContext.Session.GetString("Usuario");
                micontacto = HttpContext.Session.GetString("Contacto");
            }

            Contacto Conversacion = new Contacto();
            Conversacion.micontacto = micontacto;
            Conversacion.miusuario = miusuario;
            var response = await client.PostAsJsonAsync("http://localhost:34094/api/Chat/Conversacion", Conversacion);
            Stream contenido = await response.Content.ReadAsStreamAsync();
            
            FileStream Transcriptor = new FileStream(rootpath.WebRootPath+"\\Archivos\\Ejemplo.txt",FileMode.Open);
            await contenido.CopyToAsync(Transcriptor);
            await Transcriptor.FlushAsync();
            Transcriptor.Close();

            List<Mensaje> mensajes = new List<Mensaje>();
            StreamReader sr = new StreamReader(new FileStream(rootpath.WebRootPath + "\\Archivos\\Ejemplo.txt", FileMode.Open,FileAccess.ReadWrite));
            
                string cadenamensaje = "";
                while (cadenamensaje != null)
                {
                    Mensaje nuevoMensaje = new Mensaje();
                    nuevoMensaje.cadena = cadenamensaje;
                    cadenamensaje = await sr.ReadLineAsync();
                    mensajes.Add(nuevoMensaje);
                }
            sr.Close();
            return View(mensajes);
        }

        [HttpPost]
        public async Task<IActionResult> Chat(string _mensaje) {
            Mensaje nuevoMensaje = new Mensaje();
            nuevoMensaje.cadena = _mensaje;
            nuevoMensaje.emisor= HttpContext.Session.GetString("Usuario");
            nuevoMensaje.receptor = HttpContext.Session.GetString("Contacto");
            var response = await client.PostAsJsonAsync("http://localhost:34094/api/Chat/Escribir", nuevoMensaje);
            string resultado = await response.Content.ReadAsStringAsync();
            return RedirectToAction("Chat");

        }
    }
}
