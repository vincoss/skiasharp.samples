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
    public partial class BitmapScaleFitScreenView : ContentPage
    {
        private SKBitmap _bitmap = null;

        public BitmapScaleFitScreenView()
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

            var resizeFactor = 4F;

            // Draw a bitmap rescaled
            canvas.SetMatrix(SKMatrix.MakeScale(resizeFactor, resizeFactor));
            canvas.DrawBitmap(_bitmap, 0, 0);
            canvas.ResetMatrix();
        }
    }
}