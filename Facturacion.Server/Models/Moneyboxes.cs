using System.ComponentModel.DataAnnotations;

namespace Facturacion.Server.Models
{
    public class Moneyboxes
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float value { get; set; }
    }
}
