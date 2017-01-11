using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using XpressionMapper.Extensions;
using Xunit;

namespace XpressionMapper.Tests
{
    public class MapperTests
    {
        public MapperTests()
        {
            SetupAutoMapper();
        }
        //a => a.Include(x => x.Enrollments).ThenInclude(x => x.Select(y => y.Course)))
        #region Tests
        [Fact]
        public void Map_Then_Inclues1()
        {

            //Arrange
            Expression<Func<IQueryable<AccountModel>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<AccountModel, ICollection<ThingModel>>>> selections = a => a.Include(x => x.ThingModels);

            //Act
            Expression<Func<IQueryable<Account>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Account, ICollection<Thing>>>> selectionMapped =
                mapper.MapExpression<Func<IQueryable<AccountModel>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<AccountModel, ICollection<ThingModel>>>,
                Func<IQueryable<Account>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Account, ICollection<Thing>>>>(selections);

            //Assert
            Assert.NotNull(selectionMapped);
        }
        [Fact]
        public void Map_Then_Inclues()
        {
            //Expression<Func<IQueryable<AccountModel>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<AccountModel, object>>> selections = a => a.Include(x => x.ThingModels);
            //Arrange
            //Expression<Func<IQueryable<AccountModel>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<AccountModel, object>>> selections = a => a.Include(x => x.ThingModels).ThenInclude(x => x.Select<ThingModel, object>(y => y.Color));
            Expression<Func<IQueryable<AccountModel>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<AccountModel, object>>> selections = a => a.Include(x => x.ThingModels).ThenInclude(y => y.Car);

            //Act
            Expression<Func<IQueryable<Account>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Account, object>>> selectionMapped =
                mapper.MapExpressionAsInclude<Func<IQueryable<AccountModel>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<AccountModel, object>>,
                Func<IQueryable<Account>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Account, object>>>(selections);

            //Assert
            Assert.NotNull(selectionMapped);
        }

        [Fact]
        public void Map_includes_list()
        {
            //Arrange
            ICollection<Expression<Func<UserModel, object>>> selections = new List<Expression<Func<UserModel, object>>>() { s => s.AccountModel.Bal, s => s.AccountName };

            //Act
            ICollection<Expression<Func<User, object>>> selectionsMapped = mapper.MapIncludesList<Func<UserModel, object>, Func<User, object>>(selections);

            //Assert
            Assert.NotNull(selectionsMapped);
        }

        [Fact]
        public void Map_includes_list_2()
        {
            //Arrange
            ICollection<Expression<Func<UserModel, object>>> selections = new List<Expression<Func<UserModel, object>>>() { s => s.AccountModel.Bal, s => s.AccountName, s => s.AccountModel.ThingModels.Select<ThingModel, object>(x => x.Color) };

            //Act
            ICollection<Expression<Func<User, object>>> selectionsMapped = mapper.MapIncludesList<Func<UserModel, object>, Func<User, object>>(selections);

            //Assert
            Assert.NotNull(selectionsMapped);
        }

        [Fact]
        public void Map_includes_trim_value_types()
        {
            //Arrange
            Expression<Func<UserModel, object>> selection = s => s.AccountModel.Bal;

            //Act
            Expression<Func<User, object>> selectionMapped = mapper.MapExpressionAsInclude<Func<UserModel, object>, Func<User, object>>(selection);

            //Assert
            Assert.NotNull(selectionMapped);
        }

        [Fact]
        public void Map_includes_trim_string()
        {
            //Arrange
            Expression<Func<UserModel, object>> selection = s => s.AccountName;

            //Act
            Expression<Func<User, object>> selectionMapped = mapper.MapExpressionAsInclude<Func<UserModel, object>, Func<User, object>>(selection);

            //Assert
            Assert.NotNull(selectionMapped);
        }

        [Fact]
        public void Map_includes_trim_string_nested()
        {
            //Arrange
            Expression<Func<UserModel, object>> selection = s => s.AccountModel.ThingModels.Select<ThingModel, object>(x => x.Color);

            //Act
            Expression<Func<User, object>> selectionMapped = mapper.MapExpressionAsInclude<Func<UserModel, object>, Func<User, object>>(selection);

            //Assert
            Assert.NotNull(selectionMapped);
        }

        [Fact]
        public void Map_object_type_change()
        {
            //Arrange
            Expression<Func<UserModel, bool>> selection = s => s.LoggedOn == "Y";

            //Act
            Expression<Func<User, bool>> selectionMapped = mapper.MapExpression<Func<UserModel, bool>, Func<User, bool>>(selection);

            //Assert
            Assert.NotNull(selectionMapped);
        }

