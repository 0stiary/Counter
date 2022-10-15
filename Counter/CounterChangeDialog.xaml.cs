using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Counter
{
	/// <summary>
	/// Interaction logic for CounterChangeDialog.xaml
	/// </summary>
	public partial class CounterChangeDialog : Window
	{
		public CounterChangeDialog()
		{
			InitializeComponent();
		}

		public CounterChangeDialog(string title, int counter)
		{
			InitializeComponent();
			TitleText = title;
			InputCounter = counter;
		}

		public string TitleText
		{
			get { return TitleTextBox.Text; }
			set { TitleTextBox.Text = value; }
		}

		public int InputCounter
		{
			get { return Convert.ToInt32(InputTextBox.Text); }
			set { InputTextBox.Text = value.ToString(); }
		}

		private void BtnCancel_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

		private void BtnOk_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}

		private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!string.IsNullOrEmpty(InputTextBox.Text) && !int.TryParse(InputTextBox.Text, out int value))
			{
				MessageBox.Show("Invalid data, only (+ / -) integer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				InputTextBox.Undo();
			}
		}
	}
}
