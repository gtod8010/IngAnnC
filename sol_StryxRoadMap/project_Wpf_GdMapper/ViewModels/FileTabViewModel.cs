using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using Wpf_GdMapper.Models;
using Caliburn.Micro;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Wpf_GdMapper.ViewModels
{
    public class FileTabViewModel : Screen
    {
        public FileTabViewModel()
        {

        }

        
        #region 
        public void mSelectFolder()
        {           
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true; if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SectionModel SectionModel = new SectionModel();
                SectionModel.sFolderName = dialog.FileName;
                MessageBox.Show($"{dialog.FileName}", "폴더 선택 완료");
                SectionModel = mFillDatasetImageData(SectionModel);
                EventAggregationProvider.EventAggregator.PublishOnUIThread(SectionModel);
            }           
        }
        public SectionModel mFillDatasetImageData(SectionModel _SectionModel)
        {
            string[] sSectionShp= Directory.GetFiles(_SectionModel.sFolderName, "dataset_image_data.shp");
            _SectionModel.sSectionShp_FileName = sSectionShp[0];

            string[] sMapShp = Directory.GetFiles(_SectionModel.sFolderName, "*.shp");
            sMapShp = sMapShp.Where(val => val != _SectionModel.sSectionShp_FileName).ToArray();
            _SectionModel.sMapShp_FileName = sMapShp;

            string[] sConnectBin = Directory.GetFiles(_SectionModel.sFolderName, "*.bin");
            _SectionModel.sConnectBin_FileName = sConnectBin[0];

            string[] sFolderStructTxt = Directory.GetFiles(_SectionModel.sFolderName, "*.txt");
            _SectionModel.sFolderStructTxt_FileName = sFolderStructTxt[0];


            Console.WriteLine("========================================================");
            Console.WriteLine($"sSectionShp_FileName : {_SectionModel.sSectionShp_FileName}");
            foreach (var shp in sMapShp)
                Console.WriteLine($"sMapShp_FileName : {shp}");
            Console.WriteLine($"sConnectBin_FileName : {_SectionModel.sConnectBin_FileName}");
            Console.WriteLine($"sFolderStructTxt_FileName : {_SectionModel.sFolderStructTxt_FileName}");
            Console.WriteLine("========================================================");

            return _SectionModel;
        }
        #endregion
    }
}
