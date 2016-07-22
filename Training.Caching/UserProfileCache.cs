using Enyim.Caching;
using Enyim.Caching.Memcached;
using System;
using System.Collections.Generic;
using Training.Entities;

namespace Training.Caching
{
    public class UserProfileCache
    {
        private static UserProfileCache _instance;
        private static MemcachedClient _mc;
        private static readonly object padlock = new object();
        private int _cacheVersion;

        private UserProfileCache()
        {
            _mc = new MemcachedClient("memcached");

            // Invalidation using versioning.
            // Don't modify this code.
            _cacheVersion = 1;
        }

        public static UserProfileCache getInstance()
        {
            if (_instance == null)
            {
                lock (padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new UserProfileCache();
                    }
                }
            }
            return _instance;
        }

        public int GetNumberOfUsers(Func<int> dbCallback)
        {
            string key = "RT_count_" + _cacheVersion;
            var cacheResult = _mc.Get(key);
            if (cacheResult == null)
            {
                int t = dbCallback();
                _mc.Store(StoreMode.Set, key, t, DateTime.Now.AddSeconds(50));
                return t;
            }
            return (int)cacheResult;
        }

        public List<UserProfile> GetUserProfiles(int pageNo, string orderFormat, Func<int, string, List<UserProfile>> dbCallback)
        {
            string key = string.Concat("RT_UserList_", orderFormat, "_", pageNo, "_", _cacheVersion);
            var cacheResult = _mc.Get(key);
            if (cacheResult == null)
            {
                List<UserProfile> userList = dbCallback(pageNo, orderFormat);
                _mc.Store(StoreMode.Set, key, userList, DateTime.Now.AddSeconds(50));
                return userList;
            }
            return (List<UserProfile>)cacheResult;
        }

        public void Invalidate()
        {
            _cacheVersion++;
        }
    }
}
