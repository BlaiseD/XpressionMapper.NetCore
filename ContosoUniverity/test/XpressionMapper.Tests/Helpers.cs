using System;

namespace XpressionMapper.Tests
{
    internal static class Helpers
    {
        //[DbFunction("Edm", "TruncateTime")]
        public static DateTime? TruncateTime(DateTime? date)
        {
            return date.HasValue ? date.Value.Date : (DateTime?)null;
        }
    }
}
