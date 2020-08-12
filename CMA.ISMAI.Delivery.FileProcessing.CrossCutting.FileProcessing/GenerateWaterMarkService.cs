using CMA.ISMAI.Delivery.FileProcessing.Domain.Interfaces;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.IO;

namespace CMA.ISMAI.Delivery.FileProcessing.CrossCutting.FileProcessing
{
    public class GenerateWaterMarkService : IGenerateWaterMarkService
    {
        public bool AddWaterMark(string path, string publicFile, string privateFile)
        {
            try
            {
                GenerateWaterMark(string.Format(@"{0}\{1}", path, privateFile), string.Format(@"{0}\{1}", path, "PrivateWaterMark.pdf"), "Private evaluation File");

                GenerateWaterMark(string.Format(@"{0}\{1}", path, publicFile), string.Format(@"{0}\{1}", path, "PublicWaterMark.pdf"), "Public evaluation File");

                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        private void GenerateWaterMark(string pathToFile, string newFile, string text)
        {
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(pathToFile),
                new PdfWriter(newFile));
            Document doc = new Document(pdfDoc);
            PdfFont font = PdfFontFactory.CreateFont(FontProgramFactory.CreateFont(StandardFonts.HELVETICA));
            Paragraph paragraph = new Paragraph(text).SetFont(font).SetFontSize(30);

            PdfExtGState gs1 = new PdfExtGState().SetFillOpacity(0.5f);

            // Implement transformation matrix usage in order to scale image
            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                PdfPage pdfPage = pdfDoc.GetPage(i);
                iText.Kernel.Geom.Rectangle pageSize = pdfPage.GetPageSize();
                float x = (pageSize.GetLeft() + pageSize.GetRight()) / 2;
                float y = (pageSize.GetTop() + pageSize.GetBottom()) / 2;
                PdfCanvas over = new PdfCanvas(pdfPage);
                over.SaveState();
                over.SetExtGState(gs1);
                _ = doc.ShowTextAligned(paragraph, x, y, i, TextAlignment.CENTER, VerticalAlignment.TOP, 0);

                over.RestoreState();
            }

            doc.Close();
        }
    }
}
