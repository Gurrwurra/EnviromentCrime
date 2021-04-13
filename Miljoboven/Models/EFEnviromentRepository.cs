using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnviromentCrime.Models;
using Microsoft.EntityFrameworkCore;


namespace EnviromentCrime.Models
{
  public class EFEnviromentRepository : IErrandRepository
  {
    private ApplicationDbContext context;
    
    public EFEnviromentRepository(ApplicationDbContext ctx)
    {
      context = ctx;
    }


    //Joinar Samples och pictures
    public IQueryable<Errand> Errands => context.Errands.Include(e => e.Samples).Include(e => e.Pictures);
    public IQueryable<Employee> Employees => context.Employees;
    public IQueryable<ErrandStatus> ErrandStatuses => context.ErrandStatuses;
    public IQueryable<Department> Departments => context.Departments;
    public IQueryable<Picture> Pictures => context.Pictures;
    public IQueryable<Sequence> Sequences => context.Sequences;

    /// <summary>
    /// Denna metod lägger till ett ärende i data basen. Metoden tar in objektet errand som användaren har fyllt i via något av de två formulären. Sedan kollar metoden om ärendet har ett id (eller rättare sagt om det är lika med 0). Om den inte har ett id så läggs variablerna RefNumber(får värdet av "2018-45-" plus CurrentValue i Sequence. StatusId blir till "S_A". Sedan läggs resten av variablerna användaren har fyllt i in i databasen. Var på CurrentValue uppdateras med 1 så att nästa ärende får ett nytt RefNumber.
    /// </summary>
    /// <param name="errand"></param>
    public void SaveErrand(Errand errand)
    {
      if (errand.ErrandID == 0)
      {
        var genId = Sequences.Where(sequence => sequence.Id == 1).First();
        int sequenceValue = genId.CurrentValue;
        //Ger Refnumber "2018-45-" plus currentvalue från Sequncetabellen så att ärenderna uppdateras
        errand.RefNumber = "2018-45-" + sequenceValue;
        //status får S_A (inrapporterad
        errand.StatusId = "S_A";
        //lägger till errand i databasaen
        context.Errands.Add(errand);
        //updaterar CurrentContext
        UpdateCurrentValue(genId);
      }
      //sparar
      context.SaveChanges();
    }
    /// <summary>
    /// Denna metod uppdaterar CurrentValue med 1
    /// </summary>
    /// <param name="sequence"></param>
    public void UpdateCurrentValue(Sequence sequence)
    {
      sequence.CurrentValue = sequence.CurrentValue + 1;
    }

    /// <summary>
    /// Metoden hämtar det specefika ärendet som användaren har klickat på
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<Errand> GetErrand(int id)
    {
      return Task.Run(() =>
      {
        //errandDetail får värdet av objektet där id stämmer överens med ErrandID i Errands
        var errandDetail = Errands.Where(errand => errand.ErrandID == id).First();
        return errandDetail;
      });
    }

    /// <summary>
    /// Denna metod updaterar Department id för ett ärende. Den tar in string id, vilket är själva DepartmentId'et, samt eId som är själva ärende idet. Sedan kollar metoden om id == null och om det är det så sätts departmentId till ingenting, sedan om id har ett värde så sätts DepartmentId till id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="eId"></param>
    public void UpdateDepartment(string id, int eId)
    {
      var errandDetail = Errands.Where(errand => errand.ErrandID == eId).First();
      if (id == null)
      {
        //Om id null så ska departmentid inte ha något värde då användaren inte har valt ett department
        errandDetail.DepartmentId = "";
      }
      else
      {
        //annars får den det värdet
        errandDetail.DepartmentId = id;
      }
      context.SaveChanges();
    }
    /// <summary>
    /// Denna metod uppdaterar ett ärende från handläggarens sida. metoden tar in id, vilket är StatusId, eId (ärende idet), events(InvestigatorAction), info (InvestigatorInfo), samt namnen på de filer användaren har laddat upp. Metoden kollar sedan om ärendet har ett id, där den sedan kollar vardera variabel om dem har ett värde var för sig. Så att systemet inte krashar när det kommer in en variabel med ett null värde.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="eId"></param>
    /// <param name="events"></param>
    /// <param name="information"></param>
    /// <param name="sampleName"></param>
    /// <param name="imageName"></param>
    public void UpdateStatus(string id, int eId, string events, string information, string sampleName, string imageName)
    {
      //Om Eid inte är lika med 0 alltså om ärendet redan finns
      if (eId != 0)
      {
        //hämta det ärendet
        var errandDetail = Errands.Where(errand => errand.ErrandID == eId).First();
        //om Id inte är null alltså om användaren har angett ett statusId
        if (id != null)
        {
          //StatusId sätts till det id användaren har angett
          errandDetail.StatusId = id;
        }
        //Om events inte är null
        if (events != null)
        {
          //InvestigatorAction får värdet av tidigare events samt det nya som användaren har skrivit in
          errandDetail.InvestigatorAction = errandDetail.InvestigatorAction + "\n" + events;
        }
        //Om information inte är null
        if (information != null)
        {
          //InvestigatorInfo får värdet av tidigare InvestigatorInfo samt det nya användaren har skrivit in
          errandDetail.InvestigatorInfo = errandDetail.InvestigatorInfo + "\n" + information;
        }
        if (sampleName != null)
        {
          //Skapar ett objekt av sample där jag sedan lägger in namn och ärende id
          var sample = new Sample { SampleName = sampleName, ErrandId = eId };
          //adderar sample
          context.Samples.Add(sample);
        }
        if (imageName != null)
        {
          //skapar ett objekt av picture där jag sedan lägger in man och ärende id
          var picture = new Picture { PictureName = imageName, ErrandId = eId };
          //adderar picture
          context.Pictures.Add(picture);
        }
      }
      context.SaveChanges();
    }

