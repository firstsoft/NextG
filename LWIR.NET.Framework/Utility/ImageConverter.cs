using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LWIR.NET.Framework.Utility
{
    /// <summary>
    /// 图像类型转换器
    /// </summary>
    public class ImageConverter
    {
        /// <summary>
        /// Visual转Bitmap
        /// </summary>
        /// <param name="visual">WPF视图</param>
        /// <param name="width">控件宽度</param>
        /// <param name="height">控件高度</param>
        /// <returns>Bitmap图像</returns>
        public static System.Drawing.Bitmap VisualToBitmap(Visual visual)
        {
            /// get bound of the visual
            Rect rect = VisualTreeHelper.GetDescendantBounds(visual);

            /// new a drawing visual and get its context
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                /// generate a visual brush by input, and paint
                VisualBrush vb = new VisualBrush(visual);
                dc.DrawRectangle(vb, null, rect);
            }

            var renderTargetBitmap = new RenderTargetBitmap((int)rect.Width, (int)rect.Height, 96, 96, PixelFormats.Default);
            renderTargetBitmap.Render(dv);

            using (MemoryStream stream = new MemoryStream())
            {
                var encoder = new PngBitmapEncoder();
                var bmpFrame = BitmapFrame.Create(renderTargetBitmap);
                encoder.Frames.Add(bmpFrame);
                encoder.Save(stream);

                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(stream);
                return bitmap;
            }
        }

        /// <summary>
        /// Bitmap图像转BitmapImage
        /// </summary>
        /// <param name="bitmap">Bitmap图像</param>
        /// <returns>BitmapImage图像</returns>
        public static BitmapImage Bitmap2BitmapImage(System.Drawing.Bitmap bitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                bitmap.Save(ms, bitmap.RawFormat);
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }

            return bitmapImage;
        }

        /// <summary>
        /// BitmapImage图像转Bitmap
        /// </summary>
        /// <param name="bitmapImage"></param>
        /// <returns>Bitmap</returns>
        public static System.Drawing.Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            MemoryStream stream = new MemoryStream();
            var encoder = new PngBitmapEncoder();
            var bmpFrame = BitmapFrame.Create(bitmapImage);
            encoder.Frames.Add(bmpFrame);
            encoder.Save(stream);

            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(stream);
            stream.Close();

            return bitmap;
        }
    }
}
