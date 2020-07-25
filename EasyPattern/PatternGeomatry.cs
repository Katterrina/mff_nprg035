using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace EasyPattern
{

    /// <summary>
    /// MeasuresData structure holds body measures as is,
    /// whole circumferential measures, without additions.
    /// </summary>
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

            // if circ_sleeve, len_front, len_breast measures are not set, default values are used
            if (circ_sleeve == 0) { this.circ_sleeve = 22; } else { this.circ_sleeve = circ_sleeve; }
            if (len_front == 0) { this.len_front = len_back + 45; } else { this.len_front = len_front; }
            if (len_breast == 0) { this.len_breast = circ_bust / 4 + 40; } else { this.len_breast = len_breast; }
        }
    }

    /// <summary>
    /// OrientedAbscissa represents abscissa from XPiont start to
    /// XPiont end. Abscissa is oriented from left to right, so start
    /// point has smaller x coordinate, if x coordinates are same,
    /// then start point has smaller y coordinate.
    /// OrientedAbscissa.len is length from start to end.
    /// </summary>
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

    /// <summary>
    /// PatternControl class provides PdfPattern method to choose pattern and draw it to pdf.
    /// </summary>
    class PatternControl
    {
        MeasuresData m;
        bool visibleNet;
        public PatternControl(MeasuresData measures, bool visibleNet)
        {
            this.m = measures;
            this.visibleNet = visibleNet;
        }

        /// <summary>
        /// Enumeration of Pattern options.
        /// </summary>
        public enum Pattern
        {
            [Description("rovná sukně")] straightSkirt,
            [Description("rozšířená sukně")] wideSkirt,
            [Description("šaty")] dress,
            [Description("volná košile")] shirt,
            [Description("halenka")] blouse
        }

        /// <summary>
        /// PdfPattern method choose pattern according to the pattern parameter and draw it to pdf.
        /// </summary>
        /// <param name="pattern">enum Pattern option</param>
        /// <param name="path">string path to directory to save pdf with pattern</param>
        /// <returns>string path to resulting pdf</returns>
        public string PdfPattern(Pattern pattern, string path)
        {
            // filename is composited from pattern name and current date and time
            string filename = pattern.ToString() + "_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".pdf";
            string pathToPdf = path + "\\" + filename;

            // prepare pdf document
            PdfDocument pdfDoc = PdfControl.CreatePdfDocument(pattern.ToString());

            
            IPatternDrawing patternObjectToDraw;
            // initialize what to draw according to the pattern option
            switch (pattern)
            {
                case Pattern.straightSkirt:
                    patternObjectToDraw
                        = new StraightSkirt(visibleNet, pdfDoc, m.circ_waist, m.circ_hips, m.len_hips, m.len_knee);
                    break;

                case Pattern.wideSkirt:
                    patternObjectToDraw
                        = new WideSkirt(visibleNet, pdfDoc, m.circ_waist, m.circ_hips, m.len_hips, m.len_knee);
                    break;

                case Pattern.dress:
                    patternObjectToDraw
                        = new Dress(visibleNet, pdfDoc, m.height, m.circ_bust, m.circ_waist, m.circ_hips, m.circ_neck,
                                    m.len_hips, m.len_front, m.len_back, m.wid_back, m.len_shoulder, m.len_breast, m.len_knee);
                    break;

                case Pattern.shirt:
                    patternObjectToDraw
                        = new LooseShirt(visibleNet, pdfDoc, m.height, m.circ_bust, m.circ_waist, m.circ_hips,m.circ_neck,
                                         m.len_hips, m.len_back, m.wid_back, m.len_shoulder, m.len_breast);
                    break;

                case Pattern.blouse:
                    patternObjectToDraw
                        = new Blouse(visibleNet, pdfDoc, m.height, m.circ_bust, m.circ_waist, m.circ_hips, m.circ_neck,
                                     m.len_hips, m.len_front, m.len_back, m.wid_back, m.len_shoulder, m.len_breast);
                    break;

                default:
                    patternObjectToDraw = new PatternDrawingError();
                    break;
            }

            // draw pattern
            patternObjectToDraw.DrawPattern();

            // save pdf
            PdfControl.SaveClosePdf(pdfDoc, pathToPdf);

            return pathToPdf;
        }
    }

    /// <summary>
    /// PdfControl class contains methods to manupulation with the pdf pattern document.
    /// </summary>
    class PdfControl
    {
        // XPoint start is constant determining where pattern drawing starts.
        public static XPoint start = new XPoint(30, 60);

        /// <summary>
        /// Creates PdfDocument, set title, author and keywords.
        /// </summary>
        /// <param name="pattern">pattern name as string</param>
        /// <returns>PdfDocument</returns>
        public static PdfDocument CreatePdfDocument(string pattern)
        {
            PdfDocument pdfDoc = new PdfDocument();
            pdfDoc.Info.Title = "EasyPattern";
            pdfDoc.Info.Author = "Kateřina Č.";
            pdfDoc.Info.Keywords = pattern;

            return pdfDoc;
        }

        /// <summary>
        /// Save pdf document pdfDoc to the path.
        /// </summary>
        /// <param name="pdfDoc">PdfDocument</param>
        /// <param name="path">string path to file</param>
        public static void SaveClosePdf(PdfDocument pdfDoc, string path)
        {
            pdfDoc.Save(path);
            pdfDoc.Close();
        }

        /// <summary>
        /// Add page to pdf document, its size is the smallest possible according to
        /// minimum required height and width.
        /// </summary>
        /// <param name="minHeight">minimum required height of resulting page in mm</param>
        /// <param name="minWidth">minimum required width of resulting page in mm</param>
        /// <param name="pdfDoc">PdfDocument</param>
        /// <returns></returns>
        public static PdfPage AddPage(double minHeight, double minWidth, PdfDocument pdfDoc)
        {
            PdfPage page = pdfDoc.AddPage();

            page.Size = PageSize.A0;

            if (minWidth > minHeight)
            {
                page.Orientation = PageOrientation.Landscape;

                if (minHeight < 820 && minWidth < 550)
                {
                    page.Size = PageSize.A1;

                }
            }
            else
            {
                page.Orientation = PageOrientation.Portrait;

                if (minHeight < 550 && minWidth < 8240)
                {
                    page.Size = PageSize.A1;
                }
            }

            return page;
        }

        /// <summary>
        /// Draw scale line to XGraphics in pattern pdf.
        /// </summary>
        /// <param name="graphics"></param>
        public static void DrawScale(ref XGraphics graphics)
        {
            XPen p = XPens.Black;
            graphics.DrawLine(p, new XPoint(30,30), new XPoint(130,30));

            for (int i = 30; i <= 130; i += 10)
            {
                graphics.DrawLine(p, new XPoint(i, 30), new XPoint(i, 35));
            }

            graphics.DrawString("10 cm", new XFont("Arial", 12), XBrushes.Black, 140, 35);
        }
    }


    /// <summary>
    /// PGeometry class provides general geometry methods.
    /// </summary>
    static class PGeometry
    {
        /// <summary>
        /// Calculates the diameter of a circle from its circumference.
        /// </summary>
        /// <param name="circumference">int circle circumference</param>
        /// <returns>int perimetr of the circle</returns>
        public static int Perimetr(int circumference)
        {
            return (int)(circumference / (2* Math.PI));
        }

        /// <summary>
        /// Returns a point at the given position to the given point.
        /// </summary>
        /// <param name="point">XPoint point</param>
        /// <param name="distRight">int distance right, negative if required position is left</param>
        /// <param name="distDown">int distance down, negative if required position is up</param>
        /// <returns>XPoint</returns>
        public static XPoint PointInRelativePosition(XPoint point, int distRight, int distDown)
        {
            return new XPoint(point.X + distRight, point.Y + distDown);
        }

        /// <summary>
        /// Returns a point in given horizontal distance from p.
        /// </summary>
        /// <param name="p">XPoint</param>
        /// <param name="distRight">int distance right, negative if required position is left</param>
        /// <returns>XPoint</returns>
        public static XPoint PointInRelativePositionHorizontal(XPoint p, int distRight)
        {
            return PointInRelativePosition(p, distRight, 0);
        }

        /// <summary>
        /// Returns a point in given vertical distance from p.
        /// </summary>
        /// <param name="p">XPoint</param>
        /// <param name="distDown">int distance down, negative if required position is up</param>
        /// <returns>XPoint</returns>
        public static XPoint PointInRelativePositionVertical(XPoint p, int distDown)
        {
            return PointInRelativePosition(p, 0, distDown);
        }

        /// <summary>
        /// Shifts the line by the specified offset in perpendicular direction.
        /// </summary>
        /// <param name="A">XPoint line end</param>
        /// <param name="B">XPoint line end</param>
        /// <param name="offset">shifting distance</param>
        public static void ShiftLineParallel(ref XPoint A, ref XPoint B, double offset)
        {
            double L = Math.Sqrt(Math.Pow((A.X - B.X), 2) + Math.Pow((A.Y - B.Y), 2));

            A = new XPoint(A.X + offset * (B.Y - A.Y) / L, A.Y + offset * (A.X - B.X) / L);
            B = new XPoint(B.X + offset * (B.Y - A.Y) / L, B.Y + offset * (A.X - B.X) / L);
        }

        /// <summary>
        /// Find point on line defined by points start and end in given distance from start.
        /// </summary>
        /// <param name="start">XPoint</param>
        /// <param name="end">XPoint</param>
        /// <param name="dist">double distance of new point from start</param>
        /// <returns>XPoint</returns>
        public static XPoint FindPointOnLine(XPoint start, XPoint end, double dist)
        {
            double m = (end.Y - start.Y) / (end.X - start.X);
            double X = start.X + dist / Math.Sqrt(1 + Math.Pow(m, 2));
            double Y = m * (X - start.X) + start.Y;
            return new XPoint(X, Y);
        }

        /// <summary>
        /// Find point in center of given abscissa.
        /// </summary>
        /// <param name="a">OrientedAbsciss</param>
        /// <returns>XPoint abscissa center</returns>
        public static XPoint FindAbscissaCenter(OrientedAbscissa a)
        {
            int dist = (int)XPoint.Subtract(a.start, a.end).Length;
            return FindPointOnLine(a.start, a.end, dist / 2);
        }
    }

    interface IPatternDrawing
    {
        void DrawPattern();
    }

    /// <summary>
    /// PatternDrawingError class implements IPatternDrawing interface.
    /// It is used in PatternControl.PdfPattern method, if we want to
    /// initialize pattern to draw but pattern code is not valid.
    /// </summary>
    class PatternDrawingError : IPatternDrawing
    {
        public void DrawPattern()
        {
            throw new Exception("Choosed pattern is not valid!");
        }
    }

    /// <summary>
    /// Pattern class provides basic methods used in inherited pattern-drawing classes.
    /// </summary>
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

        /// <summary>
        /// Create pdf page with XGraphics "canvas" where it will be possible to draw a pattern.
        /// </summary>
        /// <param name="patternHeight">int height of pattern to draw</param>
        /// <param name="patternWidth">int width of pattern to draw</param>
        /// <param name="pdfDoc">PdfDocument</param>
        /// <returns>XGraphics</returns>
        protected static XGraphics CreatePdfGraphics(int patternHeight, int patternWidth, PdfDocument pdfDoc)
        {
            PdfPage page = PdfControl.AddPage(patternHeight, patternWidth, pdfDoc);
            XGraphics g = XGraphics.FromPdfPage(page, XGraphicsUnit.Millimeter);
            PdfControl.DrawScale(ref g);
            return g;
        }

        /// <summary>
        /// Creates new Dictionary<string, XPoint> with same string identifires
        /// and points shifted according distanceDown and distanceRight.
        /// </summary>
        /// <param name="line">Dictionary<string, XPoint></param>
        /// <param name="distanceDown">int, negative if distance to the up</param>
        /// <param name="distanceRight">int, negative if distance to the left</param>
        /// <returns>Dictionary<string, XPoint> shifted points with same string identifires</returns>
        protected static Dictionary<string, XPoint> ShiftedPoints(Dictionary<string, XPoint> line, int distanceDown, int distanceRight)
        {
            Dictionary<string, XPoint> newLine = new Dictionary<string, XPoint>();

            foreach (KeyValuePair<string, XPoint> point in line)
            {
                XPoint p = PGeometry.PointInRelativePosition(point.Value, distanceRight, distanceDown);
                newLine.Add(point.Key, p);
            }

            return newLine;
        }

        /// <summary>
        /// See ShiftedPoints.
        /// </summary>
        protected static Dictionary<string, XPoint> ShiftedPointsRight(Dictionary<string, XPoint> line, int dist)
        {
            return ShiftedPoints(line, 0, dist);
        }

        /// <summary>
        /// See ShiftedPoints.
        /// </summary>
        protected static Dictionary<string, XPoint> ShiftPointsDown(Dictionary<string, XPoint> line, int dist)
        {
            return ShiftedPoints(line, dist, 0);
        }

        /// <summary>
        /// Draw parallel line with given offset to the line defined by points A and B.
        /// </summary>
        /// <param name="A">XPoint</param>
        /// <param name="B">XPoint</param>
        /// <param name="gfx">XGraphics where to draw line</param>
        /// <param name="pen">XPen</param>
        /// <param name="offset">double</param>
        public static void DrawLineParallel(XPoint A, XPoint B, XGraphics gfx, XPen pen, double offset)
        {
            double L = Math.Sqrt(Math.Pow((A.X - B.X), 2) + Math.Pow((A.Y - B.Y), 2));

            gfx.DrawLine(pen, new XPoint(A.X + offset * (B.Y - A.Y) / L, A.Y + offset * (A.X - B.X) / L),
                new XPoint(B.X + offset * (B.Y - A.Y) / L, B.Y + offset * (A.X - B.X) / L));
        }
    }

    /// <summary>
    /// Common ancestor of all bodice pattern drawing classes.
    /// </summary>
    abstract class Bodice : Pattern
    {
        readonly protected PdfDocument pdfDoc;

        // measures needet to draw bodice
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

            // calculate measures for drawing form given body measures
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

        /// <summary>
        /// Creates pattern guiding net Dictionary<string, Dictionary<string, XPoint>>
        /// for bodice pattern. It consist of string named XPoints.
        /// </summary>
        /// <param name="start">starting point on XGraphics canvas</param>
        /// <param name="patternGap">pattern gap between back and front part</param>
        /// <returns>Dictionary<string, Dictionary<string, XPoint>></returns>
        protected Dictionary<string, Dictionary<string, XPoint>> PrepareBodiceNet
            (XPoint start, int lenBack, int lenHips, int lenBackArmpit, int lenFront, int lenAll, int widBack, int widArmhole, int widFront, int patternGap)
        {
            Dictionary<string, Dictionary<string, XPoint>> net = new Dictionary<string, Dictionary<string, XPoint>>();

            Dictionary<string, XPoint> chest = new Dictionary<string, XPoint>();

            int lenTorso = lenBack - lenBackArmpit;
            int lenFrontArmpit = lenFront - lenTorso;

            // calculate position of chest line - chreates back point of chest line
            if (lenFrontArmpit < lenBackArmpit + 30) { chest.Add("back", PGeometry.PointInRelativePositionVertical(start, lenBackArmpit + 30)); }
            else { chest.Add("back", PGeometry.PointInRelativePositionVertical(start, lenFrontArmpit)); }
            
            // add points to chest line
            chest.Add("armholeBack", PGeometry.PointInRelativePositionHorizontal(chest["back"], widBack));
            chest.Add("centerBack", PGeometry.PointInRelativePositionHorizontal(chest["armholeBack"], 2 * widArmhole / 3));
            chest.Add("armholeFront", PGeometry.PointInRelativePositionHorizontal(chest["armholeBack"], widArmhole + patternGap));
            chest.Add("centerFront", PGeometry.PointInRelativePositionHorizontal(chest["armholeFront"], - widArmhole / 3));
            chest.Add("front", PGeometry.PointInRelativePositionHorizontal(chest["armholeFront"], widFront));

            // shift back chest points up - back help line for neck part of pattern
            Dictionary<string, XPoint> neckBack = ShiftPointsDown(
                   chest.Where(s => s.Key == "back" || s.Key == "armholeBack")
                        .ToDictionary(dict => dict.Key, dict => dict.Value),
                   -lenBackArmpit);

            // shift front chest points up - front help line for neck part of pattern
            Dictionary<string, XPoint> neckFront = ShiftPointsDown(
                  chest.Where(s => s.Key == "armholeFront" || s.Key == "front")
                       .ToDictionary(dict => dict.Key, dict => dict.Value),
                  -lenFrontArmpit);

            // union two parts of neck line
            Dictionary<string, XPoint> neck = neckBack.Union(neckFront).ToDictionary(k => k.Key, v => v.Value);

            // shift points on chest line to waist, hips and down hem of pattern
            Dictionary<string, XPoint> waist = ShiftPointsDown(chest, lenTorso);
            Dictionary<string, XPoint> hips = ShiftPointsDown(waist, lenHips);
            Dictionary<string, XPoint> down = ShiftPointsDown(waist, lenAll - lenBack);

            // add all lines to the net dictionary
            net.Add("neck", neck);
            net.Add("chest", chest);
            net.Add("waist", waist);
            net.Add("hips", hips);
            net.Add("down", down);

            return net;
        }

        /// <summary>
        /// Draw bodice pattern guiding net from Dictionary<string, Dictionary<string, XPoint>>.
        /// </summary>
        /// <param name="gfx">XGraphics where to draw</param>
        /// <param name="p">XPen</param>
        /// <param name="net">Dictionary<string, Dictionary<string, XPoint>></param>
        protected static void DrawBodiceNet(XGraphics gfx, XPen p, Dictionary<string, Dictionary<string, XPoint>> net)
        {
            // neck horizontal lines
            gfx.DrawLine(p, net["neck"]["back"], net["neck"]["armholeBack"]);
            gfx.DrawLine(p, net["neck"]["front"], net["neck"]["armholeFront"]);

            // armhole vertical lines
            gfx.DrawLine(p, net["neck"]["armholeBack"], net["chest"]["armholeBack"]);
            gfx.DrawLine(p, net["neck"]["armholeFront"], net["chest"]["armholeFront"]);

            // pattern frame (left_up - left_down - right_down - right_up)
            gfx.DrawLines(p, new XPoint[] { net["neck"]["back"], net["down"]["back"], net["down"]["front"], net["neck"]["front"] });

            // chest, waist and hips horizontal lines
            gfx.DrawLine(p, net["chest"]["back"], net["chest"]["front"]);
            gfx.DrawLine(p, net["waist"]["back"], net["waist"]["front"]);
            gfx.DrawLine(p, net["hips"]["back"], net["hips"]["front"]);

            // patern center vertical lines
            gfx.DrawLine(p, net["chest"]["centerFront"], net["down"]["centerFront"]);
            gfx.DrawLine(p, net["chest"]["centerBack"], net["down"]["centerBack"]);
        }

        /// <summary>
        /// Draw basic shape of back-down-front part of bodice pattern.
        /// </summary>
        /// <param name="gfx">XGraphics</param>
        /// <param name="pen">XPen</param>
        /// <param name="start">left_up point (on neck line)</param>
        /// <param name="backBreak">creates waist tuck, pattern if without tuck if same as backHips</param>
        /// <param name="end">right_up point (on neck line)</param>
        protected void drawContour(XGraphics gfx, XPen pen,
            XPoint start, XPoint backBreak, XPoint backHips, XPoint backDown, XPoint centerDown, XPoint frontDown, XPoint end)
        {
            backBreak = PGeometry.PointInRelativePositionHorizontal(backBreak, backDeflection);

            int d1 = (int)(2 * (centerDown.X - backDown.X) / 3);
            XPoint h1 = PGeometry.PointInRelativePositionHorizontal(backDown, d1);

            int d2 = (int)(2 * (frontDown.X - centerDown.X) / 3);
            XPoint h2 = PGeometry.PointInRelativePositionHorizontal(frontDown, -d2);

            gfx.DrawLines(pen, new XPoint[] { start, backBreak, backHips, backDown, h1, centerDown, h2, frontDown, end});
        }

        /// <summary>
        /// Find "left-up" point in two circles intersection.
        /// </summary>
        /// <param name="A">XPoint center of circle A</param>
        /// <param name="B">XPoint center of circle B</param>
        /// <param name="Ar">double perimeter of circle A</param>
        /// <param name="Br">double perimeter of circle B</param>
        /// <returns>XPoint intersection</returns>
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

        /// <summary>
        /// Drawing one part of waist tuck
        /// </summary>
        /// <param name="center">center XPoint</param>
        /// <param name="wid">maint width of tuck</param>
        /// <param name="widDown">width of down part of tuck</param>
        /// <param name="up">up length of tuck</param>
        /// <param name="down">down length of tuck</param>
        /// <param name="left">bool 1 if it is left part of tuck, 0 right</param>
        protected static void DrawTuckPart(XGraphics gfx, XPen pen, XPoint center, int wid, int widDown, int up, int down, bool left)
        {
            if (left)
            {
                wid = -wid;
                widDown = -widDown;
            }

            gfx.DrawLines(pen, new XPoint[] {
                                   PGeometry.PointInRelativePositionVertical(center, -up),
                                   PGeometry.PointInRelativePositionHorizontal(center, wid),
                                   PGeometry.PointInRelativePosition(center, widDown, down)
                                   });
        }

        /// <summary>
        /// Draw waist tuck on bodice pattern, see DrawTuckPart.
        /// </summary>
        protected static void DrawTuck(XGraphics gfx, XPen pen, XPoint center, int wid, int widDown, int up, int down)
        {
            DrawTuckPart(gfx, pen, center, wid / 2, widDown / 2, up, down, true);
            DrawTuckPart(gfx, pen, center, wid / 2, widDown / 2, up, down, false);
        }

        /// <summary>
        /// Find abscissa of back shoulder.
        /// </summary>
        /// <returns>OrientedAbscissa</returns>
        protected static OrientedAbscissa BackShoulderLine(XPoint neckNetPoint, XPoint shoulderNetPoint, int lenShoulder, int neckHoleWidth, int shoulderSlope)
        {
            int neckHoleHeight = 20;

            // up point of neck hole
            XPoint neckHole = PGeometry.PointInRelativePosition(neckNetPoint, neckHoleWidth, -neckHoleHeight);

            // intersection of shoulder line with back armhole line in guiding net
            XPoint intersection = PGeometry.PointInRelativePositionVertical(shoulderNetPoint, shoulderSlope);

            // remaining length to draw after intersection
            int lenAfterIntersection = (int)(lenShoulder - XPoint.Subtract(neckHole, intersection).Length) + 10;

            int backWidth = (int)(neckNetPoint.X - shoulderNetPoint.X);
            int a = (int)Math.Atan((shoulderSlope + neckHoleHeight) / (backWidth - neckHoleWidth));

            // coordinates of shoulder point
            double shoulderY = intersection.Y + (Math.Sin(a) * lenAfterIntersection);
            double shoulderX = intersection.X + (Math.Cos(a) * lenAfterIntersection);

            XPoint shoulder = new XPoint(shoulderX, shoulderY);

            // back shoulder abscissa
            OrientedAbscissa backShoulder = new OrientedAbscissa(neckHole, shoulder);

            // shift shoulder seam
            PGeometry.ShiftLineParallel(ref backShoulder.start, ref backShoulder.end, 10);

            return backShoulder;
        }

        /// <summary>
        /// Shaping of back part of armhole.
        /// </summary>
        /// <param name="shoulder">XPoint</param>
        /// <param name="backArmhole">XPoint</param>
        /// <returns>OrientedAbscissa</returns>
        protected static OrientedAbscissa FindBackArmholeHelpPoints(XPoint shoulder, XPoint backArmhole)
        {
            double distX = shoulder.X - backArmhole.X < 30 ? 30 : shoulder.X - backArmhole.X;
            double distY = backArmhole.Y - shoulder.Y;
            XPoint a = PGeometry.PointInRelativePosition(shoulder, (int)(-2 * distX / 3), (int)(distY / 2));
            XPoint b = PGeometry.PointInRelativePosition(shoulder, (int)(- distX / 3), (int)(4 * distY / 5));

            return new OrientedAbscissa(a, b);
        }

        /// <summary>
        /// Draw back part of neck hole.
        /// </summary>
        protected static void DrawBackNeckHole(XGraphics gfx, XPen pen, XPoint backShoulder, XPoint upperLeftNet)
        {
            double width = backShoulder.X - upperLeftNet.X;
            double height = upperLeftNet.Y - backShoulder.Y;
            XPoint upperLeft = PGeometry.PointInRelativePosition(backShoulder, (int)(-width * 2), (int)-height);
            XPoint downRight = PGeometry.PointInRelativePositionVertical(backShoulder, (int)height);
            gfx.DrawArc(pen, new XRect(upperLeft, downRight), 0, 90);
        }

        /// <summary>
        /// Draw front part of neck hole.
        /// </summary>
        protected static void DrawFrontNeckHole(XGraphics gfx, XPen pen, XPoint frontShoulder, XPoint upperRightNet, int height)
        {
            double width = upperRightNet.X - frontShoulder.X;
            XPoint upperLeft = PGeometry.PointInRelativePositionVertical(frontShoulder, -height);
            XPoint downRight = PGeometry.PointInRelativePosition(frontShoulder, (int)(2 * width),height);
            gfx.DrawArc(pen, new XRect(upperLeft, downRight), 90, 90);
        }

    }

    /// <summary>
    /// Class to draw loose shirt pattern, implemets IPatternDrawing interface.
    /// </summary>
    class LooseShirt : Bodice, IPatternDrawing
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
            
            int frontNeckUpAddition = widthChest / 10;

            // chestAddition depends on chest circumference
            int chestAddition;
            if (circChest < 820) { chestAddition = 60; frontNeckUpAddition -= 10; }
            else if (circChest < 870) { chestAddition = 65; frontNeckUpAddition -= 5; }
            else if (circChest < 920) { chestAddition = 70; }
            else { chestAddition = 75; }

            // divide addition in all parts of pattern
            widBack = this.widthBack + 3 * chestAddition / 13;
            lenShoulderWithAddition = Math.Min(this.lenShoulder + 3 * chestAddition / 13,160);
            widArmhole = widthChest / 4 - 15 + 6 * chestAddition / 13;
            widFront = widthChest / 2 - 40 + 4 * chestAddition / 13;

            // resulting chest width
            widthChestWithAddition = widBack + widArmhole + widFront;

            lenBackArmpit = height / 10 + widthChest / 10 + 3;
            lenFront = lenBack + frontNeckUpAddition;

            // create graphics
            gfx = CreatePdfGraphics(lenFront + lenHips, widthChestWithAddition, pdfDoc);
        }

        /// <summary>
        /// Returns line of front shoulder.
        /// </summary>
        /// <returns>OrientedAbscissa</returns>
        OrientedAbscissa FrontShoulder(XPoint neckNetPoint, XPoint armholeNetPoint, int neckHoleWidth, int lenShoulder, int lenArmpit)
        {
            XPoint frontUpNeckHole = PGeometry.PointInRelativePositionHorizontal(neckNetPoint, -neckHoleWidth);
            XPoint frontS = FrontShoulderCircleIntersection(armholeNetPoint, frontUpNeckHole, lenArmpit, lenShoulder);

            OrientedAbscissa frontShoulder = new OrientedAbscissa(frontUpNeckHole, frontS);
            PGeometry.ShiftLineParallel(ref frontShoulder.start, ref frontShoulder.end, -10);

            return frontShoulder;
        }

        /// <summary>
        /// Saping of front part of armhole.
        /// </summary>
        OrientedAbscissa findFrontArmohleHelpPoints(XPoint shoulder, XPoint armholeNetPoint)
        {
            double height = armholeNetPoint.Y - shoulder.Y;
            double referenceX;
            int addition;

            if (shoulder.X < armholeNetPoint.X)
            {
                referenceX = armholeNetPoint.X;
                addition = 0;
            }
            else
            {
                referenceX = shoulder.X;
                addition = 10;
            }

            XPoint a = new XPoint(referenceX + addition + 5, shoulder.Y + height / 2);
            XPoint b = new XPoint(referenceX + addition, shoulder.Y + 5 * height / 6);

            return new OrientedAbscissa(a, b);
        }

        /// <summary>
        /// Draw upper part of the shirt pattern.
        /// </summary>
        void DrawShoulderPart(OrientedAbscissa backShoulder, OrientedAbscissa frontShoulder, XPoint neckNetBack, XPoint neckNetFront)

        {
            gfx.DrawLine(pen, backShoulder.start, backShoulder.end);
            gfx.DrawLine(pen, frontShoulder.start, frontShoulder.end);

            DrawBackNeckHole(gfx, pen, backShoulder.start, neckNetBack);
            DrawFrontNeckHole(gfx, pen, frontShoulder.end, neckNetFront, widthNeck + 20);
        }


        /// <summary>
        /// Draw curved armhole part of pattern.
        /// </summary>
        void DrawArmhole(XPoint start, XPoint center, XPoint end, OrientedAbscissa helpBack, OrientedAbscissa helpFront)
        {
            gfx.DrawCurve(pen, new XPoint[] { start,
                                              helpBack.start,
                                              helpBack.end,
                                              center,
                                              helpFront.start,
                                              helpFront.end,
                                              end});
        }


        /// <summary>
        /// Draw the whole pattern, optionaly with guiding net.
        /// </summary>
        public void DrawPattern()
        {
            Dictionary<string, Dictionary<string, XPoint>> net 
                = PrepareBodiceNet(PdfControl.start, lenBack, lenHips, lenBackArmpit, lenFront, lenBack + lenHips, widBack, widArmhole, widFront, 0);
            if (visibleNet) { DrawBodiceNet(gfx, dashedPen, net); }

            // -------------- final shape -----------------------------

            int shoulderSlope = 10;
            int armholeHeight = lenBackArmpit - shoulderSlope;

            OrientedAbscissa backShoulder = BackShoulderLine(net["neck"]["back"], net["neck"]["armholeBack"], lenShoulderWithAddition, widthNeck, shoulderSlope);

            OrientedAbscissa frontShoulder = FrontShoulder(net["neck"]["front"], net["chest"]["armholeFront"], widthNeck + 10, lenShoulderWithAddition, armholeHeight);
            
            OrientedAbscissa backArmholeHelpPoints = FindBackArmholeHelpPoints(backShoulder.end, net["chest"]["armholeBack"]);
            OrientedAbscissa frontArmholeHelpPoints = findFrontArmohleHelpPoints(frontShoulder.start, net["chest"]["armholeFront"]);

            // -------------- draw final shape ------------------------

            DrawShoulderPart(backShoulder, frontShoulder, net["neck"]["back"], net["neck"]["front"]);
            DrawArmhole(backShoulder.end, net["chest"]["centerBack"], frontShoulder.start, backArmholeHelpPoints, frontArmholeHelpPoints);


            XPoint shiftedDownCenter = PGeometry.PointInRelativePositionVertical(net["down"]["centerBack"], -5);

            drawContour(gfx, pen, net["neck"]["back"],
                net["down"]["back"],
                net["down"]["back"],
                net["down"]["back"],
                shiftedDownCenter,
                net["down"]["front"],
                new XPoint(net["neck"]["front"].X, frontShoulder.end.Y + widthNeck + 20));

            DrawTuck(gfx, pen, net["waist"]["centerBack"], 20, 0, lenBack - lenBackArmpit, lenHips - 5);

            // draw back saddle
            DrawLineParallel(net["neck"]["back"], net["neck"]["armholeBack"], gfx, grayPen, -80);
        }
    }

    /// <summary>
    /// Class to draw blouse pattern, implemets IPatternDrawing interface.
    /// </summary>
    class Blouse : Bodice, IPatternDrawing
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

            // chest addition depending on chest circumference
            if (circChest > 1000)
            {
                chestAddition += (circChest - 1000) / 10;
            }

            // divide chest addition in pattern
            widBack = this.widthBack + chestAddition / 7;
            lenShoulderWithAddition = this.lenShoulder + chestAddition / 7;
            widArmhole = widthChest / 4 - 15 + 4 * chestAddition / 7;
            widFront = widthChest / 2 - 40 + 2 * chestAddition / 7;

            // half of distance between nipples
            this.widBreastPoint = widthChest / 5 + 5;

            widthChestWithAddition = widBack + widArmhole + widFront;
            this.patternGap = widthHips - widthChest + (widFront - widthWaist / 2) / 2;

            lenBackArmpit = height / 10 + widthChest / 10 + 3;
            
            gfx = CreatePdfGraphics(lenFront + lenHips, widthChestWithAddition, pdfDoc);
        }

        /// <summary>
        /// Return upper part of front shoulder line.
        /// </summary>
        protected OrientedAbscissa FrontShoulderUp(XPoint neckNetPoint)
        {
            XPoint up = PGeometry.PointInRelativePositionHorizontal(neckNetPoint, -widthNeck);
            XPoint left = PGeometry.PointInRelativePosition(neckNetPoint, -widBreastPoint, 8);

            OrientedAbscissa frontShoulderUp = new OrientedAbscissa(up, left);

            // shifting shoulder seam
            PGeometry.ShiftLineParallel(ref frontShoulderUp.start, ref frontShoulderUp.end, -10);

            return frontShoulderUp;
        }

        /// <summary>
        /// Calculate end point of shoulder line in front patr of pattern.
        /// Rotates currentShoulderPoint in a circle with center breastPoint
        /// while x coordinate of shoulder point is greater than endXCoor
        /// </summary>
        protected static XPoint FindFrontShoulderPoint(XPoint breastPoint, XPoint currentShoulderPoint, int endXCoor)
        {
            // length of breast tuck
            int tuckLength = (int)XPoint.Subtract(currentShoulderPoint, breastPoint).Length;

            // rotation
            while (currentShoulderPoint.X > endXCoor)
            {
                XPoint shoulderShifted = PGeometry.PointInRelativePositionHorizontal(currentShoulderPoint, -1);
                XPoint newShoulder = PGeometry.FindPointOnLine(breastPoint, shoulderShifted, -tuckLength);
                currentShoulderPoint = newShoulder;
            }

            return currentShoulderPoint;
        }

        /// <summary>
        /// Calculating part of shoulder line after breast tuck.
        /// </summary>
        protected OrientedAbscissa FrontShoulder(OrientedAbscissa frontShoulderUp, XPoint armholeNetPoint, XPoint breastPoint)
        {
            // length of breast tuck
            int tuckLength = (int)XPoint.Subtract(frontShoulderUp.end, breastPoint).Length;

            // defining upper point of front armhole
            int endXCoor = (int)armholeNetPoint.X - (widthChest / 10 - 7);

            XPoint startLeftPoint = PGeometry.FindPointOnLine(frontShoulderUp.start, frontShoulderUp.end, -lenShoulder);
            XPoint leftPoint = FindFrontShoulderPoint(breastPoint, startLeftPoint, endXCoor);

            // lenght of line after breast tuck
            int lenShoulderRemaider = lenShoulder - (int)frontShoulderUp.len;

            XPoint upPoint = FrontShoulderCircleIntersection(leftPoint, breastPoint, lenShoulderRemaider, tuckLength);

            OrientedAbscissa frontShoulder = new OrientedAbscissa(leftPoint, upPoint);

            // shifting shoulder seam
            PGeometry.ShiftLineParallel(ref frontShoulder.start, ref frontShoulder.end, -10);

            return frontShoulder;
        }

        /// <summary>
        /// Draw shoulder lines and neck hole.
        /// </summary>
        protected void DrawShoulderPart(XGraphics gfx, XPen pen, OrientedAbscissa back, OrientedAbscissa frontUp, OrientedAbscissa front, XPoint breastPoint, XPoint neckNetBack, XPoint neckNetFront)
        {
            gfx.DrawLines(pen, new XPoint[] { front.start, front.end, breastPoint, frontUp.start, frontUp.end });
            gfx.DrawLine(pen, back.start, back.end);
            DrawBackNeckHole(gfx, pen, back.start, neckNetBack);
            DrawFrontNeckHole(gfx, pen, frontUp.end, neckNetFront, widthNeck + 20);
        }

        /// <summary>
        /// Draw whole curved armhole.
        /// </summary>
        protected void DrawArmhole(XGraphics gfx, XPen pen, XPoint start, XPoint end, XPoint centerBack, XPoint centerFront, OrientedAbscissa backHelp, XPoint frontHelp)
        {
            gfx.DrawCurve(pen, new XPoint[] { start, backHelp.start, backHelp.end, centerBack });
            gfx.DrawCurve(pen, new XPoint[] { centerFront, frontHelp, end });
        }

        /// <summary>
        /// Calculate and draw back saddle.
        /// </summary>
        protected void DrawBackSaddle(XGraphics gfx, XPen pen, OrientedAbscissa shoulder, XPoint neck, int depth)
        {
            XPoint l = PGeometry.PointInRelativePositionVertical(neck, depth);
            XPoint r = new XPoint(shoulder.end.X - 10, l.Y);
            gfx.DrawLine(pen, l, r);

            XPoint up = PGeometry.FindPointOnLine(shoulder.start, shoulder.end, 40);
            XPoint down = new XPoint(up.X, l.Y);
            gfx.DrawLine(pen, up, down);
        }

        /// <summary>
        /// Draw the whole pattern, optionaly with guiding net.
        /// </summary>
        virtual public void DrawPattern()
        {
            Dictionary<string, Dictionary<string, XPoint>> net
                   = PrepareBodiceNet(PdfControl.start, lenBack, lenHips, lenBackArmpit, lenFront, lenBack + lenHips, widBack, widArmhole, widFront, patternGap);

            if (visibleNet) { DrawBodiceNet(gfx, dashedPen, net); }

            // -------------- final shape -----------------------------

            int shoulderSlope = 15;

            OrientedAbscissa backShoulder = BackShoulderLine(net["neck"]["back"], net["neck"]["armholeBack"], lenShoulderWithAddition, widthNeck, shoulderSlope);

            XPoint breastPoint = PGeometry.PointInRelativePosition(net["neck"]["front"], -widBreastPoint, depthBreastPoint);

            OrientedAbscissa frontShoulderUp = FrontShoulderUp(net["neck"]["front"]);
            OrientedAbscissa frontShoulder = FrontShoulder(frontShoulderUp, net["chest"]["armholeFront"], breastPoint);

            OrientedAbscissa backArmholeHelpPoints = FindBackArmholeHelpPoints(backShoulder.end, net["chest"]["armholeBack"]);
            XPoint armholeFrontHelpPoint = new XPoint(net["chest"]["armholeFront"].X + 10, backArmholeHelpPoints.end.Y - 15);

            XPoint downCenter = PGeometry.PointInRelativePositionVertical(PGeometry.FindAbscissaCenter(new OrientedAbscissa(net["down"]["centerBack"], net["down"]["centerFront"])), -7);

            // -------------- draw final shape -----------------------------
            DrawShoulderPart(gfx, pen, backShoulder, frontShoulderUp, frontShoulder, breastPoint, net["neck"]["back"], net["neck"]["front"]);
            DrawArmhole(gfx, pen, backShoulder.end, frontShoulder.start, net["chest"]["centerBack"], net["chest"]["centerFront"], backArmholeHelpPoints, armholeFrontHelpPoint);

            drawContour(gfx, pen,
                net["neck"]["back"],
                net["waist"]["back"],
                net["hips"]["back"],
                net["down"]["back"],
                downCenter,
                net["down"]["front"],
                new XPoint(net["neck"]["front"].X, frontShoulder.end.Y + widthNeck + 20));

            gfx.DrawLine(grayPen, breastPoint, armholeFrontHelpPoint);

            // -------------- draw tucks ----------------------------------

            // count tucks width
            int tuckFrontWidth = widFront - (widthWaist / 2) + 10;
            int allTucksBackWidth = (widthChestWithAddition - tuckFrontWidth - backDeflection) - (widthWaist + 30);
            int tuckBackWidth = allTucksBackWidth / 3;

            // side tuck in pattern gap
            XPoint centerTuckBack = PGeometry.PointInRelativePosition(net["waist"]["centerBack"], -tuckBackWidth / 2, -10);
            XPoint centerTuckFront = PGeometry.PointInRelativePosition(net["waist"]["centerFront"], tuckBackWidth / 2, -10);
            gfx.DrawLines(pen, new XPoint[] { net["chest"]["centerBack"], centerTuckBack, downCenter, centerTuckFront, net["chest"]["centerFront"] });

            // front tuck
            XPoint frontTuck = new XPoint(breastPoint.X, net["waist"]["front"].Y);
            DrawTuck(gfx, pen, frontTuck, tuckFrontWidth, tuckFrontWidth / 2, 140, lenHips);

            // two back tucks
            int lenTorso = lenBack - lenBackArmpit;
            XPoint backTuck1 = PGeometry.PointInRelativePositionVertical(net["waist"]["armholeBack"], -5);
            DrawTuck(gfx, pen, backTuck1, tuckBackWidth, 0, 3 * lenTorso / 4, 3 * lenHips / 4);
            XPoint backTuck2 = PGeometry.PointInRelativePositionHorizontal(net["waist"]["armholeBack"], -widthBack/2);
            DrawTuck(gfx, pen, backTuck2, tuckBackWidth, 0, 2 * lenTorso / 3, 2 * lenHips / 3);

            // back saddle
            DrawBackSaddle(gfx, grayPen, backShoulder, net["neck"]["back"], 80);
        }
    }

    /// <summary>
    /// Class to draw dress pattern, implemets IPatternDrawing interface.
    /// </summary>
    class Dress : Blouse, IPatternDrawing
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

            pdfDoc.Pages.RemoveAt(pdfDoc.Pages.Count-1); // because the blouse contructor creates its own page
            gfx = CreatePdfGraphics(lenPattern, widthChestWithAddition, pdfDoc);
        }

        override public void DrawPattern()
        {
            Dictionary<string, Dictionary<string, XPoint>> net
                   = PrepareBodiceNet(PdfControl.start, lenBack, lenHips, lenBackArmpit, lenFront, lenPattern, widBack, widArmhole, widFront, patternGap);

            if (visibleNet) { DrawBodiceNet(gfx, dashedPen, net); }

            // -------------- final shape -----------------------------

            // simmilar as blouse
            int shoulderSlope = 15;

            OrientedAbscissa backShoulder = BackShoulderLine(net["neck"]["back"], net["neck"]["armholeBack"], lenShoulderWithAddition, widthNeck, shoulderSlope);

            XPoint breastPoint = PGeometry.PointInRelativePosition(net["neck"]["front"], -widBreastPoint, depthBreastPoint);

            OrientedAbscissa frontShoulderUp = FrontShoulderUp(net["neck"]["front"]);
            OrientedAbscissa frontShoulder = FrontShoulder(frontShoulderUp, net["chest"]["armholeFront"], breastPoint);

            OrientedAbscissa backArmholeHelpPoints = FindBackArmholeHelpPoints(backShoulder.end, net["chest"]["armholeBack"]);
            XPoint armholeFrontHelpPoint = new XPoint(net["chest"]["armholeFront"].X, backArmholeHelpPoints.end.Y - 15);

            XPoint downCenter = PGeometry.PointInRelativePositionVertical(PGeometry.FindAbscissaCenter(new OrientedAbscissa(net["down"]["centerBack"], net["down"]["centerFront"])), -7);

            // -------------- draw final shape -----------------------------
            DrawShoulderPart(gfx, pen, backShoulder, frontShoulderUp, frontShoulder, breastPoint, net["neck"]["back"], net["neck"]["front"]);
            DrawArmhole(gfx, pen, backShoulder.end, frontShoulder.start, net["chest"]["centerBack"], net["chest"]["centerFront"], backArmholeHelpPoints, armholeFrontHelpPoint);

            drawContour(gfx, pen,
                net["neck"]["back"],
                net["waist"]["back"],
                net["hips"]["back"],
                net["down"]["back"],
                downCenter,
                net["down"]["front"],
                new XPoint(net["neck"]["front"].X, frontShoulder.end.Y + widthNeck + 20));

            gfx.DrawLine(grayPen, breastPoint, armholeFrontHelpPoint);

            // -------------- draw tucks -----------------------------------

            // claculate tucks width
            int tuckFrontWidth = widFront - (widthWaist / 2) + 10;
            int allTucksBackWidth = (widthChestWithAddition - tuckFrontWidth - backDeflection) - (widthWaist + 30);
            int tuckBackWidth = allTucksBackWidth / 2;
            int hipsAddition = Math.Min(widthHips - widthChest, 10);

            // side tuck in pattern gap
            XPoint centerTuckBack = PGeometry.PointInRelativePosition(net["waist"]["centerBack"], -tuckBackWidth / 2, -10);
            XPoint centerTuckFront = PGeometry.PointInRelativePosition(net["waist"]["centerFront"], tuckBackWidth / 2, -10);
            gfx.DrawLine(pen, net["chest"]["centerBack"], centerTuckBack);
            gfx.DrawLine(pen, net["chest"]["centerFront"], centerTuckFront);

            // and curved line down over hips
            XPoint centerHipsBack = PGeometry.PointInRelativePositionHorizontal(net["hips"]["centerBack"], hipsAddition);
            XPoint centerHipsFront = PGeometry.PointInRelativePositionHorizontal(net["hips"]["centerFront"], -hipsAddition);
            gfx.DrawCurve(pen, new XPoint[] { centerTuckBack, centerHipsBack, downCenter });
            gfx.DrawCurve(pen, new XPoint[] { centerTuckFront, centerHipsFront, downCenter });

            // front tuck
            XPoint frontTuck = new XPoint(breastPoint.X, net["waist"]["front"].Y);
            DrawTuck(gfx, pen, frontTuck, tuckFrontWidth, 0, 140, 3 * lenHips / 4);

            // back tuck
            int lenTorso = lenBack - lenBackArmpit;
            XPoint backTuck = PGeometry.PointInRelativePositionHorizontal(net["waist"]["armholeBack"], -widBack / 3);
            DrawTuck(gfx, pen, backTuck, tuckBackWidth, 0, 3 * lenTorso / 4, 3 * lenHips / 4);

            // back saddle
            DrawBackSaddle(gfx, grayPen, backShoulder, net["neck"]["back"], 80);
        }
    }

    /// <summary>
    /// Common ancestor of all skirt pattern drawing classes.
    /// </summary>
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

        /// <summary>
        /// Creates pattern guiding net Dictionary<string, Dictionary<string, XPoint>>
        /// for skirt pattern. It consist of string named XPoints.
        /// </summary>
        /// <param name="start">starting point on XGraphics canvas</param>
        /// <param name="patternGap">pattern gap between back and front part</param>
        /// <returns>Dictionary<string, Dictionary<string, XPoint>></returns>
        protected Dictionary<string, Dictionary<string, XPoint>> SkirtNet(XPoint start, int lenHips, int lenKnee, int widBack, int widFront, int patternGap)
        {
            Dictionary<string, Dictionary<string, XPoint>> net = new Dictionary<string, Dictionary<string, XPoint>>();

            Dictionary<string, XPoint> back = new Dictionary<string, XPoint>();

            // add back points (waist, hips, down hem)
            back.Add("waist", start);
            back.Add("hips", PGeometry.PointInRelativePositionVertical(start, lenHips));
            back.Add("down", PGeometry.PointInRelativePositionVertical(start, lenKnee));

            // shift back line to center and front
            Dictionary<string, XPoint> centerBack = ShiftedPointsRight(back, widBack);
            Dictionary<string, XPoint> centerFront = ShiftedPointsRight(centerBack, patternGap);
            Dictionary<string, XPoint> front = ShiftedPointsRight(centerFront, widFront);

            net.Add("back", back);
            net.Add("centerFront", centerFront);
            net.Add("centerBack", centerBack);
            net.Add("front", front);

            return net;
        }

        /// <summary>
        /// Draw skirt pattern guiding net.
        /// </summary>
        protected static void DrawSkirtNet(XGraphics gfx, XPen p, Dictionary<string, Dictionary<string, XPoint>> net)
        {
            gfx.DrawLines(p, new XPoint[] { net["back"]["waist"], net["front"]["waist"], net["front"]["down"], net["back"]["down"], net["back"]["waist"] });
            gfx.DrawLine(p, net["back"]["hips"], net["front"]["hips"]);
            gfx.DrawLine(p, net["centerBack"]["waist"], net["centerBack"]["down"]);
            gfx.DrawLine(p, net["centerFront"]["waist"], net["centerFront"]["down"]);
        }

        /// <summary>
        /// Find side tuck up point in pattern center.
        /// </summary>
        /// <param name="up">base up XPoint</param>
        /// <param name="back">true back part, flase front</param>
        /// <returns></returns>
        protected XPoint FindUpPointSideTuck(XPoint up, int upAddition, int tuckWidth, bool back)
        {
            int width = back ? -tuckWidth : tuckWidth;
            XPoint upTuck = PGeometry.PointInRelativePosition(up, width, -upAddition);

            return upTuck;
        }

        /// <summary>
        /// Draw skirt side tuck.
        /// </summary>
        protected void DrawSideTuck(XGraphics gfx, XPen p, XPoint up, XPoint upTuck, XPoint down)
        {
            XPoint beziere = PGeometry.PointInRelativePositionVertical(up, lenHips / 2);

            gfx.DrawBezier(p, upTuck, upTuck, beziere, down);
        }

        /// <summary>
        /// Calculate line defining up border of skirt tuck.
        /// </summary>
        /// <param name="tuckWidth">int width of tuck</param>
        /// <param name="center">center point of tuck</param>
        /// <param name="upLine1">first point defining skirt up hem</param>
        /// <param name="upLine2">second point defining skirt up hem</param>
        /// <returns></returns>
        protected OrientedAbscissa upTuckLine(int tuckWidth, XPoint center, XPoint upLine1, XPoint upLine2)
        {
            XPoint l1;
            XPoint l2;
            int halfTuckWidth = tuckWidth / 2;

            // l1 left
            if (upLine1.X < upLine2.X) { l1 = upLine1; l2 = upLine2; }
            else { l2 = upLine1; l1 = upLine2; }

            int distX = (int)l2.X - (int)l1.X;
            int up1;
            int up2;

            if (l2.Y < l1.Y) // l2 upper
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

            XPoint tuckLine1 = PGeometry.PointInRelativePosition(center, -halfTuckWidth, -up1);
            XPoint tuckLine2 = PGeometry.PointInRelativePosition(center, halfTuckWidth, -up2);

            return new OrientedAbscissa(tuckLine1, tuckLine2);
        }

        /// <summary>
        /// Draw skirt tuck.
        /// </summary>
        protected void DrawTuck(XGraphics gfx, XPen pen, OrientedAbscissa upLine, XPoint downPoint)
        {
            gfx.DrawLines(pen, new XPoint[] { upLine.start, downPoint, upLine.end });
        }

        /// <summary>
        /// Draw front or back tucks with center line.
        /// </summary>
        protected void DrawFrontBackTuckWithCenterLine(XGraphics gfx, XPen penTuck, XPen penLine, OrientedAbscissa upLine, XPoint downPoint, int downY)
        {
            DrawTuck(gfx, penTuck, upLine, downPoint);
            XPoint center = PGeometry.FindAbscissaCenter(upLine);

            gfx.DrawLine(penLine, center, new XPoint(center.X, downY));
        }
    }

    /// <summary>
    /// Class to draw wide skirt pattern, implemets IPatternDrawing interface.
    /// </summary>
    class WideSkirt : Skirt, IPatternDrawing
    {
        readonly XGraphics gfx;
        readonly int hipsAddition = 14;
        readonly int downAddition = 40;
        readonly int widthAll;

        public WideSkirt(bool visibleNet, PdfDocument pdfDoc, int circWaist, int circHips, int lenHips, int lenKnee)
            : base(visibleNet, pdfDoc, circWaist, circHips, lenHips, lenKnee)
        {
            gfx = CreatePdfGraphics(lenKnee, widthHips + downAddition, pdfDoc);
            widthAll = widthHips + hipsAddition;
        }

        public void DrawPattern()
        {
            Dictionary<string, Dictionary<string, XPoint>> net = SkirtNet(PdfControl.start, lenHips, lenKnee, widthAll / 2, widthAll / 2, downAddition);
            if (visibleNet) { DrawSkirtNet(gfx, dashedPen, net); }

            // ------------------- tucks ------------------------

            // calculate tucks width
            int allTucksWidth = widthAll - widthWaist;
            int frontTuckWidth = 3 * allTucksWidth / 14;
            int backTuckWidth = 4 * allTucksWidth / 14;
            int centerTuckWidth = allTucksWidth / 2;

            int frontBackTuckDepth = lenHips - 30;

            // hips tuck
            XPoint centerUpBack = FindUpPointSideTuck(net["centerBack"]["waist"], centerUpAddition, centerTuckWidth / 2, true);
            XPoint centerUpFront = FindUpPointSideTuck(net["centerFront"]["waist"], centerUpAddition, centerTuckWidth / 2, false);

            // front tuck
            XPoint upFrontTuck = PGeometry.PointInRelativePositionHorizontal(net["front"]["waist"], -(widthAll / 6));
            XPoint downFrontTuck = PGeometry.PointInRelativePositionVertical(upFrontTuck, frontBackTuckDepth);

            OrientedAbscissa frontTuckLine = upTuckLine(frontTuckWidth, upFrontTuck, centerUpFront, net["front"]["waist"]);

            // back tuck
            XPoint upBackTuck = PGeometry.PointInRelativePositionHorizontal(net["back"]["waist"], +(widthAll / 6));
            XPoint downBackTuck = PGeometry.PointInRelativePositionVertical(upBackTuck, frontBackTuckDepth);

            OrientedAbscissa backTuckLine = upTuckLine(backTuckWidth, upBackTuck, centerUpBack, net["back"]["waist"]);

            // down edge
            XPoint downCenter = PGeometry.PointInRelativePositionHorizontal(net["back"]["down"], (widthAll + downAddition) / 2);

            // ------------------- draw ------------------------
            gfx.DrawLines(pen, new XPoint[] { centerUpBack, net["back"]["waist"], net["back"]["down"], net["front"]["down"], net["front"]["waist"], centerUpFront });
            gfx.DrawLines(pen, new XPoint[] { net["centerBack"]["hips"], downCenter, net["centerFront"]["hips"] });
            DrawSideTuck(gfx, pen, net["centerFront"]["waist"], centerUpFront, net["centerFront"]["hips"]);
            DrawSideTuck(gfx, pen, net["centerBack"]["waist"], centerUpBack, net["centerBack"]["hips"]);
            DrawFrontBackTuckWithCenterLine(gfx, pen, grayPen, frontTuckLine, downFrontTuck, (int)net["back"]["down"].Y);
            DrawFrontBackTuckWithCenterLine(gfx, pen, grayPen, backTuckLine, downBackTuck, (int)net["back"]["down"].Y);
        }
    }

    /// <summary>
    /// Class to draw straight skirt pattern, implemets IPatternDrawing interface.
    /// </summary>
    class StraightSkirt : Skirt, IPatternDrawing
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
            gfx = CreatePdfGraphics(lenKnee, widthAll, pdfDoc);
        }

        public void DrawPattern()
        {
            Dictionary<string, Dictionary<string, XPoint>> net = SkirtNet(PdfControl.start, lenHips, lenKnee, widthBack, widthFront, 0);
            if (visibleNet) { DrawSkirtNet(gfx, dashedPen, net); }

            // ------------------- tucks ------------------------
            int allTucksWidth = widthAll - widthWaist;

            // calculate tucks width
            int frontTuckWidth = allTucksWidth / 7;
            int backTuck1Width = 3 * allTucksWidth / 14;
            int backTuck2Width = allTucksWidth / 7;

            // side tuck
            int hipsTuckWidth = allTucksWidth / 2;
            int hipsTuckBackWidth = 3 * hipsTuckWidth / 7;
            int hipsTuckFrontWidth = 4 * hipsTuckWidth / 7;

            int frontTuckDepth = 9 * lenHips / 20;
            int backTuc1kDepth = 14 * lenHips / 20;
            int backTuck2Depth = 12 * lenHips / 20;

            // down taper
            int downTuckHalf = 10;

            // hips tuck
            XPoint centerUpBack = FindUpPointSideTuck(net["centerBack"]["waist"], centerUpAddition, hipsTuckBackWidth, true);
            XPoint centerUpFront = FindUpPointSideTuck(net["centerFront"]["waist"], centerUpAddition, hipsTuckFrontWidth, false);
            XPoint downCenterTuck = PGeometry.PointInRelativePositionVertical(net["centerFront"]["hips"], -40);

            // front tuck
            int distFrontFrontTuck = widthWaist / 5 + 60;

            XPoint upFrontTuck = PGeometry.PointInRelativePositionHorizontal(net["front"]["waist"], -distFrontFrontTuck);
            XPoint downFrontTuck = PGeometry.PointInRelativePositionVertical(upFrontTuck, frontTuckDepth);

            OrientedAbscissa frontTuckLine = upTuckLine(frontTuckWidth, upFrontTuck, centerUpFront, net["front"]["waist"]);

            // back tuck 1
            XPoint upBackTuck1 = PGeometry.PointInRelativePositionHorizontal(net["back"]["waist"], widthBack / 3);
            XPoint downBackTuck1 = PGeometry.PointInRelativePositionVertical(upBackTuck1, backTuc1kDepth);

            OrientedAbscissa backTuck1Line = upTuckLine(backTuck1Width, upBackTuck1, centerUpBack, net["back"]["waist"]);

            // back tuck 2
            XPoint upBackTuck2 = PGeometry.PointInRelativePositionHorizontal(net["back"]["waist"], 2 * widthBack / 3);
            XPoint downBackTuck2 = PGeometry.PointInRelativePositionVertical(upBackTuck2, backTuck2Depth);

            OrientedAbscissa backTuck2Line = upTuckLine(backTuck2Width, upBackTuck2, centerUpBack, net["back"]["waist"]);

            // down tuck
            XPoint downTuckBack = PGeometry.PointInRelativePositionHorizontal(net["centerBack"]["down"],- downTuckHalf);
            XPoint downTuckFront = PGeometry.PointInRelativePositionHorizontal(net["centerBack"]["down"], downTuckHalf);

            // ------------------- draw ------------------------
            gfx.DrawLines(pen, new XPoint[] { centerUpBack, net["back"]["waist"], net["back"]["down"], net["front"]["down"], net["front"]["waist"], centerUpFront });
            gfx.DrawLines(pen, new XPoint[] { downTuckBack, net["centerBack"]["hips"], downTuckFront });
            DrawSideTuck(gfx, pen, net["centerFront"]["waist"], centerUpFront, downCenterTuck);
            DrawSideTuck(gfx, pen, net["centerBack"]["waist"], centerUpBack, downCenterTuck);
            gfx.DrawLine(pen, downCenterTuck, net["centerBack"]["hips"]);
            DrawTuck(gfx, pen, frontTuckLine, downFrontTuck);
            DrawTuck(gfx, pen, backTuck1Line, downBackTuck1);
            DrawTuck(gfx, pen, backTuck2Line, downBackTuck2);

        }
    }
}

