using System.IO;
using System.IO.Packaging;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Xps.Packaging;

namespace BookKeeping.App.Exporters
{
    public class PdfExporter : IExporter
    {
        public virtual void Export(DocumentPaginator documentPaginator, string fileName)
        {
            var stream = new MemoryStream();
            using (var package = Package.Open(stream, FileMode.Create))
            using (var xpsDocument = new XpsDocument(package))
            {
                var writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
                writer.Write(documentPaginator);
            }
            var pdfXpsDocument = PdfSharp.Xps.XpsModel.XpsDocument.Open(stream);
            PdfSharp.Xps.XpsConverter.Convert(pdfXpsDocument, fileName, 0);
            stream.Close();
        }

        public virtual void Export(Visual visual, string fileName)
        {
            var stream = new MemoryStream();
            using (var package = Package.Open(stream, FileMode.Create))
            using (var xpsDocument = new XpsDocument(package))
            {
                var writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
                writer.Write(visual);
            }
            var pdfXpsDocument = PdfSharp.Xps.XpsModel.XpsDocument.Open(stream);
            PdfSharp.Xps.XpsConverter.Convert(pdfXpsDocument, fileName, 0);
            stream.Close();
        }

        public void Export(Stream source, string fileName)
        {
            throw new System.NotImplementedException();
        }
    }
}
