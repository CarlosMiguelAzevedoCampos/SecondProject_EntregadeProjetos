using CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces;

namespace CMA.ISMAI.Delivery.FileLoading.CrossCutting.FileVerifier
{
    public class FileVerifierService : IFileVerifierService
    {
        private readonly IPDFVerifierService _pdfVerifierService;
        private readonly IMediaFileVerifierService _audioVerifierService;


        public FileVerifierService(IPDFVerifierService pdfVerifierService, IMediaFileVerifierService audioVerifierService)
        {
            _pdfVerifierService = pdfVerifierService;
            _audioVerifierService = audioVerifierService;
        }

        public bool VerifyIfFilesAreCorrupted(string filePath)
        {
            if (!_pdfVerifierService.ArePdfFilesOk(filePath))
                return true;
            if (!_audioVerifierService.AreMediaFilesOk(filePath))
                return true;
            return false;
        }
    }
}
