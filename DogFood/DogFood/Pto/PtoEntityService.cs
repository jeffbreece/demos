using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DogFood.Pto
{
    public partial class PtoEntityService
    {

        public static IEnumerable<TimeOff> ReadList()
        {
            PtoTableDataContext db = new PtoTableDataContext("");
            var employeePtoData = from e in db.TimeOffs
                              select new TimeOff
                              {
                                  EmployeeId = e.EmployeeId,
                                  SickHours = e.SickHours,
                                  PtoHours = e.PtoHours
                              };
            return employeePtoData;
        }

        public static TimeOff ReadItem(int employeeId)
        {
            throw new System.NotImplementedException();
        }
    }
}
