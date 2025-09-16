using Definitions.ObjectModels.Objects.Cargo;
using Definitions.ObjectModels.Objects.CliffEdge;
using Definitions.ObjectModels.Objects.Competitor;
using Definitions.ObjectModels.Objects.HillShape;
using Definitions.ObjectModels.Objects.InterfaceSkin;
using Definitions.ObjectModels.Objects.Land;
using Definitions.ObjectModels.Objects.Road;
using Definitions.ObjectModels.Objects.RoadStation;
using Definitions.ObjectModels.Objects.Streetlight;
using Definitions.ObjectModels.Objects.Track;
using Definitions.ObjectModels.Objects.TrackExtra;
using Definitions.ObjectModels.Objects.TrackSignal;
using Definitions.ObjectModels.Objects.TrackStation;
using Definitions.ObjectModels.Objects.Wall;
using Definitions.ObjectModels.Objects.Water;
using Definitions.ObjectModels.Types;
using System.Diagnostics.CodeAnalysis;

namespace Definitions.ObjectModels;

public static class ImageTableNamer
{
	class CliffEdgeObjectNamer : ImageTableNamer<CliffEdgeObject>
	{
		public static CliffEdgeObjectNamer Instance { get; } = new();

		public override bool TryGetImageName(CliffEdgeObject model, int id, [MaybeNullWhen(false)] out string value)
		{
			if (id is >= 0 and <= 63)
			{
				var direction = id / 16 % 2 == 0 ? "west" : "east";
				var side = id is >= 16 and <= 47 ? "right" : "left";
				var level = id % 16;
				value = $"south {direction} {side} {level}";
				return true;
			}

			if (id is >= 64 and <= 69)
			{
				return ImageIdNameMap.TryGetValue(id, out value);
			}

			value = null;
			return false;
		}

		static readonly Dictionary<int, string> ImageIdNameMap = new()
		{
			{ 64, "north west slope 1" },
			{ 65, "north west slope 2" },
			{ 66, "north west slope 3" },
			{ 67, "north east slope 1" },
			{ 68, "north east slope 2" },
			{ 69, "north east slope 3" },
		};
	}

	class TrackSignalObjectNamer : ImageTableNamer<TrackSignalObject>
	{
		public static TrackSignalObjectNamer Instance { get; } = new();

		public override bool TryGetImageName(TrackSignalObject model, int id, [MaybeNullWhen(false)] out string value)
			=> ImageIdNameMap.TryGetValue(id, out value);

		static readonly Dictionary<int, string> ImageIdNameMap = new()
		{
			{ 80, "redLights" },
			{ 88, "redLights2" },
			{ 96, "greenLights" },
			{ 104, "greenLights2" },
		};
	}

	class WaterObjectNamer : ImageTableNamer<WaterObject>
	{
		public static WaterObjectNamer Instance { get; } = new();

		public override bool TryGetImageName(WaterObject model, int id, [MaybeNullWhen(false)] out string value)
			=> ImageIdNameMap.TryGetValue(id, out value);

		static readonly Dictionary<int, string> ImageIdNameMap = new()
		{
			{ 0, "zoom1 wave overlay full" },
			{ 1, "zoom1 wave overlay west" },
			{ 2, "zoom1 wave overlay east" },
			{ 3, "zoom1 wave overlay north" },
			{ 4, "zoom1 wave overlay south" },
			{ 5, "zoom1 wave overlay full" },
			{ 6, "zoom1 wave half-tile west" },
			{ 7, "zoom1 wave half-tile east" },
			{ 8, "zoom1 wave half-tile north" },
			{ 9, "zoom1 wave half-tile south" },
			{ 10, "zoom2 wave overlay full" },
			{ 11, "zoom2 wave overlay west" },
			{ 12, "zoom2 wave overlay east" },
			{ 13, "zoom2 wave overlay north" },
			{ 14, "zoom2 wave overlay south" },
			{ 15, "zoom2 wave overlay full" },
			{ 16, "zoom2 wave half-tile west" },
			{ 17, "zoom2 wave half-tile east" },
			{ 18, "zoom2 wave half-tile north" },
			{ 19, "zoom2 wave half-tile south" },
			{ 20, "zoom3 wave overlay full" },
			{ 21, "zoom3 wave overlay west" },
			{ 22, "zoom3 wave overlay east" },
			{ 23, "zoom3 wave overlay north" },
			{ 24, "zoom3 wave overlay south" },
			{ 25, "zoom3 wave overlay full" },
			{ 26, "zoom3 wave half-tile west" },
			{ 27, "zoom3 wave half-tile east" },
			{ 28, "zoom3 wave half-tile north" },
			{ 29, "zoom3 wave half-tile south" },
			{ 30, "zoom4 wave overlay full" },
			{ 31, "zoom4 wave overlay west" },
			{ 32, "zoom4 wave overlay east" },
			{ 33, "zoom4 wave overlay north" },
			{ 34, "zoom4 wave overlay south" },
			{ 35, "zoom4 wave overlay full" },
			{ 36, "zoom4 wave half-tile west" },
			{ 37, "zoom4 wave half-tile east" },
			{ 38, "zoom4 wave half-tile north" },
			{ 39, "zoom4 wave half-tile south" },
			{ 40, "minimap palette" },
			{ 41, "water colour palette" },
			{ 42, "water icon animation 0" },
			{ 43, "water icon animation 1" },
			{ 44, "water icon animation 2" },
			{ 45, "water icon animation 3" },
			{ 46, "water icon animation 4" },
			{ 47, "water icon animation 5" },
			{ 48, "water icon animation 6" },
			{ 49, "water icon animation 7" },
			{ 50, "water icon animation 8" },
			{ 51, "water icon animation 9" },
			{ 52, "water icon animation 10" },
			{ 54, "water icon animation 11" },
			{ 55, "water icon animation 12" },
			{ 56, "water icon animation 13" },
			{ 57, "water icon animation 14" },
			{ 53, "water icon animation 15" },
			{ 58, "pick up water vehicle" },
			{ 59, "place down water vehicle" },
			{ 60, "water animation frame 0" },
			{ 61, "water animation frame 1" },
			{ 62, "water animation frame 2" },
			{ 63, "water animation frame 3" },
			{ 64, "water animation frame 4" },
			{ 65, "water animation frame 5" },
			{ 66, "water animation frame 6" },
			{ 67, "water animation frame 7" },
			{ 68, "water animation frame 8" },
			{ 69, "water animation frame 9" },
			{ 70, "water animation frame 10" },
			{ 71, "water animation frame 11" },
			{ 72, "water animation frame 12" },
			{ 73, "water animation frame 13" },
			{ 74, "water animation frame 14" },
			{ 75, "water animation frame 15" },
		};
	}

	class WallObjectNamer : ImageTableNamer<WallObject>
	{
		public static WallObjectNamer Instance { get; } = new();

		public override bool TryGetImageName(WallObject model, int id, [MaybeNullWhen(false)] out string value)
		{
			var result = ImageIdNameMap.TryGetValue(id, out value);

			if (id is >= 0 and <= 5 && model.Flags1.HasFlag(WallObjectFlags1.DoubleSided))
			{
				value = $"{value} back";
			}

			if (id is >= 6 and <= 11 && model.Flags1.HasFlag(WallObjectFlags1.HasGlass))
			{
				value = $"{value} glass overlay";
			}
			else
			{
				value = $"{value} front";
			}

			return result;
		}

		static readonly Dictionary<int, string> ImageIdNameMap = new()
		{
			{ 0, "Flat SE" },
			{ 1, "Flat NE" },
			{ 2, "Sloped SE" },
			{ 3, "Sloped NE" },
			{ 4, "Sloped NW" },
			{ 5, "Sloped SW" },
			{ 6, "Flat SE" },
			{ 7, "Flat NE" },
			{ 8, "Sloped SE" },
			{ 9, "Sloped NE" },
			{ 10, "Sloped NW" },
			{ 11, "Sloped SW" },
		};
	}

	class InterfaceSkinObjectNamer : ImageTableNamer<InterfaceSkinObject>
	{
		public static InterfaceSkinObjectNamer Instance { get; } = new();

		public override bool TryGetImageName(InterfaceSkinObject model, int id, [MaybeNullWhen(false)] out string value)
			=> ImageIdNameMap.TryGetValue(id, out value);

