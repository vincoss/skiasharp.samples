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
using SkiaSharpSamples.SkiaSharpHelpers;

namespace SkiaSharpSamples.Views
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/bitmaps/displaying
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BitmapScalingModeView : ContentPage
    {
        private SKBitmap _bitmap = null;

        public BitmapScalingModeView()
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

        private void OnPickerSelectedIndexChanged(object sender, EventArgs args)
        {
            SkiaView.InvalidateSurface();
        }

        private void SkiaView_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            SKRect dest = new SKRect(0, 0, info.Width, info.Height);

            BitmapStretch stretch = (BitmapStretch)stretchPicker.SelectedItem;
            BitmapAlignment horizontal = (BitmapAlignment)horizontalPicker.SelectedItem;
            BitmapAlignment vertical = (BitmapAlignment)verticalPicker.SelectedItem;

            canvas.DrawBitmap(_bitmap, dest, stretch, horizontal, vertical);
        }     
    }
}