        [Fact]
        public void Map_object_type_change_2()
        {
            //Arrange
            Expression<Func<UserModel, bool>> selection = s => s.IsOverEighty;

            //Act
            Expression<Func<User, bool>> selectionMapped = mapper.MapExpression<Func<UserModel, bool>, Func<User, bool>>(selection);

            //Assert
            Assert.NotNull(selectionMapped);
        }

        [Fact]
        public void Map__object_including_child_and_grandchild()
        {
            //Arrange
            Expression<Func<UserModel, bool>> selection = s => s != null && s.AccountModel != null && s.AccountModel.Bal == 555.20;

            //Act
            Expression<Func<User, bool>> selectionMapped = mapper.MapExpression<Func<UserModel, bool>, Func<User, bool>>(selection);

            //Assert
            Assert.NotNull(selectionMapped);
        }

        [Fact]
        public void Map_project_truncated_time()
        {
            //Arrange
            Expression<Func<UserModel, bool>> selection = s => s != null && s.AccountModel != null && s.AccountModel.DateCreated == DateTime.Now;

            //Act
            Expression<Func<User, bool>> selectionMapped = mapper.MapExpression<Func<UserModel, bool>, Func<User, bool>>(selection);

            //Assert
            Assert.NotNull(selectionMapped);
        }

        [Fact]
        public void Map_projection()
        {
            //Arrange
            //Expression<Func<UserModel, bool>> selection = s => s != null && string.Concat("ZZZ", s.FullName, " ", "ZZZ").StartsWith("A");
            Expression<Func<UserModel, bool>> selection = s => s != null && s.AccountModel.ComboName.StartsWith("A");

            //Act
            Expression<Func<User, bool>> selectionMapped = mapper.MapExpression<Func<UserModel, bool>, Func<User, bool>>(selection);

            //Assert
            Assert.NotNull(selectionMapped);
        }

        [Fact]
        public void Map__flattened_property()
        {
            //Arrange
            int age = 21;
            Expression<Func<UserModel, bool>> selection = s => ((s != null ? s.AccountName : null) ?? "").ToLower().StartsWith("A".ToLower()) && ((s.AgeInYears == age) && s.IsActive);

            //Act
            Expression<Func<User, bool>> selectionMapped = mapper.MapExpression<Func<UserModel, bool>, Func<User, bool>>(selection);

            //Assert
            Assert.NotNull(selectionMapped);
        }

        [Fact]
        public void Map__flattened_property_2()
        {
            //Arrange
            Expression<Func<UserModel, object>> selection = s => s.AccountModel.Id;

            //Act
            Expression<Func<User, object>> selectionMapped = mapper.MapExpression<Func<UserModel, object>, Func<User, object>>(selection);

            //Assert
            Assert.NotNull(selectionMapped);
        }

        [Fact]
        public void Map__select_method()
        {
            //Arrange
            Expression<Func<UserModel, object>> selection = s => s.AccountModel.ThingModels.Select(x => x.BarModel);

            //Act
            Expression<Func<User, object>> selectionMapped = mapper.MapExpression<Func<UserModel, object>, Func<User, object>>(selection);

            //Assert
            Assert.NotNull(selectionMapped);
        }

        [Fact]
        public void Map__select_method_2()
        {
            //Arrange
            Expression<Func<UserModel, object>> selection = s => s.AccountModel.ThingModels.Select(x => new { MM = x.BarModel });

            //Act
            Expression<Func<User, object>> selectionMapped = mapper.MapExpression<Func<UserModel, object>, Func<User, object>>(selection);

            //Assert
            Assert.NotNull(selectionMapped);
        }

        [Fact]
        public void Map__select_method_3()
        {
            //Arrange
            Expression<Func<UserModel, object>> selection = s => s.AccountModel.UserModels.Select(x => x.AgeInYears);

            //Act
            Expression<Func<User, object>> selectionMapped = mapper.MapExpression<Func<UserModel, object>, Func<User, object>>(selection);

            //Assert
            Assert.NotNull(selectionMapped);
        }

        [Fact]
        public void Map_where_method()
        {
            //Arrange
            Expression<Func<UserModel, object>> selection = s => s.AccountModel.ThingModels.Where(x => x.BarModel == s.AccountName);

            //Act
            Expression<Func<User, object>> selectionMapped = mapper.MapExpression<Func<UserModel, object>, Func<User, object>>(selection);

            //Assert
            Assert.NotNull(selectionMapped);
        }

        [Fact]
        public void Map_where_method2()
        {
            //Arrange
            Expression<Func<UserModel, AccountModel, object>> selection = (u, s) => u.FullName == s.Bal.ToString();

            //Act
            Expression<Func<User, Account, object>> selectionMapped = mapper.MapExpression<Func<UserModel, AccountModel, object>, Func<User, Account, object>>(selection);

            //Assert
            Assert.NotNull(selectionMapped);
        }



