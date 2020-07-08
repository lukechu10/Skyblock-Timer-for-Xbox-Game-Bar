using System;

namespace Skyblock_Timer_Xbox_Game_Bar {
	public sealed class Constants {
		public static readonly Uri BASE_URL = new Uri("https://hypixel-api.inventivetalent.org");

		public static readonly Uri MAGMA_ESTIMATE_URL = new Uri(BASE_URL, "/api/skyblock/bosstimer/magma/estimatedSpawn");
	}
}
