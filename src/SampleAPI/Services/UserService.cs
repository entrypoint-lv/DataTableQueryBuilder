using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.EntityFrameworkCore;

namespace SampleAPI.Services
{
    using Models;
    using Models.Data;

    public class UserService : BaseService<User>, IUserService
    {
        public UserService(IDataContext dataContext)
            : base(dataContext)
        {
        }

        public IQueryable<UserListData> GetAllForUserList()
        {
            return dataContext.Users
            .Select(u => new UserListData()
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                CompanyName = u.Company != null ? u.Company.Name : string.Empty,
                Posts = u.Posts.Count,
                CreateDate = u.CreateDate
            });
        }
    }

    public interface IUserService : IService<User>
    {
        IQueryable<UserListData> GetAllForUserList();
    }
}
