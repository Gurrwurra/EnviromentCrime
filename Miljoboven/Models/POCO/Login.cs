using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EnviromentCrime.Models
{
  public class Login
  {
    [Required(ErrorMessage = "Vänligen skriv in rätt användarnamn")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Vänligen skriv in rätt lösenord")]
    [UIHint("password")]
    public string Password { get; set; }
    public string ReturnUrl { get; set; }
  }
}
