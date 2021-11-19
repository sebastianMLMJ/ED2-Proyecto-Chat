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
using System.Text;
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
            
            FileStream Transcriptor = new FileStream(rootpath.WebRootPath+"\\Archivos\\Ejemplo.txt",FileMode.Create);
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
        public async Task<IActionResult> Chat(string _mensaje)
        {
            if (_mensaje !=null)
            {
                Mensaje nuevoMensaje = new Mensaje();
                nuevoMensaje.cadena = _mensaje;
                nuevoMensaje.emisor = HttpContext.Session.GetString("Usuario");
                nuevoMensaje.receptor = HttpContext.Session.GetString("Contacto");
                var response = await client.PostAsJsonAsync("http://localhost:34094/api/Chat/Escribir", nuevoMensaje);
                string resultado = await response.Content.ReadAsStringAsync();
                
            }
            return RedirectToAction("Chat");
        }

        //[HttpPost]
        //public async Task<IActionResult> ChatArchivo([FromForm(Name="archivo")]IFormFile archivo)
        //{
        //    if (archivo!=null)
        //    {
        //        string resultado="";
        //        StringBuilder concatenador = new StringBuilder();
        //        using (StreamReader sr = new StreamReader(archivo.OpenReadStream()))
        //        {
        //            while (sr.Peek() >= 0)
        //            {
        //                concatenador.Append(await sr.ReadLineAsync());
        //            }
        //        }
        //        resultado += concatenador.ToString();

        //        Mensaje nuevoMensaje = new Mensaje();
        //        nuevoMensaje.cadena = resultado;
        //        nuevoMensaje.emisor = HttpContext.Session.GetString("Usuario");
        //        nuevoMensaje.receptor = HttpContext.Session.GetString("Contacto");
        //        var response = await client.PostAsJsonAsync("http://localhost:34094/api/Chat/Escribir", nuevoMensaje);
        //        string resultadopost = await response.Content.ReadAsStringAsync();
        //    }

        //    return RedirectToAction("Chat");
        //}

        [HttpPost]
        public async Task<IActionResult> ChatArchivo([FromForm(Name = "archivo")] IFormFile archivo)
        {
            if (archivo != null)
            {
                Mensaje nuevoMensaje = new Mensaje();

                nuevoMensaje.emisor = HttpContext.Session.GetString("Usuario");
                nuevoMensaje.receptor = HttpContext.Session.GetString("Contacto");
                string resultado = "url:"+archivo.FileName;
                nuevoMensaje.cadena = resultado;
                nuevoMensaje.emisor = HttpContext.Session.GetString("Usuario");
                nuevoMensaje.receptor = HttpContext.Session.GetString("Contacto");
                var response = await client.PostAsJsonAsync("http://localhost:34094/api/Chat/Escribir", nuevoMensaje);
                string resultadopost = await response.Content.ReadAsStringAsync();
                using var content = new MultipartFormDataContent(nuevoMensaje.emisor+nuevoMensaje.receptor);
                FileStream bytes = new FileStream(rootpath.WebRootPath + "\\Archivos\\temp.txt", FileMode.Create);
                await archivo.CopyToAsync(bytes);
                await bytes.FlushAsync();
                bytes.Close();
                var ruta = rootpath.WebRootPath + "\\Archivos\\temp.txt";
                using Stream filestream = System.IO.File.OpenRead(ruta);
                content.Add(new StreamContent(filestream),"archivo",archivo.FileName);
                content.Add(new StringContent(nuevoMensaje.emisor),"emisor");
                content.Add(new StringContent(nuevoMensaje.receptor), "receptor");
                response = await client.PostAsync("http://localhost:34094/api/Chat/subirarchivo", content);
                resultadopost = await response.Content.ReadAsStringAsync();
                filestream.Close();
                


            }

            return RedirectToAction("Chat");
        }

        [HttpGet]
        public async Task<IActionResult> devolverArchivo(string id)
        {
            Mensaje buscarArchivo = new Mensaje();


            buscarArchivo.emisor = HttpContext.Session.GetString("Usuario");
            buscarArchivo.receptor = HttpContext.Session.GetString("Contacto");
            string[] arreglo = id.Split(':');
            buscarArchivo.cadena = arreglo[2];
            var response = await client.PostAsJsonAsync("http://localhost:34094/api/Chat/descargar", buscarArchivo);
            Stream contenido = await response.Content.ReadAsStreamAsync();

            FileStream Transcriptor = new FileStream(rootpath.WebRootPath + "\\Archivos\\Ejemplo.txt", FileMode.Create);
            await contenido.CopyToAsync(Transcriptor);
            await Transcriptor.FlushAsync();
            Transcriptor.Close();

            var bytes = await System.IO.File.ReadAllBytesAsync(rootpath.WebRootPath + "\\Archivos\\Ejemplo.txt");
            var objetoStream = new MemoryStream(bytes);

            return File(objetoStream, "application/octet-stream", buscarArchivo.cadena);
        }
    }
}
