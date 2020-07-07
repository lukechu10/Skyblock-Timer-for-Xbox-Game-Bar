using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Skyblock_Timer_Xbox_Game_Bar {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class Widget1 : Page {
		public Widget1() {
			this.InitializeComponent();
		}

		private void RefreshButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
			RefreshButton.Content = "Refreshed!";
			TimerList.Items.Add(new SkyblockTimerViewModel());
		}
	}
}
