using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace EmguMagicCamera.Main
{
    public enum FilterType : int
    {
        ftNormal = 0,
        ftGray = 1,
        ftSmallGray = 2,
        ftSmoothGray = 3,
        ftCanny = 4,
        ftWinter = 5,
        ftSummer = 6,
        ftSpring = 7,
        ftRainbow = 8,
        ftPink = 9,
        ftOcean = 10,
        ftJet = 11,
        ftHsv = 12,
        ftHot = 13,
        ftCool = 14,
        ftBone = 15,
        ftAutumn = 16,
        ftBitwiseNot = 17,
        ftEdgePreservingFilter = 18
    }

    public class FilterUtils
    {
        public static string[] GFilterStr =
        {
            "Normal", "Gray", "SmallGray", "SmoothGray", "Canny", "Winter", "Summer",
            "Spring", "Rainbow", "Pink", "Ocean", "Jet", "Hsv", "Hot", "Cool", "Bone", "Autumn"
            , "BitwiseNot","EdgePreservingFilter"
        };

        public static FilterType ConvertStrToFilterType(string sFilterName)
        {
            ArrayList arrPropNames = new ArrayList(GFilterStr);
            if (arrPropNames.Contains(sFilterName))
                return (FilterType)arrPropNames.IndexOf(sFilterName);
            else
                return FilterType.ftNormal;
        }
    }
}
