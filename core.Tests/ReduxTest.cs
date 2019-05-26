using System.Linq;
using System.Threading.Tasks;
using dotnet.redux.Builders;
using Xunit;

namespace core.Tests
{
    public class ReduxTest
    {
        [Fact]
        public void Test__Base()
        {
            // Arrange
            var initialState = new State();
            
            var redux = ReduxBuilder.New<State, ActionEnums>()
                .WithInitialState(initialState)
                .WithErrorHandler((x, y) => { })
                .WithMiddleware(x => x)
                .Build();
            
            // Act, Assert
            Assert.Equal(initialState, redux.CurrentState);
            Assert.Single(redux.States);
            Assert.Contains(redux.States.Select(x => x.Key), x => x == initialState);
        }

        [Fact]
        public async Task Test__Reducers()
        {
            // Arrange
            var initialState = new State();
            
            var redux = ReduxBuilder.New<State, ActionEnums>()
                .WithInitialState(initialState)
                .WithErrorHandler((x, y) => { })
                .WithMiddleware(x => x)
                .WithReducer<AddAction>(x => x == ActionEnums.Add, (state, addAction) =>
                {
                    // ReSharper disable once ConvertToLambdaExpression
                    return new State {Values = state.Values.Add(addAction.Value)};
                })
                .WithReducer<DeleteAction>(x => x == ActionEnums.Delete, (state, deleteAction) =>
                {
                    // ReSharper disable once ConvertToLambdaExpression
                    return new State {Values = state.Values.Remove(deleteAction.Value)};
                })
                .Build();
            
            // Act
            await redux.Dispatch(new AddAction {Value = "Item1"});
            await redux.Dispatch(new DeleteAction {Value = "Item1"});
            await redux.Dispatch(new AddAction {Value = "Item2"});
            
            // Assert
            Assert.Single(redux.CurrentState.Values);
            Assert.Equal(4, redux.States.Count);
            Assert.Equal("Item2", redux.CurrentState.Values.First());
        }
    }
}