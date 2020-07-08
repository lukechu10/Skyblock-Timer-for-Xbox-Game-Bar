using System;

namespace Skyblock_Timer_Xbox_Game_Bar {
	public sealed class Constants {
		public static readonly Uri BASE_URL = new Uri("https://hypixel-api.inventivetalent.org");

		public static readonly Uri MAGMA_ESTIMATE_URL = new Uri(BASE_URL, "/api/skyblock/bosstimer/magma/estimatedSpawn");
		public static readonly Uri NEWYEAR_ESTIMATE_URL = new Uri(BASE_URL, "/api/skyblock/newyear/estimate");
		public static readonly Uri ZOO_ESTIMATE_URL = new Uri(BASE_URL, "/api/skyblock/zoo/estimate");
		public static readonly Uri WINTER_ESTIMATE_URL = new Uri(BASE_URL, "/api/skyblock/winter/estimate");
		public static readonly Uri JERRYS_WORKSHOP_ESTIMATE_URL = new Uri(BASE_URL, "/api/skyblock/jerryWorkshop/estimate");
		public static readonly Uri SPOOKY_FESTIVAL_ESTIMATE_URL = new Uri(BASE_URL, "/api/skyblock/spookyFestival/estimate");
		public static readonly Uri DARK_AUCTION_ESTIMATE_URL = new Uri(BASE_URL, "/api/skyblock/darkauction/estimate");
		public static readonly Uri BROOD_MOTHER_ESTIMATE_URL = new Uri(BASE_URL, "/api/skyblock/broodmother/estimate");
		public static readonly Uri INTEREST_ESTIMATE_URL = new Uri(BASE_URL, "/api/skyblock/bank/interest/estimate");
	}
}
