using Erp.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Repositories
{
    public class UserRepository : RepositoryBase, IUserRepository
    {
        public void Add(UserModel userModel)
        {
            throw new NotImplementedException();
        }

        public bool AuthenticateUser(NetworkCredential credential)
        {
            bool validUser;
            using (var connection =GetConnection()) 
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection= connection;
                command.CommandText = "select * from [User] where username=@username and [password]=@password";
                command.Parameters.Add("@username",System.Data.SqlDbType.NVarChar).Value= credential.UserName;
                command.Parameters.Add("@password", System.Data.SqlDbType.NVarChar).Value = credential.Password;
                validUser = command.ExecuteScalar() == null ? false : true;
                


            }
            return validUser;
        }

        public void Edit(UserModel userModel)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<UserModel> GetByAll()
        {
            throw new NotImplementedException();
        }
        public UserModel GetById(int Id)
        {
            throw new NotImplementedException();
        }
        public UserModel GetByUserName(string userName)
        {
            UserModel user =null;
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "select * from [User] where username=@username" ;
                command.Parameters.Add("@username", System.Data.SqlDbType.NVarChar).Value = userName;
                using (var reader = command.ExecuteReader()) 
                {
                    if (reader.Read())
                    {
                        user = new UserModel()
                        {
                            Id = reader[0].ToString(),
                            UserName= reader[1].ToString(),
                            Password= string.Empty,
                            Name= reader[3].ToString(),
                            LastName= reader[4].ToString(),
                            Email= reader[5].ToString(),
                        };
                    }
                }



            }
            return user;
        }

        public void Remove(int Id)
        {
            throw new NotImplementedException();
        }
    }
}
