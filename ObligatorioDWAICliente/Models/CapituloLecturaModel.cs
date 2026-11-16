namespace ObligatorioDWAICliente.Models
{
    public class CapituloLecturaModel
    {
        public string Titulo { get; set; } = string.Empty;
        public string Contenido { get; set; } = string.Empty;
        public bool EsFinal { get; set; }
        public List<OpcionCapituloModel> Opciones { get; set; } = new();
    }
}
