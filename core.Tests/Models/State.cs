using System.Collections.Immutable;
using dotnet.redux.Interfaces;

namespace core.Tests
{
    public class State : IState
    {
        public ImmutableList<string> Values { get; set; } = ImmutableList<string>.Empty;
        
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}