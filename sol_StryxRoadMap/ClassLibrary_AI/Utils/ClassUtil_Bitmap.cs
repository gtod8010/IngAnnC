using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary_AI.Namespace_ClassUtil_Bitmap
{
    // Marshal
    using System.Runtime.InteropServices;
    // Bitmap 
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    public struct ImgData
    {
        // Indicates that the name field should be marshaled as a C-style 
        // string to the runtime marshaler. 
        public Int32 nDepth;
        public Int32 nWidth;
        public Int32 nHeight;
        public Int32 nChannels;
        public Int32 nWidthStep;
        public IntPtr ImageData;
    }

    public class ClassUtil_Bitmap
    {
        #region Util
        public static bool isGrayScale(Bitmap processedBitmap)
        {
            bool res = true;
            unsafe
            {
                System.Drawing.Imaging.BitmapData bitmapData = processedBitmap.LockBits(new Rectangle(0, 0, processedBitmap.Width, processedBitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, processedBitmap.PixelFormat);
                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(processedBitmap.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* PtrFirstPixel = (byte*)bitmapData.Scan0;
                Parallel.For(0, heightInPixels, y =>
                {
                    byte* currentLine = PtrFirstPixel + (y * bitmapData.Stride);
                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {
                        int b = currentLine[x];
                        int g = currentLine[x + 1];
                        int r = currentLine[x + 2];
                        if (b != g || r != g)
                        {
                            res = false;
                            break;
                        }
                    }
                });
                processedBitmap.UnlockBits(bitmapData);
            }
            return res;
        }

        public static byte[] Bitmap2ByteArray(Bitmap mBitmap)
        {
            ImageFormat fmt = new ImageFormat(mBitmap.RawFormat.Guid);
            var imageCodecInfo = ImageCodecInfo.GetImageEncoders().FirstOrDefault(codec => codec.FormatID == mBitmap.RawFormat.Guid);
            if (imageCodecInfo == null)
            {
                fmt = ImageFormat.Jpeg;
            }
            MemoryStream ms = new MemoryStream();


            mBitmap.Save(ms, fmt);  // 이미지를 스트림형으로 변환
            return ms.ToArray();  // 이미지를 Byte Array 타입으로 변경
        }


        #endregion

        #region PInvokes
        [DllImport("Dll_roadmap_roadsign.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TEST_ImageIn(byte[] img, int data_len);
        #endregion

        #region TEST SAMPLE 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        public void TEST_GetInImage(String path_image)
        {
            Console.WriteLine("SAMPLE]]] C#_Bitmap to C++_IplImage !!");

            Bitmap mBitmap = (Bitmap)Bitmap.FromFile(path_image);

            bool isGray = isGrayScale(mBitmap);
            Console.WriteLine("isGray ?? " + isGray);

            ImageFormat fmt = new ImageFormat(mBitmap.RawFormat.Guid);
            var imageCodecInfo = ImageCodecInfo.GetImageEncoders().FirstOrDefault(codec => codec.FormatID == mBitmap.RawFormat.Guid);
            if (imageCodecInfo == null)
            {
                fmt = ImageFormat.Jpeg;
            }

            using (MemoryStream ms = new MemoryStream())
            {
                mBitmap.Save(ms, fmt);  // 이미지를 스트림형으로 변환
                byte[] image_byte_array = ms.ToArray();  // 이미지를 Byte Array 타입으로 변경
                int nLength = image_byte_array.Length;  // 길이 
                TEST_ImageIn(image_byte_array, nLength);  // 이미지 데이터의 포인터, 길이, 이미지 받을 구조체

                Console.WriteLine("COMPLETE; Press Enter");
                Console.ReadKey();
                return;
            }
        }
        #endregion
    }
}