		static readonly Dictionary<int, string> ImageIdNameMap = new()
		{
			{ 0, "preview_image" },
			{ 1, "toolbar_pause" },
			{ 2, "toolbar_pause_hover" },
			{ 3, "toolbar_loadsave" },
			{ 4, "toolbar_loadsave_hover" },
			{ 5, "toolbar_zoom" },
			{ 6, "toolbar_zoom_hover" },
			{ 7, "toolbar_rotate" },
			{ 8, "toolbar_rotate_hover" },
			{ 9, "toolbar_terraform" },
			{ 10, "toolbar_terraform_hover" },
			{ 11, "toolbar_audio_active" },
			{ 12, "toolbar_audio_active_hover" },
			{ 13, "toolbar_audio_inactive" },
			{ 14, "toolbar_audio_inactive_hover" },
			{ 15, "toolbar_view" },
			{ 16, "toolbar_view_hover" },
			{ 17, "toolbar_towns" },
			{ 18, "toolbar_towns_hover" },
			{ 19, "toolbar_empty_opaque" },
			{ 20, "toolbar_empty_opaque_hover" },
			{ 21, "toolbar_empty_transparent" },
			{ 22, "toolbar_empty_transparent_hover" },
			{ 23, "toolbar_industries" },
			{ 24, "toolbar_industries_hover" },
			{ 25, "toolbar_airports" },
			{ 26, "toolbar_airports_hover" },
			{ 27, "toolbar_ports" },
			{ 28, "toolbar_ports_hover" },
			{ 29, "toolbar_cogwheels" },
			{ 30, "toolbar_cogwheels_hover" },
			{ 31, "toolbar_build_vehicle_train" },
			{ 32, "toolbar_build_vehicle_train_hover" },
			{ 33, "toolbar_build_vehicle_bus" },
			{ 34, "toolbar_build_vehicle_bus_hover" },
			{ 35, "toolbar_build_vehicle_truck" },
			{ 36, "toolbar_build_vehicle_truck_hover" },
			{ 37, "toolbar_build_vehicle_tram" },
			{ 38, "toolbar_build_vehicle_tram_hover" },
			{ 39, "toolbar_build_vehicle_airplane" },
			{ 40, "toolbar_build_vehicle_airplane_hover" },
			{ 41, "toolbar_build_vehicle_boat" },
			{ 42, "toolbar_build_vehicle_boat_hover" },
			{ 43, "toolbar_stations" },
			{ 44, "toolbar_stations_hover" },
			{ 45, "tab_awards" },
			{ 46, "toolbar_menu_airport" },
			{ 47, "toolbar_menu_ship_port" },
			{ 48, "tab_cargo_ratings" },
			{ 49, "tab_colour_scheme_frame0" },
			{ 50, "tab_colour_scheme_frame1" },
			{ 51, "tab_colour_scheme_frame2" },
			{ 52, "tab_colour_scheme_frame3" },
			{ 53, "tab_colour_scheme_frame4" },
			{ 54, "tab_colour_scheme_frame5" },
			{ 55, "tab_colour_scheme_frame6" },
			{ 56, "tab_colour_scheme_frame7" },
			{ 57, "tab_population_frame0" },
			{ 58, "tab_population_frame1" },
			{ 59, "tab_population_frame2" },
			{ 60, "tab_population_frame3" },
			{ 61, "tab_population_frame4" },
			{ 62, "tab_population_frame5" },
			{ 63, "tab_population_frame6" },
			{ 64, "tab_population_frame7" },
			{ 65, "tab_performance_index_frame0" },
			{ 66, "tab_performance_index_frame1" },
			{ 67, "tab_performance_index_frame2" },
			{ 68, "tab_performance_index_frame3" },
			{ 69, "tab_performance_index_frame4" },
			{ 70, "tab_performance_index_frame5" },
			{ 71, "tab_performance_index_frame6" },
			{ 72, "tab_performance_index_frame7" },
			{ 73, "tab_cargo_units_frame0" },
			{ 74, "tab_cargo_units_frame1" },
			{ 75, "tab_cargo_units_frame2" },
			{ 76, "tab_cargo_units_frame3" },
			{ 77, "tab_cargo_units_frame4" },
			{ 78, "tab_cargo_units_frame5" },
			{ 79, "tab_cargo_units_frame6" },
			{ 80, "tab_cargo_units_frame7" },
			{ 81, "tab_cargo_distance_frame0" },
			{ 82, "tab_cargo_distance_frame1" },
			{ 83, "tab_cargo_distance_frame2" },
			{ 84, "tab_cargo_distance_frame3" },
			{ 85, "tab_cargo_distance_frame4" },
			{ 86, "tab_cargo_distance_frame5" },
			{ 87, "tab_cargo_distance_frame6" },
			{ 88, "tab_cargo_distance_frame7" },
			{ 89, "tab_production_frame0" },
			{ 90, "tab_production_frame1" },
			{ 91, "tab_production_frame2" },
			{ 92, "tab_production_frame3" },
			{ 93, "tab_production_frame4" },
			{ 94, "tab_production_frame5" },
			{ 95, "tab_production_frame6" },
			{ 96, "tab_production_frame7" },
			{ 97, "tab_wrench_frame0" },
			{ 98, "tab_wrench_frame1" },
			{ 99, "tab_wrench_frame2" },
			{ 100, "tab_wrench_frame3" },
			{ 101, "tab_wrench_frame4" },
			{ 102, "tab_wrench_frame5" },
			{ 103, "tab_wrench_frame6" },
			{ 104, "tab_wrench_frame7" },
			{ 105, "tab_wrench_frame8" },
			{ 106, "tab_wrench_frame9" },
			{ 107, "tab_wrench_frame10" },
			{ 108, "tab_wrench_frame11" },
			{ 109, "tab_wrench_frame12" },
			{ 110, "tab_wrench_frame13" },
			{ 111, "tab_wrench_frame14" },
			{ 112, "tab_wrench_frame15" },
			{ 113, "tab_finances_frame0" },
			{ 114, "tab_finances_frame1" },
			{ 115, "tab_finances_frame2" },
			{ 116, "tab_finances_frame3" },
			{ 117, "tab_finances_frame4" },
			{ 118, "tab_finances_frame5" },
			{ 119, "tab_finances_frame6" },
			{ 120, "tab_finances_frame7" },
			{ 121, "tab_finances_frame8" },
			{ 122, "tab_finances_frame9" },
			{ 123, "tab_finances_frame10" },
			{ 124, "tab_finances_frame11" },
			{ 125, "tab_finances_frame12" },
			{ 126, "tab_finances_frame13" },
			{ 127, "tab_finances_frame14" },
			{ 128, "tab_finances_frame15" },
			{ 129, "tab_cup_frame0" },
			{ 130, "tab_cup_frame1" },
			{ 131, "tab_cup_frame2" },
			{ 132, "tab_cup_frame3" },
			{ 133, "tab_cup_frame4" },
			{ 134, "tab_cup_frame5" },
			{ 135, "tab_cup_frame6" },
			{ 136, "tab_cup_frame7" },
			{ 137, "tab_cup_frame8" },
			{ 138, "tab_cup_frame9" },
			{ 139, "tab_cup_frame10" },
			{ 140, "tab_cup_frame11" },
			{ 141, "tab_cup_frame12" },
			{ 142, "tab_cup_frame13" },
			{ 143, "tab_cup_frame14" },
			{ 144, "tab_cup_frame15" },
			{ 145, "tab_ratings_frame0" },
			{ 146, "tab_ratings_frame1" },
			{ 147, "tab_ratings_frame2" },
			{ 148, "tab_ratings_frame3" },
			{ 149, "tab_ratings_frame4" },
			{ 150, "tab_ratings_frame5" },
			{ 151, "tab_ratings_frame6" },
			{ 152, "tab_ratings_frame7" },
			{ 153, "tab_ratings_frame8" },
			{ 154, "tab_ratings_frame9" },
			{ 155, "tab_ratings_frame10" },
			{ 156, "tab_ratings_frame11" },
			{ 157, "tab_ratings_frame12" },
			{ 158, "tab_ratings_frame13" },
			{ 159, "tab_ratings_frame14" },
			{ 160, "tab_ratings_frame15" },
			{ 161, "tab_transported_frame0" },
			{ 162, "tab_transported_frame1" },
			{ 163, "tab_transported_frame2" },
			{ 164, "tab_transported_frame3" },
			{ 165, "tab_transported_frame4" },
			{ 166, "tab_transported_frame5" },
			{ 167, "tab_transported_frame6" },
			{ 168, "tab_cogs_frame0" },
			{ 169, "tab_cogs_frame1" },
			{ 170, "tab_cogs_frame2" },
			{ 171, "tab_cogs_frame3" },
			{ 172, "tab_scenario_details" },
			{ 173, "tab_company" },
			{ 174, "tab_companies" },
			{ 175, "toolbar_menu_zoom_in" },
			{ 176, "toolbar_menu_zoom_out" },
			{ 177, "toolbar_menu_rotate_clockwise" },
			{ 178, "toolbar_menu_rotate_anti_clockwise" },
			{ 179, "toolbar_menu_plant_trees" },
			{ 180, "toolbar_menu_bulldozer" },
			{ 181, "tab_company_details" },
			{ 182, "all_stations" },
			{ 183, "rail_stations" },
			{ 184, "road_stations" },
			{ 185, "airports" },
			{ 186, "ship_ports" },
			{ 187, "toolbar_menu_build_walls" },
			{ 188, "phone" },
			{ 189, "toolbar_menu_towns" },
			{ 190, "toolbar_menu_stations" },
			{ 191, "toolbar_menu_industries" },
			{ 192, "tab_routes_frame_0" },
			{ 193, "tab_routes_frame_1" },
			{ 194, "tab_routes_frame_2" },
			{ 195, "tab_routes_frame_3" },
			{ 196, "tab_messages" },
			{ 197, "tab_message_settings" },
			{ 198, "tab_cargo_delivered_frame0" },
			{ 199, "tab_cargo_delivered_frame1" },
			{ 200, "tab_cargo_delivered_frame2" },
			{ 201, "tab_cargo_delivered_frame3" },
			{ 202, "tab_cargo_payment_rates" },
			{ 203, "tab_vehicle_train_frame0" },
			{ 204, "tab_vehicle_train_frame1" },
			{ 205, "tab_vehicle_train_frame2" },
			{ 206, "tab_vehicle_train_frame3" },
			{ 207, "tab_vehicle_train_frame4" },
			{ 208, "tab_vehicle_train_frame5" },
			{ 209, "tab_vehicle_train_frame6" },
			{ 210, "tab_vehicle_train_frame7" },
			{ 211, "tab_vehicle_aircraft_frame0" },
			{ 212, "tab_vehicle_aircraft_frame1" },
			{ 213, "tab_vehicle_aircraft_frame2" },
			{ 214, "tab_vehicle_aircraft_frame3" },
			{ 215, "tab_vehicle_aircraft_frame4" },
			{ 216, "tab_vehicle_aircraft_frame5" },
			{ 217, "tab_vehicle_aircraft_frame6" },
			{ 218, "tab_vehicle_aircraft_frame7" },
			{ 219, "tab_vehicle_bus_frame0" },
			{ 220, "tab_vehicle_bus_frame1" },
			{ 221, "tab_vehicle_bus_frame2" },
			{ 222, "tab_vehicle_bus_frame3" },
			{ 223, "tab_vehicle_bus_frame4" },
			{ 224, "tab_vehicle_bus_frame5" },
			{ 225, "tab_vehicle_bus_frame6" },
			{ 226, "tab_vehicle_bus_frame7" },
			{ 227, "tab_vehicle_tram_frame0" },
			{ 228, "tab_vehicle_tram_frame1" },
			{ 229, "tab_vehicle_tram_frame2" },
			{ 230, "tab_vehicle_tram_frame3" },
			{ 231, "tab_vehicle_tram_frame4" },
			{ 232, "tab_vehicle_tram_frame5" },
			{ 233, "tab_vehicle_tram_frame6" },
			{ 234, "tab_vehicle_tram_frame7" },
			{ 235, "tab_vehicle_truck_frame0" },
			{ 236, "tab_vehicle_truck_frame1" },
			{ 237, "tab_vehicle_truck_frame2" },
			{ 238, "tab_vehicle_truck_frame3" },
			{ 239, "tab_vehicle_truck_frame4" },
			{ 240, "tab_vehicle_truck_frame5" },
			{ 241, "tab_vehicle_truck_frame6" },
			{ 242, "tab_vehicle_truck_frame7" },
			{ 243, "tab_vehicle_ship_frame0" },
			{ 244, "tab_vehicle_ship_frame1" },
			{ 245, "tab_vehicle_ship_frame2" },
			{ 246, "tab_vehicle_ship_frame3" },
			{ 247, "tab_vehicle_ship_frame4" },
			{ 248, "tab_vehicle_ship_frame5" },
			{ 249, "tab_vehicle_ship_frame6" },
			{ 250, "tab_vehicle_ship_frame7" },
			{ 251, "build_vehicle_train_frame_0" },
			{ 252, "build_vehicle_train_frame_1" },
			{ 253, "build_vehicle_train_frame_2" },
			{ 254, "build_vehicle_train_frame_3" },
			{ 255, "build_vehicle_train_frame_4" },
			{ 256, "build_vehicle_train_frame_5" },
			{ 257, "build_vehicle_train_frame_6" },
			{ 258, "build_vehicle_train_frame_7" },
			{ 259, "build_vehicle_train_frame_8" },
			{ 260, "build_vehicle_train_frame_9" },
			{ 261, "build_vehicle_train_frame_10" },
			{ 262, "build_vehicle_train_frame_11" },
			{ 263, "build_vehicle_train_frame_12" },
			{ 264, "build_vehicle_train_frame_13" },
			{ 265, "build_vehicle_train_frame_14" },
			{ 266, "build_vehicle_train_frame_15" },
			{ 267, "build_vehicle_aircraft_frame_0" },
			{ 268, "build_vehicle_aircraft_frame_1" },
			{ 269, "build_vehicle_aircraft_frame_2" },
			{ 270, "build_vehicle_aircraft_frame_3" },
			{ 271, "build_vehicle_aircraft_frame_4" },
			{ 272, "build_vehicle_aircraft_frame_5" },
			{ 273, "build_vehicle_aircraft_frame_6" },
			{ 274, "build_vehicle_aircraft_frame_7" },
			{ 275, "build_vehicle_aircraft_frame_8" },
			{ 276, "build_vehicle_aircraft_frame_9" },
			{ 277, "build_vehicle_aircraft_frame_10" },
			{ 278, "build_vehicle_aircraft_frame_11" },
			{ 279, "build_vehicle_aircraft_frame_12" },
			{ 280, "build_vehicle_aircraft_frame_13" },
			{ 281, "build_vehicle_aircraft_frame_14" },
			{ 282, "build_vehicle_aircraft_frame_15" },
			{ 283, "build_vehicle_bus_frame_0" },
			{ 284, "build_vehicle_bus_frame_1" },
			{ 285, "build_vehicle_bus_frame_2" },
			{ 286, "build_vehicle_bus_frame_3" },
			{ 287, "build_vehicle_bus_frame_4" },
			{ 288, "build_vehicle_bus_frame_5" },
			{ 289, "build_vehicle_bus_frame_6" },
			{ 290, "build_vehicle_bus_frame_7" },
			{ 291, "build_vehicle_bus_frame_8" },
			{ 292, "build_vehicle_bus_frame_9" },
			{ 293, "build_vehicle_bus_frame_10" },
			{ 294, "build_vehicle_bus_frame_11" },
			{ 295, "build_vehicle_bus_frame_12" },
			{ 296, "build_vehicle_bus_frame_13" },
			{ 297, "build_vehicle_bus_frame_14" },
			{ 298, "build_vehicle_bus_frame_15" },
			{ 299, "build_vehicle_tram_frame_0" },
			{ 300, "build_vehicle_tram_frame_1" },
			{ 301, "build_vehicle_tram_frame_2" },
			{ 302, "build_vehicle_tram_frame_3" },
			{ 303, "build_vehicle_tram_frame_4" },
			{ 304, "build_vehicle_tram_frame_5" },
			{ 305, "build_vehicle_tram_frame_6" },
			{ 306, "build_vehicle_tram_frame_7" },
			{ 307, "build_vehicle_tram_frame_8" },
			{ 308, "build_vehicle_tram_frame_9" },
			{ 309, "build_vehicle_tram_frame_10" },
			{ 310, "build_vehicle_tram_frame_11" },
			{ 311, "build_vehicle_tram_frame_12" },
			{ 312, "build_vehicle_tram_frame_13" },
			{ 313, "build_vehicle_tram_frame_14" },
			{ 314, "build_vehicle_tram_frame_15" },
			{ 315, "build_vehicle_truck_frame_0" },
			{ 316, "build_vehicle_truck_frame_1" },
			{ 317, "build_vehicle_truck_frame_2" },
			{ 318, "build_vehicle_truck_frame_3" },
			{ 319, "build_vehicle_truck_frame_4" },
			{ 320, "build_vehicle_truck_frame_5" },
			{ 321, "build_vehicle_truck_frame_6" },
			{ 322, "build_vehicle_truck_frame_7" },
			{ 323, "build_vehicle_truck_frame_8" },
			{ 324, "build_vehicle_truck_frame_9" },
			{ 325, "build_vehicle_truck_frame_10" },
			{ 326, "build_vehicle_truck_frame_11" },
			{ 327, "build_vehicle_truck_frame_12" },
			{ 328, "build_vehicle_truck_frame_13" },
			{ 329, "build_vehicle_truck_frame_14" },
			{ 330, "build_vehicle_truck_frame_15" },
			{ 331, "build_vehicle_ship_frame_0" },
			{ 332, "build_vehicle_ship_frame_1" },
			{ 333, "build_vehicle_ship_frame_2" },
			{ 334, "build_vehicle_ship_frame_3" },
			{ 335, "build_vehicle_ship_frame_4" },
			{ 336, "build_vehicle_ship_frame_5" },
			{ 337, "build_vehicle_ship_frame_6" },
			{ 338, "build_vehicle_ship_frame_7" },
			{ 339, "build_vehicle_ship_frame_8" },
			{ 340, "build_vehicle_ship_frame_9" },
			{ 341, "build_vehicle_ship_frame_10" },
			{ 342, "build_vehicle_ship_frame_11" },
			{ 343, "build_vehicle_ship_frame_12" },
			{ 344, "build_vehicle_ship_frame_13" },
			{ 345, "build_vehicle_ship_frame_14" },
			{ 346, "build_vehicle_ship_frame_15" },
			{ 347, "build_industry_frame_0" },
			{ 348, "build_industry_frame_1" },
			{ 349, "build_industry_frame_2" },
			{ 350, "build_industry_frame_3" },
			{ 351, "build_industry_frame_4" },
			{ 352, "build_industry_frame_5" },
			{ 353, "build_industry_frame_6" },
			{ 354, "build_industry_frame_7" },
			{ 355, "build_industry_frame_8" },
			{ 356, "build_industry_frame_9" },
			{ 357, "build_industry_frame_10" },
			{ 358, "build_industry_frame_11" },
			{ 359, "build_industry_frame_12" },
			{ 360, "build_industry_frame_13" },
			{ 361, "build_industry_frame_14" },
			{ 362, "build_industry_frame_15" },
			{ 363, "build_town_frame_0" },
			{ 364, "build_town_frame_1" },
			{ 365, "build_town_frame_2" },
			{ 366, "build_town_frame_3" },
			{ 367, "build_town_frame_4" },
			{ 368, "build_town_frame_5" },
			{ 369, "build_town_frame_6" },
			{ 370, "build_town_frame_7" },
			{ 371, "build_town_frame_8" },
			{ 372, "build_town_frame_9" },
			{ 373, "build_town_frame_10" },
			{ 374, "build_town_frame_11" },
			{ 375, "build_town_frame_12" },
			{ 376, "build_town_frame_13" },
			{ 377, "build_town_frame_14" },
			{ 378, "build_town_frame_15" },
			{ 379, "build_buildings_frame_0" },
			{ 380, "build_buildings_frame_1" },
			{ 381, "build_buildings_frame_2" },
			{ 382, "build_buildings_frame_3" },
			{ 383, "build_buildings_frame_4" },
			{ 384, "build_buildings_frame_5" },
			{ 385, "build_buildings_frame_6" },
			{ 386, "build_buildings_frame_7" },
			{ 387, "build_buildings_frame_8" },
			{ 388, "build_buildings_frame_9" },
			{ 389, "build_buildings_frame_10" },
			{ 390, "build_buildings_frame_11" },
			{ 391, "build_buildings_frame_12" },
			{ 392, "build_buildings_frame_13" },
			{ 393, "build_buildings_frame_14" },
			{ 394, "build_buildings_frame_15" },
			{ 395, "build_misc_buildings_frame_0" },
			{ 396, "build_misc_buildings_frame_1" },
			{ 397, "build_misc_buildings_frame_2" },
			{ 398, "build_misc_buildings_frame_3" },
			{ 399, "build_misc_buildings_frame_4" },
			{ 400, "build_misc_buildings_frame_5" },
			{ 401, "build_misc_buildings_frame_6" },
			{ 402, "build_misc_buildings_frame_7" },
			{ 403, "build_misc_buildings_frame_8" },
			{ 404, "build_misc_buildings_frame_9" },
			{ 405, "build_misc_buildings_frame_10" },
			{ 406, "build_misc_buildings_frame_11" },
			{ 407, "build_misc_buildings_frame_12" },
			{ 408, "build_misc_buildings_frame_13" },
			{ 409, "build_misc_buildings_frame_14" },
			{ 410, "build_misc_buildings_frame_15" },
			{ 411, "build_additional_train" },
			{ 412, "build_additional_bus" },
			{ 413, "build_additional_truck" },
			{ 414, "build_additional_tram" },
			{ 415, "build_additional_aircraft" },
			{ 416, "build_additional_ship" },
			{ 417, "build_headquarters" },
			{ 418, "vehicle_train_frame_0" },
			{ 419, "vehicle_train_frame_1" },
			{ 420, "vehicle_train_frame_2" },
			{ 421, "vehicle_train_frame_3" },
			{ 422, "vehicle_train_frame_4" },
			{ 423, "vehicle_train_frame_5" },
			{ 424, "vehicle_train_frame_6" },
			{ 425, "vehicle_train_frame_7" },
			{ 426, "vehicle_aircraft_frame_0" },
			{ 427, "vehicle_aircraft_frame_1" },
			{ 428, "vehicle_aircraft_frame_2" },
			{ 429, "vehicle_aircraft_frame_3" },
			{ 430, "vehicle_aircraft_frame_4" },
			{ 431, "vehicle_aircraft_frame_5" },
			{ 432, "vehicle_aircraft_frame_6" },
			{ 433, "vehicle_aircraft_frame_7" },
			{ 434, "vehicle_buses_frame_0" },
			{ 435, "vehicle_buses_frame_1" },
			{ 436, "vehicle_buses_frame_2" },
			{ 437, "vehicle_buses_frame_3" },
			{ 438, "vehicle_buses_frame_4" },
			{ 439, "vehicle_buses_frame_5" },
			{ 440, "vehicle_buses_frame_6" },
			{ 441, "vehicle_buses_frame_7" },
			{ 442, "vehicle_trams_frame_0" },
			{ 443, "vehicle_trams_frame_1" },
			{ 444, "vehicle_trams_frame_2" },
			{ 445, "vehicle_trams_frame_3" },
			{ 446, "vehicle_trams_frame_4" },
			{ 447, "vehicle_trams_frame_5" },
			{ 448, "vehicle_trams_frame_6" },
			{ 449, "vehicle_trams_frame_7" },
			{ 450, "vehicle_trucks_frame_0" },
			{ 451, "vehicle_trucks_frame_1" },
			{ 452, "vehicle_trucks_frame_2" },
			{ 453, "vehicle_trucks_frame_3" },
			{ 454, "vehicle_trucks_frame_4" },
			{ 455, "vehicle_trucks_frame_5" },
			{ 456, "vehicle_trucks_frame_6" },
			{ 457, "vehicle_trucks_frame_7" },
			{ 458, "vehicle_ships_frame_0" },
			{ 459, "vehicle_ships_frame_1" },
			{ 460, "vehicle_ships_frame_2" },
			{ 461, "vehicle_ships_frame_3" },
			{ 462, "vehicle_ships_frame_4" },
			{ 463, "vehicle_ships_frame_5" },
			{ 464, "vehicle_ships_frame_6" },
			{ 465, "vehicle_ships_frame_7" },
			{ 466, "toolbar_menu_map_north" },
			{ 467, "toolbar_menu_map_west" },
			{ 468, "toolbar_menu_map_south" },
			{ 469, "toolbar_menu_map_east" },
		};
	}

