using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using Wpf_IMAnn_Server.Models;
using Caliburn.Micro;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Wpf_IMAnn_Server.ViewModels
{
    public class FileTabViewModel : Screen
    {
        public FileTabViewModel()
        {

        }
        
        #region 
        public void mSelectFolder() // 작업폴더선택 버튼 클릭시
        {
            // REST server1 : 이미지 로드
             EventAggregationProvider.EventAggregator.PublishOnUIThread(true); //ShellViewModel의 Handle 메소드를 실행시킴.

        }
      
        #endregion
    }
}
