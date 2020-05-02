using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SkiaSharpSamples.Views
{
    /// <summary>
    /// https://stackoverflow.com/questions/56819103/skiasharp-how-to-limit-or-lower-drawing-pixel-resolution-for-large-4k-scree
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SKGLViewDrawLinesView : ContentPage
    {
        public SKGLViewDrawLinesView()
        {
            InitializeComponent();
        }

        private void SkiaView_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintGLSurfaceEventArgs e)
        {
            var paintStroke = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Purple,
                StrokeWidth = 1,
                FilterQuality = SKFilterQuality.High
            };

            var rand = new Random();
            var sw = new Stopwatch();

            sw.Start();

            e.Surface.Canvas.Clear();

            var width = (int)SkiaView.CanvasSize.Width;
            var height = (int)SkiaView.CanvasSize.Height;

            for (int i = 0; i < 1000; i++)
            {
                var x1 = rand.Next(width);
                var x2 = rand.Next(width);
                var y1 = rand.Next(height);
                var y2 = rand.Next(height);

                paintStroke.Color = new SKColor((byte)rand.Next(255), (byte)rand.Next(255), (byte)rand.Next(255));

                e.Surface.Canvas.DrawLine(new SKPoint(x1, y1), new SKPoint(x2, y2), paintStroke);
            }

            sw.Stop();

            var message = $"ms: {sw.ElapsedMilliseconds}, w: {width}, h: {height}, pixels: {width * height}";

            Console.WriteLine(message);
        }
    }
}