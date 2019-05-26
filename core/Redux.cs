using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using dotnet.redux.Exceptions;
using dotnet.redux.Extensions;
using dotnet.redux.Interfaces;
using static dotnet.redux.Constants.ReduxConstants;

namespace dotnet.redux
{
    internal class Redux<TState, TActionType> : IRedux<TState, TActionType>
        where TState: IState
        where TActionType: Enum
    {
        private readonly Action<TState, IAction<TActionType>> _errorHandler;
        private readonly ImmutableDictionary<Func<TActionType, bool>, Func<TState, IAction<TActionType>, TState>> _reducers;
        private readonly Func<TState, TState> _middleware;
        
        private ImmutableList<KeyValuePair<TState, DateTime>> _states;

        public Redux(TState initialState, Action<TState, IAction<TActionType>> errorHandler,
            Func<TState, TState> middleware, ImmutableDictionary<Func<TActionType, bool>, Func<TState, IAction<TActionType>, TState>> reducers)
        {
            _errorHandler = errorHandler;
            _middleware = middleware;
            _reducers = reducers;
            _states = ImmutableList<KeyValuePair<TState, DateTime>>.Empty.Add(new KeyValuePair<TState, DateTime>(initialState, DateTime.Now));
            CurrentState = initialState;
        }

        /// <summary>
        ///     Dispatch an action
        /// </summary>
        /// <param name="action"></param>
        public async Task Dispatch(IAction<TActionType> action)
        {
            await Task.Factory.StartNew(() =>
            {
                var targetReducer = _reducers.FirstOrDefault(x => x.Key(action.Type));

                if (targetReducer.IsDefault())
                {
                    _errorHandler(_states.LastOrDefault().Key, action);
                    
                    throw new ReducerMatchException<TActionType>(action.Type);
                }

                // Update the state
                var updatedState = targetReducer.Value(CurrentState, action);

                // Run updated state through middleware
                updatedState = _middleware(updatedState);

                // Add new state to list of states
                _states = _states.Add(new KeyValuePair<TState, DateTime>(updatedState, DateTime.Now)).TakeLast(StatesCountLimit).ToImmutableList();

                // Set the current state
                CurrentState = _states.Last().Key;
            });
        }

        public EventHandler<TState> EventHandler { get; set; }

        public TState CurrentState { get; private set; } 

        public ImmutableList<TState> States => _states.Select(x => x.Key).ToImmutableList();
    }
}