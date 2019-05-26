using System;

namespace dotnet.redux.Exceptions
{
    internal class ReducerActionTypeMatchException<T> : Exception
    {
        public ReducerActionTypeMatchException() : base($"Failed to pattern match type with name: {typeof(T).Name}")
        {
            
        }
    }
}