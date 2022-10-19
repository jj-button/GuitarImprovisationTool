using System;
using System.Collections.Generic;
using System.Text;

namespace NEA
{
    static class ExtensionMethods
    {
        public static List<int> FindIndexAll<T>(this List<T> keyData, Func<T, bool> func)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < keyData.Count; i++)
            {
                if (func.Invoke(keyData[i]))
                {
                    list.Add(i);
                }
            }
            return list;
        }
    }
}
