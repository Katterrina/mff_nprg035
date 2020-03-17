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
    public struct MeasuresData
    {
        public int height { get; }
        public int circ_bust { get; }
        public int circ_waist { get; }
        public int circ_hips { get; }
        public int len_back { get; }
        public int wid_back { get; }
        public int len_knee { get; }
        public int len_shoulder { get; }
        public int len_sleeve { get; }
        public int circ_neck { get; }
        public int circ_sleeve { get; }
        public int len_front { get; }
        public int len_breast { get; }

        public MeasuresData(int height,
                     int circ_bust,
                     int circ_waist,
                     int circ_hips,
                     int len_back,
                     int wid_back,
                     int len_knee,
                     int len_shoulder,
                     int len_sleeve,
                     int circ_neck,
                     int circ_sleeve,
                     int len_front,
                     int len_breast)
        {
            this.height = height;
            this.circ_bust = circ_bust;
            this.circ_waist = circ_waist;
            this.circ_hips = circ_hips;
            this.len_back = len_back;
            this.wid_back = wid_back;
            this.len_knee = len_knee;
            this.len_sleeve = len_sleeve;
            this.len_shoulder = len_shoulder;
            this.circ_neck = circ_neck;
            if (circ_sleeve == 0) { this.circ_sleeve = 22; } else { this.circ_sleeve = circ_sleeve; }
            if (len_front == 0) { this.len_front = 0; } else { this.len_front = len_front; } //TODO len_front
            if (len_breast == 0) { this.len_breast = 0;  } else { this.len_breast = len_breast; } //TODO len_breast
        }
    }

    class PatternGeometry
    {
        public enum Pattern {[Description("sukně")] skirt, 
                             [Description("šaty")] dress,  
                             [Description("košile")] shirt,
                             [Description("blůza")] blouse, 
                             [Description("rukáv")] sleeve }

        public static MeasuresData measures;

        public static void PdfPattern(Pattern pattern, string path)
        {
            PdfDocument pdfDoc = CreatePdfPage(pattern);

            switch (pattern)
            {
                case Pattern.skirt:
                    Skirt(pdfDoc);
                    break;
                case Pattern.dress:
                    break;
                case Pattern.shirt:
                    break;
                case Pattern.blouse:
                    break;
                case Pattern.sleeve:
                    break;
                default:
                    break;
            }
        }
        static PdfDocument CreatePdfPage(Pattern p)
        {
            PdfDocument doc = new PdfDocument();
            doc.Info.Title = "EasyPattern";
            doc.Info.Author = "Kateřina Č.";
            doc.Info.Keywords = p.ToString();
            return doc;
        }

        static void Skirt(PdfDocument pdfDoc)
        {
            PdfPage page = pdfDoc.AddPage();
            page.Orientation = PageOrientation.Landscape;
            page.Size = PageSize.A4;
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XBrush brush = new XSolidBrush();
            gfx.DrawEllipse(brush, 0, 0, 100, 100);
        }
    }
}
