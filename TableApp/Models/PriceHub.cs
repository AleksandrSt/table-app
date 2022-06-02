using System.ComponentModel.DataAnnotations;

namespace TableApp.Models
{
    public class PriceHub
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
