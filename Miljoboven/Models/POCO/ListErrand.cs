using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnviromentCrime.Models
{
  public class ListErrand
  {
    /// <summary>
    /// Properties för ListErrand så att i vynerna kan det skrivas ut hela namn på t ex status
    /// </summary>
    public DateTime DateOfObservation { get; set; }
    public int ErrandID { get; set; }
    public string RefNumber { get; set; }
    public string TypeOfCrime { get; set; }
    public string StatusName { get; set; }
    public string DepartmentName { get; set; }
    public string EmployeeName { get; set; }
  }
}
