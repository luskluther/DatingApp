using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext : DbContext
    {
        // Here we are injecting options for the DataContext class , telling the DataContext class about the entities.
        public DataContext(DbContextOptions<DataContext> options) : base(options){}

        public DbSet<Value> Values { get;set; }
        public DbSet<User> Users { get;set; }
        public DbSet<Photo> Photos { get;set; } // create these and then create migrations
        public DbSet<Like> Likes { get;set; }

        // this is providing the delationship refinitions and table propertiers for the like table how we want to create it , this is FLUENT API
        protected override void OnModelCreating(ModelBuilder builder) { // we are overringing how the 'like' model is created by overriding default
            builder.Entity<Like>()
            .HasKey(k => new { k.LikerId, k.LikeeId }) ;// the primary key is not there for table so definiing it as combo aas likedd and likedid
            
            builder.Entity<Like>()
            .HasOne(u => u.Likee) // we can have one likee one time one person cannot like us multiple times
            .WithMany(u => u.Likers) // we can have many likers of different users
            .HasForeignKey(u => u.LikeeId) // user table foreigh key
            .OnDelete(DeleteBehavior.Restrict); // we do not want cascading delete , deleting like cannot delete the user.

            builder.Entity<Like>()
            .HasOne(u => u.Liker)
            .WithMany(u => u.Likees)
            .HasForeignKey(u => u.LikerId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}