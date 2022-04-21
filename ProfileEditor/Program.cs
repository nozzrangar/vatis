using ProfileEditor.Common;
using ProfileEditor.Config;
using ProfileEditor.Core;
using SimpleInjector;
using SimpleInjector.Diagnostics;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ProfileEditor
{
    internal static class Program
    {
        private static Container Container;
        private static IAppConfig AppConfig;
        private static string AppPath;

        [STAThread]
        static void Main()
        {
            if (!SingleAppInstance.Exists("profile-editor"))
            {
                Application.CurrentCulture = new CultureInfo("en-US");
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
                AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

                AppPath = Path.GetDirectoryName(Environment.ProcessPath);

                Container = new Container();

                AutoRegisterWindowsForms(Container);

                Container.RegisterSingleton<IAppConfig, AppConfig>();
                Container.RegisterSingleton<IUserInterface, UserInterfaceFactory>();
                Container.RegisterSingleton<INavaidDatabase, NavaidDatabase>();

                Container.Verify();

                AppConfig = Container.GetInstance<IAppConfig>();

                Application.Run(Container.GetInstance<MainForm>());
            }
        }

        private static void OnProcessExit(object? sender, EventArgs e)
        {
            AppConfig.SaveConfig();
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                using (var fs = new FileStream(Path.Combine(AppPath, "ProfileEditorExceptions.txt"), FileMode.Append, FileAccess.Write, FileShare.None, 1024, false))
                {
                    using (var sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(Assembly.GetEntryAssembly().FullName);
                        sw.WriteLine("============================================================");
                        sw.WriteLine("Culture     : " + CultureInfo.CurrentCulture.Name);
                        sw.WriteLine("OS          : " + Environment.OSVersion.ToString());
                        sw.WriteLine("Framework   : " + Environment.Version);
                        sw.WriteLine("Time        : " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                        sw.WriteLine("------------------------------------------------------------");
                        sw.WriteLine("Details: " + ex.ToString());
                        sw.WriteLine("============================================================");
                        sw.Flush();
                        MessageBox.Show(null, "Unhandled exception: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    }
                }
            }
        }

        private static void AutoRegisterWindowsForms(Container container)
        {
            var formTypes = container.GetTypesToRegister<Form>(typeof(Program).Assembly);

            foreach (var type in formTypes)
            {
                var registration = Lifestyle.Transient.CreateRegistration(type, container);

                registration.SuppressDiagnosticWarning(
                    DiagnosticType.DisposableTransientComponent,
                    "Forms should be disposed by app code; not by the container.");

                container.AddRegistration(type, registration);
            }
        }
    }
}
