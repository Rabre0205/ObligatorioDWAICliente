using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using ObligatorioDWAICliente.Auxiliar;
using ObligatorioDWAICliente.Enum;
using ObligatorioDWAICliente.Models;

namespace ObligatorioDWAICliente.Controllers
{
    public class LecturaController : Controller
    {
        private readonly string baseUrl = "http://localhost:5234";
        //private readonly string baseUrl = "http://www.Aventura363420.somee.com";
        private string? Token => HttpContext.Session.GetString("token");

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (string.IsNullOrEmpty(Token))
            {
                context.Result = RedirectToAction("Login", "Auth");
            }
        }

        [HttpGet]
        public IActionResult Index(int? categoriaId)
        {
            CatalogoModel model = new CatalogoModel { CategoriaId = categoriaId };

            using HttpResponseMessage categorias = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/api/categorias",
                VerbosHttp.GET,
                token: Token
            );

            if (categorias.IsSuccessStatusCode)
            {
                string objetoJson = ClienteHttpAuxiliar.ObtenerBody(categorias);
                List<CategoriaModel>? categoriasRecibidas = JsonConvert.DeserializeObject<List<CategoriaModel>>(objetoJson);
                if (categoriasRecibidas != null)
                {
                    model.Categorias = categoriasRecibidas;
                }
            }
            else
            {
                ViewBag.Error = "No se pudieron cargar las categorías.";
            }

            string url = baseUrl + "/api/historias/publicadas";
            if (categoriaId.HasValue)
            {
                url += "/" + categoriaId.Value;
            }

            using HttpResponseMessage historias = ClienteHttpAuxiliar.EnviarSolicitud(
                url,
                VerbosHttp.GET,
                token: Token
            );

            if (historias.IsSuccessStatusCode)
            {
                string objetoJson = ClienteHttpAuxiliar.ObtenerBody(historias);
                List<HistoriaModel>? historiasRecibidas = JsonConvert.DeserializeObject<List<HistoriaModel>>(objetoJson);
                if (historiasRecibidas != null)
                {
                    model.Historias = historiasRecibidas;
                }
                if (model.Historias.Count == 0)
                {
                    ViewBag.Mensaje = "No hay historias publicadas para la categoría seleccionada.";
                }
            }
            else if (historias.StatusCode == HttpStatusCode.NotFound)
            {
                ViewBag.Mensaje = "No hay historias publicadas para la categoría seleccionada.";
            }
            else
            {
                ViewBag.Error = "No se pudieron consultar las historias publicadas.";
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Iniciar(int historiaId)
        {
            using HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/api/lecturas",
                VerbosHttp.POST,
                new { HistoriaId = historiaId },
                Token
            );

            if (respuesta.IsSuccessStatusCode)
            {
                string objetoJson = ClienteHttpAuxiliar.ObtenerBody(respuesta);
                EstadoLecturaModel? lectura = JsonConvert.DeserializeObject<EstadoLecturaModel>(objetoJson);
                if (lectura != null)
                {
                    return RedirectToAction(nameof(Estado), new { id = lectura.Id });
                }
            }

            TempData["Error"] = "No se pudo iniciar la lectura.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Estado(int id)
        {
            using HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/api/lecturas/" + id,
                VerbosHttp.GET,
                token: Token
            );

            if (respuesta.IsSuccessStatusCode)
            {
                string objetoJson = ClienteHttpAuxiliar.ObtenerBody(respuesta);
                EstadoLecturaModel? lectura = JsonConvert.DeserializeObject<EstadoLecturaModel>(objetoJson);
                if (lectura != null)
                {
                    return View(lectura);
                }
            }

            if (respuesta.StatusCode == HttpStatusCode.NotFound)
            {
                TempData["Error"] = "No se encontró la lectura.";
            }
            else
            {
                TempData["Error"] = "No se pudo consultar la lectura.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Avanzar(int id, int opcionId)
        {
            using HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/api/lecturas/" + id + "/elecciones",
                VerbosHttp.POST,
                new { OpcionId = opcionId },
                Token
            );

            if (!respuesta.IsSuccessStatusCode)
            {
                TempData["Error"] = "No se pudo avanzar con la opción seleccionada.";
            }

            return RedirectToAction(nameof(Estado), new { id });
        }

        [HttpGet]
        public IActionResult Historial(int? anio, int? mes)
        {
            HistorialModel model = new HistorialModel
            {
                Anio = anio,
                Mes = mes
            };

            if (!anio.HasValue && !mes.HasValue && ModelState.IsValid)
            {
                return View(model);
            }

            if (!ModelState.IsValid || !anio.HasValue || anio.Value < 1 || anio.Value > 9999
                || !mes.HasValue || mes.Value < 1 || mes.Value > 12)
            {
                ViewBag.Error = "Debe indicar un año válido y un mes entre 1 y 12.";
                return View(model);
            }

            using HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "/api/lecturas/historial/" + anio + "/" + mes,
                VerbosHttp.GET,
                token: Token
            );

            if (respuesta.IsSuccessStatusCode)
            {
                string objetoJson = ClienteHttpAuxiliar.ObtenerBody(respuesta);
                List<HistorialLecturaModel>? lecturasRecibidas = JsonConvert.DeserializeObject<List<HistorialLecturaModel>>(objetoJson);
                if (lecturasRecibidas != null)
                {
                    model.Lecturas = lecturasRecibidas;
                }
                if (model.Lecturas.Count == 0)
                {
                    ViewBag.Mensaje = "No hay lecturas para el mes y año indicados.";
                }
            }
            else if (respuesta.StatusCode == HttpStatusCode.NotFound)
            {
                ViewBag.Mensaje = "No hay lecturas para el mes y año indicados.";
            }
            else
            {
                ViewBag.Error = "No se pudo consultar el historial.";
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Estilo(string estilo, string? volverA)
        {
            if (estilo == "claro" || estilo == "oscuro")
            {
                HttpContext.Session.SetString("estiloLectura", estilo);
            }

            if (volverA != null && Url.IsLocalUrl(volverA))
            {
                return LocalRedirect(volverA);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
