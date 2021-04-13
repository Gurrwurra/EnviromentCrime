using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnviromentCrime.Models;
using Microsoft.AspNetCore.Mvc;

namespace EnviromentCrime.Components
{
  public class ShowOneErrand:ViewComponent
  {
    private IErrandRepository repository;

    public ShowOneErrand(IErrandRepository repo)
    {
      repository = repo;
    }

    /// <summary>
    /// Den här metoden tar in string id som sedan startar metoden GetErrand i Repository med samt skickar med id. Det som retuneras läggs in i oneErrand och som sedan skickas till vyn "default"
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IViewComponentResult> InvokeAsync(int id)
    {

      //Anonyma objektet oneErrand får värdet av vad metoden GetErrand retunerar i IErrandRepository där metoden tar in ärende idet för ärendet.
      var oneErrand = await repository.GetErrand(id);

      //retunerar vyn "Default" med objektet oneErrand
      return View(oneErrand);
    }
  }
}
