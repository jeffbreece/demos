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
            var db = new PtoTableDataContext(GetConnString());
            IEnumerable<TimeOff> timeOffList = from t in db.TimeOffs
                                                        select t;

            return timeOffList;
        }

        public static TimeOff ReadItem(int employeeId)
        {
            var db = new PtoTableDataContext(GetConnString());
            TimeOff toRecord =
                (from to in db.TimeOffs
                 where to.EmployeeId == employeeId
                 select to).Single();
            return toRecord;
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
