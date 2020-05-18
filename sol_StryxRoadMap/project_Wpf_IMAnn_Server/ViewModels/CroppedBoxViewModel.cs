using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Wpf_IMAnn_Server.Models;

namespace Wpf_IMAnn_Server.ViewModels
{
    public class CroppedBoxViewModel
    {
        public CroppedBoxViewModel(AnnoShapeModel _annoShapeModel, ImageNodeModel _imageNodeModel)
        {
            AnnoShapeModel = _annoShapeModel;
            ImageNodeModel = _imageNodeModel;
            cropImage();
        }

        private AnnoShapeModel AnnoShapeModel;
        private ImageNodeModel ImageNodeModel;

        private BitmapImage _croppedImage;

        public BitmapImage CroppedImage
        {
            get { return _croppedImage; }
            set { _croppedImage = value; }
        }

        private void cropImage()
        {
            double X1, Y1, X2, Y2;
            int cropWidth, cropHeight;
            switch (AnnoShapeModel.shapetype)
            {
                case Utils.ShapeType.boundingbox:
                    X1 = AnnoShapeModel.col[0];
                    X2 = AnnoShapeModel.col[1];
                    Y1 = AnnoShapeModel.row[0];
                    Y2 = AnnoShapeModel.row[1];

                    cropWidth = Convert.ToInt32(Math.Ceiling(Math.Abs(X2 - X1)));
                    cropHeight = Convert.ToInt32(Math.Ceiling(Math.Abs(Y2 - Y1)));
                    if (cropWidth <= 0 | cropHeight <= 0)
                        return;
                    break;
                
                case Utils.ShapeType.point:
                    X1 = AnnoShapeModel.col[0]-50;
                    Y1 = AnnoShapeModel.row[0]-50;

                    cropWidth = 100;
                    cropHeight = 100;
                    break;

                case Utils.ShapeType.line:
                    X1 = AnnoShapeModel.col.Min();
                    Y1 = AnnoShapeModel.row.Min();

                    cropWidth = (int)(AnnoShapeModel.col.Max() - X1);
                    cropHeight = (int)(AnnoShapeModel.row.Max() - Y1);
                    break;

                case Utils.ShapeType.polygon:
                    X1 = AnnoShapeModel.col.Min();
                    Y1 = AnnoShapeModel.row.Min();

                    cropWidth = (int)(AnnoShapeModel.col.Max() - X1);
                    cropHeight = (int)(AnnoShapeModel.row.Max() - Y1);
                    break;

                default:
                    X1 = Y1 = cropHeight = cropWidth = 0;
                    return;
            }

            Bitmap bg = new Bitmap(cropWidth, cropHeight);
            Graphics imgGraphics = Graphics.FromImage(bg);

            Bitmap imgBitmap = mDPIchanger(ImageNodeModel.imagefilepath);

            Rectangle rect = new Rectangle() { Width = imgBitmap.Width, Height = imgBitmap.Height };
            rect.Intersect(new Rectangle(Convert.ToInt32(X1), Convert.ToInt32(Y1), cropWidth, cropHeight));
            Bitmap croppedBitmap1 = imgBitmap.Clone(rect, PixelFormat.DontCare); 
            imgGraphics.DrawImage(croppedBitmap1, 0, 0);

            BitmapImage sourceImage;
            using (var ms = new MemoryStream())
            {
                bg.Save(ms, ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);

                sourceImage = new BitmapImage();
                sourceImage.BeginInit();
                sourceImage.CacheOption = BitmapCacheOption.OnLoad;
                sourceImage.StreamSource = ms;
                sourceImage.EndInit();
                sourceImage.Freeze();
                ms.Dispose();
                ms.Close();
            }

            imgBitmap.Dispose();
            croppedBitmap1.Dispose();
            imgGraphics.Dispose();
            bg.Dispose();
            _croppedImage = sourceImage;
            _croppedImage.StreamSource.Dispose();
            CroppedImage.StreamSource.Dispose();
            sourceImage.StreamSource.Dispose();
            GC.Collect();
        }
        private Bitmap mDPIchanger(string filename)
        {
            //BitmapImage source = new BitmapImage();
            System.Net.WebRequest request = System.Net.WebRequest.Create(filename);
            System.Net.WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();

            Bitmap bitmap = new Bitmap(responseStream);
            bitmap.SetResolution(96, 96);
            //MemoryStream memstream = new MemoryStream();
            //bitmap.Save(memstream, ImageFormat.Bmp);
            //memstream.Seek(0, SeekOrigin.Begin);

            //source.BeginInit();
            //source.StreamSource = memstream;
            //source.CacheOption = BitmapCacheOption.OnLoad;
            //source.EndInit();
            //return source;
            return bitmap;
        }
    }
}
