﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace Wpf_IMAnn_Server.Models
{
    public class SectionModel
    {
        public SectionModel()
        {
           
        }
        public string sFolderName = @"";

        public string sSectionShp_FileName = @"";
        public string[] sMapShp_FileName;
        public string sConnectBin_FileName = @"";
        public string sFolderStructTxt_FileName = @"";

        public int N_images;
    }
}
