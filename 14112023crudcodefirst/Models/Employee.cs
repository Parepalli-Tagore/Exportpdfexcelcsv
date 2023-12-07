using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace _14112023crudcodefirst.Models
{
    public class Employee
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [Range(18, 99)]
        public int Age { get; set; }

        [Required]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone]
        [RegularExpression(@"^[0-9]+")]
        public string PhoneNumber { get; set; }

        [Required]
        public string Department { get; set; }

        [Display(Name = "Date of Joining")]
        [DataType(DataType.Date)]
        public DateTime? DateOfJoining { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
    }
}
