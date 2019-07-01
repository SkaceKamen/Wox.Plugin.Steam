using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace WoxSteam
{
	/// <summary>
	/// Interaction logic for SettingsControl.xaml
	/// </summary>
	public partial class SettingsControl
	{
		private readonly Options _options;

		public SettingsControl(Options current)
		{
			InitializeComponent();

			_options = current;
			SteamPathText.Text = _options.SteamPath ?? "";
		}

		private void PickPath_Click(object sender, RoutedEventArgs e)
		{
			// Shows path selector dialog
			var fd = new FolderBrowserDialog
			{
				Description = "Select path to Steam installation folder",
				SelectedPath = _options.SteamPath
			};

			// Stop if dialog was cancelled
			if (fd.ShowDialog() != DialogResult.OK) return;

			// Save selected path
			SteamPathText.Text = _options.SteamPath = fd.SelectedPath;
		}

		private void SteamPathText_TextChanged(object sender, TextChangedEventArgs e)
		{
			// Only update options on valid paths
			if (!Directory.Exists(SteamPathText.Text)) return;

			_options.SteamPath = SteamPathText.Text;
			_options.Save();
		}
	}
}
