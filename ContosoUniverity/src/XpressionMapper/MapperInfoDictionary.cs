using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using XpressionMapper.Structures;

namespace XpressionMapper
{
    public class MapperInfoDictionary : Dictionary<ParameterExpression, MapperInfo>
    {
        public MapperInfoDictionary(ParameterExpressionEqualityComparer comparer) : base(comparer)
        {
        }

        const string PREFIX = "p";

        public void Add(ParameterExpression key, Dictionary<Type, Type> typeMappings)
        {
            if (this.ContainsKey(key))
                return;

            this.Add(key, new MapperInfo(Expression.Parameter(typeMappings[key.Type], string.Concat(PREFIX, this.Count)), key.Type, typeMappings[key.Type]));
        }
    }
}
