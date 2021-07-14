using E5R.Architecture.Core;

namespace E5R.Zero
{
    public class Zero
    {
        public Zero(string name)
        {
            Checker.NotEmptyOrWhiteArgument(name, nameof(name));

            Name = name;
        }

        public string Name { get; }
    }
}
