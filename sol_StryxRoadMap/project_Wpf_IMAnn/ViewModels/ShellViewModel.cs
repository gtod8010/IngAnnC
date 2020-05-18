using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.IO;
using Caliburn.Micro;
using Wpf_IMAnn.Models;
using Wpf_IMAnn.Utils;
using DotSpatial.Projections;
using DotSpatial.Data;
using System.Windows.Controls;
using System.Windows.Input;
using ShapeType = Wpf_IMAnn.Utils.ShapeType;
using Wpf_IMAnn.Views;
using System.Windows.Media.Imaging;
using System.Drawing;
using Rectangle = System.Windows.Shapes.Rectangle;
using Brush = System.Windows.Media.Brush;
using Point = System.Windows.Point;
using Image = System.Windows.Controls.Image;
using System.Drawing.Imaging;
using Brushes = System.Windows.Media.Brushes;

namespace Wpf_IMAnn.ViewModels
{
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive, IHandle<SectionModel>, IHandle<ShapeType>
    {
        public ShellViewModel()
        {           
            AppDomain appDomain = AppDomain.CurrentDomain;
            appDomain.UnhandledException += new UnhandledExceptionEventHandler(ShutDown);
            EventAggregationProvider.EventAggregator.Subscribe(this);
            ActivateItem(new FileTabViewModel());
            CamWindowViewModel = new CamWindowViewModel();
        }

        public void ShutDown(object sender, UnhandledExceptionEventArgs args)
        {
            if(selectedImageNodeModel != null)
                MessageBox.Show($"프로그램이 영상 번호 [{selectedImageNodeModel.imageid}]에서 종료됩니다.","종료");
        }

        public ShellView GetShellView()
        {
            object shellView = this.GetView();
            return (ShellView)shellView;
        }

        #region Variables // 변수 선언
        public string BottomBarText;
        public ShapeType selectedShapeType { get; set; }
        public CamWindowView CamWindow;
        public double OriginalWidth, OriginalHeight;
        public double CurrentHeight, CurrentWidth;

        Brush defaultStroke = Brushes.Red;
        Brush selectedStroke = Brushes.Blue;
        Brush defaultBrush = new SolidColorBrush(Colors.Red) { Opacity = 0 };
        Brush selectedBrush = new SolidColorBrush(Colors.Blue) { Opacity = 0.3 };

        public string SignImage { get; set; }
        #endregion 

        #region Convention methods // Caliburn.micro의 Convention에 의한 메소드
        public void FileContent()
        {
            ActivateItem(new FileTabViewModel());
        }
        public void WindowContent()
        {
            ActivateItem(new WindowTabViewModel());
        }
        public void ToolContent()
        {
            ActivateItem(new ToolTabViewModel());
        }
        public void MoveButton()
        {
            try
            {
                (GetView() as ShellView).NodeDataGrid.SelectedIndex = Convert.ToInt32((GetView() as ShellView).MoveIndex.Text) - 1;
            }
            catch
            {
                MessageBox.Show("다시 입력하세요.", "입력오류");
            }
        }
             
        public void ImageNodeChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedImageNodeModel = (ImageNodeModel)sender;
            EventAggregationProvider.EventAggregator.PublishOnUIThread(this);
            EventAggregationProvider.EventAggregator.PublishOnUIThread(SelectedImageNodeModel);
            AnnoShapeModelCollection = CamWindowViewModel.AnnoShapeModelCollection;

            DataGrid Dg = e.OriginalSource as DataGrid;
            Dg.ScrollIntoView(sender);
            CamWindow = _camWindowViewModel.GetCamWindowView();
            SelectedImageNodeModel.N_anno = AnnoShapeModelCollection.Count;
            NotifyOfPropertyChange(() => ImageNodeModelCollection);

            BitmapSource img = new BitmapImage(new Uri(selectedImageNodeModel.imagefilepath));
            OriginalWidth = img.PixelWidth;
            OriginalHeight = img.PixelHeight;
        }

