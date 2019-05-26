using System;

namespace dotnet.redux.Exceptions
{
    public class ReducerMatchException<TActionType> : Exception
        where TActionType: Enum
    {
        public ReducerMatchException(TActionType actionType) : base($"Failed to match action type: {actionType}")
        {
            
        }
    }
}