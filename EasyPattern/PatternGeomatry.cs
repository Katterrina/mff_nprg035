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
            PreparePen();

            switch (pattern)
            {
                case Pattern.straightSkirt:
                    straightSkirt();
                    break;
                case Pattern.wideSkirt:
                    wideSkirt();
                    break;
                case Pattern.dress:
                    Dress();
                    break;
                case Pattern.shirt:
                    Shirt();
                    break;
                case Pattern.blouse:
                    Blouse();
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

        static XPen pen;
        static XPen dashed;

        static void PreparePen()
        {
            pen = XPens.Black;
            dashed = new XPen(XColors.Black, 1);
            dashed.DashStyle = XDashStyle.Dash;
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

            double armpitLength = measures.height / 10 + measures.circ_bust / 20 + 3;

            double backWidth = measures.circ_bust / 8 + 55 + 3 * circAddition / 13;
            double armholeWidth = measures.circ_bust / 8 - 15 + 6 * circAddition / 13;
            double frontWidth = measures.circ_bust / 4 - 40 + 4 * circAddition / 13;

            double allWidth = backWidth + armholeWidth + frontWidth;


            // prepare pdf

            PdfPage p = AddPatternPage(measures.len_back+measures.len_hips+ armpitLength, allWidth);
            XGraphics gfx = XGraphics.FromPdfPage(p, XGraphicsUnit.Millimeter);

            // net

            XPoint start = new XPoint(30, 30 + frontNeckUpAddition);
            XPoint backBreast = new XPoint(start.X, start.Y + armpitLength);
            XPoint backWaist = new XPoint(start.X, start.Y + measures.len_back);
            XPoint backHips = new XPoint(start.X, start.Y + measures.len_back + measures.len_hips);
            XPoint backDeflection = new XPoint(backHips.X + 20, backHips.Y);

            XPoint backArmholeBreast = new XPoint(backBreast.X + backWidth, backBreast.Y);
            XPoint centerBreast = new XPoint(backArmholeBreast.X + 2 * armholeWidth / 3, backArmholeBreast.Y);
            XPoint frontArmholeBreast = new XPoint(backBreast.X + backWidth + armholeWidth, backBreast.Y);
            XPoint frontBreast = new XPoint(backBreast.X + allWidth, backBreast.Y);

            XPoint backArmholeShoulder = new XPoint(backArmholeBreast.X, start.Y);
            XPoint frontArmholeNeck = new XPoint(frontArmholeBreast.X, start.Y - frontNeckUpAddition);

            XPoint frontNeck = new XPoint(frontBreast.X, frontArmholeNeck.Y);

            XPoint frontWaist = new XPoint(frontBreast.X, backWaist.Y);
            XPoint frontHips = new XPoint(frontBreast.X, backHips.Y);

            XPoint centerWaist = new XPoint(centerBreast.X, backWaist.Y);
            XPoint centerHips = new XPoint(centerBreast.X, backHips.Y);

            gfx.DrawLine(pen, start, backHips);
            gfx.DrawLine(pen, backArmholeShoulder, backArmholeBreast);
            gfx.DrawLine(pen, frontArmholeNeck, frontArmholeBreast);
            gfx.DrawLine(pen, centerBreast, centerHips);
            gfx.DrawLine(pen, frontNeck, frontHips);
            gfx.DrawLine(pen, start, backDeflection);

            gfx.DrawLine(pen, start, backArmholeShoulder);
            gfx.DrawLine(pen, frontArmholeNeck, frontNeck);
            gfx.DrawLine(pen, backBreast, frontBreast);
            gfx.DrawLine(pen, backWaist, frontWaist);
            gfx.DrawLine(pen, backHips, frontHips);

            // final shape

            // back neck hole
            double backNeckHoleHeight = 20;
            double neckHoleWidth = measures.circ_bust / 20 + 23;
            XPoint backNeckHole = new XPoint(start.X + neckHoleWidth, start.Y - 20);

            // back shoulder
            double intersectionDown = 10;
            XPoint backArmholeIntersection = new XPoint(backArmholeShoulder.X, backArmholeShoulder.Y + intersectionDown);
            
            double lenAfterIntersection = 30;
            double a = Math.Atan((intersectionDown + backNeckHoleHeight) / (backWidth - neckHoleWidth));

            double backShoulderYpos = backArmholeIntersection.Y + (Math.Sin(a) * lenAfterIntersection);
            double backShoulderXpos = backArmholeIntersection.X + (Math.Cos(a) * lenAfterIntersection);

            XPoint backShoulder = new XPoint(backShoulderXpos, backShoulderYpos);

            armpitLength -= intersectionDown;

            // front neck hole
            double neckHoleHeight = measures.circ_bust / 20 + 40;
            XPoint frontUpNeckHole = new XPoint(frontNeck.X - neckHoleWidth, frontNeck.Y);

            // front shoulder
            double lenShoulder = XPoint.Subtract(backNeckHole, backShoulder).Length;

            XPoint frontShoulder = ShoulderCircleIntersection(frontArmholeBreast, frontUpNeckHole, armpitLength, lenShoulder);

            // back

            XPoint armhole1 = new XPoint(backArmholeShoulder.X + 10, backArmholeIntersection.Y + (armpitLength / 2));
            XPoint armhole2 = new XPoint(backArmholeShoulder.X + 15, backArmholeIntersection.Y + ((3 * armpitLength) / 4));

            XPoint armhole3 = new XPoint(backArmholeShoulder.X + (centerBreast.X - backArmholeShoulder.X) /2 , backArmholeIntersection.Y + ((19 * armpitLength) / 20));

            // fornt
            XPoint armhole4 = new XPoint(frontArmholeBreast.X, frontArmholeBreast.Y - 13);
            XPoint armhole5;

            if (frontShoulder.X > frontArmholeBreast.X) 
            { armhole5 = new XPoint(frontShoulder.X + 10, frontArmholeBreast.Y - (armpitLength / 2));}
            else
            {armhole5 = new XPoint(frontArmholeBreast.X + 13, frontArmholeBreast.Y - (armpitLength / 2)); }

            // translate shoulder seam
            ShiftLineParallel(ref frontShoulder, ref frontUpNeckHole, gfx, pen, -10);
            ShiftLineParallel(ref backShoulder, ref backNeckHole, gfx, pen, -10);

            //draw neck hole - using shifted shnoulder seam
            neckHoleWidth = backNeckHole.X - start.X;
            backNeckHoleHeight = start.Y - backNeckHole.Y;
            neckHoleHeight = neckHoleHeight-(frontUpNeckHole.Y - frontNeck.Y);

            gfx.DrawArc(pen, new XRect(start.X - neckHoleWidth, start.Y - 2 * backNeckHoleHeight, neckHoleWidth * 2, backNeckHoleHeight * 2), 0, 90);
            gfx.DrawArc(pen, new XRect(frontUpNeckHole.X, frontUpNeckHole.Y - neckHoleHeight, neckHoleWidth * 2, neckHoleHeight * 2), 90, 90);

            // all armhole 
            gfx.DrawCurve(pen, new XPoint[] { backShoulder, armhole1, armhole2, armhole3, centerBreast, armhole4, armhole5, frontShoulder});

            // down hem
            XPoint centerDown = new XPoint(centerHips.X, centerHips.Y - 10);
            XPoint frontDown = new XPoint(frontHips.X - (frontHips.X - centerHips.X) / 3, frontHips.Y);

            gfx.DrawLines(pen, new XPoint[] { backDeflection, centerDown, frontDown });


            // tuck
            XPoint tuckBack = new XPoint(centerWaist.X - 10, centerWaist.Y);
            XPoint tuckFront = new XPoint(centerWaist.X + 10, centerWaist.Y);

            gfx.DrawCurve(pen, new XPoint[] { centerBreast, tuckBack, centerDown });
            gfx.DrawCurve(pen, new XPoint[] { centerBreast, tuckFront, centerDown });

            // front fastening
            DrawLineParallel(frontNeck, frontHips, gfx, dashed, -15); 
            DrawLineParallel(frontNeck, frontHips, gfx, dashed, 15); 

            // back saddle
            XPoint backSaddle1 = new XPoint(start.X, start.Y + 80);
            XPoint backSaddle2 = new XPoint(backSaddle1.X + (backWidth)/2, backSaddle1.Y);
            XPoint backSaddle3 = new XPoint(backSaddle1.X + backWidth + 12, backSaddle1.Y);
            XPoint backSaddle4 = new XPoint(backSaddle3.X, backSaddle3.Y + 5);
            gfx.DrawLine(pen, backSaddle1, backSaddle3);
            gfx.DrawLine(pen, backSaddle2, backSaddle4);

        }

        static XPoint ShoulderCircleIntersection(XPoint A, XPoint B, double Ar, double Br)
        {
            // front shlouder point circles intersection
            // https://cs.wikibooks.org/wiki/Geometrie/Numerick%C3%BD_v%C3%BDpo%C4%8Det_pr%C5%AFniku_dvou_kru%C5%BEnic
            double d = XPoint.Subtract(A, B).Length; // ok
           
            double m = (Math.Pow(Ar,2) - Math.Pow(Br,2)) / (2 * d) + d / 2;
            double h = Math.Sqrt(Math.Pow(Ar,2) - Math.Pow(m,2));

            XPoint center = A + m * (B-A) / d;

            double frontShoulderX = center.X - h * (A.Y - B.Y) / d;
            double frontShoulderY = center.Y + h * (A.X - B.X) / d;

            XPoint frontShoulder = new XPoint(frontShoulderX, frontShoulderY);

            return frontShoulder;
        }

        static void ShiftLineParallel(ref XPoint A, ref XPoint B, XGraphics gfx, XPen pen, double offset)
        {
            double L = Math.Sqrt(Math.Pow((A.X - B.X),2) + Math.Pow((A.Y - B.Y),2));

            A = new XPoint(A.X + offset * (B.Y - A.Y) / L, A.Y + offset * (A.X - B.X) / L);
            B = new XPoint(B.X + offset * (B.Y - A.Y) / L, B.Y + offset * (A.X - B.X) / L);

            gfx.DrawLine(pen, A, B);
        }

        static void DrawLineParallel(XPoint A, XPoint B, XGraphics gfx, XPen pen, double offset)
        {
            double L = Math.Sqrt(Math.Pow((A.X - B.X), 2) + Math.Pow((A.Y - B.Y), 2));

            gfx.DrawLine(pen, new XPoint(A.X + offset * (B.Y - A.Y) / L, A.Y + offset * (A.X - B.X) / L), 
                new XPoint(B.X + offset * (B.Y - A.Y) / L, B.Y + offset * (A.X - B.X) / L));
        }

        static XPoint FindPointOnLine(XPoint start, XPoint end, double dist)
        {
            double m = (end.Y - start.Y) / (end.X - start.X);
            double X = start.X + dist / Math.Sqrt(1 + Math.Pow(m, 2));
            double Y = m * (X - start.X) + start.Y;
            return new XPoint(X, Y);
        }

        static void Blouse()
        {

            double frontNeckUpAddition = 45;

            double armpitLength = measures.height / 10 + measures.circ_bust / 20 + 3;

            double additionBack = 5;
            double backWidth = measures.wid_back / 2 + additionBack + 5;
            double lenShoulder = measures.len_shoulder + additionBack;

            double additionAll = 35;
            double allWidth;
            if (measures.circ_bust > 1000)
            {
                allWidth = measures.circ_bust / 2 + (measures.circ_bust - 1000) / 10 + additionAll;
            }
            else
            {
                allWidth = measures.circ_bust / 2 + additionAll;
            }

            double frontWidth = measures.circ_bust / 4 - 30;
            double armholeWidth = allWidth - frontWidth - backWidth;

            double lenBreast;

            if(measures.len_breast != 0)
            {
                lenBreast = measures.len_breast;
            }
            else
            {
                lenBreast = measures.circ_bust / 4 + 4;
            }

            // prepare pdf

            PdfPage p = AddPatternPage(measures.len_back + measures.len_hips + armpitLength, allWidth);
            XGraphics gfx = XGraphics.FromPdfPage(p, XGraphicsUnit.Millimeter);


            // main net 

            XPoint start = new XPoint(30, 30 + frontNeckUpAddition);
            XPoint backBreast = new XPoint(start.X, start.Y + armpitLength);
            XPoint backWaist = new XPoint(start.X, start.Y + measures.len_back);
            XPoint backHips = new XPoint(start.X, start.Y + measures.len_back + measures.len_hips - 20);

            XPoint backHipsDeflection = new XPoint(backHips.X + 20, backHips.Y);
            XPoint backWaistDeflection = new XPoint(backWaist.X + 20, backWaist.Y);

            double patternGap = 50;

            XPoint backArmholeBreast = new XPoint(backBreast.X + backWidth, backBreast.Y);
            XPoint centerBreast1 = new XPoint(backArmholeBreast.X + 2 * armholeWidth / 3, backArmholeBreast.Y);
            XPoint centerBreast2 = new XPoint(backArmholeBreast.X + 2 * armholeWidth / 3 + patternGap, backArmholeBreast.Y);
            XPoint frontArmholeBreast = new XPoint(backBreast.X + backWidth + armholeWidth + patternGap, backBreast.Y);
            XPoint frontBreast = new XPoint(backBreast.X + allWidth + patternGap, backBreast.Y);

            XPoint backArmholeShoulder = new XPoint(backArmholeBreast.X, start.Y);
            XPoint frontArmholeNeck = new XPoint(frontArmholeBreast.X, start.Y - frontNeckUpAddition);

            XPoint frontNeck = new XPoint(frontBreast.X, frontArmholeNeck.Y);

            XPoint frontWaist = new XPoint(frontBreast.X, backWaist.Y);
            XPoint frontHips = new XPoint(frontBreast.X, backHips.Y);

            XPoint centerWaist1 = new XPoint(centerBreast1.X, backWaist.Y);
            XPoint centerHips1 = new XPoint(centerBreast1.X, backHips.Y);

            XPoint centerWaist2 = new XPoint(centerBreast2.X, backWaist.Y);
            XPoint centerHips2 = new XPoint(centerBreast2.X, backHips.Y);

            gfx.DrawLine(pen, start, backHips);
            gfx.DrawLine(pen, backArmholeShoulder, backArmholeBreast);
            gfx.DrawLine(pen, frontArmholeNeck, frontArmholeBreast);
            gfx.DrawLine(pen, centerBreast1, centerHips1);
            gfx.DrawLine(pen, centerBreast2, centerHips2);
            gfx.DrawLine(pen, frontNeck, frontHips);
            gfx.DrawLine(pen, start, backWaistDeflection);
            gfx.DrawLine(pen, backWaistDeflection, backHipsDeflection);

            gfx.DrawLine(pen, start, backArmholeShoulder);
            gfx.DrawLine(pen, frontArmholeNeck, frontNeck);
            gfx.DrawLine(pen, backBreast, frontBreast);
            gfx.DrawLine(pen, backWaist, frontWaist);
            gfx.DrawLine(pen, backHips, frontHips);

            // final shape

            // back neck hole
            double backNeckHoleHeight = 20;
            double neckHoleWidth = measures.circ_bust / 20 + 23;
            XPoint backNeckHole = new XPoint(start.X + neckHoleWidth, start.Y - 20);

            // back shoulder
            double intersectionDown = 15;
            XPoint backArmholeIntersection = new XPoint(backArmholeShoulder.X, backArmholeShoulder.Y + intersectionDown);

            double lenAfterIntersection = lenShoulder - XPoint.Subtract(backNeckHole,backArmholeIntersection).Length + 10;
            double a = Math.Atan((intersectionDown + backNeckHoleHeight) / (backWidth - neckHoleWidth));

            double backShoulderYpos = backArmholeIntersection.Y + (Math.Sin(a) * lenAfterIntersection);
            double backShoulderXpos = backArmholeIntersection.X + (Math.Cos(a) * lenAfterIntersection);

            XPoint backShoulder = new XPoint(backShoulderXpos, backShoulderYpos);

            armpitLength -= intersectionDown;


            // front neck hole
            double neckHoleHeight = measures.circ_bust / 20 + 30;
            XPoint frontUpNeckHole = new XPoint(frontNeck.X - neckHoleWidth, frontNeck.Y);

            // front shoulder
            double help = measures.circ_bust /20 - 5;

            XPoint frontShoulder1 = new XPoint(frontArmholeBreast.X - help, centerBreast2.Y - Math.Sqrt(Math.Abs(Math.Pow(armpitLength - 1.5, 2) - Math.Pow(help, 2))));


            XPoint frontBreastLineUp = new XPoint(frontBreast.X - (measures.circ_bust / 10 + 5), frontNeck.Y);
            XPoint breastPoint = new XPoint(frontBreastLineUp.X, frontBreastLineUp.Y + lenBreast);

            XPoint s1 = ShoulderCircleIntersection(frontShoulder1, breastPoint, lenShoulder, lenBreast);

            double shoulderUp = XPoint.Subtract(frontBreastLineUp, frontUpNeckHole).Length;

            XPoint frontShoulder2 = FindPointOnLine(frontShoulder1, s1, lenShoulder - shoulderUp);

            double len = XPoint.Subtract(frontShoulder2, breastPoint).Length;
            XPoint frontShoulder3 = new XPoint(breastPoint.X, breastPoint.Y - len);

            // translate shoulder seam
            ShiftLineParallel(ref frontShoulder1, ref frontShoulder2, gfx, pen, -10);
            ShiftLineParallel(ref backShoulder, ref backNeckHole, gfx, pen, -10);
            ShiftLineParallel(ref frontShoulder3, ref frontUpNeckHole, gfx, pen, -10);

            //draw neck hole - using shifted shnoulder seam
            neckHoleWidth = backNeckHole.X - start.X;
            backNeckHoleHeight = start.Y - backNeckHole.Y;
            neckHoleHeight = neckHoleHeight - (frontUpNeckHole.Y - frontNeck.Y);

            gfx.DrawArc(pen, new XRect(start.X - neckHoleWidth, start.Y - 2 * backNeckHoleHeight, neckHoleWidth * 2, backNeckHoleHeight * 2), 0, 90);
            gfx.DrawArc(pen, new XRect(frontUpNeckHole.X, frontUpNeckHole.Y - neckHoleHeight, neckHoleWidth * 2, neckHoleHeight * 2), 90, 90);


            //armhole 

            // back
            XPoint armhole1 = new XPoint(backArmholeShoulder.X + 10, backArmholeIntersection.Y + (armpitLength / 2));
            XPoint armhole2 = new XPoint(backArmholeShoulder.X + 15, backArmholeIntersection.Y + ((3 * armpitLength) / 4));

            XPoint armhole3 = new XPoint(backArmholeShoulder.X + (centerBreast1.X - backArmholeShoulder.X) / 2, backArmholeIntersection.Y + ((19 * armpitLength) / 20));

            // fornt
            XPoint armhole4 = new XPoint(frontArmholeBreast.X, armhole2.Y);

            // all armhole 
            gfx.DrawCurve(pen, new XPoint[] { backShoulder, armhole1, armhole2, armhole3, centerBreast1});
            gfx.DrawCurve(pen, new XPoint[] { centerBreast2, armhole4, frontShoulder1 });

            // draw front shoulder tuck
            gfx.DrawLine(dashed, frontShoulder2, breastPoint);
            gfx.DrawLine(dashed, frontShoulder3, breastPoint);
            gfx.DrawLine(dashed, armhole4, breastPoint);

            // ---------------tucks------------------------------------
            double tuckFrontWidth = frontWidth - measures.circ_waist / 4;
            double allTucksBackWidth = allWidth - measures.circ_waist / 2 - tuckFrontWidth - patternGap ;
            double tuckBackWidth = allTucksBackWidth / 3;

            // frot
            XPoint frontTuckUp = new XPoint(breastPoint.X, frontWaist.Y - 140);
            XPoint frontTuck1 = new XPoint(frontTuckUp.X - tuckFrontWidth / 2, frontWaist.Y);
            XPoint frontTuck2 = new XPoint(frontTuckUp.X + tuckFrontWidth / 2, frontWaist.Y);

            XPoint frontTuckDown1 = new XPoint(frontTuckUp.X - tuckFrontWidth / 4, frontHips.Y);
            XPoint frontTuckDown2 = new XPoint(frontTuckUp.X + tuckFrontWidth / 4, frontHips.Y);

            gfx.DrawLines(pen, new XPoint[] { frontTuckDown1, frontTuck1, frontTuckUp,frontTuck2, frontTuckDown2});

            // center

            XPoint centerTuckFront = new XPoint(centerWaist2.X + tuckBackWidth / 2, centerWaist2.Y - 10);
            XPoint centerTuckBack = new XPoint(centerWaist1.X - tuckBackWidth / 2, centerWaist1.Y - 10);

            double downAddition = measures.circ_hips / 2 - (allWidth - tuckFrontWidth / 4 - patternGap) - 20;

            XPoint centerTuckBackDown = new XPoint(centerHips1.X + downAddition / 2, centerHips1.Y - 6);
            XPoint centerTuckFrontDown = new XPoint(centerHips2.X - downAddition / 2, centerHips2.Y - 6);

            gfx.DrawLines(pen, new XPoint[] { centerBreast1, centerTuckBack, centerTuckBackDown });
            gfx.DrawLines(pen, new XPoint[] { centerBreast2, centerTuckFront, centerTuckFrontDown });

            gfx.DrawCurve(pen, new XPoint[] { centerTuckBackDown,new XPoint(backHips.X + backWidth / 2, backHips.Y) , backHips });
            gfx.DrawCurve(pen, new XPoint[] { centerTuckFrontDown, new XPoint(frontHips.X - frontWidth / 2, frontHips.Y), frontHips });

            //back 1
            double l1 = XPoint.Subtract(backBreast, backWaist).Length;
            double l2 = XPoint.Subtract(backHips, backWaist).Length;
            XPoint back1TuckUp = new XPoint(backBreast.X + backWidth / 2, backBreast.Y + l1 / 4);
            XPoint back1TuckDown = new XPoint(back1TuckUp.X, backHips.Y - l2 / 4);
            XPoint back1Tuck1 = new XPoint(back1TuckUp.X - tuckBackWidth / 2, backWaist.Y);
            XPoint back1Tuck2 = new XPoint(back1TuckUp.X + tuckBackWidth / 2, backWaist.Y);

            gfx.DrawLines(pen, new XPoint[] { back1TuckUp,back1Tuck1,back1TuckDown,back1Tuck2,back1TuckUp });

            //back 2
            XPoint back2TuckUp = new XPoint(backBreast.X + backWidth, backBreast.Y + l1 / 3);
            XPoint back2TuckDown = new XPoint(back2TuckUp.X, backHips.Y - l2 / 3);
            XPoint back2Tuck1 = new XPoint(back2TuckUp.X - tuckBackWidth / 2, backWaist.Y);
            XPoint back2Tuck2 = new XPoint(back2TuckUp.X + tuckBackWidth / 2, backWaist.Y);

            gfx.DrawLines(pen, new XPoint[] { back2TuckUp, back2Tuck1, back2TuckDown, back2Tuck2, back2TuckUp });

            // back saddle
            XPoint backSaddle1 = new XPoint(start.X, start.Y + 80);
            XPoint backSaddle2 = new XPoint(backSaddle1.X + (backWidth) / 2, backSaddle1.Y);
            XPoint backSaddle3 = new XPoint(backSaddle1.X + backWidth + 12, backSaddle1.Y);
            XPoint backSaddle4 = new XPoint(backSaddle3.X, backSaddle3.Y + 5);
            gfx.DrawLine(pen, backSaddle1, backSaddle3);
            gfx.DrawLine(pen, backSaddle2, backSaddle4);
        }

        static void Dress()
        {
            double frontNeckUpAddition = 45;

            double armpitLength = measures.height / 10 + measures.circ_bust / 20 + 3;

            double additionBack = 5;
            double backWidth = measures.wid_back / 2 + additionBack + 5;
            double lenShoulder = measures.len_shoulder + additionBack;

            double additionAll = 35;
            double allWidth;
            if (measures.circ_bust > 1000)
            {
                allWidth = measures.circ_bust / 2 + (measures.circ_bust - 1000) / 10 + additionAll;
            }
            else
            {
                allWidth = measures.circ_bust / 2 + additionAll;
            }

            double frontWidth = measures.circ_bust / 4 - 30;
            double armholeWidth = allWidth - frontWidth - backWidth;

            double lenBreast;

            if (measures.len_breast != 0)
            {
                lenBreast = measures.len_breast;
            }
            else
            {
                lenBreast = measures.circ_bust / 4 + 4;
            }

            // prepare pdf

            PdfPage p = AddPatternPage(measures.len_back + measures.len_knee + armpitLength, allWidth);
            XGraphics gfx = XGraphics.FromPdfPage(p, XGraphicsUnit.Millimeter);


            // main net 

            XPoint start = new XPoint(30, 30 + frontNeckUpAddition);
            XPoint backBreast = new XPoint(start.X, start.Y + armpitLength);
            XPoint backWaist = new XPoint(start.X, start.Y + measures.len_back);
            XPoint backHips = new XPoint(start.X, start.Y + measures.len_back + measures.len_hips);
            XPoint backKnee = new XPoint(start.X, start.Y + measures.len_back + measures.len_knee);

            double backDeflection = 20;

            XPoint backWaistDeflection = new XPoint(backWaist.X + backDeflection, backWaist.Y);
            XPoint backHipsDeflection = new XPoint(backHips.X + backDeflection, backHips.Y);
            XPoint backKneeDeflection = new XPoint(backKnee.X + backDeflection, backKnee.Y);

            double patternGap = 80;

            XPoint backArmholeBreast = new XPoint(backBreast.X + backWidth, backBreast.Y);
            XPoint centerBreast1 = new XPoint(backArmholeBreast.X + 2 * armholeWidth / 3, backArmholeBreast.Y);
            XPoint centerBreast2 = new XPoint(backArmholeBreast.X + 2 * armholeWidth / 3 + patternGap, backArmholeBreast.Y);
            XPoint frontArmholeBreast = new XPoint(backBreast.X + backWidth + armholeWidth + patternGap, backBreast.Y);
            XPoint frontBreast = new XPoint(backBreast.X + allWidth + patternGap, backBreast.Y);

            XPoint backArmholeShoulder = new XPoint(backArmholeBreast.X, start.Y);
            XPoint frontArmholeNeck = new XPoint(frontArmholeBreast.X, start.Y - frontNeckUpAddition);

            XPoint frontNeck = new XPoint(frontBreast.X, frontArmholeNeck.Y);

            XPoint frontWaist = new XPoint(frontBreast.X, backWaist.Y);
            XPoint frontHips = new XPoint(frontBreast.X, backHips.Y);
            XPoint frontKnee = new XPoint(frontBreast.X, backKnee.Y);

            XPoint centerWaist1 = new XPoint(centerBreast1.X, backWaist.Y);
            XPoint centerHips1 = new XPoint(centerBreast1.X, backHips.Y);
            XPoint centerKnee1 = new XPoint(centerBreast1.X, backKnee.Y);

            XPoint centerWaist2 = new XPoint(centerBreast2.X, backWaist.Y);
            XPoint centerHips2 = new XPoint(centerBreast2.X, backHips.Y);
            XPoint centerKnee2 = new XPoint(centerBreast2.X, backKnee.Y);



            gfx.DrawLine(pen, start, backKnee);
            gfx.DrawLine(pen, backArmholeShoulder, backArmholeBreast);
            gfx.DrawLine(pen, frontArmholeNeck, frontArmholeBreast);
            gfx.DrawLine(pen, centerBreast1, centerKnee1);
            gfx.DrawLine(pen, centerBreast2, centerKnee2);
            gfx.DrawLine(pen, frontNeck, frontKnee);
            gfx.DrawLine(pen, start, backWaistDeflection);
            gfx.DrawLine(pen, backWaistDeflection, backKneeDeflection);

            gfx.DrawLine(pen, start, backArmholeShoulder);
            gfx.DrawLine(pen, frontArmholeNeck, frontNeck);
            gfx.DrawLine(pen, backBreast, frontBreast);
            gfx.DrawLine(pen, backWaist, frontWaist);
            gfx.DrawLine(pen, backHips, frontHips);
            gfx.DrawLine(pen, backKnee, frontKnee);

            // final shape

            // back neck hole
            double backNeckHoleHeight = 20;
            double neckHoleWidth = measures.circ_bust / 20 + 23;
            XPoint backNeckHole = new XPoint(start.X + neckHoleWidth, start.Y - 20);

            // back shoulder
            double intersectionDown = 15;
            XPoint backArmholeIntersection = new XPoint(backArmholeShoulder.X, backArmholeShoulder.Y + intersectionDown);

            double lenAfterIntersection = lenShoulder - XPoint.Subtract(backNeckHole, backArmholeIntersection).Length + 10;
            double a = Math.Atan((intersectionDown + backNeckHoleHeight) / (backWidth - neckHoleWidth));

            double backShoulderYpos = backArmholeIntersection.Y + (Math.Sin(a) * lenAfterIntersection);
            double backShoulderXpos = backArmholeIntersection.X + (Math.Cos(a) * lenAfterIntersection);

            XPoint backShoulder = new XPoint(backShoulderXpos, backShoulderYpos);

            armpitLength -= intersectionDown;


            // front neck hole
            double neckHoleHeight = measures.circ_bust / 20 + 30;
            XPoint frontUpNeckHole = new XPoint(frontNeck.X - neckHoleWidth, frontNeck.Y);

            // front shoulder
            double help = measures.circ_bust / 20 - 5;

            XPoint frontShoulder1 = new XPoint(frontArmholeBreast.X - help, centerBreast2.Y - Math.Sqrt(Math.Abs(Math.Pow(armpitLength - 1.5, 2) - Math.Pow(help, 2))));


            XPoint frontBreastLineUp = new XPoint(frontBreast.X - (measures.circ_bust / 10 + 5), frontNeck.Y);
            XPoint breastPoint = new XPoint(frontBreastLineUp.X, frontBreastLineUp.Y + lenBreast);

            XPoint s1 = ShoulderCircleIntersection(frontShoulder1, breastPoint, lenShoulder, lenBreast);

            double shoulderUp = XPoint.Subtract(frontBreastLineUp, frontUpNeckHole).Length;

            XPoint frontShoulder2 = FindPointOnLine(frontShoulder1, s1, lenShoulder - shoulderUp);

            double len = XPoint.Subtract(frontShoulder2, breastPoint).Length;
            XPoint frontShoulder3 = new XPoint(breastPoint.X, breastPoint.Y - len);

            // translate shoulder seam
            ShiftLineParallel(ref frontShoulder1, ref frontShoulder2, gfx, pen, -10);
            ShiftLineParallel(ref backShoulder, ref backNeckHole, gfx, pen, -10);
            ShiftLineParallel(ref frontShoulder3, ref frontUpNeckHole, gfx, pen, -10);

            //draw neck hole - using shifted shnoulder seam
            neckHoleWidth = backNeckHole.X - start.X;
            backNeckHoleHeight = start.Y - backNeckHole.Y;
            neckHoleHeight = neckHoleHeight - (frontUpNeckHole.Y - frontNeck.Y);

            gfx.DrawArc(pen, new XRect(start.X - neckHoleWidth, start.Y - 2 * backNeckHoleHeight, neckHoleWidth * 2, backNeckHoleHeight * 2), 0, 90);
            gfx.DrawArc(pen, new XRect(frontUpNeckHole.X, frontUpNeckHole.Y - neckHoleHeight, neckHoleWidth * 2, neckHoleHeight * 2), 90, 90);


            //armhole 

            // back
            XPoint armhole1 = new XPoint(backArmholeShoulder.X + 10, backArmholeIntersection.Y + (armpitLength / 2));
            XPoint armhole2 = new XPoint(backArmholeShoulder.X + 15, backArmholeIntersection.Y + ((3 * armpitLength) / 4));

            XPoint armhole3 = new XPoint(backArmholeShoulder.X + (centerBreast1.X - backArmholeShoulder.X) / 2, backArmholeIntersection.Y + ((19 * armpitLength) / 20));

            // fornt
            XPoint armhole4 = new XPoint(frontArmholeBreast.X, armhole2.Y);

            // all armhole 
            gfx.DrawCurve(pen, new XPoint[] { backShoulder, armhole1, armhole2, armhole3, centerBreast1 });
            gfx.DrawCurve(pen, new XPoint[] { centerBreast2, armhole4, frontShoulder1 });

            // draw front shoulder tuck
            gfx.DrawLine(dashed, frontShoulder2, breastPoint);
            gfx.DrawLine(dashed, frontShoulder3, breastPoint);
            gfx.DrawLine(dashed, armhole4, breastPoint);





            // back saddle
            XPoint backSaddle1 = new XPoint(start.X, start.Y + 80);
            XPoint backSaddle2 = new XPoint(backSaddle1.X + (backWidth) / 2, backSaddle1.Y);
            XPoint backSaddle3 = new XPoint(backSaddle1.X + backWidth + 12, backSaddle1.Y);
            XPoint backSaddle4 = new XPoint(backSaddle3.X, backSaddle3.Y + 5);
            gfx.DrawLine(pen, backSaddle1, backSaddle3);
            gfx.DrawLine(pen, backSaddle2, backSaddle4);
        }
    }
}
