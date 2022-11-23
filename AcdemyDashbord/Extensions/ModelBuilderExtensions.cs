using AcdemyDashbord.Models;
using Microsoft.EntityFrameworkCore;

namespace AcdemyDashbord.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity< Employee>().HasData(
                new Employee() { Id = 1, Name = "ali", Email = "ali@email.com", Departement = Departement.Math, ImageUrl = "1.jpg" },
                new Employee() { Id = 2, Name = "youcef", Email = "youcef@email.com", Departement = Departement.Programming, ImageUrl = "2.jpg" },
                new Employee() { Id = 3, Name = "mohamed", Email = "mohamed@email.com", Departement = Departement.IT, ImageUrl = "3.jpg" }
            );
        }
    }
}
