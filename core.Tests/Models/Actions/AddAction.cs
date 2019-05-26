using dotnet.redux.Interfaces;

namespace core.Tests
{
    public class AddAction : IAction<ActionEnums>
    {
        public ActionEnums Type { get; } = ActionEnums.Add;
        
        public string Value { get; set; }
    }
}