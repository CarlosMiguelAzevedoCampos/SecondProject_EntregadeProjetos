using CMA.ISMAI.Delivery.FileLoading.Domain.Interfaces;
using NReco.VideoInfo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CMA.ISMAI.Delivery.FileLoading.CrossCutting.FileVerifier
{
    public class MediaFileVerifierService : IMediaFileVerifierService
    {
        public bool AreMediaFilesOk(string filePath)
        {
            try
            {
                List<string> files = ReturnAllFilesInADirectory(filePath);
                foreach (var item in files)
                {
                    FileInfo info = new FileInfo(item);
                    var ffProbe = new FFProbe();
                    var mediaInfo = ffProbe.GetMediaInfo(filePath);
                    if(mediaInfo.Duration == TimeSpan.Zero)
                        return false;
                }
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        private List<string> ReturnAllFilesInADirectory(string filepath)
        {
            string[] mediaExtensions = {
                    ".WAV", ".MID", ".MIDI", ".WMA", ".MP3", ".OGG", ".RMA", //etc
                    ".AVI", ".MP4", ".DIVX", ".WMV", //etc
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
