using CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces;
using CMA.ISMAI.Delivery.Logging.Interface;
using System;
using System.IO;
using System.IO.Compression;

namespace CMA.ISMAI.Delivery.FileLoading.CrossCutting.FileVerifier
{
    public class FileVerifierService : IFileVerifierService
    {
        private readonly IPDFVerifierService _pdfVerifierService;
        private readonly IVerifyFilesExtensions _audioVerifierService;
        private readonly ILoggingService _log;

        public FileVerifierService(IPDFVerifierService pdfVerifierService, IVerifyFilesExtensions audioVerifierService, ILoggingService log)
        {
            _pdfVerifierService = pdfVerifierService;
            _audioVerifierService = audioVerifierService;
            _log = log;
        }

        public bool UnzipFiles(string filePath, string extractPath)
        {
            try
            {
                using (ZipArchive zout = ZipFile.OpenRead(filePath))
                {
                    zout.ExtractToDirectory(extractPath);
                }
                return true;
            }catch(Exception ex)
            {
                _log.Fatal(ex.ToString());
            }
            return false;
        }

        public bool VerifyIfFilesAreCorrupted(string filePath)
        {
            if (!_pdfVerifierService.ArePdfFilesOk(filePath))
                return true;
            if (!_audioVerifierService.AreFilesInTheCorrectFormat(filePath))
                return true;
            return false;
        }

        public bool VerifyIfPublicAndPriateFilesExist(string filePathExtract, string privateFile, string publicFile)
        {
            return (File.Exists(string.Format("{0}/{1}", filePathExtract, publicFile)) && (Path.GetExtension(string.Format("{0}/{1}", filePathExtract, publicFile)).ToLower() == ".pdf"))
                || (File.Exists(string.Format("{0}/{1}", filePathExtract, privateFile)) && (Path.GetExtension(string.Format("{0}/{1}", filePathExtract, privateFile)).ToLower() == ".pdf"));
        }
    }
}
