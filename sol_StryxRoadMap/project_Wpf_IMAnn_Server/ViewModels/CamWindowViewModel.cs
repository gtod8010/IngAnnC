using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using Wpf_IMAnn_Server.Models;
using System.Windows;
using Caliburn.Micro;
using Wpf_IMAnn_Server.Views;
using Wpf_IMAnn_Server.restClient;
using System.Windows.Shapes;
using Wpf_IMAnn_Server.Utils;
using System.IO;

namespace Wpf_IMAnn_Server.ViewModels
{
    public class CamWindowViewModel : Screen, IHandle<ShapeType>, IHandle<ImageNodeModel>, IHandle<ShellViewModel>, IHandle<AnnoShapeModel>, IHandle<string>
    {
        public CamWindowViewModel()
        {
            EventAggregationProvider.EventAggregator.Subscribe(this);
            httpRequestClient = HttpRequestClient.Instance();
        }

        #region Model과 ViewModel 선언
        public CamWindowView GetCamWindowView()
        {
            object camWindowView = this.GetView();
            return (CamWindowView)camWindowView;
        }

        public string SelectedImageNode { get; set; }

    
        private AnnoShapeModel _selectedAnnoShapeModel { get; set; } = new AnnoShapeModel(ShapeType.none);
        public AnnoShapeModel SelectedAnnoShapeModel
        {
            get { return _selectedAnnoShapeModel; }
            set
            {
                _selectedAnnoShapeModel = value;
                NotifyOfPropertyChange(() => SelectedAnnoShapeModel);
            }
        }

