using NLog;
using System;
using System.IO;

namespace QnapBackupper
{
    internal class ManageFile
    {
        public static void CopyNewFromBranchToQnap(BackupperConnection _buConn, Logger log) 
        {
            string[] _filesInOrigin = Directory.GetFiles(_buConn.networkConnection + _buConn.originPath, "*.bak", SearchOption.TopDirectoryOnly);
            if (_filesInOrigin.Length > 0)
            {
                log.Info($"Se detectan {_filesInOrigin.Length} archivos candidatos para respaldar.");
                int i = 1;
                foreach (string _fileToCopy in _filesInOrigin)
                {
                    _buConn.fileName = _fileToCopy.Substring(_fileToCopy.LastIndexOf("\\") + 1);
                    log.Info($"Archivo candidato {i++} de {_filesInOrigin.Length}: {_buConn.fileName}.");
                    if (!File.Exists(_buConn.destinationPath + "\\" + _buConn.fileName))
                    {
                        log.Info($"Copiando archivo desde {_buConn.originPath} hacia {_buConn.destinationPath}.");
                        try
                        {
                            File.Copy(_fileToCopy, _buConn.destinationPath + "\\" + _buConn.fileName, false);
                            log.Info("Archivo copiado exitosamente.");
                        }
                        catch (Exception ex)
                        {
                            log.Error($"Falló la copia del archivo: {ex}.");
                        }
                    }
                    else {
                        log.Info($"El archivo ya fue respaldado en una ejecución previa.");
                    }
                }
            }
            else
            {
                log.Error($"ATENCION: no se encontraron archivos de extensión .bak en {_buConn.originPath}.");
            }
        }

        public static void DeleteOldFromQnap(BackupperConnection _buConn, Logger log)
        {
            string[] _filesInDestination = Directory.GetFiles(_buConn.destinationPath, "*.bak", SearchOption.TopDirectoryOnly);
            if (_filesInDestination.Length > 0)
            {
                foreach (string _fileInDestination in _filesInDestination)
                {
                    _buConn.fileName = _fileInDestination.Substring(_fileInDestination.LastIndexOf("\\") + 1);
                    if (!File.Exists(_buConn.networkConnection + _buConn.originPath + "\\" + _buConn.fileName))
                    {
                        log.Info($"Eliminando archivo de respaldo {_buConn.fileName} en QNAP porque ya no existe en la sucursal.");
                        try {
                            File.Delete(_fileInDestination);
                            log.Info("Archivo eliminado exitosamente.");
                        }
                        catch (Exception ex) 
                        {
                            log.Error($"Falló la eliminación del archivo: {ex}.");
                        }
                    }
                }
            }
        }
    }
}
