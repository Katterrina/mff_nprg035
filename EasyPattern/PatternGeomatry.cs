using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.ComponentModel;

namespace EasyPattern
{
    //https://www.slideshare.net/franckblau1/metric-pattern-cutting-womenswear-winifred-aldrich
    public struct MeasuresData
    {
        public static double mm = 2.835;
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
        public int len_hips { get; }

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
                     int len_breast,
                     int len_hips)
        {
            this.height = (int)(height * mm);
            this.circ_bust = (int)(circ_bust * mm);
            this.circ_waist = (int)(circ_waist * mm);
            this.circ_hips = (int)(circ_hips * mm);
            this.len_back = (int)(len_back * mm);
            this.wid_back = (int)(wid_back * mm);
            this.len_knee = (int)(len_knee * mm);
            this.len_sleeve = (int)(len_sleeve * mm);
            this.len_shoulder = (int)(len_shoulder * mm);
            this.circ_neck = (int)(circ_neck * mm);
            this.len_hips = (int)(len_hips * mm);
            if (circ_sleeve == 0) { this.circ_sleeve = 22; } else { this.circ_sleeve = (int)(circ_sleeve * mm); }
            if (len_front == 0) { this.len_front = 0; } else { this.len_front = (int)(len_front * mm); } //TODO len_front
            if (len_breast == 0) { this.len_breast = 0;  } else { this.len_breast = (int)(len_breast * mm); } //TODO len_breast
        }
    }

    class PatternGeometry
    {
        public enum Pattern {[Description("rovná sukně")] straightSkirt, 
                             [Description("rozšířená sukně")] skirt,
                             [Description("šaty")] dress,  
                             [Description("košile")] shirt,
                             [Description("blůza")] blouse, 
                             [Description("rukáv")] sleeve }

        public static MeasuresData measures;

        public static string PdfPattern(Pattern pattern, string path)
        {
            string pathToPdf = path + "\\" + pattern.ToString() + ".pdf";
            CreatePdfDocument(pattern);

            switch (pattern)
            {
                case Pattern.straightSkirt:
                    straightSkirt();
                    break;
                case Pattern.skirt:
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

            SaveClose(pathToPdf);

            return pathToPdf;
        }

        static PdfDocument pdfDoc;
        static void CreatePdfDocument(Pattern p)
        {
            pdfDoc = new PdfDocument();
            pdfDoc.Info.Title = "EasyPattern";
            pdfDoc.Info.Author = "Kateřina Č.";
            pdfDoc.Info.Keywords = p.ToString();
        }


        static void SaveClose(string path)
        {
            pdfDoc.Save(path);
            pdfDoc.Close();
        }

        static PdfPage AddPatternPage(int patternHeight, int patternWidth)
        {
            PdfPage p = pdfDoc.AddPage();
            p.Size = PageSize.A0;

            if (patternWidth > patternHeight) 
            { 
                p.Orientation = PageOrientation.Landscape; 

                if (patternHeight < p.Width - 100 && patternWidth < p.Height - 100)
                {
                    p.Size = PageSize.A1;
                }
                else
                {
                    p.Size = PageSize.A0;
                }
            }
            else 
            { 
                p.Orientation = PageOrientation.Portrait;

                if (patternHeight < p.Height - 100 && patternWidth < p.Width - 100)
                {
                    p.Size = PageSize.A1;
                }
            }

            return p;
        }

        static void straightSkirt()
        {
            PdfPage p = AddPatternPage(measures.len_knee, measures.circ_hips / 2);
            XGraphics gfx = XGraphics.FromPdfPage(p);

            XPoint start = new XPoint(150, 200);
            XPoint hipsBack = new XPoint(start.X, start.Y + measures.len_hips);
            XPoint downBack = new XPoint(start.X, start.Y + measures.len_knee);

            int widthBack = measures.circ_hips / 4 ;
            int widthFront = measures.circ_hips / 4 + (int)(MeasuresData.mm * 15); // todo závislost přídavku na šířce
            int widthAll = widthBack + widthFront;

            XPoint upCenter = new XPoint(start.X + widthBack, start.Y - MeasuresData.mm * 10);
            XPoint hipsCenter = new XPoint(hipsBack.X + widthBack, hipsBack.Y);
            XPoint downCenter = new XPoint(downBack.X + widthBack, downBack.Y);

            XPoint upFront = new XPoint(start.X + widthAll, start.Y);
            XPoint hipsFront = new XPoint(start.X + widthAll, hipsBack.Y);
            XPoint downFront = new XPoint(start.X + widthAll, start.Y + measures.len_knee);

            XPen pen = XPens.Black;
            gfx.DrawLine(pen, start, downBack);
            gfx.DrawLine(pen, start, upFront);
            gfx.DrawLine(pen, downBack, downFront);
            gfx.DrawLine(pen, upFront, downFront);
            gfx.DrawLine(pen, upCenter, downCenter);
            gfx.DrawLine(pen, hipsBack, hipsFront);

            // todo záševky různá hloubka

            XPoint upFrontTuck = new XPoint(upFront.X - ((measures.circ_waist / 10) + MeasuresData.mm * 70), upFront.Y);
            XPoint downFrontTuck = new XPoint(upFrontTuck.X, upFrontTuck.Y + MeasuresData.mm * 90);

            int backTuckGap = widthBack / 3;

            XPoint upFirstBackTuck = new XPoint(start.X + backTuckGap, start.Y);
            XPoint downFirstBackTuck = new XPoint(upFirstBackTuck.X, upFirstBackTuck.Y + MeasuresData.mm * 140);

            XPoint upSecondBackTuck = new XPoint(start.X + backTuckGap * 2, start.Y);
            XPoint downSecondBackTuck = new XPoint(upSecondBackTuck.X, upSecondBackTuck.Y + MeasuresData.mm * 120);

            gfx.DrawLine(pen, upFrontTuck, downFrontTuck);
            gfx.DrawLine(pen, upFirstBackTuck, downFirstBackTuck);
            gfx.DrawLine(pen, upSecondBackTuck, downSecondBackTuck);


            // ------------------ TUCKS ---------------------------------------
            double allTucksWidth = (measures.circ_hips + (MeasuresData.mm * 10) - measures.circ_waist) / 2;
            double frontTuckWidth = allTucksWidth / 6;
            double backTuckWidth = 2 * allTucksWidth / 6;
            double hipsTuckWidth = allTucksWidth / 2;
            double hipsTuckBackWidth = 3 * hipsTuckWidth / 7;
            double hipsTuckFrontWidth = 4 * hipsTuckWidth / 7;

            XPoint hipsTuckBack = new XPoint(upCenter.X - hipsTuckBackWidth, upCenter.Y);
            XPoint hipsTuckFront = new XPoint(upCenter.X + hipsTuckFrontWidth, upCenter.Y);

            gfx.DrawLine(pen, hipsTuckBack, start);
            gfx.DrawLine(pen, hipsTuckFront, upFront);

            double widthBackMinusHipsTuck = widthBack - hipsTuckBackWidth;
            // first back tuck
            double hFBT1 = (backTuckGap - (backTuckWidth/2)) * (MeasuresData.mm * 10) / widthBackMinusHipsTuck;
            XPoint firstBackTuck1 = new XPoint(upFirstBackTuck.X - (backTuckWidth / 2), upFirstBackTuck.Y - hFBT1);
            gfx.DrawLine(pen, firstBackTuck1, downFirstBackTuck);

            double hFBT2 = (backTuckGap + (backTuckWidth / 2)) * (MeasuresData.mm * 10) / widthBackMinusHipsTuck;
            XPoint firstBackTuck2 = new XPoint(upFirstBackTuck.X + (backTuckWidth / 2), upFirstBackTuck.Y - hFBT2);
            gfx.DrawLine(pen, firstBackTuck2, downFirstBackTuck);

            // second back tuck
            double hSBT1 = (backTuckGap * 2 - (backTuckWidth / 2)) * (MeasuresData.mm * 10) / widthBackMinusHipsTuck;
            XPoint secondBackTuck1 = new XPoint(upSecondBackTuck.X - (backTuckWidth / 2), upSecondBackTuck.Y - hSBT1);
            gfx.DrawLine(pen, secondBackTuck1, downSecondBackTuck);

            double hSBT2 = (backTuckGap * 2 + (backTuckWidth / 2)) * (MeasuresData.mm * 10) / widthBackMinusHipsTuck;
            XPoint secondBackTuck2 = new XPoint(upSecondBackTuck.X + (backTuckWidth / 2), upSecondBackTuck.Y - hSBT2);
            gfx.DrawLine(pen, secondBackTuck2, downSecondBackTuck);

            double frontWidthWithoutTuck = widthFront - hipsTuckFrontWidth;
            // front tuck
            double hFT1 = (upFront.X - upFrontTuck.X + (frontTuckWidth/2)) * (MeasuresData.mm * 10) / frontWidthWithoutTuck;
            XPoint frontTuck1 = new XPoint(upFrontTuck.X - (frontTuckWidth / 2), upFrontTuck.Y - hFT1);
            gfx.DrawLine(pen, frontTuck1, downFrontTuck);

            double hFT2 = (upFront.X - upFrontTuck.X - (frontTuckWidth / 2)) * (MeasuresData.mm * 10) / frontWidthWithoutTuck;
            XPoint frontTuck2 = new XPoint(upFrontTuck.X + (frontTuckWidth / 2), upFrontTuck.Y - hFT2);
            gfx.DrawLine(pen, frontTuck2, downFrontTuck);

            // hips tuck
            XPoint hipsTuckDown = new XPoint(hipsCenter.X, hipsCenter.Y - hipsTuckFrontWidth);
            XPoint hipsBezier = new XPoint(hipsCenter.X - (MeasuresData.mm * 10), hipsCenter.Y - (2*measures.len_hips / 3));
            gfx.DrawBezier(pen, hipsTuckBack, hipsBezier, hipsBezier, hipsTuckDown);

            //string test = "záševek "+ hFBT1.ToString() + "backTuckGap " + backTuckGap.ToString() + " widthB " + widthBack.ToString() + " odečet " + (MeasuresData.mm * 15).ToString();

            //XFont font = new XFont("Verdana", 20, XFontStyle.BoldItalic);
            ////// Draw the text
            //gfx.DrawString(test, font, XBrushes.Black,
            //  new XRect(0, 0, p.Width, p.Height),
            //  XStringFormats.Center);
        }
    }
}
