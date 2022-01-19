// See https://aka.ms/new-console-template for more information
using Domain.Core;
using Repository.Data;
using Domain.Models;

Console.WriteLine("Hello, World!");
string connectionString = "Host=localhost;Port=5432;Database=testDB1;Username=postgres;Password=2077";

IUserRepository userRepository = new UserRepository(connectionString, new UserPasswordRepository(connectionString));
User user = new User { Surname = "Test", Name = "namename", VkAddress = "adress123", CityId = 0, IsDelete = false, PasswordId = new Guid()};
string password = "1234567sdfsdf";
Guid id = userRepository.Create(user, password);

var selected = userRepository.GetAll();
foreach(User u in selected)
    Console.WriteLine(u.Surname+" "+u.Name+" "+u.VkAddress+" "+u.Rating.ToString()+" "+u.PasswordId.ToString());

Console.ReadKey();
