using System;

namespace dotnet.redux.Interfaces
{
    public interface IReduxBuilder<TState, TActionType>
        where TState : IState
        where TActionType : Enum
    {
        IReduxBuilderErrorHandler<TState, TActionType> WithInitialState(TState initialState);
    }
    
    public interface IReduxBuilderErrorHandler<TState, TActionType>
        where TState : IState
        where TActionType : Enum
    {
        IReduxBuilderWithMiddleware<TState, TActionType> WithErrorHandler(Action<TState, IAction<TActionType>, Exception> errorHandler);
    }

    public interface IReduxBuilderWithMiddleware<TState, in TActionType>
        where TState : IState
        where TActionType : Enum
    {
        IReduxBuilderReducers<TState, TActionType> WithMiddleware(Func<TState, TState> middleware);
    }
    
    public interface IReduxBuilderReducers<TState, in TActionType>
        where TState : IState
        where TActionType : Enum
    {
        IReduxBuilderReducers<TState, TActionType> WithReducer<TAction>(TActionType actionType,
            Func<TState, TAction, TState> handler) where TAction : IAction<TActionType>;

        IRedux<TState, TActionType> Build();
    }
}