	class CargoObjectNamer : ImageTableNamer<CargoObject>
	{
		public static CargoObjectNamer Instance { get; } = new();

		public override bool TryGetImageName(CargoObject model, int id, [MaybeNullWhen(false)] out string value)
		{
			value = id == 0
				? "kInlineSprite"
				: $"kStationPlatform{id}";

			return true;
		}
	}

	class CompetitorObjectNamer : ImageTableNamer<CompetitorObject>
	{
		public static CompetitorObjectNamer Instance { get; } = new();

		public override bool TryGetImageName(CompetitorObject model, int id, [MaybeNullWhen(false)] out string value)
			=> ImageIdNameMap.TryGetValue(id, out value);

		static readonly Dictionary<int, string> ImageIdNameMap = new()
		{
			{ 0, "smallNeutral" },
			{ 1, "largeNeutral" },
			{ 2, "smallHappy" },
			{ 3, "largeHappy" },
			{ 4, "smallWorried" },
			{ 5, "largeWorried" },
			{ 6, "smallThinking" },
			{ 7, "largeThinking" },
			{ 8, "smallDejected" },
			{ 9, "largeDejected" },
			{ 10, "smallSurprised" },
			{ 11, "largeSurprised" },
			{ 12, "smallScared" },
			{ 13, "largeScared" },
			{ 14, "smallAngry" },
			{ 15, "largeAngry" },
			{ 16, "smallDisgusted" },
			{ 17, "largeDisgusted" },
		};
	}

	class HillShapesObjectNamer : ImageTableNamer<HillShapesObject>
	{
		public static HillShapesObjectNamer Instance { get; } = new();

		public override bool TryGetImageName(HillShapesObject model, int id, [MaybeNullWhen(false)] out string value)
			=> ImageIdNameMap.TryGetValue(id, out value);

		static readonly Dictionary<int, string> ImageIdNameMap = new()
		{
			{ 0, "hill shape 1" },
			{ 1, "hill shape 2" },
			{ 2, "mountain shape 1" },
			{ 3, "mountain shape 2" },
			{ 4, "preview image" },

		};
	}

	class LandObjectNamer : ImageTableNamer<LandObject>
	{
		public static LandObjectNamer Instance { get; } = new();

		public override bool TryGetImageName(LandObject model, int id, [MaybeNullWhen(false)] out string value)
			=> ImageIdNameMap.TryGetValue(id, out value);

		static readonly Dictionary<int, string> ImageIdNameMap = new()
		{
			{ 0, "flat" },
			{ 1, "west corner up" },
			{ 2, "south corner up" },
			{ 3, "north east slope" },
			{ 4, "east corner up" },
			{ 5, "west and east corner up" },
			{ 6, "north west slope" },
			{ 7, "north corner down" },
			{ 8, "north corner up" },
			{ 9, "south east slope" },
			{ 10, "north and south corners up" },
			{ 11, "east corner down" },
			{ 12, "north west slope" },
			{ 13, "south corner down" },
			{ 14, "west corner down" },
			{ 15, "south slope" },
			{ 16, "north slope" },
			{ 17, "east slope" },
			{ 18, "west slope" }
		};
	}

	class StreetLightObjectNamer : ImageTableNamer<StreetLightObject>
	{
		public static StreetLightObjectNamer Instance { get; } = new();

		public override bool TryGetImageName(StreetLightObject model, int id, [MaybeNullWhen(false)] out string value)
			=> ImageIdNameMap.TryGetValue(id, out value);

		static readonly Dictionary<int, string> ImageIdNameMap = new()
		{
			{ 0, "style0NE" },
			{ 1, "style0SE" },
			{ 2, "style0SW" },
			{ 3, "style0NW" },
			{ 4, "style1NE" },
			{ 5, "style1SE" },
			{ 6, "style1SW" },
			{ 7, "style1NW" },
			{ 8, "style2NE" },
			{ 9, "style2SE" },
			{ 10, "style2SW" },
			{ 11, "style2NW" },
		};
	}

	class RoadStationObjectNamer : ImageTableNamer<RoadStationObject>
	{
		public static RoadStationObjectNamer Instance { get; } = new();

		public override bool TryGetImageName(RoadStationObject model, int id, [MaybeNullWhen(false)] out string value)
			=> ImageIdNameMap.TryGetValue(id, out value);

		static readonly Dictionary<int, string> ImageIdNameMap = new()
		{
			{ 0, "preview_image" },
			{ 1, "preview_image_glass_overlay" },
			{ 2, "North West Back Wall" },
			{ 3, "North West Front Platform" },
			{ 4, "North West Front Wall/Roof" },
			{ 5, "North West Glass Overlay" },
			{ 6, "South West Back Wall" },
			{ 7, "South West Front Platform" },
			{ 8, "South West Front Wall/Roof" },
			{ 9, "South West Glass Overlay" },
			{ 10, "South East Back Wall" },
			{ 11, "South East Front Platform" },
			{ 12, "South East Front Wall/Roof" },
			{ 13, "South East Glass Overlay" },
			{ 14, "North East Back Wall" },
			{ 15, "North East Front Platform" },
			{ 16, "North East Front Wall/Roof" },
			{ 17, "North East Glass Overlay" },
		};
	}

	class TrackStationObjectNamer : ImageTableNamer<TrackStationObject>
	{
		public static TrackStationObjectNamer Instance { get; } = new();

		public override bool TryGetImageName(TrackStationObject model, int id, [MaybeNullWhen(false)] out string value)
			=> ImageIdNameMap.TryGetValue(id, out value);

		static readonly Dictionary<int, string> ImageIdNameMap = new()
		{
			{ 0, "preview_image" },
			{ 1, "preview_image_glass_overlay" },
			{ 2, "totalPreviewImages" },
		};

