using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Skyblock_Timer_Xbox_Game_Bar {
	public sealed class SkyblockTimerViewModel : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;

		public TimeSpan TimeToEvent;
		private DispatcherTimer _dispatcherTimer;
		private static readonly HttpClient _httpClient = new HttpClient();

		/// <summary>
		/// Name of the timer (e.g. "Magma Boss")
		/// </summary>
		public string Name { get; private set; }
		public Uri QueryUrl { get; private set; }
		public string ImageSource { get; private set; }

		private string _relativeTimeMessage;
		public string RelativeTimeMessage {
			get => this._relativeTimeMessage;
			set {
				this._relativeTimeMessage = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RelativeTimeMessage"));
			}
		}

		public SkyblockTimerViewModel(string name, Uri queryUrl, string imageSource = "Assets/StoreLogo.png") {
			this.Name = name;
			this.QueryUrl = queryUrl;
			this.RelativeTimeMessage = "Loading...";
			this.ImageSource = imageSource;

			_ = this.RefreshTimerWithServer();
		}

		/// <summary>
		/// Queries the url specified by <c>QueryUrl</c> and starts the timer with the returned time
		/// </summary>
		public async Task RefreshTimerWithServer() {
			try {
				this._dispatcherTimer?.Stop(); // stop current timer if exists

				string responseStr = await _httpClient.GetStringAsync(this.QueryUrl);

				ResponseSchema response = JsonConvert.DeserializeObject<ResponseSchema>(responseStr);
				long estimateTimestamp = response.estimate; // unix time stamp for next spawn estimate
				DateTime dateTime = DateTimeOffset.FromUnixTimeMilliseconds(estimateTimestamp).UtcDateTime;
				this.TimeToEvent = dateTime - DateTime.UtcNow;

				this._dispatcherTimer = new DispatcherTimer() {
					Interval = TimeSpan.FromSeconds(1)
				}; // update time every second

				this._dispatcherTimer.Tick += (object sender, object e) => {
					this.TimeToEvent -= TimeSpan.FromSeconds(1); // negate 1 second from timer

					if (this.TimeToEvent.TotalSeconds <= 0) {
						_ = this.RefreshTimerWithServer(); // get next time from server
					}
					else {
						this.RelativeTimeMessage = $"in about {this.TimeToEvent.Hours} hours {this.TimeToEvent.Minutes} minutes and {this.TimeToEvent.Seconds} seconds";
					}
				};

				this._dispatcherTimer.Start();
			}
			catch (HttpRequestException err) {
				Debug.WriteLine(err);
				this.RelativeTimeMessage = "Error connecting to server";
			}
			catch (Exception err) {
				Debug.WriteLine(err);
				this.RelativeTimeMessage = "An unknown error occured";
			}
		}
	}
}
