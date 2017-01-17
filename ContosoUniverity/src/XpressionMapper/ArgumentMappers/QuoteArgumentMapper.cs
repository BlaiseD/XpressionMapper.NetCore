using System.Linq.Expressions;
using XpressionMapper.Extensions;

namespace XpressionMapper.ArgumentMappers
{
    internal class QuoteArgumentMapper : ArgumentMapper
    {
        public QuoteArgumentMapper(XpressionMapperVisitor expressionVisitor, Expression argument)
            : base(expressionVisitor, argument)
        {
        }

        public override Expression MappedArgumentExpression
        {
            get
            {
                LambdaExpression lambdaExpression = (LambdaExpression)((UnaryExpression)this.argument).Operand;
                Expression ex = this.ExpressionVisitor.Visit(lambdaExpression.Body);

                LambdaExpression mapped = Expression.Lambda(ex, lambdaExpression.GetDestinationParameterExpressions(this.ExpressionVisitor.InfoDictionary, this.ExpressionVisitor.TypeMappings));
                return Expression.Quote(mapped);
            }
        }
    }
}
