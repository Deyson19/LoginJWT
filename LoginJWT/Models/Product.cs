using System.ComponentModel.DataAnnotations;

namespace LoginJWT.Models
{
    public class Product
    {
        [Key]
        public Guid? Id { get; set; }
        public string Names { get; set; }
        public string Description { get; set; }
    }
}