using CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces;
using CMA.ISMAI.Delivery.Logging.Interface;
using System;
using System.IO;

namespace CMA.ISMAI.Delivery.FileLoading.CrossCutting.FileVerifier
{
    public class VerifyFilesExtensions : IVerifyFilesExtensions
    {
        private readonly ILoggingService _log;

        public VerifyFilesExtensions(ILoggingService log)
        {
            _log = log;
        }

        public bool AreFilesInTheCorrectFormat(string filePath)
        {
            try
            {
                string[] files = ReturnAllFilesInADirectory(filePath);
                foreach (var item in files)
                {
                    FileInfo info = new FileInfo(item);
                    string[] types = info.Name.Split('.');

                    if (types.Length > 0)
                    {
                        for (int i = 1; i < types.Length; i++)
                        {
                            if (string.Format(".{0}", types[i]) != info.Extension)
                                return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _log.Fatal(ex.ToString());
            }
            return false;
        }



        private string[] ReturnAllFilesInADirectory(string filepath)
        {
            return Directory.GetFiles(filepath, "*.*", SearchOption.AllDirectories);
        }
    }
}
