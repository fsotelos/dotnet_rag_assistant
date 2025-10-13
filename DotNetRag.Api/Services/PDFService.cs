using System.IO;


namespace DotNetRag.Api.Services
{
    public class PDFService
    {
        public static byte[] ExtractContentFromPDF(string filePath)
        {
            var pdf = IronPdf.PdfDocument.FromFile(filePath);

            //get text from pdf
            var text = pdf.ExtractAllText();

            return System.Text.Encoding.UTF8.GetBytes(text);

        }
    }
}