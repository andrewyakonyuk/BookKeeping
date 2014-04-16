namespace BookKeeping.Core
{
    using System.Collections.Generic;

    public interface IPagedEnumerable<out T> : IEnumerable<T>
    {
        /// <summary>
        ///     Total number of entries across all pages.
        /// </summary>
        int TotalCount { get; }
    }
}