		// These are relative to ImageOffsets
		// ImageOffsets is the imageIds per sequenceIndex (for start/middle/end of the platform)
		//namespace Style0
		//{
		//    constexpr uint32_t straightBackNE = 0;
		//    constexpr uint32_t straightFrontNE = 1;
		//    constexpr uint32_t straightCanopyNE = 2;
		//    constexpr uint32_t straightCanopyTranslucentNE = 3;
		//    constexpr uint32_t straightBackSE = 4;
		//    constexpr uint32_t straightFrontSE = 5;
		//    constexpr uint32_t straightCanopySE = 6;
		//    constexpr uint32_t straightCanopyTranslucentSE = 7;
		//    constexpr uint32_t diagonalNE0 = 8;
		//    constexpr uint32_t diagonalNE3 = 9;
		//    constexpr uint32_t diagonalNE1 = 10;
		//    constexpr uint32_t diagonalCanopyNE1 = 11;
		//    constexpr uint32_t diagonalCanopyTranslucentNE1 = 12;
		//    constexpr uint32_t diagonalSE1 = 13;
		//    constexpr uint32_t diagonalSE2 = 14;
		//    constexpr uint32_t diagonalSE3 = 15;
		//    constexpr uint32_t diagonalCanopyTranslucentSE3 = 16;
		//    constexpr uint32_t totalNumImages = 17;
		//}
		//namespace Style1
		//{
		//    constexpr uint32_t totalNumImages = 8;
		//}
	}

	class TrackExtraObjectNamer : ImageTableNamer<TrackExtraObject>
	{
		public static TrackExtraObjectNamer Instance { get; } = new();

		public override bool TryGetImageName(TrackExtraObject model, int id, [MaybeNullWhen(false)] out string value)
			=> ImageIdNameMap.TryGetValue(id - 8, out value);

		static readonly Dictionary<int, string> ImageIdNameMap = new()
		{
			{ -8, "previewImage0" },
			{ -7, "previewImage1" },
			{ -6, "previewImage2" },
			{ -5, "previewImage3" },
			{ -4, "previewImage4" },
			{ -3, "previewImage5" },
			{ -2, "previewImage6" },
			{ -1, "previewImage7" },
			{ 0, "kStraight0NE" },
			{ 1, "kStraight0SE" },
			{ 2, "kStraight0SW" },
			{ 3, "kStraight0NW" },
			{ 4, "kRightCurveSmall0NE" },
			{ 5, "kRightCurveSmall1NE" },
			{ 6, "kRightCurveSmall2NE" },
			{ 7, "kRightCurveSmall3NE" },
			{ 8, "kRightCurveSmall0SE" },
			{ 9, "kRightCurveSmall1SE" },
			{ 10, "kRightCurveSmall2SE" },
			{ 11, "kRightCurveSmall3SE" },
			{ 12, "kRightCurveSmall0SW" },
			{ 13, "kRightCurveSmall1SW" },
			{ 14, "kRightCurveSmall2SW" },
			{ 15, "kRightCurveSmall3SW" },
			{ 16, "kRightCurveSmall0NW" },
			{ 17, "kRightCurveSmall1NW" },
			{ 18, "kRightCurveSmall2NW" },
			{ 19, "kRightCurveSmall3NW" },
			{ 20, "kRightCurve0NE" },
			{ 21, "kRightCurve1NE" },
			{ 22, "kRightCurve2NE" },
			{ 23, "kRightCurve3NE" },
			{ 24, "kRightCurve4NE" },
			{ 25, "kRightCurve0SE" },
			{ 26, "kRightCurve1SE" },
			{ 27, "kRightCurve2SE" },
			{ 28, "kRightCurve3SE" },
			{ 29, "kRightCurve4SE" },
			{ 30, "kRightCurve0SW" },
			{ 31, "kRightCurve1SW" },
			{ 32, "kRightCurve2SW" },
			{ 33, "kRightCurve3SW" },
			{ 34, "kRightCurve4SW" },
			{ 35, "kRightCurve0NW" },
			{ 36, "kRightCurve1NW" },
			{ 37, "kRightCurve2NW" },
			{ 38, "kRightCurve3NW" },
			{ 39, "kRightCurve4NW" },
			{ 40, "kSBendLeft0NE" },
			{ 41, "kSBendLeft1NE" },
			{ 42, "kSBendLeft2NE" },
			{ 43, "kSBendLeft3NE" },
			{ 44, "kSBendLeft0SE" },
			{ 45, "kSBendLeft1SE" },
			{ 46, "kSBendLeft2SE" },
			{ 47, "kSBendLeft3SE" },
			{ 48, "kSBendLeft3SW" },
			{ 49, "kSBendLeft2SW" },
			{ 50, "kSBendLeft1SW" },
			{ 51, "kSBendLeft0SW" },
			{ 52, "kSBendLeft3NW" },
			{ 53, "kSBendLeft2NW" },
			{ 54, "kSBendLeft1NW" },
			{ 55, "kSBendLeft0NW" },
			{ 56, "kSBendRight0NE" },
			{ 57, "kSBendRight1NE" },
			{ 58, "kSBendRight2NE" },
			{ 59, "kSBendRight3NE" },
			{ 60, "kSBendRight0SE" },
			{ 61, "kSBendRight1SE" },
			{ 62, "kSBendRight2SE" },
			{ 63, "kSBendRight3SE" },
			{ 64, "kSBendRight3SW" },
			{ 65, "kSBendRight2SW" },
			{ 66, "kSBendRight1SW" },
			{ 67, "kSBendRight0SW" },
			{ 68, "kSBendRight3NW" },
			{ 69, "kSBendRight2NW" },
			{ 70, "kSBendRight1NW" },
			{ 71, "kSBendRight0NW" },
			{ 72, "kStraightSlopeUp0NE" },
			{ 73, "kStraightSlopeUp1NE" },
			{ 74, "kStraightSlopeUp0SE" },
			{ 75, "kStraightSlopeUp1SE" },
			{ 76, "kStraightSlopeUp0SW" },
			{ 77, "kStraightSlopeUp1SW" },
			{ 78, "kStraightSlopeUp0NW" },
			{ 79, "kStraightSlopeUp1NW" },
			{ 80, "kStraightSteepSlopeUp0NE" },
			{ 81, "kStraightSteepSlopeUp0SE" },
			{ 82, "kStraightSteepSlopeUp0SW" },
			{ 83, "kStraightSteepSlopeUp0NW" },
			{ 84, "kRightCurveSmallSlopeUp0NE" },
			{ 85, "kRightCurveSmallSlopeUp1NE" },
			{ 86, "kRightCurveSmallSlopeUp2NE" },
			{ 87, "kRightCurveSmallSlopeUp3NE" },
			{ 88, "kRightCurveSmallSlopeUp0SE" },
			{ 89, "kRightCurveSmallSlopeUp1SE" },
			{ 90, "kRightCurveSmallSlopeUp2SE" },
			{ 91, "kRightCurveSmallSlopeUp3SE" },
			{ 92, "kRightCurveSmallSlopeUp0SW" },
			{ 93, "kRightCurveSmallSlopeUp1SW" },
			{ 94, "kRightCurveSmallSlopeUp2SW" },
			{ 95, "kRightCurveSmallSlopeUp3SW" },
			{ 96, "kRightCurveSmallSlopeUp0NW" },
			{ 97, "kRightCurveSmallSlopeUp1NW" },
			{ 98, "kRightCurveSmallSlopeUp2NW" },
			{ 99, "kRightCurveSmallSlopeUp3NW" },
			{ 100, "kRightCurveSmallSlopeDown0NE" },
			{ 101, "kRightCurveSmallSlopeDown1NE" },
			{ 102, "kRightCurveSmallSlopeDown2NE" },
			{ 103, "kRightCurveSmallSlopeDown3NE" },
			{ 104, "kRightCurveSmallSlopeDown0SE" },
			{ 105, "kRightCurveSmallSlopeDown1SE" },
			{ 106, "kRightCurveSmallSlopeDown2SE" },
			{ 107, "kRightCurveSmallSlopeDown3SE" },
			{ 108, "kRightCurveSmallSlopeDown0SW" },
			{ 109, "kRightCurveSmallSlopeDown1SW" },
			{ 110, "kRightCurveSmallSlopeDown2SW" },
			{ 111, "kRightCurveSmallSlopeDown3SW" },
			{ 112, "kRightCurveSmallSlopeDown0NW" },
			{ 113, "kRightCurveSmallSlopeDown1NW" },
			{ 114, "kRightCurveSmallSlopeDown2NW" },
			{ 115, "kRightCurveSmallSlopeDown3NW" },
			{ 116, "kRightCurveSmallSteepSlopeUp0NE" },
			{ 117, "kRightCurveSmallSteepSlopeUp1NE" },
			{ 118, "kRightCurveSmallSteepSlopeUp2NE" },
			{ 119, "kRightCurveSmallSteepSlopeUp3NE" },
			{ 120, "kRightCurveSmallSteepSlopeUp0SE" },
			{ 121, "kRightCurveSmallSteepSlopeUp1SE" },
			{ 122, "kRightCurveSmallSteepSlopeUp2SE" },
			{ 123, "kRightCurveSmallSteepSlopeUp3SE" },
			{ 124, "kRightCurveSmallSteepSlopeUp0SW" },
			{ 125, "kRightCurveSmallSteepSlopeUp1SW" },
			{ 126, "kRightCurveSmallSteepSlopeUp2SW" },
			{ 127, "kRightCurveSmallSteepSlopeUp3SW" },
			{ 128, "kRightCurveSmallSteepSlopeUp0NW" },
			{ 129, "kRightCurveSmallSteepSlopeUp1NW" },
			{ 130, "kRightCurveSmallSteepSlopeUp2NW" },
			{ 131, "kRightCurveSmallSteepSlopeUp3NW" },
			{ 132, "kRightCurveSmallSteepSlopeDown0NE" },
			{ 133, "kRightCurveSmallSteepSlopeDown1NE" },
			{ 134, "kRightCurveSmallSteepSlopeDown2NE" },
			{ 135, "kRightCurveSmallSteepSlopeDown3NE" },
			{ 136, "kRightCurveSmallSteepSlopeDown0SE" },
			{ 137, "kRightCurveSmallSteepSlopeDown1SE" },
			{ 138, "kRightCurveSmallSteepSlopeDown2SE" },
			{ 139, "kRightCurveSmallSteepSlopeDown3SE" },
			{ 140, "kRightCurveSmallSteepSlopeDown0SW" },
			{ 141, "kRightCurveSmallSteepSlopeDown1SW" },
			{ 142, "kRightCurveSmallSteepSlopeDown2SW" },
			{ 143, "kRightCurveSmallSteepSlopeDown3SW" },
			{ 144, "kRightCurveSmallSteepSlopeDown0NW" },
			{ 145, "kRightCurveSmallSteepSlopeDown1NW" },
			{ 146, "kRightCurveSmallSteepSlopeDown2NW" },
			{ 147, "kRightCurveSmallSteepSlopeDown3NW" },
			{ 148, "kRightCurveLarge0NE" },
			{ 149, "kRightCurveLarge1NE" },
			{ 150, "kRightCurveLarge2NE" },
			{ 151, "kRightCurveLarge3NE" },
			{ 152, "kRightCurveLarge4NE" },
			{ 153, "kRightCurveLarge0SE" },
			{ 154, "kRightCurveLarge1SE" },
			{ 155, "kRightCurveLarge2SE" },
			{ 156, "kRightCurveLarge3SE" },
			{ 157, "kRightCurveLarge4SE" },
			{ 158, "kRightCurveLarge0SW" },
			{ 159, "kRightCurveLarge1SW" },
			{ 160, "kRightCurveLarge2SW" },
			{ 161, "kRightCurveLarge3SW" },
			{ 162, "kRightCurveLarge4SW" },
			{ 163, "kRightCurveLarge0NW" },
			{ 164, "kRightCurveLarge1NW" },
			{ 165, "kRightCurveLarge2NW" },
			{ 166, "kRightCurveLarge3NW" },
			{ 167, "kRightCurveLarge4NW" },
			{ 168, "kLeftCurveLarge0NE" },
			{ 169, "kLeftCurveLarge1NE" },
			{ 170, "kLeftCurveLarge2NE" },
			{ 171, "kLeftCurveLarge3NE" },
			{ 172, "kLeftCurveLarge4NE" },
			{ 173, "kLeftCurveLarge0SE" },
			{ 174, "kLeftCurveLarge1SE" },
			{ 175, "kLeftCurveLarge2SE" },
			{ 176, "kLeftCurveLarge3SE" },
			{ 177, "kLeftCurveLarge4SE" },
			{ 178, "kLeftCurveLarge0SW" },
			{ 179, "kLeftCurveLarge1SW" },
			{ 180, "kLeftCurveLarge2SW" },
			{ 181, "kLeftCurveLarge3SW" },
			{ 182, "kLeftCurveLarge4SW" },
			{ 183, "kLeftCurveLarge0NW" },
			{ 184, "kLeftCurveLarge1NW" },
			{ 185, "kLeftCurveLarge2NW" },
			{ 186, "kLeftCurveLarge3NW" },
			{ 187, "kLeftCurveLarge4NW" },
			{ 188, "kDiagonal0NE" },
			{ 189, "kDiagonal2NE" },
			{ 190, "kDiagonal1NE" },
			{ 191, "kDiagonal3NE" },
			{ 192, "kDiagonal0SE" },
			{ 193, "kDiagonal2SE" },
			{ 194, "kDiagonal1SE" },
			{ 195, "kDiagonal3SE" },
			{ 196, "kDiagonal0SW" },
			{ 197, "kDiagonal2SW" },
			{ 198, "kDiagonal1SW" },
			{ 199, "kDiagonal3SW" },
			{ 200, "kDiagonal0NW" },
			{ 201, "kDiagonal2NW" },
			{ 202, "kDiagonal1NW" },
			{ 203, "kDiagonal3NW" },
			{ 204, "kRightCurveVerySmall0NE" },
			{ 205, "kRightCurveVerySmall0SE" },
			{ 206, "kRightCurveVerySmall0SW" },
			{ 207, "kRightCurveVerySmall0NW" },
		};
	}

