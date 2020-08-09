using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SkiaSharpSamples.SkiaSharpHelpers;

namespace SkiaSharpSamples.Views.Gestures
{
    /// <summary>
    /// NOTE: This sample has an issues on screen resize and orientation change. See the hack.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TouchGestureSampleView : ContentPage
    {
        private float _dragX;
        private float _dragY;
        private SKMatrix _currentMatrix;
        private SKBitmap _bitmap = SKBitmapExtensions.LoadBitmapResource(typeof(BitmapRectangleSubsetView), "SkiaSharpSamples.resources.creek-4k.jpg");
        private SKRect _display;
        private float _MaxTransX = 0F;
        private float _MaxTransY = 0F;
        private float _previousH;

        public TouchGestureSampleView()
        {
            InitializeComponent();

            _currentMatrix = SKMatrix.CreateIdentity();
            _display = new SKRect(0, 0, SkiaView.CanvasSize.Width, SkiaView.CanvasSize.Height);
        }

        private void SkiaView_Touch(object sender, SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            switch (e.ActionType)
            {
                case SkiaSharp.Views.Forms.SKTouchAction.Pressed:
                    {
                        _dragX = e.Location.X;
                        _dragY = e.Location.Y;
                        break;
                    }
                case SkiaSharp.Views.Forms.SKTouchAction.Moved:
                    {
                        if (e.InContact)
                        {
                            Move(e.Location.X, e.Location.Y);
                        }
                        break;
                    }
                default:
                    break;
            }

            e.Handled = true;
            SkiaView.InputTransparent = false;
        }
 
        public void Move(float locationX, float locationY)
        {
            var refresh = false;
            float deltaX = locationX - _dragX;
            float deltaY = locationY - _dragY;

            /*
                * NOTE:
                * deltaX < 0 // right to left
                * deltaX > 0 // left to right
            */

            deltaX = GetTrans(_MaxTransX, _currentMatrix.TransX, deltaX);
            deltaY = GetTrans(_MaxTransY, _currentMatrix.TransY, deltaY);

            if (deltaX != 0 || deltaY != 0)
            {
                _currentMatrix.TransX += deltaX;
                _currentMatrix.TransY += deltaY;
                _dragX += deltaX;
                _dragY += deltaY;
                refresh = true;
            }

            if (refresh)
            {
                SkiaView.InvalidateSurface();
            }
        }

        public static float GetTrans(float max, float actual, float delta, float tolerance = 0)
        {
            max += tolerance;

            if (max >= Math.Abs(actual + delta))
            {
                return delta;
            }
            var result = max - Math.Abs(actual);
            return delta > 0 ? result : result * -1;
        }

        private void SkiaView_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            // Hack: to fix screen resize and rotate - Begin

            if (_previousH <= 0)
            {
                _previousH = SkiaView.CanvasSize.Height;
            }

            var rh = (SkiaView.CanvasSize.Height - _previousH);
            _previousH = SkiaView.CanvasSize.Height;

            if (rh != 0)
            {
                _currentMatrix.TransX = 0;
                _currentMatrix.TransY = 0;
            }

            // Hack: to fix screen resize and rotate - End

            surface.Canvas.SetMatrix(_currentMatrix);
            surface.Canvas.Clear(SKColors.Black);

            using (SKPaint p = new SKPaint())
            {
                p.IsAntialias = true;
                p.IsDither = true;
                p.FilterQuality = SKFilterQuality.High;

                var stretch = BitmapStretch.AspectFill;
                var horizontal = BitmapAlignment.Center;
                var vertical = BitmapAlignment.Center;
               
                var dest = new SKRect(0, 0, info.Width, info.Height);

                _display = canvas.DrawBitmap(_bitmap, dest, stretch, horizontal, vertical, p);

                _MaxTransX = GetMaxTrans(_display.Width, SkiaView.CanvasSize.Width);
                _MaxTransY = GetMaxTrans(_display.Height, SkiaView.CanvasSize.Height);

                e.Surface.Canvas.ResetMatrix();
            }
        }

        public static float GetMaxTrans(float display, float canvas)
        {
            var result = (display / 2) - (canvas / 2);
            return result < 0 ? 0 : result;
        }
    }
}