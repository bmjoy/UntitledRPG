using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResourceManager
{
    static Dictionary<string, Object> mCaches = new Dictionary<string, Object>();
    public static T GetResource<T>(string path) where T : Object
    {
        if (!mCaches.ContainsKey(path))
            mCaches[path] = Resources.Load<T>(path);
        return (T)mCaches[path];
    }
}