	class TrackObjectNamer : ImageTableNamer<TrackObject>
	{
		public static TrackObjectNamer Instance { get; } = new();

		public override bool TryGetImageName(TrackObject model, int id, [MaybeNullWhen(false)] out string value)
			=> ImageIdNameMap.TryGetValue(id, out value);

		static readonly Dictionary<int, string> ImageIdNameMap = new()
		{
			{ 0, "uiPreviewImage0" },
			{ 1, "uiPreviewImage1" },
			{ 2, "uiPreviewImage2" },
			{ 3, "uiPreviewImage3" },
			{ 4, "uiPreviewImage4" },
			{ 5, "uiPreviewImage5" },
			{ 6, "uiPreviewImage6" },
			{ 7, "uiPreviewImage7" },
			{ 8, "uiPreviewImage8" },
			{ 9, "uiPreviewImage9" },
			{ 10, "uiPreviewImage10" },
			{ 11, "uiPreviewImage11" },
			{ 12, "uiPreviewImage12" },
			{ 13, "uiPreviewImage13" },
			{ 14, "uiPreviewImage14" },
			{ 15, "uiPreviewImage15" },
			{ 16, "uiPickupFromTrack" },
			{ 17, "uiPlaceOnTrack" },
			//
			{ 18, "straight0BallastNE" },
			{ 19, "straight0BallastSE" },
			{ 20, "straight0SleeperNE" },
			{ 21, "straight0SleeperSE" },
			{ 22, "straight0RailNE" },
			{ 23, "straight0RailSE" },
			{ 24, "rightCurveSmall0BallastNE" },
			{ 25, "rightCurveSmall1BallastNE" },
			{ 26, "rightCurveSmall2BallastNE" },
			{ 27, "rightCurveSmall3BallastNE" },
			{ 28, "rightCurveSmall0BallastSE" },
			{ 29, "rightCurveSmall1BallastSE" },
			{ 30, "rightCurveSmall2BallastSE" },
			{ 31, "rightCurveSmall3BallastSE" },
			{ 32, "rightCurveSmall0BallastSW" },
			{ 33, "rightCurveSmall1BallastSW" },
			{ 34, "rightCurveSmall2BallastSW" },
			{ 35, "rightCurveSmall3BallastSW" },
			{ 36, "rightCurveSmall0BallastNW" },
			{ 37, "rightCurveSmall1BallastNW" },
			{ 38, "rightCurveSmall2BallastNW" },
			{ 39, "rightCurveSmall3BallastNW" },
			{ 40, "rightCurveSmall0SleeperNE" },
			{ 41, "rightCurveSmall1SleeperNE" },
			{ 42, "rightCurveSmall2SleeperNE" },
			{ 43, "rightCurveSmall3SleeperNE" },
			{ 44, "rightCurveSmall0SleeperSE" },
			{ 45, "rightCurveSmall1SleeperSE" },
			{ 46, "rightCurveSmall2SleeperSE" },
			{ 47, "rightCurveSmall3SleeperSE" },
			{ 48, "rightCurveSmall0SleeperSW" },
			{ 49, "rightCurveSmall1SleeperSW" },
			{ 50, "rightCurveSmall2SleeperSW" },
			{ 51, "rightCurveSmall3SleeperSW" },
			{ 52, "rightCurveSmall0SleeperNW" },
			{ 53, "rightCurveSmall1SleeperNW" },
			{ 54, "rightCurveSmall2SleeperNW" },
			{ 55, "rightCurveSmall3SleeperNW" },
			{ 56, "rightCurveSmall0RailNE" },
			{ 57, "rightCurveSmall1RailNE" },
			{ 58, "rightCurveSmall2RailNE" },
			{ 59, "rightCurveSmall3RailNE" },
			{ 60, "rightCurveSmall0RailSE" },
			{ 61, "rightCurveSmall1RailSE" },
			{ 62, "rightCurveSmall2RailSE" },
			{ 63, "rightCurveSmall3RailSE" },
			{ 64, "rightCurveSmall0RailSW" },
			{ 65, "rightCurveSmall1RailSW" },
			{ 66, "rightCurveSmall2RailSW" },
			{ 67, "rightCurveSmall3RailSW" },
			{ 68, "rightCurveSmall0RailNW" },
			{ 69, "rightCurveSmall1RailNW" },
			{ 70, "rightCurveSmall2RailNW" },
			{ 71, "rightCurveSmall3RailNW" },
			{ 72, "rightCurveSmallSlopeUp0NE" },
			{ 73, "rightCurveSmallSlopeUp1NE" },
			{ 74, "rightCurveSmallSlopeUp2NE" },
			{ 75, "rightCurveSmallSlopeUp3NE" },
			{ 76, "rightCurveSmallSlopeUp0SE" },
			{ 77, "rightCurveSmallSlopeUp1SE" },
			{ 78, "rightCurveSmallSlopeUp2SE" },
			{ 79, "rightCurveSmallSlopeUp3SE" },
			{ 80, "rightCurveSmallSlopeUp0SW" },
			{ 81, "rightCurveSmallSlopeUp1SW" },
			{ 82, "rightCurveSmallSlopeUp2SW" },
			{ 83, "rightCurveSmallSlopeUp3SW" },
			{ 84, "rightCurveSmallSlopeUp0NW" },
			{ 85, "rightCurveSmallSlopeUp1NW" },
			{ 86, "rightCurveSmallSlopeUp2NW" },
			{ 87, "rightCurveSmallSlopeUp3NW" },
			{ 88, "rightCurveSmallSlopeDown0NE" },
			{ 89, "rightCurveSmallSlopeDown1NE" },
			{ 90, "rightCurveSmallSlopeDown2NE" },
			{ 91, "rightCurveSmallSlopeDown3NE" },
			{ 92, "rightCurveSmallSlopeDown0SE" },
			{ 93, "rightCurveSmallSlopeDown1SE" },
			{ 94, "rightCurveSmallSlopeDown2SE" },
			{ 95, "rightCurveSmallSlopeDown3SE" },
			{ 96, "rightCurveSmallSlopeDown0SW" },
			{ 97, "rightCurveSmallSlopeDown1SW" },
			{ 98, "rightCurveSmallSlopeDown2SW" },
			{ 99, "rightCurveSmallSlopeDown3SW" },
			{ 100, "rightCurveSmallSlopeDown0NW" },
			{ 101, "rightCurveSmallSlopeDown1NW" },
			{ 102, "rightCurveSmallSlopeDown2NW" },
			{ 103, "rightCurveSmallSlopeDown3NW" },
			{ 104, "rightCurveSmallSteepSlopeUp0NE" },
			{ 105, "rightCurveSmallSteepSlopeUp1NE" },
			{ 106, "rightCurveSmallSteepSlopeUp2NE" },
			{ 107, "rightCurveSmallSteepSlopeUp3NE" },
			{ 108, "rightCurveSmallSteepSlopeUp0SE" },
			{ 109, "rightCurveSmallSteepSlopeUp1SE" },
			{ 110, "rightCurveSmallSteepSlopeUp2SE" },
			{ 111, "rightCurveSmallSteepSlopeUp3SE" },
			{ 112, "rightCurveSmallSteepSlopeUp0SW" },
			{ 113, "rightCurveSmallSteepSlopeUp1SW" },
			{ 114, "rightCurveSmallSteepSlopeUp2SW" },
			{ 115, "rightCurveSmallSteepSlopeUp3SW" },
			{ 116, "rightCurveSmallSteepSlopeUp0NW" },
			{ 117, "rightCurveSmallSteepSlopeUp1NW" },
			{ 118, "rightCurveSmallSteepSlopeUp2NW" },
			{ 119, "rightCurveSmallSteepSlopeUp3NW" },
			{ 120, "rightCurveSmallSteepSlopeDown0NE" },
			{ 121, "rightCurveSmallSteepSlopeDown1NE" },
			{ 122, "rightCurveSmallSteepSlopeDown2NE" },
			{ 123, "rightCurveSmallSteepSlopeDown3NE" },
			{ 124, "rightCurveSmallSteepSlopeDown0SE" },
			{ 125, "rightCurveSmallSteepSlopeDown1SE" },
			{ 126, "rightCurveSmallSteepSlopeDown2SE" },
			{ 127, "rightCurveSmallSteepSlopeDown3SE" },
			{ 128, "rightCurveSmallSteepSlopeDown0SW" },
			{ 129, "rightCurveSmallSteepSlopeDown1SW" },
			{ 130, "rightCurveSmallSteepSlopeDown2SW" },
			{ 131, "rightCurveSmallSteepSlopeDown3SW" },
			{ 132, "rightCurveSmallSteepSlopeDown0NW" },
			{ 133, "rightCurveSmallSteepSlopeDown1NW" },
			{ 134, "rightCurveSmallSteepSlopeDown2NW" },
			{ 135, "rightCurveSmallSteepSlopeDown3NW" },
			{ 136, "rightCurve0BallastNE" },
			{ 137, "rightCurve1BallastNE" },
			{ 138, "rightCurve2BallastNE" },
			{ 139, "rightCurve3BallastNE" },
			{ 140, "rightCurve4BallastNE" },
			{ 141, "rightCurve0BallastSE" },
			{ 142, "rightCurve1BallastSE" },
			{ 143, "rightCurve2BallastSE" },
			{ 144, "rightCurve3BallastSE" },
			{ 145, "rightCurve4BallastSE" },
			{ 146, "rightCurve0BallastSW" },
			{ 147, "rightCurve1BallastSW" },
			{ 148, "rightCurve2BallastSW" },
			{ 149, "rightCurve3BallastSW" },
			{ 150, "rightCurve4BallastSW" },
			{ 151, "rightCurve0BallastNW" },
			{ 152, "rightCurve1BallastNW" },
			{ 153, "rightCurve2BallastNW" },
			{ 154, "rightCurve3BallastNW" },
			{ 155, "rightCurve4BallastNW" },
			{ 156, "rightCurve0SleeperNE" },
			{ 157, "rightCurve1SleeperNE" },
			{ 158, "rightCurve2SleeperNE" },
			{ 159, "rightCurve3SleeperNE" },
			{ 160, "rightCurve4SleeperNE" },
			{ 161, "rightCurve0SleeperSE" },
			{ 162, "rightCurve1SleeperSE" },
			{ 163, "rightCurve2SleeperSE" },
			{ 164, "rightCurve3SleeperSE" },
			{ 165, "rightCurve4SleeperSE" },
			{ 166, "rightCurve0SleeperSW" },
			{ 167, "rightCurve1SleeperSW" },
			{ 168, "rightCurve2SleeperSW" },
			{ 169, "rightCurve3SleeperSW" },
			{ 170, "rightCurve4SleeperSW" },
			{ 171, "rightCurve0SleeperNW" },
			{ 172, "rightCurve1SleeperNW" },
			{ 173, "rightCurve2SleeperNW" },
			{ 174, "rightCurve3SleeperNW" },
			{ 175, "rightCurve4SleeperNW" },
			{ 176, "rightCurve0RailNE" },
			{ 177, "rightCurve1RailNE" },
			{ 178, "rightCurve2RailNE" },
			{ 179, "rightCurve3RailNE" },
			{ 180, "rightCurve4RailNE" },
			{ 181, "rightCurve0RailSE" },
			{ 182, "rightCurve1RailSE" },
			{ 183, "rightCurve2RailSE" },
			{ 184, "rightCurve3RailSE" },
			{ 185, "rightCurve4RailSE" },
			{ 186, "rightCurve0RailSW" },
			{ 187, "rightCurve1RailSW" },
			{ 188, "rightCurve2RailSW" },
			{ 189, "rightCurve3RailSW" },
			{ 190, "rightCurve4RailSW" },
			{ 191, "rightCurve0RailNW" },
			{ 192, "rightCurve1RailNW" },
			{ 193, "rightCurve2RailNW" },
			{ 194, "rightCurve3RailNW" },
			{ 195, "rightCurve4RailNW" },
			{ 196, "straightSlopeUp0NE" },
			{ 197, "straightSlopeUp1NE" },
			{ 198, "straightSlopeUp0SE" },
			{ 199, "straightSlopeUp1SE" },
			{ 200, "straightSlopeUp0SW" },
			{ 201, "straightSlopeUp1SW" },
			{ 202, "straightSlopeUp0NW" },
			{ 203, "straightSlopeUp1NW" },
			{ 204, "straightSteepSlopeUp0NE" },
			{ 205, "straightSteepSlopeUp0SE" },
			{ 206, "straightSteepSlopeUp0SW" },
			{ 207, "straightSteepSlopeUp0NW" },
			{ 208, "rightCurveLarge0BallastNE" },
			{ 209, "rightCurveLarge1BallastNE" },
			{ 210, "rightCurveLarge2BallastNE" },
			{ 211, "rightCurveLarge3BallastNE" },
			{ 212, "rightCurveLarge4BallastNE" },
			{ 213, "rightCurveLarge0BallastSE" },
			{ 214, "rightCurveLarge1BallastSE" },
			{ 215, "rightCurveLarge2BallastSE" },
			{ 216, "rightCurveLarge3BallastSE" },
			{ 217, "rightCurveLarge4BallastSE" },
			{ 218, "rightCurveLarge0BallastSW" },
			{ 219, "rightCurveLarge1BallastSW" },
			{ 220, "rightCurveLarge2BallastSW" },
			{ 221, "rightCurveLarge3BallastSW" },
			{ 222, "rightCurveLarge4BallastSW" },
			{ 223, "rightCurveLarge0BallastNW" },
			{ 224, "rightCurveLarge1BallastNW" },
			{ 225, "rightCurveLarge2BallastNW" },
			{ 226, "rightCurveLarge3BallastNW" },
			{ 227, "rightCurveLarge4BallastNW" },
			{ 228, "leftCurveLarge0BallastNE" },
			{ 229, "leftCurveLarge1BallastNE" },
			{ 230, "leftCurveLarge2BallastNE" },
			{ 231, "leftCurveLarge3BallastNE" },
			{ 232, "leftCurveLarge4BallastNE" },
			{ 233, "leftCurveLarge0BallastSE" },
			{ 234, "leftCurveLarge1BallastSE" },
			{ 235, "leftCurveLarge2BallastSE" },
			{ 236, "leftCurveLarge3BallastSE" },
			{ 237, "leftCurveLarge4BallastSE" },
			{ 238, "leftCurveLarge0BallastSW" },
			{ 239, "leftCurveLarge1BallastSW" },
			{ 240, "leftCurveLarge2BallastSW" },
			{ 241, "leftCurveLarge3BallastSW" },
			{ 242, "leftCurveLarge4BallastSW" },
			{ 243, "leftCurveLarge0BallastNW" },
			{ 244, "leftCurveLarge1BallastNW" },
			{ 245, "leftCurveLarge2BallastNW" },
			{ 246, "leftCurveLarge3BallastNW" },
			{ 247, "leftCurveLarge4BallastNW" },
			{ 248, "rightCurveLarge0SleeperNE" },
			{ 249, "rightCurveLarge1SleeperNE" },
			{ 250, "rightCurveLarge2SleeperNE" },
			{ 251, "rightCurveLarge3SleeperNE" },
			{ 252, "rightCurveLarge4SleeperNE" },
			{ 253, "rightCurveLarge0SleeperSE" },
			{ 254, "rightCurveLarge1SleeperSE" },
			{ 255, "rightCurveLarge2SleeperSE" },
			{ 256, "rightCurveLarge3SleeperSE" },
			{ 257, "rightCurveLarge4SleeperSE" },
			{ 258, "rightCurveLarge0SleeperSW" },
			{ 259, "rightCurveLarge1SleeperSW" },
			{ 260, "rightCurveLarge2SleeperSW" },
			{ 261, "rightCurveLarge3SleeperSW" },
			{ 262, "rightCurveLarge4SleeperSW" },
			{ 263, "rightCurveLarge0SleeperNW" },
			{ 264, "rightCurveLarge1SleeperNW" },
			{ 265, "rightCurveLarge2SleeperNW" },
			{ 266, "rightCurveLarge3SleeperNW" },
			{ 267, "rightCurveLarge4SleeperNW" },
			{ 268, "leftCurveLarge0SleeperNE" },
			{ 269, "leftCurveLarge1SleeperNE" },
			{ 270, "leftCurveLarge2SleeperNE" },
			{ 271, "leftCurveLarge3SleeperNE" },
			{ 272, "leftCurveLarge4SleeperNE" },
			{ 273, "leftCurveLarge0SleeperSE" },
			{ 274, "leftCurveLarge1SleeperSE" },
			{ 275, "leftCurveLarge2SleeperSE" },
			{ 276, "leftCurveLarge3SleeperSE" },
			{ 277, "leftCurveLarge4SleeperSE" },
			{ 278, "leftCurveLarge0SleeperSW" },
			{ 279, "leftCurveLarge1SleeperSW" },
			{ 280, "leftCurveLarge2SleeperSW" },
			{ 281, "leftCurveLarge3SleeperSW" },
			{ 282, "leftCurveLarge4SleeperSW" },
			{ 283, "leftCurveLarge0SleeperNW" },
			{ 284, "leftCurveLarge1SleeperNW" },
			{ 285, "leftCurveLarge2SleeperNW" },
			{ 286, "leftCurveLarge3SleeperNW" },
			{ 287, "leftCurveLarge4SleeperNW" },
			{ 288, "rightCurveLarge0RailNE" },
			{ 289, "rightCurveLarge1RailNE" },
			{ 290, "rightCurveLarge2RailNE" },
			{ 291, "rightCurveLarge3RailNE" },
			{ 292, "rightCurveLarge4RailNE" },
			{ 293, "rightCurveLarge0RailSE" },
			{ 294, "rightCurveLarge1RailSE" },
			{ 295, "rightCurveLarge2RailSE" },
			{ 296, "rightCurveLarge3RailSE" },
			{ 297, "rightCurveLarge4RailSE" },
			{ 298, "rightCurveLarge0RailSW" },
			{ 299, "rightCurveLarge1RailSW" },
			{ 300, "rightCurveLarge2RailSW" },
			{ 301, "rightCurveLarge3RailSW" },
			{ 302, "rightCurveLarge4RailSW" },
			{ 303, "rightCurveLarge0RailNW" },
			{ 304, "rightCurveLarge1RailNW" },
			{ 305, "rightCurveLarge2RailNW" },
			{ 306, "rightCurveLarge3RailNW" },
			{ 307, "rightCurveLarge4RailNW" },
			{ 308, "leftCurveLarge0RailNE" },
			{ 309, "leftCurveLarge1RailNE" },
			{ 310, "leftCurveLarge2RailNE" },
			{ 311, "leftCurveLarge3RailNE" },
			{ 312, "leftCurveLarge4RailNE" },
			{ 313, "leftCurveLarge0RailSE" },
			{ 314, "leftCurveLarge1RailSE" },
			{ 315, "leftCurveLarge2RailSE" },
			{ 316, "leftCurveLarge3RailSE" },
			{ 317, "leftCurveLarge4RailSE" },
			{ 318, "leftCurveLarge0RailSW" },
			{ 319, "leftCurveLarge1RailSW" },
			{ 320, "leftCurveLarge2RailSW" },
			{ 321, "leftCurveLarge3RailSW" },
			{ 322, "leftCurveLarge4RailSW" },
			{ 323, "leftCurveLarge0RailNW" },
			{ 324, "leftCurveLarge1RailNW" },
			{ 325, "leftCurveLarge2RailNW" },
			{ 326, "leftCurveLarge3RailNW" },
			{ 327, "leftCurveLarge4RailNW" },
			{ 328, "diagonal0BallastNE" },
			{ 329, "diagonal1BallastSW" },
			{ 330, "diagonal1BallastNE" },
			{ 331, "diagonal0BallastSW" },
			{ 332, "diagonal0BallastSE" },
			{ 333, "diagonal1BallastNW" },
			{ 334, "diagonal1BallastSE" },
			{ 335, "diagonal0BallastNW" },
			{ 336, "diagonal0SleeperNE" },
			{ 337, "diagonal1SleeperSW" },
			{ 338, "diagonal1SleeperNE" },
			{ 339, "diagonal0SleeperSW" },
			{ 340, "diagonal0SleeperSE" },
			{ 341, "diagonal1SleeperNW" },
			{ 342, "diagonal1SleeperSE" },
			{ 343, "diagonal0SleeperNW" },
			{ 344, "diagonal0RailNE" },
			{ 345, "diagonal1RailSW" },
			{ 346, "diagonal1RailNE" },
			{ 347, "diagonal0RailSW" },
			{ 348, "diagonal0RailSE" },
			{ 349, "diagonal1RailNW" },
			{ 350, "diagonal1RailSE" },
			{ 351, "diagonal0RailNW" },
			{ 352, "sBendLeft0BallastNE" },
			{ 353, "sBendLeft1BallastNE" },
			{ 354, "sBendLeft1BallastSW" },
			{ 355, "sBendLeft0BallastSW" },
			{ 356, "sBendLeft0BallastSE" },
			{ 357, "sBendLeft1BallastSE" },
			{ 358, "sBendLeft1BallastNW" },
			{ 359, "sBendLeft0BallastNW" },
			{ 360, "sBendRight0BallastNE" },
			{ 361, "sBendRight1BallastNE" },
			{ 362, "sBendRight1BallastSW" },
			{ 363, "sBendRight0BallastSW" },
			{ 364, "sBendRight0BallastSE" },
			{ 365, "sBendRight1BallastSE" },
			{ 366, "sBendRight1BallastNW" },
			{ 367, "sBendRight0BallastNW" },
			{ 368, "sBendLeft0SleeperNE" },
			{ 369, "sBendLeft1SleeperNE" },
			{ 370, "sBendLeft1SleeperSW" },
			{ 371, "sBendLeft0SleeperSW" },
			{ 372, "sBendLeft0SleeperSE" },
			{ 373, "sBendLeft1SleeperSE" },
			{ 374, "sBendLeft1SleeperNW" },
			{ 375, "sBendLeft0SleeperNW" },
			{ 376, "sBendRight0SleeperNE" },
			{ 377, "sBendRight1SleeperNE" },
			{ 378, "sBendRight1SleeperSW" },
			{ 379, "sBendRight0SleeperSW" },
			{ 380, "sBendRight0SleeperSE" },
			{ 381, "sBendRight1SleeperSE" },
			{ 382, "sBendRight1SleeperNW" },
			{ 383, "sBendRight0SleeperNW" },
			{ 384, "sBendLeft0RailNE" },
			{ 385, "sBendLeft1RailNE" },
			{ 386, "sBendLeft1RailSW" },
			{ 387, "sBendLeft0RailSW" },
			{ 388, "sBendLeft0RailSE" },
			{ 389, "sBendLeft1RailSE" },
			{ 390, "sBendLeft1RailNW" },
			{ 391, "sBendLeft0RailNW" },
			{ 392, "sBendRight0RailNE" },
			{ 393, "sBendRight1RailNE" },
			{ 394, "sBendRight1RailSW" },
			{ 395, "sBendRight0RailSW" },
			{ 396, "sBendRight0RailSE" },
			{ 397, "sBendRight1RailSE" },
			{ 398, "sBendRight1RailNW" },
			{ 399, "sBendRight0RailNW" },
			{ 400, "rightCurveVerySmall0BallastNE" },
			{ 401, "rightCurveVerySmall0BallastSE" },
			{ 402, "rightCurveVerySmall0BallastSW" },
			{ 403, "rightCurveVerySmall0BallastNW" },
			{ 404, "rightCurveVerySmall0SleeperNE" },
			{ 405, "rightCurveVerySmall0SleeperSE" },
			{ 406, "rightCurveVerySmall0SleeperSW" },
			{ 407, "rightCurveVerySmall0SleeperNW" },
			{ 408, "rightCurveVerySmall0RailNE" },
			{ 409, "rightCurveVerySmall0RailSE" },
			{ 410, "rightCurveVerySmall0RailSW" },
			{ 411, "rightCurveVerySmall0RailNW" },
		};
	}

