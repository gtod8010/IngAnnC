using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wpf_IMAnn.Utils;

namespace Wpf_IMAnn.Models
{
    public class AnnoShapeModel : IComparable<AnnoShapeModel>
    {
        public AnnoShapeModel(ShapeType _shapetype)
        {
            shapetype = _shapetype;
            signfield = SignField.미할당;
        }

        public int shapeid { get; set; }
        public int imageid { get; set; }
        public int uniqueShapeid { get; set; }

        public ShapeType shapetype { get; set; }

        public object signfield { get; set; }

        public string memo { get; set; }

        public List<double> row { get; set; } = new List<double>();
        public List<double> col { get; set; } = new List<double>();

        public double width { get; set; }
        public double height { get; set; }

        public string signImageSource { get; set; }
        public Shape Shape { get; set; }
        public int CompareTo(AnnoShapeModel other) { return shapeid.CompareTo(other.shapeid); }

        public AnnoShapeModel ShallowCopy()
        {
            return (AnnoShapeModel) this.MemberwiseClone();
        }
    }
}
