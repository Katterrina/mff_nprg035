using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.ComponentModel;
using System.Collections.Generic;

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
            if (len_breast == 0) { this.len_breast = 0; } else { this.len_breast = len_breast; } //TODO len_breast
        }
    }
    public struct Abscissa
    {
        public XPoint a;
        public XPoint b;
        public Abscissa(XPoint start, XPoint end)
        {
            a = start;
            b = end;
        }
    }
    class PatternControl
    {
        public enum Pattern
        {
            [Description("rovná sukně")] straightSkirt,
            [Description("rozšířená sukně")] wideSkirt,
            [Description("šaty")] dress,
            [Description("košile")] shirt,
            [Description("halenka")] blouse,
            [Description("rukáv")] sleeve
        }

        MeasuresData m;
        public PatternControl(MeasuresData measures)
        {
            this.m = measures;
        }

        public string PdfPattern(Pattern pattern, string path)
        {
            string pathToPdf = path + "\\" + pattern.ToString() + ".pdf";
            PdfDocument pdfDoc = CreatePdfDocument(pattern);

            switch (pattern)
            {
                case Pattern.straightSkirt:
                    DrawingPattern sSkirt = new StraightSkirt(pdfDoc, m.circ_waist, m.circ_hips, m.len_hips, m.len_knee);
                    sSkirt.drawPattern();
                    break;
                case Pattern.wideSkirt:
                    DrawingPattern wSkirt = new WideSkirt(pdfDoc, m.circ_waist, m.circ_hips, m.len_hips, m.len_knee);
                    wSkirt.drawPattern();
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

            SaveClose(pdfDoc, pathToPdf);

            return pathToPdf;
        }

        PdfDocument CreatePdfDocument(Pattern p)
        {
            PdfDocument pdfDoc = new PdfDocument();
            pdfDoc.Info.Title = "EasyPattern";
            pdfDoc.Info.Author = "Kateřina Č.";
            pdfDoc.Info.Keywords = p.ToString();

            return pdfDoc;
        }


        void SaveClose(PdfDocument pdfDoc, string path)
        {
            pdfDoc.Save(path);
            pdfDoc.Close();
        }
    }

    class PdfControl
    {
        public static PdfPage AddPage(double minHeight, double minWidth, PdfDocument pdfDoc)
        {
            PdfPage p = pdfDoc.AddPage();
            p.Size = PageSize.A0;

            if (minWidth > minHeight)
            {
                p.Orientation = PageOrientation.Landscape;

                if (minHeight < 820 && minWidth < 550)
                {
                    p.Size = PageSize.A1;

                }
            }
            else
            {
                p.Orientation = PageOrientation.Portrait;

                if (minHeight < 550 && minWidth < 8240)
                {
                    p.Size = PageSize.A1;
                }
            }

            return p;
        }

        public static XPoint start = new XPoint(30, 30);
    }

    interface DrawingPattern
    {
        void drawPattern();
    }

    static class PatternGeometry
    {
        public static XPoint ShiftOnePoint(XPoint p, int distRight, int distDown)
        {
            return new XPoint(p.X + distRight, p.Y + distDown);
        }

        public static XPoint ShiftOnePointHorizontal(XPoint p, int distRight)
        {
            return ShiftOnePoint(p, distRight, 0);
        }

        public static XPoint ShiftOnePointVertical(XPoint p, int distDown)
        {
            return ShiftOnePoint(p, 0, distDown);
        }

        public static void ShiftLineParallel(ref XPoint A, ref XPoint B, XGraphics gfx, XPen pen, double offset)
        {
            double L = Math.Sqrt(Math.Pow((A.X - B.X), 2) + Math.Pow((A.Y - B.Y), 2));

            A = new XPoint(A.X + offset * (B.Y - A.Y) / L, A.Y + offset * (A.X - B.X) / L);
            B = new XPoint(B.X + offset * (B.Y - A.Y) / L, B.Y + offset * (A.X - B.X) / L);
        }

        public static XPoint FindPointOnLine(XPoint start, XPoint end, double dist)
        {
            double m = (end.Y - start.Y) / (end.X - start.X);
            double X = start.X + dist / Math.Sqrt(1 + Math.Pow(m, 2));
            double Y = m * (X - start.X) + start.Y;
            return new XPoint(X, Y);
        }

        public static XPoint FindLineCenter(Abscissa line)
        {
            int dist = (int)XPoint.Subtract(line.a, line.b).Length;
            return FindPointOnLine(line.a, line.b, dist / 2);
        }
    }

    abstract class Pattern
    {
        readonly protected XPen pen;
        readonly protected XPen grayPen;
        readonly protected XPen dashedPen;

        public Pattern()
        {
            pen = new XPen(XColors.Black);
            grayPen = new XPen(XColors.Gray);
            dashedPen = new XPen(XColors.Black);
            dashedPen.DashStyle = XDashStyle.Dash;
        }

        protected static XGraphics createPdfGraphics(int patternHeight, int patternWidth, PdfDocument pdfDoc)
        {
            PdfPage page = PdfControl.AddPage(patternHeight, patternWidth, pdfDoc);
            return XGraphics.FromPdfPage(page, XGraphicsUnit.Millimeter);
        }

        protected static Dictionary<string, XPoint> ShiftPoints(Dictionary<string, XPoint> line, int distanceVertical, int distanceHorizontal, bool upDirection, bool leftDirection)
        {

            Dictionary<string, XPoint> newLine = new Dictionary<string, XPoint>();

            int distV = upDirection ? -distanceVertical : distanceVertical;
            int distH = leftDirection ? -distanceHorizontal : distanceHorizontal;

            foreach (KeyValuePair<string, XPoint> point in line)
            {
                XPoint p = PatternGeometry.ShiftOnePoint(point.Value, distH, distV);
                newLine.Add(point.Key, p);
            }

            return newLine;
        }

        protected static Dictionary<string, XPoint> ShiftPointsRight(Dictionary<string, XPoint> line, int dist)
        {
            return ShiftPoints(line, 0, dist, false, false);
        }

        public static void DrawLineParallel(XPoint A, XPoint B, XGraphics gfx, XPen pen, double offset)
        {
            double L = Math.Sqrt(Math.Pow((A.X - B.X), 2) + Math.Pow((A.Y - B.Y), 2));

            gfx.DrawLine(pen, new XPoint(A.X + offset * (B.Y - A.Y) / L, A.Y + offset * (A.X - B.X) / L),
                new XPoint(B.X + offset * (B.Y - A.Y) / L, B.Y + offset * (A.X - B.X) / L));
        }
    }

    abstract class Bodice : Pattern
    {
        readonly protected PdfDocument pdfDoc;

        readonly protected int height;
        readonly protected int widthWaist;
        readonly protected int widthHips;
        readonly protected int lenHips;
        readonly protected int lenKnee;

        public Bodice(PdfDocument pdfDoc, int circWaist, int circHips, int lenHips, int lenKnee) : base()
        {
            this.pdfDoc = pdfDoc;

            // todo
            this.widthHips = circHips / 2;
            this.widthWaist = circWaist / 2;
            this.lenHips = lenHips;
            this.lenKnee = lenKnee;
        }

        //todo
        protected Dictionary<string, Dictionary<string, XPoint>> BodiceNet(XPoint start, int lenHips, int lenKnee, int widBack, int widFront, int patternGap)
        {
            Dictionary<string, Dictionary<string, XPoint>> net = new Dictionary<string, Dictionary<string, XPoint>>();

            Dictionary<string, XPoint> back = new Dictionary<string, XPoint>();

            back.Add("waist", start);
            back.Add("hips", PatternGeometry.ShiftOnePointVertical(start, lenHips));
            back.Add("down", PatternGeometry.ShiftOnePointVertical(start, lenKnee));

            Dictionary<string, XPoint> centerBack = ShiftPointsRight(back, widBack);
            Dictionary<string, XPoint> centerFront = ShiftPointsRight(centerBack, patternGap);
            Dictionary<string, XPoint> front = ShiftPointsRight(centerFront, widFront);

            net.Add("back", back);
            net.Add("centerFront", centerFront);
            net.Add("centerBack", centerBack);
            net.Add("front", front);

            return net;
        }

        static XPoint FrontShoulderCircleIntersection(XPoint A, XPoint B, double Ar, double Br)
        {
            // front shlouder point circles intersection
            // https://cs.wikibooks.org/wiki/Geometrie/Numerick%C3%BD_v%C3%BDpo%C4%8Det_pr%C5%AFniku_dvou_kru%C5%BEnic
            double d = XPoint.Subtract(A, B).Length; // ok

            double m = (Math.Pow(Ar, 2) - Math.Pow(Br, 2)) / (2 * d) + d / 2;
            double h = Math.Sqrt(Math.Pow(Ar, 2) - Math.Pow(m, 2));

            XPoint center = A + m * (B - A) / d;

            double frontShoulderX = center.X - h * (A.Y - B.Y) / d;
            double frontShoulderY = center.Y + h * (A.X - B.X) / d;

            XPoint frontShoulder = new XPoint(frontShoulderX, frontShoulderY);

            return frontShoulder;
        }
    }
        //    static void Shirt()
        //    {

        //        // prepare measures

        //        double circAddition;
        //        double frontNeckUpAddition = measures.circ_bust / 20;

        //        if (measures.circ_bust < 820) { circAddition = 60; frontNeckUpAddition -= 10; }
        //        else if (measures.circ_bust < 870) { circAddition = 65; frontNeckUpAddition -= 5; }
        //        else if (measures.circ_bust < 920) { circAddition = 70; }
        //        else { circAddition = 75; }

        //        double armpitLength = measures.height / 10 + measures.circ_bust / 20 + 3;

        //        double backWidth = measures.circ_bust / 8 + 55 + 3 * circAddition / 13;
        //        double armholeWidth = measures.circ_bust / 8 - 15 + 6 * circAddition / 13;
        //        double frontWidth = measures.circ_bust / 4 - 40 + 4 * circAddition / 13;

        //        double allWidth = backWidth + armholeWidth + frontWidth;


        //        // prepare pdf

        //        PdfPage p = AddPatternPage(measures.len_back + measures.len_hips + armpitLength, allWidth);
        //        XGraphics gfx = XGraphics.FromPdfPage(p, XGraphicsUnit.Millimeter);

        //        // net

        //        XPoint start = new XPoint(30, 30 + frontNeckUpAddition);
        //        XPoint backBreast = new XPoint(start.X, start.Y + armpitLength);
        //        XPoint backWaist = new XPoint(start.X, start.Y + measures.len_back);
        //        XPoint backHips = new XPoint(start.X, start.Y + measures.len_back + measures.len_hips);
        //        XPoint backDeflection = new XPoint(backHips.X + 20, backHips.Y);

        //        XPoint backArmholeBreast = new XPoint(backBreast.X + backWidth, backBreast.Y);
        //        XPoint centerBreast = new XPoint(backArmholeBreast.X + 2 * armholeWidth / 3, backArmholeBreast.Y);
        //        XPoint frontArmholeBreast = new XPoint(backBreast.X + backWidth + armholeWidth, backBreast.Y);
        //        XPoint frontBreast = new XPoint(backBreast.X + allWidth, backBreast.Y);

        //        XPoint backArmholeShoulder = new XPoint(backArmholeBreast.X, start.Y);
        //        XPoint frontArmholeNeck = new XPoint(frontArmholeBreast.X, start.Y - frontNeckUpAddition);

        //        XPoint frontNeck = new XPoint(frontBreast.X, frontArmholeNeck.Y);

        //        XPoint frontWaist = new XPoint(frontBreast.X, backWaist.Y);
        //        XPoint frontHips = new XPoint(frontBreast.X, backHips.Y);

        //        XPoint centerWaist = new XPoint(centerBreast.X, backWaist.Y);
        //        XPoint centerHips = new XPoint(centerBreast.X, backHips.Y);

        //        gfx.DrawLine(pen, start, backHips);
        //        gfx.DrawLine(pen, backArmholeShoulder, backArmholeBreast);
        //        gfx.DrawLine(pen, frontArmholeNeck, frontArmholeBreast);
        //        gfx.DrawLine(pen, centerBreast, centerHips);
        //        gfx.DrawLine(pen, frontNeck, frontHips);
        //        gfx.DrawLine(pen, start, backDeflection);

        //        gfx.DrawLine(pen, start, backArmholeShoulder);
        //        gfx.DrawLine(pen, frontArmholeNeck, frontNeck);
        //        gfx.DrawLine(pen, backBreast, frontBreast);
        //        gfx.DrawLine(pen, backWaist, frontWaist);
        //        gfx.DrawLine(pen, backHips, frontHips);

        //        // final shape

        //        // back neck hole
        //        double backNeckHoleHeight = 20;
        //        double neckHoleWidth = measures.circ_bust / 20 + 23;
        //        XPoint backNeckHole = new XPoint(start.X + neckHoleWidth, start.Y - 20);

        //        // back shoulder
        //        double intersectionDown = 10;
        //        XPoint backArmholeIntersection = new XPoint(backArmholeShoulder.X, backArmholeShoulder.Y + intersectionDown);

        //        double lenAfterIntersection = 30;
        //        double a = Math.Atan((intersectionDown + backNeckHoleHeight) / (backWidth - neckHoleWidth));

        //        double backShoulderYpos = backArmholeIntersection.Y + (Math.Sin(a) * lenAfterIntersection);
        //        double backShoulderXpos = backArmholeIntersection.X + (Math.Cos(a) * lenAfterIntersection);

        //        XPoint backShoulder = new XPoint(backShoulderXpos, backShoulderYpos);

        //        armpitLength -= intersectionDown;

        //        // front neck hole
        //        double neckHoleHeight = measures.circ_bust / 20 + 40;
        //        XPoint frontUpNeckHole = new XPoint(frontNeck.X - neckHoleWidth, frontNeck.Y);

        //        // front shoulder
        //        double lenShoulder = XPoint.Subtract(backNeckHole, backShoulder).Length;

        //        XPoint frontShoulder = ShoulderCircleIntersection(frontArmholeBreast, frontUpNeckHole, armpitLength, lenShoulder);

        //        // back

        //        XPoint armhole1 = new XPoint(backArmholeShoulder.X + 10, backArmholeIntersection.Y + (armpitLength / 2));
        //        XPoint armhole2 = new XPoint(backArmholeShoulder.X + 15, backArmholeIntersection.Y + ((3 * armpitLength) / 4));

        //        XPoint armhole3 = new XPoint(backArmholeShoulder.X + (centerBreast.X - backArmholeShoulder.X) / 2, backArmholeIntersection.Y + ((19 * armpitLength) / 20));

        //        // fornt
        //        XPoint armhole4 = new XPoint(frontArmholeBreast.X, frontArmholeBreast.Y - 13);
        //        XPoint armhole5;

        //        if (frontShoulder.X > frontArmholeBreast.X)
        //        { armhole5 = new XPoint(frontShoulder.X + 10, frontArmholeBreast.Y - (armpitLength / 2)); }
        //        else
        //        { armhole5 = new XPoint(frontArmholeBreast.X + 13, frontArmholeBreast.Y - (armpitLength / 2)); }

        //        // translate shoulder seam
        //        ShiftLineParallel(ref frontShoulder, ref frontUpNeckHole, gfx, pen, -10);
        //        ShiftLineParallel(ref backShoulder, ref backNeckHole, gfx, pen, -10);

        //        //draw neck hole - using shifted shnoulder seam
        //        neckHoleWidth = backNeckHole.X - start.X;
        //        backNeckHoleHeight = start.Y - backNeckHole.Y;
        //        neckHoleHeight = neckHoleHeight - (frontUpNeckHole.Y - frontNeck.Y);

        //        gfx.DrawArc(pen, new XRect(start.X - neckHoleWidth, start.Y - 2 * backNeckHoleHeight, neckHoleWidth * 2, backNeckHoleHeight * 2), 0, 90);
        //        gfx.DrawArc(pen, new XRect(frontUpNeckHole.X, frontUpNeckHole.Y - neckHoleHeight, neckHoleWidth * 2, neckHoleHeight * 2), 90, 90);

        //        // all armhole 
        //        gfx.DrawCurve(pen, new XPoint[] { backShoulder, armhole1, armhole2, armhole3, centerBreast, armhole4, armhole5, frontShoulder });

        //        // down hem
        //        XPoint centerDown = new XPoint(centerHips.X, centerHips.Y - 10);
        //        XPoint frontDown = new XPoint(frontHips.X - (frontHips.X - centerHips.X) / 3, frontHips.Y);

        //        gfx.DrawLines(pen, new XPoint[] { backDeflection, centerDown, frontDown });


        //        // tuck
        //        XPoint tuckBack = new XPoint(centerWaist.X - 10, centerWaist.Y);
        //        XPoint tuckFront = new XPoint(centerWaist.X + 10, centerWaist.Y);

        //        gfx.DrawCurve(pen, new XPoint[] { centerBreast, tuckBack, centerDown });
        //        gfx.DrawCurve(pen, new XPoint[] { centerBreast, tuckFront, centerDown });

        //        // front fastening
        //        DrawLineParallel(frontNeck, frontHips, gfx, dashedPen, -15);
        //        DrawLineParallel(frontNeck, frontHips, gfx, dashedPen, 15);

        //        // back saddle
        //        XPoint backSaddle1 = new XPoint(start.X, start.Y + 80);
        //        XPoint backSaddle2 = new XPoint(backSaddle1.X + (backWidth) / 2, backSaddle1.Y);
        //        XPoint backSaddle3 = new XPoint(backSaddle1.X + backWidth + 12, backSaddle1.Y);
        //        XPoint backSaddle4 = new XPoint(backSaddle3.X, backSaddle3.Y + 5);
        //        gfx.DrawLine(pen, backSaddle1, backSaddle3);
        //        gfx.DrawLine(pen, backSaddle2, backSaddle4);

        //    }



        //    static void Blouse()
        //    {

        //        double frontNeckUpAddition = 45;

        //        double armpitLength = measures.height / 10 + measures.circ_bust / 20 + 3;

        //        double additionBack = 5;
        //        double backWidth = measures.wid_back / 2 + additionBack + 5;
        //        double lenShoulder = measures.len_shoulder + additionBack;

        //        double additionAll = 35;
        //        double allWidth;
        //        if (measures.circ_bust > 1000)
        //        {
        //            allWidth = measures.circ_bust / 2 + (measures.circ_bust - 1000) / 10 + additionAll;
        //        }
        //        else
        //        {
        //            allWidth = measures.circ_bust / 2 + additionAll;
        //        }

        //        double frontWidth = measures.circ_bust / 4 - 30;
        //        double armholeWidth = allWidth - frontWidth - backWidth;

        //        double lenBreast;

        //        if (measures.len_breast != 0)
        //        {
        //            lenBreast = measures.len_breast;
        //        }
        //        else
        //        {
        //            lenBreast = measures.circ_bust / 4 + 4;
        //        }

        //        // prepare pdf

        //        PdfPage p = AddPatternPage(measures.len_back + measures.len_hips + armpitLength, allWidth);
        //        XGraphics gfx = XGraphics.FromPdfPage(p, XGraphicsUnit.Millimeter);


        //        // main net 

        //        XPoint start = new XPoint(30, 30 + frontNeckUpAddition);
        //        XPoint backBreast = new XPoint(start.X, start.Y + armpitLength);
        //        XPoint backWaist = new XPoint(start.X, start.Y + measures.len_back);
        //        XPoint backHips = new XPoint(start.X, start.Y + measures.len_back + measures.len_hips - 20);

        //        XPoint backHipsDeflection = new XPoint(backHips.X + 20, backHips.Y);
        //        XPoint backWaistDeflection = new XPoint(backWaist.X + 20, backWaist.Y);

        //        double patternGap = 50;

        //        XPoint backArmholeBreast = new XPoint(backBreast.X + backWidth, backBreast.Y);
        //        XPoint centerBreast1 = new XPoint(backArmholeBreast.X + 2 * armholeWidth / 3, backArmholeBreast.Y);
        //        XPoint centerBreast2 = new XPoint(backArmholeBreast.X + 2 * armholeWidth / 3 + patternGap, backArmholeBreast.Y);
        //        XPoint frontArmholeBreast = new XPoint(backBreast.X + backWidth + armholeWidth + patternGap, backBreast.Y);
        //        XPoint frontBreast = new XPoint(backBreast.X + allWidth + patternGap, backBreast.Y);

        //        XPoint backArmholeShoulder = new XPoint(backArmholeBreast.X, start.Y);
        //        XPoint frontArmholeNeck = new XPoint(frontArmholeBreast.X, start.Y - frontNeckUpAddition);

        //        XPoint frontNeck = new XPoint(frontBreast.X, frontArmholeNeck.Y);

        //        XPoint frontWaist = new XPoint(frontBreast.X, backWaist.Y);
        //        XPoint frontHips = new XPoint(frontBreast.X, backHips.Y);

        //        XPoint centerWaist1 = new XPoint(centerBreast1.X, backWaist.Y);
        //        XPoint centerHips1 = new XPoint(centerBreast1.X, backHips.Y);

        //        XPoint centerWaist2 = new XPoint(centerBreast2.X, backWaist.Y);
        //        XPoint centerHips2 = new XPoint(centerBreast2.X, backHips.Y);

        //        gfx.DrawLine(pen, start, backHips);
        //        gfx.DrawLine(pen, backArmholeShoulder, backArmholeBreast);
        //        gfx.DrawLine(pen, frontArmholeNeck, frontArmholeBreast);
        //        gfx.DrawLine(pen, centerBreast1, centerHips1);
        //        gfx.DrawLine(pen, centerBreast2, centerHips2);
        //        gfx.DrawLine(pen, frontNeck, frontHips);
        //        gfx.DrawLine(pen, start, backWaistDeflection);
        //        gfx.DrawLine(pen, backWaistDeflection, backHipsDeflection);

        //        gfx.DrawLine(pen, start, backArmholeShoulder);
        //        gfx.DrawLine(pen, frontArmholeNeck, frontNeck);
        //        gfx.DrawLine(pen, backBreast, frontBreast);
        //        gfx.DrawLine(pen, backWaist, frontWaist);
        //        gfx.DrawLine(pen, backHips, frontHips);

        //        // final shape

        //        // back neck hole
        //        double backNeckHoleHeight = 20;
        //        double neckHoleWidth = measures.circ_bust / 20 + 23;
        //        XPoint backNeckHole = new XPoint(start.X + neckHoleWidth, start.Y - 20);

        //        // back shoulder
        //        double intersectionDown = 15;
        //        XPoint backArmholeIntersection = new XPoint(backArmholeShoulder.X, backArmholeShoulder.Y + intersectionDown);

        //        double lenAfterIntersection = lenShoulder - XPoint.Subtract(backNeckHole, backArmholeIntersection).Length + 10;
        //        double a = Math.Atan((intersectionDown + backNeckHoleHeight) / (backWidth - neckHoleWidth));

        //        double backShoulderYpos = backArmholeIntersection.Y + (Math.Sin(a) * lenAfterIntersection);
        //        double backShoulderXpos = backArmholeIntersection.X + (Math.Cos(a) * lenAfterIntersection);

        //        XPoint backShoulder = new XPoint(backShoulderXpos, backShoulderYpos);

        //        armpitLength -= intersectionDown;


        //        // front neck hole
        //        double neckHoleHeight = measures.circ_bust / 20 + 30;
        //        XPoint frontUpNeckHole = new XPoint(frontNeck.X - neckHoleWidth, frontNeck.Y);

        //        // front shoulder
        //        double help = measures.circ_bust / 20 - 5;

        //        XPoint frontShoulder1 = new XPoint(frontArmholeBreast.X - help, centerBreast2.Y - Math.Sqrt(Math.Abs(Math.Pow(armpitLength - 1.5, 2) - Math.Pow(help, 2))));


        //        XPoint frontBreastLineUp = new XPoint(frontBreast.X - (measures.circ_bust / 10 + 5), frontNeck.Y);
        //        XPoint breastPoint = new XPoint(frontBreastLineUp.X, frontBreastLineUp.Y + lenBreast);

        //        XPoint s1 = ShoulderCircleIntersection(frontShoulder1, breastPoint, lenShoulder, lenBreast);

        //        double shoulderUp = XPoint.Subtract(frontBreastLineUp, frontUpNeckHole).Length;

        //        XPoint frontShoulder2 = FindPointOnLine(frontShoulder1, s1, lenShoulder - shoulderUp);

        //        double len = XPoint.Subtract(frontShoulder2, breastPoint).Length;
        //        XPoint frontShoulder3 = new XPoint(breastPoint.X, breastPoint.Y - len);

        //        // translate shoulder seam
        //        ShiftLineParallel(ref frontShoulder1, ref frontShoulder2, gfx, pen, -10);
        //        ShiftLineParallel(ref backShoulder, ref backNeckHole, gfx, pen, -10);
        //        ShiftLineParallel(ref frontShoulder3, ref frontUpNeckHole, gfx, pen, -10);

        //        //draw neck hole - using shifted shnoulder seam
        //        neckHoleWidth = backNeckHole.X - start.X;
        //        backNeckHoleHeight = start.Y - backNeckHole.Y;
        //        neckHoleHeight = neckHoleHeight - (frontUpNeckHole.Y - frontNeck.Y);

        //        gfx.DrawArc(pen, new XRect(start.X - neckHoleWidth, start.Y - 2 * backNeckHoleHeight, neckHoleWidth * 2, backNeckHoleHeight * 2), 0, 90);
        //        gfx.DrawArc(pen, new XRect(frontUpNeckHole.X, frontUpNeckHole.Y - neckHoleHeight, neckHoleWidth * 2, neckHoleHeight * 2), 90, 90);


        //        //armhole 

        //        // back
        //        XPoint armhole1 = new XPoint(backArmholeShoulder.X + 10, backArmholeIntersection.Y + (armpitLength / 2));
        //        XPoint armhole2 = new XPoint(backArmholeShoulder.X + 15, backArmholeIntersection.Y + ((3 * armpitLength) / 4));

        //        XPoint armhole3 = new XPoint(backArmholeShoulder.X + (centerBreast1.X - backArmholeShoulder.X) / 2, backArmholeIntersection.Y + ((19 * armpitLength) / 20));

        //        // fornt
        //        XPoint armhole4 = new XPoint(frontArmholeBreast.X, armhole2.Y);

        //        // all armhole 
        //        gfx.DrawCurve(pen, new XPoint[] { backShoulder, armhole1, armhole2, armhole3, centerBreast1 });
        //        gfx.DrawCurve(pen, new XPoint[] { centerBreast2, armhole4, frontShoulder1 });

        //        // draw front shoulder tuck
        //        gfx.DrawLine(dashedPen, frontShoulder2, breastPoint);
        //        gfx.DrawLine(dashedPen, frontShoulder3, breastPoint);
        //        gfx.DrawLine(dashedPen, armhole4, breastPoint);

        //        // ---------------tucks------------------------------------
        //        double tuckFrontWidth = frontWidth - measures.circ_waist / 4;
        //        double allTucksBackWidth = allWidth - measures.circ_waist / 2 - tuckFrontWidth - patternGap;
        //        double tuckBackWidth = allTucksBackWidth / 3;

        //        // frot
        //        XPoint frontTuckUp = new XPoint(breastPoint.X, frontWaist.Y - 140);
        //        XPoint frontTuck1 = new XPoint(frontTuckUp.X - tuckFrontWidth / 2, frontWaist.Y);
        //        XPoint frontTuck2 = new XPoint(frontTuckUp.X + tuckFrontWidth / 2, frontWaist.Y);

        //        XPoint frontTuckDown1 = new XPoint(frontTuckUp.X - tuckFrontWidth / 4, frontHips.Y);
        //        XPoint frontTuckDown2 = new XPoint(frontTuckUp.X + tuckFrontWidth / 4, frontHips.Y);

        //        gfx.DrawLines(pen, new XPoint[] { frontTuckDown1, frontTuck1, frontTuckUp, frontTuck2, frontTuckDown2 });

        //        // center

        //        XPoint centerTuckFront = new XPoint(centerWaist2.X + tuckBackWidth / 2, centerWaist2.Y - 10);
        //        XPoint centerTuckBack = new XPoint(centerWaist1.X - tuckBackWidth / 2, centerWaist1.Y - 10);

        //        double downAddition = measures.circ_hips / 2 - (allWidth - tuckFrontWidth / 4 - patternGap) - 20;

        //        XPoint centerTuckBackDown = new XPoint(centerHips1.X + downAddition / 2, centerHips1.Y - 6);
        //        XPoint centerTuckFrontDown = new XPoint(centerHips2.X - downAddition / 2, centerHips2.Y - 6);

        //        gfx.DrawLines(pen, new XPoint[] { centerBreast1, centerTuckBack, centerTuckBackDown });
        //        gfx.DrawLines(pen, new XPoint[] { centerBreast2, centerTuckFront, centerTuckFrontDown });

        //        gfx.DrawCurve(pen, new XPoint[] { centerTuckBackDown, new XPoint(backHips.X + backWidth / 2, backHips.Y), backHips });
        //        gfx.DrawCurve(pen, new XPoint[] { centerTuckFrontDown, new XPoint(frontHips.X - frontWidth / 2, frontHips.Y), frontHips });

        //        //back 1
        //        double l1 = XPoint.Subtract(backBreast, backWaist).Length;
        //        double l2 = XPoint.Subtract(backHips, backWaist).Length;
        //        XPoint back1TuckUp = new XPoint(backBreast.X + backWidth / 2, backBreast.Y + l1 / 4);
        //        XPoint back1TuckDown = new XPoint(back1TuckUp.X, backHips.Y - l2 / 4);
        //        XPoint back1Tuck1 = new XPoint(back1TuckUp.X - tuckBackWidth / 2, backWaist.Y);
        //        XPoint back1Tuck2 = new XPoint(back1TuckUp.X + tuckBackWidth / 2, backWaist.Y);

        //        gfx.DrawLines(pen, new XPoint[] { back1TuckUp, back1Tuck1, back1TuckDown, back1Tuck2, back1TuckUp });

        //        //back 2
        //        XPoint back2TuckUp = new XPoint(backBreast.X + backWidth, backBreast.Y + l1 / 3);
        //        XPoint back2TuckDown = new XPoint(back2TuckUp.X, backHips.Y - l2 / 3);
        //        XPoint back2Tuck1 = new XPoint(back2TuckUp.X - tuckBackWidth / 2, backWaist.Y);
        //        XPoint back2Tuck2 = new XPoint(back2TuckUp.X + tuckBackWidth / 2, backWaist.Y);

        //        gfx.DrawLines(pen, new XPoint[] { back2TuckUp, back2Tuck1, back2TuckDown, back2Tuck2, back2TuckUp });

        //        // back saddle
        //        XPoint backSaddle1 = new XPoint(start.X, start.Y + 80);
        //        XPoint backSaddle2 = new XPoint(backSaddle1.X + (backWidth) / 2, backSaddle1.Y);
        //        XPoint backSaddle3 = new XPoint(backSaddle1.X + backWidth + 12, backSaddle1.Y);
        //        XPoint backSaddle4 = new XPoint(backSaddle3.X, backSaddle3.Y + 5);
        //        gfx.DrawLine(pen, backSaddle1, backSaddle3);
        //        gfx.DrawLine(pen, backSaddle2, backSaddle4);
        //    }

        //    static void Dress()
        //    {
        //        double frontNeckUpAddition = 45;

        //        double armpitLength = measures.height / 10 + measures.circ_bust / 20 + 3;

        //        double additionBack = 5;
        //        double backWidth = measures.wid_back / 2 + additionBack + 5;
        //        double lenShoulder = measures.len_shoulder + additionBack;

        //        double additionAll = 35;
        //        double allWidth;
        //        if (measures.circ_bust > 1000)
        //        {
        //            allWidth = measures.circ_bust / 2 + (measures.circ_bust - 1000) / 10 + additionAll;
        //        }
        //        else
        //        {
        //            allWidth = measures.circ_bust / 2 + additionAll;
        //        }

        //        double frontWidth = measures.circ_bust / 4 - 30;
        //        double armholeWidth = allWidth - frontWidth - backWidth;

        //        double lenBreast;

        //        if (measures.len_breast != 0)
        //        {
        //            lenBreast = measures.len_breast;
        //        }
        //        else
        //        {
        //            lenBreast = measures.circ_bust / 4 + 4;
        //        }

        //        // prepare pdf

        //        PdfPage p = AddPatternPage(measures.len_back + measures.len_knee + armpitLength, allWidth);
        //        XGraphics gfx = XGraphics.FromPdfPage(p, XGraphicsUnit.Millimeter);


        //        // main net 

        //        XPoint start = new XPoint(30, 30 + frontNeckUpAddition);
        //        XPoint backBreast = new XPoint(start.X, start.Y + armpitLength);
        //        XPoint backWaist = new XPoint(start.X, start.Y + measures.len_back);
        //        XPoint backHips = new XPoint(start.X, start.Y + measures.len_back + measures.len_hips);
        //        XPoint backKnee = new XPoint(start.X, start.Y + measures.len_back + measures.len_knee);

        //        double backDeflection = 20;

        //        XPoint backWaistDeflection = new XPoint(backWaist.X + backDeflection, backWaist.Y);
        //        XPoint backHipsDeflection = new XPoint(backHips.X + backDeflection, backHips.Y);
        //        XPoint backKneeDeflection = new XPoint(backKnee.X + backDeflection, backKnee.Y);

        //        double patternGap = 80;

        //        XPoint backArmholeBreast = new XPoint(backBreast.X + backWidth, backBreast.Y);
        //        XPoint centerBreast1 = new XPoint(backArmholeBreast.X + 2 * armholeWidth / 3, backArmholeBreast.Y);
        //        XPoint centerBreast2 = new XPoint(backArmholeBreast.X + 2 * armholeWidth / 3 + patternGap, backArmholeBreast.Y);
        //        XPoint frontArmholeBreast = new XPoint(backBreast.X + backWidth + armholeWidth + patternGap, backBreast.Y);
        //        XPoint frontBreast = new XPoint(backBreast.X + allWidth + patternGap, backBreast.Y);

        //        XPoint backArmholeShoulder = new XPoint(backArmholeBreast.X, start.Y);
        //        XPoint frontArmholeNeck = new XPoint(frontArmholeBreast.X, start.Y - frontNeckUpAddition);

        //        XPoint frontNeck = new XPoint(frontBreast.X, frontArmholeNeck.Y);

        //        XPoint frontWaist = new XPoint(frontBreast.X, backWaist.Y);
        //        XPoint frontHips = new XPoint(frontBreast.X, backHips.Y);
        //        XPoint frontKnee = new XPoint(frontBreast.X, backKnee.Y);

        //        XPoint centerWaist1 = new XPoint(centerBreast1.X, backWaist.Y);
        //        XPoint centerHips1 = new XPoint(centerBreast1.X, backHips.Y);
        //        XPoint centerKnee1 = new XPoint(centerBreast1.X, backKnee.Y);

        //        XPoint centerWaist2 = new XPoint(centerBreast2.X, backWaist.Y);
        //        XPoint centerHips2 = new XPoint(centerBreast2.X, backHips.Y);
        //        XPoint centerKnee2 = new XPoint(centerBreast2.X, backKnee.Y);



        //        gfx.DrawLine(pen, start, backKnee);
        //        gfx.DrawLine(pen, backArmholeShoulder, backArmholeBreast);
        //        gfx.DrawLine(pen, frontArmholeNeck, frontArmholeBreast);
        //        gfx.DrawLine(pen, centerBreast1, centerKnee1);
        //        gfx.DrawLine(pen, centerBreast2, centerKnee2);
        //        gfx.DrawLine(pen, frontNeck, frontKnee);
        //        gfx.DrawLine(pen, start, backWaistDeflection);
        //        gfx.DrawLine(pen, backWaistDeflection, backKneeDeflection);

        //        gfx.DrawLine(pen, start, backArmholeShoulder);
        //        gfx.DrawLine(pen, frontArmholeNeck, frontNeck);
        //        gfx.DrawLine(pen, backBreast, frontBreast);
        //        gfx.DrawLine(pen, backWaist, frontWaist);
        //        gfx.DrawLine(pen, backHips, frontHips);
        //        gfx.DrawLine(pen, backKnee, frontKnee);

        //        // final shape

        //        // back neck hole
        //        double backNeckHoleHeight = 20;
        //        double neckHoleWidth = measures.circ_bust / 20 + 23;
        //        XPoint backNeckHole = new XPoint(start.X + neckHoleWidth, start.Y - 20);

        //        // back shoulder
        //        double intersectionDown = 15;
        //        XPoint backArmholeIntersection = new XPoint(backArmholeShoulder.X, backArmholeShoulder.Y + intersectionDown);

        //        double lenAfterIntersection = lenShoulder - XPoint.Subtract(backNeckHole, backArmholeIntersection).Length + 10;
        //        double a = Math.Atan((intersectionDown + backNeckHoleHeight) / (backWidth - neckHoleWidth));

        //        double backShoulderYpos = backArmholeIntersection.Y + (Math.Sin(a) * lenAfterIntersection);
        //        double backShoulderXpos = backArmholeIntersection.X + (Math.Cos(a) * lenAfterIntersection);

        //        XPoint backShoulder = new XPoint(backShoulderXpos, backShoulderYpos);

        //        armpitLength -= intersectionDown;


        //        // front neck hole
        //        double neckHoleHeight = measures.circ_bust / 20 + 30;
        //        XPoint frontUpNeckHole = new XPoint(frontNeck.X - neckHoleWidth, frontNeck.Y);

        //        // front shoulder
        //        double help = measures.circ_bust / 20 - 5;

        //        XPoint frontShoulder1 = new XPoint(frontArmholeBreast.X - help, centerBreast2.Y - Math.Sqrt(Math.Abs(Math.Pow(armpitLength - 1.5, 2) - Math.Pow(help, 2))));


        //        XPoint frontBreastLineUp = new XPoint(frontBreast.X - (measures.circ_bust / 10 + 5), frontNeck.Y);
        //        XPoint breastPoint = new XPoint(frontBreastLineUp.X, frontBreastLineUp.Y + lenBreast);

        //        XPoint s1 = ShoulderCircleIntersection(frontShoulder1, breastPoint, lenShoulder, lenBreast);

        //        double shoulderUp = XPoint.Subtract(frontBreastLineUp, frontUpNeckHole).Length;

        //        XPoint frontShoulder2 = FindPointOnLine(frontShoulder1, s1, lenShoulder - shoulderUp);

        //        double len = XPoint.Subtract(frontShoulder2, breastPoint).Length;
        //        XPoint frontShoulder3 = new XPoint(breastPoint.X, breastPoint.Y - len);

        //        // translate shoulder seam
        //        ShiftLineParallel(ref frontShoulder1, ref frontShoulder2, gfx, pen, -10);
        //        ShiftLineParallel(ref backShoulder, ref backNeckHole, gfx, pen, -10);
        //        ShiftLineParallel(ref frontShoulder3, ref frontUpNeckHole, gfx, pen, -10);

        //        //draw neck hole - using shifted shnoulder seam
        //        neckHoleWidth = backNeckHole.X - start.X;
        //        backNeckHoleHeight = start.Y - backNeckHole.Y;
        //        neckHoleHeight = neckHoleHeight - (frontUpNeckHole.Y - frontNeck.Y);

        //        gfx.DrawArc(pen, new XRect(start.X - neckHoleWidth, start.Y - 2 * backNeckHoleHeight, neckHoleWidth * 2, backNeckHoleHeight * 2), 0, 90);
        //        gfx.DrawArc(pen, new XRect(frontUpNeckHole.X, frontUpNeckHole.Y - neckHoleHeight, neckHoleWidth * 2, neckHoleHeight * 2), 90, 90);


        //        //armhole 

        //        // back
        //        XPoint armhole1 = new XPoint(backArmholeShoulder.X + 10, backArmholeIntersection.Y + (armpitLength / 2));
        //        XPoint armhole2 = new XPoint(backArmholeShoulder.X + 15, backArmholeIntersection.Y + ((3 * armpitLength) / 4));

        //        XPoint armhole3 = new XPoint(backArmholeShoulder.X + (centerBreast1.X - backArmholeShoulder.X) / 2, backArmholeIntersection.Y + ((19 * armpitLength) / 20));

        //        // fornt
        //        XPoint armhole4 = new XPoint(frontArmholeBreast.X, armhole2.Y);

        //        // all armhole 
        //        gfx.DrawCurve(pen, new XPoint[] { backShoulder, armhole1, armhole2, armhole3, centerBreast1 });
        //        gfx.DrawCurve(pen, new XPoint[] { centerBreast2, armhole4, frontShoulder1 });

        //        // draw front shoulder tuck
        //        gfx.DrawLine(dashedPen, frontShoulder2, breastPoint);
        //        gfx.DrawLine(dashedPen, frontShoulder3, breastPoint);
        //        gfx.DrawLine(dashedPen, armhole4, breastPoint);





        //        // back saddle
        //        XPoint backSaddle1 = new XPoint(start.X, start.Y + 80);
        //        XPoint backSaddle2 = new XPoint(backSaddle1.X + (backWidth) / 2, backSaddle1.Y);
        //        XPoint backSaddle3 = new XPoint(backSaddle1.X + backWidth + 12, backSaddle1.Y);
        //        XPoint backSaddle4 = new XPoint(backSaddle3.X, backSaddle3.Y + 5);
        //        gfx.DrawLine(pen, backSaddle1, backSaddle3);
        //        gfx.DrawLine(pen, backSaddle2, backSaddle4);
        //    }
        //}


    abstract class Skirt : Pattern
    {
        readonly protected PdfDocument pdfDoc;

        readonly protected int widthWaist;
        readonly protected int widthHips;
        readonly protected int lenHips;
        readonly protected int lenKnee;
        readonly protected int centerUpAddition = 12;

        public Skirt(PdfDocument pdfDoc, int circWaist, int circHips, int lenHips, int lenKnee) : base()
        {
            this.pdfDoc = pdfDoc;
            this.widthHips = circHips / 2;
            this.widthWaist = circWaist / 2;
            this.lenHips = lenHips;
            this.lenKnee = lenKnee;
        }
        protected Dictionary<string, Dictionary<string, XPoint>> SkirtNet(XPoint start, int lenHips, int lenKnee, int widBack, int widFront, int patternGap)
        {
            Dictionary<string, Dictionary<string, XPoint>> net = new Dictionary<string, Dictionary<string, XPoint>>();

            Dictionary<string, XPoint> back = new Dictionary<string, XPoint>();

            back.Add("waist", start);
            back.Add("hips", PatternGeometry.ShiftOnePointVertical(start, lenHips));
            back.Add("down", PatternGeometry.ShiftOnePointVertical(start, lenKnee));

            Dictionary<string, XPoint> centerBack = ShiftPointsRight(back, widBack);
            Dictionary<string, XPoint> centerFront = ShiftPointsRight(centerBack, patternGap);
            Dictionary<string, XPoint> front = ShiftPointsRight(centerFront, widFront);

            net.Add("back", back);
            net.Add("centerFront", centerFront);
            net.Add("centerBack", centerBack);
            net.Add("front", front);

            return net;
        }

        protected static void DrawSkirtNet(XGraphics gfx, XPen p, Dictionary<string, Dictionary<string, XPoint>> n)
        {
            gfx.DrawLines(p, new XPoint[] { n["back"]["waist"], n["front"]["waist"], n["front"]["down"], n["back"]["down"], n["back"]["waist"] });
            gfx.DrawLine(p, n["back"]["hips"], n["front"]["hips"]);
            gfx.DrawLine(p, n["centerBack"]["waist"], n["centerBack"]["down"]);
            gfx.DrawLine(p, n["centerFront"]["waist"], n["centerFront"]["down"]);
        }

        protected XPoint FindUpPointHipsTuck(XPoint up, int upAddition, int tuckWidth, bool back)
        {
            int width = back ? -tuckWidth : tuckWidth;

            XPoint upTuck = PatternGeometry.ShiftOnePoint(up, width, -upAddition);

            return upTuck;
        }

        protected void DrawHipsTuck(XGraphics gfx, XPen p, XPoint up, XPoint upTuck, XPoint down)
        {
            XPoint beziere = PatternGeometry.ShiftOnePointVertical(up, lenHips / 2);

            gfx.DrawBezier(p, upTuck, upTuck, beziere, down);
        }

        protected Abscissa upTuckLine(int tuckWidth, XPoint center, XPoint upLine1, XPoint upLine2) // todo
        {
            XPoint l1;
            XPoint l2;
            int halfTuckWidth = tuckWidth / 2;

            // l1 je víc vlevo
            if (upLine1.X < upLine2.X) { l1 = upLine1; l2 = upLine2; }
            else { l2 = upLine1; l1 = upLine2; }

            int distX = (int)l2.X - (int)l1.X;
            int up1;
            int up2;

            if (l2.Y < l1.Y) // l2 je výš
            {
                int distY = (int)l1.Y - (int)l2.Y;
                up1 = ((int)center.X - (int)l1.X - halfTuckWidth) * distY / distX;
                up2 = ((int)center.X - (int)l1.X + halfTuckWidth) * distY / distX;
            }
            else
            {
                int distY = (int)l2.Y - (int)l1.Y;
                up1 = ((int)l2.X - (int)center.X + halfTuckWidth) * distY / distX;
                up2 = ((int)l2.X - (int)center.X - halfTuckWidth) * distY / distX;
            }

            XPoint tuckLine1 = PatternGeometry.ShiftOnePoint(center, -halfTuckWidth, -up1);
            XPoint tuckLine2 = PatternGeometry.ShiftOnePoint(center, halfTuckWidth, -up2);

            return new Abscissa(tuckLine1, tuckLine2);
        }

        protected void DrawTuck(XGraphics gfx, XPen pen, Abscissa upLine, XPoint downPoint)
        {
            gfx.DrawLines(pen, new XPoint[] { upLine.a, downPoint, upLine.b });
        }

        protected void DrawFrontBackTuckWithCenterLine(XGraphics gfx, XPen penTuck, XPen penLine, Abscissa upLine, XPoint downPoint, int downY)
        {
            DrawTuck(gfx, penTuck, upLine, downPoint);
            XPoint center = PatternGeometry.FindLineCenter(upLine);

            gfx.DrawLine(penLine, center, new XPoint(center.X, downY));
        }
    }

    class WideSkirt : Skirt, DrawingPattern
    {
        readonly XGraphics gfx;
        readonly int hipsAddition = 14;
        readonly int downAddition = 40;
        readonly int widthAll;

        public WideSkirt(PdfDocument pdfDoc, int circWaist, int circHips, int lenHips, int lenKnee)
            : base(pdfDoc, circWaist, circHips, lenHips, lenKnee)
        {
            gfx = createPdfGraphics(lenKnee, widthHips + downAddition, pdfDoc);
            widthAll = widthHips + hipsAddition;
        }

        public void drawPattern()
        {
            Dictionary<string, Dictionary<string, XPoint>> net = SkirtNet(PdfControl.start, lenHips, lenKnee, widthAll / 2, widthAll / 2, downAddition);
            DrawSkirtNet(gfx, dashedPen, net);

            // ------------------- TUCKS ------------------------

            int allTucksWidth = widthAll - widthWaist;
            int frontTuckWidth = 3 * allTucksWidth / 14;
            int backTuckWidth = 4 * allTucksWidth / 14;
            int centerTuckWidth = allTucksWidth / 2;

            int frontBackTuckDepth = lenHips - 30;

            // hips tuck
            XPoint centerUpBack = FindUpPointHipsTuck(net["centerBack"]["waist"], centerUpAddition, centerTuckWidth / 2, true);
            XPoint centerUpFront = FindUpPointHipsTuck(net["centerFront"]["waist"], centerUpAddition, centerTuckWidth / 2, false);

            // front tuck
            XPoint upFrontTuck = PatternGeometry.ShiftOnePointHorizontal(net["front"]["waist"], -(widthAll / 6));
            XPoint downFrontTuck = PatternGeometry.ShiftOnePointVertical(upFrontTuck, frontBackTuckDepth);

            Abscissa frontTuckLine = upTuckLine(frontTuckWidth, upFrontTuck, centerUpFront, net["front"]["waist"]);

            // back tuck
            XPoint upBackTuck = PatternGeometry.ShiftOnePointHorizontal(net["back"]["waist"], +(widthAll / 6));
            XPoint downBackTuck = PatternGeometry.ShiftOnePointVertical(upBackTuck, frontBackTuckDepth);

            Abscissa backTuckLine = upTuckLine(backTuckWidth, upBackTuck, centerUpBack, net["back"]["waist"]);

            // down edge
            XPoint downCenter = PatternGeometry.ShiftOnePointHorizontal(net["back"]["down"], (widthAll + downAddition) / 2);

            // ------------------- DRAW ------------------------
            gfx.DrawLines(pen, new XPoint[] { centerUpBack, net["back"]["waist"], net["back"]["down"], net["front"]["down"], net["front"]["waist"], centerUpFront });
            gfx.DrawLines(pen, new XPoint[] { net["centerBack"]["hips"], downCenter, net["centerFront"]["hips"] });
            DrawHipsTuck(gfx, pen, net["centerFront"]["waist"], centerUpFront, net["centerFront"]["hips"]);
            DrawHipsTuck(gfx, pen, net["centerBack"]["waist"], centerUpBack, net["centerBack"]["hips"]);
            DrawFrontBackTuckWithCenterLine(gfx, pen, grayPen, frontTuckLine, downFrontTuck, (int)net["back"]["down"].Y);
            DrawFrontBackTuckWithCenterLine(gfx, pen, grayPen, backTuckLine, downBackTuck, (int)net["back"]["down"].Y);
        }
    }

    class StraightSkirt : Skirt, DrawingPattern
    {
        readonly XGraphics gfx;
        readonly int hipsAddition = 12;
        readonly int widthAll;
        readonly int widthBack;
        readonly int widthFront;

        public StraightSkirt(PdfDocument pdfDoc, int circWaist, int circHips, int lenHips, int lenKnee)
            : base(pdfDoc, circWaist, circHips, lenHips, lenKnee)
        {
            widthBack = widthHips / 2;
            widthFront = widthHips / 2 + hipsAddition;
            widthAll = widthHips + hipsAddition;
            gfx = createPdfGraphics(lenKnee, widthAll, pdfDoc);
        }

        public void drawPattern()
        {
            Dictionary<string, Dictionary<string, XPoint>> net = SkirtNet(PdfControl.start, lenHips, lenKnee, widthBack, widthFront, 0);
            DrawSkirtNet(gfx, dashedPen, net);

            // ------------------- TUCKS ------------------------
            int allTucksWidth = widthAll - widthWaist;

            int frontTuckWidth = allTucksWidth / 7;
            int backTuck1Width = 3 * allTucksWidth / 14;
            int backTuck2Width = allTucksWidth / 7;

            int hipsTuckWidth = allTucksWidth / 2;
            int hipsTuckBackWidth = 3 * hipsTuckWidth / 7;
            int hipsTuckFrontWidth = 4 * hipsTuckWidth / 7;

            int frontTuckDepth = 9 * lenHips / 20;
            int backTuc1kDepth = 14 * lenHips / 20;
            int backTuck2Depth = 12 * lenHips / 20;

            int downTuckHalf = 10;

            // hips tuck
            XPoint centerUpBack = FindUpPointHipsTuck(net["centerBack"]["waist"], centerUpAddition, hipsTuckBackWidth, true);
            XPoint centerUpFront = FindUpPointHipsTuck(net["centerFront"]["waist"], centerUpAddition, hipsTuckFrontWidth, false);
            XPoint downCenterTuck = PatternGeometry.ShiftOnePointVertical(net["centerFront"]["hips"], -40);

            // front tuck
            int distFrontFrontTuck = widthWaist / 5 + 60;

            XPoint upFrontTuck = PatternGeometry.ShiftOnePointHorizontal(net["front"]["waist"], -distFrontFrontTuck);
            XPoint downFrontTuck = PatternGeometry.ShiftOnePointVertical(upFrontTuck, frontTuckDepth);

            Abscissa frontTuckLine = upTuckLine(frontTuckWidth, upFrontTuck, centerUpFront, net["front"]["waist"]);

            // back tuck 1
            XPoint upBackTuck1 = PatternGeometry.ShiftOnePointHorizontal(net["back"]["waist"], widthBack / 3);
            XPoint downBackTuck1 = PatternGeometry.ShiftOnePointVertical(upBackTuck1, backTuc1kDepth);

            Abscissa backTuck1Line = upTuckLine(backTuck1Width, upBackTuck1, centerUpBack, net["back"]["waist"]);

            // back tuck 2
            XPoint upBackTuck2 = PatternGeometry.ShiftOnePointHorizontal(net["back"]["waist"], 2 * widthBack / 3);
            XPoint downBackTuck2 = PatternGeometry.ShiftOnePointVertical(upBackTuck2, backTuck2Depth);

            Abscissa backTuck2Line = upTuckLine(backTuck2Width, upBackTuck2, centerUpBack, net["back"]["waist"]);

            // down tuck
            XPoint downTuckBack = PatternGeometry.ShiftOnePointHorizontal(net["centerBack"]["down"],- downTuckHalf);
            XPoint downTuckFront = PatternGeometry.ShiftOnePointHorizontal(net["centerBack"]["down"], downTuckHalf);

            // ------------------- DRAW ------------------------
            gfx.DrawLines(pen, new XPoint[] { centerUpBack, net["back"]["waist"], net["back"]["down"], net["front"]["down"], net["front"]["waist"], centerUpFront });
            gfx.DrawLines(pen, new XPoint[] { downTuckBack, net["centerBack"]["hips"], downTuckFront });
            DrawHipsTuck(gfx, pen, net["centerFront"]["waist"], centerUpFront, downCenterTuck);
            DrawHipsTuck(gfx, pen, net["centerBack"]["waist"], centerUpBack, downCenterTuck);
            DrawTuck(gfx, pen, frontTuckLine, downFrontTuck);
            DrawTuck(gfx, pen, backTuck1Line, downBackTuck1);
            DrawTuck(gfx, pen, backTuck2Line, downBackTuck2);

        }
    }
}

