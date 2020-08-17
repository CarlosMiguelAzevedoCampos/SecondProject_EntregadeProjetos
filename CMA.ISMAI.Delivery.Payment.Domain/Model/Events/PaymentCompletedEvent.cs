using NetDevPack.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CMA.ISMAI.Delivery.Payment.Domain.Model.Events
{
    public class PaymentCompletedEvent : Event
    {
        public PaymentCompletedEvent(string studentNumber, string instituteName, string courseName)
        {
            StudentNumber = studentNumber;
            InstituteName = instituteName;
            CourseName = courseName;
        }

        public string StudentNumber { get; set; }
        public string InstituteName { get; set; }
        public string CourseName { get; set; }
    }
}
