using System;
using System.Collections.Generic;
using System.Text;

namespace Dy.TaskCache
{
    public interface ICache
    {
        string Get(string key);
        string Get(string key, Func<string> acquire, int expireSeconds = 0);
        T Get<T>(string key, Func<T> acquire, int expireSeconds = 0);
        bool Set<T>(string key, Func<T> acquire, int expireSeconds = 0);
        bool Set<T>(string key, T value, int expireSeconds = 0);
        bool Add(string key, string value, int expireSeconds = 0);
        void SetList(string key, string[] values);

        void SetList<T>(string key, Func<T[]> acquire, int expireSeconds = 0);

        void AddList(string key, string[] values);
        string[] GetList(string key);
        string[] GetList(string key, Func<string[]> acquire);
        int[] GetList(string key, Func<int[]> acquire);
        T[] GetList<T>(string key, Func<T[]> acquire);
        CacheHash[] GetHash(string key);
        CacheHash[] GetHash(string key, Func<CacheHash[]> acquire);
        CacheHash GetHash(string key, string hashField);
        void SetHash(string key, string hashField, string value);
        void SetHash(string key, params CacheHash[] cacheHashArray);
        void RemoveHash(string key, string hashField);
        bool Contains(string key);
        long ListLength(string key);
        bool Remove(string key);
        void Remove(string[] keys);
        void RemoveAll();
        void RemoveStartWith(string key);
        void RemoveEndWith(string key);
        void RemoveContaions(string key);
        string Dequeue(string key);
        T Dequeue<T>(string key);
        void Enqueue(string key, string value);
        void Enqueue(string key, string[] values);
        void Enqueue<T>(string key, T value);
        string[] GetKeys();
        string[] GetKeysStartWith(string key);
        string[] GetKeysEndWith(string key);
        string[] GetKeysContains(string key);
    }
}
