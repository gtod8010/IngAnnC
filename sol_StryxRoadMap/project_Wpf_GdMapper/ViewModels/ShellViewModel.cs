using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Wpf_GdMapper.Models;
using DotSpatial.Projections;
using DotSpatial.Data;

namespace Wpf_GdMapper.ViewModels
{
    class ShellViewModel : Conductor<IScreen>.Collection.OneActive, IHandle<SectionModel>
    {
        public ShellViewModel()
        {
            EventAggregationProvider.EventAggregator.Subscribe(this);

        }

        #region Convention methods
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
        #endregion

        #region Models
        private SectionModel _sectionModel { get; set; }
        SectionModel SectionModel
        {
            get { return _sectionModel; }
            set
            {
                _sectionModel = value;
                NotifyOfPropertyChange(() => SectionModel);
            }
        }
        #endregion

        #region Handlers
        public void Handle(SectionModel message)
        {
            SectionModel = message;
            mReadImageDataShp(message.sFolderName+@"\dataset_image_data.shp");
        }

        #endregion

        #region methods
        public void mReadImageDataShp(string sfilePath)
        {            
            var ShpData= Shapefile.OpenFile(sfilePath);
            DataTable ShpaDataTble = ShpData.DataTable;
        }

        #endregion
    }
}
