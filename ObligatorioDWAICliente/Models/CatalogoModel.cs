namespace ObligatorioDWAICliente.Models
{
    public class CatalogoModel
    {
        public int? CategoriaId { get; set; }
        public List<CategoriaModel> Categorias { get; set; } = new();
        public List<HistoriaModel> Historias { get; set; } = new();
    }
}
