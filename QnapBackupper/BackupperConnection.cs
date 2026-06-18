using System.Configuration;

namespace QnapBackupper
{
    internal class BackupperConnection
    {
        public string branchOffice { get; set; }
        public string originPath { get; set; }
        public string destinationPath { get; set; }
        public string networkConnection { get; set; }
        public string networkCredential { get; set; }
        public string networkDomain { get; set; }
        public string fileName { get; set; }

        public BackupperConnection(string _sucursal) 
        {
            branchOffice = _sucursal;
            originPath = ConfigurationManager.AppSettings["ORIGIN_PATH"];
            destinationPath = $"{ConfigurationManager.AppSettings["DESTINATION_PATH"]}{_sucursal}";
            networkConnection = $"\\\\rex{_sucursal}a\\C$";
            networkCredential = $"rex{_sucursal}a\\support";
            networkDomain = $"\\\\rex{_sucursal}a";
            fileName = "";

            if (_sucursal.Equals("00"))
            {
                networkConnection = "\\\\rex_ts\\C$";
                networkCredential = "rex_ts\\support";
                networkDomain = "\\\\rex_ts";
            }
        }
    }
}
