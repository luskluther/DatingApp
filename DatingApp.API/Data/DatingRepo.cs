using System;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepo : IDatingRepo
    {
        private readonly DataContext _context;
        public DatingRepo(DataContext context)
        {
            this._context = context;

        }
        public void Add<T>(T entity) where T : class // not async because when we add , we are not querying so wont take time as it will be temporarily
            /// stored in memory no need to wait for anything
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
             _context.Remove(entity);
        }

        public async Task<Photo> GetMainPhoto(int userId)
        {
            return await _context.Photos.Where(u => u.UserId == userId).
            FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
           return photo;
        }

        public async Task<User> GetUser(int id)
        {
            // including the photos table or photo of that user too
           var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);
           return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users =  _context.Users.Include(p => p.Photos).OrderByDescending(u => u.LastActive).AsQueryable(); // no need to user await here becaues we are using in next step

            users = users.Where(u => u.Id != userParams.UserId); // removing logged in user
            users = users.Where(u => u.Gender == userParams.Gender); // removing other gender users

            if (userParams.MinAge != 18 || userParams.MaxAge != 99) { // not default values we have to send custom values
                var minDateOfBirth = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxDateOfBirth = DateTime.Today.AddYears(-userParams.MinAge);

                users = users.Where(u => u.DateOfBirth >= minDateOfBirth && u.DateOfBirth <= maxDateOfBirth);
            }

            if(!string.IsNullOrEmpty(userParams.OrderBy)) {
                switch(userParams.OrderBy) {
                    case "created":
                        users = users.OrderByDescending(u => u.Created); break;
                    default:
                        users = users.OrderByDescending(u => u.LastActive); break;
                }
            }
            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0; // if this returns more than 0 it means save is done if its = 0 , nothing saved so false
        }
    }
}