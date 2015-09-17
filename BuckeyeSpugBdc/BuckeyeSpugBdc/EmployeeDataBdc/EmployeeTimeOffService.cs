using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace BuckeyeSpugBdc.EmployeeDataBdc
{
    public partial class EmployeeTimeOffService
    {
        public static IEnumerable<TimeOff> ReadList()
        {
            using (var db = new HRLobDataContext(GetConnString()))
            {
                IEnumerable<TimeOff> timeOffList = from t in db.TimeOffs
                                                   select t;

                return timeOffList;
            }
        }

        public static TimeOff ReadItem(int employeeId)
        {
            using (var db = new HRLobDataContext(GetConnString()))
            {
                TimeOff toRecord =
                    (from to in db.TimeOffs
                     where to.EmployeeId == employeeId
                     select to).Single();
                return toRecord;
            }
        }

        // Utility method to get Connection Variables from the Secure Store Service
        // executing a proxy class
        // Code example provided located in the GIT Repo of this solution
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
