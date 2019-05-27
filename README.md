# dotnet-redux
Simple redux implementation in C# dotnet core. See the unit test project for more examples.

```csharp
// Build redux instance
State initialState = new State();

IRedux<State, ActionEnums> redux = ReduxBuilder.New<State, ActionEnums>()
    .WithInitialState(initialState)
    .WithErrorHandler((state, action, exception) => {
        // Log the exception if any ...
    })
    .WithMiddleware(state => {
        // This func is called after state has been updated; this is the place to modify the state
        // just before state being finalized
        return x;
    })
    .WithReducer<AddAction>(x => x == ActionEnums.Add, (state, addAction) =>
    {
        // Return an updated state
        return new State {Values = state.Values.Add(addAction.Value)};
    })
    .WithReducer<DeleteAction>(x => x == ActionEnums.Delete, (state, deleteAction) =>
    {
        // Return an updated state
        return new State {Values = state.Values.Remove(deleteAction.Value)};
    })
    .Build();

// Dispatch some events
await redux.Dispatch(new AddAction {Value = "Item1"});
await redux.Dispatch(new DeleteAction {Value = "Item1"});
await redux.Dispatch(new AddAction {Value = "Item2"});

// Get current states and states
// Note that states contains the last n = 100 states with timestamp of when they are created
State currenState = redux.CurrentState;
ImmutableList<KeyValuePair<State, DateTime>> states = redux.States;

// Listen to changes
redux.EventHandler += (reduxInstance, state) => {
    // Listen to any changes to redux
}
```
