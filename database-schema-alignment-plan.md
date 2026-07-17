# Database Schema Alignment Plan

## Objective

Update the database schema files in `/Definitions/Database/DataTables/Objects/` to match the actual object models in `/Definitions/ObjectModels/Objects/`, so that all object properties can be stored in the database.

## Design Rules

1. **Lists/Arrays → JSON columns** (not sub-tables for denormalization)
2. **Sub-object properties (non-POCO) → JSON columns**
3. **ObjectModelHeader references** → Foreign keys to the appropriate object table:
    - Single `ObjectModelHeader` → simple FK column
    - `List<ObjectModelHeader>` → junction table
4. **Flags enums** → JSON array of individual flag names (e.g., `["HasHeight", "IsCustom"]`)
5. **Simple enums** → Text representation (e.g., `"Winter"`, `"Left"`, `"Rail"`)
6. **Binary blobs** → `byte[]` columns (native EF Core + SQLite)
7. **Derivable Count properties** → Remove
8. **BuildingComponents** → Inline JSON column
9. **Sub-objects with embedded ObjectModelHeaders** → Normalized into their own tables (e.g., EmitterAnimation gets its own FK to SteamObject)

## Infrastructure Changes

### New Junction Table Folder

Create `Definitions/Database/DataTables/JunctionTables/` containing:

- `TblBridgeCompatibleTrack.cs`
- `TblBridgeCompatibleRoad.cs`
- `TblVehicleCompatibleVehicle.cs`
- `TblVehicleRequiredTrackExtra.cs`
- `TblVehicleStartSound.cs`
- `TblBuildingProducedCargo.cs`
- `TblBuildingConsumedCargo.cs`
- `TblIndustryProducedCargo.cs`
- `TblIndustryRequiredCargo.cs`
- `TblIndustryWallType.cs`
- `TblTrackTrackMod.cs`
- `TblTrackSignal.cs`
- `TblTrackCompatibleTrackRoad.cs`
- `TblTrackBridge.cs`
- `TblTrackStation.cs`
- `TblRoadBridge.cs`
- `TblRoadStation.cs`
- `TblRoadRoadMod.cs`
- `TblRoadCompatibleTrackRoad.cs`
- `TblTrackStationCompatibleTrack.cs`
- `TblTrackSignalCompatibleTrack.cs`
- `TblRoadStationCompatibleRoad.cs`
- `TblRegionCargoInfluence.cs`
- `TblRegionDependentObject.cs`
- `TblSteamSoundEffect.cs`

### New Normalized Sub-Object Tables

Create `Definitions/Database/DataTables/NormalizedTables/` containing:

- `TblVehicleEmitterAnimation.cs` (FK→TblObjectVehicle, FK→TblObjectSteam)

### LocoDbContext Updates

- Add `DbSet<>` properties for each junction table and normalized table
- Add composite key configuration in `OnModelCreating`

## Per-Object Plan

### Already Complete (no changes needed)

| File                   | Notes                       |
| ---------------------- | --------------------------- |
| TblObjectCargo         | Minor: missing `UnitWeight` |
| TblObjectClimate       | ✅ Complete                 |
| TblObjectCompetitor    | ✅ Complete                 |
| TblObjectCurrency      | ✅ Complete                 |
| TblObjectInterface     | ✅ Complete                 |
| TblObjectHillShapes    | ✅ Complete                 |
| TblObjectTree          | ✅ Complete                 |
| TblObjectWall          | ✅ Complete                 |
| TblObjectTunnel        | ✅ Complete (no data)       |
| TblObjectSnow          | ✅ Complete (no data)       |
| TblObjectCliffEdge     | ✅ Complete (no data)       |
| TblObjectScenarioText  | ✅ Complete (no data)       |
| TblObjectTrackExtra    | ✅ Complete                 |
| TblObjectRoadExtra     | ✅ Complete                 |
| TblObjectLevelCrossing | ✅ Complete                 |

### 1. TblObjectBridge

**Changes:**

- Add junction tables for `CompatibleTrackObjects` and `CompatibleRoadObjects`

### 2. TblObjectVehicle (largest change)

**Scalar columns:**

- Keep: `Mode`, `Type`, `NumCarComponents`, `TrackTypeId`, `CostIndex`, `CostFactor`, `Reliability`, `RunCostIndex`, `RunCostFactor`, `Power`, `Speed`, `RackSpeed`, `Weight`, `Flags`, `ShipWakeSpacing`, `DesignedYear`, `ObsoleteYear`, `DrivingSoundType`
- Remove: `NumRequiredTrackExtras`, `NumCompatibleVehicles` (derivable)
- Add: `CompanyColourSchemeIndex` (string), `RackRailTypeId` (nullable FK→TblObjectTrackExtra)
- Add: `RoadOrTrackTypeId` (nullable FK→TblObjectTrack or TblObjectRoad)

**FK columns:**

- `RackRailTypeId` (nullable FK→TblObjectTrackExtra)
- `RoadOrTrackTypeId` (nullable FK→TblObjectTrack/Road)
- `SoundId` (nullable FK→TblObjectSound)
- `FrictionSoundObjectId` (nullable FK→TblObjectSound)
- `SimpleMotorSoundObjectId` (nullable FK→TblObjectSound)
- `GearboxMotorSoundObjectId` (nullable FK→TblObjectSound)

