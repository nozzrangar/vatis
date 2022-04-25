using SimpleInjector;

namespace Vatsim.Vatis.Client.Core
{
    internal sealed class UserInterfaceFactory : IUserInterface
    {
        private readonly Container mContainer;

        public UserInterfaceFactory(Container container) => mContainer = container;

        public MainForm CreateMainForm() => mContainer.GetInstance<MainForm>();

        public ProfileList CreateFacilityDialogForm() => mContainer.GetInstance<ProfileList>();

        public ProfileConfiguration CreateProfileConfigurationForm() => mContainer.GetInstance<ProfileConfiguration>();

        public SettingsForm CreateSettingsForm() => mContainer.GetInstance<SettingsForm>();

        public UserInputForm CreateUserInputForm() => mContainer.GetInstance<UserInputForm>();

        public VersionCheckForm CreateVersionCheckForm() => mContainer.GetInstance<VersionCheckForm>();

        public NewCompositeDialog CreateNewCompositeDialog() => mContainer.GetInstance<NewCompositeDialog>();

        public MiniDisplay CreateMiniDisplay() => mContainer.GetInstance<MiniDisplay>();
    }

    internal interface IUserInterface
    {
        MainForm CreateMainForm();
        SettingsForm CreateSettingsForm();
        ProfileList CreateFacilityDialogForm();
        UserInputForm CreateUserInputForm();
        ProfileConfiguration CreateProfileConfigurationForm();
        VersionCheckForm CreateVersionCheckForm();
        NewCompositeDialog CreateNewCompositeDialog();
        MiniDisplay CreateMiniDisplay();
    }
}
