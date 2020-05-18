using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;

namespace Wpf_IMAnn.ViewModels
{
    public class WindowTabViewModel : Screen
    {
        public WindowTabViewModel()
        {
            
        }
        public void OpenHelp()
        {
            Wpf_IMAnn.Views.HelpWindowView helpWindowView = new Views.HelpWindowView();
            helpWindowView.Owner = Window.GetWindow(GetView() as Views.WindowTabView);
            helpWindowView.Show();
        }
        public void OpenCamLeft()
        {

        }
        public void OpenCamHead()
        {

        }
        public void OpenCamRight()
        {

        }
        public void OpenMap()
        {

        }
    }
}
