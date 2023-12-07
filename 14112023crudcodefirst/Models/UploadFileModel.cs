using System.ComponentModel.DataAnnotations;

namespace _14112023crudcodefirst.Models
{
    public class UploadFileModel
    {
        [Required(ErrorMessage = "Please select a file.")]
        [DataType(DataType.Upload)]
        [Display(Name = "Select File")]
        public IFormFile File { get; set; }
        //    [Required(ErrorMessage = "Please provide a file path.")]
        //    [Display(Name = "File Path")]
        //    public string FilePath { get; set; }
        //}
    }
}
