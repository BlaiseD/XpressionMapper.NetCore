# XpressionMapper.NetCore
<html><head><meta charset="utf-8"><style></style></head><body>
<h2 id="what-is-xpressionmapper-">What is XpressionMapper?</h2>
<p>XpressionMapper.NetCore leverages AutoMapper version 5.2 and .Net Standard Library version 1.6.1 to transform business model expressions into data model expressions.</p>

<p>Expression mapping provides a couple of advantages:
<ol class="list">
            <li>Improved separation of concerns:  The Entity Framework (or other ORM) layer has no knowledge of the business model.</li>
            <li>Removes the need for projection in the data layer or the need to return an IQueryable object from the data layer.</li>
        </ol>
</p>
<h2 id="how-it-works-">How it works?</h2>
<p>The service layer references business model classes only and has no knowledge of the data model (POCOs) or the EF (Entity Framework) layer</p>
<pre><code>public class PersonService : IPersonService
{
    private IPersonRepository repository;

    public ICollection&lt;PersonModel&gt; GetList(Expression&lt;Func&lt;PersonModel, bool&gt;&gt; filter = null, Expression&lt;Func&lt;IQueryable&lt;PersonModel&gt;, IQueryable&lt;PersonModel&gt;&gt;&gt; orderBy = null, ICollection&lt;Expression&lt;Func&lt;IQueryable&lt;PersonModel&gt;, IIncludableQueryable&lt;PersonModel, object&gt;&gt;&gt;&gt; includeProperties = null)
    {
        ICollection&lt;PersonModel&gt; list = repository.GetList(filter, orderBy, includeProperties);
        return list.ToList();
    }
}
</code></pre><p>The repository layer references the business model and data model (POCOs) classes and has no knowledge of the EF layer.
    public class PersonRepository : IPersonRepository
    {
        private IPersonStore store;</p>
<pre><code>    public ICollection&lt;PersonModel&gt; GetList(Expression&lt;Func&lt;PersonModel, bool&gt;&gt; filter = null, Expression&lt;Func&lt;IQueryable&lt;PersonModel&gt;, IQueryable&lt;PersonModel&gt;&gt;&gt; orderBy = null, ICollection&lt;Expression&lt;Func&lt;IQueryable&lt;PersonModel&gt;, IIncludableQueryable&lt;PersonModel, object&gt;&gt;&gt;&gt; includeProperties = null)
    {
        Expression&lt;Func&lt;Person, bool&gt;&gt; f = filter.MapExpression&lt;Func&lt;PersonModel, bool&gt;, Func&lt;Person, bool&gt;&gt;();
        Expression&lt;Func&lt;IQueryable&lt;Person&gt;, IQueryable&lt;Person&gt;&gt;&gt; mappedOrderBy = orderBy.MapExpression&lt;Func&lt;IQueryable&lt;PersonModel&gt;, IQueryable&lt;PersonModel&gt;&gt;, Func&lt;IQueryable&lt;Person&gt;, IQueryable&lt;Person&gt;&gt;&gt;();
        ICollection&lt;Expression&lt;Func&lt;IQueryable&lt;Person&gt;, IIncludableQueryable&lt;Person, object&gt;&gt;&gt;&gt; includes = mapper.MapIncludesList&lt;Func&lt;IQueryable&lt;PersonModel&gt;, IIncludableQueryable&lt;PersonModel, object&gt;&gt;, Func&lt;IQueryable&lt;Person&gt;, IIncludableQueryable&lt;Person, object&gt;&gt;&gt;(includeProperties);

        ICollection&lt;Person&gt; list = store.Get(f,
                mappedOrderBy == null ? null : mappedOrderBy.Compile(),
                includes == null ? null : includes.Select(i => i.Compile()).ToList());
        return Mapper.Map&lt;IEnumerable&lt;Person&gt;, IEnumerable&lt;PersonModel&gt;&gt;(list).ToList();
    }
}
</code></pre><p>The EF layer references the data model (POCOs) and has no knowledge of the business model.</p>
<pre><code>public class PersonStore : IPersonStore
{
    public IList&lt;Person&gt; Get(Expression&lt;Func&lt;Person, bool&gt;&gt; filter = null, Func&lt;IQueryable&lt;Person&gt;, IQueryable&lt;Person&gt;&gt; orderBy = null, ICollection&lt;Func&lt;IQueryable&lt;Person&gt;, IIncludableQueryable&lt;Person, object&gt;&gt;&gt; includeProperties = null)
    {
        IList&lt;Person&gt; list = null;
        using (IPersonUnitOfWork unitOfWork = new PersonUnitOfWork())
        {
            list = new PersonDbMapper(unitOfWork).Get(filter, orderBy, includeProperties);
        }

        return list;
    }
}
</code></pre>
</body></html>
