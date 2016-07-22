using System.Collections.Generic;
using System.Configuration;
using Training.Caching;
using Training.DataAccess;
using Training.Entities;

namespace Training.BusinessLogic
{
    public class DataProcessor
    {
        private bool _useMemCache = false;
        private UserProfilesAccess _dataLayer;
        private UserProfileCache _memCache;
        public DataProcessor()
        {
            bool.TryParse(ConfigurationManager.AppSettings["MemCacheEnabled"], out _useMemCache);
            if (_useMemCache)
            {
                _memCache = UserProfileCache.getInstance();
            }
            _dataLayer = new UserProfilesAccess();
        }

        public int GetNumberOfUsers()
        {
            if (_useMemCache)
                return _memCache.GetNumberOfUsers(_dataLayer.GetNumberOfUsers);
            else
                return _dataLayer.GetNumberOfUsers();
        }

        public List<UserProfile> GetUserProfiles(int pageNo, string orderFormat)
        {
            if (_useMemCache)
                return _memCache.GetUserProfiles(pageNo, orderFormat, _dataLayer.ReadUserProfiles);
            else
                return _dataLayer.ReadUserProfiles(pageNo, orderFormat);
        }

        public int InsertUserProfile(UserProfile user)
        {
            if (_useMemCache)
                _memCache.Invalidate();
            return _dataLayer.CreateUserProfile(user);
        }

        public int UpdateUserProfile(UserProfile user)
        {
            if (_useMemCache)
                _memCache.Invalidate();
            return _dataLayer.UpdateUserProfile(user);
        }

        public int RemoveUserProfile(int id)
        {
            if (_useMemCache)
                _memCache.Invalidate();
            return _dataLayer.DeleteUserProfile(id);
        }
    }
}
