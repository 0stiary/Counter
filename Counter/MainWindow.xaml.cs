using Counter.Configuration;
using Counter.Resources.Utils;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

		public MainWindow(IOptions<AppConfiguration> config, KeyboardListener keyboardListener)
		{
			InitializeComponent();

			_Config = config.Value;
			_KeyboardListener = keyboardListener;

			InitializeUIFromConfig();

			InitializeEvents();

			AlwaysOnTopButton.IsChecked = Topmost;
		}


		#region Initializing

		private void InitializeEvents()
		{
			this.Closing += MainFrame_Closing;

			PlusButton.Click += PlusButton_Click;
			PlusButton.MouseWheel += PlusButtton_MouseWheel;

			MinusButton.Click += MinusButton_Click;
			MinusButton.MouseWheel += PlusButtton_MouseWheel;

			CounterBlockContainer.MouseUp += CounterBlockContainer_MouseUp;

			#region ContextMenu

			ResetButton.Click += ResetButton_Click;
			AlwaysOnTopButton.Click += AlwaysOnTopButton_Click;
			ChangeCounterButton.Click += ChangeCounterButton_Click;

			#endregion ContextMenu

			_KeyboardListener.KeyUp += KeyboardListener_KeyUp;
		}

		private void InitializeUIFromConfig()
		{
			//(?'settingsProp'\w.+)(?'initSign' = )(?'winProp'.+)(?'dotCommaSign';)

			CounterBlock.Text = (_Counter = _AppSettings.Counter).ToString();

			this.Topmost = _UISettings.TopMost;
			this.Width = _UISettings.WindowWidth;
			this.Height = _UISettings.WindowHeight;

			PlusButton.Opacity = _PlusButtonSettings.Opacity;
			PlusButton.Margin = _PlusButtonSettings.Margin;

			MinusButton.Opacity = _MinusButtonSettings.Opacity;
			MinusButton.Margin = _MinusButtonSettings.Margin;
		}

		#endregion Initializing


		#region Events

		private void MinusButton_Click(object sender, RoutedEventArgs e) => ChangeCounter(-1);

		private void PlusButton_Click(object sender, RoutedEventArgs e) => ChangeCounter(+1);

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

		private void PlusButtton_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			var butt = sender as Button;

			butt.Opacity += e.Delta / (double)1000;

			if (butt.Opacity > 1)
			{
				butt.Opacity = 1;
			}
			else if (butt.Opacity < 0)
			{
				butt.Opacity = 0;
			}
		}

		void KeyboardListener_KeyUp(object sender, RawKeyEventArgs e)
		{
			if (e.Key == Key.RightCtrl)
				ChangeCounter(+1);
		}

		private void MainFrame_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{

			_AppSettings.Counter = _Counter;

			_UISettings.TopMost = this.Topmost;
			_UISettings.WindowWidth = this.Width;
			_UISettings.WindowHeight = this.Height;

			_PlusButtonSettings.Opacity = PlusButton.Opacity;
			_PlusButtonSettings.Margin = PlusButton.Margin;

			_MinusButtonSettings.Opacity = MinusButton.Opacity;
			_MinusButtonSettings.Margin = MinusButton.Margin;

			_Config.Dispose();
		}

		#endregion Events


		#region Logic

		private void ChangeCounter(int value)
		{
			_Counter += value;
			CounterBlock.Text = _Counter.ToString();
		}

		#endregion
	}
}