        private AnnoShapeModel _annoShapeModel;
        public AnnoShapeModel AnnoShapeModel
        {
            get { return _annoShapeModel; }
            set
            {
                _annoShapeModel = value;
                NotifyOfPropertyChange(() => AnnoShapeModel);
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
        private ImageNodeModel _selectedImageNodeModel { get; set; }
        public ImageNodeModel SelectedImageNodeModel
        {
            get { return _selectedImageNodeModel; }
            set
            {
                _selectedImageNodeModel = value;
                NotifyOfPropertyChange(() => SelectedImageNodeModel);
            }
        }
        #endregion
        
        #region Variables
        public double OriginalWidth, OriginalHeight;
        public double CurrentHeight, CurrentWidth;
        public ShapeType selectedShapeType { get; set; }
        Rectangle boundingBoxShape = new Rectangle();
        Ellipse pointShape = new Ellipse();
        Polyline lineShape = new Polyline();
        PointCollection polylineCollection = new PointCollection();
        Polygon polygonShape = new Polygon();
        PointCollection polygonCollection = new PointCollection();

        Brush defaultStroke = Brushes.Red;
        Brush defaultBrush = new SolidColorBrush(Colors.Red) { Opacity = 0 };

        Brush selectedStroke = Brushes.Blue;
        Brush selectedBrush = new SolidColorBrush(Colors.Blue) { Opacity = 0.3 };

        Brush vertexStroke = Brushes.Blue;
        Brush vertexBrush = new SolidColorBrush(Colors.White) { Opacity = 1 };

        Point downPoint;
        ShellView ShellView;
        ShellViewModel ShellViewModel;

        //드로잉 라인
        Line tempLine;

        HttpRequestClient httpRequestClient;

        //SignField설정
        object selectedSignField = SignField.미할당;
        #endregion

        #region Handle
        public void Handle(ShapeType message)
        {
            selectedShapeType = message;
            NotifyOfPropertyChange(() => selectedShapeType);

            _annoShapeModel = null;
            polylineCollection.Clear();
            polygonCollection.Clear();
        }
        
        public void Handle(ImageNodeModel message)
        {
            mClearAllShapes();
            SelectedImageNodeModel = message;
            SelectedImageNode = null;
            SelectedImageNode = SelectedImageNodeModel.imagefilepath;
            NotifyOfPropertyChange(() => SelectedImageNode);
            // 원본 영상의 사이즈 측정
            OriginalWidth = SelectedImageNodeModel.PixelWidth;
            OriginalHeight = SelectedImageNodeModel.PixelHeight;

            mClearAnnShape();
            mLoadAnno();
        }

        public void Handle(AnnoShapeModel message)
        {
            SelectedAnnoShapeModel = message;
            selectedSignField = SelectedAnnoShapeModel.signfield;
        }

        public void Handle(ShellViewModel message)
        {
            ShellView = (ShellView)message.GetView();
            ShellViewModel = message;
        }

        public void Handle(string message)
        {
            selectedSignField = ShellViewModel.mIdentifySignField(message);
        }

        #endregion
        
        #region Mouse와 Key events
        public void CamKeyDown(object sender, KeyEventArgs e)
        {
            if (e.OriginalSource.GetType() != typeof(TextBox))
                switch (e.Key)
                {
                    case Key.Escape:
                        if (_annoShapeModel != null & _annoShapeModel.shapetype == ShapeType.polygon)
                        {
                            if (polygonCollection.Count > 1)
                            {
                                polygonCollection.RemoveAt(polygonCollection.Count - 1);
                                mDrawTempLine(polygonCollection[polygonCollection.Count - 1]);
                                _annoShapeModel.row.RemoveAt(_annoShapeModel.row.Count - 1);
                                _annoShapeModel.col.RemoveAt(_annoShapeModel.col.Count - 1);
                            }
                        }
                        break;
                }
        }

        public void LeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            downPoint = e.GetPosition(GetCamWindowView().SelectedImageNode as Image);

            switch (selectedShapeType)
            {
                case ShapeType.boundingbox:
                    var boundingBoxRect = new Rectangle()
                    {
                        Stroke = defaultStroke,
                        Fill = defaultBrush,
                        Height = 1,
                        Width = 1,
                        StrokeThickness = 1,
                        Margin = new Thickness(downPoint.X, downPoint.Y, 0, 0)
                    };
                    boundingBoxShape = boundingBoxRect;
                    GetCamWindowView().ImageCanvas.Children.Add(boundingBoxShape);
                    break;

                case ShapeType.point:
                    var pointEllipse = new Ellipse()
                    {
                        Stroke = defaultStroke,
                        Fill = defaultBrush,
                        Height = 8,
                        Width = 8,
                        StrokeThickness = 1,
                        Margin = new Thickness(downPoint.X, downPoint.Y, 0, 0)
                    };
                    pointShape = pointEllipse;
                    GetCamWindowView().ImageCanvas.Children.Add(pointShape);
                    break;

                case ShapeType.line:
                    if (polylineCollection.Count == 0)
                    {
                        var linePolyline = new Polyline()
                        {
                            Stroke = defaultStroke,
                            Fill = defaultBrush,
                            StrokeThickness = 1,
                            FillRule = FillRule.EvenOdd

                        };
                        polylineCollection.Add(downPoint);
                        linePolyline.Points = polylineCollection;
                        lineShape = linePolyline;
                        GetCamWindowView().ImageCanvas.Children.Add(lineShape);
                        mDrawTempLine(downPoint);
                    }
                    else
                    {
                        polylineCollection.Add(downPoint);
                        mDrawTempLine(downPoint);
                        return;
                    }
                    break;

                case ShapeType.polygon:
                    if (polygonCollection.Count == 0)
                    {
                        var polygonPolygon = new Polygon()
                        {
                            Stroke = defaultStroke,
                            Fill = defaultBrush,
                            StrokeThickness = 1,
                            FillRule = FillRule.EvenOdd
                        };                       
                        polygonCollection.Add(downPoint);
                        polygonPolygon.Points = polygonCollection;
                        polygonShape = polygonPolygon;
                        GetCamWindowView().ImageCanvas.Children.Add(polygonShape);
                        mDrawTempLine(downPoint);
                    }
                    else
                    {
                        polygonCollection.Add(downPoint);
                        mDrawTempLine(downPoint);
                        return;
                    }
                    break;
                default:
                    return;
            }
            _annoShapeModel = new AnnoShapeModel(selectedShapeType);
            _annoShapeModel.signfield = selectedSignField;
            _annoShapeModel.imageid = SelectedImageNodeModel.imageid;
            _annoShapeModel.shapeid = mDecideShapeid();
        }

       
        public void MouseMove(MouseEventArgs e)
        {
            if (_annoShapeModel != null)
            {
                Point movePoint = e.GetPosition(GetCamWindowView().SelectedImageNode as Image);
                if (Mouse.LeftButton == MouseButtonState.Pressed && selectedShapeType != ShapeType.line && selectedShapeType != ShapeType.polygon)
                {
                    Point initPoint = new Point();
                    if (downPoint.X > movePoint.X)
                        initPoint.X = movePoint.X;
                    else
                        initPoint.X = downPoint.X;

                    if (downPoint.Y > movePoint.Y)
                        initPoint.Y = movePoint.Y;
                    else
                        initPoint.Y = downPoint.Y;

                    boundingBoxShape.Margin = new Thickness(initPoint.X, initPoint.Y, 0, 0);

                    mGetCurrentSize();
                    if (_annoShapeModel.col.Count == 0)
                        _annoShapeModel.col.Add(OriginalWidth * initPoint.X / CurrentWidth);
                    else
                        _annoShapeModel.col[0] = (OriginalWidth * initPoint.X / CurrentWidth);

                    if (_annoShapeModel.row.Count == 0)
                        _annoShapeModel.row.Add(OriginalHeight * initPoint.Y / CurrentHeight);
                    else
                        _annoShapeModel.row[0] = (OriginalHeight * initPoint.Y / CurrentHeight);

                    boundingBoxShape.Height = Math.Abs(downPoint.Y - movePoint.Y);
                    boundingBoxShape.Width = Math.Abs(downPoint.X - movePoint.X);
                }
                else if(selectedShapeType == ShapeType.line || selectedShapeType == ShapeType.polygon)
                {
                    tempLine.X2 = movePoint.X; tempLine.Y2 = movePoint.Y;
                }
            }
        }

        public void MouseLeave(MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed && selectedShapeType != ShapeType.line && selectedShapeType != ShapeType.polygon)
                MouseUp(e);
        }

