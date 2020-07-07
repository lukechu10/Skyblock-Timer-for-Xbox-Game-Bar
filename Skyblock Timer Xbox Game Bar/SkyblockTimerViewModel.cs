using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;

namespace Skyblock_Timer_Xbox_Game_Bar {
	class SkyblockTimerViewModel : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;

		private TimeSpan _timeToEvent;
		private readonly DispatcherTimer _dispatcherTimer;

		public SkyblockTimerViewModel() {
			this._timeToEvent = TimeSpan.FromMinutes(60); // placeholder

			this._dispatcherTimer = new DispatcherTimer() {
				Interval = TimeSpan.FromSeconds(1)
			}; // update time every second

			this._dispatcherTimer.Tick += new EventHandler<object>(dispatcherTimerTick);

			this._dispatcherTimer.Start();
		}

		private void dispatcherTimerTick(object sender, object e) {
			this.TimeToEvent -= TimeSpan.FromSeconds(1); // negate 1 second from timer
			Debug.WriteLine(this._timeToEvent.TotalSeconds);
		}

		public TimeSpan TimeToEvent {
			set {
				this._timeToEvent = value;
				//this.OnPropertyChanged();
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TimeToEvent"));
			}
			get => this._timeToEvent;
		}

		protected void OnPropertyChanged([CallerMemberName] string name = null) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
