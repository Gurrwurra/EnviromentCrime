using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EnviromentCrime.Models;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EnviromentCrime.Controllers
{
  [Authorize(Roles = "Investigator")]
  public class InvestigatorController : Controller
  {
    private IErrandRepository repository;

    private IHostingEnvironment environment;

    private IHttpContextAccessor accessor;

    public InvestigatorController(IErrandRepository repo, IHostingEnvironment env, IHttpContextAccessor acc)
    {
      repository = repo;

      environment = env;

      accessor = acc;
    }

    /// <summary>
    /// Denna metod tar in string id vilket är det ÄrendeID som använderen väljer att klicka på. Som sedan retunerar Vyn med "ViewBag.Eid"(IDet) som i sin tur kallar på Component ShowOneErrand med det IDet
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public ViewResult CrimeInvestigator(int id)
    {
      ViewBag.Eid = id;

      ViewBag.Status = repository.ErrandStatuses;
      //TempData ID(ärendeidet) som används i hjälpmetoden UpdateSta
      TempData["ID"] = id;

      return View();
    }

    /*Metode retunerar vyn StartManager med fakedata från repository*/
    public ViewResult StartInvestigator()
    {
      return View(repository);
    }

    /// <summary>
    /// Denna metod tar in StatusId, events(vilket är investigatoraction), information(vilket är investigatorinfo), IFormFile loadSample(vilket är filen användaren väljer att ladda upp i under fliken prover) och IFormFile loadImage(vilket också är en fil, men i detta fall bör vara en bild, användaren väljer att ladda upp). Sedan läggs filerna till vardera mapp i wwwroot (sample och picture). För att sedan starta metoden UpdateStatus där namnen på filerna skickas med, samt tempdatan som är ErrandId så att variablerna läggs till i rätt ärende.
    /// </summary>
    /// <param name="StatusId"></param>
    /// <param name="events"></param>
    /// <param name="information"></param>
    /// <param name="loadSample"></param>
    /// <param name="loadImage"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> UpdateSta(string StatusId, string events, string information, IFormFile loadSample, IFormFile loadImage)
    {
      //Parsar tempdata ID till en integer
      int ErrId = int.Parse(TempData["ID"].ToString());
      //Om alla variablar är tomma
      if(StatusId == null && events == null && information == null && loadSample == null && loadImage == null){
        //returneras CrimeInvestigator med Ärendeidet
        return RedirectToAction("CrimeInvestigator", new { id = ErrId });
      }
      //Om filen loadSample inte är null
        if (loadSample != null) {
        //Om filen är större än 0 (om den har innehåll)
          if (loadSample.Length > 0)
          {
          //skapar en temporär fil
            var tempPath = Path.GetTempFileName();
            using (var stream = new FileStream(tempPath, FileMode.Create))
            {
            //kopirerar filen 
              await loadSample.CopyToAsync(stream);
         
            }
            //Gör så att filen "laddas upp/Sparas ner" till sample mappen i wwwroot
          var path = Path.Combine(environment.WebRootPath, "sample", loadSample.FileName);
          System.IO.File.Move(tempPath, path);
          }
        }
        //Samma som för loadSample
        if (loadImage != null)
        {
          if (loadImage.Length > 0)
          {
            var tempPath = Path.GetTempFileName();
            
            using (var stream = new FileStream(tempPath, FileMode.Create))
            {
              await loadImage.CopyToAsync(stream);
            }
           var path = Path.Combine(environment.WebRootPath, "picture", loadImage.FileName);
            System.IO.File.Move(tempPath, path);
          }
        }

        //Startar metoden UpdateStatus och skickar med variablerna plus loadSamples filnamn samt loadImages filnamn
        repository.UpdateStatus(StatusId, ErrId, events, information, loadSample?.FileName, loadImage?.FileName);

      //start actionmetoden Crimeinvestigator med ärendeidet
        return RedirectToAction("CrimeInvestigator", new { id = ErrId });
      }
    }
  }