        public void MouseUp(MouseEventArgs e)
        {
            if (_annoShapeModel != null)
            {
                mGetCurrentSize();
                switch (selectedShapeType)
                {
                    case ShapeType.boundingbox:
                        _annoShapeModel.row.Add(_annoShapeModel.row[0] + OriginalHeight * boundingBoxShape.Height / CurrentHeight);
                        _annoShapeModel.col.Add(_annoShapeModel.col[0] + OriginalWidth * boundingBoxShape.Width / CurrentWidth);

                        _annoShapeModel.width = _annoShapeModel.col[1] - _annoShapeModel.col[0];
                        _annoShapeModel.height = _annoShapeModel.row[1] - _annoShapeModel.row[0];

                        GetCamWindowView().ImageCanvas.Children.Remove(boundingBoxShape);
                        if (_annoShapeModel.width < 30 | _annoShapeModel.height < 30)
                        {
                            _annoShapeModel = null;
                            return;
                        }
                        mRegisterAnnShape(_annoShapeModel);

                        mDrawAnnShapeModel(_annoShapeModel);

                        ShellView.ShapeDataGrid.SelectedIndex = AnnoShapeModelCollection.IndexOf(_annoShapeModel);

                        httpRequestClient.addAnnotation(_annoShapeModel);

                        _annoShapeModel = null;
                        break;

                    case ShapeType.point:
                        _annoShapeModel.row.Add(OriginalHeight * downPoint.Y / CurrentHeight);
                        _annoShapeModel.col.Add(OriginalWidth * downPoint.X / CurrentWidth);

                        _annoShapeModel.width = OriginalWidth * 8 / CurrentWidth;
                        _annoShapeModel.height = OriginalHeight * 8 / CurrentHeight;

                        mRegisterAnnShape(_annoShapeModel);

                        GetCamWindowView().ImageCanvas.Children.Remove(pointShape);
                        mDrawAnnShapeModel(_annoShapeModel);

                        ShellView.ShapeDataGrid.SelectedIndex = AnnoShapeModelCollection.IndexOf(_annoShapeModel);

                        httpRequestClient.addAnnotation(_annoShapeModel);

                        _annoShapeModel = null;
                        break;

                    case ShapeType.line:
                        _annoShapeModel.row.Add(OriginalHeight * downPoint.Y / CurrentHeight);
                        _annoShapeModel.col.Add(OriginalWidth * downPoint.X / CurrentWidth);
                        break;

                    case ShapeType.polygon:
                        _annoShapeModel.row.Add(OriginalHeight * downPoint.Y / CurrentHeight);
                        _annoShapeModel.col.Add(OriginalWidth * downPoint.X / CurrentWidth);
                        break;
                }
            }
            GC.Collect();
        }
        public void MouseDoubleClick(MouseEventArgs e)
        {
            if (_annoShapeModel != null)
            {
                switch (selectedShapeType)
                {
                    case ShapeType.line:
                        if (polylineCollection.Count == 2)
                        {
                            polylineCollection.Clear();
                            return;
                        }
                        mRegisterAnnShape(_annoShapeModel);
                        GetCamWindowView().ImageCanvas.Children.Remove(lineShape);
                        polylineCollection.Clear();
                        break;

                    case ShapeType.polygon:
                        if (polygonCollection.Count == 2)
                        {
                            polygonCollection.Clear();
                            return;
                        }
                        mRegisterAnnShape(_annoShapeModel);
                        GetCamWindowView().ImageCanvas.Children.Remove(polygonShape);
                        polygonCollection.Clear();
                        break;

                    default:
                        return;
                }
                mDrawAnnShapeModel(_annoShapeModel);
                e.Handled = true;
                ShellView.ShapeDataGrid.SelectedIndex = AnnoShapeModelCollection.IndexOf(_annoShapeModel);
                httpRequestClient.addAnnotation(_annoShapeModel);
                _annoShapeModel = null;
            }
        }
        #endregion

        #region Methods
        public void mClearAllShapes()
        {
            GetCamWindowView().ImageCanvas.Children.RemoveRange(1, GetCamWindowView().ImageCanvas.Children.Count - 1);
        }

        private void mDrawTempLine(Point point)
        {
            GetCamWindowView().ImageCanvas.Children.Remove(tempLine);
            //tempLine
            tempLine = new Line()
            {
                Stroke = defaultStroke,
                Fill = defaultBrush,
                StrokeThickness = 1,
                X1 = point.X,
                Y1 = point.Y,
                X2 = point.X,
                Y2 = point.Y
            };
            GetCamWindowView().ImageCanvas.Children.Add(tempLine);
            tempLine.MouseLeftButtonDown += new MouseButtonEventHandler(LeftButtonDown);
        }

        public void mLoadAnno()
        {
            httpRequestClient.loadImageIntoWorkplace(SelectedImageNodeModel);
            List<AnnoShapeModel> List_AnnoShapeModel = httpRequestClient.getAnnotations(SelectedImageNodeModel.imageid);
            foreach (AnnoShapeModel thisAnnoShapeModel in List_AnnoShapeModel)
            {
                mRegisterAnnShape(thisAnnoShapeModel);
                mDrawAnnShapeModel(thisAnnoShapeModel);
            }
        }
      
        public void mRegisterAnnShape(AnnoShapeModel annoShapeModel)
        {
            AnnoShapeModelCollection.Add(annoShapeModel);
        }

