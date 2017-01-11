using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ContosoUniversity.Repositories;
using ContosoUniversity.Repositories.School;
using ContosoUniversity.AutoMapperProfiles;
using ContosoUniversity.Contexts;
using ContosoUniversity.Crud.DataStores;
using ContosoUniversity.Data.Enitities;
using ContosoUniversity.Domain.School;

namespace BusinessTestConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            IServiceProvider serviceProvider = new ServiceCollection().AddDbContext<SchoolContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient)
                .AddTransient<ISchoolStore, SchoolStore>()
                .AddTransient<IStudentRepository, StudentRepository>()
                .AddTransient<ISchoolRepository, SchoolRepository>()
                .AddSingleton<AutoMapper.IConfigurationProvider>(new MapperConfiguration(cfg => cfg.AddProfiles(typeof(UniversityProfile).GetTypeInfo().Assembly)))
                .AddTransient<IMapper>(sp => new Mapper(sp.GetRequiredService<AutoMapper.IConfigurationProvider>(), sp.GetService))
                .BuildServiceProvider();

            try
            {
                Task.Run(async () =>
                    {
                        await GetFromStudentRepository(serviceProvider.GetRequiredService<IStudentRepository>());
                        //await GetFromSchoolRepository(serviceProvider.GetRequiredService<ISchoolRepository>());
                        //ICollection<StudentModel> students = await store.GetItemsAsync<StudentModel, Student>(null, o => o.OrderBy(a => a.FullName), new List<Expression<Func<StudentModel, object>>>() { item => item.Enrollments });
                        Console.WriteLine("");

                    }).Wait();/**/
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private static async Task GetFromStudentRepository(IStudentRepository store)
        {
            IList<StudentModel> students = (await store.GetItemsAsync(null, o => o.OrderBy(a => a.FullName), new List<Expression<Func<IQueryable<StudentModel>, IIncludableQueryable<StudentModel, object>>>>() { item => item.Include(x => x.Enrollments) })).ToList();

            Console.WriteLine("");
        }

        private static async Task SaveStudentRepository(IStudentRepository store)
        {
            List<StudentModel> toSave = new List<StudentModel>
            {
                new StudentModel { EntityState = ContosoUniversity.Domain.EntityStateType.Added, FirstName = "Tom", LastName = "Spratt", EnrollmentDate = DateTime.Parse("2010-09-01"),
                    Enrollments = new HashSet<EnrollmentModel>
                    {
                        new EnrollmentModel
                        {
                            CourseID = 1045,
                            Grade = ContosoUniversity.Domain.School.Grade.B
                        }
                    } },
               new StudentModel { EntityState = ContosoUniversity.Domain.EntityStateType.Added, FirstName = "Billie",   LastName = "Spratt", EnrollmentDate = DateTime.Parse("2010-09-01"),
                    Enrollments = new HashSet<EnrollmentModel>
                    {
                        new EnrollmentModel
                        {
                            CourseID = 1050,
                            Grade = ContosoUniversity.Domain.School.Grade.B
                        }
                    }}
            };
            bool sucess = await store.SaveGraphsAsync(toSave);

            Console.WriteLine("");
        }

        /*private static async Task GetFromSchoolRepository(ISchoolRepository store)
        {
            ICollection<StudentModel> students = await store.GetItemsAsync<StudentModel, Student>(null, o => o.OrderBy(a => a.FullName), new List<Expression<Func<IQueryable<StudentModel>, IIncludableQueryable<StudentModel, object>>>>() { item => item.Include(s => s.Enrollments) });

            List<StudentModel> toSave = new List<StudentModel>
            {
                new StudentModel { EntityState = ContosoUniversity.Domain.EntityStateType.Added, FirstName = "Jack", LastName = "Spratt", EnrollmentDate = DateTime.Parse("2010-09-01"),
                    Enrollments = new HashSet<EnrollmentModel>
                    {
                        new EnrollmentModel
                        {
                            CourseID = 1045,
                            Grade = ContosoUniversity.Domain.School.Grade.B
                        }
                    } },
               new StudentModel { EntityState = ContosoUniversity.Domain.EntityStateType.Added, FirstName = "Jackie",   LastName = "Spratt", EnrollmentDate = DateTime.Parse("2010-09-01"),
                    Enrollments = new HashSet<EnrollmentModel>
                    {
                        new EnrollmentModel
                        {
                            CourseID = 1050,
                            Grade = ContosoUniversity.Domain.School.Grade.B
                        }
                    }}
            };

            store.AddGraphChanges<StudentModel, Student>(toSave);
            DepartmentModel[] departments = new DepartmentModel[]
            {
                new DepartmentModel
                {
                    EntityState = ContosoUniversity.Domain.EntityStateType.Added,
                    Name = "Arabic",     Budget = 350000,
                    StartDate = DateTime.Parse("2007-09-01"),
                   // Administrator = new InstructorModel { FirstName = "John", LastName = "Brown", HireDate = DateTime.Parse("1995-03-11")},
                    Courses =  new HashSet<CourseModel>
                    {
                        new CourseModel {CourseID = 7021, Title = "Arabic 1",    Credits = 3},
                        new CourseModel {CourseID = 7042, Title = "Arabic 2",     Credits = 4}
                    }
                }
            };

            store.AddGraphChanges<DepartmentModel, Department>(departments);

            bool success = await store.SaveChangesAsync();

            Console.WriteLine("");
        }*/

    }
}
