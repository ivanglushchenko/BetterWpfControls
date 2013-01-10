using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BetterWpfControls.Components
{
    public static class Extensions
    {
        #region Methods

        public static T GetParent<T>(this DependencyObject obj)
            where T : DependencyObject
        {
            var t = obj;
            while (!(t is T) && (t = VisualTreeHelper.GetParent(t)) != null) ;
            return t as T;
        }

        public static void DoWhenLoaded(this FrameworkElement fe, Action action)
        {
            if (fe.IsLoaded)
            {
                action();
            }
            else
            {
                RoutedEventHandler handler = null;
                handler = (sender, args) =>
                {
                    fe.Loaded -= handler;
                    action();
                };
                fe.Loaded += handler;
            }
        }

        public static void TraverseVisualTree(this DependencyObject obj, Action<DependencyObject> action)
        {
            TraverseVisualTree(obj, (o) => { action(o); return TraverseResult.Continue; });
        }

        public static TraverseResult TraverseVisualTree(this DependencyObject obj, Func<DependencyObject, TraverseResult> action)
        {
            if (obj == null)
            {
                return TraverseResult.Break;
            }
            if (action(obj) == TraverseResult.Break)
            {
                return TraverseResult.Break;
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                if (TraverseVisualTree(VisualTreeHelper.GetChild(obj, i), action) == TraverseResult.Break)
                {
                    return TraverseResult.Break;
                }
            }
            return TraverseResult.Continue;
        }

        public static byte[] GetScreenshot(this UIElement source, double scale, int quality)
        {
            var actualHeight = source.RenderSize.Height;
            var actualWidth = source.RenderSize.Width;
            var renderHeight = actualHeight * scale;
            var renderWidth = actualWidth * scale;

            var renderTarget = new RenderTargetBitmap((int)renderWidth, (int)renderHeight, 96, 96, PixelFormats.Pbgra32);
            var sourceBrush = new VisualBrush(source);

            var drawingVisual = new DrawingVisual();
            var drawingContext = drawingVisual.RenderOpen();

            using (drawingContext)
            {
                drawingContext.PushTransform(new ScaleTransform(scale, scale));
                drawingContext.DrawRectangle(sourceBrush, null, new Rect(new Point(0, 0), new Point(actualWidth, actualHeight)));
            }
            renderTarget.Render(drawingVisual);

            var jpgEncoder = new JpegBitmapEncoder();
            jpgEncoder.QualityLevel = quality;
            jpgEncoder.Frames.Add(BitmapFrame.Create(renderTarget));

            Byte[] _imageArray;

            using (var outputStream = new MemoryStream())
            {
                jpgEncoder.Save(outputStream);
                _imageArray = outputStream.ToArray();
            }

            return _imageArray;
        }

        #endregion Methods

        #region Internal Classes

        public enum TraverseResult
        {
            Continue,
            Break
        }

        #endregion Internal Classes
    }
}