using CMA.ISMAI.Delivery.FileProcessing.Domain.Interfaces;
using CMA.ISMAI.Delivery.Logging.Interface;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.Collections.Generic;

namespace CMA.ISMAI.Delivery.FileProcessing.CrossCutting.FileProcessing
{
    public class GenerateJuryPageService : IGenerateJuryPageService
    {
        private readonly ILoggingService _log;

        public GenerateJuryPageService(ILoggingService log)
        {
            _log = log;
        }

        public bool AddJuryPage(string filePath, List<string> jury)
        {
            try
            {

                GenerateJuryPage(string.Format(@"{0}\PrivateCover.pdf", filePath), string.Format(@"{0}\{1}", filePath, "FinalPrivate_Delivery.pdf"), jury);

                GenerateJuryPage(string.Format(@"{0}\PublicCover.pdf", filePath), string.Format(@"{0}\{1}", filePath, "FinalPublic_Delivery.pdf"), jury);

                return true;
            }
            catch (Exception ex)
            {
                _log.Fatal(ex.ToString());
            }
            return false;
        }

        private void GenerateJuryPage(string pathToFile, string newFile, List<string> text)
        {
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(pathToFile),
                new PdfWriter(newFile));
            Document doc = new Document(pdfDoc);
            var pdfPage = pdfDoc.AddNewPage(3);
            PdfFont font = PdfFontFactory.CreateFont(FontProgramFactory.CreateFont(StandardFonts.HELVETICA));
            WriteParagraph(doc, pdfPage, font, 0, "Composição do Júri:");
            int i = 30;
            foreach (var item in text)
            {
                WriteParagraph(doc, pdfPage, font, i, item);
                i += 30;
            }
            doc.Close();
        }

        private static void WriteParagraph(Document doc, PdfPage pdfPage, PdfFont font, int i, string item)
        {
            Paragraph paragraph = new Paragraph(item).SetFont(font).SetFontSize(12);
            Rectangle pageSize = pdfPage.GetPageSize();
            float x = (pageSize.GetLeft() + pageSize.GetRight()) / 2;
            float y = pageSize.GetTop() - (100 + i);
            PdfCanvas over = new PdfCanvas(pdfPage);
            over.SaveState();
            _ = doc.ShowTextAligned(paragraph, x, y, 3, TextAlignment.CENTER, VerticalAlignment.TOP, 0);
            over.RestoreState();
        }
    }
}
