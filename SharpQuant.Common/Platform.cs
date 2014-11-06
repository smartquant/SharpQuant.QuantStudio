using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpQuant.Common
{
    public static class Platform
    {
        private static bool? _IsUnix = null;
        private static PlatformID? _platformID = null;

        public static bool IsUnix
        {
            get
            {
                if (_IsUnix.HasValue) return _IsUnix.Value;

                PlatformID p = GetPlatformID;

                _IsUnix = ((p == PlatformID.Unix) || (p == PlatformID.MacOSX) ||
                    ((int)p == 128));

                return _IsUnix.Value;
            }
        }

        public static PlatformID GetPlatformID
        {
            get
            {
                if (_platformID.HasValue) return _platformID.Value;


                _platformID = Environment.OSVersion.Platform;

                return _platformID.Value;
            }

        }
    }
}
