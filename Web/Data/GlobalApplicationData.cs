using System;
using System.Collections;

namespace Web.Data
{
    internal enum GlobalDataKey
    {
        Core,
        Config
    }

    static class GlobalApplicationData
    {
        private static Hashtable GlobalData = new Hashtable();

        public static void SetGlobalData<TObject>(GlobalDataKey key, TObject data)
        {
            GlobalData.Add(key, data);
        }

        public static TObject GetGlobalData<TObject>(GlobalDataKey key)
        {
            return (TObject)GlobalData[key];
        }
    }
}