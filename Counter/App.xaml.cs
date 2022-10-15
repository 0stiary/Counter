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


		protected override void OnStartup(StartupEventArgs e)
		{
			Configuration = new ConfigurationBuilder()
			 .SetBasePath(Directory.GetCurrentDirectory())
			 .AddJsonFile("appConfig.json", optional: false)
			 .Build();


			var serviceCollection = new ServiceCollection();
			ConfigureServices(serviceCollection);

			ServiceProvider = serviceCollection.BuildServiceProvider();
			
			//Show MainWindow
			var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
			mainWindow.Show();
		}

		private void ConfigureServices(IServiceCollection services)
		{
			services.Configure<AppConfiguration>(Configuration);
			services.AddSingleton(KeyboardListener = new KeyboardListener());
			// ...

			services.AddTransient(typeof(MainWindow));
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			KeyboardListener.Dispose();
		}

	}
}
