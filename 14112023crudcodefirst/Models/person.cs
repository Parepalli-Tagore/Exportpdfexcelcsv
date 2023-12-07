using System.ComponentModel.DataAnnotations;

namespace _14112023crudcodefirst.Models
{
    public class person
    {
        [Key]
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string role { get; set; }
    }
}
