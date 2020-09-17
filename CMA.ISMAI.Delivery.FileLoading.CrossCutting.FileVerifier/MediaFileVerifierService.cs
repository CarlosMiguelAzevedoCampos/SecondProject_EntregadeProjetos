using CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces;
using CMA.ISMAI.Delivery.Logging.Interface;
using NReco.VideoInfo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CMA.ISMAI.Delivery.FileLoading.CrossCutting.FileVerifier
{
    public class MediaFileVerifierService : IMediaFileVerifierService
    {
        private readonly ILoggingService _log;

        public MediaFileVerifierService(ILoggingService log)
        {
            _log = log;
        }

        public bool AreMediaFilesOk(string filePath)
        {
            try
            {
                List<string> files = ReturnAllFilesInADirectory(filePath);
                foreach (var item in files)
                {
                    FileInfo info = new FileInfo(item);
                    var ffProbe = new FFProbe();
                    var mediaInfo = ffProbe.GetMediaInfo(item);
                    if(mediaInfo.Duration == TimeSpan.Zero)
                        return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _log.Fatal(ex.ToString());
            }
            return false;
        }

        private List<string> ReturnAllFilesInADirectory(string filepath)
        {
            string[] mediaExtensions = {
                    "*.WAV","*.WMA", "*.MP3", //etc
                    "*.AVI", "*.MP4", "*.WMV", //etc
                };
            List<string> files = new List<string>();
            foreach (var item in mediaExtensions)
            {
                files.AddRange(Directory.GetFiles(filepath, item, SearchOption.AllDirectories).ToList());
            }
            return files;
        }
    }
}