	class RoadObjectNamer : ImageTableNamer<RoadObject>
	{
		public static RoadObjectNamer Instance { get; } = new();

		public override bool TryGetImageName(RoadObject model, int id, [MaybeNullWhen(false)] out string value)
		{
			if (id is >= 0 and <= 31)
			{
				value = $"uiPreviewImage{id}";
				return true;
			}

			if (id is >= 32 and <= 33)
			{
				return ImageIdNameMap.TryGetValue(id, out value);
			}

			// style dependent
			return model.PaintStyle switch
			{
				0 => ImageIdNameMap_Style0.TryGetValue(id, out value),
				1 => ImageIdNameMap_Style1.TryGetValue(id, out value),
				2 => ImageIdNameMap_Style2.TryGetValue(id, out value),
				_ => throw new NotImplementedException(id.ToString()),
			};
		}

		static readonly Dictionary<int, string> ImageIdNameMap = new()
		{
			{ 32, "uiPickupFromTrack" },
			{ 33, "uiPlaceOnTrack" },
		};

		static readonly Dictionary<int, string> ImageIdNameMap_Style0 = new()
		{
			{ 34, "kStraight0NE" },
			{ 35, "kStraight0SE" },
			{ 36, "kRightCurveVerySmall0NE" },
			{ 37, "kRightCurveVerySmall0SE" },
			{ 38, "kRightCurveVerySmall0SW" },
			{ 39, "kRightCurveVerySmall0NW" },
			{ 40, "kJunctionLeft0NE" },
			{ 41, "kJunctionLeft0SE" },
			{ 42, "kJunctionLeft0SW" },
			{ 43, "kJunctionLeft0NW" },
			{ 44, "kJunctionCrossroads0NE" },
			{ 45, "kRightCurveSmall0NE" },
			{ 46, "kRightCurveSmall1NE" },
			{ 47, "kRightCurveSmall2NE" },
			{ 48, "kRightCurveSmall3NE" },
			{ 49, "kRightCurveSmall0SE" },
			{ 50, "kRightCurveSmall1SE" },
			{ 51, "kRightCurveSmall2SE" },
			{ 52, "kRightCurveSmall3SE" },
			{ 53, "kRightCurveSmall0SW" },
			{ 54, "kRightCurveSmall1SW" },
			{ 55, "kRightCurveSmall2SW" },
			{ 56, "kRightCurveSmall3SW" },
			{ 57, "kRightCurveSmall0NW" },
			{ 58, "kRightCurveSmall1NW" },
			{ 59, "kRightCurveSmall2NW" },
			{ 60, "kRightCurveSmall3NW" },
			{ 61, "kStraightSlopeUp0NE" },
			{ 62, "kStraightSlopeUp1NE" },
			{ 63, "kStraightSlopeUp0SE" },
			{ 64, "kStraightSlopeUp1SE" },
			{ 65, "kStraightSlopeUp0SW" },
			{ 66, "kStraightSlopeUp1SW" },
			{ 67, "kStraightSlopeUp0NW" },
			{ 68, "kStraightSlopeUp1NW" },
			{ 69, "kStraightSteepSlopeUp0NE" },
			{ 70, "kStraightSteepSlopeUp0SE" },
			{ 71, "kStraightSteepSlopeUp0SW" },
			{ 72, "kStraightSteepSlopeUp0NW" },
			{ 73, "kTurnaround0NE" },
			{ 74, "kTurnaround0SE" },
			{ 75, "kTurnaround0SW" },
			{ 76, "kTurnaround0NW" },
		};

