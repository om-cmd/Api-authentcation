using DomainLayer.Data;
using DomainLayer.DomainModel;

namespace DomainLayer.Datas
{
    public interface IUnitOfWork
    {
        public UserDbContext Context { get; }
        public IEnumerable<User> User { get; }

        public void Save();
        void SaveOrUpdateUserRefreshToken(User userRefreshToken);
        bool CheckIfRefreshTokenIsValid(string username, string refreshToken);
    }
}
