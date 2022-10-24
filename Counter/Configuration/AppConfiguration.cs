using System;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace Counter.Configuration
{
	public class AppConfiguration : IDisposable
	{
		public AppSettings AppSettings { get; set; }
		
		public UISettings UISettings { get; set; }

		public ButtonSettings PlusButtonSettings { get; set; }

		public ButtonSettings MinusButtonSettings { get; set; }

		private void Save()
		{
			var jsonConfig = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });

			File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "appConfig.json"), jsonConfig);
		}

		public void Dispose()
		{
			Save();
		}
	}

	public class AppSettings
	{
		public int Counter { get; set; }
	}

	public class UISettings
	{
		public bool TopMost { get; set; }
		public double WindowWidth { get; set; }
		public double WindowHeight { get; set; }
		public double CounterFontSize { get; set; }
		public string CounterFontFamily { get; set; }
		public string ButtonsFontFamily { get; set; }
		public string MainWindowBackgroundFilePath { get; set; }
	}

	public class ButtonSettings
	{
		public double Opacity { get; set; }
		public Thickness Margin { get; set; }
	}
		
}