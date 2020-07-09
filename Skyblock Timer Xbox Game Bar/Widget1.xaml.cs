using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Skyblock_Timer_Xbox_Game_Bar {
	public sealed partial class Widget1 : Page {
		private readonly List<SkyblockTimerViewModel> _timerList;
		private IEnumerable<SkyblockTimerViewModel> _visibleTimerList;
		private TimerType _filterSelection = TimerType.All;
		private string _sortSelection = "Name";

		private List<Task> _refreshTaskList;

		public Widget1() {
			this.InitializeComponent();

			this._timerList = new List<SkyblockTimerViewModel>() {
				new SkyblockTimerViewModel("Magma Boss Spawn", Constants.MAGMA_ESTIMATE_URL, "Assets/TimerIcons/MagmaCube.png", TimerType.Boss),
				new SkyblockTimerViewModel("New Year Event", Constants.NEWYEAR_ESTIMATE_URL, "Assets/TimerIcons/NewYearCake.png", TimerType.Event),
				new SkyblockTimerViewModel("Zoo Event", Constants.ZOO_ESTIMATE_URL, "Assets/TimerIcons/TravellingZoo.png", TimerType.Event),
				new SkyblockTimerViewModel("Winter Event", Constants.WINTER_ESTIMATE_URL, "Assets/TimerIcons/WinterEvent.png", TimerType.Event),
				new SkyblockTimerViewModel("Jerry's Workshop Event", Constants.JERRYS_WORKSHOP_ESTIMATE_URL, "Assets/TimerIcons/JerrysWorkshop.png", TimerType.Event),
				new SkyblockTimerViewModel("Spooky Festival Event", Constants.SPOOKY_FESTIVAL_ESTIMATE_URL, "Assets/TimerIcons/SpookyFestival.png", TimerType.Event),
				new SkyblockTimerViewModel("Dark Auction", Constants.DARK_AUCTION_ESTIMATE_URL, "Assets/TimerIcons/DarkAuction.png", TimerType.Event),
				new SkyblockTimerViewModel("Brood Mother Boss Spawn", Constants.BROOD_MOTHER_ESTIMATE_URL, "Assets/TimerIcons/BroodMother.png", TimerType.Boss),
				new SkyblockTimerViewModel("Bank Interest Timer", Constants.INTEREST_ESTIMATE_URL, "Assets/TimerIcons/BankInterest.png", TimerType.Event)
			};

			this.UpdateVisibleTimerList();
		}

		private void UpdateVisibleTimerList() {
			if (this._timerList != null) {
				// filter this._visibleTimerList
				this._visibleTimerList = from timer in this._timerList
										 where (this._filterSelection == TimerType.All || timer.Type == this._filterSelection)
										 select timer;

				// sort this._visibleTimerList
				if (this._sortSelection == "Name")
					this._visibleTimerList = this._visibleTimerList.OrderBy(timer => timer.Name);
				else
					this._visibleTimerList = this._visibleTimerList.OrderBy(timer => timer.TimeToEvent);


				this.TimerList.Items.Clear();
				foreach (var timer in this._visibleTimerList) {
					this.TimerList.Items.Add(timer);
				}
			}
		}

		private void RefreshButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
			_ = this.RefreshAllTimers();
		}

		private async Task RefreshAllTimers() {
			this.RefreshButton.Content = "Refreshing...";
			this.RefreshButton.IsEnabled = false;

			this._refreshTaskList = new List<Task>();

			foreach (var timer in this._timerList) {
				timer.RelativeTimeMessage = "Refreshing...";
				this._refreshTaskList.Add(timer.RefreshTimerWithServer());
			}

			try {
				await Task.WhenAll(this._refreshTaskList.ToArray());
			}
			finally {
				// always perform this step
				this.RefreshButton.Content = "Refresh";
				this.RefreshButton.IsEnabled = true;
				this.UpdateVisibleTimerList();
			}
		}

		private void FilterChanged(object sender, SelectionChangedEventArgs e) {
			Debug.Assert(e.AddedItems.Count == 1);
			this.SortFlyoutButton.Flyout?.Hide();

			string selection = e.AddedItems[0] as string;

			switch(selection) {
				case "All":
					this._filterSelection = TimerType.All;
					break;
				case "Bosses":
					this._filterSelection = TimerType.Boss;
					break;
				case "Events":
					this._filterSelection = TimerType.Event;
					break;
			}
			this.UpdateVisibleTimerList();
		}

		private void SortChanged(object sender, SelectionChangedEventArgs e) {
			Debug.Assert(e.AddedItems.Count == 1);
			this.SortFlyoutButton.Flyout?.Hide();

			this._sortSelection = e.AddedItems[0] as string;
			this.UpdateVisibleTimerList();
		}
	}
}
