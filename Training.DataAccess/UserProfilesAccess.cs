using Dapper;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using Training.Entities;

namespace Training.DataAccess
{
    public class UserProfilesAccess
    {
        private string _connectionString;
        private DbProviderFactory _dataProviderFactory;
        int pageSize;

        public UserProfilesAccess()
        {
            _connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            _dataProviderFactory = DbProviderFactories.GetFactory(ConfigurationManager.AppSettings["DataProvider"]);
            if (!int.TryParse(ConfigurationManager.AppSettings["PageSize"], out pageSize))
            {
                pageSize = 10;  // Defualt page size 10.
            }
        }

        public List<UserProfile> ReadUserProfiles(int pageNo, string orderFormat)
        {
            using (DbConnection conn = _dataProviderFactory.CreateConnection())
            {
                List<UserProfile> userList = new List<UserProfile>();
                conn.ConnectionString = _connectionString;
                using (DbCommand cmd = conn.CreateCommand())
                {
                    userList = conn.Query<UserProfile>("RT_GetUserProfiles", new { PageOffSet = (pageNo - 1) * pageSize, PageSize = pageSize, ordering = orderFormat }, commandType: CommandType.StoredProcedure).AsList();
                    return userList;
                }
            }
        }

        public int CreateUserProfile(UserProfile user)
        {
            using (DbConnection conn = _dataProviderFactory.CreateConnection())
            {
                conn.ConnectionString = _connectionString;
                using (DbCommand cmd = conn.CreateCommand())
                {
                    // Parameters for stored procedure with OUT parameter for generated ID.
                    DynamicParameters param = new DynamicParameters();
                    param.Add("fName", user.FirstName, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add("lName", user.LastName, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add("DOB", user.DateOfBirth, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param.Add("gen", user.Gender, dbType: DbType.StringFixedLength, direction: ParameterDirection.Input);
                    param.Add("userId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    conn.Execute("RT_InsertUserProfile", param, commandType: CommandType.StoredProcedure);
                    return (param.Get<int>("userId"));
                }
            }
        }

        // Return value -1 means duplicate entry exists in DB; 0 means ID not present; 1 means update success.
        public int UpdateUserProfile(UserProfile user)
        {
            using (DbConnection conn = _dataProviderFactory.CreateConnection())
            {
                conn.ConnectionString = _connectionString;
                using (DbCommand cmd = conn.CreateCommand())
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("userId", user.Id, dbType: DbType.Int32, direction: ParameterDirection.Input);
                    param.Add("fName", user.FirstName, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add("lName", user.LastName, dbType: DbType.String, direction: ParameterDirection.Input);
                    param.Add("DOB", user.DateOfBirth, dbType: DbType.Date, direction: ParameterDirection.Input);
                    param.Add("gen", user.Gender, dbType: DbType.StringFixedLength, direction: ParameterDirection.Input);
                    param.Add("count", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    conn.Execute("RT_UpdateUserProfile", param, commandType: CommandType.StoredProcedure);
                    return (param.Get<int>("count"));
                }
            }
        }

        public int DeleteUserProfile(int id)
        {
            using (DbConnection conn = _dataProviderFactory.CreateConnection())
            {
                conn.ConnectionString = _connectionString;
                using (DbCommand cmd = conn.CreateCommand())
                {
                    DynamicParameters param = new DynamicParameters(new { userId = id });
                    param.Add("resultCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    conn.Execute("RT_RemoveUserProfile", param, commandType: CommandType.StoredProcedure);
                    return (param.Get<int>("resultCode"));
                }
            }
        }

        public int GetNumberOfUsers()
        {
            using (DbConnection conn = _dataProviderFactory.CreateConnection())
            {
                conn.ConnectionString = _connectionString;
                using (DbCommand cmd = conn.CreateCommand())
                {
                    int count = conn.Query<int>("RT_NumberOfUsers", commandType: CommandType.StoredProcedure).AsList()[0];
                    return count;
                }
            }
        }
    }
}
