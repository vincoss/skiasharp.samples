using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

/// <summary>
/// https://github.com/xamarin/xamarin-forms-samples/tree/master/SkiaSharpForms/Demos/Demos/SkiaSharpFormsDemos
/// </summary>
namespace SkiaSharpSamples.SkiaSharpHelpers
{
    public enum TouchActionType
    {
        Entered,
        Pressed,
        Moved,
        Released,
        Exited,
        Cancelled,
        WheelChanged
    }

     enum TouchManipulationMode
    {
        None,
        PanOnly,
        IsotropicScale,     // includes panning
        AnisotropicScale,   // includes panning
        ScaleRotate,        // implies isotropic scaling
        ScaleDualRotate     // adds one-finger rotation
    }

    enum BitmapStretch
    {
        None,
        Fill,
        Uniform,
        UniformToFill,
        AspectFit = Uniform,
        AspectFill = UniformToFill
    }

    enum BitmapAlignment
    {
        Start,
        Center,
        End
    }

    static class SKBitmapExtensions
    {
        public static SKBitmap LoadBitmapResource(Type type, string resourceID)
        {
            Assembly assembly = type.GetTypeInfo().Assembly;

            using (Stream stream = assembly.GetManifestResourceStream(resourceID))
            {
                return SKBitmap.Decode(stream);
            }
        }

        public static SKRect DrawBitmap(this SKCanvas canvas, SKBitmap bitmap, SKRect dest,
                                      BitmapStretch stretch,
                                      BitmapAlignment horizontal = BitmapAlignment.Center,
                                      BitmapAlignment vertical = BitmapAlignment.Center,
                                      SKPaint paint = null, float percentZoom = 0)
        {
            if(percentZoom < 0 || percentZoom > 1)
            {
                percentZoom = 0;
            }

            if (stretch == BitmapStretch.Fill)
            {
                canvas.DrawBitmap(bitmap, dest, paint);
                return dest;
            }
            else
            {
                var scale = 1F;

                switch (stretch)
                {
                    case BitmapStretch.None:
                        break;

                    case BitmapStretch.Uniform:
                        scale = Math.Min(dest.Width / bitmap.Width, dest.Height / bitmap.Height);
                        break;

                    case BitmapStretch.UniformToFill:
                        scale = Math.Max(dest.Width / bitmap.Width, dest.Height / bitmap.Height);
                        break;
                }

                var valueW = (float)bitmap.Width;
                var valueH = (float)bitmap.Height;

                var width = (scale * bitmap.Width) + valueW.GetPercent(percentZoom);
                var height = (scale * bitmap.Height) + valueH.GetPercent(percentZoom);

                SKRect display = CalculateDisplayRect(dest, width, height, horizontal, vertical);

                canvas.DrawBitmap(bitmap, display, paint);
                return display;
            }
        }

        public static float GetPercent(this float value, float percent)
        {
            if (percent < 0 || percent > 1)
            {
                percent = 0;
            }
            if(percent == 0)
            {
                return 0;
            }
            return (value * percent);
        }

        public static SKRect DrawBitmap(this SKCanvas canvas, SKBitmap bitmap, SKRect source, SKRect dest,
                                      BitmapStretch stretch,
                                      BitmapAlignment horizontal = BitmapAlignment.Center,
                                      BitmapAlignment vertical = BitmapAlignment.Center,
                                      SKPaint paint = null)
        {
            if (stretch == BitmapStretch.Fill)
            {
                canvas.DrawBitmap(bitmap, source, dest, paint);
                return dest;
            }
            else
            {
                float scale = 1;

                switch (stretch)
                {
                    case BitmapStretch.None:
                        break;

                    case BitmapStretch.Uniform:
                        scale = Math.Min(dest.Width / source.Width, dest.Height / source.Height);
                        break;

                    case BitmapStretch.UniformToFill:
                        scale = Math.Max(dest.Width / source.Width, dest.Height / source.Height);
                        break;
                }

                SKRect display = CalculateDisplayRect(dest, scale * source.Width, scale * source.Height,
                                                      horizontal, vertical);

                canvas.DrawBitmap(bitmap, source, display, paint);
                return display;
            }
        }

       public static SKRect CalculateDisplayRect(SKRect dest, float bmpWidth, float bmpHeight, BitmapAlignment horizontal, BitmapAlignment vertical)
        {
            float x = 0;
            float y = 0;

            switch (horizontal)
            {
                case BitmapAlignment.Center:
                    x = (dest.Width - bmpWidth) / 2;
                    break;

                case BitmapAlignment.Start:
                    break;

                case BitmapAlignment.End:
                    x = dest.Width - bmpWidth;
                    break;
            }

            switch (vertical)
            {
                case BitmapAlignment.Center:
                    y = (dest.Height - bmpHeight) / 2;
                    break;

                case BitmapAlignment.Start:
                    break;

                case BitmapAlignment.End:
                    y = dest.Height - bmpHeight;
                    break;
            }

            x += dest.Left;
            y += dest.Top;

            return new SKRect(x, y, x + bmpWidth, y + bmpHeight);
        }
    }
}
