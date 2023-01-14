using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BroadVoicePOC.Common.Constants.Enums
{
    public enum HTTPLoggingCallType
    {
        [Description("log all internal call types")]
        Internal = 0,
        [Description("log all external (IN and OUT) call types")]
        External = 1,
        [Description("log both internal and external call types")]
        Both = 2
    }

    public enum HTTPCallType
    {
        [Description("calls initiated by the frontend")]
        Internal = 0,
        [Description("calls that 3rd parties make to our services")]
        ExternalIn = 1,
        [Description("calls that we make to 3rd parties")]
        ExternalOut = 2
    }
}
