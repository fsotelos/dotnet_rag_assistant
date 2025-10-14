using Docnet.Core;
using Docnet.Core.Models;
using System.Text;


namespace DotNetRag.Api.Services
{
    public class PDFService
    {
        public static string ExtractTextFromPDF(byte[] content)
        {
            using (var docReader = DocLib.Instance.GetDocReader(content, new PageDimensions()))
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (var i = 0; i < docReader.GetPageCount(); i++)
                {
                    using (var pageReader = docReader.GetPageReader(i))
                    {
                        var text = pageReader.GetText();
                        stringBuilder.AppendLine(text);
                    }
                }
                return stringBuilder.ToString();
            }

        }
    }
}