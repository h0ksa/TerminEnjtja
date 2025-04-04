using System.ComponentModel.DataAnnotations;

namespace TerminEnjtja.Models
{
    public class LoginViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
