namespace ObligatorioDWAICliente.Models
{
    public class EstadoLecturaModel
    {
        public int Id { get; set; }
        public string TituloHistoria { get; set; } = string.Empty;
        public CapituloLecturaModel CapituloActual { get; set; } = new();
        public string? TituloFinalAlcanzado { get; set; }
    }
}
