using dotnet.redux.Interfaces;

namespace core.Tests
{
    public class DeleteAction : IAction<ActionEnums>
    {
        public ActionEnums Type { get; } = ActionEnums.Delete;

        public string Value { get; set; }
    }
}