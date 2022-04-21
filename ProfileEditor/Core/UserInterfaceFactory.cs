using SimpleInjector;

namespace ProfileEditor.Core
{
    internal sealed class UserInterfaceFactory : IUserInterface
    {
        private readonly Container mContainer;

        public UserInterfaceFactory(Container container) => mContainer = container;

        public MainForm CreateMainForm() => mContainer.GetInstance<MainForm>();

        public UserInputForm CreateUserInputForm() => mContainer.GetInstance<UserInputForm>();
    }

    internal interface IUserInterface
    {
        MainForm CreateMainForm();
        UserInputForm CreateUserInputForm();
    }
}
