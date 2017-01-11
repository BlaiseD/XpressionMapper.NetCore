using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoMapper;
using XpressionMapper.ArgumentMappers;
using XpressionMapper.Extensions;
using XpressionMapper.Structures;
//using static System.Linq.Expressions.Expression;

namespace XpressionMapper
{
    public class XpressionMapperVisitor : ExpressionVisitor
    {
        public XpressionMapperVisitor(IConfigurationProvider configurationProvider, Dictionary<Type, Type> typeMappings)
        {
            this.typeMappings = typeMappings;
            this.infoDictionary = new MapperInfoDictionary(new ParameterExpressionEqualityComparer());
            this.configurationProvider = configurationProvider;
        }

        #region Variables
        private MapperInfoDictionary infoDictionary;
        private Dictionary<Type, Type> typeMappings;
        private IConfigurationProvider configurationProvider;
        #endregion Variables

        #region Properties
        public MapperInfoDictionary InfoDictionary
        {
            get { return this.infoDictionary; }
        }

        public Dictionary<Type, Type> TypeMappings
        {
            get { return this.typeMappings; }
        }

        protected IConfigurationProvider ConfigurationProvider { get { return this.configurationProvider; } }
        #endregion Properties

        #region Methods
        protected override Expression VisitParameter(ParameterExpression parameterExpression)
        {
            infoDictionary.Add(parameterExpression, this.TypeMappings);
            KeyValuePair<ParameterExpression, MapperInfo> pair = infoDictionary.SingleOrDefault(a => a.Key.Equals(parameterExpression));
            if (!pair.Equals(default(KeyValuePair<Type, MapperInfo>)))
            {
                return pair.Value.NewParameter;
            }

            return base.VisitParameter(parameterExpression);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.NodeType == ExpressionType.Constant)
                return base.VisitMember(node);

            string sourcePath = null;

            ParameterExpression parameterExpression = node.GetParameterExpression();
            if (parameterExpression == null)
                return base.VisitMember(node);

            infoDictionary.Add(parameterExpression, this.TypeMappings);

            Type sType = parameterExpression.Type;
            if (infoDictionary.ContainsKey(parameterExpression) && node.IsMemberExpression())
            {
                sourcePath = node.GetPropertyFullName();
            }
            else
            {
                return base.VisitMember(node);
            }

            List<PropertyMapInfo> propertyMapInfoList = new List<PropertyMapInfo>();
            FindDestinationFullName(sType, infoDictionary[parameterExpression].DestType, sourcePath, propertyMapInfoList);
            string fullName = null;

            if (propertyMapInfoList[propertyMapInfoList.Count - 1].CustomExpression != null)
            {
                PropertyMapInfo last = propertyMapInfoList[propertyMapInfoList.Count - 1];
                propertyMapInfoList.Remove(last);

                //Get the fullname of the reference object - this means building the reference name from all but the last expression.
                fullName = BuildFullName(propertyMapInfoList);
                PrependParentNameVisitor visitor = new PrependParentNameVisitor(infoDictionary[parameterExpression].DestType, last.CustomExpression.Parameters[0].Type/*Parent type of current property*/, fullName, infoDictionary[parameterExpression].NewParameter);
                Expression ex = visitor.Visit(last.CustomExpression.Body);
                return ex;
            }
            else
            {
                fullName = BuildFullName(propertyMapInfoList);
                MemberExpression me = infoDictionary[parameterExpression].NewParameter.BuildExpression(infoDictionary[parameterExpression].DestType, fullName);
                return me;
            }
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var constantExpression = node.Right as ConstantExpression;
            if (constantExpression != null)
            {
                if (constantExpression.Value == null)
                    return base.VisitBinary(node.Update(node.Left, node.Conversion, Expression.Constant(null)));
            }

            constantExpression = node.Left as ConstantExpression;
            if (constantExpression != null)
            {
                if (constantExpression.Value == null)
                    return base.VisitBinary(node.Update(Expression.Constant(null), node.Conversion, node.Right));
            }

