﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model
{
    public interface IUserRepository
    {
        bool AuthenticateUser(NetworkCredential credential);
        void Add(UserModel userModel);
        void Edit(UserModel userModel);
        void Remove(int Id);
        UserModel GetById(int Id);
        UserModel GetByUserName (string userName);
        IEnumerable<UserModel> GetByAll();
    }
}
