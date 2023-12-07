using System.ComponentModel.DataAnnotations;

namespace _14112023crudcodefirst.Models
{
    public class StudentModel
    {
        [Key]
        public int Studentid { get; set; }
        public string StudentName { get; set; }
        public string Section { get; set; }
        public int Subject1 { get; set; }
        public int Subject2 { get; set; }
        public int Total { get; set; }
    }
}

