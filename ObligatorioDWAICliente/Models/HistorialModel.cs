namespace ObligatorioDWAICliente.Models
{
    public class HistorialModel
    {
        public int? Anio { get; set; }
        public int? Mes { get; set; }
        public List<HistorialLecturaModel> Lecturas { get; set; } = new();
    }
}
