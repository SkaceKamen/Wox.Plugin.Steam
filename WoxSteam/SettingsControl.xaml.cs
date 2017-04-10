using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using UserControl = System.Windows.Controls.UserControl;

namespace WoxSteam
{
	/// <summary>
	/// Interaction logic for SettingsControl.xaml
	/// </summary>
	public partial class SettingsControl : UserControl
	{
		public Options Options;

		public SettingsControl(Options current)
		{
			InitializeComponent();

			Options = current;
			SteamPathText.Text = Options.SteamPath ?? "";
		}

		private void PickPath_Click(object sender, RoutedEventArgs e)
		{
			// Shows path selector dialog
			var fd = new FolderBrowserDialog
			{
				Description = "Select path to Steam installation folder",
				SelectedPath = Options.SteamPath
			};

			// Stop if dialog was cancelled
			if (fd.ShowDialog() != DialogResult.OK) return;

			// Save selected path
			SteamPathText.Text = Options.SteamPath = fd.SelectedPath;
		}

		private void SteamPathText_TextChanged(object sender, TextChangedEventArgs e)
		{
			// Only update options on valid paths
			if (!Directory.Exists(SteamPathText.Text)) return;

			Options.SteamPath = SteamPathText.Text;
			Options.Save();
		}
	}
}
