using System;
using System.Collections.Generic;

namespace Vertical.ConsoleApplications.Pipeline
{
    /// <summary>
    /// Represents the features of a command request.
    /// </summary>
    /// <remarks>
    /// This collection maintains a property bag of objects. The key of each
    /// object is the value type. When items are added to the bag they are
    /// retrievable by their type, therefore only a single type per bag is
    /// allowed.
    /// </remarks>
    public class RequestItems
    {
        private readonly Dictionary<Type, object> _data = new();
        
        /// <summary>
        /// Sets data in the property bag.
        /// </summary>
        /// <param name="feature">The feature data to set.</param>
        /// <typeparam name="T">Data type</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="feature"/> is null</exception>
        public void Set<T>(T feature) where T : class
        {
            if (feature == null)
            {
                throw new ArgumentNullException(nameof(feature));
            }
            
            _data[typeof(T)] = feature!;
        }

        /// <summary>
        /// Retrieves an item from the bag.
        /// </summary>
        /// <typeparam name="T">The type of data to retrieve</typeparam>
        /// <returns>A reference to the data, or null if the feature was not registered</returns>
        public T? Get<T>() where T : class
        {
            return _data.TryGetValue(typeof(T), out var data)
                ? (T)data
                : null;
        }
        
        /// <summary>
        /// Retrieves an item from the bag or throws an exception.
        /// </summary>
        /// <typeparam name="T">The type of data to retrieve.</typeparam>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">The data was not added to the bag</exception>
        public T GetRequired<T>() where T : class
        {
            return Get<T>() ?? throw new InvalidOperationException($"Required feature '{typeof(T)}' is not available.");
        }
    }
}