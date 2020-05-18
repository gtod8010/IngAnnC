using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary_AI
{
    // Bitmap 
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    // TEST SAMPLES
    using Namespace_ClassMarshal_Roadmap_Roadsign;
    using Namespace_ClassUtil_Bitmap;

    public class Class_Roadmap
    {
        public static void Main()   // for test
        {
            // initialize
            ClassMarshal_Roadmap_Roadsign mRoadmap_Roadsign = new ClassMarshal_Roadmap_Roadsign();
            mRoadmap_Roadsign.init_module_onlyRecognizer(); // init_ai_module
            
            // load image as Bitmap
            String path_image = @"E:\20200407_sRoadmap\sample_pg2\50_small.jpg";    // test image (50)
            Bitmap mBitmap = (Bitmap)Bitmap.FromFile(path_image);

            // check isGray
            bool isGray = ClassUtil_Bitmap.isGrayScale(mBitmap);

            // convert BItmap to image_byte_array
            byte[] image_byte_array = ClassUtil_Bitmap.Bitmap2ByteArray(mBitmap);
            int image_byte_array_len = image_byte_array.Length;
            
            // recognize class with its probability
            int classNo = 0;
            double prob = 0;
            mRoadmap_Roadsign.crop_image_recogize(image_byte_array, image_byte_array_len, 1, ref classNo, ref prob, isGray);

            //TEST_ImageIn(image_byte_array, image_byte_array_len);  // 이미지 데이터의 포인터, 길이, 이미지 받을 구조체                    
            Console.WriteLine("classNo {0} // prob {1}", classNo, prob);
            Console.WriteLine("COMPLETE; Press Enter");
            Console.ReadKey();
            
            // dispose ClassMarshal_Roadmap_Roadsign
            mRoadmap_Roadsign.Dispose();
            return;
            
        }
    }
}
