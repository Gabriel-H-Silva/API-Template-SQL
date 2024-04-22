using ManagerApi.Model;
using ManagerIO.Business.Interfaces;
using ManagerIO.Repository;
using ManagerIO.Repository.Generic;

namespace ManagerIO.Business
{
    public class UsersBusiness : IUsersBusiness
    {
        UsersRepository repository;
        public Users Create(Users person)
        {
            throw new NotImplementedException();
        }

        public void Delete(long id)
        {

        }

        public List<Users> FindAll()
        {
            return repository.FindAll();
        }

        public Users FindById(long id)
        {
            throw new NotImplementedException();
        }

        public Users Update(Users person)
        {
            throw new NotImplementedException();
        }
    }
}
