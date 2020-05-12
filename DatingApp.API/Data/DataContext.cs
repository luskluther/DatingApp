using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext : DbContext
    {
        // Here we are injecting options for the DataContext class , telling the DataContext class about the entities.
        public DataContext(DbContextOptions<DataContext> options) : base(options){}

        public DbSet<Value> Values {get;set;}
    }
}