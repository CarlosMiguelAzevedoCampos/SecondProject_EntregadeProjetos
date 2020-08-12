using CMA.ISMAI.Delivery.FileProcessing.Domain.Models;
using NetDevPack.Mediator;
using System;

namespace CMA.ISMAI.Delivery.FileProcessing.UI
{
    internal class ConsoleApplication
    {
        private readonly IMediatorHandler _mediatr;

        public ConsoleApplication(IMediatorHandler mediatr)
        {
            _mediatr = mediatr;
        }

        public void StartService()
        {
            GenerateJuryPageCommand generateJuryPageCommand = new GenerateJuryPageCommand("Carlos Campos","A029216", "Informática", "ISMAI", @"C:\Users\Carlos Campos\Downloads\as");

            this._mediatr.SendCommand<GenerateJuryPageCommand>(generateJuryPageCommand);
        }
    }
}