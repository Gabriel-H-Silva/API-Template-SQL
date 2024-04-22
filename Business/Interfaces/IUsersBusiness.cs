using ManagerApi.Model;

namespace ManagerIO.Business.Interfaces
{
    public interface IUsersBusiness
    {
        Users Create(Users person);
        Users FindById(long id);
        List<Users> FindAll();
        Users Update(Users person);
        void Delete(long id);
    }
}
