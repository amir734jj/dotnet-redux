using System;

namespace dotnet.redux.Exceptions
{
    internal class ReducerMatchException<TActionType> : Exception
        where TActionType: Enum
    {
        public ReducerMatchException(TActionType actionType) : base($"Failed to match action type: {actionType}")
        {
            
        }
    }
}