		static readonly Dictionary<int, string> ImageIdNameMap_Style1 = new()
		{
			{ 34, "kStraight0BallastNE" },
			{ 35, "kStraight0BallastSE" },
			{ 36, "kStraight0SleeperNE" },
			{ 37, "kStraight0SleeperSE" },
			{ 38, "kStraight0RailNE" },
			{ 39, "kStraight0RailSE" },
			{ 40, "kRightCurveSmall0BallastNE" },
			{ 41, "kRightCurveSmall1BallastNE" },
			{ 42, "kRightCurveSmall2BallastNE" },
			{ 43, "kRightCurveSmall3BallastNE" },
			{ 44, "kRightCurveSmall0BallastSE" },
			{ 45, "kRightCurveSmall1BallastSE" },
			{ 46, "kRightCurveSmall2BallastSE" },
			{ 47, "kRightCurveSmall3BallastSE" },
			{ 48, "kRightCurveSmall0BallastSW" },
			{ 49, "kRightCurveSmall1BallastSW" },
			{ 50, "kRightCurveSmall2BallastSW" },
			{ 51, "kRightCurveSmall3BallastSW" },
			{ 52, "kRightCurveSmall0BallastNW" },
			{ 53, "kRightCurveSmall1BallastNW" },
			{ 54, "kRightCurveSmall2BallastNW" },
			{ 55, "kRightCurveSmall3BallastNW" },
			{ 56, "kRightCurveSmall0SleeperNE" },
			{ 57, "kRightCurveSmall1SleeperNE" },
			{ 58, "kRightCurveSmall2SleeperNE" },
			{ 59, "kRightCurveSmall3SleeperNE" },
			{ 60, "kRightCurveSmall0SleeperSE" },
			{ 61, "kRightCurveSmall1SleeperSE" },
			{ 62, "kRightCurveSmall2SleeperSE" },
			{ 63, "kRightCurveSmall3SleeperSE" },
			{ 64, "kRightCurveSmall0SleeperSW" },
			{ 65, "kRightCurveSmall1SleeperSW" },
			{ 66, "kRightCurveSmall2SleeperSW" },
			{ 67, "kRightCurveSmall3SleeperSW" },
			{ 68, "kRightCurveSmall0SleeperNW" },
			{ 69, "kRightCurveSmall1SleeperNW" },
			{ 70, "kRightCurveSmall2SleeperNW" },
			{ 71, "kRightCurveSmall3SleeperNW" },
			{ 72, "kRightCurveSmall0RailNE" },
			{ 73, "kRightCurveSmall1RailNE" },
			{ 74, "kRightCurveSmall2RailNE" },
			{ 75, "kRightCurveSmall3RailNE" },
			{ 76, "kRightCurveSmall0RailSE" },
			{ 77, "kRightCurveSmall1RailSE" },
			{ 78, "kRightCurveSmall2RailSE" },
			{ 79, "kRightCurveSmall3RailSE" },
			{ 80, "kRightCurveSmall0RailSW" },
			{ 81, "kRightCurveSmall1RailSW" },
			{ 82, "kRightCurveSmall2RailSW" },
			{ 83, "kRightCurveSmall3RailSW" },
			{ 84, "kRightCurveSmall0RailNW" },
			{ 85, "kRightCurveSmall1RailNW" },
			{ 86, "kRightCurveSmall2RailNW" },
			{ 87, "kRightCurveSmall3RailNW" },
			{ 88, "kStraightSlopeUp0BallastNE" },
			{ 89, "kStraightSlopeUp1BallastNE" },
			{ 90, "kStraightSlopeUp0BallastSE" },
			{ 91, "kStraightSlopeUp1BallastSE" },
			{ 92, "kStraightSlopeUp0BallastSW" },
			{ 93, "kStraightSlopeUp1BallastSW" },
			{ 94, "kStraightSlopeUp0BallastNW" },
			{ 95, "kStraightSlopeUp1BallastNW" },
			{ 96, "kStraightSlopeUp0SleeperNE" },
			{ 97, "kStraightSlopeUp1SleeperNE" },
			{ 98, "kStraightSlopeUp0SleeperSE" },
			{ 99, "kStraightSlopeUp1SleeperSE" },
			{ 100, "kStraightSlopeUp0SleeperSW" },
			{ 101, "kStraightSlopeUp1SleeperSW" },
			{ 102, "kStraightSlopeUp0SleeperNW" },
			{ 103, "kStraightSlopeUp1SleeperNW" },
			{ 104, "kStraightSlopeUp0RailNE" },
			{ 105, "kStraightSlopeUp1RailNE" },
			{ 106, "kStraightSlopeUp0RailSE" },
			{ 107, "kStraightSlopeUp1RailSE" },
			{ 108, "kStraightSlopeUp0RailSW" },
			{ 109, "kStraightSlopeUp1RailSW" },
			{ 110, "kStraightSlopeUp0RailNW" },
			{ 111, "kStraightSlopeUp1RailNW" },
			{ 112, "kStraightSteepSlopeUp0BallastNE" },
			{ 113, "kStraightSteepSlopeUp0BallastSE" },
			{ 114, "kStraightSteepSlopeUp0BallastSW" },
			{ 115, "kStraightSteepSlopeUp0BallastNW" },
			{ 116, "kStraightSteepSlopeUp0SleeperNE" },
			{ 117, "kStraightSteepSlopeUp0SleeperSE" },
			{ 118, "kStraightSteepSlopeUp0SleeperSW" },
			{ 119, "kStraightSteepSlopeUp0SleeperNW" },
			{ 120, "kStraightSteepSlopeUp0RailNE" },
			{ 121, "kStraightSteepSlopeUp0RailSE" },
			{ 122, "kStraightSteepSlopeUp0RailSW" },
			{ 123, "kStraightSteepSlopeUp0RailNW" },
			{ 124, "kRightCurveVerySmall0BallastNE" },
			{ 125, "kRightCurveVerySmall0BallastSE" },
			{ 126, "kRightCurveVerySmall0BallastSW" },
			{ 127, "kRightCurveVerySmall0BallastNW" },
			{ 128, "kRightCurveVerySmall0SleeperNE" },
			{ 129, "kRightCurveVerySmall0SleeperSE" },
			{ 130, "kRightCurveVerySmall0SleeperSW" },
			{ 131, "kRightCurveVerySmall0SleeperNW" },
			{ 132, "kRightCurveVerySmall0RailNE" },
			{ 133, "kRightCurveVerySmall0RailSE" },
			{ 134, "kRightCurveVerySmall0RailSW" },
			{ 135, "kRightCurveVerySmall0RailNW" },
			{ 136, "kTurnaround0BallastNE" },
			{ 137, "kTurnaround0BallastSE" },
			{ 138, "kTurnaround0BallastSW" },
			{ 139, "kTurnaround0BallastNW" },
			{ 140, "kTurnaround0SleeperNE" },
			{ 141, "kTurnaround0SleeperSE" },
			{ 142, "kTurnaround0SleeperSW" },
			{ 143, "kTurnaround0SleeperNW" },
			{ 144, "kTurnaround0RailNE" },
			{ 145, "kTurnaround0RailSE" },
			{ 146, "kTurnaround0RailSW" },
			{ 147, "kTurnaround0RailNW" },
		};

		static readonly Dictionary<int, string> ImageIdNameMap_Style2 = new()
		{
			{ 34, "kStraight0NE" },
			{ 35, "kStraight0SE" },
			{ 36, "kLeftCurveVerySmall0NW" },
			{ 37, "kLeftCurveVerySmall0NE" },
			{ 38, "kLeftCurveVerySmall0SE" },
			{ 39, "kLeftCurveVerySmall0SW" },
			{ 40, "kJunctionLeft0NE" },
			{ 41, "kJunctionLeft0SE" },
			{ 42, "kJunctionLeft0SW" },
			{ 43, "kJunctionLeft0NW" },
			{ 44, "kJunctionCrossroads0NE" },
			{ 45, "kLeftCurveSmall3NW" },
			{ 46, "kLeftCurveSmall1NW" },
			{ 47, "kLeftCurveSmall2NW" },
			{ 48, "kLeftCurveSmall0NW" },
			{ 49, "kLeftCurveSmall3NE" },
			{ 50, "kLeftCurveSmall1NE" },
			{ 51, "kLeftCurveSmall2NE" },
			{ 52, "kLeftCurveSmall0NE" },
			{ 53, "kLeftCurveSmall3SE" },
			{ 54, "kLeftCurveSmall1SE" },
			{ 55, "kLeftCurveSmall2SE" },
			{ 56, "kLeftCurveSmall0SE" },
			{ 57, "kLeftCurveSmall3SW" },
			{ 58, "kLeftCurveSmall1SW" },
			{ 59, "kLeftCurveSmall2SW" },
			{ 60, "kLeftCurveSmall0SW" },
			{ 61, "kStraightSlopeUp0NE" },
			{ 62, "kStraightSlopeUp1NE" },
			{ 63, "kStraightSlopeUp0SE" },
			{ 64, "kStraightSlopeUp1SE" },
			{ 65, "kStraightSlopeUp0SW" },
			{ 66, "kStraightSlopeUp1SW" },
			{ 67, "kStraightSlopeUp0NW" },
			{ 68, "kStraightSlopeUp1NW" },
			{ 69, "kStraightSteepSlopeUp0NE" },
			{ 70, "kStraightSteepSlopeUp0SE" },
			{ 71, "kStraightSteepSlopeUp0SW" },
			{ 72, "kStraightSteepSlopeUp0NW" },
			{ 73, "kTurnaround0NE" },
			{ 74, "kTurnaround0SE" },
			{ 75, "kTurnaround0SW" },
			{ 76, "kTurnaround0NW" },

			{ 85, "kStraight0SW" },
			{ 86, "kStraight0NW" },
			{ 87, "kRightCurveVerySmall0NE" },
			{ 88, "kRightCurveVerySmall0SE" },
			{ 89, "kRightCurveVerySmall0SW" },
			{ 90, "kRightCurveVerySmall0NW" },
			{ 91, "kJunctionRight0NE" },
			{ 92, "kJunctionRight0SE" },
			{ 93, "kJunctionRight0SW" },
			{ 94, "kJunctionRight0NW" },
			// Must duplicate kJunctionCrossroads0NE
			{ 95, "kJunctionCrossroads0NE2" },
			{ 96, "kRightCurveSmall0NE" },
			{ 97, "kRightCurveSmall1NE" },
			{ 98, "kRightCurveSmall2NE" },
			{ 99, "kRightCurveSmall3NE" },
			{ 100, "kRightCurveSmall0SE" },
			{ 101, "kRightCurveSmall1SE" },
			{ 102, "kRightCurveSmall2SE" },
			{ 103, "kRightCurveSmall3SE" },
			{ 104, "kRightCurveSmall0SW" },
			{ 105, "kRightCurveSmall1SW" },
			{ 106, "kRightCurveSmall2SW" },
			{ 107, "kRightCurveSmall3SW" },
			{ 108, "kRightCurveSmall0NW" },
			{ 109, "kRightCurveSmall1NW" },
			{ 110, "kRightCurveSmall2NW" },
			{ 111, "kRightCurveSmall3NW" },
			{ 112, "kStraightSlopeDown1SW" },
			{ 113, "kStraightSlopeDown0SW" },
			{ 114, "kStraightSlopeDown1NW" },
			{ 115, "kStraightSlopeDown0NW" },
			{ 116, "kStraightSlopeDown1NE" },
			{ 117, "kStraightSlopeDown0NE" },
			{ 118, "kStraightSlopeDown1SE" },
			{ 119, "kStraightSlopeDown0SE" },
			{ 120, "kStraightSteepSlopeDown0SW" },
			{ 121, "kStraightSteepSlopeDown0NW" },
			{ 122, "kStraightSteepSlopeDown0NE" },
			{ 123, "kStraightSteepSlopeDown0SE" },
		};
	}

