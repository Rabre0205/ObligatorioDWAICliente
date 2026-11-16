namespace ObligatorioDWAICliente.Models
{
    public class HistorialLecturaModel
    {
        public string TituloHistoria { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public bool Completada { get; set; }
        public string? TituloFinalAlcanzado { get; set; }
    }
}
