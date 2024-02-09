﻿using System.ComponentModel;
using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor.DatFileParsing;
using OpenLoco.ObjectEditor.Types;

namespace OpenLoco.ObjectEditor.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x18)]
	[LocoStructType(ObjectType.InterfaceSkin)]
	[LocoStringTable("Name")]
	public record InterfaceSkinObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02), Browsable(false)] image_id Image,
		[property: LocoStructOffset(0x06)] Colour Colour_06,
		[property: LocoStructOffset(0x07)] Colour Colour_07,
		[property: LocoStructOffset(0x08)] Colour TooltipColour,
		[property: LocoStructOffset(0x09)] Colour ErrorColour,
		[property: LocoStructOffset(0x0A)] Colour Colour_0A,
		[property: LocoStructOffset(0x0B)] Colour Colour_0B,
		[property: LocoStructOffset(0x0C)] Colour Colour_0C,
		[property: LocoStructOffset(0x0D)] Colour Colour_0D,
		[property: LocoStructOffset(0x0E)] Colour Colour_0E,
		[property: LocoStructOffset(0x0F)] Colour Colour_0F,
		[property: LocoStructOffset(0x10)] Colour Colour_10,
		[property: LocoStructOffset(0x11)] Colour Colour_11,
		[property: LocoStructOffset(0x12)] Colour Colour_12,
		[property: LocoStructOffset(0x13)] Colour Colour_13,
		[property: LocoStructOffset(0x14)] Colour Colour_14,
		[property: LocoStructOffset(0x15)] Colour Colour_15,
		[property: LocoStructOffset(0x16)] Colour Colour_16,
		[property: LocoStructOffset(0x17)] Colour Colour_17
		) : ILocoStruct, IImageTableStrings
	{
		public bool TryGetImageName(int id, out string? value)
			=> ImageIdNameMap.TryGetValue(id, out value);

		public static Dictionary<int, string> ImageIdNameMap = new()
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
}
