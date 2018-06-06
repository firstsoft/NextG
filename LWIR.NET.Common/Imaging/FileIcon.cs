using LWIR.NET.Common.Enum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LWIR.NET.Common.Imaging
{
    /// <summary>
    /// 获取文件的图标
    /// </summary>
    public class FileIcon
    {
        private static readonly Lazy<ImageSource> LazyFolderIcon = new Lazy<ImageSource>(() => GetIcon(FileTypeEnum.FOLDER));
        private static readonly Lazy<ImageSource> LazyJPGIcon = new Lazy<ImageSource>(() => GetIcon(FileTypeEnum.JPG));
        private static readonly Lazy<ImageSource> LazyTIFIcon = new Lazy<ImageSource>(() => GetIcon(FileTypeEnum.TIF));
        private static readonly Lazy<ImageSource> LazyBMPIcon = new Lazy<ImageSource>(() => GetIcon(FileTypeEnum.BMP));
        private static readonly Lazy<ImageSource> LazyMP4Icon = new Lazy<ImageSource>(() => GetIcon(FileTypeEnum.MP4));
        private static readonly Lazy<ImageSource> LazyPDFIcon = new Lazy<ImageSource>(() => GetIcon(FileTypeEnum.PDF));
        private static readonly Lazy<ImageSource> LazyCSVIcon = new Lazy<ImageSource>(() => GetIcon(FileTypeEnum.CSV));
        private static readonly Lazy<ImageSource> LazyPNGIcon = new Lazy<ImageSource>(() => GetIcon(FileTypeEnum.PNG));
        private static readonly Lazy<ImageSource> LazyGeneralFileIcon = new Lazy<ImageSource>(() => GetIcon(FileTypeEnum.UNKNOWN));

        /// <summary>
        /// 获取文件或文件夹的图标
        /// </summary>
        /// <param name="filePath">文件或文件夹的路径</param>
        /// <returns></returns>
        public static ImageSource GetIcon(string filePath)
        {
            if (Directory.Exists(filePath))
            {
                return LazyFolderIcon.Value;
            }

            if (!File.Exists(filePath))
                return null;

            string ext = Path.GetExtension(filePath).ToUpper();
            string iconPath = string.Empty;
            switch (ext)
            {
                case ".JPG":
                case ".JPEG":
                    return LazyJPGIcon.Value;

                case ".TIF":
                case ".TIFF":
                    return LazyTIFIcon.Value;

                case ".MP4":
                    return LazyMP4Icon.Value;

                case ".PDF":
                    return LazyPDFIcon.Value;

                case ".CSV":
                    return LazyCSVIcon.Value;

                case ".BMP":
                    return LazyBMPIcon.Value;

                case ".PNG":
                    return LazyPNGIcon.Value;

                default:
                    return LazyGeneralFileIcon.Value;
            }
        }

        /// <summary>
        /// 根据文件类型获取文件图标
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static ImageSource GetIcon(FileTypeEnum type)
        {
            string iconPath = string.Empty;

            switch (type)
            {
                case FileTypeEnum.JPG:
                    iconPath = "pack://application:,,,/LWIR.NET.Common;component/Images/fileIcon_jpg.png";
                    break;

                case FileTypeEnum.TIF:
                    iconPath = "pack://application:,,,/LWIR.NET.Common;component/Images/fileIcon_tiff.png";
                    break;

                case FileTypeEnum.MP4:
                    iconPath = "pack://application:,,,/LWIR.NET.Common;component/Images/fileIcon_mp4.png";
                    break;

                case FileTypeEnum.PDF:
                    iconPath = "pack://application:,,,/LWIR.NET.Common;component/Images/fileIcon_pdf.png";
                    break;

                case FileTypeEnum.CSV:
                    iconPath = "pack://application:,,,/LWIR.NET.Common;component/Images/fileIcon_csv.png";
                    break;

                case FileTypeEnum.BMP:
                    iconPath = "pack://application:,,,/LWIR.NET.Common;component/Images/fileIcon_bmp.png";
                    break;

                case FileTypeEnum.PNG:
                    iconPath = "pack://application:,,,/LWIR.NET.Common;component/Images/fileIcon_png.png";
                    break;

                case FileTypeEnum.FOLDER:
                    iconPath = "pack://application:,,,/LWIR.NET.Common;component/Images/folder.png";
                    break;

                default:
                    iconPath = "pack://application:,,,/LWIR.NET.Common;component/Images/fileIcon_general.png";
                    break;
            }

            return CreateIcon(iconPath);
        }

        /// <summary>
        /// 创建图标
        /// </summary>
        /// <param name="iconPath">图标路径</param>
        /// <returns></returns>
        private static ImageSource CreateIcon(string iconPath)
        {
            BitmapImage bmpImage = new BitmapImage();
            bmpImage.BeginInit();
            bmpImage.UriSource = new Uri(iconPath, UriKind.RelativeOrAbsolute);
            bmpImage.EndInit();
            bmpImage.Freeze();

            return bmpImage;
        }
    }
}
