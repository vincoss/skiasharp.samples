using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SkiaSharpSamples.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContinuousUseView : ContentPage
    {
        private bool _running = false;
        private SKBitmap _bitmap = null;

        public ContinuousUseView()
        {
            InitializeComponent();
        }


        private void LoadImage(string name)
        {
            if(_bitmap != null)
            {
                _bitmap.Dispose();
                _bitmap = null;
            }

            Assembly assembly = GetType().GetTypeInfo().Assembly;

            using (Stream stream = assembly.GetManifestResourceStream(name))
            {
                _bitmap = SKBitmap.Decode(stream);
                SkiaView.InvalidateSurface();
            }
        }

        private async void btnStart_Clicked(object sender, EventArgs e)
        {
            var files = new[]
            {
                "SkiaSharpSamples.resources.pluto-8k.jpg",
                "SkiaSharpSamples.resources.moon-16k.jpg"
            };

            _running = !_running;

            if (_running)
            {
                while (true)
                {
                    foreach (var name in files)
                    {
                        LoadImage(name);
                        await Task.Delay(1000);
                    }
                }
            }
        }

        private void SkiaView_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();
            surface.Canvas.Clear(SKColors.LemonChiffon);

            if (_bitmap == null)
            {
                return;
            }

            // Scale Image to Fit View And Device Rotation
            int newHeight = _bitmap.Height;
            int newWidth = _bitmap.Width;
            var maxHeight = info.Height;
            var maxWidth = info.Width;

            if (maxHeight > 0 && newHeight > maxHeight)
            {
                double scale = (double)maxHeight / newHeight;
                newHeight = maxHeight;
                newWidth = (int)Math.Floor(newWidth * scale);
            }

            if (maxWidth > 0 && newWidth > maxWidth)
            {
                double scale = (double)maxWidth / newWidth;
                newWidth = maxWidth;
                newHeight = (int)Math.Floor(newHeight * scale);
            }

            SKRect rect = new SKRect(0, 0, newWidth, newHeight);
            rect.Offset(0, 0);

            canvas.DrawBitmap(_bitmap, rect);
        }
    }
}