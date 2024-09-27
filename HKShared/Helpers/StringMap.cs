using System;
using System.Collections.Generic;
using System.Text;

namespace HKShared.Helpers
{
    public class StringMap<T> : SortedList<string, T>
    {
        public new T this[string key]
        {
            get
            {
                if (key != null && ContainsKey(key))
                    return base[key];
                else
                    return default(T);
            }
            set
            {
                base[key] = value;
            }
        }
    }

    public class StringMap : StringMap<string>
    {
    }

    public class NumberMap<T> : SortedList<int, T>
    {
        public new T this[int key]
        {
            get
            {
                if (ContainsKey(key))
                    return base[key];
                else
                    return default(T);
            }
            set
            {
                base[key] = value;
            }
        }
    }

    public class NumberMap : NumberMap<string>
    {
    }
}
