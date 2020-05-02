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
    public partial class DisplayingInPixelDimensionsView : ContentPage
    {
        private SKBitmap _bitmap = null;

        public DisplayingInPixelDimensionsView()
        {
            InitializeComponent();

            LoadImage();
        }

        private void LoadImage()
        {
            string resourceID = "SkiaSharpSamples.resources.banana.jpg";
            Assembly assembly = GetType().GetTypeInfo().Assembly;

            using (Stream stream = assembly.GetManifestResourceStream(resourceID))
            {
                _bitmap = SKBitmap.Decode(stream);
            }
        }

        private void SkiaView_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            float x = (info.Width - _bitmap.Width) / 2;
            float y = (info.Height - _bitmap.Height) / 2;

            canvas.DrawBitmap(_bitmap, x, y);
        }
    }
}