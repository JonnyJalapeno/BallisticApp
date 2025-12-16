using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallisticApp
{
    using System.Collections.Generic;

    public class DragTablePoint
    {
        public double Mach { get; set; }
        public double CD { get; set; }

        public DragTablePoint(double mach, double cd)
        {
            Mach = mach;
            CD = cd;
        }
    }

    public static class DragTables
    {
        

        public static List<DragTablePoint> TableG1 = new List<DragTablePoint>
        {
            new DragTablePoint(0.00, 0.2629),
            new DragTablePoint(0.05, 0.2558),
            new DragTablePoint(0.10, 0.2487),
            new DragTablePoint(0.15, 0.2413),
            new DragTablePoint(0.20, 0.2344),
            new DragTablePoint(0.25, 0.2278),
            new DragTablePoint(0.30, 0.2214),
            new DragTablePoint(0.35, 0.2155),
            new DragTablePoint(0.40, 0.2104),
            new DragTablePoint(0.45, 0.2061),
            new DragTablePoint(0.50, 0.2032),
            new DragTablePoint(0.55, 0.2020),
            new DragTablePoint(0.60, 0.2034),
            new DragTablePoint(0.70, 0.2165),
            new DragTablePoint(0.725, 0.2230),
            new DragTablePoint(0.75, 0.2313),
            new DragTablePoint(0.775, 0.2417),
            new DragTablePoint(0.80, 0.2546),
            new DragTablePoint(0.825, 0.2706),
            new DragTablePoint(0.85, 0.2901),
            new DragTablePoint(0.875, 0.3136),
            new DragTablePoint(0.90, 0.3415),
            new DragTablePoint(0.925, 0.3734),
            new DragTablePoint(0.95, 0.4084),
            new DragTablePoint(0.975, 0.4448),
            new DragTablePoint(1.00, 0.4805),
            new DragTablePoint(1.025, 0.5136),
            new DragTablePoint(1.05, 0.5427),
            new DragTablePoint(1.075, 0.5677),
            new DragTablePoint(1.10, 0.5883),
            new DragTablePoint(1.125, 0.6053),
            new DragTablePoint(1.15, 0.6191),
            new DragTablePoint(1.20, 0.6393),
            new DragTablePoint(1.25, 0.6518),
            new DragTablePoint(1.30, 0.6589),
            new DragTablePoint(1.35, 0.6621),
            new DragTablePoint(1.40, 0.6625),
            new DragTablePoint(1.45, 0.6607),
            new DragTablePoint(1.50, 0.6573),
            new DragTablePoint(1.55, 0.6528),
            new DragTablePoint(1.60, 0.6474),
            new DragTablePoint(1.65, 0.6413),
            new DragTablePoint(1.70, 0.6347),
            new DragTablePoint(1.75, 0.6280),
            new DragTablePoint(1.80, 0.6210),
            new DragTablePoint(1.85, 0.6141),
            new DragTablePoint(1.90, 0.6072),
            new DragTablePoint(1.95, 0.6003),
            new DragTablePoint(2.00, 0.5934),
            new DragTablePoint(2.05, 0.5867),
            new DragTablePoint(2.10, 0.5804),
            new DragTablePoint(2.15, 0.5743),
            new DragTablePoint(2.20, 0.5685),
            new DragTablePoint(2.25, 0.5630),
            new DragTablePoint(2.30, 0.5577),
            new DragTablePoint(2.35, 0.5527),
            new DragTablePoint(2.40, 0.5481),
            new DragTablePoint(2.45, 0.5438),
            new DragTablePoint(2.50, 0.5397),
            new DragTablePoint(2.60, 0.5325),
            new DragTablePoint(2.70, 0.5264),
            new DragTablePoint(2.80, 0.5211),
            new DragTablePoint(2.90, 0.5168),
            new DragTablePoint(3.00, 0.5133),
            new DragTablePoint(3.10, 0.5105),
            new DragTablePoint(3.20, 0.5084),
            new DragTablePoint(3.30, 0.5067),
            new DragTablePoint(3.40, 0.5054),
            new DragTablePoint(3.50, 0.5040),
            new DragTablePoint(3.60, 0.5030),
            new DragTablePoint(3.70, 0.5022),
            new DragTablePoint(3.80, 0.5016),
            new DragTablePoint(3.90, 0.5010),
            new DragTablePoint(4.00, 0.5006),
            new DragTablePoint(4.20, 0.4998),
            new DragTablePoint(4.40, 0.4995),
            new DragTablePoint(4.60, 0.4992),
            new DragTablePoint(4.80, 0.4990),
            new DragTablePoint(5.00, 0.4988),
        };

            public static List<DragTablePoint> TableG7 = new List<DragTablePoint>
        {
            new DragTablePoint(0.00, 0.1198),
            new DragTablePoint(0.05, 0.1197),
            new DragTablePoint(0.10, 0.1196),
            new DragTablePoint(0.15, 0.1194),
            new DragTablePoint(0.20, 0.1193),
            new DragTablePoint(0.25, 0.1194),
            new DragTablePoint(0.30, 0.1194),
            new DragTablePoint(0.35, 0.1194),
            new DragTablePoint(0.40, 0.1193),
            new DragTablePoint(0.45, 0.1193),
            new DragTablePoint(0.50, 0.1194),
            new DragTablePoint(0.55, 0.1193),
            new DragTablePoint(0.60, 0.1194),
            new DragTablePoint(0.65, 0.1197),
            new DragTablePoint(0.70, 0.1202),
            new DragTablePoint(0.725, 0.1207),
            new DragTablePoint(0.75, 0.1215),
            new DragTablePoint(0.775, 0.1226),
            new DragTablePoint(0.80, 0.1242),
            new DragTablePoint(0.825, 0.1266),
            new DragTablePoint(0.85, 0.1306),
            new DragTablePoint(0.875, 0.1368),
            new DragTablePoint(0.90, 0.1464),
            new DragTablePoint(0.925, 0.1660),
            new DragTablePoint(0.95, 0.2054),
            new DragTablePoint(0.975, 0.2993),
            new DragTablePoint(1.0, 0.3803),
            new DragTablePoint(1.025, 0.4015),
            new DragTablePoint(1.05, 0.4043),
            new DragTablePoint(1.075, 0.4034),
            new DragTablePoint(1.10, 0.4014),
            new DragTablePoint(1.125, 0.3987),
            new DragTablePoint(1.15, 0.3955),
            new DragTablePoint(1.20, 0.3884),
            new DragTablePoint(1.25, 0.3810),
            new DragTablePoint(1.30, 0.3732),
            new DragTablePoint(1.35, 0.3657),
            new DragTablePoint(1.40, 0.3580),
            new DragTablePoint(1.50, 0.3440),
            new DragTablePoint(1.55, 0.3376),
            new DragTablePoint(1.60, 0.3315),
            new DragTablePoint(1.65, 0.3260),
            new DragTablePoint(1.70, 0.3209),
            new DragTablePoint(1.75, 0.3160),
            new DragTablePoint(1.80, 0.3117),
            new DragTablePoint(1.85, 0.3078),
            new DragTablePoint(1.90, 0.3042),
            new DragTablePoint(1.95, 0.3010),
            new DragTablePoint(2.00, 0.2980),
            new DragTablePoint(2.05, 0.2951),
            new DragTablePoint(2.10, 0.2922),
            new DragTablePoint(2.15, 0.2892),
            new DragTablePoint(2.20, 0.2864),
            new DragTablePoint(2.25, 0.2835),
            new DragTablePoint(2.30, 0.2807),
            new DragTablePoint(2.35, 0.2779),
            new DragTablePoint(2.40, 0.2752),
            new DragTablePoint(2.45, 0.2725),
            new DragTablePoint(2.50, 0.2697),
            new DragTablePoint(2.55, 0.2670),
            new DragTablePoint(2.60, 0.2643),
            new DragTablePoint(2.65, 0.2615),
            new DragTablePoint(2.70, 0.2588),
            new DragTablePoint(2.75, 0.2561),
            new DragTablePoint(2.80, 0.2533),
            new DragTablePoint(2.85, 0.2506),
            new DragTablePoint(2.90, 0.2479),
            new DragTablePoint(2.95, 0.2451),
            new DragTablePoint(3.00, 0.2424),
            new DragTablePoint(3.10, 0.2368),
            new DragTablePoint(3.20, 0.2313),
            new DragTablePoint(3.30, 0.2258),
            new DragTablePoint(3.40, 0.2205),
            new DragTablePoint(3.50, 0.2154),
            new DragTablePoint(3.60, 0.2106),
            new DragTablePoint(3.70, 0.2060),
            new DragTablePoint(3.80, 0.2017),
            new DragTablePoint(3.90, 0.1975),
            new DragTablePoint(4.00, 0.1935),
            new DragTablePoint(4.20, 0.1861),
            new DragTablePoint(4.40, 0.1793),
            new DragTablePoint(4.60, 0.1730),
            new DragTablePoint(4.80, 0.1672),
            new DragTablePoint(5.00, 0.1618),
        };

        public static readonly Dictionary<string, List<DragTablePoint>> Tables = new Dictionary<string, List<DragTablePoint>>
        {
            { "TableG1", TableG1 },
            { "TableG7", TableG7 }
        };
    }

}
