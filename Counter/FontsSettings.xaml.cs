using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Counter
{
	/// <summary>
	/// Interaction logic for FontsSettings.xaml
	/// </summary>
	public partial class FontsSettings : Window
	{
		public TextBlock CounterTextBlock { get; set; }
		public IEnumerable<TextBlock> ButtonsTextBlocks { get; set; }

		public FontsSettings(TextBlock counterTextBlock, params TextBlock[] buttonsText)
		{
			InitializeComponent();

			ButtonsTextBlocks = buttonsText;
			CounterTextBlock = counterTextBlock;

			InitializeListBoxes();

			#region Events

			ButtonsFontListBox.SelectionChanged += ButtonsFontListBox_SelectionChanged;
			CounterFontListBox.SelectionChanged += CounterFontListBox_SelectionChanged;
			this.Closing += FontsSettings_Closing;

			#endregion Events
		}

		#region Initializing
		private void InitializeListBoxes()
		{
			//var listItems = Fonts.SystemFontFamilies
			var listItems = new InstalledFontCollection().Families
				.Select(x => new TextBlock()
				{
					Text = x.Name,
					FontFamily = new FontFamily(x.Name)
				});

			ButtonsFontListBox.ItemsSource = listItems.ToList();
			CounterFontListBox.ItemsSource = listItems.ToList();
		}

		#endregion Initializing

		#region Events

		private void CounterFontListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			CounterTextBlock.FontFamily = GetFontFromSelectedItem(CounterFontListBox);
		}

		private void ButtonsFontListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var font = GetFontFromSelectedItem(ButtonsFontListBox);

			foreach (var button in ButtonsTextBlocks)
				button.FontFamily = font;
		}

		private void FontsSettings_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = true;
			this.Hide();
		}

		#endregion Events

		#region Logic

		private FontFamily GetFontFromSelectedItem(ListBox listbox)
		{
			var item = listbox.SelectedItem as TextBlock;
			return new FontFamily(item.FontFamily.Source);
		}

		#endregion Logic
	}
}
