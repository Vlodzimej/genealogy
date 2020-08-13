using Genealogy.Models;

namespace Genealogy.Repository.Abstract
{

    public interface IUserRepository : IGenericRepository<User>
    {
        /// <summary>
        /// Получить учетную запись по логину
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        User GetByUsername(string username);

        /// <summary>
        /// Проверить существование учетной записи по логину
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        bool CheckUsername(string username);

        /// <summary>
        /// Получить общее кол-во зарегистрированных пользователей
        /// </summary>
        /// <returns></returns>
        int GetUserAmount();
    }

}