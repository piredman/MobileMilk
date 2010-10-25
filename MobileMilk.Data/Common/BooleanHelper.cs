using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MobileMilk.Data.Common
{
    public static class BooleanHelper
    {
        public static bool AsBoolean(string value)
        {
            if (null == value)
                return false;

            bool boolean;
            if (!Boolean.TryParse(value, out boolean))
                boolean = false;

            return boolean;
        }
    }
}
