using System;
using System.Collections.Immutable;
using dotnet.redux.Exceptions;
using dotnet.redux.Interfaces;

namespace dotnet.redux.Builders
{
    public static class ReduxBuilder
    {
        public static ReduxBuilder<TState, TActionType> New<TState, TActionType>()
            where TState : IState
            where TActionType : Enum
        {
            return new ReduxBuilder<TState, TActionType>();
        }
    }
    
    public class ReduxBuilder<TState, TActionType> : IReduxBuilder<TState, TActionType>
        where TState: IState
        where TActionType: Enum
    {
        public IReduxBuilderErrorHandler<TState, TActionType> WithInitialState(TState initialState)
        { 
            return new ReduxBuilderErrorHandler<TState, TActionType>(initialState);
        }
    }
    
    public class ReduxBuilderErrorHandler<TState, TActionType> : IReduxBuilderErrorHandler<TState, TActionType>
        where TState: IState
        where TActionType: Enum
    {
        private readonly TState _initialState;

        public ReduxBuilderErrorHandler(TState initialState)
        {
            _initialState = initialState;
        }

        public IReduxBuilderWithMiddleware<TState, TActionType> WithErrorHandler(Action<TState, IAction<TActionType>, Exception> errorHandler)
        {
            return new ReduxBuilderWithMiddleware<TState, TActionType>(_initialState, errorHandler);
        }
    }
    
    public class ReduxBuilderWithMiddleware<TState, TActionType> : IReduxBuilderWithMiddleware<TState, TActionType>
        where TState: IState
        where TActionType: Enum
    {
        private readonly TState _initialState;
        private readonly Action<TState, IAction<TActionType>, Exception> _errorHandler;

        public ReduxBuilderWithMiddleware(TState initialState, Action<TState, IAction<TActionType>, Exception> errorHandler)
        {
            _initialState = initialState;
            _errorHandler = errorHandler;
        }

        public IReduxBuilderReducers<TState, TActionType> WithMiddleware(Func<TState, TState> middleware)
        {
            return new ReduxBuilderReducers<TState, TActionType>(_initialState, _errorHandler, middleware);
        }
    }
    
    public class ReduxBuilderReducers<TState, TActionType> : IReduxBuilderReducers<TState, TActionType>
        where TState: IState
        where TActionType: Enum
    {
        private readonly TState _initialState;
        private readonly Action<TState, IAction<TActionType>, Exception> _errorHandler;
        private ImmutableDictionary<Func<TActionType, bool>, Func<TState, IAction<TActionType>, TState>> _reducers;
        private readonly Func<TState, TState> _middleware;

        public ReduxBuilderReducers(TState initialState, Action<TState, IAction<TActionType>, Exception> errorHandler, Func<TState, TState> middleware)
        {
            _initialState = initialState;
            _errorHandler = errorHandler;
            _middleware = middleware;
            _reducers = ImmutableDictionary<Func<TActionType, bool>, Func<TState, IAction<TActionType>, TState>>.Empty;
        }

        public IReduxBuilderReducers<TState, TActionType> WithReducer<TAction>(Func<TActionType, bool> predicate, Func<TState, TAction, TState> handler)
            where TAction: IAction<TActionType>
        {
            _reducers = _reducers.Add(predicate, (x, y) =>
            {
                switch (y)
                {
                    case TAction action:
                        return handler(x, action);
                    default:
                        throw new ReducerActionTypeMatchException<TAction>();
                }
            });

            return this;
        }

        public IRedux<TState, TActionType> Build()
        {
            return new Redux<TState, TActionType>(_initialState, _errorHandler, _middleware, _reducers);
        }
    }
}