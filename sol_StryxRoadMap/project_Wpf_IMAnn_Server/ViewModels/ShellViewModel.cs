using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using Wpf_IMAnn_Server.Models;
using Wpf_IMAnn_Server.Utils;
using Wpf_IMAnn_Server.restClient;
using System.Windows.Controls;
using System.Windows.Input;
using ShapeType = Wpf_IMAnn_Server.Utils.ShapeType;
using Wpf_IMAnn_Server.Views;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace Wpf_IMAnn_Server.ViewModels
{    
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive, IHandle<bool>, IHandle<ShapeType>, IHandle<string[]>
    {        
        public ShellViewModel()
        {
            AppDomain appDomain = AppDomain.CurrentDomain;
            appDomain.UnhandledException += new UnhandledExceptionEventHandler(ShutDown);
            EventAggregationProvider.EventAggregator.Subscribe(this);
            ActivateItem(new FileTabViewModel());
            CamWindowViewModel = new CamWindowViewModel();

            httpRequestClient = HttpRequestClient.Instance();
        }

        public void ShutDown(object sender, UnhandledExceptionEventArgs args)
        {
            if(selectedImageNodeModel != null)
                MessageBox.Show($"프로그램이 영상 번호 [{selectedImageNodeModel.imageid}]에서 종료됩니다.","종료");
            if(SelectedImageNodeModel != null)
                httpRequestClient.commitImage(SelectedImageNodeModel);
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

        HttpRequestClient httpRequestClient;
        
        #endregion

        #region Models // 모델과 뷰모델 선언

        private object _shortKey_F1;
        public object ShortKey_F1
        {
            get { return _shortKey_F1; }
            set
            {
                _shortKey_F1 = value;
                NotifyOfPropertyChange(() => ShortKey_F1);
            }
        }

        private object _shortKey_F2;
        public object ShortKey_F2
        {
            get { return _shortKey_F2; }
            set
            {
                _shortKey_F2 = value;
                NotifyOfPropertyChange(() => ShortKey_F2);
            }
        }

        private object _shortKey_F3;
        public object ShortKey_F3
        {
            get { return _shortKey_F3; }
            set
            {
                _shortKey_F3 = value;
                NotifyOfPropertyChange(() => ShortKey_F3);
            }
        }

        private object _shortKey_F4;
        public object ShortKey_F4
        {
            get { return _shortKey_F4; }
            set
            {
                _shortKey_F4 = value;
                NotifyOfPropertyChange(() => ShortKey_F4);
            }
        }

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
                selectedImageNodeModel = value;
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

        public string TextBlockFolderName { get; set; }

        #endregion // 모델과 뷰모델 선언
        
        #region Convention methods // Caliburn.micro의 Convention에 의한 메소드

        public void Closing()
        {
            if (SelectedImageNodeModel != null)
                httpRequestClient.commitImage(SelectedImageNodeModel);
        }
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
             
        public void MemoButton()
        {
            httpRequestClient.modifyAnnotation(SelectedAnnoShapeModel);
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

            OriginalWidth = SelectedImageNodeModel.PixelWidth;
            OriginalHeight = SelectedImageNodeModel.PixelHeight;

            if (ImageNodeModelCollection.Count - _selectedIndex == 2)
            {
                List<ImageNodeModel> List_ImageNodeModels = httpRequestClient.getImageUrl(10);
                foreach (ImageNodeModel thisImageNodeModel in List_ImageNodeModels)
                    _imageNodeModelcollection.Add(thisImageNodeModel); 
            }
        }

        public void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.OriginalSource.GetType() != typeof(TextBox))
                switch (e.Key)
                {
                    case Key.A:
                        try
                        {
                           // mSaveAnno();
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
                            //mSaveAnno();
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
                    case Key.F1:
                        SignImage  = "/Resources/" + ((int)ShortKey_F1).ToString() + ".png";
                        NotifyOfPropertyChange(() => SignImage);
                        EventAggregationProvider.EventAggregator.PublishOnUIThread(ShortKey_F1.ToString());

                        break;
                    case Key.F2:
                        SignImage = "/Resources/" + ((int)ShortKey_F2).ToString() + ".png";
                        NotifyOfPropertyChange(() => SignImage);
                        EventAggregationProvider.EventAggregator.PublishOnUIThread(ShortKey_F2.ToString());
                        break;
                    case Key.F3:
                        SignImage = "/Resources/" + ((int)ShortKey_F3).ToString() + ".png";
                        NotifyOfPropertyChange(() => SignImage);
                        EventAggregationProvider.EventAggregator.PublishOnUIThread(ShortKey_F3.ToString());
                        break;
                    case Key.F4:
                        SignImage = "/Resources/" + ((int)ShortKey_F4).ToString() + ".png";
                        NotifyOfPropertyChange(() => SignImage);
                        EventAggregationProvider.EventAggregator.PublishOnUIThread(ShortKey_F4.ToString());
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
                //mSaveAnno();
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
                //mSaveAnno();
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
                    httpRequestClient.modifyAnnotation(SelectedAnnoShapeModel);
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

        #region Handlers // Subscribe Handle 이벤트
        public  void Handle(bool message) // FileTabViewModel 에서 옴
        {          
            // REST server1 : 이미지 로드
            List<ImageNodeModel> List_ImageNodeModels = httpRequestClient.getImageUrl(10);
            foreach(ImageNodeModel thisImageNodeModel in List_ImageNodeModels)
                _imageNodeModelcollection.Add(thisImageNodeModel);
            mIndexInit();
        }

        public void Handle(ShapeType message)
        {
            selectedShapeType = message;
            NotifyOfPropertyChange(() => selectedShapeType);
        }

        public void Handle(string[] message)
        {   
            if(message[0] != null)
            {
                ShortKey_F1 = mIdentifySignField(message[0]);
                NotifyOfPropertyChange(() => ShortKey_F1);
            }
            if (message[1] != null)
            {
                ShortKey_F2 = mIdentifySignField(message[1]);
                NotifyOfPropertyChange(() => ShortKey_F2);
            }
            if (message[2] != null)
            {
                ShortKey_F3 = mIdentifySignField(message[2]);
                NotifyOfPropertyChange(() => ShortKey_F3);
            }
            if (message[3] != null)
            {
                ShortKey_F4 = mIdentifySignField(message[3]);
                NotifyOfPropertyChange(() => ShortKey_F4);
            }
        }

        #endregion

        public object mIdentifySignField(string signFieldContent)
        {
            if (Enum.IsDefined(typeof(SignField_Caution), signFieldContent))
                return (SignField_Caution)Enum.Parse(typeof(SignField_Caution), signFieldContent.ToString());
            else if (Enum.IsDefined(typeof(SignField_Instruction), signFieldContent))
                return (SignField_Instruction)Enum.Parse(typeof(SignField_Instruction), signFieldContent.ToString());
            else if (Enum.IsDefined(typeof(SignField_Regulation), signFieldContent))
                return (SignField_Regulation)Enum.Parse(typeof(SignField_Regulation), signFieldContent.ToString());
            else if (Enum.IsDefined(typeof(LightField), signFieldContent))
                return (LightField)Enum.Parse(typeof(LightField), signFieldContent.ToString());
            else if (Enum.IsDefined(typeof(SurfaceMark), signFieldContent))
                return (SurfaceMark)Enum.Parse(typeof(SurfaceMark), signFieldContent.ToString());
            else if (Enum.IsDefined(typeof(SignField_ETC), signFieldContent))
                return (SignField_ETC)Enum.Parse(typeof(SignField_ETC), signFieldContent.ToString());
            else if (Enum.IsDefined(typeof(SignField_Assistant), signFieldContent))
                return (SignField_Assistant)Enum.Parse(typeof(SignField_Assistant), signFieldContent.ToString());
            else if (Enum.IsDefined(typeof(Construction), signFieldContent))
                return (Construction)Enum.Parse(typeof(Construction), signFieldContent.ToString());
            else if (Enum.IsDefined(typeof(Human), signFieldContent))
                return (Human)Enum.Parse(typeof(Human), signFieldContent.ToString());
            else if (Enum.IsDefined(typeof(Car), signFieldContent))
                return (Car)Enum.Parse(typeof(Car), signFieldContent.ToString());
            else
                return SignField.미할당;
        }

        #region methods // Code behind 딴에서 쓰이는 메소드


        public void mIndexAdd(int interval)
        {
            if (_selectedIndex + interval <= ImageNodeModelCollection.Count)
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

        #endregion
    }
}
