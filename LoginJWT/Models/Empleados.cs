using System.ComponentModel.DataAnnotations;

namespace LoginJWT.Models
{
    public class Empleados
    {
        [Key]
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}