        public void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.OriginalSource.GetType() != typeof(TextBox))
                switch (e.Key)
                {
                    case Key.A:
                        try
                        {
                            mSaveCsv();
                        }
                        catch
                        {
                            MessageBox.Show("SignField 선택을 완료하세요.");
                            return;
                        }
                        mIndexSubtract(1);
                        break;
                    case Key.D:
                        try
                        {
                            mSaveCsv();
                        }
                        catch
                        {
                            MessageBox.Show("SignField 선택을 완료하세요.");
                            return;
                        }
                        mIndexAdd(1);
                        break;
                    case Key.S:
                        if (selectedShapeType == ShapeType.boundingbox)
                            EventAggregationProvider.EventAggregator.PublishOnUIThread(ShapeType.polygon);
                        else if (selectedShapeType == ShapeType.polygon)
                            EventAggregationProvider.EventAggregator.PublishOnUIThread(ShapeType.boundingbox);
                        break;
                }
        }

        public void MemoNumClicked(Button sender)
        {
            GetShellView().memoText.Text = sender.Content.ToString();
            GetShellView().memoText.Focus();
        }
        public void MoveBack()
        {
            try
            {
                mSaveCsv();
            }
            catch
            {
                MessageBox.Show("SignField 선택을 완료하세요.");
                return;
            }
            mIndexSubtract(5);
        }

        public void MoveFront()
        {
            try
            {
                mSaveCsv();
            }
            catch
            {
                MessageBox.Show("SignField 선택을 완료하세요.");
                return;
            }
            mIndexAdd(5);
        }

        public void BorderSizeChanged()
        {
            foreach (AnnoShapeModel annoShapeModel in AnnoShapeModelCollection)
            {
                CamWindowViewModel.mTranslatePosition(annoShapeModel);
            }       
        }

        public bool CanLeftButtonDown
        {
            get
            {
                return (selectedImageNodeModel != null);
            }
        }

        
        public void AnnSelectionChanged(object sender)
        {
            CamWindowViewModel.mDeleteVertices();
            foreach (var thisAnnoShapeModel in AnnoShapeModelCollection)
            {
                thisAnnoShapeModel.Shape.Fill = defaultBrush;
                thisAnnoShapeModel.Shape.Stroke = defaultStroke;
            }
            if (sender != null)
            {
                AnnoShapeModel thisAnnoShapeModel = sender as AnnoShapeModel;
                SelectedAnnoShapeModel = thisAnnoShapeModel;
                if (SelectedAnnoShapeModel.shapetype != ShapeType.line)
                    SelectedAnnoShapeModel.Shape.Fill = selectedBrush;

                CamWindowViewModel.AnnSelectionChanged();
                
                SelectedAnnoShapeModel.Shape.Stroke = selectedStroke;
                SignImage = SelectedAnnoShapeModel.signImageSource;
                NotifyOfPropertyChange(() => SignImage);
                CroppedBoxViewModel = new CroppedBoxViewModel(SelectedAnnoShapeModel, selectedImageNodeModel);
                GC.Collect();
            }
        }
        
      
        public void SignFieldChanged()
        {
            if (SelectedAnnoShapeModel != null)
            {
                if (SelectedAnnoShapeModel.signfield != null)
                {
                    SelectedAnnoShapeModel.signImageSource = "/Resources/" + ((int)SelectedAnnoShapeModel.signfield).ToString() + ".png";
                    SignImage = SelectedAnnoShapeModel.signImageSource;
                    NotifyOfPropertyChange(() => SignImage);
                    EventAggregationProvider.EventAggregator.PublishOnUIThread(SelectedAnnoShapeModel);
                }
            }
        }

        public void AnnMouseRightButtonDown(object sender)
        {
            var menuItem = (MenuItem)sender;
            var contextMenu = (ContextMenu)menuItem.Parent;
            var item = (DataGrid)contextMenu.PlacementTarget;
            AnnoShapeModel thisAnnoShapeModel = (AnnoShapeModel)item.SelectedItem;
            if (thisAnnoShapeModel != null)
            {
                CamWindow.ImageCanvas.Children.Remove(thisAnnoShapeModel.Shape);
                AnnoShapeModelCollection.Remove(thisAnnoShapeModel);
            }
        }

        public void ComboBoxOpened(object e)
        {
            (e as ComboBox).SelectedItem = null;
        }
        #endregion

        #region Models // 모델과 뷰모델 선언


        private SectionModel _sectionModel { get; set; }
        public SectionModel SectionModel
        {
            get { return _sectionModel; }
            set
            {
                _sectionModel = value;
                NotifyOfPropertyChange(() => SectionModel);
            }
        }
        private ImageNodeModel selectedImageNodeModel { get; set; }

        public ImageNodeModel SelectedImageNodeModel
        {
            get 
            { 
                return selectedImageNodeModel; 
            }
            set 
            {
                selectedImageNodeModel =  value;
                NotifyOfPropertyChange(() => SelectedImageNodeModel);                
            }
        }

        private BindableCollection<ImageNodeModel> _imageNodeModelcollection = new BindableCollection<ImageNodeModel>();
        public BindableCollection<ImageNodeModel> ImageNodeModelCollection
        {
            get { return _imageNodeModelcollection; }
            set
            {
                _imageNodeModelcollection = value;
                NotifyOfPropertyChange(() => ImageNodeModelCollection);                
            }
        }

        private CamWindowViewModel _camWindowViewModel;
        public CamWindowViewModel CamWindowViewModel
        {
            get { return _camWindowViewModel; }
            set
            {
                _camWindowViewModel = value;
                NotifyOfPropertyChange(() => CamWindowViewModel);
            }
        }

        private CroppedBoxViewModel _croppedBoxViewModel;

        public CroppedBoxViewModel CroppedBoxViewModel
        {
            get { return _croppedBoxViewModel; }
            set
            {
                _croppedBoxViewModel = value;
                NotifyOfPropertyChange(() => CroppedBoxViewModel);
            }
        }

        private int _selectedIndex { get; set; } = -1;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                NotifyOfPropertyChange(() => SelectedIndex);
            }
        }

        private AnnoShapeModel _selectedAnnoShapeModel { get; set; } = new AnnoShapeModel(ShapeType.none);
        public AnnoShapeModel SelectedAnnoShapeModel
        {
            get { return _selectedAnnoShapeModel; }
            set
            {
                _selectedAnnoShapeModel = value;
                NotifyOfPropertyChange(() => SelectedAnnoShapeModel);
                NotifyOfPropertyChange(() => AnnoShapeModelCollection);
                EventAggregationProvider.EventAggregator.PublishOnUIThread(SelectedAnnoShapeModel);
            }
        }

        private BindableCollection<AnnoShapeModel> _annoShapeModelCollection = new BindableCollection<AnnoShapeModel>();
        public BindableCollection<AnnoShapeModel> AnnoShapeModelCollection
        {
            get { return _annoShapeModelCollection; }
            set
            {
                _annoShapeModelCollection = value;
                NotifyOfPropertyChange(() => AnnoShapeModelCollection);
            }
        }

        public string TextBlockFolderName{get; set;}

        #endregion // 모델과 뷰모델 선언

        #region Handlers // Subscribe Handle 이벤트
        public void Handle(SectionModel message) // FileTabViewModel 에서 옴
        {
            _sectionModel = message;
            mReadSectionShpFile(_sectionModel.sSectionShp_FileName);
            mReadFolderStructTxtFile(_sectionModel.sFolderStructTxt_FileName);
            TextBlockFolderName = _sectionModel.sFolderName;
            NotifyOfPropertyChange(() => TextBlockFolderName);
            mIndexInit();            
        }

        public void Handle(ShapeType message)
        {
            selectedShapeType = message;
            NotifyOfPropertyChange(() => selectedShapeType);
        }


        #endregion

        #region methods // Code behind 딴에서 쓰이는 메소드
        public void mReadSectionShpFile(string sfilePath)
        {
            var ShpData = Shapefile.OpenFile(sfilePath);
            DataTable ShpfileTable = ShpData.DataTable;
            foreach (DataRow shp in ShpfileTable.Rows)
            {
                ImageNodeModel thisImageNode = new ImageNodeModel((int)shp[0], (Int16)shp[1], (Int16)shp[2], (Int16)shp[3], (string)shp[4],
                    (double)shp[5], (double)shp[6], (double)shp[7], (double)shp[8], (double)shp[9], (double)shp[10]);
                _imageNodeModelcollection.Add(thisImageNode);
            }
            SectionModel.N_images = ShpfileTable.Rows.Count;
        }
        public void mReadFolderStructTxtFile(string sfilePath)
        {
            string[] tlines = File.ReadAllLines(sfilePath, System.Text.Encoding.GetEncoding(949));
            foreach(var _thisimagenode in _imageNodeModelcollection)
            {
                for(int i =0; i<tlines.Length; i++)
                {
                    if (tlines[i].Contains("survid=" + _thisimagenode.survid))
                    {
                        string[] foldername = tlines[i + 1].Split(new char[] { '=' });
                        _thisimagenode.survfoldername = foldername[1];
                        continue;
                    }
                    if (_thisimagenode.survfoldername != null && tlines[i].Contains("trackid=" + _thisimagenode.trackid))
                    {
                        string[] foldername = tlines[i + 1].Split(new char[] { '=' });
                        _thisimagenode.trackfoldername = foldername[1];
                        continue;
                    }
                    if(_thisimagenode.trackfoldername != null && tlines[i].Contains("camid=" + _thisimagenode.camid))
                    {
                        string[] foldername = tlines[i + 1].Split(new char[] { '=' });
                        _thisimagenode.camfoldername = foldername[1];
                        _thisimagenode.imagefilepath = Directory.GetParent( _sectionModel.sFolderName) +@"\"+ _thisimagenode.survfoldername + @"\" + 
                            _thisimagenode.trackfoldername + @"\" + _thisimagenode.camfoldername + @"\" + _thisimagenode.filename;
                        _thisimagenode.AnnCsvPath = _sectionModel.sFolderName + @"\dataset_map_csv" + @"\" + _thisimagenode.imageid + ".csv";
                        break;
                    }                   
                }
            }
        }
        public void mIndexAdd(int interval)
        {
            if (_selectedIndex + interval <= SectionModel.N_images)
            {
                _selectedIndex += interval;
                NotifyOfPropertyChange(() => SelectedIndex);
                GC.Collect();
            }
        }
        public void mIndexSubtract(int interval)
        {
            if (_selectedIndex - interval >= 0)
            {
                _selectedIndex -= interval;
                NotifyOfPropertyChange(() => SelectedIndex);
                GC.Collect();
            }
        }
        public void mIndexInit()
        {
            _selectedIndex = 0;
            NotifyOfPropertyChange(() => SelectedIndex);
        }


        public void mSaveCsv()
        {
                if (Directory.Exists(Directory.GetParent(selectedImageNodeModel.AnnCsvPath).ToString()) == false)
                    Directory.CreateDirectory(Directory.GetParent(selectedImageNodeModel.AnnCsvPath).ToString());
                if (File.Exists(selectedImageNodeModel.AnnCsvPath) == false)
                    File.Create(selectedImageNodeModel.AnnCsvPath).Close();
                using (StreamWriter sw = new StreamWriter(selectedImageNodeModel.AnnCsvPath, false, Encoding.UTF8))
                {
                string Lines;

                    foreach (var AnnShape in AnnoShapeModelCollection)
                    {
                        string rowString = null;
                        foreach (var row in AnnShape.row)
                        {
                            rowString += row + ";";
                        }
                        string colString = null;
                        foreach (var col in AnnShape.col)
                        {
                            colString += col + ";";
                        }

                        Lines = $"{AnnShape.imageid},{AnnShape.shapeid},{(int)AnnShape.signfield},{(int)AnnShape.shapetype},{rowString},{colString},{AnnShape.width},{AnnShape.height},{AnnShape.memo}";
                        sw.WriteLine(Lines);
                    }
                    sw.Close();
                }
        
        }

        #endregion
    }
}