    /// <summary>
    /// Denna metod uppdaterar ett ärende utifrån var en chef har ändrat på i sin vy. Metoden tar in id(EmployeeId), eId (ärende idet), noReason (som är en boolean) samt reason. Om noReason är lika med true (vilket betyder att chefen har valt att ärendet ska läggas ned) så ska StatusId på ärendet samt Investigator info uppdateras. Där StatusId blir till "S_B" och InvestigatorInfo får det värde chefen har skrivit in. Sedan ger den också EmployeeId ett null värde då ärendet inte ska hanteras längre. Om noReason inte är true så ska EmployeeId uppdateras med den anställdas id som chefen har valt samt att StatusId blir till "S_A"
    /// </summary>
    /// <param name="id"></param>
    /// <param name="eId"></param>
    /// <param name="noReason"></param>
    /// <param name="reason"></param>
    public void UpdateEmployee(string id, int eId, bool noReason, string reason)
    {
      var errandDetail = Errands.Where(errand => errand.ErrandID == eId).First();
      //om noReason är true alttså att användaren har valt att inte utvärdera ärendet 
      if (noReason == true)
      {
        //statusId får S_B (Ingen Åtgärd)
        errandDetail.StatusId = "S_B";
        //Anledning till varför det inte blir någon åtgärd sätts till Investigatorinfo
        errandDetail.InvestigatorInfo = reason;
        //ingen anställd sätts till ärendet
        errandDetail.EmployeeId = null;
      }
      //annars sätts en anställd till ärendet
      else
      {
        errandDetail.EmployeeId = id;
        //ärendetstatuset ändras till S_A (inrapporterad) samt tas investigatorinfo bort då det inte finns en anledning längre.
        errandDetail.StatusId = "S_A";
        errandDetail.InvestigatorInfo = "";
      }
      context.SaveChanges();
    }

    /// <summary>
    /// Denna metod retunerar en lista med ärenden där statusnamn, employeename samt departmentname är med så de kan skrivas ut rätt på respektive vy. 
    /// </summary>
    /// <returns></returns>

    public IQueryable<ListErrand> ErrandsName()
    {
      //errandList får värden där jag joinar ErrandStatuses, Departments och Employees med LINQ-query så att listan som ska returneras returnerar namnen på samtliga variabler istället för själv idet
      var errandList = from err in Errands
                       //joinar ErrandStatuses där StatusId är samma för både Errands och ErrandStatuses
                       join stat in ErrandStatuses on err.StatusId equals stat.StatusId
                       //joinar Departments med DepartmentId för de är samma för både Departments och Errands
                       join dep in Departments on err.DepartmentId equals dep.DepartmentId
                       into departmentErrand
                       from deptE in departmentErrand.DefaultIfEmpty()
                       join em in Employees on err.EmployeeId equals em.EmployeeId
                       into employeeErrand
                       from empE in employeeErrand.DefaultIfEmpty()
                       orderby err.RefNumber descending
                       //ger värdena för ListErrand
                       select new ListErrand
                       {
                         DateOfObservation = err.DateOfObservation,
                         ErrandID = err.ErrandID,
                         RefNumber = err.RefNumber,
                         TypeOfCrime = err.TypeOfCrime,
                         StatusName = stat.StatusName,
                         //Om departmedId är null Så skrivs "Ej tillsatt" samma för EmployeeId
                         DepartmentName = (err.DepartmentId == null ? "Ej tillsatt" : deptE.DepartmentName),
                         EmployeeName = (err.EmployeeId == null ? "Ej tillsatt" : empE.EmployeeName)
                       };
      //retunerar errandList
      return errandList;
    }

