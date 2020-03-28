using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.ComponentModel;
using System.IO;

namespace EasyPattern
{
    //https://www.slideshare.net/franckblau1/metric-pattern-cutting-womenswear-winifred-aldrich
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
            this.len_hips = len_hips;
            if (circ_sleeve == 0) { this.circ_sleeve = 22; } else { this.circ_sleeve = circ_sleeve; }
            if (len_front == 0) { this.len_front = 0; } else { this.len_front = len_front; } //TODO len_front
            if (len_breast == 0) { this.len_breast = 0;  } else { this.len_breast = len_breast; } //TODO len_breast
        }
    }

    class PatternGeometry
    {
        public enum Pattern {[Description("rovná sukně")] straightSkirt, 
                             [Description("rozšířená sukně")] wideSkirt,
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
                case Pattern.wideSkirt:
                    wideSkirt();
                    break;
                case Pattern.dress:
                    break;
                case Pattern.shirt:
                    Shirt();
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

        static PdfPage AddPatternPage(double patternHeight, double patternWidth)
        {
            PdfPage p = pdfDoc.AddPage();
            p.Size = PageSize.A0;

            if (patternWidth > patternHeight) 
            { 
                p.Orientation = PageOrientation.Landscape; 

                if (patternHeight < 820 && patternWidth < 550)
                {
                    p.Size = PageSize.A1;
                    
                }
            }
            else 
            { 
                p.Orientation = PageOrientation.Portrait;

                if (patternHeight < 550 && patternWidth < 8240)
                {
                    p.Size = PageSize.A1;
                }
            }

            return p;
        }

        static void wideSkirt()
        {
            double hipsAddition = 15;
            double downAddition = 40;

            PdfPage p = AddPatternPage(measures.len_knee, measures.circ_hips / 2 + hipsAddition + downAddition);
            XGraphics gfx = XGraphics.FromPdfPage(p, XGraphicsUnit.Millimeter);

            XPoint start = new XPoint(30, 30);
            XPoint hipsBack = new XPoint(start.X, start.Y + measures.len_hips);
            XPoint downBack = new XPoint(start.X, start.Y + measures.len_knee);

            double widthOnePart = measures.circ_hips / 4 + hipsAddition / 2;
            double widthAll = 2 * widthOnePart + downAddition;

            double plusUpHips = 10;

            XPoint upCenter1 = new XPoint(start.X + widthOnePart, start.Y - plusUpHips);
            XPoint hipsCenter1 = new XPoint(hipsBack.X + widthOnePart, hipsBack.Y);

            XPoint upCenter2 = new XPoint(start.X + widthOnePart + downAddition, start.Y - plusUpHips);
            XPoint hipsCenter2 = new XPoint(hipsBack.X + widthOnePart + downAddition, hipsBack.Y);

            XPoint upFront = new XPoint(start.X + widthAll, start.Y);
            XPoint hipsFront = new XPoint(start.X + widthAll, hipsBack.Y);
            XPoint downFront = new XPoint(start.X + widthAll, start.Y + measures.len_knee);

            XPen pen = XPens.Black;
            gfx.DrawLine(pen, start, downBack);
            gfx.DrawLine(pen, start, upFront);
            gfx.DrawLine(pen, downBack, downFront);
            gfx.DrawLine(pen, upFront, downFront);
            gfx.DrawLine(pen, upCenter1, hipsCenter1);
            gfx.DrawLine(pen, upCenter2, hipsCenter2);
            gfx.DrawLine(pen, hipsBack, hipsFront);

            // ------------------- TUCKS ------------------------

            double allTucksWidth = (measures.circ_hips + hipsAddition - measures.circ_waist) / 2;
            double frontTuckWidth = 3 * allTucksWidth / 14;
            double backTuckWidth = 4 * allTucksWidth / 14;
            double hipsTuckWidth = allTucksWidth / 2;

            XPoint upFrontTuck = new XPoint(upFront.X - widthOnePart / 3, upFront.Y);
            XPoint downFrontTuck = new XPoint(upFrontTuck.X, upFrontTuck.Y + measures.len_hips -30);

            XPoint upBackTuck = new XPoint(start.X + widthOnePart / 3, start.Y);
            XPoint downBackTuck = new XPoint(upBackTuck.X, upBackTuck.Y + measures.len_hips - 30);

            gfx.DrawLine(pen, upFrontTuck, downFrontTuck);
            gfx.DrawLine(pen, upBackTuck, downBackTuck);

            XPoint hipsTuckBack = new XPoint(upCenter1.X - (hipsTuckWidth / 2), upCenter1.Y);
            XPoint hipsTuckFront = new XPoint(upCenter2.X + (hipsTuckWidth / 2), upCenter2.Y);

            gfx.DrawLine(pen, start, hipsTuckBack);
            gfx.DrawLine(pen, upFront, hipsTuckFront);

            XPoint hipsBezierDown1 = new XPoint(hipsCenter1.X, hipsCenter1.Y - (measures.len_hips / 2));
            XPoint hipsBezierDown2 = new XPoint(hipsCenter2.X, hipsCenter2.Y - (measures.len_hips / 2));

            gfx.DrawBezier(pen,hipsTuckBack, hipsTuckBack, hipsBezierDown1, hipsCenter1);
            gfx.DrawBezier(pen, hipsTuckFront, hipsTuckFront, hipsBezierDown2, hipsCenter2);

            // front tuck

            double hFT1 = (upFront.X - upFrontTuck.X + (frontTuckWidth / 2)) * plusUpHips / widthOnePart;
            XPoint frontTuck1 = new XPoint(upFrontTuck.X - (frontTuckWidth / 2), upFrontTuck.Y - hFT1);
            gfx.DrawLine(pen, frontTuck1, downFrontTuck);

            double hFT2 = (upFront.X - upFrontTuck.X - (frontTuckWidth / 2)) * plusUpHips / widthOnePart;
            XPoint frontTuck2 = new XPoint(upFrontTuck.X + (frontTuckWidth / 2), upFrontTuck.Y - hFT2);
            gfx.DrawLine(pen, frontTuck2, downFrontTuck);

            XPoint FTDownHem = new XPoint(downFrontTuck.X,downBack.Y);
            gfx.DrawLine(pen, FTDownHem, downFrontTuck);

            // back tuck


            double hBT1 = (upBackTuck.X + (backTuckWidth / 2) - start.X) * plusUpHips / widthOnePart;
            XPoint backTuck1 = new XPoint(upBackTuck.X + (backTuckWidth / 2), upBackTuck.Y - hBT1);
            gfx.DrawLine(pen, backTuck1, downBackTuck);

            double hBT2 = (upBackTuck.X - (backTuckWidth / 2) - start.X) * plusUpHips / widthOnePart;
            XPoint backTuck2 = new XPoint(upBackTuck.X - (backTuckWidth / 2), upBackTuck.Y - hBT2);
            gfx.DrawLine(pen, backTuck2, downBackTuck);

            XPoint BTDownHem = new XPoint(downBackTuck.X, downBack.Y);
            gfx.DrawLine(pen, BTDownHem, downBackTuck);

            // down edge


            XPoint downCenter = new XPoint(start.X + (widthAll/2),downBack.Y);
            gfx.DrawLine(pen, hipsCenter1, downCenter);
            gfx.DrawLine(pen, hipsCenter2, downCenter);
        }

        static void straightSkirt()
        {
            double hipsAddition = 20;

            PdfPage p = AddPatternPage(measures.len_knee, measures.circ_hips / 2 + hipsAddition);
            XGraphics gfx = XGraphics.FromPdfPage(p, XGraphicsUnit.Millimeter);

            XPoint start = new XPoint(30, 30);
            XPoint hipsBack = new XPoint(start.X, start.Y + measures.len_hips);
            XPoint downBack = new XPoint(start.X, start.Y + measures.len_knee);

            double widthBack = measures.circ_hips / 4 ;
            double widthFront = measures.circ_hips / 4 + hipsAddition;
            double widthAll = widthBack + widthFront;

            double plusUpHips = 10;

            XPoint upCenter = new XPoint(start.X + widthBack, start.Y - plusUpHips);
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


            double frontTuckDepth = 9 * measures.len_hips / 20;
            double firstBackTuckDepth = 14 * measures.len_hips / 20;
            double secondBackTuckDepth = 12 * measures.len_hips / 20;

            XPoint upFrontTuck = new XPoint(upFront.X - ((measures.circ_waist / 10) + 70), upFront.Y);
            XPoint downFrontTuck = new XPoint(upFrontTuck.X, upFrontTuck.Y + frontTuckDepth);

            double backTuckGap = widthBack / 3;

            XPoint upFirstBackTuck = new XPoint(start.X + backTuckGap, start.Y);
            XPoint downFirstBackTuck = new XPoint(upFirstBackTuck.X, upFirstBackTuck.Y + firstBackTuckDepth);

            XPoint upSecondBackTuck = new XPoint(start.X + backTuckGap * 2, start.Y);
            XPoint downSecondBackTuck = new XPoint(upSecondBackTuck.X, upSecondBackTuck.Y + secondBackTuckDepth);

            gfx.DrawLine(pen, upFrontTuck, downFrontTuck);
            gfx.DrawLine(pen, upFirstBackTuck, downFirstBackTuck);
            gfx.DrawLine(pen, upSecondBackTuck, downSecondBackTuck);


            // ------------------ TUCKS ---------------------------------------
            double allTucksWidth = (measures.circ_hips + hipsAddition - measures.circ_waist) / 2;
            double frontTuckWidth = allTucksWidth / 6;
            double backTuckWidth = allTucksWidth / 6;
            double hipsTuckWidth = allTucksWidth / 2;
            double hipsTuckBackWidth = 3 * hipsTuckWidth / 7;
            double hipsTuckFrontWidth = 4 * hipsTuckWidth / 7;

            double downTuck = 20;

            XPoint hipsTuckBack = new XPoint(upCenter.X - hipsTuckBackWidth, upCenter.Y);
            XPoint hipsTuckFront = new XPoint(upCenter.X + hipsTuckFrontWidth, upCenter.Y);

            gfx.DrawLine(pen, hipsTuckBack, start);
            gfx.DrawLine(pen, hipsTuckFront, upFront);

            double widthBackMinusHipsTuck = widthBack - hipsTuckBackWidth;
            // first back tuck
            double hFBT1 = (backTuckGap - (backTuckWidth/2)) * plusUpHips / widthBackMinusHipsTuck;
            XPoint firstBackTuck1 = new XPoint(upFirstBackTuck.X - (backTuckWidth / 2), upFirstBackTuck.Y - hFBT1);
            gfx.DrawLine(pen, firstBackTuck1, downFirstBackTuck);

            double hFBT2 = (backTuckGap + (backTuckWidth / 2)) * plusUpHips / widthBackMinusHipsTuck;
            XPoint firstBackTuck2 = new XPoint(upFirstBackTuck.X + (backTuckWidth / 2), upFirstBackTuck.Y - hFBT2);
            gfx.DrawLine(pen, firstBackTuck2, downFirstBackTuck);

            // second back tuck
            double hSBT1 = (backTuckGap * 2 - (backTuckWidth / 2)) * plusUpHips / widthBackMinusHipsTuck;
            XPoint secondBackTuck1 = new XPoint(upSecondBackTuck.X - (backTuckWidth / 2), upSecondBackTuck.Y - hSBT1);
            gfx.DrawLine(pen, secondBackTuck1, downSecondBackTuck);

            double hSBT2 = (backTuckGap * 2 + (backTuckWidth / 2)) * plusUpHips / widthBackMinusHipsTuck;
            XPoint secondBackTuck2 = new XPoint(upSecondBackTuck.X + (backTuckWidth / 2), upSecondBackTuck.Y - hSBT2);
            gfx.DrawLine(pen, secondBackTuck2, downSecondBackTuck);

            double frontWidthWithoutTuck = widthFront - hipsTuckFrontWidth;
            // front tuck
            double hFT1 = (upFront.X - upFrontTuck.X + (frontTuckWidth/2)) * plusUpHips / frontWidthWithoutTuck;
            XPoint frontTuck1 = new XPoint(upFrontTuck.X - (frontTuckWidth / 2), upFrontTuck.Y - hFT1);
            gfx.DrawLine(pen, frontTuck1, downFrontTuck);

            double hFT2 = (upFront.X - upFrontTuck.X - (frontTuckWidth / 2)) * plusUpHips / frontWidthWithoutTuck;
            XPoint frontTuck2 = new XPoint(upFrontTuck.X + (frontTuckWidth / 2), upFrontTuck.Y - hFT2);
            gfx.DrawLine(pen, frontTuck2, downFrontTuck);

            // hips tuck
            XPoint hipsTuckDown = new XPoint(hipsCenter.X, hipsCenter.Y - hipsTuckFrontWidth);

            XPoint hipsBackBezierUp = new XPoint(hipsCenter.X - (hipsTuckBackWidth / 2), hipsCenter.Y - (3 * measures.len_hips / 4));
            XPoint hipsBackBezierDown = new XPoint(hipsCenter.X - (hipsTuckBackWidth / 10), hipsCenter.Y - (measures.len_hips / 3));
            gfx.DrawBezier(pen, hipsTuckBack, hipsBackBezierUp, hipsBackBezierDown, hipsTuckDown);

            XPoint hipsFrontBezierUp = new XPoint(hipsCenter.X + (hipsTuckFrontWidth / 2), hipsCenter.Y - (3 * measures.len_hips / 4));
            XPoint hipsFrontBezierDown = new XPoint(hipsCenter.X + (hipsTuckFrontWidth / 10), hipsCenter.Y - (measures.len_hips / 3));
            gfx.DrawBezier(pen, hipsTuckFront, hipsFrontBezierUp, hipsFrontBezierDown, hipsTuckDown);


            // down tuck
            XPoint downTuckBack = new XPoint(downCenter.X - (downTuck / 2), downCenter.Y);
            XPoint downTuckFront = new XPoint(downCenter.X + (downTuck / 2), downCenter.Y);
            gfx.DrawLine(pen,hipsCenter, downTuckBack);
            gfx.DrawLine(pen, hipsCenter, downTuckFront);


            //string test = p.Size.ToString();

            //XFont font = new XFont("Verdana", 20, XFontStyle.BoldItalic);
            ////// Draw the text
            //gfx.DrawString(test, font, XBrushes.Black,
            //  new XRect(0, 0, p.Width, p.Height),
            //  XStringFormats.Center);
        }

        static void Shirt()
        {

            // prepare measures

            double circAddition;
            double frontNeckUpAddition = measures.circ_bust / 20;

            if (measures.circ_bust < 820) { circAddition = 60; frontNeckUpAddition -= 10; }
            else if (measures.circ_bust < 870) { circAddition = 65; frontNeckUpAddition -= 5; }
            else if (measures.circ_bust < 920) { circAddition = 70; }
            else { circAddition = 75; }

            double armpitBackLength = measures.height / 10 + measures.circ_bust / 20 + 3;

            double backWidth = measures.circ_bust / 8 + 55 + 3 * circAddition / 13;
            double armholeWidth = measures.circ_bust / 8 - 15 + 6 * circAddition / 13;
            double frontWidth = measures.circ_bust / 4 - 40 + 4 * circAddition / 13;

            double allWidth = backWidth + armholeWidth + frontWidth;


            // net
            PdfPage p = AddPatternPage(measures.len_back+measures.len_hips+ armpitBackLength, allWidth);
            XGraphics gfx = XGraphics.FromPdfPage(p, XGraphicsUnit.Millimeter);
            XPen pen = XPens.Black;

            XPoint start = new XPoint(30, 30 + frontNeckUpAddition);
            XPoint backBreast = new XPoint(start.X, start.Y + armpitBackLength);
            XPoint backWaist = new XPoint(start.X, start.Y + measures.len_back);
            XPoint backHips = new XPoint(start.X, start.Y + measures.len_back + measures.len_hips);
            XPoint backDeflection = new XPoint(backHips.X + 20, backHips.Y);

            XPoint backArmholeBreast = new XPoint(backBreast.X + backWidth, backBreast.Y);
            XPoint centerBreast = new XPoint(backArmholeBreast.X + 2 * armholeWidth / 3, backArmholeBreast.Y);
            XPoint frontArmholeBreast = new XPoint(backBreast.X + backWidth + armholeWidth, backBreast.Y);
            XPoint frontBreast = new XPoint(backBreast.X + allWidth, backBreast.Y);

            XPoint backArmholeNeck = new XPoint(backArmholeBreast.X, start.Y);
            XPoint frontArmholeNeck = new XPoint(frontArmholeBreast.X, start.Y - frontNeckUpAddition);

            XPoint frontNeck = new XPoint(frontBreast.X, frontArmholeNeck.Y);

            XPoint frontWaist = new XPoint(frontBreast.X, backWaist.Y);
            XPoint frontHips = new XPoint(frontBreast.X, backHips.Y);

            XPoint centerWaist = new XPoint(centerBreast.X, backWaist.Y);
            XPoint centerHips = new XPoint(centerBreast.X, backHips.Y);

            gfx.DrawLine(pen, start, backHips);
            gfx.DrawLine(pen, backArmholeNeck, backArmholeBreast);
            gfx.DrawLine(pen, frontArmholeNeck, frontArmholeBreast);
            gfx.DrawLine(pen, centerBreast, centerHips);
            gfx.DrawLine(pen, frontNeck, frontHips);

            gfx.DrawLine(pen, start, backArmholeNeck);
            gfx.DrawLine(pen, frontArmholeNeck, frontNeck);
            gfx.DrawLine(pen, backBreast, frontBreast);
            gfx.DrawLine(pen, backWaist, frontWaist);
            gfx.DrawLine(pen, backHips, frontHips);

            // final shape
            double backNechHoleHeight = 20;
            double backNeckHoleWidth = measures.circ_bust / 20 + backNechHoleHeight;
            XPoint backNeckHole = new XPoint(start.X + backNeckHoleWidth, start.Y - 20);

            gfx.DrawArc(pen, new XRect(start.X - backNeckHoleWidth, start.Y - 2 * backNechHoleHeight, backNeckHoleWidth * 2, backNechHoleHeight * 2),0,90);

            XPoint backArmholeIntersectio = new XPoint(backArmholeNeck.X, backArmholeNeck.Y + 10);

            armpitBackLength -= 10;

            // draw curve

        }
    }
}
