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

namespace InterfazMVC.Controllers
{
    public class HomeController : Controller
    {

        private static readonly HttpClient client = new HttpClient();
        [HttpGet]
        public IActionResult Index(string id) {
            ViewBag.error = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(Usuario nuevoUsuario)
        {
            var response = await client.PostAsJsonAsync("http://localhost:34094/api/Registro/login", nuevoUsuario);
            var responseString = await response.Content.ReadAsStringAsync();

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
            var response = await client.PostAsJsonAsync("http://localhost:34094/api/Registro/registrar", nuevoUsuario);
            var responseString = await response.Content.ReadAsStringAsync();

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
