using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EnviromentCrime.Models
{
  public class ErrandStatus
  {
    /*properties för ErrandStatus*/
    [Key]
    public string StatusId { get; set; }

    public string StatusName { get; set; }
  }
}
