using System;
using System.Collections.Generic;
using System.Text;

namespace Netsphere.Client.Extensions
{
    public static class ListExtensions
    {
        public static bool IsEmpty<T>(this List<T> collection)
        {
            return collection.Count == 0;
        }
    }
}
