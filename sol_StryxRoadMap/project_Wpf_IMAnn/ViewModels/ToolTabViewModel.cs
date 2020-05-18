using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using Wpf_IMAnn.Models;
using Wpf_IMAnn.Utils;

namespace Wpf_IMAnn.ViewModels
{
    class ToolTabViewModel : Screen
    {
        public ToolTabViewModel()
        {
            
        }

        #region Convetion methods
        
        public void ShortKey()
        {
            Wpf_IMAnn.Views.ShortKeyView ShortKeyView = new Views.ShortKeyView();
            ShortKeyView.Owner = Window.GetWindow(GetView() as Views.ToolTabView);
            ShortKeyView.Show();
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
