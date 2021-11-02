using System;
using System.Collections.Generic;

namespace Vertical.ConsoleApplications.Pipeline
{
    /// <summary>
    /// Represents the features of a command request.
    /// </summary>
    public class RequestItems
    {
        private readonly Dictionary<Type, object> _data = new();
        
        public void Set<T>(T feature) where T : class
        {
            if (feature == null)
            {
                throw new InvalidOperationException("Null feature data is unsupported");
            }
            
            _data[typeof(T)] = feature!;
        }

        public T? Get<T>() where T : class
        {
            return _data.TryGetValue(typeof(T), out var data)
                ? (T)data
                : null;
        }
        
        public T GetRequired<T>() where T : class
        {
            return Get<T>() ?? throw new InvalidOperationException($"Required feature '{typeof(T)}' is not available.");
        }
    }
}