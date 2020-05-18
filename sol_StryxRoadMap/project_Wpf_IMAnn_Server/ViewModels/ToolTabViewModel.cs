using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using Wpf_IMAnn_Server.Models;
using Wpf_IMAnn_Server.Utils;
using Wpf_IMAnn_Server.Views;

namespace Wpf_IMAnn_Server.ViewModels
{
    class ToolTabViewModel : Screen
    {
        public ToolTabViewModel()
        {
            shotKeyViewModel = new ShortKeyViewModel();
        }

        #region Convetion methods

        IWindowManager manager = new WindowManager();
        ShortKeyViewModel shotKeyViewModel;

        public void ShortKey()
        {            
            manager.ShowWindow(shotKeyViewModel, null, null);
        }

        public void DrawPoint()
        {
            EventAggregationProvider.EventAggregator.PublishOnUIThread(ShapeType.point);
        }
        public void DrawLine()
        {
            EventAggregationProvider.EventAggregator.PublishOnUIThread(ShapeType.line);
        }
        public void DrawPolygon()
        {
            EventAggregationProvider.EventAggregator.PublishOnUIThread(ShapeType.polygon);
        }
        public void DrawBoundingBox()
        {
            EventAggregationProvider.EventAggregator.PublishOnUIThread(ShapeType.boundingbox);
        }
        #endregion
    }
}
