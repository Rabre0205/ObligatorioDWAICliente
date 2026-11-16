using System.Net;
using Microsoft.AspNetCore.Mvc;
using ObligatorioDWAICliente.Auxiliar;
using ObligatorioDWAICliente.Enum;
using ObligatorioDWAICliente.Models;
using Newtonsoft.Json;

namespace ObligatorioDWAICliente.Controllers
{
    public class AuthController : Controller
    {
        
        private string baseUrl = "http://localhost:5234/api/tokens";
        //private string baseUrl = "http://www.Aventura363420.somee.com/api/tokens";


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginModel model)
        {
            HttpContext.Session.Clear();

            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl,
                VerbosHttp.POST,
                model
                );

            if (respuesta.IsSuccessStatusCode)
            {
                string objetoJson = ClienteHttpAuxiliar.ObtenerBody(respuesta);
                TokenModel tokenModel = JsonConvert.DeserializeObject<TokenModel>(objetoJson);
                HttpContext.Session.SetString("token", tokenModel.Token);
                return RedirectToAction("Index", "Home");
            }

            if(respuesta.StatusCode == HttpStatusCode.Unauthorized)
            {
                ViewBag.Error = ClienteHttpAuxiliar.ObtenerBody(respuesta);
                return View(model);
            }
            return View();            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