        [Fact]
        public void Map_orderBy_thenBy_expression()
        {
            //Arrange
            Expression<Func<IQueryable<UserModel>, IQueryable<UserModel>>> exp = q => q.OrderBy(s => s.Id).ThenBy(s => s.FullName);

            //Act
            Expression<Func<IQueryable<User>, IQueryable<User>>> expMapped = mapper.MapExpression<Func<IQueryable<UserModel>, IQueryable<UserModel>>, Func<IQueryable<User>, IQueryable<User>>>(exp);

            //Assert
            Assert.NotNull(expMapped);
        }

        [Fact]
        public void Map_orderBy_thenBy_GroupBy_expression()
        {
            //Arrange
            Expression<Func<IQueryable<UserModel>, IQueryable<IGrouping<int, UserModel>>>> grouped = q => q.OrderBy(s => s.Id).ThenBy(s => s.FullName).GroupBy(s => s.AgeInYears);

            //Act
            Expression<Func<IQueryable<User>, IQueryable<IGrouping<int, User>>>> expMapped = mapper.MapExpression<Func<IQueryable<UserModel>, IQueryable<IGrouping<int, UserModel>>>, Func<IQueryable<User>, IQueryable<IGrouping<int, User>>>>(grouped);

            //Assert
            Assert.NotNull(expMapped);
        }

        [Fact]
        public void Map_dynamic_return_type()
        {
            //Arrange
            Expression<Func<IQueryable<UserModel>, dynamic>> exp = q => q.OrderBy(s => s.Id).ThenBy(s => s.FullName);

            //Act
            Expression<Func<IQueryable<User>, dynamic>> expMapped = mapper.MapExpression<Func<IQueryable<UserModel>, dynamic>, Func<IQueryable<User>, dynamic>>(exp);

            //Assert
            Assert.NotNull(expMapped);
        }

        [Fact]
        public void Test_model_to_view_model()
        {
            //Arrange
            UserM userM = new UserM { Active = true, Age = 25, IsLoggedOn = false, Name = "Jack", UserId = 7 };

            //Act
            UserVM userVM = mapper.Map<UserVM>(userM);

            //Assert
            Assert.True(userVM.Age == userM.Age);
        }

        [Fact]
        public void Test_view_model_to_model()
        {
            //Arrange
            UserVM userVM = new UserVM { Active = true, Age = 25, IsLoggedOn = false, Name = "Jack" };
            UserM userM = new UserM { Active = true, Age = 21, IsLoggedOn = false, Name = "Jack", UserId = 7 };

            //Act
            UserM userB = mapper.Map<UserVM, UserM>(userVM, userM);

            //Assert
            Assert.True(userB.Age == userVM.Age);
        }

        [Fact]
        public void Map_Item()
        {
            //Arrange
            Expression<Func<Item, object>> exp = x => x.Name;

            //Act
            Expression<Func<ItemDto, object>> expMapped = mapper.MapExpression<Func<Item, object>, Func<ItemDto, object>>(exp);

            //Assert
            Assert.NotNull(expMapped);
        }
        #endregion Tests

