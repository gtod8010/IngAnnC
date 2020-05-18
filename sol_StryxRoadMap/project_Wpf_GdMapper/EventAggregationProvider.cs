using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace Wpf_GdMapper
{
    class EventAggregationProvider
    {
        public static EventAggregator EventAggregator { get; set; } = new EventAggregator();
    }
}
