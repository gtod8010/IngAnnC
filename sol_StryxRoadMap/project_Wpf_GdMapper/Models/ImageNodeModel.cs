using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf_IMAnn.Models
{
    public class ImageNodeModel
    {
        public ImageNodeModel(int _imageid, int _survid, int _trackid, int _camid, string _filename, double _utm_x, double _utm_y, double _height, double _roll, double _pitch, double _heading)
        {
            imageid = _imageid;
            survid = _survid;
            trackid = _trackid;
            camid = _camid;
            filename = _filename;

            utm_x = _utm_x;
            utm_y = _utm_y;
            height = _height;

            roll = _roll;
            pitch = _pitch;
            heading = _heading;
        }

        public int imageid { get; set; }
        public int survid { get; set; }
        public int trackid { get; set; }
        public int camid { get; set; }
        public string filename { get; set; }

        public string survfoldername { get; set; }
        public string trackfoldername { get; set; }
        public string camfoldername { get; set; }
        public string imagefilepath { get; set; }

        public double utm_x { get; set; }
        public double utm_y { get; set; }
        public double height { get; set; }

        public double roll { get; set; }
        public double pitch { get; set; }
        public double heading { get; set; }

    }
}
