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
    public partial class ShowBitmapView : ContentPage
    {
        private SKBitmap _bitmap = null;

        public ShowBitmapView()
        {
            InitializeComponent();

            LoadImage();
        }

        private void LoadImage()
        {
            string resourceID = "SkiaSharpSamples.resources.pluto-8k.jpg";
            Assembly assembly = GetType().GetTypeInfo().Assembly;

            using (Stream stream = assembly.GetManifestResourceStream(resourceID))
            {
                _bitmap = SKBitmap.Decode(stream);
            }
        }

        private void SkiaView_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

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