        public void mClearAnnShape()
        {
            AnnoShapeModelCollection.Clear();
        }

        public int mDecideShapeid()
        {
            if (AnnoShapeModelCollection.Count == 0)
                return 0;
            else
                return AnnoShapeModelCollection.Max().shapeid + 1;
        }

        public void mDrawAnnShapeModel(AnnoShapeModel thisAnnoShapeModel)
        {
            mGetCurrentSize();
            switch (thisAnnoShapeModel.shapetype)
            {
                case ShapeType.boundingbox:
                    var boundingBoxRect = new Rectangle()
                    {
                        Stroke = defaultStroke,
                        Fill = defaultBrush,
                        Height = CurrentHeight * thisAnnoShapeModel.height / OriginalHeight,
                        Width = CurrentWidth * thisAnnoShapeModel.width / OriginalWidth,
                        StrokeThickness = 1,
                        Margin = new Thickness(CurrentWidth * thisAnnoShapeModel.col[0] / OriginalWidth, CurrentHeight * thisAnnoShapeModel.row[0] / OriginalHeight, 0, 0)
                    };
                    thisAnnoShapeModel.Shape = boundingBoxRect;

                    AddEventHandlers(boundingBoxRect);

                    GetCamWindowView().ImageCanvas.Children.Add(boundingBoxRect);
                    break;

                case ShapeType.point:
                    var pointEllipse = new Ellipse()
                    {
                        Stroke = defaultStroke,
                        Fill = defaultBrush,
                        Height = 8,
                        Width = 8,
                        StrokeThickness = 1,
                        Margin = new Thickness(CurrentWidth * thisAnnoShapeModel.col[0] / OriginalWidth, CurrentHeight * thisAnnoShapeModel.row[0] / OriginalHeight, 0, 0)
                    };
                    thisAnnoShapeModel.Shape = pointEllipse;
                    AddEventHandlers(pointEllipse);
                    GetCamWindowView().ImageCanvas.Children.Add(pointEllipse);
                    break;
                case ShapeType.line:
                    var linePolyline = new Polyline()
                    {
                        Stroke = defaultStroke,
                        Fill = defaultBrush,
                        StrokeThickness = 1,
                        FillRule = FillRule.EvenOdd
                    };
                    PointCollection linePolylineCollection = new PointCollection();

                    for (int i = 0; i < thisAnnoShapeModel.row.Count; i++)
                    {
                        Point point = new Point() { X = CurrentWidth * thisAnnoShapeModel.col[i] / OriginalWidth, Y = CurrentHeight * thisAnnoShapeModel.row[i] / OriginalHeight };
                        linePolylineCollection.Add(point);
                    }
                    linePolyline.Points = linePolylineCollection;

                    thisAnnoShapeModel.Shape = linePolyline;
                    AddEventHandlers(linePolyline);
                    GetCamWindowView().ImageCanvas.Children.Add(linePolyline);
                    break;
                case ShapeType.polygon:
                    var polygonPolygon = new Polygon()
                    {
                        Stroke = defaultStroke,
                        Fill = defaultBrush,
                        StrokeThickness = 1,
                        FillRule = FillRule.EvenOdd
                    };
                    PointCollection polygonPolygonCollection = new PointCollection();

                    for (int i = 0; i < thisAnnoShapeModel.row.Count; i++)
                    {
                        Point point = new Point() { X = CurrentWidth * thisAnnoShapeModel.col[i] / OriginalWidth, Y = CurrentHeight * thisAnnoShapeModel.row[i] / OriginalHeight };
                        polygonPolygonCollection.Add(point);
                    }
                    polygonPolygon.Points = polygonPolygonCollection;

                    thisAnnoShapeModel.Shape = polygonPolygon;
                    AddEventHandlers(polygonPolygon);
                    GetCamWindowView().ImageCanvas.Children.Add(polygonPolygon);
                    break;
            }
        }

        public void mGetCurrentSize()
        {
            GetCamWindowView().SelectedImageNode.UpdateLayout();
            CurrentWidth = GetCamWindowView().SelectedImageNode.ActualHeight / SelectedImageNodeModel.PixelHeight * SelectedImageNodeModel.PixelWidth;
            CurrentHeight = GetCamWindowView().SelectedImageNode.ActualHeight;

            if (CurrentWidth == 0)
            {
                CurrentWidth = ShellView.CamWindowViewModel.ActualWidth;
                CurrentHeight = ShellView.CamWindowViewModel.ActualHeight;
            }
        }

        private void AddEventHandlers(System.Windows.Shapes.Shape shape)
        {
            shape.MouseRightButtonDown += DeleteShape;
            shape.MouseEnter += MouseEnterShape;
            shape.MouseLeave += MouseLeaveShape;
            shape.PreviewMouseLeftButtonDown += MouseLeftButtonDownShape;
            shape.MouseLeftButtonUp += MouseLeftButtonUpShape;
        }

