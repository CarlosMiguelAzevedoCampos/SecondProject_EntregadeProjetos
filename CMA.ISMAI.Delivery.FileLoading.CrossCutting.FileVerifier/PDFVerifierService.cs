using CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces;
using CMA.ISMAI.Delivery.Logging.Interface;
using iText.Kernel.Pdf;
using System;
using System.IO;

namespace CMA.ISMAI.Delivery.FileLoading.CrossCutting.FileVerifier
{
    public class PDFVerifierService : IPDFVerifierService
    {
        private readonly ILoggingService _log;

        public PDFVerifierService(ILoggingService log)
        {
            _log = log;
        }

        public bool ArePdfFilesOk(string filePath)
        {
            try
            {
                string[] pdfFiles = ReturnPDFFiles(filePath, "*.pdf*");
                foreach (var item in pdfFiles)
                {
                    FileInfo info = new FileInfo(item);
                    PdfDocument pdfDoc = new PdfDocument(new PdfReader(info.FullName));
                    if (pdfDoc.GetNumberOfPages() == 0)
                        return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _log.Fatal(ex.ToString());
            }
            return false;
        }

        private string[] ReturnPDFFiles(string filepath, string fileExtension)
        {
            return Directory.GetFiles(filepath, fileExtension, SearchOption.AllDirectories);
        }
    }
}
