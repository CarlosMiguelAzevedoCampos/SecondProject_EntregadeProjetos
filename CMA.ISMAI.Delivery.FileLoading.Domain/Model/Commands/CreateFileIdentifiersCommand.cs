using NetDevPack.Messaging;
using System;

namespace CMA.ISMAI.Delivery.FileLoading.Domain.Model
{
    public class CreateFileIdentifiersCommand : Command
    {
        public CreateFileIdentifiersCommand(Guid id, string filePath, string studentEmail, string universityEmail)
        {
            Id = id;
            FilePath = filePath;
            StudentEmail = studentEmail;
            UniversityEmail = universityEmail;
        }
        public Guid Id { get; private set; }
        public string FilePath { get; private set; } 
        public string StudentEmail { get; private set; }
        public string UniversityEmail { get; private set; }
    }
}
