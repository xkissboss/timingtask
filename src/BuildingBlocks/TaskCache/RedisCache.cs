using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dy.TaskCache
{
    public class RedisCache : ICache
    {
        string _connectionString;
        string _prefix;
        readonly int _dbNum;
        int _expireSeconds;
        ConnectionMultiplexer _redis;
        object _locker = new object();
        public RedisCache(string connectionString, int dbNum, string prefix, int expireSeconds)
        {
            _connectionString = connectionString;
            _dbNum = dbNum;
            _prefix = prefix;
            _expireSeconds = expireSeconds;
        }
        ConnectionMultiplexer Manager
        {
            get
            {
                if (_redis == null)
                {
                    lock (_locker)
                    {
                        if (_redis != null) return _redis;
                        _redis = GetManager();
                        return _redis;
                    }
                }
                return _redis;
            }
        }
        ConnectionMultiplexer GetManager()
        {
            ConfigurationOptions connection = new ConfigurationOptions();
            connection.EndPoints.Add(_connectionString);
            connection.AbortOnConnectFail = false;// 当为true时，当没有可用的服务器时则不会创建一个连接
            connection.ConnectRetry = 5; // 重试连接的次数
            connection.KeepAlive = 60; // 保存x秒的活动连接
            //connection.Ssl = true;
            connection.AllowAdmin = true;
            connection.SyncTimeout = 8000; //异步超时时间
            connection.ConnectTimeout = 20000; //超时时间
            return ConnectionMultiplexer.Connect(connection);
        }
        private string KeyFormatting(string key)
        {
            if (key.StartsWith(_prefix))
            {
                return key;
            }
            else
            {
                return $"{_prefix}{key}";
            }
        }
        private string KeyRemoveFormatting(string key)
        {
            if (key.StartsWith(_prefix))
            {
                return key.Remove(0, _prefix.Length);
            }
            else
            {
                return key;
            }
        }
        public bool Add(string key, string value, int expireSeconds = 0)
        {
            var db = Manager.GetDatabase(_dbNum);
            if (expireSeconds == -1)
            {
                db.StringSet(KeyFormatting(key), value);
            }
            else if (expireSeconds > 0)
            {
                db.StringSet(KeyFormatting(key), value, TimeSpan.FromSeconds(expireSeconds));
            }
            else
            {
                db.StringSet(KeyFormatting(key), value, TimeSpan.FromSeconds(_expireSeconds));
            }
            return db.StringSet(KeyFormatting(key), value);
        }
        public void AddList(string key, string[] values)
        {
            var db = Manager.GetDatabase(_dbNum);
            if (db.KeyExists(KeyFormatting(key)))
            {
                db.KeyDelete(KeyFormatting(key));
            }
            for (int i = 0; i < values.Length; i++)
            {
                db.ListRightPush(KeyFormatting(key), values[i]);
            }
        }
        public bool Contains(string key)
        {
            var db = Manager.GetDatabase(_dbNum);
            return db.KeyExists(KeyFormatting(key));
        }
        public string Dequeue(string key)
        {
            var db = Manager.GetDatabase(_dbNum);
            var value = db.ListRightPop(KeyFormatting(key), 0);
            if (value.IsNullOrEmpty)
            {
                return "";
            }
            else
            {
                return value;
            }
        }
        public T Dequeue<T>(string key)
        {
            var db = Manager.GetDatabase(_dbNum);
            var value = db.ListRightPop(KeyFormatting(key), 0);
            if (value.IsNullOrEmpty)
            {
                return default(T);
            }
            else
            {
                return value.ToString().ToObject<T>();
            }
        }
        public void Enqueue(string key, string value)
        {
            var db = Manager.GetDatabase(_dbNum);
            db.ListLeftPush(KeyFormatting(key), value, 0, 0);
        }
        public void Enqueue(string key, string[] values)
        {
            var db = Manager.GetDatabase(_dbNum);
            for (int i = 0; i < values.Length; i++)
            {
                db.ListLeftPush(KeyFormatting(key), values[i], 0, 0);
            }
        }
        public void Enqueue<T>(string key, T value)
        {
            var db = Manager.GetDatabase(_dbNum);
            db.ListLeftPush(KeyFormatting(key), value.ToJson(), 0, 0);
        }
        public string Get(string key)
        {
            var db = Manager.GetDatabase(_dbNum);
            return db.StringGet(KeyFormatting(key));
        }
        public string Get(string key, Func<string> acquire, int expireSeconds = 0)
        {
            var db = Manager.GetDatabase(_dbNum);
            if (db.KeyExists(KeyFormatting(key)))
            {
                return db.StringGet(KeyFormatting(key));
            }
            else
            {
                string value = acquire.Invoke();
                if (expireSeconds == -1)
                {
                    db.StringSet(KeyFormatting(key), value);
                }
                else if (expireSeconds > 0)
                {
                    db.StringSet(KeyFormatting(key), value, TimeSpan.FromSeconds(expireSeconds));
                }
                else
                {
                    db.StringSet(KeyFormatting(key), value, TimeSpan.FromSeconds(_expireSeconds));
                }
                return value;
            }
        }
        public T Get<T>(string key, Func<T> acquire, int expireSeconds = 0)
        {
            var db = Manager.GetDatabase(_dbNum);
            if (db.KeyExists(KeyFormatting(key)))
            {
                string value = db.StringGet(KeyFormatting(key));
                return value.ToObject<T>();
            }
            else
            {
                T t = acquire.Invoke();
                string value = t.ToJson();
                if (expireSeconds == -1)
                {
                    db.StringSet(KeyFormatting(key), value);
                }
                else if (expireSeconds > 0)
                {
                    db.StringSet(KeyFormatting(key), value, TimeSpan.FromSeconds(expireSeconds));
                }
                else
                {
                    db.StringSet(KeyFormatting(key), value, TimeSpan.FromSeconds(_expireSeconds));
                }
                return t;
            }
        }
        public CacheHash[] GetHash(string key)
        {
            var db = Manager.GetDatabase(_dbNum);
            HashEntry[] hashArray = db.HashGetAll(KeyFormatting(key));
            return Array.ConvertAll(hashArray, item => new CacheHash() { Key = item.Name, Value = item.Value });
        }
        public CacheHash GetHash(string key, string hashField)
        {
            var db = Manager.GetDatabase(_dbNum);
            var hash = db.HashGet(KeyFormatting(key), hashField);
            if (hash.IsNullOrEmpty)
            {
                return null;
            }
            else
            {
                return new CacheHash() { Key = hashField, Value = hash };
            }
        }
        public string[] GetKeys()
        {
            var server = Manager.GetServer(_connectionString);
            var keys = server.Keys(_dbNum, "*").ToArray();
            return Array.ConvertAll(keys, k => KeyRemoveFormatting(k.ToString()));
        }
        public string[] GetKeysContains(string key)
        {
            var server = Manager.GetServer(_connectionString);
            var keys = server.Keys(_dbNum, KeyFormatting($"*{key}*")).ToArray();
            return Array.ConvertAll(keys, k => KeyRemoveFormatting(k.ToString()));
        }
        public string[] GetKeysEndWith(string key)
        {
            var server = Manager.GetServer(_connectionString);
            var keys = server.Keys(_dbNum, KeyFormatting($"*{key}")).ToArray();
            return Array.ConvertAll(keys, k => KeyRemoveFormatting(k.ToString()));
        }
        public string[] GetKeysStartWith(string key)
        {
            var server = Manager.GetServer(_connectionString);
            var keys = server.Keys(_dbNum, KeyFormatting($"{key}*")).ToArray();
            return Array.ConvertAll(keys, k => KeyRemoveFormatting(k.ToString()));
        }
        public string[] GetList(string key)
        {
            var db = Manager.GetDatabase(_dbNum);
            return db.ListRange(KeyFormatting(key)).ToStringArray();
        }
        public bool Remove(string key)
        {
            var db = Manager.GetDatabase(_dbNum);
            return db.KeyDelete(KeyFormatting(key));
        }
        public void Remove(string[] keys)
        {
            var db = Manager.GetDatabase(_dbNum);
            db.KeyDelete(Array.ConvertAll(keys, item => (RedisKey)KeyFormatting(item)));
        }
        public void RemoveAll()
        {
            var server = Manager.GetServer(_connectionString);
            var keys = server.Keys(_dbNum, KeyFormatting("*")).ToArray();
            var db = Manager.GetDatabase(_dbNum);
            db.KeyDelete(Array.ConvertAll(keys, item => (RedisKey)(item)));
        }
        public void RemoveContaions(string key)
        {
            string[] keys = GetKeysContains(key);
            Remove(keys);
        }
        public void RemoveEndWith(string key)
        {
            string[] keys = GetKeysEndWith(key);
            Remove(keys);
        }
        public void RemoveHash(string key, string hashField)
        {
            var db = Manager.GetDatabase(_dbNum);
            db.HashDelete(KeyFormatting(key), hashField);
        }
        public void RemoveStartWith(string key)
        {
            string[] keys = GetKeysStartWith(key);
            Remove(keys);
        }
        public bool Set<T>(string key, Func<T> acquire, int expireSeconds = 0)
        {
            var db = Manager.GetDatabase(_dbNum);
            if (db.KeyExists(KeyFormatting(key)))
            {
                db.KeyDelete(KeyFormatting(key));
            }
            T t = acquire.Invoke();
            string value = t.ToJson();
            if (expireSeconds == -1)
            {
                return db.StringSet(KeyFormatting(key), value);
            }
            else if (expireSeconds > 0)
            {
                return db.StringSet(KeyFormatting(key), value, TimeSpan.FromSeconds(expireSeconds));
            }
            else
            {
                return db.StringSet(KeyFormatting(key), value, TimeSpan.FromSeconds(_expireSeconds));
            }
        }
        public bool Set<T>(string key, T value, int expireSeconds = 0)
        {
            var db = Manager.GetDatabase(_dbNum);
            if (db.KeyExists(KeyFormatting(key)))
            {
                db.KeyDelete(KeyFormatting(key));
            }
            if (expireSeconds == -1)
            {
                return db.StringSet(KeyFormatting(key), value.ToJson());
            }
            else if (expireSeconds > 0)
            {
                return db.StringSet(KeyFormatting(key), value.ToJson(), TimeSpan.FromSeconds(expireSeconds));
            }
            else
            {
                return db.StringSet(KeyFormatting(key), value.ToJson(), TimeSpan.FromSeconds(_expireSeconds));
            }
        }
        public void SetHash(string key, string hashField, string value)
        {
            var db = Manager.GetDatabase(_dbNum);
            db.HashSet(KeyFormatting(key), hashField, value);
        }
        public void SetHash(string key, params CacheHash[] cacheHashArray)
        {
            var db = Manager.GetDatabase(_dbNum);
            db.HashSet(KeyFormatting(key), Array.ConvertAll(cacheHashArray, item => new HashEntry(item.Key, item.Value)));
        }
        public void SetList(string key, string[] values)
        {
            var db = Manager.GetDatabase(_dbNum);
            for (int i = 0; i < values.Length; i++)
            {
                db.ListRightPush(KeyFormatting(key), values[i]);
            }
        }


        public void SetList<T>(string key, Func<T[]> acquire, int expireSeconds = 0)
        {
            var db = Manager.GetDatabase(_dbNum);
            string formatKey = KeyFormatting(key);
            if (db.KeyExists(formatKey))
            {
                db.KeyDelete(formatKey);
            }

            T[] values = acquire.Invoke();
            if (values == null)
                return;
            for (int i = 0; i < values.Length; i++)
            {
                db.ListRightPush(formatKey, values[i].ToJson());
            }
            
        }





        public long ListLength(string key)
        {
            var db = Manager.GetDatabase(_dbNum);
            return db.ListLength(KeyFormatting(key));
        }
        public CacheHash[] GetHash(string key, Func<CacheHash[]> acquire)
        {
            var db = Manager.GetDatabase(_dbNum);
            if (db.KeyExists(KeyFormatting(key)))
            {
                var hashArray = db.HashGetAll(KeyFormatting(key));
                return Array.ConvertAll(hashArray, item => new CacheHash() { Key = item.Name, Value = item.Value });
            }
            else
            {
                CacheHash[] valueArray = acquire.Invoke();
                db.HashSet(KeyFormatting(key), Array.ConvertAll(valueArray, item => new HashEntry(item.Key, item.Value)));
                return valueArray;
            }
        }
        public T[] GetList<T>(string key, Func<T[]> acquire)
        {
            var db = Manager.GetDatabase(_dbNum);
            if (db.KeyExists(KeyFormatting(key)))
            {
                return Array.ConvertAll(db.ListRange(KeyFormatting(key)).ToStringArray(), i => i.ToObject<T>());
            }
            else
            {
                T[] values = acquire.Invoke();
                if (values == null)
                    return null;
                for (int i = 0; i < values.Length; i++)
                {
                    db.ListRightPush(KeyFormatting(key), values[i].ToJson());
                }
                return values;
            }
        }
        public string[] GetList(string key, Func<string[]> acquire)
        {
            var db = Manager.GetDatabase(_dbNum);
            if (db.KeyExists(KeyFormatting(key)))
            {
                return db.ListRange(KeyFormatting(key)).ToStringArray();
            }
            else
            {
                string[] values = acquire.Invoke();
                for (int i = 0; i < values.Length; i++)
                {
                    db.ListRightPush(KeyFormatting(key), values[i]);
                }
                return values;
            }
        }
        public int[] GetList(string key, Func<int[]> acquire)
        {
            var db = Manager.GetDatabase(_dbNum);
            if (db.KeyExists(KeyFormatting(key)))
            {
                return Array.ConvertAll(db.ListRange(KeyFormatting(key)).ToStringArray(), a => int.Parse(a));
            }
            else
            {
                int[] values = acquire.Invoke();
                for (int i = 0; i < values.Length; i++)
                {
                    db.ListRightPush(KeyFormatting(key), values[i]);
                }
                return values;
            }
        }
    }
}
