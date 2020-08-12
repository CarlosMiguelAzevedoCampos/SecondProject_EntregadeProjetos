using System.Collections.Generic;

namespace CMA.ISMAI.Delivery.FileProcessing.Domain.Interfaces
{
    public interface IGenerateJuryPageService
    {
        bool AddJuryPage(string filePath, List<string> jury);
    }
}
