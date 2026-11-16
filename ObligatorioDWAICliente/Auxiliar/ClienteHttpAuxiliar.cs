using System.Net;
using ObligatorioDWAICliente.Enum;

namespace ObligatorioDWAICliente.Auxiliar
{
    public class ClienteHttpAuxiliar
    {
        public static HttpResponseMessage EnviarSolicitud
        (
            string url,
            VerbosHttp verbo,
            object? obj = null,
            string? token = null
        )
        {
            using HttpClient cliente = new HttpClient();
            cliente.Timeout = TimeSpan.FromSeconds(15);
            Task<HttpResponseMessage> tarea;

            if (!string.IsNullOrEmpty(token)) 
            {
                cliente.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            }

            switch (verbo)
            {
                case VerbosHttp.GET:
                    tarea = cliente.GetAsync(url);
                    break;
                case VerbosHttp.POST:
                    tarea = cliente.PostAsJsonAsync(url, obj);
                    break;
                case VerbosHttp.DELETE:
                    tarea = cliente.DeleteAsync(url);
                    break;
                case VerbosHttp.PUT:
                    tarea = cliente.PutAsJsonAsync(url, obj);
                    break;
                default:
                    throw new Exception("Http Method incorrecto.");
            }

            try
            {
                return tarea.GetAwaiter().GetResult();
            }
            catch (HttpRequestException)
            {
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
            }
            catch (TaskCanceledException)
            {
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
            }
        }

        public static string ObtenerBody(HttpResponseMessage respuesta)
        {
            HttpContent body = respuesta.Content;
            return body.ReadAsStringAsync().GetAwaiter().GetResult();
        }
    }
}
