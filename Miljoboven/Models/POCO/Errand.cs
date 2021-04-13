using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnviromentCrime.Models
{
  public class Errand
  {
    /*Properties för Errand. Samt validering för de input som krävs på Index-sidan, samt ReportCrime. Valideringen sker i controllerna Citizen-controller samt CoordinatorController*/
    public int ErrandID { get; set; }

    public string RefNumber { get; set; }

    [Required(ErrorMessage = "Vänligen skriv in platsen brottet skedde")]
    [Display(Name = "Var har brottet skett någonstans?")]
    public string Place { get; set; }

    [Required(ErrorMessage = "Vänligen beskriv vilken typ av brott")]
    [Display(Name = "Vilken typ av brott?")]
    public string TypeOfCrime { get; set; }

   
    [Required(ErrorMessage = "Vänligen skriv in datumet brottet skedde")]
    [Display(Name = "När skedde brottet?")]
    [UIHint("Date")]
    public DateTime DateOfObservation { get; set; }

    [Display(Name = "Beskriv din observation" + 
      "(ex.namn på misstänkt person):")] 
    public string Observation { get; set; }

    public string InvestigatorInfo { get; set; }

    public string InvestigatorAction { get; set; }

    [Required(ErrorMessage = "Vänligen skriv in ditt namn")]
    [Display(Name = "Ditt namn (för- och efternamn):")]
    public string InformerName { get; set; }

    [Required(ErrorMessage = "Vänligen skriv in ditt telefonnummer (0432-xxxxxxx")]
    [Display(Name = "Din telefon:")]
    [RegularExpression(@"^[0432]{1,4}-[0-9]{5,11}$", ErrorMessage = "Felaktigt telefonnummer det ska vara 0432-xxxxxxx)")]
    public string InformerPhone { get; set; }

    public string StatusId { get; set; }

    public string DepartmentId { get; set; }

    public string EmployeeId { get; set; }

    //Collection(lista) med samples från poco-klassen Sample för det ärendet
    public ICollection<Sample> Samples { get; set; }

    //Collection(lista) med pictures från poco-klassen Picture för det ärendet
    public ICollection<Picture> Pictures { get; set; }
     

  }
}
