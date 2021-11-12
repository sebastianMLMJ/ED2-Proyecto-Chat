using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using ServicioAPI.Models;

namespace ServicioAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private IWebHostEnvironment rootpath;

        public ChatController(IWebHostEnvironment appEnvironment)
        {
            rootpath = appEnvironment;
        }
        

        [HttpPost]
        [Route("Escribir")]
        public async Task<IActionResult> EscribirChat(Mensaje insertar)
        {
            if (!Directory.Exists(rootpath.WebRootPath+"\\Chats\\"))
            {
                Directory.CreateDirectory(rootpath.WebRootPath + "\\Chats\\");
            }
            if (Directory.Exists(rootpath.WebRootPath+ "\\Chats\\"+ insertar.emisor+insertar.receptor+".sdes")==false)
            {
                using (StreamWriter sw = new StreamWriter(rootpath.WebRootPath + "\\Chats\\" + insertar.emisor + insertar.receptor + ".sdes",true))
                {
                    string resultado = insertar.emisor + ":" + insertar.cadena;
                    await sw.WriteLineAsync(resultado);
                    sw.Close();
                } 
            }
            return Ok();
        }


        [HttpPost]
        [Route("Conversacion")]
        public async Task<IActionResult> DevolverConversacion([FromBody]Contacto conversacion)
        {
            var bytes = await System.IO.File.ReadAllBytesAsync(rootpath.WebRootPath + "\\Chats\\"+conversacion.miusuario+conversacion.micontacto+".sdes");
            var objetoStream = new MemoryStream(bytes);
            return File(objetoStream, "application/octet-stream", conversacion.miusuario+conversacion.micontacto + ".sdes");
        }
       
      
    }
}
