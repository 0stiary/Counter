using Counter.Configuration;
using Counter.Resources.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;

namespace Counter
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public IServiceProvider ServiceProvider { get; private set; }

		public IConfiguration Configuration { get; private set; }

		public KeyboardListener KeyboardListener { get; private set; }

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			ShutdownMode = ShutdownMode.OnMainWindowClose;

			var configuration = new ConfigurationBuilder();

			var userConfigFilePath = Path.Combine(Directory.GetCurrentDirectory(), "appConfig.json");
			var configfilePath = File.Exists(userConfigFilePath) ? userConfigFilePath : "Configuration/defaultConfig.json";

			configuration.AddJsonFile(configfilePath, optional: false);

			Configuration = configuration.Build();

			var serviceCollection = new ServiceCollection();
			ConfigureServices(serviceCollection);

			ServiceProvider = serviceCollection.BuildServiceProvider();

			var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
			mainWindow.Show();
			MainWindow = mainWindow;
		}

		private void ConfigureServices(IServiceCollection services)
		{
			services.Configure<AppConfiguration>(Configuration);
			services.AddSingleton(KeyboardListener = new KeyboardListener());
			services.AddTransient(typeof(MainWindow));
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			KeyboardListener.Dispose();
		}
	}
}
