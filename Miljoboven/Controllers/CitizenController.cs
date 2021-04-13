using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EnviromentCrime.Models;
using EnviromentCrime.Infrastructure;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EnviromentCrime.Controllers
{


  public class CitizenController : Controller
  {
    private IErrandRepository repository;

    public CitizenController(IErrandRepository repo)
    {
      repository = repo;
    }

    public ViewResult Services()
    {
      return View();
    }

    public ViewResult Faq()
    {
      return View();
    }

    public ViewResult Contact()
    {
      return View();
    }

    /// <summary>
    /// Metoden retunerar vyn "Thanks" som tackar användaren för att raportera in ett brott.
    /// </summary>
    /// <param name="errand"></param>
    /// <returns></returns>
    public ViewResult Thanks(Errand errand)
    {
      //Ger ett Errand objektet addErrand värdet av Sessionen "NewErrand".
      Errand addErrand = HttpContext.Session.GetJson<Errand>("NewErrand");

      //Startar metoden SaveErrand där jag skickar med addErrand objektet. Så att SaveErrand metoden kan spara ärendet
      repository.SaveErrand(addErrand);
      //Tar bort Session "NewErrand" så det inte visas i formulären
      HttpContext.Session.Remove("NewErrand");
      //retunerar addErrand objektet så att rätt RefNumber kan skrivas ut
      return View(addErrand);
    }


    /// <summary>
    /// Denna metod retunerar sidan validate om det användaren har skrivit in i inputen stämmer överens med de krav på vad användaren ska skriva in som finns på POCO-klassen Errand. Om det inte gör det retunerar den Index-vyn
    /// </summary>
    /// <param name="errand"></param>
    /// <returns></returns>

    [HttpPost]
    public ViewResult ReportCrime(Errand errand)
    {
      //Om inputsen är korrekta så ska vi ge session till "errand" objektet som sedan retunerar vyn Validate som skriver ut värdena.
      if (ModelState.IsValid)
      {
        HttpContext.Session.SetJson("NewErrand", errand);
        return View("Validate", errand);
      }
      //Annars retunerar vi Index-vyn där felmedelanddanden skrivs ut
      else
      {
        return View("../Home/Index");
      }
    }

  }
}
