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
        private readonly Action<TState, IAction<TActionType>, Exception> _errorHandler;
        private readonly ImmutableDictionary<Func<TActionType, bool>, Func<TState, IAction<TActionType>, TState>> _reducers;
        private readonly Func<TState, TState> _middleware;

        public Redux(TState initialState, Action<TState, IAction<TActionType>, Exception> errorHandler,
            Func<TState, TState> middleware, ImmutableDictionary<Func<TActionType, bool>, Func<TState, IAction<TActionType>, TState>> reducers)
        {
            _errorHandler = errorHandler;
            _middleware = middleware;
            _reducers = reducers;
            States = ImmutableList<KeyValuePair<TState, DateTime>>.Empty.Add(new KeyValuePair<TState, DateTime>(initialState, DateTime.Now));
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
                    _errorHandler(States.LastOrDefault().Key, action, new Exception($"Failed to find a reducer for action type: {action.Type}"));
                    
                    throw new ReducerMatchException<TActionType>(action.Type);
                }

                var updatedState = CurrentState;

                try
                {
                    // Update the state
                    updatedState = targetReducer.Value(CurrentState, action);
                }
                catch (Exception e)
                {
                    _errorHandler(States.LastOrDefault().Key, action, e);

                    throw;
                }
                
                // Run updated state through middleware
                updatedState = _middleware(updatedState);

                // Add new state to list of states
                States = States.Add(new KeyValuePair<TState, DateTime>(updatedState, DateTime.Now)).TakeLast(StatesCountLimit).ToImmutableList();

                // Set the current state
                CurrentState = States.Last().Key;
            });
        }

        public EventHandler<TState> EventHandler { get; set; }

        public TState CurrentState { get; private set; }

        public ImmutableList<KeyValuePair<TState, DateTime>> States { get; private set; }
    }
}