using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;

//#region HSH_SAMPLE_AI
//using ClassLibrary_AI;
//using ClassLibrary_AI.Namespace_ClassMarshal_Roadmap_Roadsign;
//using ClassLibrary_AI.Namespace_ClassUtil_Bitmap;
//#endregion HSH_SAMPLE_AI


namespace IMAnn_Automation_Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string image_uri_source;
        Rectangle boundingBox = new Rectangle();
        Point downPoint;
        Brush defaultBrush = new SolidColorBrush(Colors.Red) { Opacity = 0.3 };
        BitmapSource Bs;
        
        //#region HSH_SAMPLE_AI
        //ClassMarshal_Roadmap_Roadsign mRoadmap_Roadsign = new ClassMarshal_Roadmap_Roadsign();
        //string[] class_names;
        //public int cnt_try = 0;
        //#endregion HSH_SAMPLE_AI

        public double OriginalWidth;
        public double OriginalHeight;
        public double CurrentWidth;
        public double CurrentHeight;
        public double X1, X2, Y1, Y2;

        public MainWindow()
        {
            InitializeComponent();

            //#region HSH_SAMPLE_AI
            //// sample image path
            ////image_path_input.Text = @"C:\Users\hongs\Documents\stryx_roadmap\sol_StryxRoadMap\Bin64\sample_image\pg2_full_50.jpg";
            ////image_path_input.Text = @"C:\Users\hongs\Documents\stryx_roadmap\sol_StryxRoadMap\Bin64\sample_image\test3.jpg";
            //image_path_input.Text = @"C:\Users\hongs\Documents\stryx_roadmap\sol_StryxRoadMap\Bin64\sample_image\test1.jpg";

            //// initialize
            //class_names = System.IO.File.ReadAllLines(@"model_default\kylimnet_class.txt");
            //mRoadmap_Roadsign.init_module_onlyRecognizer();
            //memo_log.Text = "AI_MODULE is activated";
            //#endregion HSH_SAMPLE_AI

            // 이 영상에 표지판 두 개 있어요
            // string image_uri_source_test = @"\\10.0.0.113\Proj_HDmap\HPRM\고속국도1호선(경부선)\SEC01_양재IC~서울TG\SURV02_서울TG~양재IC\TRACK02\Camera01\Track_B-Cam1-320_03.52.30(483).jpg";
        }

        // 버튼 클릭시, load image & metadata(width, height) read
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                image_uri_source = @"" + image_path_input.Text;
                Bs = new BitmapImage(new Uri(image_uri_source));
                sourceImage.Source = Bs;
                OriginalWidth = Bs.PixelWidth;
                OriginalHeight = Bs.PixelHeight;
            }
            catch
            {
                MessageBox.Show("영상 경로 입력하세요");
            }
        }
      
        private void sourceImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 기존 바운딩박스 가리기
            if (boundingBox != null)
                ImageCanvas.Children.Remove(boundingBox);

            downPoint = e.GetPosition(ImageCanvas);
            var boundingBoxRect = new Rectangle()
            {
                Stroke = Brushes.Red,
                Fill = defaultBrush,
                Height = 1,
                Width = 1,
                StrokeThickness = 1,
                Margin = new Thickness(downPoint.X, downPoint.Y, 0, 0)
            };
            boundingBox = boundingBoxRect;
            ImageCanvas.Children.Add(boundingBox);

            // 바운딩박스 시작 좌표 입력(left-up)
            CurrentWidth = sourceImage.Width;
            CurrentHeight = sourceImage.Height;
            X1 = OriginalWidth * downPoint.X / CurrentWidth;
            Y1 = OriginalHeight * downPoint.Y / CurrentHeight;
        }

        private void image_path_input_MouseEnter(object sender, MouseEventArgs e)
        {
            image_path_input.Text = "";
        }

        private void image_path_input_GotFocus(object sender, RoutedEventArgs e)
        {
            image_path_input.Text = "";
        }

        private void sourceImage_MouseMove(object sender, MouseEventArgs e)
        {
            //마우스 움직임 위치에 따라 바운딩박스 크기조절
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                Point movePoint = e.GetPosition(ImageCanvas);
                boundingBox.Height = Math.Abs(downPoint.Y - movePoint.Y);
                boundingBox.Width = Math.Abs(downPoint.X - movePoint.X);
            }
        }

        private void sourceImage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //바운박스 끝 좌표 입력(right-down)
            Point upPoint = e.GetPosition(sourceImage);
            CurrentWidth = sourceImage.Width;
            CurrentHeight = sourceImage.Height;
            X2 = OriginalWidth * upPoint.X / CurrentWidth;
            Y2 = OriginalHeight * upPoint.Y / CurrentHeight;
            // 크롭 이미지 로드
            crop_Image();
        }

        private void crop_Image()
        { 
            int cropWidth = Convert.ToInt32(Math.Abs(X2 - X1));
            int cropHeight = Convert.ToInt32(Math.Abs(Y2 - Y1));
            if (cropWidth <= 0 | cropHeight <= 0)
                return;

            Bitmap bg = new Bitmap(cropWidth, cropHeight);
            Graphics imgGraphics = Graphics.FromImage(bg);

            Bitmap imgBitmap = new Bitmap(image_uri_source);
                     
            Bitmap croppedBitmap1 = imgBitmap.Clone(new System.Drawing.Rectangle(Convert.ToInt32(X1), Convert.ToInt32(Y1), cropWidth, cropHeight)
                , System.Drawing.Imaging.PixelFormat.DontCare);
            imgGraphics.DrawImage(croppedBitmap1, 0, 0);
            // Bitmap 으로 이용할 경우 여기까지
            // return : bg

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
                ms.Close();
            }

            // return sourceImage

            //#region HSH_SAMPLE_AI
            //// check isGray
            //bool isGray = ClassUtil_Bitmap.isGrayScale(croppedBitmap1);

            //// convert BItmap to image_byte_array
            //byte[] image_byte_array = ClassUtil_Bitmap.Bitmap2ByteArray(croppedBitmap1);
            //int image_byte_array_len = image_byte_array.Length;

            //// recognize class with its probability
            //int classNo = 0;
            //double prob = 0;
            //mRoadmap_Roadsign.crop_image_recogize(image_byte_array, image_byte_array_len, 1, ref classNo, ref prob, isGray);

            ////TEST_ImageIn(image_byte_array, image_byte_array_len);  // 이미지 데이터의 포인터, 길이, 이미지 받을 구조체                    
            //memo_log.Text = String.Format("[{0}] Class = {1}, prob = {2} ({3})", ++cnt_try, classNo, prob, class_names[classNo]);
            //#endregion HSH_SAMPLE_AI
            // /////////////////////////////////////////////////////

            // Xaml에 선언한 croppedImage의 source로 BitmapImage 형식이 들어감

            croppedImage.Source = sourceImage;

            // BitmapImage 로 이용할 경우 여기까지
            imgBitmap.Dispose();
            croppedBitmap1.Dispose();
        }
    }
}
