using SkiaSharp;
using SkiaSharpSamples.SkiaSharpHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace SkiaSharpSamples.Views.Shared
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SkiaSharpZoomContentView : ContentView
    {
        private SKRect _display;

        // Tap
        private bool _hasScale = false;
        private const float TAP_SCALE = 0.25F;
        private long _numberOfTapsReceived;

        // Pinch & Pan
        private TouchManipulationBitmap _bitmapManipulation = new TouchManipulationBitmap(new SKBitmap());
        private List<long> touchIds = new List<long>();
        
        #region Properties

        public static readonly BindableProperty ImageStreamProperty = BindableProperty.Create("ImageStream", typeof(Stream), typeof(SkiaSharpZoomContentView), null, propertyChanged: OnImageStreamChanged);

        public Stream ImageStream
        {
            get { return (Stream)GetValue(ImageStreamProperty); }
            set { SetValue(ImageStreamProperty, value); }
        }

        private static void OnImageStreamChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var newStream = (Stream)newValue;
            var view = (SkiaSharpZoomContentView)bindable;

            using (Stream stream = newStream)
            {
                if (view._bitmapManipulation.Bitmap != null)
                {
                    view._bitmapManipulation.Bitmap.Dispose();
                }
                SKBitmap bitmap = newStream != null ? SKBitmap.Decode(stream) : new SKBitmap();
                if (bitmap == null)
                {
                    bitmap = new SKBitmap(); // Might fail to decode.
                }
                view._bitmapManipulation = new TouchManipulationBitmap(bitmap);
                view._bitmapManipulation.TouchManager.Mode = TouchManipulationMode.IsotropicScale;
                view.SkiaView.InvalidateSurface();
            }
        }

        public static readonly BindableProperty NumberOfTapsRequiredProperty = BindableProperty.Create("NumberOfTapsRequired", typeof(int), typeof(SkiaSharpZoomContentView), 1, validateValue: IsValidNumberOfTapsRequired);

        public int NumberOfTapsRequired
        {
            get { return (int)GetValue(NumberOfTapsRequiredProperty); }
            set { SetValue(NumberOfTapsRequiredProperty, value); }
        }

        static bool IsValidNumberOfTapsRequired(BindableObject view, object value)
        {
            int result;
            bool flag = int.TryParse(value.ToString(), out result);
            return (result >= 1 && result <= Int32.MaxValue);
        } 

        #endregion

        public SkiaSharpZoomContentView()
        {
            InitializeComponent();

            Initialize();
        }

        #region Private methods

        private void Initialize()
        {
            _display = new SKRect(0, 0, SkiaView.CanvasSize.Width, SkiaView.CanvasSize.Height);
        }

        private bool OnTapTimerElapsed()
        {
            try
            {
                if (_numberOfTapsReceived == NumberOfTapsRequired)
                {
                    TapZoom();
                }
            }
            finally
            {
                _numberOfTapsReceived = 0;
            }
            return false;
        }

        private TouchActionType Map(SkiaSharp.Views.Forms.SKTouchAction action)
        {
            switch (action)
            {
                case SkiaSharp.Views.Forms.SKTouchAction.Entered:
                    return TouchActionType.Entered;
                case SkiaSharp.Views.Forms.SKTouchAction.Pressed:
                    return TouchActionType.Pressed;
                case SkiaSharp.Views.Forms.SKTouchAction.Moved:
                    return TouchActionType.Moved;
                case SkiaSharp.Views.Forms.SKTouchAction.Released:
                    return TouchActionType.Released;
                case SkiaSharp.Views.Forms.SKTouchAction.Cancelled:
                    return TouchActionType.Cancelled;
                case SkiaSharp.Views.Forms.SKTouchAction.Exited:
                    return TouchActionType.Exited;
                case SkiaSharp.Views.Forms.SKTouchAction.WheelChanged:
                    return TouchActionType.WheelChanged;
                default:
                    return TouchActionType.Exited;
            }
        }

        private void TapZoom()
        {
            _hasScale = !_hasScale;
            SkiaView.InvalidateSurface();
        }

        /// <summary>
        /// Bitmap display rectangle.
        /// </summary>
        private SKRect GetDisplayRectangle()
        {
            return new SKRect(_display.Left, _display.Top, _display.Right, _display.Bottom);
        }

        // TODO: windows has id from 1, Android has 0, test and fix this
        private void ProcessPressedValueForTapGesture(SkiaSharp.Views.Forms.SKTouchDeviceType type, long id)
        {
            if (type == SkiaSharp.Views.Forms.SKTouchDeviceType.Mouse)
            {
                if (id == 1) _numberOfTapsReceived++;
            }
            else
            {
                if (id == 0) _numberOfTapsReceived++;
            }

            // Used to capture Tap gestures, if the same ID is within timer interval.
            Xamarin.Forms.Device.StartTimer(new TimeSpan(0, 0, 0, 0, 300), OnTapTimerElapsed);
        }

        #endregion

        #region Events

        private void SkiaView_Touch(object sender, SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            SKPoint point = new SKPoint(e.Location.X, e.Location.Y);

            switch (e.ActionType)
            {
                case SkiaSharp.Views.Forms.SKTouchAction.Pressed:
                    {
                        if (_bitmapManipulation.HitTest(point, GetDisplayRectangle()))
                        {
                            ProcessPressedValueForTapGesture(e.DeviceType, e.Id);

                            touchIds.Add(e.Id);
                            _bitmapManipulation.ProcessTouchEvent(e.Id, Map(e.ActionType), point);
                            break;
                        }
                        break;
                    }
                case SkiaSharp.Views.Forms.SKTouchAction.Moved:
                    {
                        // Allow move only if there is initial scale of the bitmap.
                        if (e.InContact && touchIds.Contains(e.Id) && _hasScale)
                        {
                            _bitmapManipulation.ProcessTouchEvent(e.Id, Map(e.ActionType), point);
                            SkiaView.InvalidateSurface();
                        }
                        break;
                    }
                case SkiaSharp.Views.Forms.SKTouchAction.Released:
                case SkiaSharp.Views.Forms.SKTouchAction.Cancelled:
                    if (touchIds.Contains(e.Id))
                    {
                        _bitmapManipulation.ProcessTouchEvent(e.Id, Map(e.ActionType), point);
                        touchIds.Remove(e.Id);
                        SkiaView.InvalidateSurface();
                    }
                    break;
            }

            // NOTE: Handle only touch events if there is zoom already, Othervise pass touch event to higher controls.
            if (_hasScale)
            {
                e.Handled = true;
            }

            SkiaView.InputTransparent = false;
        }

        private void SkiaView_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            surface.Canvas.Clear(SKColors.Black);
            canvas.Save();

            // Might be not loaded yet
            if (_bitmapManipulation == null || _bitmapManipulation.Bitmap == null)
            {
                return;
            }

            // If there is no a scale reset the matrix.
            if (_hasScale == false)
            {
                _bitmapManipulation.Matrix = SKMatrix.CreateIdentity();
            }

            SKMatrix matrix = _bitmapManipulation.Matrix;
            canvas.Concat(ref matrix);

            var scale = _hasScale ? TAP_SCALE : 0;

            var stretch = BitmapStretch.Uniform;
            var horizontal = BitmapAlignment.Center;
            var vertical = BitmapAlignment.Center;

            var dest = new SKRect(0, 0, info.Width, info.Height);

            _display = canvas.DrawBitmap(_bitmapManipulation.Bitmap, dest, stretch, horizontal, vertical, null, scale);
            canvas.Restore();
        } 

        #endregion

    }
}