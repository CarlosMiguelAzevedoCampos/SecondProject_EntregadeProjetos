using CMA.ISMAI.Delivery.FileProcessing.Domain.Interfaces;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;

namespace CMA.ISMAI.Delivery.FileProcessing.CrossCutting.FileReader
{
    public class FileReaderService : IFileReaderService
    {
        public List<string> ReturnJury(string studentNumber, string studentInstitute, string studentCourseName)
        {
            List<string> jury = new List<string>();
            try
            {
                var fi = new FileInfo(@"C:\Users\Carlos Campos\Desktop\jury.xlsx");
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
                                         package.Workbook.Worksheets[i].Cells[j, 2].Value.ToString().ToLower() == studentInstitute.ToLower() &&
                                         package.Workbook.Worksheets[i].Cells[j, 3].Value.ToString().ToLower() == studentCourseName.ToLower())
                                    {
                                        for (int p = 4; p <9; p++)
                                        {
                                            jury.Add(package.Workbook.Worksheets[i].Cells[j, p].Value.ToString());
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    break;
                                    //     _log.Info(string.Format("{0}, This error's can happend when empty cells are in the excel..", ex.ToString()));
                                }
                            }
                        }
                    }
                    return jury;
                }
                //   _log.Info("File not found...");
            }
            catch (Exception ex)
            {
                //   _log.Fatal(ex.ToString());
            }
            return jury;
        }
    }
}
