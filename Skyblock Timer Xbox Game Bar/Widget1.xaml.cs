using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Skyblock_Timer_Xbox_Game_Bar {
	public sealed partial class Widget1 : Page {
		private readonly List<SkyblockTimerViewModel> _timerList;
		//private List<SkyblockTimerViewModel> _visibleTimerList;
		private string _filterSelection;
		private string _sortSelection = "Name";

		private List<Task> _refreshTaskList;

		public Widget1() {
			this.InitializeComponent();

			this._timerList = new List<SkyblockTimerViewModel>() {
				new SkyblockTimerViewModel("Magma Boss Spawn", Constants.MAGMA_ESTIMATE_URL, "Assets/TimerIcons/MagmaCube.png"),
				new SkyblockTimerViewModel("New Year Event", Constants.NEWYEAR_ESTIMATE_URL, "Assets/TimerIcons/NewYearCake.png"),
				new SkyblockTimerViewModel("Zoo Event", Constants.ZOO_ESTIMATE_URL, "Assets/TimerIcons/TravellingZoo.png"),
				new SkyblockTimerViewModel("Winter Event", Constants.WINTER_ESTIMATE_URL, "Assets/TimerIcons/WinterEvent.png"),
				new SkyblockTimerViewModel("Jerry's Workshop Event", Constants.JERRYS_WORKSHOP_ESTIMATE_URL, "Assets/TimerIcons/JerrysWorkshop.png"),
				new SkyblockTimerViewModel("Spooky Festival Event", Constants.SPOOKY_FESTIVAL_ESTIMATE_URL, "Assets/TimerIcons/SpookyFestival.png"),
				new SkyblockTimerViewModel("Dark Auction", Constants.DARK_AUCTION_ESTIMATE_URL, "Assets/TimerIcons/DarkAuction.png"),
				new SkyblockTimerViewModel("Brood Mother Boss Spawn", Constants.BROOD_MOTHER_ESTIMATE_URL, "Assets/TimerIcons/BroodMother.png"),
				new SkyblockTimerViewModel("Bank Interest Timer", Constants.INTEREST_ESTIMATE_URL, "Assets/TimerIcons/BankInterest.png")
			};

			this.UpdateVisibleTimerList();
		}

		private void UpdateVisibleTimerList() {
			if (this._timerList != null) {
				// update sort paramater
				switch (this._sortSelection) {
					case "Name":
						this._timerList.Sort((SkyblockTimerViewModel a, SkyblockTimerViewModel b) => {
							return a.Name.CompareTo(b.Name);
						});

						break;
					case "Time to event":
						this._timerList.Sort((SkyblockTimerViewModel a, SkyblockTimerViewModel b) => {
							return a.TimeToEvent.CompareTo(b.TimeToEvent);
						});
						break;
					default:
						throw new Exception(); // should never happen
				}

				this.TimerList.Items.Clear();
				foreach (var timer in this._timerList) {
					this.TimerList.Items.Add(timer);
				}
			}
		}

		private void RefreshButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
			_ = this.RefreshAllTimers();
			this.UpdateVisibleTimerList();
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
			}
		}

		private void FilterChanged(object sender, SelectionChangedEventArgs e) {
			Debug.Assert(e.AddedItems.Count == 1);
			this.SortFlyoutButton.Flyout?.Hide();

			this._filterSelection = e.AddedItems[0] as string;
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
