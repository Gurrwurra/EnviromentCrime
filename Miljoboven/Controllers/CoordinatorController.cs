using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EnviromentCrime.Models;
using EnviromentCrime.Infrastructure;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EnviromentCrime.Controllers
{
  [Authorize(Roles = "Coordinator")]
  public class CoordinatorController : Controller
  {
    private IErrandRepository repository;

    public CoordinatorController(IErrandRepository repo)
    {
      repository = repo;
    }

    /* Denna metod tar in string id vilket är det ÄrendeID som använderen väljer att klicka på. Som sedan retunerar Vyn med "ViewBag.Eid"(IDet) som i sin tur kallar på Component ShowOneErrand med det IDet */
    public ViewResult CrimeCoordinator(int id)
    {
      ViewBag.Eid = id;
      //ViewBag Department får inte det värdet "Småstads kommun"
      ViewBag.Department = repository.Departments.Where(d => d.DepartmentName != "Småstads kommun");

      //Skapar TempData av id så att hjälp-metoden UpdateDepa kan skicka med rätt ärende id
      TempData["EID"] = id;

      return View();
    }

    /// <summary>
    /// Anonyma variabeln får värdet av session som heter "NewErrandCoor"  som metoden sedan kollar om den är tom eller inte. Om den e tom retunerar det vyn utan något, men om den har innehåll så retuneras det med innehållet. Denna metod finns två exemplar av. Denna metod ska endast retunera vyn med formuläret, medans den andra reportCrime ska ta in ett Errand objekt som sedan validerare inputsen
    /// </summary>
    /// <returns></returns>
    public ViewResult ReportCrime()
    {
      //Hämtar session NewErrandCoor
      var reportErrandCoor = HttpContext.Session.GetJson<Errand>("NewErrandCoor");

      //Om den är tom så retuneras reportCrime eftersom det kan vara så att användaren inte har skrivit in ett ärende ännu eller har precis loggat in
      if (reportErrandCoor == null)
      {
        return View();
      }
      //Annars retuneras vyn med session ReportErrandCoor
      else
      {
        return View(reportErrandCoor);
      }

    }

    /// <summary>
    /// Metode retunerar vyn StartManager med data från repository
    /// </summary>
    /// <returns></returns>
    public ViewResult StartCoordinator()
    {
      return View(repository);
    }

    /// <summary>
    /// Tar bort värdena från session och visar tack vyn
    /// </summary>
    /// <returns></returns>
    public ViewResult Thanks()
    {
      //Errand objektet får värdet av sessionen "NewErrandCoor"
      Errand addErrand = HttpContext.Session.GetJson<Errand>("NewErrandCoor");
      //Skickar addErrand till metoden SaveErrand som sparar ner objektet till databasen
      repository.SaveErrand(addErrand);
      //Tar bort sessionen
      HttpContext.Session.Remove("NewErrandCoor");
      //retunerar vyn Thanks med addErrand så att rätt RefNumber kan skrivas ut
      return View(addErrand);
    }

    public ViewResult Validate()
    {
      return View();
    }

    /// <summary>
    /// Denna metod retunerar sidan validate om det användaren har skrivit in i inputen stämmer överens med de krav på vad användaren ska skriva in som finns på POCO-klassen Errand. Om det inte gör det retunerar den ReportCrime-vyn. Samt sätter min Session till de värden användaren skrivit in
    /// </summary>
    /// <param name="errand"></param>
    /// <returns></returns>
    [HttpPost]
    public ViewResult ReportCrime(Errand errand)
    {
      //Om inputsen är korrekta
      if (ModelState.IsValid)
      {
        //skapar vi sessionen "NewErrandCoor" där objektet errand är värdena
        HttpContext.Session.SetJson("NewErrandCoor", errand);
        //Returnerar vyn Validate med objektet errand så att värdena kan valideras av användaren
        return View("Validate", errand);
      }
      else
      {
        return View();
      }
    }
  
    /// <summary>
    /// Denna metod är en hjälpmetod så att den avdelning samordnaren väljer läggs till på rätt ärende som tar in det departmentIdet som användaren har valt ifrån vyn CrimeCoordinator
    /// </summary>
    /// <param name="DepartmentId"></param>
    /// <returns></returns>
    public IActionResult UpdateDepa(string DepartmentId)
    {
      //Konventerar stringen "EID"(som är tempdata från actionmetoden CrimeCoordinator) till en integer
      int ErrId = int.Parse(TempData["EID"].ToString());

      //Startar metoden UpdateDepartment där vi skickar med DepartmentId och ErrandId så att metoden kan uppdatera rätt ärende
      repository.UpdateDepartment(DepartmentId, ErrId);

      //Redirect tillbaka till vyn "CrimeCoordinator" där vi skickar med ErrandId
      return RedirectToAction("CrimeCoordinator",new { id = ErrId });
    }

  }
}
