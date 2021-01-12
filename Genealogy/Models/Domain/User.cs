using System;

namespace Genealogy.Models
{
    /// <summary>
    /// Пользователь
    /// </summary>
    public class User
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Имя
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Логин
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Хэш пароля
        /// </summary>
        public byte[] PasswordHash { get; set; }

        /// <summary>
        /// Соль пароля
        /// </summary>
        public byte[] PasswordSalt { get; set; }

        /// <summary>
        /// Дата регистрации
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Дата удаления
        /// </summary>
        public DateTime FinishDate { get; set; }

        /// <summary>
        /// Флаг подтверждения регистрации
        /// </summary>
        public Guid? RoleId { get; set; }
        public Role Role { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }

    }
}

