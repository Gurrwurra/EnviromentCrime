using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnviromentCrime.Models;
using Microsoft.AspNetCore.Mvc;
using EnviromentCrime.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace EnviromentCrime.Components
{
  public class StartEmployee:ViewComponent
  {

    private IErrandRepository repository;
    private IHttpContextAccessor accessor;

    public StartEmployee(IErrandRepository repo, IHttpContextAccessor acc)
    {
      repository = repo;
      accessor = acc;
    }

    /// <summary>
    /// Denna ViewComponent skapar Viewbag som innehåller ärende listan beroende på vilken roll användaren har.
    /// </summary>
    /// <returns></returns>
    public  IViewComponentResult Invoke()
    {
      //Hämtar ut Employee objektet för användaren så jag kan jämföra vilken roll användaren har.
      var user = repository.Employees.Where(em => em.EmployeeId == accessor.HttpContext.User.Identity.Name).First();

      //I dem här if satserna kollar jag om användarens roll är lika med t ex Coordinator för att starta rätt metod i IErrandRepository
      if (user.RoleTitle == "Coordinator")
      {
        //Skapar ViewBags för controllern och action så att användaren kan klicka på ett ärende
        ViewBag.Controller = "Coordinator";
        ViewBag.Action = "CrimeCoordinator";
        ViewBag.listErrands = repository.ErrandsName();
      }
      else if (user.RoleTitle == "Manager")
      {
        ViewBag.Controller = "Manager";
        ViewBag.Action = "CrimeManager";
        ViewBag.listErrands = repository.ErrandsManager(accessor.HttpContext.User.Identity.Name);
        
      }
      else if (user.RoleTitle == "Investigator")
      {
        ViewBag.Controller = "Investigator";
        ViewBag.Action = "CrimeInvestigator";
        ViewBag.listErrands = repository.ErrandsEmplo(accessor.HttpContext.User.Identity.Name);
        
      }

      return View();
    }
  }
}
