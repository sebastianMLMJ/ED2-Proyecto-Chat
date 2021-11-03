using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InterfazMVC.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Newtonsoft.Json;

namespace InterfazMVC.Controllers
{
    public class UsuarioController : Controller
    {

        private static readonly HttpClient client = new HttpClient();

        [HttpGet]
        public IActionResult HomeUsuario(string idusuario, string mensaje)
        {
            ViewBag.Mensaje = mensaje;
            if (idusuario != null)
            {
                HttpContext.Session.SetString("Usuario", idusuario);
            }
            
            ViewBag.Usuario = HttpContext.Session.GetString("Usuario");

            return View();
        }

        [HttpGet]
        public IActionResult AgregarContacto(string mensaje)
        {
            ViewBag.Mensaje = mensaje;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AgregarContacto(SolicitudContacto usuarioAgregar)
        {

            usuarioAgregar.Emisor = HttpContext.Session.GetString("Usuario");
            usuarioAgregar.Status = 1;

            var response = await client.PostAsJsonAsync("http://localhost:34094/api/enviarsolicitud", usuarioAgregar);
            var responseString = await response.Content.ReadAsStringAsync();

            if (responseString == "true")
            {
                return RedirectToAction("HomeUsuario", new { mensaje = "Su solicitud fue enviada exitosamente" });
            }
            else
            {
                return RedirectToAction("AgregarContacto", new { mensaje = "el usuario no fue encontrado" });
            }

            
        }

        [HttpGet]
        public async Task<IActionResult> VerSolicitudes()
        {
            string usuario = HttpContext.Session.GetString("Usuario");
            var response = await client.PostAsJsonAsync("http://localhost:34094/api/versolicitudes",usuario);
            string resultado = await response.Content.ReadAsStringAsync();
            List<SolicitudContacto> listasolicitudes= JsonConvert.DeserializeObject<List<SolicitudContacto>>(resultado);
            //var dato = JsonSerializer.Deserialize<List<Movie>>(contenido, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            
            return View(listasolicitudes);
        }

    }
}