        private void DeleteShape(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Shapes.Shape selectedRectangle = sender as System.Windows.Shapes.Shape;

            foreach (AnnoShapeModel shapeModel in AnnoShapeModelCollection)
            {
                if (shapeModel.Shape == selectedRectangle)
                {
                    GetCamWindowView().ImageCanvas.Children.Remove(selectedRectangle);
                    httpRequestClient.deleteAnnotation(shapeModel);
                    AnnoShapeModelCollection.Remove(shapeModel);
                    break;
                }
            }
        }
        private void MouseEnterShape(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void MouseLeaveShape(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void MouseLeftButtonDownShape(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Shapes.Shape selectedShape = sender as System.Windows.Shapes.Shape;

            foreach (AnnoShapeModel shapeModel in AnnoShapeModelCollection)
            {
                if (shapeModel.Shape == selectedShape)
                {
                    ShellView.ShapeDataGrid.SelectedItem = shapeModel;
                    ShellViewModel.AnnSelectionChanged(shapeModel);
                }
            }
            e.Handled = true;
        }
        
        private void MouseLeftButtonUpShape(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        public void mTranslatePosition(AnnoShapeModel thisnAnnoShapeModel)
        {
            try
            {
                mGetCurrentSize();            
                L_X = (SelectedAnnoShapeModel.col[0]) / OriginalWidth * CurrentWidth - size / 2;
                U_Y = (SelectedAnnoShapeModel.row[0]) / OriginalHeight * CurrentHeight - size / 2;
                R_X = (SelectedAnnoShapeModel.col[0] + SelectedAnnoShapeModel.width) / OriginalWidth * CurrentWidth - size / 2;
                D_Y = (SelectedAnnoShapeModel.row[0] + SelectedAnnoShapeModel.height) / OriginalHeight * CurrentHeight - size / 2;

                if (GetCamWindowView().ImageCanvas.Children.Contains(LD))
                {
                    LD.Margin = new Thickness(L_X, D_Y, 0, 0);
                }
                if (GetCamWindowView().ImageCanvas.Children.Contains(LU))
                {
                    LU.Margin = new Thickness(L_X, U_Y, 0, 0);
                }
                if (GetCamWindowView().ImageCanvas.Children.Contains(RU))
                {
                    RU.Margin = new Thickness(R_X, U_Y, 0, 0);
                }
                if (GetCamWindowView().ImageCanvas.Children.Contains(RD))
                {
                    RD.Margin = new Thickness(R_X, D_Y, 0, 0);
                }
                switch (thisnAnnoShapeModel.shapetype)
                {
                    case ShapeType.boundingbox:
                        Rectangle boundingBox = thisnAnnoShapeModel.Shape as Rectangle;
                        boundingBox.Margin = new Thickness(CurrentWidth * thisnAnnoShapeModel.col[0] / OriginalWidth, CurrentHeight * thisnAnnoShapeModel.row[0] / OriginalHeight, 0, 0);
                        boundingBox.Width = CurrentWidth * thisnAnnoShapeModel.width / OriginalWidth;
                        boundingBox.Height = CurrentHeight * thisnAnnoShapeModel.height / OriginalHeight;
                        break;

                    case ShapeType.point:
                        Ellipse point = thisnAnnoShapeModel.Shape as Ellipse;
                        point.Margin = new Thickness(CurrentWidth * thisnAnnoShapeModel.col[0] / OriginalWidth, CurrentHeight * thisnAnnoShapeModel.row[0] / OriginalHeight, 0, 0);
                        point.Width = CurrentWidth * thisnAnnoShapeModel.width / OriginalWidth;
                        point.Height = CurrentHeight * thisnAnnoShapeModel.height / OriginalHeight;
                        break;

                    case ShapeType.line:
                        Polyline line = thisnAnnoShapeModel.Shape as Polyline;
                        PointCollection lineCollection = new PointCollection();
                        line.Points = null;
                        for (int i = 0; i < thisnAnnoShapeModel.row.Count; i++)
                        {
                            Point pt = new Point() { X = CurrentWidth * thisnAnnoShapeModel.col[i] / OriginalWidth, Y = CurrentHeight * thisnAnnoShapeModel.row[i] / OriginalHeight };
                            lineCollection.Add(pt);
                        }
                        line.Points = lineCollection;
                        break;

                    case ShapeType.polygon:
                        Polygon polygon = thisnAnnoShapeModel.Shape as Polygon;
                        PointCollection polygonCollection = new PointCollection();
                        polygon.Points = null;
                        for (int i = 0; i < thisnAnnoShapeModel.row.Count; i++)
                        {
                            Point pt = new Point() { X = CurrentWidth * thisnAnnoShapeModel.col[i] / OriginalWidth, Y = CurrentHeight * thisnAnnoShapeModel.row[i] / OriginalHeight };
                            polygonCollection.Add(pt);
                        }
                        polygon.Points = polygonCollection;
                        break;
                }
            }
            catch
            {

            }
        }

        double L_X,U_Y,R_X,D_Y;
        Ellipse LU, LD, RU, RD;
        List<Ellipse> verticesList = new List<Ellipse>();
        int size = 8;
        public void AnnSelectionChanged()
        {
            mDeleteVertices();
            if (SelectedAnnoShapeModel.shapetype == ShapeType.boundingbox)
            {
                #region BBox 점편집 관련
                mGetCurrentSize();
                L_X = (SelectedAnnoShapeModel.col[0]) / OriginalWidth * CurrentWidth - size / 2;
                U_Y = (SelectedAnnoShapeModel.row[0]) / OriginalHeight * CurrentHeight - size / 2;
                R_X = (SelectedAnnoShapeModel.col[0] + SelectedAnnoShapeModel.width) / OriginalWidth * CurrentWidth - size / 2;
                D_Y = (SelectedAnnoShapeModel.row[0] + SelectedAnnoShapeModel.height) / OriginalHeight * CurrentHeight - size / 2;

                LU = new Ellipse
                {
                    Width = size,
                    Height = size,
                    Fill = vertexBrush,
                    Stroke = vertexStroke,
                    Margin = new Thickness(L_X, U_Y, 0, 0)
                };
                LD = new Ellipse
                {
                    Width = size,
                    Height = size,
                    Fill = vertexBrush,
                    Stroke = vertexStroke,
                    Margin = new Thickness(L_X, D_Y, 0, 0)
                };
                RU = new Ellipse
                {
                    Width = size,
                    Height = size,
                    Fill = vertexBrush,
                    Stroke = vertexStroke,
                    Margin = new Thickness(R_X, U_Y, 0, 0)
                };
                RD = new Ellipse
                {
                    Width = size,
                    Height = size,
                    Fill = vertexBrush,
                    Stroke = vertexStroke,
                    Margin = new Thickness(R_X, D_Y, 0, 0)
                };
                GetCamWindowView().ImageCanvas.Children.Add(LU);
                GetCamWindowView().ImageCanvas.Children.Add(LD);
                GetCamWindowView().ImageCanvas.Children.Add(RU);
                GetCamWindowView().ImageCanvas.Children.Add(RD);

                LU.MouseMove += LU_MouseMove;
                LU.MouseLeave += LU_MouseMove;
                LU.MouseLeftButtonDown += MouseLeftButtonDown;
                LU.MouseEnter += CursorNWSE;

                LD.MouseMove += LD_MouseMove;
                LD.MouseLeave += LD_MouseMove;
                LD.MouseLeftButtonDown += MouseLeftButtonDown;
                LD.MouseEnter += CursorNESW;

                RU.MouseMove += RU_MouseMove;
                RU.MouseLeave += RU_MouseMove;
                RU.MouseLeftButtonDown += MouseLeftButtonDown;
                RU.MouseEnter += CursorNESW;

                RD.MouseMove += RD_MouseMove;
                RD.MouseLeave += RD_MouseMove;
                RD.MouseLeftButtonDown += MouseLeftButtonDown;
                RD.MouseEnter += CursorNWSE;
                #endregion
            }
            else if(SelectedAnnoShapeModel.shapetype == ShapeType.polygon)
            {
                #region Polygon 점편집 관련
                mGetCurrentSize();
                Polygon selectedPolygon = SelectedAnnoShapeModel.Shape as Polygon;
                for (int i=0; i< selectedPolygon.Points.Count; i++)
                {
                    Ellipse vertex = new Ellipse
                    {
                        Width = size,
                        Height = size,
                        Stroke = vertexStroke,
                        Fill = vertexBrush,
                        StrokeThickness = 1,
                        Margin = new Thickness(selectedPolygon.Points[i].X - size / 2, selectedPolygon.Points[i].Y - size / 2, 0, 0)
                    };
                    verticesList.Add(vertex);
                    GetCamWindowView().ImageCanvas.Children.Add(vertex);
                    vertex.MouseMove += Vertex_MouseMove;
                    vertex.MouseLeftButtonDown += Vertex_MouseLeftButtonDown;
                    vertex.MouseLeftButtonUp += Vertex_MouseLeftButtonUp;
                    vertex.MouseEnter += Vertex_MouseEnter;
                    vertex.MouseLeave += Vertex_MouseMove;
                }
                #endregion
            }
        }

        public void CropBoxUpdate()
        {
            ShellViewModel.CroppedBoxViewModel = new CroppedBoxViewModel(SelectedAnnoShapeModel, SelectedImageNodeModel);
        }

        #region BBox 점편집

        private void CursorNESW(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.SizeNESW;
        }

        private void CursorNWSE(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.SizeNWSE;
        }

        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            e.Handled=true;
        }

        Point movedPoint;
      
        private void LU_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.RoutedEvent == Mouse.MouseLeaveEvent)
                Mouse.OverrideCursor = Cursors.Arrow;
            if (_annoShapeModel == null)
            {
                movedPoint = e.GetPosition(GetCamWindowView().SelectedImageNode as Image);
                double BoxWidth = R_X - movedPoint.X + size / 2;
                double BoxHeight = D_Y - movedPoint.Y + size / 2;
                if (BoxWidth > 20 && BoxHeight >20)
                {
                    if (Mouse.LeftButton == MouseButtonState.Pressed)
                    {
                        (sender as Ellipse).Margin = new Thickness(movedPoint.X - size / 2, movedPoint.Y - size / 2, 0, 0);
                        (SelectedAnnoShapeModel.Shape as Rectangle).Width = BoxWidth;
                        (SelectedAnnoShapeModel.Shape as Rectangle).Height = BoxHeight;
                        (SelectedAnnoShapeModel.Shape as Rectangle).Margin = new Thickness(movedPoint.X, movedPoint.Y, 0, 0);

                        RU.Margin = new Thickness(movedPoint.X + (SelectedAnnoShapeModel.Shape as Rectangle).Width - size / 2, movedPoint.Y - size / 2, 0, 0);
                        LD.Margin = new Thickness(movedPoint.X - size / 2, movedPoint.Y + (SelectedAnnoShapeModel.Shape as Rectangle).Height - size / 2, 0, 0);
                        RD.Margin = new Thickness(movedPoint.X + (SelectedAnnoShapeModel.Shape as Rectangle).Width - size / 2, movedPoint.Y + (SelectedAnnoShapeModel.Shape as Rectangle).Height - size / 2,0,0);
                    }
                    else
                    {
                        L_X = LD.Margin.Left;
                        U_Y = RU.Margin.Top;
                        R_X = RU.Margin.Left;
                        D_Y = LD.Margin.Top;
                        EditSelectedAnno();
                    }
                }
            }
        }

       

