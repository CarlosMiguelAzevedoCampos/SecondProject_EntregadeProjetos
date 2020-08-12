using System;
using System.Collections.Generic;

namespace CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces
{
    public interface IFileIdentifierService
    {
        Dictionary<string, Guid> GenerateFileIdentifier(string filepath);
    }
}
