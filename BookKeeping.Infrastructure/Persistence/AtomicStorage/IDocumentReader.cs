namespace BookKeeping.Persistance.AtomicStorage
{
    public interface IDocumentReader<in TKey, TView>
    {
        /// <summary>
        /// Gets the view with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="view">The view.</param>
        /// <returns>
        /// true, if it exists
        /// </returns>
        bool TryGet(TKey key, out TView view);
    }
}