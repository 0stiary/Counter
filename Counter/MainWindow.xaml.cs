using Counter.Configuration;
using Counter.Resources.Utils;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Counter
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private int _Counter { get; set; }
		private AppConfiguration _Config { get; set; }
		private AppSettings _AppSettings => _Config.AppSettings;
		private UISettings _UISettings => _Config.UISettings;
		private ButtonSettings _PlusButtonSettings => _Config.PlusButtonSettings;
		private ButtonSettings _MinusButtonSettings => _Config.MinusButtonSettings;
		private KeyboardListener _KeyboardListener { get; set; }
		private FontsSettings _FontsSettings { get; set; }

		public MainWindow(IOptions<AppConfiguration> config, KeyboardListener keyboardListener)
		{
			InitializeComponent();

			_Config = config.Value;
			_KeyboardListener = keyboardListener;

			InitializeUIFromConfig();

			InitializeEvents();

			_FontsSettings = new FontsSettings(this.CounterBlock, PlusButtonText, MinusButtonText);

			AlwaysOnTopButton.IsChecked = Topmost;
		}


		#region Initializing

		private void InitializeEvents()
		{
			PlusButton.Click += PlusButton_Click;
			PlusButton.MouseWheel += Buttton_MouseWheel;

			MinusButton.Click += MinusButton_Click;
			MinusButton.MouseWheel += Buttton_MouseWheel;

			CounterBlockContainer.MouseUp += CounterBlockContainer_MouseUp;
			CounterBlockContainer.MouseWheel += CounterBlockContainer_MouseWheel;

			_KeyboardListener.KeyUp += KeyboardListener_KeyUp;

			#region ContextMenu

			ResetButton.Click += ResetButton_Click;
			AlwaysOnTopButton.Click += AlwaysOnTopButton_Click;
			ChangeCounterButton.Click += ChangeCounterButton_Click;
			ChangeFontsButton.Click += ChangeFontsButton_Click;
			ChangeBackgroundButton.Click += ChangeBackgroundButton_Click;

			#endregion ContextMenu

			this.Closing += MainFrame_Closing;
		}

		private void InitializeUIFromConfig()
		{
			CounterBlock.Text = (_Counter = _AppSettings.Counter).ToString();

			this.Topmost = _UISettings.TopMost;
			this.Width = _UISettings.WindowWidth;
			this.Height = _UISettings.WindowHeight;
			SetMainWindowBackground(_UISettings.MainWindowBackgroundFilePath);

			CounterBlock.FontSize = _UISettings.CounterFontSize;
			CounterBlock.FontFamily = new FontFamily(_UISettings.CounterFontFamily);

			PlusButtonText.FontFamily 
				= MinusButtonText.FontFamily = new FontFamily(_UISettings.ButtonsFontFamily);

			PlusButton.Opacity = _PlusButtonSettings.Opacity;
			PlusButton.Margin = _PlusButtonSettings.Margin;

			MinusButton.Opacity = _MinusButtonSettings.Opacity;
			MinusButton.Margin = _MinusButtonSettings.Margin;
		}

		#endregion Initializing


		#region Events

		private void Buttton_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			var butt = sender as Button;

			butt.Opacity += e.Delta < 0 ? -0.05 : +0.05;

			if (butt.Opacity >= 1)
			{
				butt.Opacity = 1;
			}
			else if (butt.Opacity <= 0)
			{
				butt.Opacity = 0;
			}
		}

		private void MinusButton_Click(object sender, RoutedEventArgs e) => ChangeCounter(-1);

		private void PlusButton_Click(object sender, RoutedEventArgs e) => ChangeCounter(+1);

		private void CounterBlockContainer_MouseUp(object sender, MouseButtonEventArgs e)
		{
			switch (e.ChangedButton)
			{
				case MouseButton.Left:
					{
						ChangeCounter(+1);
						break;
					}
				case MouseButton.Right:
					{
						ChangeCounter(-1);
						break;
					}
				case MouseButton.Middle:
					{
						MainContainer.ContextMenu.IsOpen = true;
						break;
					}
				default:
					break;
			}
		}

		private void CounterBlockContainer_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			CounterBlock.FontSize += e.Delta < 0 ? -1 : +1;

			if (CounterBlock.FontSize < 2)
				CounterBlock.FontSize = 2;
		}

		private void KeyboardListener_KeyUp(object sender, RawKeyEventArgs e)
		{
			if (e.Key == Key.RightCtrl)
				ChangeCounter(+1);
		}

		#region ContextMenu

		private void ResetButton_Click(object sender, RoutedEventArgs e)
		{
			_Counter = 0;
			CounterBlock.Text = "0";
		}

		private void AlwaysOnTopButton_Click(object sender, RoutedEventArgs e)
		{
			(sender as MenuItem).IsChecked = Topmost = !Topmost;
		}

		private void ChangeCounterButton_Click(object sender, RoutedEventArgs e)
		{
			this.IsEnabled = false;

			try
			{
				var dialog = new CounterChangeDialog("Change counter", _Counter) { Owner = this };
				dialog.ShowDialog();
				if (dialog.DialogResult == true)
					CounterBlock.Text = (_Counter = dialog.InputCounter).ToString();
			}
			finally
			{
				this.IsEnabled = true;
			}
		}

		private void ChangeFontsButton_Click(object sender, RoutedEventArgs e)
		{
			_FontsSettings.Owner = this;
			_FontsSettings.Show();
		}

		private void ChangeBackgroundButton_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog op = new OpenFileDialog();
			op.Multiselect = false;
			op.Title = "Select a picture";
			op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
			  "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
			  "Portable Network Graphic (*.png)|*.png";

			if (op.ShowDialog() == true)
			{
				var destFile = "Resources/Images/background" + new FileInfo(op.FileName).Extension;
				//var fullDestFilePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), destFile);

				this.Background = Brushes.White;

				File.Copy(op.FileName, destFile, true);

				var file = new FileInfo(destFile);

				SetMainWindowBackground(file.FullName);

				_UISettings.MainWindowBackgroundFilePath = file.FullName;

				MessageBox.Show($"Your image for background was save to \n\n \"{file.FullName}\"", "Confirmed", MessageBoxButton.OK, MessageBoxImage.Information);
			
			}
		}

		#endregion ContextMenu


		private void MainFrame_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			_AppSettings.Counter = _Counter;

			_UISettings.TopMost = this.Topmost;
			_UISettings.WindowWidth = this.Width;
			_UISettings.WindowHeight = this.Height;
			_UISettings.CounterFontSize = CounterBlock.FontSize;
			_UISettings.CounterFontFamily = CounterBlock.FontFamily.Source;
			_UISettings.ButtonsFontFamily = PlusButtonText.FontFamily.Source;

			_PlusButtonSettings.Opacity = PlusButton.Opacity;
			_PlusButtonSettings.Margin = PlusButton.Margin;

			_MinusButtonSettings.Opacity = MinusButton.Opacity;
			_MinusButtonSettings.Margin = MinusButton.Margin;

			_Config.Dispose();
			_FontsSettings.Close();
		}

		#endregion Events


		#region Logic

		private void ChangeCounter(int value)
		{
			_Counter += value;
			CounterBlock.Text = _Counter.ToString();
		}

		private void SetMainWindowBackground(string fullFilePath)
		{
			this.Background = new ImageBrush(LoadBitmapImage(fullFilePath)) { Opacity = 0.5, Stretch = Stretch.Fill, TileMode = TileMode.Tile };
		}

		public static BitmapImage LoadBitmapImage(string fileName)
		{
			using (var stream = new FileStream(fileName, FileMode.Open))
			{
				var bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.StreamSource = stream;
				bitmapImage.EndInit();
				bitmapImage.Freeze();
				return bitmapImage;
			}
		}

		#endregion
	}
}
