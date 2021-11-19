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
using MongoDB.Driver;

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
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("CHAT");
            var chatdb= database.GetCollection<Chat>("Chats");
                        
            CifradorSDES cifrador = new CifradorSDES(1024);
            CompresorLZW compresor = new CompresorLZW(1024);

            if (!Directory.Exists(rootpath.WebRootPath+"\\Chats\\"))
            {
                Directory.CreateDirectory(rootpath.WebRootPath + "\\Chats\\");
            }

            //Si la conversacion esta vacia
            if (System.IO.File.Exists(rootpath.WebRootPath + "\\Chats\\" + insertar.emisor + insertar.receptor + ".lzw") == false || System.IO.File.Exists(rootpath.WebRootPath + "\\Chats\\" + insertar.receptor + insertar.emisor + ".lzw") == false)
            {
                Chat nuevoMensaje = new Chat();
                nuevoMensaje.emisor = insertar.emisor;
                nuevoMensaje.receptor = insertar.receptor;
                nuevoMensaje.ruta = rootpath.WebRootPath;
                await chatdb.InsertOneAsync(nuevoMensaje);
               
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

                System.IO.File.Delete(rootpath.WebRootPath + "\\Chats\\" + insertar.emisor + insertar.receptor + ".sdes");
                System.IO.File.Delete(rootpath.WebRootPath + "\\Chats\\" + insertar.receptor + insertar.emisor + ".sdes");
                System.IO.File.Delete(rootpath.WebRootPath + "\\Chats\\" + insertar.emisor + insertar.receptor + "temp.txt");
                System.IO.File.Delete(rootpath.WebRootPath + "\\Chats\\" + insertar.receptor + insertar.emisor + "temp.txt");

            }

            if (insertar.archivo!=null)
            {
                Stream contenido = insertar.archivo.OpenReadStream();

                FileStream Transcriptor = new FileStream(rootpath.WebRootPath + "\\Archivos\\"+insertar.archivo.FileName, FileMode.Create);
                await contenido.CopyToAsync(Transcriptor);
                await Transcriptor.FlushAsync();
                Transcriptor.Close();
            }

            return Ok();
        }

        [HttpPost]
        [Route("subirarchivo")]
        public async Task<IActionResult> SubirArchivo([FromForm] IFormFile archivo,[FromForm] string emisor, [FromForm] string receptor)
        {
            CifradorSDES cifrador = new CifradorSDES(1024);
            CompresorLZW compresor = new CompresorLZW(1024);
            string[] arreglonombre = archivo.FileName.Split('.');
            string nombreArchivo = arreglonombre[0];
            Stream bytes = archivo.OpenReadStream();
            FileStream filestream = new FileStream(rootpath.WebRootPath + "\\ArchivosChats\\" + archivo.FileName, FileMode.Create);
            await bytes.CopyToAsync(filestream);
            filestream.Flush();
            filestream.Close();

            int cv = cifrador.ClaveChat(emisor);
            int cv2 = cifrador.ClaveChat(receptor);

            cifrador.Cifrar(rootpath.WebRootPath + "\\ArchivosChats\\" + archivo.FileName, rootpath.WebRootPath + "\\ArchivosChats\\", rootpath.WebRootPath + "\\Archivos\\Permutaciones.txt", emisor + receptor+nombreArchivo, cv);
            cifrador.Cifrar(rootpath.WebRootPath + "\\ArchivosChats\\" + archivo.FileName, rootpath.WebRootPath + "\\ArchivosChats\\", rootpath.WebRootPath + "\\Archivos\\Permutaciones.txt", receptor + emisor+ nombreArchivo, cv2);

            compresor.Comprimir(rootpath.WebRootPath + "\\ArchivosChats\\" + emisor + receptor + nombreArchivo + ".sdes", rootpath.WebRootPath + "\\ArchivosChats\\", emisor + receptor + nombreArchivo);
            compresor.Comprimir(rootpath.WebRootPath + "\\ArchivosChats\\" + receptor + emisor + nombreArchivo + ".sdes", rootpath.WebRootPath + "\\ArchivosChats\\", receptor + emisor + nombreArchivo);

            System.IO.File.Delete(rootpath.WebRootPath + "\\ArchivosChats\\" + emisor + receptor + ".sdes");
            System.IO.File.Delete(rootpath.WebRootPath + "\\ArchivosChats\\" + receptor + emisor + ".sdes");
            System.IO.File.Delete(rootpath.WebRootPath + "\\ArchivosChats\\" + archivo.FileName);

            return Ok();
        }

        [HttpPost]
        [Route("descargar")]
        public async Task<IActionResult> devolverArchivo(Mensaje buscarArchivo)
        {
            string emisor = buscarArchivo.emisor;
            string receptor = buscarArchivo.receptor;
            string [] arreglonombre = buscarArchivo.cadena.Split('.');
            string archivo = arreglonombre[0];
            string extension = arreglonombre[1];
            CifradorSDES cifrador = new CifradorSDES(1024);
            CompresorLZW compresor = new CompresorLZW(1024);
            int cv = cifrador.ClaveChat(emisor);
            int cv2 = cifrador.ClaveChat(receptor);
            compresor.Descomprimir(rootpath.WebRootPath + "\\ArchivosChats\\" + emisor + receptor + archivo +".lzw", rootpath.WebRootPath + "\\ArchivosChats\\");
            cifrador.Decifrar(rootpath.WebRootPath + "\\ArchivosChats\\" +emisor+receptor+archivo+".sdes" , rootpath.WebRootPath + "\\ArchivosChats\\", rootpath.WebRootPath + "\\Archivos\\Permutaciones.txt", cv);
            var bytes = await System.IO.File.ReadAllBytesAsync(rootpath.WebRootPath + "\\ArchivosChats\\"+archivo+"."+ extension);
            var objetoStream = new MemoryStream(bytes);

            return File(objetoStream, "application/octet-stream",archivo+"."+ extension);
        }

        [HttpPost]
        [Route("Conversacion")]
        public async Task<IActionResult> DevolverConversacion([FromBody]Contacto conversacion)
        {
            CifradorSDES cifrador = new CifradorSDES(1024);
            CompresorLZW compresor = new CompresorLZW(1024);

            if (System.IO.File.Exists(rootpath.WebRootPath + "\\Chats\\DocumentoVacio.txt") == false)
            {
                var archivo = System.IO.File.Create(rootpath.WebRootPath + "\\Chats\\DocumentoVacio.txt");
                archivo.Close();
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(rootpath.WebRootPath + "\\Chats\\DocumentoVacio.txt");
            var objetoStream = new MemoryStream(bytes);
            
            
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