**JSON columns:**

- `CarComponents` (List<VehicleObjectCar>)
- `BodySprites` (List<BodySprite>)
- `BogieSprites` (List<BogieSprite>)
- `MaxCargo` (List<uint8_t>)
- `CompatibleCargoCategories` (List<CargoCategory>[])
- `CargoTypeSpriteOffsets` (Dictionary<CargoCategory, uint8_t>)
- `FrictionSoundData` (FrictionSound?)
- `SimpleMotorSoundData` (SimpleMotorSound?)
- `GearboxMotorSoundData` (GearboxMotorSound?)
- `var_135` (uint8_t[])

**Junction tables:**

- `TblVehicleCompatibleVehicle` (FK→TblObjectVehicle, FK→TblObjectVehicle)
- `TblVehicleRequiredTrackExtra` (FK→TblObjectVehicle, FK→TblObjectTrackExtra)
- `TblVehicleStartSound` (FK→TblObjectVehicle, FK→TblObjectSound)

**Normalized sub-table:**

- `TblVehicleEmitterAnimation` (FK→TblObjectVehicle, FK→TblObjectSteam, plus scalar columns for EmitterVerticalPos, SimpleAnimationType)

### 3. TblObjectBuilding

**Scalar columns:**

- Remove: `NumBuildingParts`, `NumBuildingVariations` (derivable from BuildingComponents)
- Add: `TownAmenityCategory` (string)

**JSON columns:**

- `BuildingComponents` (BuildingComponents → contains BuildingHeights, BuildingAnimations, BuildingVariations)
- `ProducedQuantity` (List<uint8_t>)
- `ProducedCargoQuantity` (List<uint8_t>)
- `ConsumedCargoQuantity` (List<uint8_t>)
- `ElevatorHeightSequences` (List<uint8_t[]>)

**Junction tables:**

- `TblBuildingProducedCargo` (FK→TblObjectBuilding, FK→TblObjectCargo)
- `TblBuildingConsumedCargo` (FK→TblObjectBuilding, FK→TblObjectCargo)

### 4. TblObjectAirport

**Scalar columns:**

- Add: `var_B6` (uint32_t)

**JSON columns:**

- `BuildingComponents` (BuildingComponents)
- `BuildingPositions` (List<AirportBuilding>)
- `MovementNodes` (List<MovementNode>)
- `MovementEdges` (List<MovementEdge>)

### 5. TblObjectIndustry

**Scalar columns:**

- Add: `var_E8` (uint8_t)
- Rename: `FarmIdealSize` → `FarmNumFields` to match model

**JSON columns:**

- `BuildingComponents` (BuildingComponents)
- `AnimationSequences` (List<List<uint8_t>>)
- `var_38` (List<IndustryObjectUnk38>)
- `InitialProductionRate` (List<IndustryObjectProductionRateRange>)
- `Buildings` (List<uint8_t>)

**FK columns:**

- `BuildingWallId` (nullable FK→TblObjectWall)
- `BuildingWallEntranceId` (nullable FK→TblObjectWall)

**Junction tables:**

- `TblIndustryProducedCargo` (FK→TblObjectIndustry, FK→TblObjectCargo)
- `TblIndustryRequiredCargo` (FK→TblObjectIndustry, FK→TblObjectCargo)
- `TblIndustryWallType` (FK→TblObjectIndustry, FK→TblObjectWall)

### 6. TblObjectTrack

**Scalar columns:**

- Add: `var_06` (uint8_t)

**FK columns:**

- `TunnelId` (FK→TblObjectTunnel, required)

**Junction tables:**

- `TblTrackTrackMod` (FK→TblObjectTrack, FK→TblObjectTrackExtra)
- `TblTrackSignal` (FK→TblObjectTrack, FK→TblObjectTrackSignal)
- `TblTrackCompatibleTrackRoad` (FK→TblObjectTrack, FK→TblObjectTrack or TblObjectRoad)
- `TblTrackBridge` (FK→TblObjectTrack, FK→TblObjectBridge)
- `TblTrackStation` (FK→TblObjectTrack, FK→TblObjectTrackStation)

### 7. TblObjectRoad

**Scalar columns:**

- Add: `pad_2F` (uint8_t)
- Rename: `MaxSpeed` → `MaxCurveSpeed` to match model

**FK columns:**

- `TunnelId` (FK→TblObjectTunnel, required)

**Junction tables:**

- `TblRoadBridge` (FK→TblObjectRoad, FK→TblObjectBridge)
- `TblRoadStation` (FK→TblObjectRoad, FK→TblObjectRoadStation)
- `TblRoadRoadMod` (FK→TblObjectRoad, FK→TblObjectRoadExtra)
- `TblRoadCompatibleTrackRoad` (FK→TblObjectRoad, FK→TblObjectTrack or TblObjectRoad)

### 8. TblObjectLand

**FK columns:**

