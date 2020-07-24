using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

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
            if (len_front == 0) { this.len_front = len_back + 45; } else { this.len_front = len_front; }
            if (len_breast == 0) { this.len_breast = circ_bust / 4 + 40; } else { this.len_breast = len_breast; }
        }
    }
    public struct OrientedAbscissa
    {
        public XPoint start;
        public XPoint end;
        public double len;
        public OrientedAbscissa(XPoint a, XPoint b)
        {
            if (a.X < b.X || (a.X == b.X && a.Y < b.Y))
            {
                start = a;
                end = b;
            }
            else
            {
                start = b;
                end = a;
            }

            len = XPoint.Subtract(start, end).Length;
        }
    }
    class PatternControl
    {
        public enum Pattern
        {
            [Description("rovná sukně")] straightSkirt,
            [Description("rozšířená sukně")] wideSkirt,
            [Description("šaty")] dress,
            [Description("volná košile")] shirt,
            [Description("halenka")] blouse
        }

        MeasuresData m;
        bool visibleNet;
        public PatternControl(MeasuresData measures, bool visibleNet)
        {
            this.m = measures;
            this.visibleNet = visibleNet;
        }

        public string PdfPattern(Pattern pattern, string path)
        {
            string pathToPdf = path + "\\" + pattern.ToString() + ".pdf";
            PdfDocument pdfDoc = CreatePdfDocument(pattern);

            switch (pattern)
            {
                case Pattern.straightSkirt:
                    PatternDrawing sSkirt = new StraightSkirt(visibleNet, pdfDoc, m.circ_waist, m.circ_hips, m.len_hips, m.len_knee);
                    sSkirt.drawPattern();
                    break;
                case Pattern.wideSkirt:
                    PatternDrawing wSkirt 
                        = new WideSkirt(visibleNet, pdfDoc, m.circ_waist, m.circ_hips, m.len_hips, m.len_knee);
                    wSkirt.drawPattern();
                    break;
                case Pattern.dress:
                    PatternDrawing dress
                        = new Dress(visibleNet, pdfDoc, m.height, m.circ_bust, m.circ_waist, m.circ_hips, m.circ_neck, m.len_hips, m.len_front, m.len_back, m.wid_back, m.len_shoulder, m.len_breast, m.len_knee);
                    dress.drawPattern();
                    break;
                case Pattern.shirt:
                    PatternDrawing shirt 
                        = new LooseShirt(visibleNet, pdfDoc, m.height, m.circ_bust, m.circ_waist, m.circ_hips,m.circ_neck, m.len_hips, m.len_back, m.wid_back, m.len_shoulder, m.len_breast);
                    shirt.drawPattern();
                    break;
                case Pattern.blouse:
                    PatternDrawing blouse
                        = new Blouse(visibleNet, pdfDoc, m.height, m.circ_bust, m.circ_waist, m.circ_hips, m.circ_neck, m.len_hips, m.len_front, m.len_back, m.wid_back, m.len_shoulder, m.len_breast);
                    blouse.drawPattern();
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

        public static XPoint start = new XPoint(30, 85);

        public static void WriteBasicInfoToPdf(ref XGraphics gfx)
        {
            XFont font = new XFont("Arial", 12);

            DateTime now = DateTime.Now;
            string text = "Střih byl vygenerován " + now.ToString();
            gfx.DrawString(text, font, XBrushes.Black, 30, 40);

            XPen p = XPens.Black;
            gfx.DrawLine(p, new XPoint(30,50), new XPoint(130,50));

            for (int i = 30; i <= 130; i += 10)
            {
                gfx.DrawLine(p, new XPoint(i, 50), new XPoint(i, 55));
            }

            gfx.DrawString("10 cm", font, XBrushes.Black, 140, 55);
        }
    }

    interface PatternDrawing
    {
        void drawPattern();
    }

    static class PGeometry
    {
        public static int Perimetr(int circ)
        {
            return (int)(circ / (2* Math.PI));
        }
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

        public static void ShiftLineParallel(ref XPoint A, ref XPoint B, double offset)
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

        public static XPoint FindLineCenter(OrientedAbscissa line)
        {
            int dist = (int)XPoint.Subtract(line.start, line.end).Length;
            return FindPointOnLine(line.start, line.end, dist / 2);
        }
    }

    abstract class Pattern
    {
        readonly protected XPen pen;
        readonly protected XPen grayPen;
        readonly protected XPen dashedPen;
        readonly protected bool visibleNet;

        public Pattern(bool visibleNet)
        {
            this.visibleNet = visibleNet;
            pen = new XPen(XColors.Black);
            grayPen = new XPen(XColors.Gray);
            dashedPen = new XPen(XColors.Black);
            dashedPen.DashStyle = XDashStyle.Dash;
        }

        protected static XGraphics createPdfGraphics(int patternHeight, int patternWidth, PdfDocument pdfDoc)
        {
            PdfPage page = PdfControl.AddPage(patternHeight, patternWidth, pdfDoc);
            XGraphics g = XGraphics.FromPdfPage(page, XGraphicsUnit.Millimeter);
            PdfControl.WriteBasicInfoToPdf(ref g);
            return g;
        }

        protected static Dictionary<string, XPoint> ShiftPoints(Dictionary<string, XPoint> line, int distanceVertical, int distanceHorizontal, bool upDirection, bool leftDirection)
        {

            Dictionary<string, XPoint> newLine = new Dictionary<string, XPoint>();

            int distV = upDirection ? -distanceVertical : distanceVertical;
            int distH = leftDirection ? -distanceHorizontal : distanceHorizontal;

            foreach (KeyValuePair<string, XPoint> point in line)
            {
                XPoint p = PGeometry.ShiftOnePoint(point.Value, distH, distV);
                newLine.Add(point.Key, p);
            }

            return newLine;
        }

        protected static Dictionary<string, XPoint> ShiftPointsRight(Dictionary<string, XPoint> line, int dist)
        {
            return ShiftPoints(line, 0, dist, false, false);
        }

        protected static Dictionary<string, XPoint> ShiftPointsDown(Dictionary<string, XPoint> line, int dist)
        {
            return ShiftPoints(line, dist, 0, false, false);
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
        readonly protected int widthChest;
        readonly protected int widthWaist;
        readonly protected int widthHips;
        readonly protected int widthNeck;
        readonly protected int lenHips;
        readonly protected int lenBack;
        readonly protected int widthBack;
        readonly protected int lenShoulder;
        readonly protected int backDeflection = 20;

        public Bodice(bool visibleNet, PdfDocument pdfDoc, int height, int circChest, int circWaist, int circHips, int circNeck, int lenHips, int lenBack, int widthBack, int lenShoulder) : base(visibleNet)
        {
            this.pdfDoc = pdfDoc;

            this.height = height;
            this.widthChest = circChest / 2;
            this.widthWaist = circWaist / 2;
            this.widthHips = circHips / 2;
            this.lenHips = lenHips;
            this.lenBack = lenBack;
            this.widthBack = widthBack / 2 + backDeflection / 2;
            this.lenShoulder = lenShoulder;
            this.widthNeck = PGeometry.Perimetr(circNeck);
        }


        protected Dictionary<string, Dictionary<string, XPoint>> prepareBodiceNet
            (XPoint start, int lenBack, int lenHips, int lenBackArmpit, int lenFront, int lenAll, int widBack, int widArmhole, int widFront, int patternGap)
        {
            Dictionary<string, Dictionary<string, XPoint>> net = new Dictionary<string, Dictionary<string, XPoint>>();

            Dictionary<string, XPoint> chest = new Dictionary<string, XPoint>();

            int lenTorso = lenBack - lenBackArmpit;
            int lenFrontArmpit = lenFront - lenTorso;

            if (lenFrontArmpit < lenBackArmpit + 30) { chest.Add("back", PGeometry.ShiftOnePointVertical(start, lenBackArmpit + 30)); }
            else { chest.Add("back", PGeometry.ShiftOnePointVertical(start, lenFrontArmpit)); }
            
            chest.Add("armholeBack", PGeometry.ShiftOnePointHorizontal(chest["back"], widBack));
            chest.Add("centerBack", PGeometry.ShiftOnePointHorizontal(chest["armholeBack"], 2 * widArmhole / 3));
            chest.Add("armholeFront", PGeometry.ShiftOnePointHorizontal(chest["armholeBack"], widArmhole + patternGap));
            chest.Add("centerFront", PGeometry.ShiftOnePointHorizontal(chest["armholeFront"], - widArmhole / 3));
            chest.Add("front", PGeometry.ShiftOnePointHorizontal(chest["armholeFront"], widFront));

            Dictionary<string, XPoint> neckBack = ShiftPointsDown(
                   chest.Where(s => s.Key == "back" || s.Key == "armholeBack")
                        .ToDictionary(dict => dict.Key, dict => dict.Value),
                   -lenBackArmpit);


            Dictionary<string, XPoint> neckFront = ShiftPointsDown(
                  chest.Where(s => s.Key == "armholeFront" || s.Key == "front")
                       .ToDictionary(dict => dict.Key, dict => dict.Value),
                  -lenFrontArmpit);

            Dictionary<string, XPoint> neck = neckBack.Union(neckFront).ToDictionary(k => k.Key, v => v.Value);

            Dictionary<string, XPoint> waist = ShiftPointsDown(chest, lenTorso);
            Dictionary<string, XPoint> hips = ShiftPointsDown(waist, lenHips);
            Dictionary<string, XPoint> down = ShiftPointsDown(waist, lenAll - lenBack);

            net.Add("neck", neck);
            net.Add("chest", chest);
            net.Add("waist", waist);
            net.Add("hips", hips);
            net.Add("down", down);

            return net;
        }

        protected static void DrawBodiceNet(XGraphics gfx, XPen p, Dictionary<string, Dictionary<string, XPoint>> n)
        {
            gfx.DrawLine(p, n["neck"]["back"], n["neck"]["armholeBack"]);
            gfx.DrawLine(p, n["neck"]["front"], n["neck"]["armholeFront"]);

            gfx.DrawLine(p, n["neck"]["armholeBack"], n["chest"]["armholeBack"]);
            gfx.DrawLine(p, n["neck"]["armholeFront"], n["chest"]["armholeFront"]);

            gfx.DrawLines(p, new XPoint[] { n["neck"]["back"], n["down"]["back"], n["down"]["front"], n["neck"]["front"] });
            gfx.DrawLine(p, n["chest"]["back"], n["chest"]["front"]);
            gfx.DrawLine(p, n["waist"]["back"], n["waist"]["front"]);
            gfx.DrawLine(p, n["hips"]["back"], n["hips"]["front"]);

            gfx.DrawLine(p, n["chest"]["centerFront"], n["down"]["centerFront"]);
            gfx.DrawLine(p, n["chest"]["centerBack"], n["down"]["centerBack"]);
        }

        protected void drawContour(XGraphics gfx, XPen pen, XPoint start, XPoint backBreak, XPoint backHips, XPoint backDown, XPoint centerDown, XPoint frontDown, XPoint end)
        {
            backBreak = PGeometry.ShiftOnePointHorizontal(backBreak, backDeflection);

            int d1 = (int)(2 * (centerDown.X - backDown.X) / 3);
            XPoint h1 = PGeometry.ShiftOnePointHorizontal(backDown, d1);

            int d2 = (int)(2 * (frontDown.X - centerDown.X) / 3);
            XPoint h2 = PGeometry.ShiftOnePointHorizontal(frontDown, -d2);

            gfx.DrawLines(pen, new XPoint[] { start, backBreak, backHips, backDown, h1, centerDown, h2, frontDown, end});
        }

        protected static XPoint FrontShoulderCircleIntersection(XPoint A, XPoint B, double Ar, double Br)
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

        protected static void DrawTuckPart(XGraphics gfx, XPen pen, XPoint center, int wid, int widDown, int up, int down, bool left)
        {
            if (left)
            {
                wid = -wid;
                widDown = -widDown;
            }

            gfx.DrawLines(pen, new XPoint[] {
                                   PGeometry.ShiftOnePointVertical(center, -up),
                                   PGeometry.ShiftOnePointHorizontal(center, wid),
                                   PGeometry.ShiftOnePoint(center, widDown, down)
                                   });
        }

        protected static void DrawTuck(XGraphics gfx, XPen pen, XPoint center, int wid, int widDown, int up, int down)
        {
            DrawTuckPart(gfx, pen, center, wid / 2, widDown / 2, up, down, true);
            DrawTuckPart(gfx, pen, center, wid / 2, widDown / 2, up, down, false);
        }

        protected static OrientedAbscissa BackShoulderLine(XPoint neckNetPoint, XPoint shoulderNetPoint, int lenShoulder, int neckHoleWidth, int shoulderSlope)
        {
            int neckHoleHeight = 20;

            XPoint neckHole = PGeometry.ShiftOnePoint(neckNetPoint, neckHoleWidth, -neckHoleHeight);

            XPoint intersection = PGeometry.ShiftOnePointVertical(shoulderNetPoint, shoulderSlope);

            int lenAfterIntersection = (int)(lenShoulder - XPoint.Subtract(neckHole, intersection).Length) + 10;
            int backWidth = (int)(neckNetPoint.X - shoulderNetPoint.X);
            int a = (int)Math.Atan((shoulderSlope + neckHoleHeight) / (backWidth - neckHoleWidth));

            double shoulderY = intersection.Y + (Math.Sin(a) * lenAfterIntersection);
            double shoulderX = intersection.X + (Math.Cos(a) * lenAfterIntersection);

            XPoint shoulder = new XPoint(shoulderX, shoulderY);

            OrientedAbscissa backShoulder = new OrientedAbscissa(neckHole, shoulder);

            PGeometry.ShiftLineParallel(ref backShoulder.start, ref backShoulder.end, 10);

            return backShoulder;
        }

        protected static OrientedAbscissa findBackArmholeHelpPoints(XPoint shoulder, XPoint backArmhole)
        {
            double distX = shoulder.X - backArmhole.X < 30 ? 30 : shoulder.X - backArmhole.X;
            double distY = backArmhole.Y - shoulder.Y;
            XPoint a = PGeometry.ShiftOnePoint(shoulder, (int)(-2 * distX / 3), (int)(distY / 2));
            XPoint b = PGeometry.ShiftOnePoint(shoulder, (int)(- distX / 3), (int)(4 * distY / 5));

            return new OrientedAbscissa(a, b);
        }

        protected static void DrawBackNeckHole(XGraphics gfx, XPen pen, XPoint backShoulder, XPoint upperLeftNet)
        {
            double width = backShoulder.X - upperLeftNet.X;
            double height = upperLeftNet.Y - backShoulder.Y;
            XPoint upperLeft = PGeometry.ShiftOnePoint(backShoulder, (int)(-width * 2), (int)-height);
            XPoint downRight = PGeometry.ShiftOnePointVertical(backShoulder, (int)height);
            gfx.DrawArc(pen, new XRect(upperLeft, downRight), 0, 90);
        }

        protected static void DrawFrontNeckHole(XGraphics gfx, XPen pen, XPoint frontShoulder, XPoint upperRightNet, int height)
        {
            double width = upperRightNet.X - frontShoulder.X;
            XPoint upperLeft = PGeometry.ShiftOnePointVertical(frontShoulder, -height);
            XPoint downRight = PGeometry.ShiftOnePoint(frontShoulder, (int)(2 * width),height);
            gfx.DrawArc(pen, new XRect(upperLeft, downRight), 90, 90);
        }

    }

    class LooseShirt : Bodice, PatternDrawing
    {
        XGraphics gfx;
        readonly int lenFront;
        readonly int lenBackArmpit;
        readonly int widBack;
        readonly int widArmhole;
        readonly int widFront;
        readonly int widthChestWithAddition;
        readonly int lenShoulderWithAddition;

        public LooseShirt(bool visibleNet, PdfDocument pdfDoc, int height, int circChest, int circWaist, int circHips, int circNeck, int lenHips, int lenBack, int widthBack, int lenShoulder, int depthBreastPoint)
            : base(visibleNet, pdfDoc, height, circChest, circWaist, circHips, circNeck, lenHips, lenBack, widthBack, lenShoulder)
        {
            int chestAddition;
            int frontNeckUpAddition = widthChest / 10;

            if (circChest < 820) { chestAddition = 60; frontNeckUpAddition -= 10; }
            else if (circChest < 870) { chestAddition = 65; frontNeckUpAddition -= 5; }
            else if (circChest < 920) { chestAddition = 70; }
            else { chestAddition = 75; }

            widBack = this.widthBack + 3 * chestAddition / 13;
            lenShoulderWithAddition = Math.Min(this.lenShoulder + 3 * chestAddition / 13,160);
            widArmhole = widthChest / 4 - 15 + 6 * chestAddition / 13;
            widFront = widthChest / 2 - 40 + 4 * chestAddition / 13;

            widthChestWithAddition = widBack + widArmhole + widFront;

            lenBackArmpit = height / 10 + widthChest / 10 + 3;
            lenFront = lenBack + frontNeckUpAddition;
            gfx = createPdfGraphics(lenFront + lenHips, widthChestWithAddition, pdfDoc);
        }

        OrientedAbscissa FrontShoulder(XPoint neckNetPoint, XPoint armholeNetPoint, int neckHoleWidth, int lenShoulder, int lenArmpit)
        {

            XPoint frontUpNeckHole = PGeometry.ShiftOnePointHorizontal(neckNetPoint, -neckHoleWidth);
            XPoint frontS = FrontShoulderCircleIntersection(armholeNetPoint, frontUpNeckHole, lenArmpit, lenShoulder);

            OrientedAbscissa frontShoulder = new OrientedAbscissa(frontUpNeckHole, frontS);
            PGeometry.ShiftLineParallel(ref frontShoulder.start, ref frontShoulder.end, -10);

            return frontShoulder;
        }

        OrientedAbscissa findFrontArmohleHelpPoints(XPoint shoulder, XPoint armholeNetPoint)
        {

            double height = armholeNetPoint.Y - shoulder.Y;
            double refercneX;
            int addition;

            if (shoulder.X < armholeNetPoint.X)
            {
                refercneX = armholeNetPoint.X;
                addition = 0;
            }
            else
            {
                refercneX = shoulder.X;
                addition = 10;
            }

            XPoint a = new XPoint(refercneX + addition + 5, shoulder.Y + height / 2);
            XPoint b = new XPoint(refercneX + addition, shoulder.Y + 5 * height / 6);

            return new OrientedAbscissa(a, b);
        }

        void DrawShoulderPart(XGraphics gfx, XPen pen, OrientedAbscissa backShoulder, OrientedAbscissa frontShoulder, XPoint neckNetBack, XPoint neckNetFront)

        {
            gfx.DrawLine(pen, backShoulder.start, backShoulder.end);
            gfx.DrawLine(pen, frontShoulder.start, frontShoulder.end);

            DrawBackNeckHole(gfx, pen, backShoulder.start, neckNetBack);
            DrawFrontNeckHole(gfx, pen, frontShoulder.end, neckNetFront, widthNeck + 20);
        }

        void DrawArmhole(XGraphics gfx, XPen pen, XPoint start, XPoint center, XPoint end, OrientedAbscissa helpBack, OrientedAbscissa helpFront)
        {
            gfx.DrawCurve(pen, new XPoint[] { start,
                                              helpBack.start,
                                              helpBack.end,
                                              center,
                                              helpFront.start,
                                              helpFront.end,
                                              end});
        }

        public void drawPattern()
        {
            Dictionary<string, Dictionary<string, XPoint>> net 
                = prepareBodiceNet(PdfControl.start, lenBack, lenHips, lenBackArmpit, lenFront, lenBack + lenHips, widBack, widArmhole, widFront, 0);
            if (visibleNet) { DrawBodiceNet(gfx, dashedPen, net); }

            // -------------- final shape -----------------------------

            int shoulderSlope = 10;
            int armholeHeight = lenBackArmpit - shoulderSlope;

            OrientedAbscissa backShoulder = BackShoulderLine(net["neck"]["back"], net["neck"]["armholeBack"], lenShoulderWithAddition, widthNeck, shoulderSlope);

            OrientedAbscissa frontShoulder = FrontShoulder(net["neck"]["front"], net["chest"]["armholeFront"], widthNeck + 10, lenShoulderWithAddition, armholeHeight);
            

            OrientedAbscissa backArmholeHelpPoints = findBackArmholeHelpPoints(backShoulder.end, net["chest"]["armholeBack"]);
            OrientedAbscissa frontArmholeHelpPoints = findFrontArmohleHelpPoints(frontShoulder.start, net["chest"]["armholeFront"]);

            // -------------- draw final shape ------------------------

            DrawShoulderPart(gfx, pen, backShoulder, frontShoulder, net["neck"]["back"], net["neck"]["front"]);
            DrawArmhole(gfx, pen, backShoulder.end, net["chest"]["centerBack"], frontShoulder.start, backArmholeHelpPoints, frontArmholeHelpPoints);


            XPoint shiftedDownCenter = PGeometry.ShiftOnePointVertical(net["down"]["centerBack"], -5);

            drawContour(gfx, pen, net["neck"]["back"],
                net["down"]["back"],
                net["down"]["back"],
                net["down"]["back"],
                shiftedDownCenter,
                net["down"]["front"],
                new XPoint(net["neck"]["front"].X, frontShoulder.end.Y + widthNeck + 20));

            DrawTuck(gfx, pen, net["waist"]["centerBack"], 20, 0, lenBack - lenBackArmpit, lenHips - 5);

            // back saddle
            DrawLineParallel(net["neck"]["back"], net["neck"]["armholeBack"], gfx, grayPen, -80);
        }
    }

    class Blouse : Bodice, PatternDrawing
    {
        readonly XGraphics gfx;
        protected readonly int lenFront;
        protected readonly int lenBackArmpit;
        protected readonly int widBack;
        protected readonly int widArmhole;
        protected readonly int widFront;
        protected readonly int widthChestWithAddition;
        protected readonly int lenShoulderWithAddition;
        protected readonly int depthBreastPoint;
        protected readonly int widBreastPoint;
        readonly int patternGap;

        public Blouse(bool visibleNet, PdfDocument pdfDoc, int height, int circChest, int circWaist, int circHips, int circNeck, int lenHips, int lenFront, int lenBack, int widthBack, int lenShoulder, int depthBreastPoint)
            : base(visibleNet, pdfDoc, height, circChest, circWaist, circHips, circNeck, lenHips, lenBack, widthBack, lenShoulder)
        {
            this.lenFront = lenFront;
            this.depthBreastPoint = depthBreastPoint;

            int chestAddition = 35;

            if (circChest > 1000)
            {
                chestAddition += (circChest - 1000) / 10;
            }

            widBack = this.widthBack + chestAddition / 7;
            lenShoulderWithAddition = this.lenShoulder + chestAddition / 7;
            widArmhole = widthChest / 4 - 15 + 4 * chestAddition / 7;
            widFront = widthChest / 2 - 40 + 2 * chestAddition / 7;

            this.widBreastPoint = widthChest / 5 + 5;

            widthChestWithAddition = widBack + widArmhole + widFront;
            this.patternGap = widthHips - widthChest + (widFront - widthWaist / 2) / 2;

            lenBackArmpit = height / 10 + widthChest / 10 + 3;
            
            gfx = createPdfGraphics(lenFront + lenHips, widthChestWithAddition, pdfDoc);
        }

        protected static XPoint FindFrontShoulderPoint(XPoint breast, XPoint currentShoulder, int endXCoor)
        {
            int tuckLength = (int)XPoint.Subtract(currentShoulder, breast).Length;

            while (currentShoulder.X > endXCoor)
            {
                XPoint shoulderShifted = PGeometry.ShiftOnePointHorizontal(currentShoulder, -1);
                XPoint newShoulder = PGeometry.FindPointOnLine(breast, shoulderShifted, -tuckLength);
                currentShoulder = newShoulder;
            }

            return currentShoulder;
        }

        protected OrientedAbscissa FrontShoulderUp(XPoint neckNetPoint)
        {
            XPoint up = PGeometry.ShiftOnePointHorizontal(neckNetPoint, -widthNeck);
            XPoint left = PGeometry.ShiftOnePoint(neckNetPoint, -widBreastPoint, 8);

            OrientedAbscissa frontShoulderUp = new OrientedAbscissa(up, left);
            PGeometry.ShiftLineParallel(ref frontShoulderUp.start, ref frontShoulderUp.end, -10);

            return frontShoulderUp;
        }

        protected OrientedAbscissa FrontShoulder(OrientedAbscissa frontShoulderUp, XPoint armholeNetPoint, XPoint breastPoint)
        {
            int tuckLength = (int)XPoint.Subtract(frontShoulderUp.end, breastPoint).Length;

            int endXCoor = (int)armholeNetPoint.X - (widthChest / 10 - 7);

            XPoint startBackPoint = PGeometry.FindPointOnLine(frontShoulderUp.start, frontShoulderUp.end, -lenShoulder);
            XPoint backPoint = FindFrontShoulderPoint(breastPoint, startBackPoint, endXCoor);

            int lenShoulderRemaider = lenShoulder - (int)frontShoulderUp.len;

            XPoint upPoint = FrontShoulderCircleIntersection(backPoint, breastPoint, lenShoulderRemaider, tuckLength);

            OrientedAbscissa frontShoulder = new OrientedAbscissa(backPoint, upPoint);
            PGeometry.ShiftLineParallel(ref frontShoulder.start, ref frontShoulder.end, -10);

            return frontShoulder;
        }

        protected void drawShoulderPart(XGraphics gfx, XPen pen, OrientedAbscissa back, OrientedAbscissa frontUp, OrientedAbscissa front, XPoint breastPoint, XPoint neckNetBack, XPoint neckNetFront)
        {
            gfx.DrawLines(pen, new XPoint[] { front.start, front.end, breastPoint, frontUp.start, frontUp.end });
            gfx.DrawLine(pen, back.start, back.end);
            DrawBackNeckHole(gfx, pen, back.start, neckNetBack);
            DrawFrontNeckHole(gfx, pen, frontUp.end, neckNetFront, widthNeck + 20);
        }

        protected void drawArmhole(XGraphics gfx, XPen pen, XPoint start, XPoint end, XPoint centerBack, XPoint centerFront, OrientedAbscissa backHelp, XPoint frontHelp)
        {
            gfx.DrawCurve(pen, new XPoint[] { start, backHelp.start, backHelp.end, centerBack });
            gfx.DrawCurve(pen, new XPoint[] { centerFront, frontHelp, end });
        }

        protected void drawBackSaddle(XGraphics gfx, XPen pen, OrientedAbscissa shoulder, XPoint neck, int depth)
        {
            XPoint l = PGeometry.ShiftOnePointVertical(neck, depth);
            XPoint r = new XPoint(shoulder.end.X - 10, l.Y);
            gfx.DrawLine(pen, l, r);

            XPoint up = PGeometry.FindPointOnLine(shoulder.start, shoulder.end, 40);
            XPoint down = new XPoint(up.X, l.Y);
            gfx.DrawLine(pen, up, down);
        }


        virtual public void drawPattern()
        {
            Dictionary<string, Dictionary<string, XPoint>> net
                   = prepareBodiceNet(PdfControl.start, lenBack, lenHips, lenBackArmpit, lenFront, lenBack + lenHips, widBack, widArmhole, widFront, patternGap);

            if (visibleNet) { DrawBodiceNet(gfx, dashedPen, net); }

            // -------------- final shape -----------------------------

            int shoulderSlope = 15;
            int armholeHeight = lenBackArmpit - shoulderSlope;

            OrientedAbscissa backShoulder = BackShoulderLine(net["neck"]["back"], net["neck"]["armholeBack"], lenShoulderWithAddition, widthNeck, shoulderSlope);

            XPoint breastPoint = PGeometry.ShiftOnePoint(net["neck"]["front"], -widBreastPoint, depthBreastPoint);

            OrientedAbscissa frontShoulderUp = FrontShoulderUp(net["neck"]["front"]);
            //OrientedAbscissa frontShoulder = FrontShoulder(net["chest"]["armholeFront"], breastPoint, armholeHeight, (int)frontShoulderUp.len);
            OrientedAbscissa frontShoulder = FrontShoulder(frontShoulderUp, net["chest"]["armholeFront"], breastPoint);

            OrientedAbscissa backArmholeHelpPoints = findBackArmholeHelpPoints(backShoulder.end, net["chest"]["armholeBack"]);
            XPoint armholeFrontHelpPoint = new XPoint(net["chest"]["armholeFront"].X + 10, backArmholeHelpPoints.end.Y - 15);

            XPoint downCenter = PGeometry.ShiftOnePointVertical(PGeometry.FindLineCenter(new OrientedAbscissa(net["down"]["centerBack"], net["down"]["centerFront"])), -7);

            // -------------- draw final shape -----------------------------
            drawShoulderPart(gfx, pen, backShoulder, frontShoulderUp, frontShoulder, breastPoint, net["neck"]["back"], net["neck"]["front"]);
            drawArmhole(gfx, pen, backShoulder.end, frontShoulder.start, net["chest"]["centerBack"], net["chest"]["centerFront"], backArmholeHelpPoints, armholeFrontHelpPoint);

            drawContour(gfx, pen,
                net["neck"]["back"],
                net["waist"]["back"],
                net["hips"]["back"],
                net["down"]["back"],
                downCenter,
                net["down"]["front"],
                new XPoint(net["neck"]["front"].X, frontShoulder.end.Y + widthNeck + 20));

            gfx.DrawLine(grayPen, breastPoint, armholeFrontHelpPoint);

            // draw tucks

            int tuckFrontWidth = widFront - (widthWaist / 2) + 10;
            int allTucksBackWidth = (widthChestWithAddition - tuckFrontWidth - backDeflection) - (widthWaist + 30);
            int tuckBackWidth = allTucksBackWidth / 3;

            XPoint centerTuckBack = PGeometry.ShiftOnePoint(net["waist"]["centerBack"], -tuckBackWidth / 2, -10);
            XPoint centerTuckFront = PGeometry.ShiftOnePoint(net["waist"]["centerFront"], tuckBackWidth / 2, -10);
            gfx.DrawLines(pen, new XPoint[] { net["chest"]["centerBack"], centerTuckBack, downCenter, centerTuckFront, net["chest"]["centerFront"] });

            XPoint frontTuck = new XPoint(breastPoint.X, net["waist"]["front"].Y);
            DrawTuck(gfx, pen, frontTuck, tuckFrontWidth, tuckFrontWidth / 2, 140, lenHips);

            int lenTorso = lenBack - lenBackArmpit;
            XPoint backTuck1 = PGeometry.ShiftOnePointVertical(net["waist"]["armholeBack"], -5);
            DrawTuck(gfx, pen, backTuck1, tuckBackWidth, 0, 3 * lenTorso / 4, 3 * lenHips / 4);
            XPoint backTuck2 = PGeometry.ShiftOnePointHorizontal(net["waist"]["armholeBack"], -widthBack/2);
            DrawTuck(gfx, pen, backTuck2, tuckBackWidth, 0, 2 * lenTorso / 3, 2 * lenHips / 3);

            // back saddle
            drawBackSaddle(gfx, grayPen, backShoulder, net["neck"]["back"], 80);
        }
    }

    class Dress : Blouse, PatternDrawing
    {
        readonly int lenPattern;
        readonly int patternGap;
        readonly XGraphics gfx;
        readonly int downAddition = 20;
        public Dress(bool visibleNet, PdfDocument pdfDoc, int height, int circChest, int circWaist, int circHips, int circNeck, int lenHips, int lenFront, int lenBack, int widthBack, int lenShoulder, int depthBreastPoint, int lenKnee)
            : base(visibleNet, pdfDoc, height, circChest, circWaist, circHips, circNeck, lenHips, lenFront, lenBack, widthBack, lenShoulder, depthBreastPoint)
        {
            lenPattern = lenKnee + lenBack;
            this.patternGap = widthHips - widthChest + (widFront - widthWaist / 2) / 2 + 2 * downAddition;

            pdfDoc.Pages.RemoveAt(pdfDoc.Pages.Count-1); // todo: it is because the blouse contructor creates its own page, do it better
            gfx = createPdfGraphics(lenPattern, widthChestWithAddition, pdfDoc);
        }

        override public void drawPattern()
        {
            Dictionary<string, Dictionary<string, XPoint>> net
                   = prepareBodiceNet(PdfControl.start, lenBack, lenHips, lenBackArmpit, lenFront, lenPattern, widBack, widArmhole, widFront, patternGap);

            if (visibleNet) { DrawBodiceNet(gfx, dashedPen, net); }

            // -------------- final shape -----------------------------

            int shoulderSlope = 15;
            int armholeHeight = lenBackArmpit - shoulderSlope;

            OrientedAbscissa backShoulder = BackShoulderLine(net["neck"]["back"], net["neck"]["armholeBack"], lenShoulderWithAddition, widthNeck, shoulderSlope);

            XPoint breastPoint = PGeometry.ShiftOnePoint(net["neck"]["front"], -widBreastPoint, depthBreastPoint);

            OrientedAbscissa frontShoulderUp = FrontShoulderUp(net["neck"]["front"]);
            //OrientedAbscissa frontShoulder = FrontShoulder(net["chest"]["armholeFront"], breastPoint, armholeHeight, (int)frontShoulderUp.len);
            OrientedAbscissa frontShoulder = FrontShoulder(frontShoulderUp, net["chest"]["armholeFront"], breastPoint);

            OrientedAbscissa backArmholeHelpPoints = findBackArmholeHelpPoints(backShoulder.end, net["chest"]["armholeBack"]);
            XPoint armholeFrontHelpPoint = new XPoint(net["chest"]["armholeFront"].X, backArmholeHelpPoints.end.Y - 15);

            XPoint downCenter = PGeometry.ShiftOnePointVertical(PGeometry.FindLineCenter(new OrientedAbscissa(net["down"]["centerBack"], net["down"]["centerFront"])), -7);

            // -------------- draw final shape -----------------------------
            drawShoulderPart(gfx, pen, backShoulder, frontShoulderUp, frontShoulder, breastPoint, net["neck"]["back"], net["neck"]["front"]);
            drawArmhole(gfx, pen, backShoulder.end, frontShoulder.start, net["chest"]["centerBack"], net["chest"]["centerFront"], backArmholeHelpPoints, armholeFrontHelpPoint);

            drawContour(gfx, pen,
                net["neck"]["back"],
                net["waist"]["back"],
                net["hips"]["back"],
                net["down"]["back"],
                downCenter,
                net["down"]["front"],
                new XPoint(net["neck"]["front"].X, frontShoulder.end.Y + widthNeck + 20));

            gfx.DrawLine(grayPen, breastPoint, armholeFrontHelpPoint);

            // draw tucks

            int tuckFrontWidth = widFront - (widthWaist / 2) + 10;
            int allTucksBackWidth = (widthChestWithAddition - tuckFrontWidth - backDeflection) - (widthWaist + 30);
            int tuckBackWidth = allTucksBackWidth / 2;
            int hipsAddition = Math.Min(widthHips - widthChest, 10);

            XPoint centerTuckBack = PGeometry.ShiftOnePoint(net["waist"]["centerBack"], -tuckBackWidth / 2, -10);
            XPoint centerTuckFront = PGeometry.ShiftOnePoint(net["waist"]["centerFront"], tuckBackWidth / 2, -10);
            gfx.DrawLine(pen, net["chest"]["centerBack"], centerTuckBack);
            gfx.DrawLine(pen, net["chest"]["centerFront"], centerTuckFront);

            XPoint centerHipsBack = PGeometry.ShiftOnePointHorizontal(net["hips"]["centerBack"], hipsAddition);
            XPoint centerHipsFront = PGeometry.ShiftOnePointHorizontal(net["hips"]["centerFront"], -hipsAddition);

            gfx.DrawCurve(pen, new XPoint[] { centerTuckBack, centerHipsBack, downCenter });
            gfx.DrawCurve(pen, new XPoint[] { centerTuckFront, centerHipsFront, downCenter });

            XPoint frontTuck = new XPoint(breastPoint.X, net["waist"]["front"].Y);
            DrawTuck(gfx, pen, frontTuck, tuckFrontWidth, 0, 140, 3 * lenHips / 4);

            int lenTorso = lenBack - lenBackArmpit;
            XPoint backTuck = PGeometry.ShiftOnePointHorizontal(net["waist"]["armholeBack"], -widBack / 3);
            DrawTuck(gfx, pen, backTuck, tuckBackWidth, 0, 3 * lenTorso / 4, 3 * lenHips / 4);

            // back saddle
            drawBackSaddle(gfx, grayPen, backShoulder, net["neck"]["back"], 80);
        }
    }

        abstract class Skirt : Pattern
    {
        readonly protected PdfDocument pdfDoc;

        readonly protected int widthWaist;
        readonly protected int widthHips;
        readonly protected int lenHips;
        readonly protected int lenKnee;
        readonly protected int centerUpAddition = 12;

        public Skirt(bool visibleNet, PdfDocument pdfDoc, int circWaist, int circHips, int lenHips, int lenKnee) : base(visibleNet)
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
            back.Add("hips", PGeometry.ShiftOnePointVertical(start, lenHips));
            back.Add("down", PGeometry.ShiftOnePointVertical(start, lenKnee));

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

            XPoint upTuck = PGeometry.ShiftOnePoint(up, width, -upAddition);

            return upTuck;
        }

        protected void DrawHipsTuck(XGraphics gfx, XPen p, XPoint up, XPoint upTuck, XPoint down)
        {
            XPoint beziere = PGeometry.ShiftOnePointVertical(up, lenHips / 2);

            gfx.DrawBezier(p, upTuck, upTuck, beziere, down);
        }

        protected OrientedAbscissa upTuckLine(int tuckWidth, XPoint center, XPoint upLine1, XPoint upLine2)
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

            XPoint tuckLine1 = PGeometry.ShiftOnePoint(center, -halfTuckWidth, -up1);
            XPoint tuckLine2 = PGeometry.ShiftOnePoint(center, halfTuckWidth, -up2);

            return new OrientedAbscissa(tuckLine1, tuckLine2);
        }

        protected void DrawTuck(XGraphics gfx, XPen pen, OrientedAbscissa upLine, XPoint downPoint)
        {
            gfx.DrawLines(pen, new XPoint[] { upLine.start, downPoint, upLine.end });
        }

        protected void DrawFrontBackTuckWithCenterLine(XGraphics gfx, XPen penTuck, XPen penLine, OrientedAbscissa upLine, XPoint downPoint, int downY)
        {
            DrawTuck(gfx, penTuck, upLine, downPoint);
            XPoint center = PGeometry.FindLineCenter(upLine);

            gfx.DrawLine(penLine, center, new XPoint(center.X, downY));
        }
    }

    class WideSkirt : Skirt, PatternDrawing
    {
        readonly XGraphics gfx;
        readonly int hipsAddition = 14;
        readonly int downAddition = 40;
        readonly int widthAll;

        public WideSkirt(bool visibleNet, PdfDocument pdfDoc, int circWaist, int circHips, int lenHips, int lenKnee)
            : base(visibleNet, pdfDoc, circWaist, circHips, lenHips, lenKnee)
        {
            gfx = createPdfGraphics(lenKnee, widthHips + downAddition, pdfDoc);
            widthAll = widthHips + hipsAddition;
        }

        public void drawPattern()
        {
            Dictionary<string, Dictionary<string, XPoint>> net = SkirtNet(PdfControl.start, lenHips, lenKnee, widthAll / 2, widthAll / 2, downAddition);
            if (visibleNet) { DrawSkirtNet(gfx, dashedPen, net); }

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
            XPoint upFrontTuck = PGeometry.ShiftOnePointHorizontal(net["front"]["waist"], -(widthAll / 6));
            XPoint downFrontTuck = PGeometry.ShiftOnePointVertical(upFrontTuck, frontBackTuckDepth);

            OrientedAbscissa frontTuckLine = upTuckLine(frontTuckWidth, upFrontTuck, centerUpFront, net["front"]["waist"]);

            // back tuck
            XPoint upBackTuck = PGeometry.ShiftOnePointHorizontal(net["back"]["waist"], +(widthAll / 6));
            XPoint downBackTuck = PGeometry.ShiftOnePointVertical(upBackTuck, frontBackTuckDepth);

            OrientedAbscissa backTuckLine = upTuckLine(backTuckWidth, upBackTuck, centerUpBack, net["back"]["waist"]);

            // down edge
            XPoint downCenter = PGeometry.ShiftOnePointHorizontal(net["back"]["down"], (widthAll + downAddition) / 2);

            // ------------------- DRAW ------------------------
            gfx.DrawLines(pen, new XPoint[] { centerUpBack, net["back"]["waist"], net["back"]["down"], net["front"]["down"], net["front"]["waist"], centerUpFront });
            gfx.DrawLines(pen, new XPoint[] { net["centerBack"]["hips"], downCenter, net["centerFront"]["hips"] });
            DrawHipsTuck(gfx, pen, net["centerFront"]["waist"], centerUpFront, net["centerFront"]["hips"]);
            DrawHipsTuck(gfx, pen, net["centerBack"]["waist"], centerUpBack, net["centerBack"]["hips"]);
            DrawFrontBackTuckWithCenterLine(gfx, pen, grayPen, frontTuckLine, downFrontTuck, (int)net["back"]["down"].Y);
            DrawFrontBackTuckWithCenterLine(gfx, pen, grayPen, backTuckLine, downBackTuck, (int)net["back"]["down"].Y);
        }
    }

    class StraightSkirt : Skirt, PatternDrawing
    {
        readonly XGraphics gfx;
        readonly int hipsAddition = 12;
        readonly int widthAll;
        readonly int widthBack;
        readonly int widthFront;

        public StraightSkirt(bool visibleNet, PdfDocument pdfDoc, int circWaist, int circHips, int lenHips, int lenKnee)
            : base(visibleNet, pdfDoc, circWaist, circHips, lenHips, lenKnee)
        {
            widthBack = widthHips / 2;
            widthFront = widthHips / 2 + hipsAddition;
            widthAll = widthHips + hipsAddition;
            gfx = createPdfGraphics(lenKnee, widthAll, pdfDoc);
        }

        public void drawPattern()
        {
            Dictionary<string, Dictionary<string, XPoint>> net = SkirtNet(PdfControl.start, lenHips, lenKnee, widthBack, widthFront, 0);
            if (visibleNet) { DrawSkirtNet(gfx, dashedPen, net); }

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
            XPoint downCenterTuck = PGeometry.ShiftOnePointVertical(net["centerFront"]["hips"], -40);

            // front tuck
            int distFrontFrontTuck = widthWaist / 5 + 60;

            XPoint upFrontTuck = PGeometry.ShiftOnePointHorizontal(net["front"]["waist"], -distFrontFrontTuck);
            XPoint downFrontTuck = PGeometry.ShiftOnePointVertical(upFrontTuck, frontTuckDepth);

            OrientedAbscissa frontTuckLine = upTuckLine(frontTuckWidth, upFrontTuck, centerUpFront, net["front"]["waist"]);

            // back tuck 1
            XPoint upBackTuck1 = PGeometry.ShiftOnePointHorizontal(net["back"]["waist"], widthBack / 3);
            XPoint downBackTuck1 = PGeometry.ShiftOnePointVertical(upBackTuck1, backTuc1kDepth);

            OrientedAbscissa backTuck1Line = upTuckLine(backTuck1Width, upBackTuck1, centerUpBack, net["back"]["waist"]);

            // back tuck 2
            XPoint upBackTuck2 = PGeometry.ShiftOnePointHorizontal(net["back"]["waist"], 2 * widthBack / 3);
            XPoint downBackTuck2 = PGeometry.ShiftOnePointVertical(upBackTuck2, backTuck2Depth);

            OrientedAbscissa backTuck2Line = upTuckLine(backTuck2Width, upBackTuck2, centerUpBack, net["back"]["waist"]);

            // down tuck
            XPoint downTuckBack = PGeometry.ShiftOnePointHorizontal(net["centerBack"]["down"],- downTuckHalf);
            XPoint downTuckFront = PGeometry.ShiftOnePointHorizontal(net["centerBack"]["down"], downTuckHalf);

            // ------------------- DRAW ------------------------
            gfx.DrawLines(pen, new XPoint[] { centerUpBack, net["back"]["waist"], net["back"]["down"], net["front"]["down"], net["front"]["waist"], centerUpFront });
            gfx.DrawLines(pen, new XPoint[] { downTuckBack, net["centerBack"]["hips"], downTuckFront });
            DrawHipsTuck(gfx, pen, net["centerFront"]["waist"], centerUpFront, downCenterTuck);
            DrawHipsTuck(gfx, pen, net["centerBack"]["waist"], centerUpBack, downCenterTuck);
            gfx.DrawLine(pen, downCenterTuck, net["centerBack"]["hips"]);
            DrawTuck(gfx, pen, frontTuckLine, downFrontTuck);
            DrawTuck(gfx, pen, backTuck1Line, downBackTuck1);
            DrawTuck(gfx, pen, backTuck2Line, downBackTuck2);

        }
    }
}

