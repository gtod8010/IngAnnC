using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary_AI.Namespace_ClassMarshal_Roadmap_Roadsign
{
    #region Namespaces
    // Marshal
    using System.Runtime.InteropServices;
    // Bitmap 
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    // Util
    using ClassLibrary_AI.Namespace_ClassUtil_Bitmap;
    #endregion

    [StructLayout(LayoutKind.Sequential)]
    public struct RoiBox
    {
        // Indicates that the name field should be marshaled as a C-style 
        // string to the runtime marshaler. 
        public int x;              /// x 좌표
        public int y;              /// y 좌표
        public int width;          /// 가로너비
        public int height;         /// 세로길이
        public double score;           /// MCT Score
        public int classType;      /// 검출기 타입 (원형 등)
        public double prob;
    }

    [StructLayout(LayoutKind.Sequential)]
    unsafe public struct RoiBox_list
    {
        public int size;
        public RoiBox* data;
    };

    unsafe public class ClassMarshal_Roadmap_Roadsign : IDisposable
    {
        #region PInvokes
        [DllImport("Dll_roadmap_roadsign.dll", CallingConvention = CallingConvention.Cdecl)]
        static private extern IntPtr cpp_CreateClass_Stryx_RoadMap_RoadSign(); // cpp_CreateClass_Stryx_RoadMap_RoadSign
        [DllImport("Dll_roadmap_roadsign.dll", CallingConvention = CallingConvention.Cdecl)]
        static private extern void cpp_DisposeClass_Stryx_RoadMap_RoadSign(IntPtr pClassObject); // cpp_DisposeClass_Stryx_RoadMap_RoadSign

        [DllImport("Dll_roadmap_roadsign.dll", CallingConvention = CallingConvention.Cdecl)]
        static private extern void cpp_Test_sample(IntPtr pClassObject);
        public void Test_sample()
        {
            Console.WriteLine("TEST C#");
            cpp_Test_sample(this.m_pNativeObject);
        }

        [DllImport("Dll_roadmap_roadsign.dll", CallingConvention = CallingConvention.Cdecl)]
        static private extern void cpp_init_module(IntPtr pClassObject, int mode, float crop_vertical_bottom,
            float crop_vertical_top, float mctThreshold, bool roiVerification);

        [DllImport("Dll_roadmap_roadsign.dll", CallingConvention = CallingConvention.Cdecl)]
        static private extern void cpp_init_module_onlyRecognizer(IntPtr pClassObject);

        [DllImport("Dll_roadmap_roadsign.dll", CallingConvention = CallingConvention.Cdecl)]
        static private extern int cpp_full_iamge_detect_recognize(IntPtr pClassObject,
            byte[] srcImage_ptr, int srcImage_ptr_len, ref RoiBox_list detectedObjs, int typeIdx);

        [DllImport("Dll_roadmap_roadsign.dll", CallingConvention = CallingConvention.Cdecl)]
        static private extern void cpp_RoiBox_list_release(ref RoiBox_list detectedObjs);

        [DllImport("Dll_roadmap_roadsign.dll", CallingConvention = CallingConvention.Cdecl)]
        static private extern int cpp_crop_image_recogize(IntPtr pClassObject,
        byte[] srcImage_ptr, int srcImage_ptr_len, int typeIdx, ref int classNo, ref double prob, bool isGray);
        #endregion PInvokes

        #region Members
        private IntPtr m_pNativeObject;     // Variable to hold the C++ class's this pointer
        #endregion Members

        public ClassMarshal_Roadmap_Roadsign()
        {
            // We have to Create an instance of this class through an exported function
            this.m_pNativeObject = cpp_CreateClass_Stryx_RoadMap_RoadSign();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool bDisposing)
        {
            if (this.m_pNativeObject != IntPtr.Zero)
            {
                // Call the DLL Export to dispose this class
                cpp_DisposeClass_Stryx_RoadMap_RoadSign(this.m_pNativeObject);
                this.m_pNativeObject = IntPtr.Zero;
            }

            if (bDisposing)
            {
                // No need to call the finalizer since we've now cleaned
                // up the unmanaged memory
                GC.SuppressFinalize(this);
            }
        }

        // This finalizer is called when Garbage collection occurs, but only if
        // the IDisposable.Dispose method wasn't already called.
        ~ClassMarshal_Roadmap_Roadsign()
        {
            Dispose(false);
        }

        #region Wrapper methods
        //    [DllImport("Dll_roadmap_roadsign.dll")]
        //    static private extern void cpp_crop_image_recogize(IntPtr pClassObject,
        //    IntPtr srcImage_ptr, int srcImage_ptr_len, int typeIdx, ref int classNo, ref double prob, bool isGray);
        public void init_module(int mode, float crop_vertical_bottom,
            float crop_vertical_top, float mctThreshold, bool roiVerification)
        {
            cpp_init_module(this.m_pNativeObject, mode, crop_vertical_bottom, crop_vertical_top, mctThreshold, roiVerification);
        }

        public void init_module_onlyRecognizer()
        {
            cpp_init_module_onlyRecognizer(this.m_pNativeObject);
        }

        public void full_iamge_detect_recognize(byte[] srcImage_ptr, int srcImage_ptr_len, ref RoiBox_list detectedObjs, int typeIdx)
        {
            cpp_full_iamge_detect_recognize(this.m_pNativeObject,
                srcImage_ptr, srcImage_ptr_len, ref detectedObjs, typeIdx);
        }

        public void RoiBox_list_release(ref RoiBox_list detectedObjs)
        {
            cpp_RoiBox_list_release(ref detectedObjs);
        }

        public void crop_image_recogize(byte[] srcImage_ptr, int srcImage_ptr_len, int typeIdx, ref int classNo, ref double prob, bool isGray)
        {
            cpp_crop_image_recogize(this.m_pNativeObject,
                srcImage_ptr, srcImage_ptr_len, typeIdx, ref classNo, ref prob, isGray);
        }
        #endregion Wrapper methods

        #region Sample
        public void Testsample1()   // for test
        {
            unsafe
            {
                ClassMarshal_Roadmap_Roadsign testClass = new ClassMarshal_Roadmap_Roadsign();

                testClass.Test_sample();
                testClass.init_module_onlyRecognizer();

                String path_image = @"E:\20200407_sRoadmap\sample_pg2\50_small.jpg";
                Bitmap mBitmap = (Bitmap)Bitmap.FromFile(path_image);

                bool isGray = ClassUtil_Bitmap.isGrayScale(mBitmap);
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
                    int image_byte_array_len = image_byte_array.Length;  // 길이 

                    int classNo = 0;
                    double prob = 0;
                    testClass.crop_image_recogize(image_byte_array, image_byte_array_len, 1, ref classNo, ref prob, isGray);

                    //TEST_ImageIn(image_byte_array, image_byte_array_len);  // 이미지 데이터의 포인터, 길이, 이미지 받을 구조체

                    Console.WriteLine("classNo {0} // prob {1}", classNo, prob);
                    Console.WriteLine("COMPLETE; Press Enter");
                    Console.ReadKey();
                }

                testClass.Dispose();
                return;
            }
        }


        #endregion Sample
    }
}
