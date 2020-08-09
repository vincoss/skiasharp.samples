using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkiaSharpSamples.SkiaSharpHelpers
{
    class TouchManipulationBitmap
    {
        public SKBitmap Bitmap { get; private set; }

        private Dictionary<long, TouchManipulationInfo> _touchDictionary = new Dictionary<long, TouchManipulationInfo>();

        public TouchManipulationBitmap(SKBitmap bitmap)
        {
            this.Bitmap = bitmap;
            Matrix = SKMatrix.CreateIdentity();

            TouchManager = new TouchManipulationManager
            {
                Mode = TouchManipulationMode.ScaleRotate
            };
        }

        public TouchManipulationManager TouchManager { set; get; }

        public SKMatrix Matrix { set; get; }

        public void Paint(SKCanvas canvas)
        {
            canvas.Save();
            SKMatrix matrix = Matrix;
            canvas.Concat(ref matrix);
            canvas.DrawBitmap(Bitmap, 0, 0);
            canvas.Restore();
        }

        public bool HitTest(SKPoint location, SKRect? display = null)
        {
            // Invert the matrix
            SKMatrix inverseMatrix;

            if (Matrix.TryInvert(out inverseMatrix))
            {
                // Transform the point using the inverted matrix
                SKPoint transformedPoint = inverseMatrix.MapPoint(location);

                // Check if it's in the untransformed bitmap rectangle
                SKRect rect = new SKRect(0, 0, Bitmap.Width, Bitmap.Height);
                if(display != null)
                {
                    rect = display.Value;
                }
                return rect.Contains(transformedPoint);
            }
            return false;
        }

        public void ProcessTouchEvent(long id, TouchActionType type, SKPoint location)
        {
            switch (type)
            {
                case TouchActionType.Pressed:
                    if (_touchDictionary.ContainsKey(id) == false)
                    {
                        _touchDictionary.Add(id, new TouchManipulationInfo());
                    }
                    _touchDictionary[id].PreviousPoint = location;
                    _touchDictionary[id].NewPoint = location;
                    break;

                case TouchActionType.Moved:
                    if (_touchDictionary.ContainsKey(id))
                    {
                        TouchManipulationInfo info = _touchDictionary[id];
                        info.NewPoint = location;
                        Manipulate();
                        info.PreviousPoint = info.NewPoint;
                    }
                    break;
                case TouchActionType.Released:
                    if (_touchDictionary.ContainsKey(id))
                    {
                        _touchDictionary[id].NewPoint = location;
                        Manipulate();
                        _touchDictionary.Remove(id);
                    }
                    break;

                case TouchActionType.Cancelled:
                    if (_touchDictionary.ContainsKey(id))
                    {
                        _touchDictionary.Remove(id);
                    }
                    break;
            }
        }

        void Manipulate()
        {
            TouchManipulationInfo[] infos = new TouchManipulationInfo[_touchDictionary.Count];
            _touchDictionary.Values.CopyTo(infos, 0);
            SKMatrix touchMatrix = SKMatrix.CreateIdentity();

            if (infos.Length == 1)
            {
                SKPoint prevPoint = infos[0].PreviousPoint;
                SKPoint newPoint = infos[0].NewPoint;
                SKPoint pivotPoint = Matrix.MapPoint(Bitmap.Width / 2, Bitmap.Height / 2);

                touchMatrix = TouchManager.OneFingerManipulate(prevPoint, newPoint, pivotPoint);
            }
            else if (infos.Length >= 2)
            {
                int pivotIndex = infos[0].NewPoint == infos[0].PreviousPoint ? 0 : 1;
                SKPoint pivotPoint = infos[pivotIndex].NewPoint;
                SKPoint newPoint = infos[1 - pivotIndex].NewPoint;
                SKPoint prevPoint = infos[1 - pivotIndex].PreviousPoint;

                touchMatrix = TouchManager.TwoFingerManipulate(prevPoint, newPoint, pivotPoint);
            }

            SKMatrix matrix = Matrix;
            SKMatrix.PostConcat(ref matrix, touchMatrix);
            Matrix = matrix;
        }
    }
}
