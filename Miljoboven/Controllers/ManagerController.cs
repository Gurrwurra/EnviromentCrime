using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EnviromentCrime.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EnviromentCrime.Controllers
{
  [Authorize(Roles = "Manager")]
  public class ManagerController : Controller
  {
    private IErrandRepository repository;
    private IHttpContextAccessor accessor;

    public ManagerController(IErrandRepository repo, IHttpContextAccessor acc)
    {
      repository = repo;
      accessor = acc;
    }

    /// <summary>
    /// Metode retunerar vyn StartManager med data från repository samt viewBag Employee där en lista med namnen med anställda som tillhör chefens avdelning
    /// </summary>
    /// <returns></returns>
    public ViewResult StartManager()
    {
      //Startar metoden EmployManager där jag skickar med chefens EmployeeId så metoden kan returnera en lista med rätt anställda
      ViewBag.Employee = repository.EmployManager(accessor.HttpContext.User.Identity.Name);
      return View(repository);
    }

    /// <summary>
    /// Denna metod tar in string id vilket är det ÄrendeID som använderen väljer att klicka på. Som sedan retunerar Vyn med "ViewBag.Eid"(IDet) som i sin tur kallar på Component ShowOneErrand med det IDet, samt en viewBag med anställda som tillhör chefens avdelning
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public ViewResult CrimeManager(int id)
    {
      ViewBag.Employee = repository.EmployManager(accessor.HttpContext.User.Identity.Name);
     
      ViewBag.Eid = id;
      //TempData för hjälpmetoden UpdateEmp
      TempData["ID"] = id;

      return View();
    }

    public ActionResult UpdateEmp(string EmployeeId, bool noAction, string reason)
    {
      //konventerar om tempdata ID till en integer
      int ErrId = int.Parse(TempData["ID"].ToString());

      //startar metoden UpdateEmployee 
      repository.UpdateEmployee(EmployeeId, ErrId, noAction, reason);
     
      //returnerar vyn CrimeManager med ärende idet
      return RedirectToAction("CrimeManager", new { id = ErrId });
    }
  }
}
