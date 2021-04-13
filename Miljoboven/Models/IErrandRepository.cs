using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnviromentCrime.Models
{
  public interface IErrandRepository
  {
    IQueryable<Errand> Errands { get; }
    IQueryable<Employee> Employees { get; }
    IQueryable<ErrandStatus> ErrandStatuses { get; }
    IQueryable<Department> Departments { get; }

    /* Den här metoden tar in en string id vilket i sin tur start upp metoden GetErrand i FakeErrandRepository som tar fram det ärendet med det samma id */
    Task<Errand> GetErrand(int id);

    void SaveErrand(Errand errand);

    void UpdateDepartment(string id, int eID);

    void UpdateStatus(string id, int eID, string events, string information, string sampleName, string imageName);

    void UpdateEmployee(string id, int eID, bool noReason, string reason);

    IQueryable<ListErrand> ErrandsName();

    IQueryable<ListErrand> ErrandsManager(string id);

    IQueryable<ListErrand> ErrandsEmplo(string id);

    IQueryable<Employee> EmployManager(string id);
  }
}
