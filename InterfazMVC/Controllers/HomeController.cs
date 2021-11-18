using InterfazMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Libreria_ED2;

namespace InterfazMVC.Controllers
{
    public class HomeController : Controller
    {
        private IWebHostEnvironment rootpath;

        public HomeController(IWebHostEnvironment appEnvironment)
        {
            rootpath = appEnvironment;
        }

        private static readonly HttpClient client = new HttpClient();
        [HttpGet]
        public IActionResult Index(string id) {
            ViewBag.error = id;
            return View();
        }

        //LOGIN
        [HttpPost]
        public async Task<IActionResult> Index(Usuario nuevoUsuario)
        {

            // COMIENZA PROCESO DE CIFRADO
            CifradorSDES cifrador = new CifradorSDES(1024);
            string contrasenia = nuevoUsuario.contrasenia;
            StreamWriter sw = new StreamWriter(rootpath.WebRootPath + "\\Archivos\\" + nuevoUsuario.usuario + "temp.txt", false);
            await sw.WriteLineAsync(contrasenia);
            sw.Close();

            //string[] correo = nuevoUsuario.correo.Split('@');
            int cv = cifrador.ClaveChat(nuevoUsuario.usuario);
            cifrador.Cifrar(rootpath.WebRootPath + "\\Archivos\\" + nuevoUsuario.usuario + "temp.txt", rootpath.WebRootPath + "\\Archivos\\", rootpath.WebRootPath + "\\Archivos\\Permutaciones.txt", "Registro" + nuevoUsuario.usuario, cv);

            string contraseniacif = "";
            StreamReader sr = new StreamReader(rootpath.WebRootPath + "\\Archivos\\" + "Registro" + nuevoUsuario.usuario + ".sdes");
            contraseniacif = await sr.ReadToEndAsync();
            sr.Close();
            //TERMINA PROCESO DE CIFRADO Y SE ASIGNA
            nuevoUsuario.contrasenia = contraseniacif;

            var response = await client.PostAsJsonAsync("http://localhost:34094/api/Registro/login", nuevoUsuario);
            var responseString = await response.Content.ReadAsStringAsync();
            System.IO.File.Delete(rootpath.WebRootPath + "\\Archivos\\" + nuevoUsuario.usuario + "temp.txt");
            System.IO.File.Delete(rootpath.WebRootPath + "\\Archivos\\" + "Registro" + nuevoUsuario.usuario + ".sdes");
            if (responseString == "true")
            {
                return RedirectToAction("HomeUsuario","Usuario",new {idusuario = nuevoUsuario.usuario});
            }
            else
            {
                
                return RedirectToAction("Index",new {id= "Campos Incorrectos" });
            }
        }

        [HttpGet]
        [Route("Registro")]
        public IActionResult Registro(string id)
        {
            ViewBag.error = id;
            return View();
        }

        [HttpPost]
        [Route("Registro")]
        public async Task<IActionResult> RegistrarUsuario(Usuario nuevoUsuario)
        {
            // COMIENZA PROCESO DE CIFRADO
            CifradorSDES cifrador = new CifradorSDES(1024);
            string contrasenia = nuevoUsuario.contrasenia;
            StreamWriter sw = new StreamWriter(rootpath.WebRootPath + "\\Archivos\\"+nuevoUsuario.usuario+"temp.txt", false);
            await sw.WriteLineAsync(contrasenia);
            sw.Close();

            //string[] correo = nuevoUsuario.correo.Split('@');
            int cv = cifrador.ClaveChat(nuevoUsuario.usuario);
            cifrador.Cifrar(rootpath.WebRootPath + "\\Archivos\\" + nuevoUsuario.usuario + "temp.txt", rootpath.WebRootPath + "\\Archivos\\", rootpath.WebRootPath + "\\Archivos\\Permutaciones.txt","Registro"+nuevoUsuario.usuario,cv);
            
            string contraseniacif = "";
            StreamReader sr = new StreamReader(rootpath.WebRootPath + "\\Archivos\\"+ "Registro" + nuevoUsuario.usuario+".sdes");
            contraseniacif = await sr.ReadToEndAsync();
            sr.Close();
            //TERMINA PROCESO DE CIFRADO Y SE ASIGNA
            nuevoUsuario.contrasenia = contraseniacif;

            var response = await client.PostAsJsonAsync("http://localhost:34094/api/Registro/registrar", nuevoUsuario);
            var responseString = await response.Content.ReadAsStringAsync();

            System.IO.File.Delete(rootpath.WebRootPath + "\\Archivos\\" + nuevoUsuario.usuario + "temp.txt");
            System.IO.File.Delete(rootpath.WebRootPath + "\\Archivos\\"+"Registro" + nuevoUsuario.usuario+".sdes");

            if (responseString=="true")
            {
                return RedirectToAction("Index",new {id="Tu registro fue exitoso"});
            }
            else
            {
                return RedirectToAction("Registro", new { id = "El nombre de usuario o correo ya estan registrados" });
            }
            
        }
    }
}
