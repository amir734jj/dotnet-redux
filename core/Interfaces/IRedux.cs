using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace dotnet.redux.Interfaces
{
    public interface IRedux<TState, in TActionType>
        where TState : IState
        where TActionType : Enum
    {
        Task Dispatch(IAction<TActionType> action);
        
        EventHandler<TState> EventHandler { get; set; }
        
        TState CurrentState { get; }
        
        ImmutableList<KeyValuePair<TState, DateTime>> States { get; }
    }
}