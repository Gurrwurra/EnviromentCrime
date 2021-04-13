using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EnviromentCrime.Models;
using EnviromentCrime.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Miljoboven.Controllers
{
  public class HomeController : Controller
  {
    private UserManager<IdentityUser> userManager;
    private SignInManager<IdentityUser> signInManager;
    private IErrandRepository repository;

    public HomeController(UserManager<IdentityUser> userMgr, SignInManager<IdentityUser> signInMgr, IErrandRepository repo)
    {
      userManager = userMgr;
      signInManager = signInMgr;
      repository = repo;
    }

   /// <summary>
   /// Denna metod retunerar vyn Index. 
   /// </summary>
   /// <returns></returns>
    public ViewResult Index()
    {
      //Hämtar sessionen "NewErrand" 
      var ReportErrand = HttpContext.Session.GetJson<Errand>("NewErrand");

      //Om ReportErrand är null så ska vyn retuneras. Detta eftersosm om användaren inte har skrivit in något ännu i formuläret.
      if(ReportErrand == null)
      {
        return View();
      }
      //annars retuneras vyn med sessionen
      else
      {
        return View(ReportErrand);
      }
      
    }

    /// <summary>
    /// Returnerar vyn Login
    /// </summary>
    /// <param name="returnUrl"></param>
    /// <returns></returns>
    [AllowAnonymous]
    public ViewResult Login(string returnUrl)
    {
      return View(new Login { ReturnUrl = returnUrl });
    }

    /// <summary>
    /// Denna metod kollar det användaren har skrivit in som användarnamn och lösenord de stämmer överens. Alltså om kontot finns i databasen
    /// </summary>
    /// <param name="login"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(Login login)
    {
      //Om det anvädnaren har skrivit in är korrekt 
      if (ModelState.IsValid)
      {
        //Objektet user får värderna av det användar namn användaren har skrivit in
        IdentityUser user = await userManager.FindByNameAsync(login.UserName);
        //om user inte är null
        if (user != null)
        {
          await signInManager.SignOutAsync();
          //Om lösenord är rätt
          if((await signInManager.PasswordSignInAsync(user, login.Password, false, false)).Succeeded)
          {
            //om user har rollen Coordinator
            if(await userManager.IsInRoleAsync(user, "Coordinator"))
            {
              //returneras vyn StartCoordinator
              return Redirect(login?.ReturnUrl ?? "/Coordinator/StartCoordinator");
            }
            //om user har rollen Investigator
            else if (await userManager.IsInRoleAsync(user, "Investigator")){
              //returneras vyn StartInvestigator
              return Redirect(login?.ReturnUrl ?? "/Investigator/StartInvestigator");
            }
            //om user har rollen Manager
            else if(await userManager.IsInRoleAsync(user, "Manager"))
            {
              //returneras vyn StartManager
              return Redirect(login?.ReturnUrl ?? "/Manager/StartManager");
            }
          }
        }
      }
      //Om det skulle vara så att användaren har skrivit in fel användarnamn eller lösenord så ska felmedelandet skrivas ut och sedan retunera vyn med login objektet
      ModelState.AddModelError("", "Fel användarnamn eller lösenord");
      return View(login);
    }

    public async Task<RedirectResult> Logout(string returnUrl = "/")
    {
      await signInManager.SignOutAsync();
      return Redirect(returnUrl);
    }

    /// <summary>
    /// Om en användare skulle försöka komma åt sidorna utan att vara inloggad eller inte har tillgång till sidorna så ska vyn AccessDenied returneras
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
      return View();
    }

  }
}
