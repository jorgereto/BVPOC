using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BroadVoicePOC.Common.Helpers
{
    public class DateTimeHelper
    {
        public static bool TryParse(string str, out DateTime dt)
        {
            return DateTime.TryParse(str, null, System.Globalization.DateTimeStyles.RoundtripKind, out dt);
        }
    }
}
