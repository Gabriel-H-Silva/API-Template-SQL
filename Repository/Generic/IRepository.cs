using ManagerIO.Model;
using ManagerIO.Model.Base;

namespace ManagerIO.Repository
{
    public interface IRepository<T> where T : BaseEntity
    {
        T Create(T item);
        T FindById(long id);
        List<T> FindAll();
        T Update(T item);
        void Delete(long id);

    }
}
