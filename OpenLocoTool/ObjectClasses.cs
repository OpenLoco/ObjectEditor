namespace OpenLocoTool
{
	public static class ObjectClasses
	{
		private static int descnumuntil() => 0x01000000;
		private static int descnumand(int x, int y) => 0x02000000 | (((y) & 0xff) << 16) | ((-x) & 0xffff);
		private static int descnumif(int x) => 0x03000000 | ((-x) & 0xffff);
		private static int descnumspec(int x) => 0x04000000 | ((x) & 0xffff);

		// for bits whose meaning is not predefined
		//const char* nobits[] = { NULL };
		private static List<string> nobits = null;


		// ***********************
		//  SIMPLE CLASS HANDLER
		// ***********************

		private static varinf[] simplevars = new varinf[]
		{
			new varinf(0x00, 1, 6, string.Empty),
		};

		private static objdesc[] simpledesc = new objdesc[]
		{
			new objdesc(desctype.objdata),
			new objdesc(desctype.lang, new int[]{ 0 }),
			new objdesc(desctype.sprites),
			new objdesc(desctype.END),
		};

		// ***********************
		// Class 00:  INTERFACES
		// ***********************

		private static varinf[] interfacevars = new varinf[]
		{
			new varinf(0x0, 1, 24, string.Empty),
			new varinf(-1),
		};


		// ***********************
		// Class 01:  SOUNDS
		// ***********************

		private static varinf[] sfxvars = new varinf[]
		{
			new varinf( 0x00, 1, 8, string.Empty ),
			new varinf( 0x08,-4, 1, string.Empty ),
			new varinf( -1 ),
		};

		private static objdesc[] sfxdesc = new objdesc[]
		{
			new objdesc(desctype.objdata),
			new objdesc( desctype.lang, new int[]{ 0 } ),
			new objdesc(desctype.sounds),
			new objdesc(desctype.END),
		};


		// ***********************
		// Class 02:  CURRENCIES
		// ***********************

		private static varinf[] currvars = new varinf[]
		{
			new varinf( 0x00, 1, 10, string.Empty ),
			new varinf( 0x0A, 1, 1, "zeroes" ),
			new varinf( 0x0B, 1, 1, "shiftnum" ),
			new varinf( -1 ),
		};

		private static objdesc[] currdesc = new objdesc[]
		{
			new objdesc(desctype.objdata),
			new objdesc( desctype.lang, new int[]{ 0 } ),
			new objdesc(desctype.lang, new int[]{ 1 } ),
			new objdesc(desctype.lang, new int[]{ 2 } ),
			new objdesc(desctype.sprites),
			new objdesc(desctype.END),
		};


		// ***********************
		// Class 03:  EXHAUST FX
		// ***********************

		private static varinf[] exhfxvars = new varinf[]
		{
			new varinf( 0x00, 1, 30, string.Empty ),
			new varinf( 0x1E, 1, 1, "numsnd" ),
			new varinf( 0x1F, 1, 9, string.Empty ),
			new varinf( -1 ),
		};

		private static auxdesc[] exhfxaux = new auxdesc[2]
		{
			new auxdesc(string.Empty, null),
			new auxdesc(string.Empty, null),
		};

		private static objdesc[] exhfxdesc = new objdesc[]
		{
			new objdesc(desctype.objdata),
			new objdesc( desctype.lang, new int[]{ 0 } ),
			new objdesc(desctype.auxdatavar, new int[]{ 0, 0, 2, 1 } ),
			new objdesc(desctype.auxdatavar, new int[]{ 1, 0, 2, 1 } ),
			new objdesc(desctype.useobj, new int[]{ -0x1e, (int)useobjid.ob_soundeffect, 0x01, -1 } ),
			new objdesc(desctype.sprites),
			new objdesc(desctype.END),
		};


		// ***********************
		// Class 04:  CLIFF FACES
		// ***********************

		// (see simple vars above)


		// ***********************
		// Class 05:   WATER
		// ***********************

		private static varinf[] watervars = new varinf[]
		{
			new varinf( 0x00, 1, 2, string.Empty ),
			new varinf( 0x02, 1, 1, "costind" ),
			new varinf( 0x03, 1, 1, string.Empty ),
			new varinf( 0x04,-2, 1, "costfactor" ),
			new varinf( 0x06, 1, 8, string.Empty ),
			new varinf( -1 ),
		};

		// ***********************
		// Class 06:  GROUND
		// ***********************

		private static varinf[] groundvars = new varinf[]
		{
			new varinf( 0x00, 1, 2, string.Empty ),
			new varinf( 0x02, 1, 1, "costind" ),
			new varinf( 0x03, 1, 5, string.Empty ),
			new varinf( 0x08,-2, 1, "costfactor" ),
			new varinf( 0x0A, 1, 20, string.Empty ),
			new varinf( -1 ),
		};

		private static objdesc[] grounddesc = new objdesc[]
		{
			new objdesc(desctype.objdata),
			new objdesc( desctype.lang, new int[]{ 0 } ),
			new objdesc(desctype.useobj, new int[]{ 0, (int)useobjid.ob_cliff, 0x04, -1 } ),
			new objdesc(desctype.sprites),
			new objdesc(desctype.END),
		};


		// ***********************
		// Class 07:  TOWN NAMES
		// ***********************

		private static varinf[] townpart = new varinf[]
		{
			new varinf(0x00, 1, 1, "num"),
			new varinf(0x01, 1, 1, "numempty"),
			new varinf(0x02, 2, 1, "indexofs"),
			new varinf(-1 ),
		};

		private static varinf[] townvars = new varinf[]
		{
			new varinf(0x00, 1, 2, string.Empty),
			new varinf(0x02, 4, 6, "part", townpart),
			new varinf(-1 ),
		};

		private static objdesc[] towndesc = new objdesc[]
		{
			new objdesc(desctype.objdata),
			new objdesc( desctype.lang, new int[]{ 0 } ),
			new objdesc(desctype.strtable, new int[]{ 0, 2 + 0 * 4, 4 + 0 * 4 }),
			new objdesc(desctype.strtable, new int[]{ 1, 2 + 1 * 4, 4 + 1 * 4 }),
			new objdesc(desctype.strtable, new int[]{ 2, 2 + 2 * 4, 4 + 2 * 4 }),
			new objdesc(desctype.strtable, new int[]{ 3, 2 + 3 * 4, 4 + 3 * 4 }),
			new objdesc(desctype.strtable, new int[]{ 4, 2 + 4 * 4, 4 + 4 * 4 }),
			new objdesc(desctype.strtable, new int[]{ 5, 2 + 5 * 4, 4 + 5 * 4 }),
			new objdesc(desctype.END),
		};


		// ***********************
		// Class 08:  CARGO TYPES
		// ***********************

		private static List<string> cargoflags = new()
		{
			string.Empty,
			"refitoption",
			null,
		};

		private static varinf[] cargovars = new varinf[]
		{
			new varinf( 0x00, 1, 2, string.Empty ),
			new varinf( 0x02, 2, 1, "unitweight" ),
			new varinf( 0x04, 1, 12, string.Empty ),
			new varinf( 0x10, 1, 1, "id" ),
			new varinf( 0x11, 1, 1, string.Empty ),
			new varinf( 0x12, 1, 1, "flags", null, cargoflags ),
			new varinf( 0x13, 1, 2, string.Empty ),
			new varinf( 0x15, 1, 1, "peakdays" ),
			new varinf( 0x16, 1, 1, "decay1days" ),
			new varinf( 0x17, 2, 1, "decay1rate" ),
			new varinf( 0x19, 2, 1, "decay2rate" ),
			new varinf( 0x1B, 2, 1, "paymentfactor" ),
			new varinf( 0x1D, 1, 1, "paymentind" ),
			new varinf( 0x1E, 1, 1, string.Empty ),
			new varinf( -1 ),
		};

		private static objdesc[] cargodesc = new objdesc[]
		{
			new objdesc(desctype.objdata),
			new objdesc( desctype.lang, new int[]{ 0 } ),
			new objdesc(desctype.lang, new int[]{ 1 } ),
			new objdesc(desctype.lang, new int[]{ 2 } ),
			new objdesc(desctype.lang, new int[]{ 3 } ),
			new objdesc(desctype.sprites),
			new objdesc(desctype.END),
		};


		// ***********************
		// Class 09:    FENCES
		// ***********************

		private static List<string> fenceflags = new()
		{
			null,
		};

		private static varinf[] fencevars = new varinf[]
		{
			new varinf( 0x00, 1, 7, string.Empty ),
			new varinf( 0x07, 1, 1, "flags", null, fenceflags ),
			new varinf( 0x08, 1, 1, string.Empty ),
			new varinf( 0x09, 1, 1, string.Empty ),
			new varinf( -1 ),
		};


		// ***********************
		// Class 0A:    SIGNALS
		// ***********************

		private static varinf[] signalvars = new varinf[]
		{
			new varinf( 0x00, 1, 6, string.Empty ),
			new varinf( 0x06,-2, 1, "costfactor" ),
			new varinf( 0x08,-2, 1, "sellcostfactor" ),
			new varinf( 0x0A, 1, 1, "costind" ),
			new varinf( 0x0B, 1, 15, string.Empty ),
			new varinf( 0x1A, 2, 1, "designed" ),
			new varinf( 0x1C, 2, 1, "obsolete" ),
			new varinf( -1 ),
		};

		private static objdesc[] signaldesc = new objdesc[]
		{
			new objdesc(desctype.objdata),
			new objdesc( desctype.lang, new int[]{ 0 } ),
			new objdesc(desctype.lang, new int[]{ 1 } ),
			new objdesc(desctype.sprites),
			new objdesc(desctype.END),
		};


		// ***********************
		// Class 0B:  CROSSINGS
		// ***********************

		private static varinf[] crossingvars = new varinf[]
		{
			new varinf( 0x00, 1, 2, string.Empty ),
			new varinf( 0x02,-2, 1, "costfactor" ),
			new varinf( 0x04, 1, 2, string.Empty ),
			new varinf( 0x06, 1, 1, "costind" ),
			new varinf( 0x07, 1, 11, string.Empty ),
			new varinf( -1 ),
		};


		// ***********************
		// Class 0C: STREET LIGHTS
		// ***********************

		private static varinf[] lightvars = new varinf[]
		{
			new varinf( 0x00, 1, 2, string.Empty ),
			new varinf( 0x02, 2, 3, "year" ),
			new varinf( 0x08, 1, 4, string.Empty ),
			new varinf( -1 ),
		};


		// ***********************
		// Class 0D:    TUNNELS
		// ***********************

		// (see simplevars above)


		// ***********************
		// Class 0E:   BRIDGES
		// ***********************

		private static varinf[] bridgevars = new varinf[]
		{
			new varinf( 0x00, 1, 2, string.Empty ),
			new varinf( 0x02, 1, 1, "noroof" ),
			new varinf( 0x03, 1, 1, string.Empty ),
			new varinf( 0x04, 1, 1, string.Empty ),
			new varinf( 0x05, 1, 1, string.Empty ),
			new varinf( 0x06, 1, 1, string.Empty ),
			new varinf( 0x07, 1, 1, string.Empty ),
			new varinf( 0x08, 1, 1, "spanlength" ),
			new varinf( 0x09, 1, 1, "pillarspacing" ),
			new varinf( 0x0A, 2, 1, "maxspeed" ),
			new varinf( 0x0C, 1, 1, "maxheight" ),
			new varinf( 0x0D, 1, 1, "costind" ),
			new varinf( 0x0E,-2, 1, "basecostfact" ),
			new varinf( 0x10,-2, 1, "heightcostfact" ),
			new varinf( 0x12,-2, 1, "sellcostfact" ),
			new varinf( 0x14, 2, 1, "disabledtrackcfg" ),
			new varinf( 0x16, 1,22, string.Empty ),
			new varinf( -1 ),
		};


		// ***********************
		// Class 0F:  TRAIN STATIONS
		// ***********************

		private static List<string> track_pieces = new()
		{
			"diagonal",
			"widecurve",
			"mediumcurve",
			"smallcurve",   // 1 2 4 8
			"tightcurve",
			"normalslope",
			"steepslope",
			string.Empty,       // 10 20 40 80
			"slopedcurve",
			"sbend",                    // 100 200
			null,
		};

		private static varinf[] trnstatvars = new varinf[]
		{
			new varinf( 0x00, 1, 4, string.Empty ),
			new varinf( 0x04, 2, 1, "trackpieces", null, track_pieces ),
			new varinf( 0x06,-2, 1, "buildcostfact" ),
			new varinf( 0x08,-2, 1, "sellcostfact" ),
			new varinf( 0x0A, 1, 1, "costind" ),
			new varinf( 0x0B, 1,23, string.Empty ),
			new varinf( 0x22, 1, 1, "numcompat" ),
			new varinf( 0x23, 1, 7, string.Empty ),
			new varinf( 0x2a, 2, 1, "designed" ),
			new varinf( 0x2c, 2, 1, "obsolete" ),
			new varinf( 0x2e, 1, 128, string.Empty ),
			new varinf( -1 ),
		};

		private static auxdesc[] trnstataux = new auxdesc[]
		{
			new auxdesc( string.Empty, null ),
		};

		private static objdesc[] trnstatdesc = new objdesc[]
		{
			new objdesc(desctype.objdata),
			new objdesc( desctype.lang, new int[]{ 0 } ),
			new objdesc(desctype.auxdatavar, new int[]{ 0, 32, 1, 4 } ),
			new objdesc(desctype.sprites),
			new objdesc(desctype.END),
		};


		// ***********************
		// Class 10:  TRACK MODS
		// ***********************

		private static varinf[] trkmodvars = new varinf[]
		{
			new varinf( 0x00, 1, 2, string.Empty ),
			new varinf( 0x02, 2, 1, "trackpieces", null, track_pieces ),
			new varinf( 0x04, 1, 1, "isoverhead" ),
			new varinf( 0x05, 1, 1, "costind" ),
			new varinf( 0x06,-2, 1, "buildcostfact" ),
			new varinf( 0x08,-2, 1, "sellcostfact" ),
			new varinf( 0x0A, 1, 8, string.Empty ),
			new varinf( -1 ),
		};


		// ***********************
		// Class 11:   TRACKS
		// ***********************

		private static varinf[] trackvars = new varinf[]
		{
			new varinf( 0x00, 1, 2, string.Empty ),
			new varinf( 0x02, 2, 1, "trackpieces", null, track_pieces ),
			new varinf( 0x04, 2, 1, "stationtrackpieces", null, track_pieces ),
			new varinf( 0x06, 1, 1, string.Empty ),
			new varinf( 0x07, 1, 1, "numcompat" ),
			new varinf( 0x08, 1, 1, "nummods" ),
			new varinf( 0x09, 1, 1, "numsignals" ),
			new varinf( 0x0A, 1, 10, string.Empty ),
			new varinf( 0x14,-2, 1, "buildcostfact" ),
			new varinf( 0x16,-2, 1, "sellcostfact" ),
			new varinf( 0x18,-2, 1, "tunnelcostfact" ),
			new varinf( 0x1A, 1, 1, "costind" ),
			new varinf( 0x1B, 1, 1, string.Empty ),
			new varinf( 0x1C, 2, 1, "curvespeed" ),
			new varinf( 0x1E, 1, 6, string.Empty ),
			new varinf( 0x24, 1, 1, "numbridges" ),
			new varinf( 0x25, 1, 7, string.Empty ),
			new varinf( 0x2C, 1, 1, "numstations" ),
			new varinf( 0x2D, 1, 7, string.Empty ),
			new varinf( 0x34, 1, 1, "displayoffset" ),
			new varinf( 0x35, 1, 1, string.Empty ),
			new varinf( -1 ),
		};

		private static objdesc[] trackdesc = new objdesc[]
		{
			new objdesc(desctype.objdata),
			new objdesc( desctype.lang, new int[]{ 0 } ),
			new objdesc(desctype.useobj, new int[]{ -7, (int)useobjid.ob_compatible, 0x11, 0x14, -1 } ),
			new objdesc(desctype.useobj, new int[]{ -8, (int)useobjid.ob_trackmod, 0x10, -1 } ),
			new objdesc(desctype.useobj, new int[]{ -9, (int)useobjid.ob_signal, 0x0A, -1 } ),
			new objdesc(desctype.useobj, new int[]{  0, (int)useobjid.ob_tunnel, 0x0D, -1 } ),
			new objdesc(desctype.useobj, new int[]{ -0x24, (int)useobjid.ob_bridge, 0x0E, -1 } ),
			new objdesc(desctype.useobj, new int[]{ -0x2C, (int)useobjid.ob_station, 0x0F, -1 } ),
			new objdesc(desctype.sprites),
			new objdesc(desctype.END),
		};


		// ***********************
		// Class 12:  ROAD STATIONS
		// ***********************

		private static List<string> road_pieces = new()
		{
			"smallcurve",
			"tightcurve",
			"normalslope",
			"steepslope",
			string.Empty,
			"reverse",
			null,
		};

		private static varinf[] roadstvars = new varinf[]
		{
			new varinf( 0x00, 1, 4, string.Empty ),
			new varinf( 0x04, 2, 1, "roadpieces", null, road_pieces ),
			new varinf( 0x06,-2, 1, "buildcostfact" ),
			new varinf( 0x08,-2, 1, "sellcostfact" ),
			new varinf( 0x0A, 1, 1, "costind" ),
			new varinf( 0x0B, 1,22, string.Empty ),
			new varinf( 0x21, 1, 1, "numcompat" ),
			new varinf( 0x22, 1, 6, string.Empty ),
			new varinf( 0x28, 2, 1, "designed" ),
			new varinf( 0x2a, 2, 1, "obsolete" ),
			new varinf( 0x2c, 1,66, string.Empty ),
			new varinf( -1 ),
		};

		private static auxdesc[] roadstaux = new auxdesc[]
		{
			new auxdesc(string.Empty, null),
		};

		private static objdesc[] roadstdesc = new objdesc[]
		{
			new objdesc(desctype.objdata),
			new objdesc( desctype.lang, new int[]{ 0 } ),
			new objdesc(desctype.useobj, new int[]{ 0, (int)useobjid.ob_cargo, 0x08, -1 } ),
			new objdesc(desctype.auxdatavar, new int[]{ 0, 16, 1, 4 } ),
			new objdesc(desctype.sprites),
			new objdesc(desctype.END),
		};


		// ***********************
		// Class 13:  ROAD MODS
		// ***********************

		// (same as track mods)


		// ***********************
		// Class 14:    ROADS
		// ***********************

		private static varinf[] roadvars = new varinf[]
		{
			new varinf( 0x00, 1, 2, string.Empty ),
			new varinf( 0x02, 2, 1, "roadpieces", null, road_pieces ),
			new varinf( 0x04,-2, 1, "buildcostfact" ),
			new varinf( 0x06,-2, 1, "sellcostfact" ),
			new varinf( 0x08,-2, 1, "tunnelcostfact" ),
			new varinf( 0x0A, 1, 1, "costind" ),
			new varinf( 0x0B, 1, 9, string.Empty ),
			new varinf( 0x14, 1, 1, "numbridges" ),
			new varinf( 0x15, 1, 8, string.Empty ),
			new varinf( 0x1D, 1, 1, "numstations" ),
			new varinf( 0x1E, 1, 10, string.Empty ),
			new varinf( 0x28, 1, 1, "numcompat" ),
			new varinf( 0x29, 1, 7, string.Empty ),
			new varinf( -1 ),
		};
		private static objdesc[] roaddesc = new objdesc[]
		{
			new objdesc(desctype.objdata),
			new objdesc( desctype.lang, new int[]{ 0 } ),
			new objdesc(desctype.useobj, new int[]{ -40, (int)useobjid.ob_compatible, 0x11, 0x14, -1 } ),
			new objdesc(desctype.useobj, new int[]{ -37, (int)useobjid.ob_roadmod, 0x13, -1 } ),
			new objdesc(desctype.useobj, new int[]{ 0, (int)useobjid.ob_tunnel, 0x0D, -1 } ),
			new objdesc(desctype.useobj, new int[]{ -20, (int)useobjid.ob_bridge, 0x0E, -1 } ),
			new objdesc(desctype.useobj, new int[]{ -28, (int)useobjid.ob_station, 0x12, -1 } ),
			new objdesc(desctype.sprites),
			new objdesc(desctype.END),
		};


		// ***********************
		// Class 15:   AIRPORTS
		// ***********************

		private static varinf[] airport_aux0 = new varinf[]
		{
			new varinf( 0x00, 1, 0, "height" ),
			new varinf( -1 ),
		};

		private static varinf[] airport_aux1 = new varinf[]
		{
			new varinf( 0x00, 2, 0, "frames" ),
			new varinf( -1 ),
		};

		private static varinf[] airport_aux2 = new varinf[]
		{
			new varinf( 0x00, 1, 0, "spriteset" ),
			new varinf( -1 ),
		};

		private static varinf[] layoutvars = new varinf[]
		{
			new varinf( 0x00, 1, 1, "tilenum" ),
			new varinf( 0x01, 1, 1, "rotate" ),
			new varinf( 0x02,-1, 1, "x" ),
			new varinf( 0x03,-1, 1, "y" ),
			new varinf( -1 ),
		};

		private static varinf[] airport_aux3 = new varinf[]
		{
			new varinf( 0x00, 4, 0, "tilepos", layoutvars ),
			new varinf( -1 ),
		};

		private static List<string> airportaux4flags = new()
		{
			"terminal",
			"aircraftend",
			string.Empty,
			"ground",
			"flight",
			"helibegin",
			"aircraftbegin",
			"heliend",
			"touchdown",
			null,
		};

		private static varinf[] airport_aux4 = new varinf[] {
			new varinf( 0x00,-2, 1, "x" ),
			new varinf( 0x02,-2, 1, "y" ),
			new varinf( 0x04,-2, 1, "z" ),
			new varinf( 0x06, 2, 1, "flags", null, airportaux4flags ),
			new varinf( -1 ),
		};

		private static varinf[] airport_aux5 = new varinf[] {
			new varinf( 0x00, 1, 1, string.Empty ),
			new varinf( 0x01, 1, 1, "from" ),
			new varinf( 0x02, 1, 1, "to" ),
			new varinf( 0x03, 1, 1, string.Empty ),
			new varinf( 0x04, 4, 1, "busymask", null, nobits ),
			new varinf( 0x08, 4, 1, string.Empty, null, nobits ),
			new varinf( -1 ),
		};

		private static varinf[] airportvars = new varinf[] {
			new varinf( 0x00, 1, 2, string.Empty ),
			new varinf( 0x02,-2, 1, "buildcostfact" ),
			new varinf( 0x04,-2, 1, "sellcostfact" ),
			new varinf( 0x06, 1, 1, "costind" ),
			new varinf( 0x07, 1, 9, string.Empty ),
			new varinf( 0x10, 2, 1, "allowedplanetypes" ),
			new varinf( 0x12, 1, 1, "numspritesets" ),
			new varinf( 0x13, 1, 1, "numtiles" ),
			new varinf( 0x14, 1,140,string.Empty ),
			new varinf( 0xA0, 4, 1, "2x2tiles", null, nobits ),
			new varinf( 0xA4,-1, 1, "minx" ),
			new varinf( 0xA5,-1, 1, "miny" ),
			new varinf( 0xA6,-1, 1, "maxx" ),
			new varinf( 0xA7,-1, 1, "maxy" ),
			new varinf( 0xA8, 2, 1, "designed" ),
			new varinf( 0xAA, 2, 1, "obsolete" ),
			new varinf( 0xAC, 1, 1, "numnodes" ),
			new varinf( 0xAD, 1, 1, "numedges" ),
			new varinf( 0xAE, 1, 8, string.Empty),
			new varinf( 0xB6, 4, 1, string.Empty, null, nobits ),
			new varinf( -1 ),
		};

		private static auxdesc[] airportaux = new auxdesc[]
		{
			new auxdesc("spriteheight", airport_aux0),
			new auxdesc("spriteanimframes", airport_aux1),
			new auxdesc("tile", airport_aux2),
			new auxdesc("layout", airport_aux3),
			new auxdesc("node", airport_aux4),
			new auxdesc("edge", airport_aux5),
		};

		private static objdesc[] airportdesc = new objdesc[]
		{
			new objdesc(desctype.objdata),
			new objdesc( desctype.lang, new int[]{ 0 } ),
			new objdesc(desctype.auxdata, new int[]{ 0, 0, 1, -0x12 } ),
			new objdesc(desctype.auxdata, new int[]{ 1, 0, -2, -0x12 } ),
			new objdesc(desctype.auxdatavar, new int[]{ 2, -0x13, 1, 1 } ),
			new objdesc(desctype.auxdatavar, new int[]{ 3, 0, -4, 1 } ),
			new objdesc(desctype.auxdata, new int[]{ 4, -0xac, 1, 8 } ),
			new objdesc(desctype.auxdata, new int[]{ 5, -0xad, 1, 12 } ),
			new objdesc(desctype.sprites),
			new objdesc(desctype.END),
		};


		// ***********************
		// Class 16:     DOCKS
		// ***********************

		private static varinf[] dockvars = new varinf[]
		{
			new varinf( 0x00, 1, 2, string.Empty ),
			new varinf( 0x02,-2, 1, "buildcostfact" ),
			new varinf( 0x04,-2, 1, "sellcostfact" ),
			new varinf( 0x06, 1, 1, "costind" ),
			new varinf( 0x07, 1,11, string.Empty ),
			new varinf( 0x12, 1, 1, "numaux01" ),
			new varinf( 0x13, 1, 1, "numaux2ent" ),
			new varinf( 0x14, 1,12, string.Empty ),
			new varinf( 0x20, 2, 1, "designed" ),
			new varinf( 0x22, 2, 1, "obsolete" ),
			new varinf( 0x24, 1, 4, string.Empty ),
			new varinf( -1 ),
		};

		private static auxdesc[] dockaux = new auxdesc[]
		{
			new auxdesc(string.Empty, null),
			new auxdesc(string.Empty, null),
			new auxdesc(string.Empty, null),
		};

		private static objdesc[] dockdesc = new objdesc[]
		{
			new objdesc(desctype.objdata),
			new objdesc( desctype.lang, new int[]{ 0 } ),
			new objdesc(desctype.auxdata, new int[]{ 0, 0, 1, -0x12 } ),
			new objdesc(desctype.auxdata, new int[]{ 1, 0, -2, -0x12 } ),
			new objdesc(desctype.auxdatavar, new int[]{ 2, -0x13, 1, 1 } ),
			new objdesc(desctype.sprites),
			new objdesc(desctype.END),
		};


		// ***********************
		// Class 17:   VEHICLES
		// ***********************


		// the known bits in the vehicle sprite structure flags
		private static List<string> vehspriteflags = new()
		{
			"hassprites",
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			"reversed",
			null
		};

		// the vehicle sprite definition structure
		private static varinf[] vehsprites = new varinf[]
		{
			new varinf( 0x00, 1, 1, "numdir" ),
			new varinf( 0x01, 1, 1, string.Empty ),
			new varinf( 0x02, 1, 1, string.Empty ),
			new varinf( 0x03, 1, 1, "vehtype" ),
			new varinf( 0x04, 1, 1, "numunits" ),
			new varinf( 0x05, 1, 1, string.Empty ),
			new varinf( 0x06, 1, 1, "bogeypos" ),
			new varinf( 0x07, 1, 1, "flags", null, vehspriteflags ),
			new varinf( 0x08, 1, 1, string.Empty ),
			new varinf( 0x09, 1, 1, string.Empty ),
			new varinf( 0x0A, 1, 1, string.Empty ),
			new varinf( 0x0B, 1, 1, string.Empty ),
			new varinf( 0x0C, 1, 1, string.Empty ),
			new varinf( 0x0D, 1, 1, string.Empty ),
			new varinf( 0x0E, 1, 1, "spritenum" ),
			new varinf( 0x0F, 1, 3, string.Empty ),
			new varinf( 0x12, 4, 1, string.Empty ),
			new varinf( 0x16, 4, 1, string.Empty ),
			new varinf( 0x1A, 4, 1, string.Empty ),
			new varinf( -1 ),
		};

		// a vehicle structure whose purpose is mostly unknown
		private static varinf[] vehunk = new varinf[]
		{
			new varinf( 0x00, 1, 1, "length" ),
			new varinf( 0x01, 1, 1, string.Empty ),
			new varinf( 0x02, 1, 1, string.Empty ),
			new varinf( 0x03, 1, 1, string.Empty ),
			new varinf( 0x04, 1, 1, "spriteind" ),
			new varinf( 0x05, 1, 1, string.Empty ),
			new varinf( -1 ),
		};

		// the vehicle flags
		private static List<string> vehflags = new()
		{
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			"rackrail",
			string.Empty,
			string.Empty,
			"anytrack",
			string.Empty,
			"cancouple",
			"dualhead",
			string.Empty,
			"refittable",
			"noannounce",
			null
		};

		// the main vehicle data structure
		private static varinf[] vehvars = new varinf[]
		{
			new varinf( 0x000, 1, 2, string.Empty ),
			new varinf( 0x002, 1, 1, "class" ),
			new varinf( 0x003, 1, 1, "type" ),
			new varinf( 0x004, 1, 1, string.Empty ),
			new varinf( 0x005, 1, 1, string.Empty ),
			new varinf( 0x006, 1, 1, "nummods" ),
			new varinf( 0x007, 1, 1, "costind" ),
			new varinf( 0x008,-2, 1, "costfact" ),	// size -2 means signed word
			new varinf( 0x00A, 1, 1, "reliability" ),
			new varinf( 0x00B, 1, 1, "runcostind" ),
			new varinf( 0x00C,-2, 1, "runcostfact" ),
			new varinf( 0x00E, 1, 1, "colourtype" ),
			new varinf( 0x00F, 1, 1, "numcompat" ),
			new varinf( 0x010, 1,20, string.Empty ),
			new varinf( 0x024, 6, 4, string.Empty, vehunk ),
			new varinf( 0x03C,30, 4, "sprites", vehsprites ),
			new varinf( 0x0B4, 1,36, string.Empty ),
			new varinf( 0x0D8, 2, 1, "power" ),
			new varinf( 0x0DA, 2, 1, "speed" ),
			new varinf( 0x0DC, 2, 1, "rackspeed" ),
			new varinf( 0x0DE, 2, 1, "weight" ),
			new varinf( 0x0E0, 2, 1, "flags", null, vehflags ),
			new varinf( 0x0E2, 1,44, string.Empty ),
			new varinf( 0x10E, 1, 1, "visfxheight" ),
			new varinf( 0x10F, 1, 1, "visfxtype" ),
			new varinf( 0x110, 1, 1, string.Empty ),
			new varinf( 0x111, 1, 1, string.Empty ),
			new varinf( 0x112, 1, 1, "wakefxtype" ),
			new varinf( 0x113, 1, 1, string.Empty ),
			new varinf( 0x114, 2, 1, "designed" ),
			new varinf( 0x116, 2, 1, "obsolete" ),
			new varinf( 0x118, 1, 1, string.Empty ),
			new varinf( 0x119, 1, 1, "startsndtype" ),
			new varinf( 0x11A, 1,64, string.Empty ),
			new varinf( 0x15A, 1, 1, "numsnd" ),
			new varinf( 0x15B, 1, 3, string.Empty ),
			new varinf( -1 ),
		};

		private static objdesc[] vehdesc = new objdesc[]
		{
			new objdesc(desctype.objdata),
			new objdesc( desctype.lang, new int[]{ 0 } ),
			new objdesc(desctype.useobj, new int[]{ descnumspec(0), (int)useobjid.ob_tracktype, 0x11, 0x14, -1 } ),
			new objdesc(desctype.useobj, new int[]{ -6, (int)useobjid.ob_trackmod, 0x10, 0x13, -1 } ),
			new objdesc(desctype.cargo, new int[] { 2 }),
			new objdesc(desctype.useobj, new int[]{ descnumif(0x10f), (int)useobjid.ob_visualeffect, 0x03, -1 } ),
			new objdesc(desctype.useobj, new int[]{ descnumif(0x112), (int)useobjid.ob_wakeeffect, 0x03, -1 } ),
			new objdesc(desctype.useobj, new int[]{ descnumspec(1), (int)useobjid.ob_rackrail, 0x10, -1 } ),
			new objdesc(desctype.useobj, new int[]{ -15, (int)useobjid.ob_compatible, 0x17, -1 } ),
			new objdesc(desctype.useobj, new int[]{ descnumif(0x119), (int)useobjid.ob_startsnd, 0x01, -1 } ),
			new objdesc(desctype.useobj, new int[]{ descnumand(0x15a, 0x7f), (int)useobjid.ob_soundeffect, 0x01, -1 } ),
			new objdesc(desctype.sprites),
			new objdesc(desctype.END),
		};


		// ***********************
		// Class 18:    TREES
		// ***********************

		private static varinf[] treevars = new varinf[]
		{
			new varinf( 0x00, 1, 3, string.Empty ),
			new varinf( 0x03, 1, 1, "height" ),
			new varinf( 0x04, 1,59, string.Empty ),
			new varinf( 0x3F, 1, 1, "costind" ),
			new varinf( 0x40,-2, 1, "buildcostfact" ),
			new varinf( 0x42,-2, 1, "clearcostfact" ),
			new varinf( 0x44, 1, 8, string.Empty ),
			new varinf( -1 ),
		};


		// ***********************
		// Class 19:    SNOW
		// ***********************

		// (see simple vars above)


		// ***********************
		// Class 1A:   CLIMATES
		// ***********************

		private static varinf[] climvars = new varinf[]
		{
			new varinf( 0x00, 1, 2, string.Empty ),
			new varinf( 0x02, 1, 1, "firstseason" ),
			new varinf( 0x03, 1, 4, "seasonlength" ),
			new varinf( 0x07, 1, 3, string.Empty ),
			new varinf( -1 ),
		};

		private static objdesc[] climdesc = new objdesc[]
		{
			new objdesc(desctype.objdata),
			new objdesc( desctype.lang, new int[]{ 0 } ),
			new objdesc(desctype.END),
		};


		// ***********************
		// Class 1B:   SHAPES
		// ***********************

		private static varinf[] shapevars = new varinf[]
		{
			new varinf( 0x00, 1, 14, string.Empty ),
			new varinf( -1 ),
		};


		// ***********************
		// Class 1C:   BUILDINGS
		// ***********************

		private static List<string> bldg_flags = new()
		{
			string.Empty,
			string.Empty,
			string.Empty,
			"ishq", // 1 2 4 8
			null,
		};

		private static varinf[] bldngvars = new varinf[]
		{
			new varinf( 0x00, 1, 6, string.Empty ),
			new varinf( 0x06, 1, 1, "numaux01" ),
			new varinf( 0x07, 1, 1, "numaux2ent" ),
			new varinf( 0x08, 1,140,string.Empty ),
			new varinf( 0x94, 2, 1, "earliestyr" ),
			new varinf( 0x96, 2, 1, "latestyr" ),
			new varinf( 0x98, 1, 1, "flags", null, bldg_flags ),
			new varinf( 0x99, 1, 1, "costind" ),
			new varinf( 0x9A,-2, 1, "clearcostfact" ),
			new varinf( 0x9C, 1, 4, string.Empty ),
			new varinf( 0xA0, 1, 2, "numproduce" ),
			new varinf( 0xA2, 1, 4, string.Empty ),
			new varinf( 0xA6, 1, 4, "numaccept" ),
			new varinf( 0xAA, 1, 3, string.Empty ),
			new varinf( 0xAD, 1, 1, "numaux3ent" ),
			new varinf( 0xAE, 1,16, string.Empty ),
			new varinf( -1 ),
		};

		private static auxdesc[] bldngaux = new auxdesc[]
		{
			new auxdesc(string.Empty, null),
			new auxdesc(string.Empty, null),
			new auxdesc(string.Empty, null),
			new auxdesc(string.Empty, null),
		};

		private static objdesc[] bldngdesc = new objdesc[]
		{
			new objdesc(desctype.objdata),
			new objdesc( desctype.lang, new int[]{ 0 } ),
			new objdesc(desctype.auxdata, new int[]{ 0, 0, 1, -6 } ),
			new objdesc(desctype.auxdata, new int[]{ 1, 0, -2, -6 } ),
			new objdesc(desctype.auxdatavar, new int[]{ 2, -7, 1, 1 } ),
			new objdesc(desctype.useobj, new int[]{ 4, (int)useobjid.ob_cargo, 0x08, 0xFF, -1 } ),
			new objdesc(desctype.auxdatafix, new int[]{ 3, -0xad, 1, 2 } ),
			new objdesc(desctype.sprites),
			new objdesc(desctype.END),
		};


		// ***********************
		// Class 1D:  SCAFFOLDING
		// ***********************

		private static varinf[] scaffvars = new varinf[]
		{
			new varinf( 0x00, 1, 18, string.Empty ),
			new varinf( -1 ),
		};


		// ***********************
		// Class 1E:  INDUSTRIES
		// ***********************

		private static List<string> industryflags = new()
		{
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,       // 1, 2, 4, 8
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,       // 10, 20, 40, 80
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,       // 100, 200, 400, 800
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,       // 1000, 2000, 4000, 8000
			string.Empty,
			"needall",
			"canincreaseproduction",
			"candecreaseproduction",
			// 100000, 200000, 400000, 800000
			null,
		};

		private static varinf[] indvars = new varinf[]
		{
			new varinf( 0x00, 1,30, string.Empty ),
			new varinf( 0x1E, 1, 1, "numaux01" ),
			new varinf( 0x1F, 1, 1, "numaux4ent" ),
			new varinf( 0x20, 1,157,string.Empty ),
			new varinf( 0xBD, 1, 1, "numaux5" ),
			new varinf( 0xBE, 1,12, string.Empty ),
			new varinf( 0xCA, 2, 1, "firstyear" ),
			new varinf( 0xCC, 2, 1, "lastyear" ),
			new varinf( 0xCE, 1, 1, string.Empty ),
			new varinf( 0xCF, 1, 1, "costind" ),
			new varinf( 0xD0,-2, 1, "costfactor1" ),
			new varinf( 0xD2, 1, 18, string.Empty ),
			new varinf( 0xE4, 4, 1, "flags", null, industryflags ),
			new varinf( 0xE8, 1, 12, string.Empty ),
			new varinf( -1 ),
		};

		private static auxdesc[] indaux = new auxdesc[]
		{
			new auxdesc(string.Empty, null),
			new auxdesc(string.Empty, null),
			new auxdesc(string.Empty, null),
			new auxdesc(string.Empty, null),
			new auxdesc(string.Empty, null),
			new auxdesc(string.Empty, null),
		};

		private static objdesc[] inddesc = new objdesc[]
		{
			new objdesc(desctype.objdata),
			new objdesc( desctype.lang, new int[]{ 0 } ),
			new objdesc(desctype.lang, new int[]{ 1 } ),
			new objdesc(desctype.lang, new int[]{ 2 } ),
			new objdesc(desctype.lang, new int[]{ 3 } ),
			new objdesc(desctype.lang, new int[]{ 4 } ),
			new objdesc(desctype.lang, new int[]{ 5 } ),
			new objdesc(desctype.lang, new int[]{ 6 } ),
			new objdesc(desctype.lang, new int[]{ 7 } ),
			new objdesc(desctype.auxdata, new int[]{ 0, 0, 1, -30 } ),
			new objdesc(desctype.auxdata, new int[]{ 1, 0, 2, -30 } ),
			new objdesc(desctype.auxdatafix, new int[]{ 2, 4, 1, 1 }),
			new objdesc(desctype.auxdatavar, new int[]{ 3, 0, 2, 1 }),
			new objdesc(desctype.auxdatavar, new int[]{ 4, -31, 1, 1 }),
			new objdesc(desctype.auxdata, new int[] { 5, 0, 1, -0xbd }),
			new objdesc(desctype.useobj, new int[]{ 2, (int)useobjid.ob_produces, 0x08, 0xFF, -1 }),
			new objdesc(desctype.useobj, new int[]{ 3, (int)useobjid.ob_accepts, 0x08, 0xFF, -1 }),
			new objdesc(desctype.useobj, new int[]{ 6, (int)useobjid.ob_fence, 0x09, 0xFF, -1 }),
			new objdesc(desctype.sprites),
			new objdesc(desctype.END),
		};


		// ***********************
		// Class 1F:   REGIONS
		// ***********************

		private static varinf[] regionvars = new varinf[]
		{
			new varinf( 0x00, 1, 18, string.Empty ),
			new varinf( -1 ),
		};

		private static objdesc[] regiondesc = new objdesc[]
		{
			new objdesc(desctype.objdata),
			new objdesc( desctype.lang, new int[]{ 0 } ),
			new objdesc(desctype.useobj, new int[]{ -8, (int)useobjid.ob_cargo, 0x08, -1 } ),
			new objdesc(desctype.useobj, new int[]{ descnumuntil(), (int)useobjid.ob_default, 0x100, -1 } ),
			new objdesc(desctype.sprites),
			new objdesc(desctype.END),
		};


		// ***********************
		// Class 20:  COMPANIES
		// ***********************

		private static List<string> comp_sprites = new()
		{
			null,
		};

		private static varinf[] compvars = new varinf[]
		{
			new varinf( 0x00, 1,12, string.Empty ),
			new varinf( 0x0C, 1, 2, "spritesets", null, comp_sprites ),
			new varinf( 0x0E, 1,38, string.Empty ),
			new varinf( 0x34, 1, 1, "intelligence" ),
			new varinf( 0x35, 1, 1, "aggressiveness" ),
			new varinf( 0x36, 1, 1, "competitiveness" ),
			new varinf( 0x37, 1, 1, string.Empty ),
			new varinf( -1 ),
		};

		private static objdesc[] compdesc = new objdesc[]
		{
			new objdesc(desctype.objdata),
			new objdesc( desctype.lang, new int[]{ 0 } ),
			new objdesc(desctype.lang, new int[]{ 1 } ),
			new objdesc(desctype.sprites),
			new objdesc(desctype.END),
		};


		// ***********************
		// Class 21:   TEXTS
		// ***********************

		// (uses simple vars)

		private static objdesc[] textdesc = new objdesc[]
		{
			new objdesc(desctype.objdata),
			new objdesc( desctype.lang, new int[]{ 0 } ),
			new objdesc(desctype.lang, new int[]{ 1 } ),
			new objdesc(desctype.END),
		};


		// ***********************
		//  END OF CLASS HANDLERS
		// ***********************

		public static objclass[] ObjectClass = new[]
{
			// structure	size auxdef	description
			new objclass(interfacevars, 24, null, simpledesc),    // 00 Interfaces
			new objclass(sfxvars, 12, null, sfxdesc), // 01 Sound effects
			new objclass(currvars, 12, null, currdesc),   // 02 Currencies
			new objclass(exhfxvars, 40, exhfxaux, exhfxdesc), // 03 Exhaust effects
			new objclass(simplevars, 6, null, simpledesc),    // 04 Cliff faces
			new objclass(watervars, 14, null, simpledesc),    // 05 Water
			new objclass(groundvars, 30, null, grounddesc),   // 06 Ground
			new objclass(townvars, 26, null, towndesc),   // 07 Town names
			new objclass(cargovars, 31, null, cargodesc), // 08 Cargos
			new objclass(fencevars, 10, null, simpledesc),    // 09 Fences
			new objclass(signalvars, 30, null, signaldesc),   // 0A Signals
			new objclass(crossingvars, 18, null, simpledesc), // 0B Crossings
			new objclass(lightvars, 12, null, simpledesc),    // 0C Street lights
			new objclass(simplevars, 6, null, simpledesc),    // 0D Tunnels
			new objclass(bridgevars, 44, null, simpledesc),   // 0E Bridges
			new objclass(trnstatvars, 174, trnstataux, trnstatdesc),  // 0F Train stations
			new objclass(trkmodvars, 18, null, simpledesc),   // 10 Track modifications
			new objclass(trackvars, 54, null, trackdesc), // 11 Tracks
			new objclass(roadstvars, 110, roadstaux, roadstdesc), // 12 Road stations
			new objclass(trkmodvars, 18, null, simpledesc),   // 13 Road modifications
			new objclass(roadvars, 48, null, roaddesc),   // 14 Roads
			new objclass(airportvars, 186, airportaux, airportdesc),  // 15 Airports
			new objclass(dockvars, 40, dockaux, dockdesc),    // 16 Docks
			new objclass(vehvars, 350, null, vehdesc),    // 17 Vehicles
			new objclass(treevars, 76, null, simpledesc), // 18 Trees
			new objclass(simplevars, 6, null, simpledesc),    // 19 Snow
			new objclass(climvars, 10, null, climdesc),   // 1A Climates
			new objclass(shapevars, 14, null, simpledesc),    // 1B Shapes
			new objclass(bldngvars, 190, bldngaux, bldngdesc),    // 1C bldngs
			new objclass(scaffvars, 18, null, simpledesc),    // 1D Scaffolding
			new objclass(indvars, 244, indaux, inddesc),  // 1E Industries
			new objclass(regionvars, 18, null, regiondesc),   // 1F Regions
			new objclass(compvars, 56, null, compdesc),   // 20 Companies
			new objclass(simplevars, 6, null, textdesc),  // 21 Texts
		};
	}
}
