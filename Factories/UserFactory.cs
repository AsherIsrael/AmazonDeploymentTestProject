using System.Data;
using Dapper;
using QuotingDojoRedux.Models;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace QuotingDojoRedux.Factories
{
    public class UserFactory : IFactory<User>
    {
        private readonly IOptions<MySqlOptions> MySqlConfig;
        
        public UserFactory(IOptions<MySqlOptions> config)
        {
            MySqlConfig = config;
        }

        internal IDbConnection Connection
        {
            get { return new MySqlConnection(MySqlConfig.Value.ConnectionString); }
        }

        public void Add(User Item)
        {
            using(IDbConnection dbConnection = Connection)
            {
                string Query = "INSERT INTO users (FirstName, LastName, Email, Password, CreatedAt, UpdatedAt) VALUES (@FirstName, @LastName, @Email, @Password, NOW(), NOW())";
                dbConnection.Open();
                dbConnection.Execute(Query, Item);
            }
        }

        public User GetLatestUser()
        {
            using(IDbConnection dbConnection = Connection)
            {
                string Query = "SELECT * FROM users ORDER BY UserId DESC LIMIT 1";
                dbConnection.Open();
                return dbConnection.QuerySingleOrDefault<User>(Query);
            }
        }

        public User GetUserById(int Id)
        {
            using(IDbConnection dbConnection = Connection)
            {
                string Query = $"SELECT * FROM users WHERE UserId = {Id}";
                dbConnection.Open();
                return dbConnection.QuerySingleOrDefault<User>(Query);
            }
        }

        public User GetuserByEmail(string Email)
        {
            using(IDbConnection dbConnection = Connection)
            {
                string Query = $"SELECT * FROM users WHERE Email = '{Email}'";
                dbConnection.Open();
                return dbConnection.QuerySingleOrDefault<User>(Query);
            }
        }
    }
}