using CMA.ISMAI.Delivery.FileLoading.Domain.Model;
using NetDevPack.Mediator;
using System;

namespace CMA.ISMAI.Delivery.FileLoading.UI
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
            VerifyFilesCommand createFileIdentifiersCommand = new VerifyFilesCommand(Guid.NewGuid(), @"C:\Users\Carlos Campos\Downloads\x.mp3");

            this._mediatr.SendCommand<VerifyFilesCommand>(createFileIdentifiersCommand);
        }
    }
}