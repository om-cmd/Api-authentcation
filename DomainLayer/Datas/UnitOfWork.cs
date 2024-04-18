using DomainLayer.Data;
using DomainLayer.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Datas
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(UserDbContext userDbContext)
        {
            Context = userDbContext;

        }
        public UserDbContext Context { get; set; }

        public IEnumerable<User> User => Context.Users;

        public bool CheckIfRefreshTokenIsValid(string username, string refreshToken)
        {
            var user = Context.Users.FirstOrDefault(u => u.UserName == username);

            // Check if user exists and if the refresh token matches
            return user != null && user.RefreshToken == refreshToken;
        }

        public void Save()
        {
            Context.SaveChanges();
        }

        public void SaveOrUpdateUserRefreshToken(User userRefreshToken)
        {
            var existingUser = Context.Users.FirstOrDefault(u => u.UserName == userRefreshToken.UserName);

            if (existingUser != null)
            {
                // Update existing user's refresh token
                existingUser.RefreshToken = userRefreshToken.RefreshToken;
            }
            else
            {
                // If user doesn't exist, add new user with refresh token
                Context.Users.Add(userRefreshToken);
            }

            // Save changes to the database
            Context.SaveChanges();
        }
    }
}

   

      