        private static void SetupAutoMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfiles(typeof(OrganizationProfile));
            });

            mapper = config.CreateMapper();
        }

        static IMapper mapper;
    }

    public class Account
    {
        public Account()
        {
            Things = new List<Thing>();
        }
        public int Id { get; set; }
        public double Balance { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreateDate { get; set; }
        public Location Location { get; set; }
        public Branch Branch { get; set; }
        public ICollection<Thing> Things { get; set; }
        public ICollection<User> Users { get; set; }
    }

    public class AccountModel
    {
        public AccountModel()
        {
            ThingModels = new List<ThingModel>();
        }
        public int Id { get; set; }
        public double Bal { get; set; }
        public string ComboName { get; set; }
        public string FirstName { get; set; }
        public DateTime DateCreated { get; set; }
        public ICollection<ThingModel> ThingModels { get; set; }
        public ICollection<UserModel> UserModels { get; set; }
    }

    public class Thing
    {
        public int Foo { get; set; }
        public string Bar { get; set; }
        public Car Car { get; set; }
    }

    public class ThingModel
    {
        public int FooModel { get; set; }
        public string BarModel { get; set; }
        public string Color { get; set; }
        public CarModel Car { get; set; }
    }

    public class Car
    {
        public string Color { get; set; }
        public int Year { get; set; }
    }

    public class CarModel
    {
        public string Color { get; set; }
        public int Year { get; set; }
    }

    public class Location
    {
        public string City { get; set; }
        public int Year { get; set; }
    }

    public class Branch
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    //public class CarModel
    //{
    //    public string Color { get; set; }
    //    public int Year { get; set; }
    //}
    public class UserVM
    {
        public string Name { get; set; }
        public bool IsLoggedOn { get; set; }
        public int Age { get; set; }
        public bool Active { get; set; }
        public Account Account { get; set; }
    }
    public class UserM
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public bool IsLoggedOn { get; set; }
        public int Age { get; set; }
        public bool Active { get; set; }
        public Account Account { get; set; }
    }

    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public bool IsLoggedOn { get; set; }
        public int Age { get; set; }
        public bool Active { get; set; }
        public Account Account { get; set; }
    }

    public class UserModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string AccountName { get; set; }
        public bool IsOverEighty { get; set; }
        public string LoggedOn { get; set; }
        public int AgeInYears { get; set; }
        public bool IsActive { get; set; }
        public AccountModel AccountModel { get; set; }
    }

    public class ItemDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime CreateDate { get; set; }
    }


    public class Item
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }
    }

    public class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            CreateMap<User, UserModel>()
                    .ForMember(d => d.Id, opt => opt.MapFrom(s => s.UserId))
                    .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.Account.FirstName))
                    .ForMember(d => d.LoggedOn, opt => opt.MapFrom(s => s.IsLoggedOn ? "Y" : "N"))
                    .ForMember(d => d.IsOverEighty, opt => opt.MapFrom(s => s.Age > 80))
                    .ForMember(d => d.AccountName, opt => opt.MapFrom(s => s.Account == null ? string.Empty : string.Concat(s.Account.Branch.Name, " ", s.Account.Branch.Id.ToString())))
                    .ForMember(d => d.AgeInYears, opt => opt.MapFrom(s => s.Age))
                    .ForMember(d => d.IsActive, opt => opt.MapFrom(s => s.Active))
                    .ForMember(d => d.AccountModel, opt => opt.MapFrom(s => s.Account));

            CreateMap<UserModel, User>()
                .ForMember(d => d.UserId, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.FullName))
                .ForMember(d => d.IsLoggedOn, opt => opt.MapFrom(s => s.LoggedOn.ToUpper() == "Y"))
                .ForMember(d => d.Age, opt => opt.MapFrom(s => s.AgeInYears))
                .ForMember(d => d.Active, opt => opt.MapFrom(s => s.IsActive))
                .ForMember(d => d.Account, opt => opt.MapFrom(s => s.AccountModel));

            CreateMap<Account, AccountModel>()
                .ForMember(d => d.Bal, opt => opt.MapFrom(s => s.Balance))
                .ForMember(d => d.DateCreated, opt => opt.MapFrom(s => Helpers.TruncateTime(s.CreateDate).Value))
                .ForMember(d => d.ComboName, opt => opt.MapFrom(s => string.Concat(s.FirstName, " ", s.LastName)))
                //.ForMember(d => d.ComboName, opt => opt.ResolveUsing<CustomResolver>())
                .ForMember(d => d.ThingModels, opt => opt.MapFrom(s => s.Things))
                .ForMember(d => d.UserModels, opt => opt.MapFrom(s => s.Users));

            CreateMap<AccountModel, Account>()
                .ForMember(d => d.Balance, opt => opt.MapFrom(s => s.Bal))
                .ForMember(d => d.Things, opt => opt.MapFrom(s => s.ThingModels))
                .ForMember(d => d.Users, opt => opt.MapFrom(s => s.UserModels));

            CreateMap<Thing, ThingModel>()
                .ForMember(d => d.FooModel, opt => opt.MapFrom(s => s.Foo))
                .ForMember(d => d.BarModel, opt => opt.MapFrom(s => s.Bar))
                .ForMember(d => d.Color, opt => opt.MapFrom(s => s.Car.Color));

            CreateMap<ThingModel, Thing>()
                .ForMember(d => d.Foo, opt => opt.MapFrom(s => s.FooModel))
                .ForMember(d => d.Bar, opt => opt.MapFrom(s => s.BarModel));

            CreateMap<ItemDto, Item>()
                    .ForMember(dest => dest.Date, opts => opts.MapFrom(x => x.CreateDate));

            CreateMap<Item, ItemDto>()
                .ForMember(dest => dest.CreateDate, opts => opts.MapFrom(x => x.Date));

            CreateMissingTypeMaps = true;
        }
        //protected override void Configure()
        //{
        //    CreateMap<Foo, FooDto>();
        //    // Use CreateMap... Etc.. here (Profile methods are the same as configuration methods)
        //}
    }
}
