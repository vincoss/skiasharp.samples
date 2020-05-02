using SkiaSharp;
using SkiaSharp.Views.Forms;
using SkiaSharpSamples.SkiaSharpHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SkiaSharpSamples.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BitmapRectangleSubsetView : ContentPage
    {
        SKBitmap _bitmap = SKBitmapExtensions.LoadBitmapResource(typeof(BitmapRectangleSubsetView),
                                               "SkiaSharpSamples.resources.moon-16k.jpg");

        static readonly SKRect SOURCE = new SKRect(7900, 3000, 8250, 3250);

        public BitmapRectangleSubsetView()
        {
            InitializeComponent();
        }

        private void SkiaView_PaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            SKRect dest = new SKRect(0, 0, info.Width, info.Height);

            BitmapStretch stretch = BitmapStretch.None;
            BitmapAlignment horizontal = BitmapAlignment.Center;
            BitmapAlignment vertical = BitmapAlignment.Center;

            canvas.DrawBitmap(_bitmap, SOURCE, dest, stretch, horizontal, vertical);
        }
    }
}