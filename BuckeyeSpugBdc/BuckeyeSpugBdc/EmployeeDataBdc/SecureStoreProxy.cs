using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.BusinessData.Infrastructure.SecureStore;
using Microsoft.Office.SecureStoreService.Server;
using Microsoft.SharePoint;
using System.Security;
using Microsoft.SharePoint.Administration;
using System.Runtime.InteropServices;
using System.Web;

namespace BuckeyeSpugBdc.EmployeeDataBdc
{
    class SecureStoreProxy
    {
        public enum CredentialType
        {
            Domain,
            Generic
        }

        // retrieve credentials
        public static UserCredentials GetCredentialsGetCredentialsFromSecureStoreService(string applicationId,
            CredentialType credentialType)
        {
            ISecureStoreProvider provider = SecureStoreProviderFactory.Create();
            ISecureStoreServiceContext providerContext = provider as ISecureStoreServiceContext;
            providerContext.Context = SPServiceContext.GetContext(GetCentralAdminSite());

            if (provider == null)
            {
                throw new InvalidOperationException("Unable to get an ISecureStoreProvider");
            }


            using (SecureStoreCredentialCollection credentials = provider.GetCredentials(applicationId))
            {
                var un = from c in credentials
                         where
                             c.CredentialType ==
                             (credentialType == CredentialType.Domain
                                 ? SecureStoreCredentialType.WindowsUserName
                                 : SecureStoreCredentialType.UserName)
                         select c.Credential;

                var pd = from c in credentials
                         where
                             c.CredentialType ==
                             (credentialType == CredentialType.Domain
                                 ? SecureStoreCredentialType.WindowsPassword
                                 : SecureStoreCredentialType.Password)
                         select c.Credential;

                var dm = from c in credentials
                         where
                             c.CredentialType == SecureStoreCredentialType.Generic
                         select c.Credential;

                SecureString userName = un.First((d => d.Length > 0));
                SecureString password = pd.First((d => d.Length > 0));
                SecureString domain = dm.First((d => d.Length > 0));

                var userCredentials = new UserCredentials(userName, password, domain);
                return userCredentials;
            }
        }

        public static SPSite GetCentralAdminSite()
        {
            SPAdministrationWebApplication adminWebApp = SPAdministrationWebApplication.Local;
            if (adminWebApp == null)
            {
                throw new InvalidProgramException("Unable to get the admin web app");
            }

            SPSite adminSite = null;
            Uri adminSiteUri = adminWebApp.GetResponseUri(SPUrlZone.Default);
            if (adminSiteUri != null)
            {
                adminSite = adminWebApp.Sites[adminSiteUri.AbsoluteUri];
            }
            else
            {
                throw new InvalidProgramException("Unable to get Central Admin Site.");
            }

            return adminSite;
        }


    }

    public class UserCredentials : IDisposable
    {
        // stores credentaisl
        public String DomainName;

        private readonly SecureString _userName;
        public String UserName
        {
            get { return ConvertToUnsecuredString(_userName); }
        }

        private readonly SecureString _password;
        public String Password
        {
            get { return ConvertToUnsecuredString(_password); }
        }

        public UserCredentials(SecureString username, SecureString password)
        {
            _userName = username.Copy();
            _password = password.Copy();
        }

        public UserCredentials(SecureString username, SecureString password, SecureString domain)
        {
            _userName = username.Copy();
            _password = password.Copy();
            DomainName = ConvertToUnsecuredString(domain);
        }

        private static string ConvertToUnsecuredString(SecureString securedString)
        {
            if (securedString == null) return string.Empty;
            IntPtr uString = IntPtr.Zero;
            try
            {
                uString = Marshal.SecureStringToGlobalAllocUnicode(securedString);
                return Marshal.PtrToStringUni(uString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(uString);
            }
        }

        private Boolean _isDisposed;
        public void Dispose()
        {
            if (!_isDisposed) return;
            _userName.Dispose();
            _password.Dispose();
            _isDisposed = true;

        }
    }
}
