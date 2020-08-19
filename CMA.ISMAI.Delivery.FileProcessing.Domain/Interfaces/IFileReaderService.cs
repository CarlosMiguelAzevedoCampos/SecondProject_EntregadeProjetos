using System.Collections.Generic;

namespace CMA.ISMAI.Delivery.FileProcessing.Domain.Interfaces
{
    public interface IFileReaderService
    {
        List<string> ReturnJury(string studentNumber, string studentInstitute, string studentCourseName, string filePath);
    }
}
