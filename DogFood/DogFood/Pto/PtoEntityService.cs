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
            PtoTableDataContext db = new PtoTableDataContext(GetConnString());
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
            var employeeDbContext = new PtoTableDataContext(GetConnString());
            TimeOff employee =
                (from emp in employeeDbContext.TimeOffs
                 where emp.EmployeeId == employeeId
                 select emp).Single();
            return employee;
        }

        private static string GetConnString()
        {
            string connString = string.Empty;
            using (var userCredentials = SecureStoreProxy.GetCredentialsGetCredentialsFromSecureStoreService("DogFoodKey", SecureStoreProxy.CredentialType.Domain))
            {
                // Secure store service must have the SQL user, password and Host/Instance populated for this to return a proper connection string.
                connString = "Server=" + userCredentials.DomainName + ";Database=DemoDb;User Id=" + userCredentials.UserName + " ;Password=" + userCredentials.Password + ";";
            }
            return connString;
        }
    }
}