	public static void NameImages(ILocoStruct obj, ObjectType objectType, List<GraphicsElement> imageList)
	{
		IImageTableNameProvider imageNamer = objectType switch
		{
			ObjectType.InterfaceSkin => InterfaceSkinObjectNamer.Instance,
			ObjectType.Sound => DefaultImageTableNameProvider.Instance,
			ObjectType.Currency => DefaultImageTableNameProvider.Instance,
			ObjectType.Steam => DefaultImageTableNameProvider.Instance,
			ObjectType.CliffEdge => CliffEdgeObjectNamer.Instance,
			ObjectType.Water => WaterObjectNamer.Instance,
			ObjectType.Land => LandObjectNamer.Instance,
			ObjectType.TownNames => DefaultImageTableNameProvider.Instance,
			ObjectType.Cargo => CargoObjectNamer.Instance,
			ObjectType.Wall => WallObjectNamer.Instance,
			ObjectType.TrackSignal => TrackSignalObjectNamer.Instance,
			ObjectType.LevelCrossing => DefaultImageTableNameProvider.Instance,
			ObjectType.StreetLight => StreetLightObjectNamer.Instance,
			ObjectType.Tunnel => DefaultImageTableNameProvider.Instance,
			ObjectType.Bridge => DefaultImageTableNameProvider.Instance,
			ObjectType.TrackStation => TrackSignalObjectNamer.Instance,
			ObjectType.TrackExtra => TrackExtraObjectNamer.Instance,
			ObjectType.Track => TrackObjectNamer.Instance,
			ObjectType.RoadStation => RoadStationObjectNamer.Instance,
			ObjectType.RoadExtra => DefaultImageTableNameProvider.Instance,
			ObjectType.Road => RoadObjectNamer.Instance,
			ObjectType.Airport => DefaultImageTableNameProvider.Instance,
			ObjectType.Dock => DefaultImageTableNameProvider.Instance,
			ObjectType.Vehicle => DefaultImageTableNameProvider.Instance,
			ObjectType.Tree => DefaultImageTableNameProvider.Instance,
			ObjectType.Snow => DefaultImageTableNameProvider.Instance,
			ObjectType.Climate => DefaultImageTableNameProvider.Instance,
			ObjectType.HillShapes => HillShapesObjectNamer.Instance,
			ObjectType.Building => DefaultImageTableNameProvider.Instance,
			ObjectType.Scaffolding => DefaultImageTableNameProvider.Instance,
			ObjectType.Industry => DefaultImageTableNameProvider.Instance,
			ObjectType.Region => DefaultImageTableNameProvider.Instance,
			ObjectType.Competitor => CompetitorObjectNamer.Instance,
			ObjectType.ScenarioText => DefaultImageTableNameProvider.Instance,
			_ => DefaultImageTableNameProvider.Instance,
		};

		for (var i = 0; i < imageList.Count; ++i)
		{
			imageList[i].Name = imageNamer.TryGetImageName(obj, i, out var name)
				? name!
				: DefaultImageTableNameProvider.GetImageName(i);
		}
	}
}

public static class ImageTableGrouper
{
	public static ImageTable CreateImageTable(ILocoStruct obj, ObjectType objectType, List<GraphicsElement> imageList)
	{
		ImageTableNamer.NameImages(obj, objectType, imageList);

		var imageTable = new ImageTable();

		switch (objectType)
		{
			case ObjectType.InterfaceSkin:
				CreateInterfaceGroups(imageList, imageTable);
				break;
			case ObjectType.Sound:
				imageTable.Groups.Add(("<none>", imageList.ToList()));
				break;
			case ObjectType.Currency:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.Steam:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.CliffEdge:
				CreateCliffEdgeGroups(imageList, imageTable);
				break;
			case ObjectType.Water:
				CreateWaterGroups(imageList, imageTable);
				break;
			case ObjectType.Land:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.TownNames:
				imageTable.Groups.Add(("<none>", imageList.ToList()));
				break;
			case ObjectType.Cargo:
				CreateCargoGroups(imageList, imageTable);
				break;
			case ObjectType.Wall:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.TrackSignal:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.LevelCrossing:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.StreetLight:
				CreateStreetLightGroups(imageList, imageTable);
				break;
			case ObjectType.Tunnel:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.Bridge:
				CreateBridgeGroups(imageList, imageTable);
				break;
			case ObjectType.TrackStation:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.TrackExtra:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.Track:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.RoadStation:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.RoadExtra:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.Road:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.Airport:
				CreateAirportGroups(imageList, imageTable);
				break;
			case ObjectType.Dock:
				CreateDockGroups(imageList, imageTable);
				break;
			case ObjectType.Vehicle:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.Tree:
				CreateTreeGroups(imageList, imageTable);
				break;
			case ObjectType.Snow:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.Climate:
				imageTable.Groups.Add(("<none>", imageList.ToList()));
				break;
			case ObjectType.HillShapes:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.Building:
				CreateBuildingGroups(imageList, imageTable);
				break;
			case ObjectType.Scaffolding:
				CreateScaffoldingGroups(imageList, imageTable);
				break;
			case ObjectType.Industry:
				CreateBuildingGroups(imageList, imageTable);
				break;
			case ObjectType.Region:
				imageTable.Groups.Add(("<uncategorised>", imageList.ToList()));
				break;
			case ObjectType.Competitor:
				CreateCompetitorGroups(imageList, imageTable);
				break;
			case ObjectType.ScenarioText:
				imageTable.Groups.Add(("<none>", imageList.ToList()));
				break;
			default:
				break;
		}

		return imageTable;
	}

	private static void CreateAirportGroups(List<GraphicsElement> imageList, ImageTable imageTable)
	{
		imageTable.Groups.Add(("preview", imageList[0..1]));

		imageTable.Groups.AddRange(imageList
			.Skip(1)
			.Chunk(4)
			.Select((x, i) => ($"Part {i}", x.ToList()))
			.ToList());
	}

	private static void CreateBridgeGroups(List<GraphicsElement> imageList, ImageTable imageTable)
	{
		imageTable.Groups.Add(("preview", imageList[0..1]));
		imageTable.Groups.Add(("base plates", imageList[1..5]));
		imageTable.Groups.Add(("unk", imageList[6..11]));
		imageTable.Groups.Add(("<uncategorised>", imageList[12..]));
	}

	private static void CreateBuildingGroups(List<GraphicsElement> imageList, ImageTable imageTable)
		=> imageTable.Groups = imageList
			.Chunk(4)
			.Select((x, i) => ($"Part {i}", x.ToList()))
			.ToList();

	private static void CreateCargoGroups(List<GraphicsElement> imageList, ImageTable imageTable)
	{
		imageTable.Groups.Add(("preview", imageList[0..1]));
		imageTable.Groups.Add(("station variations", imageList[1..]));
	}

	private static void CreateCliffEdgeGroups(List<GraphicsElement> imageList, ImageTable imageTable)
	{
		imageTable.Groups.Add(("left west", imageList[0..16]));
		imageTable.Groups.Add(("right east", imageList[16..32]));
		imageTable.Groups.Add(("right west", imageList[32..48]));
		imageTable.Groups.Add(("left east", imageList[48..64]));
		imageTable.Groups.Add(("far-side slopes", imageList[64..]));
	}

	private static void CreateCompetitorGroups(List<GraphicsElement> imageList, ImageTable imageTable)
	{
		imageTable.Groups.Add(("neutral", imageList[0..2]));
		imageTable.Groups.Add(("happy", imageList[2..4]));
		imageTable.Groups.Add(("worried", imageList[4..6]));
		imageTable.Groups.Add(("thinking", imageList[6..8]));
		imageTable.Groups.Add(("dejected", imageList[8..10]));
		imageTable.Groups.Add(("surprised", imageList[10..12]));
		imageTable.Groups.Add(("scared", imageList[12..14]));
		imageTable.Groups.Add(("angry", imageList[14..16]));
		imageTable.Groups.Add(("disgusted", imageList[16..18]));
	}

	private static void CreateDockGroups(List<GraphicsElement> imageList, ImageTable imageTable)
	{
		imageTable.Groups.Add(("preview", [imageList[0]]));

		imageTable.Groups.AddRange(imageList
			.Skip(1)
			.Chunk(4)
			.Select((x, i) => ($"Part {i}", x.ToList()))
			.ToList());
	}

	private static void CreateInterfaceGroups(List<GraphicsElement> imageList, ImageTable imageTable)
	{
		imageTable.Groups.Add(("preview", imageList[0..1]));
		imageTable.Groups.Add(("toolbar", imageList[1..31]));
		imageTable.Groups.Add(("build-vehicle", imageList[31..43]));
		imageTable.Groups.Add(("toolbar", imageList[43..49]));
		imageTable.Groups.Add(("paint", imageList[49..55]));
		imageTable.Groups.Add(("population", imageList[57..65]));
		imageTable.Groups.Add(("performance-index", imageList[65..73]));
		imageTable.Groups.Add(("cargo-units", imageList[73..81]));
		imageTable.Groups.Add(("cargo-distance", imageList[81..89]));
		imageTable.Groups.Add(("production", imageList[89..97]));
		imageTable.Groups.Add(("wrench", imageList[97..113]));
		imageTable.Groups.Add(("finances", imageList[113..129]));
		imageTable.Groups.Add(("cup", imageList[129..145]));
		imageTable.Groups.Add(("ratings", imageList[145..161]));
		imageTable.Groups.Add(("transported", imageList[161..168]));
		imageTable.Groups.Add(("cogs", imageList[168..172]));
		imageTable.Groups.Add(("toolbar", imageList[172..203]));
		imageTable.Groups.Add(("tab-train", imageList[203..211]));
		imageTable.Groups.Add(("tab-aircraft", imageList[211..219]));
		imageTable.Groups.Add(("tab-bus", imageList[219..227]));
		imageTable.Groups.Add(("tab-tram", imageList[227..235]));
		imageTable.Groups.Add(("tab-truck", imageList[235..243]));
		imageTable.Groups.Add(("tab-ship", imageList[243..251]));
		imageTable.Groups.Add(("build-train", imageList[251..267]));
		imageTable.Groups.Add(("build-aircraft", imageList[267..283]));
		imageTable.Groups.Add(("build-bus", imageList[283..299]));
		imageTable.Groups.Add(("build-tram", imageList[299..315]));
		imageTable.Groups.Add(("build-truck", imageList[315..331]));
		imageTable.Groups.Add(("build-ship", imageList[331..347]));
		imageTable.Groups.Add(("build-industry", imageList[347..363]));
		imageTable.Groups.Add(("build-town", imageList[363..379]));
		imageTable.Groups.Add(("build-buildings", imageList[379..395]));
		imageTable.Groups.Add(("build-misc-buildings", imageList[395..411]));
		imageTable.Groups.Add(("build-extra", imageList[411..418]));
		imageTable.Groups.Add(("train", imageList[418..426]));
		imageTable.Groups.Add(("aircraft", imageList[426..434]));
		imageTable.Groups.Add(("bus", imageList[434..442]));
		imageTable.Groups.Add(("tram", imageList[442..450]));
		imageTable.Groups.Add(("truck", imageList[450..458]));
		imageTable.Groups.Add(("ship", imageList[458..466]));
		imageTable.Groups.Add(("toolbar-map", imageList[466..470]));
	}

	private static void CreateScaffoldingGroups(List<GraphicsElement> imageList, ImageTable imageTable)
	{
		imageTable.Groups.Add(("type 0", imageList[0..10]));
		imageTable.Groups.Add(("type 1", imageList[10..24]));
		imageTable.Groups.Add(("type 2", imageList[24..36]));
	}

	private static void CreateStreetLightGroups(List<GraphicsElement> imageList, ImageTable imageTable)
		=> imageTable.Groups.AddRange(imageList
			.Chunk(4)
			.Select((x, i) => ($"Year group {i}", x.ToList()))
			.ToList());

	private static void CreateTreeGroups(List<GraphicsElement> imageList, ImageTable imageTable)
		=> imageTable.Groups.AddRange(imageList
			.Chunk(4)
			.Select((x, i) => ($"Variation {i}", x.ToList()))
			.ToList());

	private static void CreateWaterGroups(List<GraphicsElement> imageList, ImageTable imageTable)
	{
		imageTable.Groups.Add(("zoom 1", imageList[0..10]));
		imageTable.Groups.Add(("zoom 2", imageList[10..20]));
		imageTable.Groups.Add(("zoom 3", imageList[20..30]));
		imageTable.Groups.Add(("zoom 4", imageList[30..40]));
		imageTable.Groups.Add(("palettes", imageList[40..42]));
		imageTable.Groups.Add(("icon-animation", imageList[42..58]));
		imageTable.Groups.Add(("icon-interaction", imageList[58..60]));
		imageTable.Groups.Add(("animation", imageList[60..76]));
	}
}
