using CMA.ISMAI.Delivery.Logging.Interface;
using CMA.ISMAI.Delivery.Payment.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using System;
using System.IO;

namespace CMA.ISMAI.Delivery.Payment.CrossCutting.FileReader
{
    public class FileReaderService : IFileReaderService
    {
        private readonly ILoggingService _log;
        public FileReaderService(ILoggingService log)
        {
            _log = log;
        }
        public bool PaymentHasBeenDone(string studentNumber, string courseName, string institutionName, string filePath)
        {
            return ReturnFileData(studentNumber, courseName, institutionName, filePath);
        }

        private bool ReturnFileData(string studentNumber, string courseName, string institutionName, string filePath)
        {
            try
            {
                var fi = new FileInfo(filePath);
                if (fi.Exists)
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (var package = new ExcelPackage(fi))
                    {
                        for (int i = 0; i < package.Workbook.Worksheets.Count; i++)
                        {
                            var totalRows = package.Workbook.Worksheets[i].Dimension?.Rows;
                            for (int j = 1; j <= totalRows.Value; j++)
                            {
                                try
                                {
                                    if (package.Workbook.Worksheets[i].Cells[j, 1].Value.ToString().ToLower() == studentNumber.ToLower() &&
                                         package.Workbook.Worksheets[i].Cells[j, 2].Value.ToString().ToLower() == courseName.ToLower() &&
                                         package.Workbook.Worksheets[i].Cells[j, 3].Value.ToString().ToLower() == institutionName.ToLower())
                                        return true;
                                }
                                catch (Exception ex)
                                {
                                    _log.Info(string.Format("{0}, This error's can happend when empty cells are in the excel..", ex.ToString()));
                                }
                            }
                        }
                    }
                    return false;
                }
                _log.Info("File not found...");
            }
            catch (Exception ex)
            {
                _log.Fatal(ex.ToString());
            }
            return false;
        }
    }
}
