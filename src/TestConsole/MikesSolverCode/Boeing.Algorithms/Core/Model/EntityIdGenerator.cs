using System;
using System.Collections.Generic;

namespace Boeing.Algorithms.Core.Model
{
    public static class EntityIdGenerator
    {
        private static readonly Dictionary<Type, int> ObjectIds = new Dictionary<Type, int>();

        public static int NextId(this object obj)
        {
            var type = obj.GetType();
            int current;
            if (!ObjectIds.TryGetValue(type, out current))
            {
                current = 0;
                ObjectIds.Add(obj.GetType(), current);
            }
            ObjectIds[type] = ++current;
            return current;
        }
    }
}