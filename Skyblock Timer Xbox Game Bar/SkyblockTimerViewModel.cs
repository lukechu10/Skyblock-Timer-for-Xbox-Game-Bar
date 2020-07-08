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

		private TimeSpan _timeToEvent;
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
			private set {
				this._relativeTimeMessage = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RelativeTimeMessage"));
			}
		}

		public SkyblockTimerViewModel(string name, Uri queryUrl, string imageSource = "Assets/StoreLogo.png") {
			this.Name = name;
			this.QueryUrl = queryUrl;
			this.RelativeTimeMessage = "Loading...";
			this.ImageSource = imageSource;

			_ = this.SetupTimer();
		}

		/// <summary>
		/// Queries the url specified by <c>QueryUrl</c> and starts the timer with the returned time
		/// </summary>
		private async Task SetupTimer() {
			try {
				string responseStr = await _httpClient.GetStringAsync(this.QueryUrl);

				ResponseSchema response = JsonConvert.DeserializeObject<ResponseSchema>(responseStr);
				long estimateTimestamp = response.estimate; // unix time stamp for next spawn estimate
				DateTime dateTime = DateTimeOffset.FromUnixTimeMilliseconds(estimateTimestamp).UtcDateTime;
				this._timeToEvent = dateTime - DateTime.UtcNow;

				this._dispatcherTimer = new DispatcherTimer() {
					Interval = TimeSpan.FromSeconds(1)
				}; // update time every second

				this._dispatcherTimer.Tick += (object sender, object e) => {
					this._timeToEvent -= TimeSpan.FromSeconds(1); // negate 1 second from timer
					this.RelativeTimeMessage = $"in about {this._timeToEvent.Hours} hours {this._timeToEvent.Minutes} minutes and {this._timeToEvent.Seconds} seconds";
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
