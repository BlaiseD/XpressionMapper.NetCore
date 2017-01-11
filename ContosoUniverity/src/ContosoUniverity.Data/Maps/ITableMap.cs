using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Data.Maps
{
    public interface ITableMap
    {
        void Map(ModelBuilder modelBuilder);
    }
}
