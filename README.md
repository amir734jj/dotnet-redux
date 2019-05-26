# dotnet-redux
Simple redux implementation in C# dotnet core

```csharp
// Build redux instance
State initialState = new State();

IRedux<State, ActionEnums> redux = ReduxBuilder.New<State, ActionEnums>()
    .WithInitialState(initialState)
    .WithErrorHandler((x, y) => { })
    .WithMiddleware(x => x)
    .WithReducer<AddAction>(x => x == ActionEnums.Add, (state, addAction) =>
    {
        return new State {Values = state.Values.Add(addAction.Value)};
    })
    .WithReducer<DeleteAction>(x => x == ActionEnums.Delete, (state, deleteAction) =>
    {
        return new State {Values = state.Values.Remove(deleteAction.Value)};
    })
    .Build();

// Dispatch some events
await redux.Dispatch(new AddAction {Value = "Item1"});
await redux.Dispatch(new DeleteAction {Value = "Item1"});
await redux.Dispatch(new AddAction {Value = "Item2"});

// Get current states and states
State currenState = redux.CurrentState;
ImmutableList<KeyValuePair<State, DateTime>> states = redux.States;
```