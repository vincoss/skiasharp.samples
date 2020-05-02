using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SkiaSharpSamples.SkiaSharpHelpers;

namespace SkiaSharpSamples.Views.Gestures
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TapGestureSampleView : ContentPage
    {
        private bool _hasScale = false;
        private const float TAP_SCALE = 0.25F;
        SKBitmap _bitmap = SKBitmapExtensions.LoadBitmapResource(typeof(BitmapRectangleSubsetView), "SkiaSharpSamples.resources.pluto-8k.jpg");

        public TapGestureSampleView()
        {
            InitializeComponent();

            LoadGestures();
        }

        private void LoadGestures()
        {
            var tap = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
            tap.Tapped += OnTapped;
            SkiaView.GestureRecognizers.Add(tap);
        }

        private void OnTapped(object sender, EventArgs e)
        {
            if (_hasScale)
            {
                ZoomOut();
                _hasScale = false;
            }
            else
            {
                ZoomIn();
                _hasScale = true;
            }
        }

        private void SkiaView_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            surface.Canvas.Clear(SKColors.Black);

            var scale = _hasScale ? TAP_SCALE : 0;

            var stretch = BitmapStretch.Uniform;
            var horizontal = BitmapAlignment.Center;
            var vertical = BitmapAlignment.Center;

            var dest = new SKRect(0, 0, info.Width, info.Height);

            canvas.DrawBitmap(_bitmap, dest, stretch, horizontal, vertical, null, scale);
        }

        private void ZoomIn()
        {
            SkiaView.InvalidateSurface();
        }

        private void ZoomOut()
        {
            SkiaView.InvalidateSurface();
        }
    }
}