using System;

namespace dotnet.redux.Interfaces
{
    public interface IAction<out TActionType> where TActionType: Enum
    {
        TActionType Type { get; }
    }
}