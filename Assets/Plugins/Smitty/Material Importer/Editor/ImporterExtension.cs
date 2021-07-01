using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Smitty.MaterialImporter
{
    public static class ImporterExtension
    {
        /// <summary>
        /// Remove duplicate entries from the List
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> MakeUnique<T>(this List<T> list)
        {
            return new List<T>(new HashSet<T>(list));
        }
    }
}
