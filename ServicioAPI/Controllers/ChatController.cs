using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using ServicioAPI.Models;
using Libreria_ED2;

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
            CifradorSDES cifrador = new CifradorSDES(1024);
            CompresorLZW compresor = new CompresorLZW(1024);

            if (!Directory.Exists(rootpath.WebRootPath+"\\Chats\\"))
            {
                Directory.CreateDirectory(rootpath.WebRootPath + "\\Chats\\");
            }

            //Si la conversacion esta vacia
            if (System.IO.File.Exists(rootpath.WebRootPath + "\\Chats\\" + insertar.emisor + insertar.receptor + ".lzw") == false || System.IO.File.Exists(rootpath.WebRootPath + "\\Chats\\" + insertar.receptor + insertar.emisor + ".lzw") == false)
            {
                if (System.IO.File.Exists(rootpath.WebRootPath + "\\Chats\\" + insertar.emisor + insertar.receptor + ".lzw") == false)
                {
                    using (StreamWriter sw = new StreamWriter(rootpath.WebRootPath + "\\Chats\\" + insertar.emisor + insertar.receptor + "temp.txt", true))
                    {
                        string resultado = insertar.emisor + ":" + insertar.cadena;
                        await sw.WriteLineAsync(resultado);
                        await sw.FlushAsync();
                        sw.Close();
                    }
                }
                if (System.IO.File.Exists(rootpath.WebRootPath + "\\Chats\\" + insertar.receptor + insertar.emisor + ".lzw") == false)
                {
                    using (StreamWriter sw = new StreamWriter(rootpath.WebRootPath + "\\Chats\\" + insertar.receptor + insertar.emisor + "temp.txt", true))
                    {
                        string resultado = insertar.emisor + ":" + insertar.cadena;
                        await sw.WriteLineAsync(resultado);
                        await sw.FlushAsync();
                        sw.Close();
                    }
                }

                
                int cv = cifrador.ClaveChat(insertar.emisor);
                int cv2 = cifrador.ClaveChat(insertar.receptor);
                cifrador.Cifrar(rootpath.WebRootPath + "\\Chats\\" + insertar.emisor + insertar.receptor + "temp.txt", rootpath.WebRootPath + "\\Chats\\",rootpath.WebRootPath+"\\Archivos\\Permutaciones.txt",insertar.emisor+insertar.receptor,cv);
                cifrador.Cifrar(rootpath.WebRootPath + "\\Chats\\" + insertar.receptor + insertar.emisor + "temp.txt", rootpath.WebRootPath + "\\Chats\\", rootpath.WebRootPath + "\\Archivos\\Permutaciones.txt", insertar.receptor + insertar.emisor, cv2);

                compresor.Comprimir(rootpath.WebRootPath + "\\Chats\\" + insertar.emisor + insertar.receptor + ".sdes", rootpath.WebRootPath + "\\Chats\\",insertar.emisor+insertar.receptor);
                compresor.Comprimir(rootpath.WebRootPath + "\\Chats\\" + insertar.receptor + insertar.emisor + ".sdes", rootpath.WebRootPath + "\\Chats\\", insertar.receptor + insertar.emisor);

                System.IO.File.Delete(rootpath.WebRootPath + "\\Chats\\" + insertar.emisor + insertar.receptor + ".sdes") ;
                System.IO.File.Delete(rootpath.WebRootPath + "\\Chats\\" + insertar.receptor + insertar.emisor+".sdes");
                System.IO.File.Delete(rootpath.WebRootPath + "\\Chats\\" + insertar.emisor + insertar.receptor + "temp.txt");
                System.IO.File.Delete(rootpath.WebRootPath + "\\Chats\\" + insertar.receptor + insertar.emisor + "temp.txt");
            }
            else
            {
                compresor.Descomprimir(rootpath.WebRootPath + "\\Chats\\" + insertar.emisor + insertar.receptor + ".lzw", rootpath.WebRootPath + "\\Chats\\");
                compresor.Descomprimir(rootpath.WebRootPath + "\\Chats\\" + insertar.receptor + insertar.emisor + ".lzw", rootpath.WebRootPath + "\\Chats\\");

                int cv = cifrador.ClaveChat(insertar.emisor);
                int cv2 = cifrador.ClaveChat(insertar.receptor);

                cifrador.Decifrar(rootpath.WebRootPath + "\\Chats\\" + insertar.emisor + insertar.receptor + ".sdes", rootpath.WebRootPath + "\\Chats\\", rootpath.WebRootPath + "\\Archivos\\Permutaciones.txt", cv);
                cifrador.Decifrar(rootpath.WebRootPath + "\\Chats\\" + insertar.receptor + insertar.emisor + ".sdes", rootpath.WebRootPath + "\\Chats\\", rootpath.WebRootPath + "\\Archivos\\Permutaciones.txt", cv2);

                using (StreamWriter sw = new StreamWriter(rootpath.WebRootPath + "\\Chats\\" + insertar.emisor + insertar.receptor + "temp.txt", true))
                {
                    string resultado = insertar.emisor + ":" + insertar.cadena;
                    await sw.WriteLineAsync(resultado);
                    await sw.FlushAsync();
                    sw.Close();
                }
                using (StreamWriter sw = new StreamWriter(rootpath.WebRootPath + "\\Chats\\" + insertar.receptor + insertar.emisor + "temp.txt", true))
                {
                    string resultado = insertar.emisor + ":" + insertar.cadena;
                    await sw.WriteLineAsync(resultado);
                    await sw.FlushAsync();
                    sw.Close();
                }

                cifrador.Cifrar(rootpath.WebRootPath + "\\Chats\\" + insertar.emisor + insertar.receptor + "temp.txt", rootpath.WebRootPath + "\\Chats\\", rootpath.WebRootPath + "\\Archivos\\Permutaciones.txt", insertar.emisor + insertar.receptor, cv);
                cifrador.Cifrar(rootpath.WebRootPath + "\\Chats\\" + insertar.receptor + insertar.emisor + "temp.txt", rootpath.WebRootPath + "\\Chats\\", rootpath.WebRootPath + "\\Archivos\\Permutaciones.txt", insertar.receptor + insertar.emisor, cv2);

                compresor.Comprimir(rootpath.WebRootPath + "\\Chats\\" + insertar.emisor + insertar.receptor + ".sdes", rootpath.WebRootPath + "\\Chats\\", insertar.emisor + insertar.receptor);
                compresor.Comprimir(rootpath.WebRootPath + "\\Chats\\" + insertar.receptor + insertar.emisor + ".sdes", rootpath.WebRootPath + "\\Chats\\", insertar.receptor + insertar.emisor);

                //System.IO.File.Delete(rootpath.WebRootPath + "\\Chats\\" + insertar.emisor + insertar.receptor + ".sdes");
                //System.IO.File.Delete(rootpath.WebRootPath + "\\Chats\\" + insertar.receptor + insertar.emisor + ".sdes");
                //System.IO.File.Delete(rootpath.WebRootPath + "\\Chats\\" + insertar.emisor + insertar.receptor + "temp.txt");
                //System.IO.File.Delete(rootpath.WebRootPath + "\\Chats\\" + insertar.receptor + insertar.emisor + "temp.txt");

            }
            
            

            return Ok();
        }


        [HttpPost]
        [Route("Conversacion")]
        public async Task<IActionResult> DevolverConversacion([FromBody]Contacto conversacion)
        {
            CifradorSDES cifrador = new CifradorSDES(1024);
            CompresorLZW compresor = new CompresorLZW(1024);

            var bytes = await System.IO.File.ReadAllBytesAsync(rootpath.WebRootPath + "\\Chats\\DocumentoVacio.txt");
            var objetoStream = new MemoryStream(bytes);

            if (System.IO.File.Exists(rootpath.WebRootPath + "\\Chats\\DocumentoVacio.txt") == false)
            {
                var archivo = System.IO.File.Create(rootpath.WebRootPath + "\\Chats\\DocumentoVacio.txt");
                archivo.Close();
            }

            if (System.IO.File.Exists(rootpath.WebRootPath + "\\Chats\\" + conversacion.miusuario + conversacion.micontacto + ".lzw") == true)
            {
                compresor.Descomprimir(rootpath.WebRootPath + "\\Chats\\" + conversacion.miusuario + conversacion.micontacto + ".lzw", rootpath.WebRootPath + "\\Chats\\");
                
                int cv = cifrador.ClaveChat(conversacion.miusuario);
                int cv2 = cifrador.ClaveChat(conversacion.micontacto);

                cifrador.Decifrar(rootpath.WebRootPath + "\\Chats\\" + conversacion.miusuario + conversacion.micontacto + ".sdes", rootpath.WebRootPath + "\\Chats\\", rootpath.WebRootPath + "\\Archivos\\Permutaciones.txt", cv);
                
                bytes = await System.IO.File.ReadAllBytesAsync(rootpath.WebRootPath + "\\Chats\\" + conversacion.miusuario + conversacion.micontacto + "temp" + ".txt");
                objetoStream = new MemoryStream(bytes);
                
                System.IO.File.Delete(rootpath.WebRootPath + "\\Chats\\" + conversacion.miusuario + conversacion.micontacto + ".sdes");
                System.IO.File.Delete(rootpath.WebRootPath + "\\Chats\\" + conversacion.miusuario + conversacion.micontacto + "temp.txt");
                
                return File(objetoStream, "application/octet-stream", conversacion.miusuario + conversacion.micontacto + "temp" + ".txt");
            }



            return File(objetoStream, "application/octet-stream", conversacion.miusuario+conversacion.micontacto + ".sdes");
        }
       
      
    }
}
