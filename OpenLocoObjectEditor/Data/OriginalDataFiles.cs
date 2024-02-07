namespace OpenLocoObjectEditor
{
	public static class OriginalDataFiles
	{
		public static readonly Dictionary<string, string> Music = new()
		{
			{ "chrysanthemum.dat", "Chrysanthemum" },
			{ "eugenia.dat", "Eugenia" },
			{ "rag1.dat", "Easy Winners" },
			{ "rag2.dat", "The Ragtime Dance" },
			{ "rag3.dat", "Solace" },
			//
			{ "20s1.dat", "Chuggin' Along" },
			{ "20s2.dat", "Long Dusty Road" },
			{ "20s3.dat", "Setting Off" },
			{ "20s4.dat", "Flying High" },
			{ "20s5.dat", "Get Me To Gladstone Bay" },
			{ "20s6.dat", "Sandy Track Blues" },
			{ "40s1.dat", "A Traveller's Serenade" }, // todo: in loco its misspelt - Seranade
			{ "40s2.dat", "Latino Trip" },
			{ "40s3.dat", "Head To The Bop" },
			{ "50s1.dat", "Gettin' On The Gas" },
			{ "50s2.dat", "Jumpin' The Rails" },
			{ "50s3.dat", "A Good Head Of Steam" },
			{ "60s1.dat", "Steamin' Down Town" },
			{ "60s2.dat", "Mo' Station" },
			{ "60s3.dat", "Far Out" },
			{ "70s1.dat", "Smooth Running" },
			{ "70s2.dat", "Traffic Jam" },
			{ "70s3.dat", "Never Stop 'til You Get There" },
			{ "80s1.dat", "Soaring Away" },
			{ "80s2.dat", "The City Lights" },
			{ "80s3.dat", "Bright Expectations" },
			{ "80s4.dat", "Running On Time" },
			{ "90s1.dat", "Techno Torture" },
			{ "90s2.dat", "Everlasting High-Rise" },
		};

		public const string SoundEffect = "css1.dat";

		//public static readonly Dictionary<string, string> SoundEffect = new()
		//{
		//	{ "css1.dat", "Sound Effects" },
		//};

		public static readonly Dictionary<string, string> MiscellaneousTracks = new()
		{
			{ "css2.dat", "Ambient Wind" },
			{ "css3.dat", "Ambient Ocean" },
			{ "css4.dat", "Ambient Forest" },
			{ "css5.dat", "Main Theme" },
		};

		public static readonly HashSet<string> Tutorials =
		[
			"tut1024_1.dat",
			"tut1024_2.dat",
			"tut1024_3.dat",
			"tut800_1.dat",
			"tut800_2.dat",
			"tut800_3.dat",
		];

		public static readonly HashSet<string> Uncategorised =
		[
			"data/g1.dat",
			"plugin.dat",
			"plugin2.dat",
			"game.cfg",
			"data/kanji.dat",
			"data/title.dat",
			"scores.dat",
			//"scenarios/boulder breakers.sc5",
			//"openloco.yml",
			//"language",
			//"save",
			//"save/autosave",
			//"1.tmp",
			//"objdata",
			//"scenarios",
		];
	}
}
