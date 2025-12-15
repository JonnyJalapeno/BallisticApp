using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BallisticApp.SettingsWindow;

namespace BallisticApp
{
    public class ViewSettings
    {
        public TargetKind TargetType { get; set; }

        public enum TargetKind
        {
            Circular,
            Axes
        }
    }
}