            Expression newLeft = this.Visit(node.Left);
            Expression newRight = this.Visit(node.Right);
            if ((newLeft.Type.GetTypeInfo().IsGenericType && newLeft.Type.GetGenericTypeDefinition().Equals(typeof(Nullable<>))) ^ (newRight.Type.GetTypeInfo().IsGenericType && newRight.Type.GetGenericTypeDefinition().Equals(typeof(Nullable<>))))
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resource.cannotCreateBinaryExpressionFormat, newLeft.ToString(), newLeft.Type.Name, newRight.ToString(), newRight.Type.Name));

            return base.VisitBinary(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            ParameterExpression parameterExpression = node.GetParameterExpression();
            if (parameterExpression == null)
                return base.VisitMethodCall(node);

            infoDictionary.Add(parameterExpression, this.TypeMappings);

            bool isExtension = node.Method.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), true);
            List<Expression> listOfArgumentsForNewMethod = node.Arguments.Aggregate(new List<Expression>(), (lst, next) =>
            {
                Expression mappedNext = ArgumentMapper.Create(this, next).MappedArgumentExpression;
                List<Type> sourceArguments = next.Type.GetTypeInfo().GetGenericArguments().ToList();
                List<Type> destArguments = mappedNext.Type.GetTypeInfo().GetGenericArguments().ToList();
                if (sourceArguments != null && sourceArguments.Count > 0)
                {
                    for (int i=0; i< sourceArguments.Count; i++)
                    {
                        if (typeof(Delegate).GetTypeInfo().IsAssignableFrom(sourceArguments[i]))
                        {
                            List<Type> sources = sourceArguments[i].GetTypeInfo().GetGenericArguments().ToList();
                            List<Type> dests= destArguments[i].GetTypeInfo().GetGenericArguments().ToList();
                            if (sources != null && sources.Count > 0)
                            {
                                for (int j = 0; j < sources.Count; j++)
                                    this.TypeMappings.AddTypeMapping(sources[j], dests[j]);
                            }
                        }
                        {
                            this.TypeMappings.AddTypeMapping(sourceArguments[i], destArguments[i]);
                        }
                    }
                }

                if (isExtension && lst.Count == 0)
                {
                    Type typeSource = next.Type.GetUnderlyingGenericType();
                    Type typeDest = mappedNext.Type.GetUnderlyingGenericType();

                    if (typeSource != null && typeDest != null)
                        this.TypeMappings.AddTypeMapping(typeSource, typeDest);
                }

                lst.Add(mappedNext);
                return lst;
            });//Arguments could be expressions or other objects. e.g. s => s.UserId  or a string "ZZZ".  For extention methods node.Arguments[0] is usually the helper object itself

            //type args are the generic type args e.g. T1 and T2 MethodName<T1, T2>(method arguments);
            List<Type> typeArgsForNewMethod = node.Method.IsGenericMethod
                ? node.Method.GetGenericArguments().Select(i => typeMappings.ContainsKey(i) ? typeMappings[i] : i).ToList()//not converting the type it is not in the typeMappings dictionary
                : null;

            MethodCallExpression resultExp = null;
            if (!node.Method.IsStatic)
            {
                Expression instance = ArgumentMapper.Create(this, node.Object).MappedArgumentExpression;

                resultExp = node.Method.IsGenericMethod
                    ? Expression.Call(instance, node.Method.Name, typeArgsForNewMethod.ToArray(), listOfArgumentsForNewMethod.ToArray())
                    : Expression.Call(instance, node.Method, listOfArgumentsForNewMethod.ToArray());
            }
            else
            {
                resultExp = node.Method.IsGenericMethod
                    ? Expression.Call(node.Method.DeclaringType, node.Method.Name, typeArgsForNewMethod.ToArray(), listOfArgumentsForNewMethod.ToArray())
                    : Expression.Call(node.Method, listOfArgumentsForNewMethod.ToArray());
            }

            return resultExp;
        }
        #endregion Methods

        #region Private Methods
        protected string BuildFullName(List<PropertyMapInfo> propertyMapInfoList)
        {
            string fullName = string.Empty;
            foreach (PropertyMapInfo info in propertyMapInfoList)
            {
                if (info.CustomExpression != null)
                {
                    fullName = string.IsNullOrEmpty(fullName)
                        ? info.CustomExpression.GetMemberFullName()
                        : string.Concat(fullName, ".", info.CustomExpression.GetMemberFullName());
                }
                else
                {
                    fullName = string.IsNullOrEmpty(fullName)
                        ? info.DestinationPropertyInfo.Name
                        : string.Concat(fullName, ".", info.DestinationPropertyInfo.Name);
                }
            }

            return fullName;
        }
        
        protected void FindDestinationFullName(Type typeSource, Type typeDestination, string sourceFullName, List<PropertyMapInfo> propertyMapInfoList)
        {
            const string PERIOD = ".";
            TypeMap typeMap = this.ConfigurationProvider.FindTypeMapFor(typeDestination, typeSource);//The destination becomes the source because to map a source expression to a destination expression,
            //we need the expressions used to create the source from the destination 

            if (sourceFullName.IndexOf(PERIOD) < 0)
            {
                PropertyMap propertyMap = typeMap.GetPropertyMaps().SingleOrDefault(item => item.DestinationProperty.Name == sourceFullName);
                if (propertyMap.ValueResolverConfig != null)
                {
                    #region Research
                    /*TypeMapPlanBuilder builder = new TypeMapPlanBuilder(this.ConfigurationProvider, null, typeMap);
            MethodCallExpression e = (MethodCallExpression)builder.CreatePropertyMapFunc(propertyMap);

            ParameterExpression pe = e.GetParameterExpression();*/
                    //propertyMap.SetCustomValueResolverExpression(builder.CreatePropertyMapFunc(propertyMap));
                    //System.Reflection.MethodInfo mi = propertyMap.ValueResolverConfig.Type.GetMethod("Resolve");
                    ////object o = Activator.CreateInstance(type);
                    ////mi.Invoke(o, null);
                    ////Expression.Call(node.Method.DeclaringType, node.Method.Name, typeArgsForNewMethod.ToArray(), listOfArgumentsForNewMethod.ToArray())
                    //bool canResolve = propertyMap.CanResolveValue();
                    //Type resolverType = propertyMap.ValueResolverConfig.Type;
                    //var iResolverType = resolverType.GetTypeInfo()
                    //        .ImplementedInterfaces.First(t => t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == typeof(IValueResolver<,,>));

                    //var sourceResolverParam = iResolverType.GetGenericArguments()[0];
                    //var destResolverParam = iResolverType.GetGenericArguments()[1];
                    //var destMemberResolverParam = iResolverType.GetGenericArguments()[2];


                    //Expression valueResolverFunc =
                    //    ToType(Expression.Call(ToType(ctor, resolverType), iResolverType.GetDeclaredMethod("Resolve"),
                    //        ToType(_source, sourceResolverParam),
                    //        ToType(_destination, destResolverParam),
                    //        ToType(destValueExpr, destMemberResolverParam),
                    //        _context),
                    //        destinationPropertyType); 
                    #endregion

                    throw new InvalidOperationException(Resource.customResolversNotSupported);
                }
                else if (propertyMap.CustomExpression != null)
                {
                    if (propertyMap.CustomExpression.ReturnType.GetTypeInfo().IsValueType && typeSource.GetTypeInfo().GetProperty(propertyMap.DestinationProperty.Name).PropertyType != propertyMap.CustomExpression.ReturnType)
                        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resource.expressionMapValueTypeMustMatchFormat, propertyMap.CustomExpression.ReturnType.Name, propertyMap.CustomExpression.ToString(), typeSource.GetTypeInfo().GetProperty(propertyMap.DestinationProperty.Name).PropertyType.Name, propertyMap.DestinationProperty.Name));
                }
                else
                {
                    if (((PropertyInfo)propertyMap.SourceMember).PropertyType.GetTypeInfo().IsValueType && typeSource.GetTypeInfo().GetProperty(propertyMap.DestinationProperty.Name).PropertyType != ((PropertyInfo)propertyMap.SourceMember).PropertyType)
                        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resource.expressionMapValueTypeMustMatchFormat, ((PropertyInfo)propertyMap.SourceMember).PropertyType.Name, propertyMap.SourceMember.Name, typeSource.GetTypeInfo().GetProperty(propertyMap.DestinationProperty.Name).PropertyType.Name, propertyMap.DestinationProperty.Name));
                }

                propertyMapInfoList.Add(new PropertyMapInfo(propertyMap.CustomExpression, propertyMap.SourceMember));
            }
            else
            {
                string propertyName = sourceFullName.Substring(0, sourceFullName.IndexOf(PERIOD));
                PropertyMap propertyMap = typeMap.GetPropertyMaps().SingleOrDefault(item => item.DestinationProperty.Name == propertyName);
                if (propertyMap.SourceMember == null)//If sourceFullName has a period then the SourceMember cannot be null.  The SourceMember is required to find the ProertyMap of its child object.
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resource.srcMemberCannotBeNullFormat, typeSource.Name, typeDestination.Name, propertyName));

                propertyMapInfoList.Add(new PropertyMapInfo(propertyMap.CustomExpression, propertyMap.SourceMember));
                string childFullName = sourceFullName.Substring(sourceFullName.IndexOf(PERIOD) + 1);
                FindDestinationFullName(typeSource.GetTypeInfo().GetProperty(propertyMap.DestinationProperty.Name).PropertyType, ((PropertyInfo)propertyMap.SourceMember).PropertyType, childFullName, propertyMapInfoList);
            }
        }
        #endregion Private Methods
    }
}