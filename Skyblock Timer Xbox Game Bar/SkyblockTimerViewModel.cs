using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Skyblock_Timer_Xbox_Game_Bar {
	class SkyblockTimerViewModel : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;

		private TimeSpan _timeToEvent;
		private DispatcherTimer _dispatcherTimer;
		private static readonly HttpClient _httpClient = new HttpClient();

		public SkyblockTimerViewModel(Uri queryUrl) {
			this.QueryUrl = queryUrl;
			if (this.QueryUrl == null) throw new ArgumentException("queryUrl can not be null");

			_ = this.setupTimer();
		}

		/// <summary>
		/// Queries the url specified by <c>QueryUrl</c> and starts the timer with the returned time
		/// </summary>
		private async Task setupTimer() {
			try {
				string responseStr = await _httpClient.GetStringAsync(this.QueryUrl);

				MagmaResponseSchema response = JsonConvert.DeserializeObject<MagmaResponseSchema>(responseStr);
				long estimateTimestamp = response.estimate; // unix time stamp for next spawn estimate
				DateTime dateTime = DateTimeOffset.FromUnixTimeMilliseconds(estimateTimestamp).UtcDateTime;
				this._timeToEvent = dateTime - DateTime.UtcNow;

				this._dispatcherTimer = new DispatcherTimer() {
					Interval = TimeSpan.FromSeconds(1)
				}; // update time every second

				this._dispatcherTimer.Tick += new EventHandler<object>(this.dispatcherTimerTick);

				this._dispatcherTimer.Start();
			}
			catch (Exception err) {
				// TODO: show error
				Debug.WriteLine(err);
			}
		}

		private void dispatcherTimerTick(object sender, object e) {
			this._timeToEvent -= TimeSpan.FromSeconds(1); // negate 1 second from timer
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RelativeTimeMessage")); // update view
		}

		public Uri QueryUrl { get; private set; }

		public string RelativeTimeMessage => $"in {this._timeToEvent.Hours} hours {this._timeToEvent.Minutes} minutes and {this._timeToEvent.Seconds} seconds";
	}
}