- `CliffEdgeId` (FK→TblObjectCliffEdge, required)
- `ReplacementLandId` (nullable FK→TblObjectLand)

### 9. TblObjectTrackStation

**Scalar columns:**

- Add: `var_0B` (uint8_t), `var_0D` (uint8_t)

**JSON columns:**

- `CargoOffsets` (CargoOffset[][][])
- `var_6E` (uint8_t[][])

**Junction tables:**

- `TblTrackStationCompatibleTrack` (FK→TblObjectTrackStation, FK→TblObjectTrack)

### 10. TblObjectTrackSignal

**Scalar columns:**

- Add: `var_0B` (uint8_t)

**Junction tables:**

- `TblTrackSignalCompatibleTrack` (FK→TblObjectTrackSignal, FK→TblObjectTrack)

### 11. TblObjectRoadStation

**Scalar columns:**

- Remove: `CompatibleRoadObjectCount` (derivable)
- Add: `pad_2D` (uint8_t)

**JSON columns:**

- `CargoOffsets` (CargoOffset[][][])

**FK columns:**

- `CargoTypeId` (nullable FK→TblObjectCargo)

**Junction tables:**

- `TblRoadStationCompatibleRoad` (FK→TblObjectRoadStation, FK→TblObjectRoad)

### 12. TblObjectDock

**Scalar columns:**

- Remove: `NumBuildingPartAnimations`, `NumBuildingVariationParts` (derivable)
- Keep: `BoatPositionX`, `BoatPositionY`

**JSON columns:**

- `BuildingComponents` (BuildingComponents)

### 13. TblObjectRegion

**Scalar columns:**

- Add: `VehicleDrivingSide` (string)

**JSON columns:**

- `CargoInfluenceTownFilter` (List<CargoInfluenceTownFilterType>)

**Junction tables:**

- `TblRegionCargoInfluence` (FK→TblObjectRegion, FK→TblObjectCargo)
- `TblRegionDependentObject` (FK→TblObjectRegion, FK→TblObject)

### 14. TblObjectSteam

**Scalar columns:**

- Add: `var_0A` (uint32_t)

**JSON columns:**

- `FrameInfoType0` (List<SteamImageAndHeight>)
- `FrameInfoType1` (List<SteamImageAndHeight>)

**Junction tables:**

- `TblSteamSoundEffect` (FK→TblObjectSteam, FK→TblObjectSound)

### 15. TblObjectSound

**Binary columns:**

- `PcmData` (byte[])
- `UnkData` (byte[])

**JSON columns:**

- `SoundObjectData` (SoundObjectData, which contains SoundEffectWaveFormat)

### 16. TblObjectTownNames

**JSON columns:**

- `MorphemeCategories` (List<MorphemeCategory>)

### 17. TblObjectWater

**Scalar columns:**

- Add: `var_03` (uint8_t)

### 18. TblObjectScaffolding

**JSON columns:**

- `SegmentHeights` (List<uint16_t>)
- `RoofHeights` (List<uint16_t>)

### 19. TblObjectStreetLight

**JSON columns:**

- `DesignedYears` (List<uint16_t>)

## Implementation Order

### Phase 1: Infrastructure

- [ ] Create junction table classes directory and files
- [ ] Create normalized sub-table classes
- [ ] Update LocoDbContext with new DbSet properties
- [ ] Update OnModelCreating for composite keys

### Phase 2: Simple Scalar/JSON Changes

- [ ] TblObjectCargo (add UnitWeight)
- [ ] TblObjectWater (add var_03)
- [ ] TblObjectScaffolding (add JSON columns)
- [ ] TblObjectStreetLight (add JSON columns)
- [ ] TblObjectTownNames (add JSON columns)
- [ ] TblObjectSound (add binary + JSON columns)
- [ ] TblObjectWall (minor)
- [ ] TblObjectTree (minor)

### Phase 3: Medium Complexity

- [ ] TblObjectBridge (junction tables)
- [ ] TblObjectLand (FK columns)
- [ ] TblObjectTrackExtra (already complete)
- [ ] TblObjectRoadExtra (already complete)
- [ ] TblObjectTrackSignal (junction tables)
- [ ] TblObjectTrackStation (JSON + junction tables)
- [ ] TblObjectRoadStation (JSON + FK + junction tables)
- [ ] TblObjectSteam (JSON + junction tables)
- [ ] TblObjectRegion (JSON + junction tables)
- [ ] TblObjectDock (JSON columns)

### Phase 4: High Complexity

- [ ] TblObjectBuilding (JSON + junction tables)
- [ ] TblObjectAirport (JSON columns)
- [ ] TblObjectIndustry (JSON + FK + junction tables)
- [ ] TblObjectTrack (FK + junction tables)
- [ ] TblObjectRoad (FK + junction tables)

### Phase 5: VehicleObject (largest)

- [ ] TblObjectVehicle (scalar + JSON + FK + junction tables + normalized sub-table)

### Phase 6: Finalization

- [ ] Update DbSubObjectHelper for FK resolution if needed
- [ ] Build and fix compilation errors
- [ ] Generate EF Core migration
- [ ] Test
