using System.Linq.Expressions;
using System.Reflection;

namespace XpressionMapper.Structures
{
    public class PropertyMapInfo
    {
        public PropertyMapInfo(LambdaExpression CustomExpression, MemberInfo DestinationPropertyInfo)
        {
            this.CustomExpression = CustomExpression;
            this.DestinationPropertyInfo = DestinationPropertyInfo;
        }

        public Expression ResolveExpression { get; set; }
        public LambdaExpression CustomExpression { get; set; }
        public MemberInfo DestinationPropertyInfo { get; set; }
    }
}
