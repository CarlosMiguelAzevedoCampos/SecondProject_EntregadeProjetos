using CMA.ISMAI.Delivery.FileProcessing.Domain.Interfaces;
using CMA.ISMAI.Delivery.Logging.Interface;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;

namespace CMA.ISMAI.Delivery.FileProcessing.CrossCutting.FileProcessing
{
    public class CoverPageService : ICoverPageService
    {
        private readonly ILoggingService _log;

        public CoverPageService(ILoggingService log)
        {
            _log = log;
        }

        public bool AddCoverPage(string basepath, string title, string studentName, string cordenatorName, string contextOfDelivery)
        {
            try
            {
                PdfDocument pdfDocPrivate = new PdfDocument(new PdfReader(string.Format(@"{0}\PrivateWaterMark.pdf", basepath)),
                    new PdfWriter(string.Format(@"{0}\{1}", basepath, "PrivateCover.pdf")));

                PrepareToAddCover(title, studentName, cordenatorName, contextOfDelivery, pdfDocPrivate);

                PdfDocument pdfDocPublic = new PdfDocument(new PdfReader(string.Format(@"{0}\PublicWaterMark.pdf", basepath)),
                   new PdfWriter(string.Format(@"{0}\{1}", basepath, "PublicCover.pdf")));
                PrepareToAddCover(title, studentName, cordenatorName, contextOfDelivery, pdfDocPublic);

                
                return true;
            }
            catch (Exception ex)
            {
                _log.Fatal(ex.ToString());
            }
            return false;
        }

        private void PrepareToAddCover(string title, string studentName, string cordenatorName, string contextOfDelivery, PdfDocument pdfDoc)
        {
            Document doc = new Document(pdfDoc);
            var pdfPage = pdfDoc.AddNewPage(2);
           Rectangle pageSize = pdfPage.GetPageSize();


            AddParagraph(title, pdfDoc, doc, (pageSize.GetLeft() + pageSize.GetRight()) / 2, (pageSize.GetLeft() + pageSize.GetRight()) / 2);
            AddParagraph(contextOfDelivery, pdfDoc, doc, (pageSize.GetLeft() + pageSize.GetRight()) / 2, (pageSize.GetLeft() + pageSize.GetRight()) / 3);
            AddParagraph(studentName, pdfDoc, doc, (pageSize.GetLeft() + pageSize.GetRight()) / 3, (pageSize.GetLeft() + pageSize.GetRight()) / 5);
            AddParagraph(cordenatorName, pdfDoc, doc, (pageSize.GetLeft() + pageSize.GetRight()) / 3, (pageSize.GetLeft() + pageSize.GetRight()) / 6);
            AddLogoImage(pdfDoc, pageSize);

            doc.Close();
        }

        private void AddLogoImage(PdfDocument pdfDoc, Rectangle pageSize)
        {
            ImageData img = ImageDataFactory.Create(string.Format(@"{0}\\Image\ismai.png", Environment.CurrentDirectory));
            float w = img.GetWidth() / 2;
            float h = img.GetHeight() / 2;
            var pdfPage = pdfDoc.GetPage(2);
            float x = (pageSize.GetLeft() + pageSize.GetRight()) / 2;
            float y = pageSize.GetTop() - 100;
            PdfCanvas over = new PdfCanvas(pdfPage);
            over.AddImage(img, w, 0, 0, h, x - (w / 2), y - (h / 2), false);
            over.SaveState();
        }

        private void AddParagraph(string title, PdfDocument pdfDoc, Document doc, float x, float y)
        {
            PdfFont font = PdfFontFactory.CreateFont(FontProgramFactory.CreateFont(StandardFonts.HELVETICA));

            Paragraph paragraph = new Paragraph(title).SetFont(font).SetFontSize(12);
            var pdfPage = pdfDoc.GetPage(2);

            PdfCanvas over = new PdfCanvas(pdfPage);
            over.SaveState();
            _ = doc.ShowTextAligned(paragraph, x, y, 2, TextAlignment.CENTER, VerticalAlignment.TOP, 0);
            over.RestoreState();
        }
    }
}
