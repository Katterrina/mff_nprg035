using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.IO;

namespace EasyPattern
{
    class PatternGeometry
    {
        public void Skirt(int length, int up_circ, PdfPage page)
        {
            page.Orientation = PageOrientation.Landscape;
            page.Size = PageSize.A4;
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XBrush brush = new XSolidBrush();
            gfx.DrawEllipse(brush, 0, 0, 100, 100);
        }
    }
}
