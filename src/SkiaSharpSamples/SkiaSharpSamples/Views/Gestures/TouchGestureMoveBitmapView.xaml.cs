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
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TouchGestureMoveBitmapView : ContentPage
    {
        private SKRect _display;
        private SKMatrix _currentMatrix;
        private SKBitmap _bitmap = SKBitmapExtensions.LoadBitmapResource(typeof(BitmapRectangleSubsetView), "SkiaSharpSamples.resources.creek-4k.jpg");

        SKPoint pressedLocation;
        SKMatrix pressedMatrix;

        // Limit the move x,y
        private float _MaxTransX = 0F;
        private float _MaxTransY = 0F;

        public TouchGestureMoveBitmapView()
        {
            InitializeComponent();

            _currentMatrix = SKMatrix.CreateIdentity();
            _display = new SKRect(0, 0, SkiaView.CanvasSize.Width, SkiaView.CanvasSize.Height);
        }

        private void SkiaView_Touch(object sender, SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            SKPoint point = new SKPoint(e.Location.X, e.Location.Y);

            switch (e.ActionType)
            {
                case SkiaSharp.Views.Forms.SKTouchAction.Pressed:
                    {
                        if (IsHitTest(point))
                        {
                            pressedLocation = point;
                            pressedMatrix = _currentMatrix;
                        }
                        break;
                    }
                case SkiaSharp.Views.Forms.SKTouchAction.Moved:
                    {
                        if (e.InContact && IsHitTest(point))
                        {
                            SKMatrix matrix = SKMatrix.CreateIdentity();
                            SKPoint delta = point - pressedLocation;
                            matrix = SKMatrix.CreateTranslation(delta.X, delta.Y);

                            // Concatenate the matrices
                            matrix = matrix.PostConcat(pressedMatrix);
                             _currentMatrix = matrix;

                            SkiaView.InvalidateSurface();
                        }
                        break;
                    }
                default:
                    break;
            }

            e.Handled = true;
            SkiaView.InputTransparent = false;
        }

        private void SkiaView_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear(SKColors.GreenYellow);
            canvas.SetMatrix(_currentMatrix);

            var stretch = BitmapStretch.Uniform;
            var horizontal = BitmapAlignment.Center;
            var vertical = BitmapAlignment.Center;

            var dest = new SKRect(0, 0, info.Width, info.Height);
            _display = canvas.DrawBitmap(_bitmap, dest, stretch, horizontal, vertical, null, .2F);

            _MaxTransX = GetMaxTrans(_display.Width, SkiaView.CanvasSize.Width);
            _MaxTransY = GetMaxTrans(_display.Height, SkiaView.CanvasSize.Height);

            Debug.WriteLine($"Info      : W {info.Width}, H: {info.Height}");
            Debug.WriteLine($"Display   : W {(int)_display.Width}, H: {(int)_display.Height}");
            Debug.WriteLine($"Trans     : X {(int)_MaxTransX}, Y: {(int)_MaxTransY}");
            Debug.WriteLine($"MatrixX   : X {(int)_currentMatrix.TransX}, Y: {(int)_currentMatrix.TransY}");
        }

        public bool IsHitTest(SKPoint location)
        {
            var flag = false;
            // Invert the matrix
            SKMatrix inverseMatrix;

            if (_currentMatrix.TryInvert(out inverseMatrix))
            {
                // Transform the point using the inverted matrix
                SKPoint transformedPoint = inverseMatrix.MapPoint(location);

                // Check if it's in the untransformed bitmap rectangle
                SKRect rect = new SKRect(_display.Left, _display.Top, _display.Right, _display.Bottom);
                flag = rect.Contains(transformedPoint);
            }
            return flag;
        }

        public static float GetMaxTrans(float display, float canvas)
        {
            var result = (display / 2) - (canvas / 2);
            return result < 0 ? 0 : result;
        }
    }
}