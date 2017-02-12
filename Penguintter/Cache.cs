using System;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Cache;
using System.Runtime.Serialization.Formatters.Binary;
using CoreTweet;

namespace Penguintter
{
    [Serializable()]
    public class Cache
    {
        private Dictionary<long, BitmapImage> IconDict { get; } = new Dictionary<long, BitmapImage>();

        static public Cache LoadCache()
        {
            try
            {
                using (var st = new System.IO.FileStream("cache", System.IO.FileMode.Open))
                {
                    return (Cache)(new BinaryFormatter()).Deserialize(st);
                }
            }
            catch
            {
                return null;
            }
        }

        public void Store(Status status)
        {
            if (status.User.Id != null)
            {
                IconDict[(long)status.User.Id] = new BitmapImage(new Uri(status.User.ProfileImageUrlHttps));
            }
        }

        public BitmapImage GetIcon(long? id)
        {
            BitmapImage res = null;
            if (id != null)
            {
                IconDict.TryGetValue((long)id, out res);
            }
            return res;
        }
    }
}
