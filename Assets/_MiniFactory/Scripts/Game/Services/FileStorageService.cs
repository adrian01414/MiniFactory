using System;
using System.IO;
using MiniFactory.Core;
using UnityEngine;

namespace MiniFactory.Game.Services
{
    public sealed class FileStorageService : IStorageService
    {
        public void Save(string key, object data, Action<bool> callback)
        {
            try
            {
                File.WriteAllText(BuildPath(key), JsonUtility.ToJson(data));
                callback?.Invoke(true);
            }
            catch (Exception exception)
            {
                Debug.LogError(exception);
                callback?.Invoke(false);
            }
        }

        public void Load<T>(string key, Action<T> callback)
        {
            try
            {
                string path = BuildPath(key);

                if (!File.Exists(path))
                {
                    callback?.Invoke(default(T));
                    return;
                }

                callback?.Invoke(JsonUtility.FromJson<T>(File.ReadAllText(path)));
            }
            catch (Exception exception)
            {
                Debug.LogError(exception);
                callback?.Invoke(default(T));
            }
        }

        public void Delete(string key, Action<bool> callback)
        {
            try
            {
                string path = BuildPath(key);

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                callback?.Invoke(true);
            }
            catch (Exception exception)
            {
                Debug.LogError(exception);
                callback?.Invoke(false);
            }
        }

        private static string BuildPath(string key)
        {
            return Path.Combine(Application.persistentDataPath, key + ".json");
        }
    }
}
