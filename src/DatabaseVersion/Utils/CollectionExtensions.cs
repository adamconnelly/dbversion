namespace dbversion.Utils
{
    using System;
    using System.Collections.Generic;

    public static class CollectionExtensions
    {
        /// <summary>
        /// Performs the specified action for each item in the collection.
        /// </summary>
        /// <param name='collection'>
        /// The collection.
        /// </param>
        /// <param name='action'>
        /// The action to perform.
        /// </param>
        /// <typeparam name='T'>
        /// The type of items in the collection.
        /// </typeparam>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection != null)
            {
                foreach (var item in collection)
                {
                    action(item);
                }
            }
        }
    }
}

