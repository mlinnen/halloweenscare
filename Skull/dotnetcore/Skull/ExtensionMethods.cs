using System;
using System.Collections.Generic;
using System.Text;

namespace Skull
{
    public static class ExtensionMethods
    {
        public static int Map(this int value, int fromSource, int toSource, int fromTarget, int toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }
    }
}
