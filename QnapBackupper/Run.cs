using System;
using System.Configuration;
using System.IO;
using System.Net;
using NLog;

namespace QnapBackupper
{
    internal class Run
    {
        public static BackupperConnection _buConn;

        public static void Backup() 
        {
            string _sucursalesString = ConfigurationManager.AppSettings["SUCURSALES"];
            string[] _sucursalesArray = _sucursalesString.Split(',');

            if (_sucursalesArray.Length > 0 && _sucursalesArray[0].Length > 0) 
            {
                Logger log = LogManager.GetCurrentClassLogger();
                log.Info("Se iniciar proceso de copiado de archivos de respaldo con extensión .bak desde sucursal hacia QNAP.");
                log.Info($"Se encuentran {_sucursalesArray.Length} sucursales listadas para respaldar en el archivo App.config.");
                int i = 1;
                foreach (string _sucursal in _sucursalesArray)
                {
                    log.Info($"Sucursal {_sucursal} ({i++} de {_sucursalesArray.Length}).");
                    _buConn = new BackupperConnection(_sucursal);

                    if (!Directory.Exists(_buConn.destinationPath))
                    {
                        log.Info($"Se crea la carpeta {_buConn.destinationPath} para guardar respaldos en QNAP.");
                        Directory.CreateDirectory(_buConn.destinationPath);
                    }

                    string laps = SQL.GetLaps(_sucursal, log);
                    if (!laps.Equals("VACIO"))
                    {
                        log.Info($"Generando conexión con terminal de caja rex{_sucursal}a.");
                        try
                        {
                            using (new NetworkConnection(_buConn.networkConnection, 
                                new NetworkCredential(_buConn.networkCredential, laps, _buConn.networkDomain)))
                            {
                                ManageFile.CopyNewFromBranchToQnap(_buConn, log);
                                ManageFile.DeleteOldFromQnap(_buConn, log);
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error($"Falló el método Run.Backup: {ex}.");
                        }
                    }
                }
                log.Info("Proceso finalizado");
                log.Info("---------------------------------------------------------------------------------------------------------");
            }
        }
    }
}