        private void RD_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.RoutedEvent == Mouse.MouseLeaveEvent)
                Mouse.OverrideCursor = Cursors.Arrow;
            if (_annoShapeModel == null)
            {
                movedPoint = e.GetPosition(GetCamWindowView().SelectedImageNode as Image);
                double BoxWidth = movedPoint.X - L_X - size / 2 ;
                double BoxHeight = movedPoint.Y - U_Y - size / 2;
                if (BoxWidth > 20 && BoxHeight>20)
                {
                    if (Mouse.LeftButton == MouseButtonState.Pressed)
                    {
                        (sender as Ellipse).Margin = new Thickness(movedPoint.X - size / 2, movedPoint.Y - size / 2, 0, 0);
                        (SelectedAnnoShapeModel.Shape as Rectangle).Width = BoxWidth;
                        (SelectedAnnoShapeModel.Shape as Rectangle).Height = BoxHeight;

                        RU.Margin = new Thickness(movedPoint.X - size / 2, movedPoint.Y - (SelectedAnnoShapeModel.Shape as Rectangle).Height - size / 2, 0, 0);
                        LD.Margin = new Thickness(movedPoint.X - (SelectedAnnoShapeModel.Shape as Rectangle).Width - size / 2, movedPoint.Y - size / 2, 0, 0);
                        LU.Margin = new Thickness(movedPoint.X - (SelectedAnnoShapeModel.Shape as Rectangle).Width - size / 2, movedPoint.Y - (SelectedAnnoShapeModel.Shape as Rectangle).Height - size / 2, 0,0);
                    }
                    else
                    {
                        L_X = LD.Margin.Left;
                        U_Y = RU.Margin.Top;
                        R_X = RU.Margin.Left;
                        D_Y = LD.Margin.Top;
                        EditSelectedAnno();
                    }
                }                
            }
        }

     
        private void RU_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.RoutedEvent == Mouse.MouseLeaveEvent)
                Mouse.OverrideCursor = Cursors.Arrow;
            if (_annoShapeModel == null)
            {
                movedPoint = e.GetPosition(GetCamWindowView().SelectedImageNode as Image);
                double BoxWidth = movedPoint.X - L_X - size / 2;
                double BoxHeight = D_Y - movedPoint.Y + size / 2;
                if (BoxWidth >20 && BoxHeight > 20)
                {
                    if (Mouse.LeftButton == MouseButtonState.Pressed)
                    {
                        (sender as Ellipse).Margin = new Thickness(movedPoint.X - size / 2, movedPoint.Y - size / 2, 0, 0);
                        (SelectedAnnoShapeModel.Shape as Rectangle).Width = BoxWidth;
                        (SelectedAnnoShapeModel.Shape as Rectangle).Height = BoxHeight;
                        (SelectedAnnoShapeModel.Shape as Rectangle).Margin = new Thickness(movedPoint.X - (SelectedAnnoShapeModel.Shape as Rectangle).Width, movedPoint.Y, 0, 0);

                        LU.Margin = new Thickness(movedPoint.X - (SelectedAnnoShapeModel.Shape as Rectangle).Width - size / 2, movedPoint.Y - size / 2, 0, 0);
                        RD.Margin = new Thickness(movedPoint.X - size / 2, movedPoint.Y + (SelectedAnnoShapeModel.Shape as Rectangle).Height - size / 2, 0, 0);
                        LD.Margin = new Thickness(movedPoint.X - (SelectedAnnoShapeModel.Shape as Rectangle).Width - size / 2, movedPoint.Y + (SelectedAnnoShapeModel.Shape as Rectangle).Height - size / 2, 0,0);
                    }
                    else
                    {
                        L_X = LU.Margin.Left;
                        U_Y = LU.Margin.Top;
                        R_X = RD.Margin.Left;
                        D_Y = RD.Margin.Top;
                        EditSelectedAnno();
                    }
                }
            }
        }     

        private void LD_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.RoutedEvent == Mouse.MouseLeaveEvent)
                Mouse.OverrideCursor = Cursors.Arrow;
            if (_annoShapeModel == null)
            {
                movedPoint = e.GetPosition(GetCamWindowView().SelectedImageNode as Image);
                double BoxWidth = R_X - movedPoint.X + size / 2;
                double BoxHeight = movedPoint.Y - U_Y - size / 2;
                if (BoxWidth > 20 && BoxHeight > 20)
                {
                    if (Mouse.LeftButton == MouseButtonState.Pressed)
                    {
                        (sender as Ellipse).Margin = new Thickness(movedPoint.X - size / 2, movedPoint.Y - size / 2, 0, 0);
                        (SelectedAnnoShapeModel.Shape as Rectangle).Width = BoxWidth;
                        (SelectedAnnoShapeModel.Shape as Rectangle).Height = BoxHeight;
                        (SelectedAnnoShapeModel.Shape as Rectangle).Margin = new Thickness(movedPoint.X, movedPoint.Y - (SelectedAnnoShapeModel.Shape as Rectangle).Height, 0, 0);

                        LU.Margin = new Thickness(movedPoint.X - size / 2, movedPoint.Y - (SelectedAnnoShapeModel.Shape as Rectangle).Height - size / 2, 0, 0);
                        RD.Margin = new Thickness(movedPoint.X + (SelectedAnnoShapeModel.Shape as Rectangle).Width - size / 2, movedPoint.Y - size / 2, 0, 0);
                        RU.Margin = new Thickness(movedPoint.X + (SelectedAnnoShapeModel.Shape as Rectangle).Width - size / 2, movedPoint.Y - (SelectedAnnoShapeModel.Shape as Rectangle).Height - size / 2, 0, 0);
                    }
                    else
                    {
                        L_X = LU.Margin.Left;
                        U_Y = LU.Margin.Top;
                        R_X = RD.Margin.Left;
                        D_Y = RD.Margin.Top;
                        EditSelectedAnno();
                    }
                }
            }
        }

        private void EditSelectedAnno()
        {
            SelectedAnnoShapeModel.col[0] = (L_X + size / 2) / CurrentWidth * OriginalWidth;
            SelectedAnnoShapeModel.col[1] = (R_X + size / 2) / CurrentWidth * OriginalWidth;
            SelectedAnnoShapeModel.row[0] = (U_Y + size / 2) / CurrentHeight * OriginalHeight;
            SelectedAnnoShapeModel.row[1] = (D_Y + size / 2) / CurrentHeight * OriginalHeight;
            SelectedAnnoShapeModel.width = SelectedAnnoShapeModel.col[1] - SelectedAnnoShapeModel.col[0];
            SelectedAnnoShapeModel.height = SelectedAnnoShapeModel.row[1] - SelectedAnnoShapeModel.row[0];
            httpRequestClient.modifyAnnotation(SelectedAnnoShapeModel);
            CropBoxUpdate();
        }

        public void mDeleteVertices()
        {
            if (GetCamWindowView().ImageCanvas.Children.Contains(LD))
                GetCamWindowView().ImageCanvas.Children.Remove(LD);
            if (GetCamWindowView().ImageCanvas.Children.Contains(LU))
                GetCamWindowView().ImageCanvas.Children.Remove(LU);
            if (GetCamWindowView().ImageCanvas.Children.Contains(RU))
                GetCamWindowView().ImageCanvas.Children.Remove(RU);
            if (GetCamWindowView().ImageCanvas.Children.Contains(RD))
                GetCamWindowView().ImageCanvas.Children.Remove(RD);
            foreach(var vertex in verticesList)
            {
                if (GetCamWindowView().ImageCanvas.Children.Contains(vertex))
                    GetCamWindowView().ImageCanvas.Children.Remove(vertex);
            }            
        }
        #endregion

        #region Polygon 점편집
        private void Vertex_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void Vertex_MouseMove(object sender, MouseEventArgs e)
        {
            Point movedPoint = e.GetPosition(GetCamWindowView().SelectedImageNode);
            Polygon selectedPolygon = SelectedAnnoShapeModel.Shape as Polygon;
            if (e.RoutedEvent == Mouse.MouseLeaveEvent)
                Mouse.OverrideCursor = Cursors.Arrow;
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                Point vertexPoint = new Point() { X = (sender as Ellipse).Margin.Left + size / 2, Y = (sender as Ellipse).Margin.Top + size / 2 };
                (sender as Ellipse).Margin = new Thickness(movedPoint.X - size / 2, movedPoint.Y - size / 2, 0, 0);
                int vertexIndex = -1;
                foreach (var point in selectedPolygon.Points)
                {
                    if (point == vertexPoint)
                    {
                        vertexIndex = selectedPolygon.Points.IndexOf(point);                        
                        break;
                    }
                }
                if (vertexIndex != -1)
                {
                    selectedPolygon.Points[vertexIndex] = movedPoint;
                    SelectedAnnoShapeModel.col[vertexIndex] = movedPoint.X / CurrentWidth * OriginalWidth;
                    SelectedAnnoShapeModel.row[vertexIndex] = movedPoint.Y / CurrentHeight * OriginalHeight;

                }
            }
            e.Handled = true;
        }

        private void Vertex_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CropBoxUpdate();
        }

        private void Vertex_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
        }
              
        #endregion

        #endregion
    }
}
