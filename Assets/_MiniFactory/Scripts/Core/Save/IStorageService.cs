using System;

namespace MiniFactory.Core
{
    public interface IStorageService
    {
        void Save(string key, object data, Action<bool> callback);
        void Load<T>(string key, Action<T> callback);
    }
}
