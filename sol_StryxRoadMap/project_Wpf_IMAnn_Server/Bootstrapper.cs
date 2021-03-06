﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Wpf_IMAnn_Server.ViewModels;
using Wpf_IMAnn_Server.Models;

namespace Wpf_IMAnn_Server
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {            
            DisplayRootViewFor<LoginViewModel>();
        }

        private SimpleContainer _container = new SimpleContainer(); 

        protected override void Configure()
        {
            _container.Instance(_container);
            _container.Singleton<IWindowManager, WindowManager>();
            _container.Singleton<ShellViewModel>();
            _container.Singleton<LoginViewModel>();
            _container.Singleton<FileTabViewModel>();
            _container.Singleton<CamWindowViewModel>();
            _container.Singleton<ShortKeyViewModel>();
            _container.Singleton<EventAggregationProvider>();
            //_container.Singleton<DatasetImageDataModel>();
            //_container.RegisterSingleton(typeof(DatasetImageDataModel), "DatasetImageDataModel", typeof(DatasetImageDataModel));
        }
        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }
        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }


    }
}
