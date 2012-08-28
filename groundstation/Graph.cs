using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace groundstation
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:groundstation"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:groundstation;assembly=groundstation"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:Graph/>
    ///
    /// </summary>

    public class Graph : Control
    {
        public class Point
        {
            public double x;
            public List<double> y = new List<double>();
        }

        //public List<Point> points = new List<Point>();
        public List<Point> points = new List<Point>();
        public bool zero = false;
        public bool custom = false;
        public double from = 0;
        public double to = 1;

        private WriteableBitmap bitmap = null;
        private uint[] clearLine;

        static Graph()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Graph), new FrameworkPropertyMetadata(typeof(Graph)));
        }
        
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            Rect bounds = new Rect(0, 0, base.ActualWidth + 1, base.ActualHeight + 1);
            dc.DrawRectangle(new SolidColorBrush(Colors.LightGray), null, bounds);

            if (points.Count() == 0)
                return;

            double min, max;
            if (custom)
            {
                min = from;
                max = to;
            }
            else
            {
                if (zero)
                {
                    min = 0;
                    max = 0;
                }
                else
                {
                    min = 1e300;
                    max = -1e300;
                }

                for (int visX = Math.Max(0, (points.Count() - (int)base.ActualWidth)); visX < points.Count(); visX++)
                {
                    var point = points[visX];
                    foreach (double y in point.y)
                    {
                        if (y < min)
                            min = y;

                        if (y > max)
                            max = y;
                    }
                }

                if (max == min)
                    max = min + 1;
            }


            dc.DrawText(
                new FormattedText(min.ToString(), System.Globalization.CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 8, System.Windows.Media.Brushes.Black),
                new System.Windows.Point(0, base.ActualHeight - 10));

            dc.DrawText(
                new FormattedText(max.ToString(), System.Globalization.CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 8, System.Windows.Media.Brushes.Black),
                new System.Windows.Point(0, 0));

            if (bitmap == null)
                initBitmap();

            var lineRect = new Int32Rect(0, 0, bitmap.PixelWidth, 1);
            for (int line = 0; line < bitmap.PixelHeight; line++)
                bitmap.WritePixels(
                    lineRect,
                    clearLine,
                    clearLine.Length * 4,
                    0,
                    line);

            uint[][] colors = new uint[][]
            {
                new uint[] { 0xFFFF0000 },
                new uint[] { 0xFF00FF00 },
                new uint[] { 0xFF0000FF },
                new uint[] { 0xFF800000 },
                new uint[] { 0xFF008000 },
                new uint[] { 0xFF000080 },
                //new uint[] { 0xFF00FFFF },
                //new uint[] { 0xFFFF00FF },
                //new uint[] { 0xFFFFFF00 }
            };

            var pixelRect = new Int32Rect(0, 0, 1, 1);
            for (int pixelX = Math.Max(0, (points.Count() - bitmap.PixelWidth)); pixelX < points.Count(); pixelX++)
            {
                var point = points[pixelX];
                for (int idxY = 0; idxY < point.y.Count(); idxY++)
                {
                    double y = point.y[idxY];
                    int pixelY = (int)((max - y) * (bitmap.PixelHeight - 1) / (max - min));
                    if (pixelY < 0)
                        pixelY = 0;
                    else if (pixelY >= bitmap.PixelHeight)
                        pixelY = bitmap.PixelHeight - 1;

                    bitmap.WritePixels(
                        pixelRect,
                        colors[idxY % colors.Length],
                        4,
                        pixelX % bitmap.PixelWidth,
                        pixelY);
                }
            }

            dc.DrawImage(bitmap, bounds);

            //var lastPoint = points[points.Count() - 1];
            //for (int idxY = 0; idxY < lastPoint.y.Count(); idxY++)
            //{
            //    double y = lastPoint.y[idxY];
            //    int pixelY = (int)((max - y) * (bitmap.PixelHeight - 1) / (max - min));

            //    dc.DrawText(
            //        new FormattedText(lastPoint.y[idxY].ToString(), System.Globalization.CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 8, System.Windows.Media.Brushes.Black),
            //        new System.Windows.Point((points.Count() - 1) % bitmap.PixelWidth, pixelY));
            //}
        }


        protected void initBitmap()
        {
            bitmap = new WriteableBitmap((int)base.ActualWidth, (int)base.ActualHeight, 96, 96, PixelFormats.Bgra32, null);
            clearLine = new uint[bitmap.PixelWidth];
            for (int x = 0; x < clearLine.Length; x++)
                clearLine[x] = 0x00000000;
        }

    }
}