    /// <summary>
    /// Denna metod retunerar en lista med ärenden som tillhör en viss avdelning. som sedan retuneras till vyer för chefer
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public IQueryable<ListErrand> ErrandsManager(string id)
    {
      //Tar ut department idét som tillhör chefen genom hens EmployeeId
      var dertMan = context.Employees.Where(em => em.EmployeeId == id).First().DepartmentId;
    
              //errandList får värden där jag joinar ErrandStatuses, Departments och Employees så att listan som ska returneras returnerar namnen på samtliga variabler istället för själv idet 
             var errandList = from err in Errands
                               join stat in ErrandStatuses on err.StatusId equals stat.StatusId
                               join dep in Departments on err.DepartmentId equals dep.DepartmentId
                               into departmentErrand
                               from deptE in departmentErrand.DefaultIfEmpty()
                               join em in Employees on err.EmployeeId equals em.EmployeeId
                               into employeeErrand
                               from empE in employeeErrand.DefaultIfEmpty()
                               //lägg till i listan var Errands.DepartmentsId == dertMan som är chefens DepartmentId
                               where err.DepartmentId == dertMan
                               orderby err.RefNumber descending
                               select new ListErrand
                               {
                                 DateOfObservation = err.DateOfObservation,
                                 ErrandID = err.ErrandID,
                                 RefNumber = err.RefNumber,
                                 TypeOfCrime = err.TypeOfCrime,
                                 StatusName = stat.StatusName,
                                 DepartmentName = (err.DepartmentId == null ? "Ej tillsatt" : deptE.DepartmentName),
                                 EmployeeName = (err.EmployeeId == null ? "Ej tillsatt" : empE.EmployeeName)
                               };
      return errandList;
    }
    /// <summary>
    /// Denna metod retunerar en lista med anställda för en viss avdelning som sedan skrivs ut på chefens vy, så att hen endast kan välja anställda från dens avdelning
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public IQueryable<Employee> EmployManager(string id)
    {
      //Tar ut department idét som tillhör chefen genom hens EmployeeId
      var dertMan = context.Employees.Where(em => em.EmployeeId == id).First().DepartmentId;
      //employeeList får värden där jag joinar Departments och Employees med LINQ-query så att listan som ska returneras returnerar namnen på samtliga variabler istället för själv idet
      var employeeList = from emplo in Employees
                         where emplo.DepartmentId == dertMan
                         select new Employee
                         {
                           EmployeeName = emplo.EmployeeName,
                           EmployeeId = emplo.EmployeeId
                         };
        return employeeList;
    }

    /// <summary>
    /// Denna metod retunerar en lista för en specifik Handläggare. Metoden tar id vilket är då själva användarnamnet för inloggningen vilket är samma som EmployeeId. Sedan skapar jag en anonym variabel som får värdet av en lista där ärenderna med samma employeeId som Handläggaren.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public IQueryable<ListErrand> ErrandsEmplo(string id)
    {
      //errandList får värden där jag joinar ErrandStatuses, Departments och Employees med LINQ-query så att listan som ska returneras returnerar namnen på samtliga variabler istället för själv idet
      var errandList = from err in Errands
                       join stat in ErrandStatuses on err.StatusId equals stat.StatusId
                       join dep in Departments on err.DepartmentId equals dep.DepartmentId
                       into departmentErrand
                       from deptE in departmentErrand.DefaultIfEmpty()
                       join em in Employees on err.EmployeeId equals em.EmployeeId
                       into employeeErrand
                       from empE in employeeErrand.DefaultIfEmpty()
                         //lägg till i listan var Errands.EmployeeId == id (vilket är användarnamnet
                       where err.EmployeeId == id
                       orderby err.RefNumber descending
                       select new ListErrand
                       {
                         DateOfObservation = err.DateOfObservation,
                         ErrandID = err.ErrandID,
                         RefNumber = err.RefNumber,
                         TypeOfCrime = err.TypeOfCrime,
                         StatusName = stat.StatusName,
                         DepartmentName = (err.DepartmentId == null ? "Ej tillsatt" : deptE.DepartmentName),
                         EmployeeName = (err.EmployeeId == null ? "Ej tillsatt" : empE.EmployeeName)
                       };
      return errandList;
    }

  }
}
