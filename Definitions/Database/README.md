# Database Relationships

```mermaid
erDiagram

TblObject }o--o{ TblAuthor : many-to-many
TblObject }o--o{ TblTag : many-to-many
TblObject }o--|| TblLicence : one-to-many

TblObjectPack }o--o{ TblAuthor : many-to-many
TblObjectPack }o--o{ TblTag : many-to-many
TblObjectPack }o--|| TblLicence : one-to-many

TblScenario }o--o{ TblAuthor : many-to-many
TblScenario }o--o{ TblTag : many-to-many
TblScenario }o--|| TblLicence : one-to-many

TblScenarioPack }o--o{ TblAuthor : many-to-many
TblScenarioPack }o--o{ TblTag : many-to-many
TblScenarioPack }o--|| TblLicence : one-to-many

TblMusic }o--o{ TblAuthor : many-to-many
TblMusic }o--o{ TblTag : many-to-many
TblMusic }o--|| TblLicence : one-to-many

TblSoundEffect }o--o{ TblAuthor : many-to-many
TblSoundEffect }o--o{ TblTag : many-to-many
TblSoundEffect }o--|| TblLicence : one-to-many

TblTutorial }o--o{ TblAuthor : many-to-many
TblTutorial }o--o{ TblTag : many-to-many
TblTutorial }o--|| TblLicence : one-to-many

TblGraphics }o--o{ TblAuthor : many-to-many
TblGraphics }o--o{ TblTag : many-to-many
TblGraphics }o--|| TblLicence : one-to-many

TblObject ||--o{ TblStringTableRow : many-to-one
TblObject ||--o| TblObjectAirport : one-per-object-type
TblObject ||--o{ TblDatObject : many-to-one

TblUser ||--o{ TblObject : owns

TblObjectPack ||--|| TblObject : many-to-many
TblScenarioPack ||--|| TblScenario : many-to-many

TblObject {
  ulong Id
  string Name
  string Description
  DateOnly CreateDate
  DateOnly ModifiedDate
  DateOnly UploadedDate
  ObjectSource ObjectSource
  ObjectType ObjectType
  VehicleType VehicleType
  ObjectAvailability Availability
}

TblObjectPack {
  ulong Id
  string Name
  string Description
  DateOnly CreateDate
  DateOnly ModifiedDate
  DateOnly UploadedDate
}

TblDatObject {
  ulong Id
  string DatName
  string DatChecksum
  ulong xxHash3
}

TblStringTableRow {
  ulong Id
  string Name
  LanguageId Language
  string Text
}

TblScenario {
  ulong Id
  string Name
  string Description
  DateOnly CreateDate
  DateOnly ModifiedDate
  DateOnly UploadedDate
  ObjectSource ObjectSource
  ObjectAvailability Availability
}

TblScenarioPack {
  ulong Id
  string Name
  string Description
  DateOnly CreateDate
  DateOnly ModifiedDate
  DateOnly UploadedDate
}

TblMusic {
  ulong Id
  string Name
  string Description
  ObjectSource ObjectSource
  DateOnly CreatedDate
  DateOnly ModifiedDate
  DateOnly UploadedDate
  ObjectAvailability Availability
}

TblSoundEffect {
  ulong Id
  string Name
  string Description
  ObjectSource ObjectSource
  DateOnly CreatedDate
  DateOnly ModifiedDate
  DateOnly UploadedDate
  ObjectAvailability Availability
}

TblTutorial {
  ulong Id
  string Name
  string Description
  ObjectSource ObjectSource
  DateOnly CreatedDate
  DateOnly ModifiedDate
  DateOnly UploadedDate
  ObjectAvailability Availability
}

TblGraphics {
  ulong Id
  string Name
  string Description
  ObjectSource ObjectSource
  DateOnly CreatedDate
  DateOnly ModifiedDate
  DateOnly UploadedDate
  ObjectAvailability Availability
}


TblAuthor {
  ulong Id
  string Name
}

TblTag {
  ulong Id
  string Name
}

TblLicence {
  ulong Id
  string Name
  string Text
}
```

## File-backed entities and availability

The game object and the files dropped into the `GameData` folders (scenarios, landscapes, music, sound
effects, tutorials and graphics) are all backed by a file on disk, so they share the `DbFileObject`
base and its `Availability` (`ObjectAvailability`) column. Object/scenario **packs** are *not* file
entities and stay on `DbCoreObject`.

The database is the source of truth for these rows: when a file disappears the row is **kept** and
marked `Unavailable` (rather than deleted), so curated metadata and any pack/scenario references
survive; when the file reappears the row is marked `Available` again. Only `Custom` content can ever
be `Available`: vanilla Locomotion (Steam/GoG) and OpenLoco content is placed on the server by hand and
is never downloadable, so its availability is always `Unavailable` (see `ObjectAvailabilityRules`).
Only game objects previously had this column - `20261005013618_GameDataFileAvailability` adds it to
`Scenarios`, `Music`, `SoundEffects`, `Tutorials` and `Graphics`.

## Object data structure

Every game object - all 34 `ObjectType` values - is stored in three parts:

1. **Header row**: `Objects` (`TblObject`). One row per object whatever its type, holding the generic
   information: `Name` (unique), `Description`, the three dates, `ObjectSource` / `ObjectType` /
   `VehicleType` / `Availability`, `Licence`, `Authors`, `Tags`, `ObjectPacks` and the owning `OwnerUser`.
2. **Sub-object row**: `Obj<ObjectType>` (`TblObject<ObjectType>`), at most one row per object, holding the
   type-specific fields - e.g. `ObjAirport` (`TblObjectAirport`). It links back to its header row through
   the required `Parent` FK, which is the single source of truth for the relationship: a parent id can only
   ever have one row per sub-object table, and deleting the header row cascade-deletes it. The one type that
   does not follow the `Obj<ObjectType>` naming pattern is `ObjectType.InterfaceSkin`, whose table is
   `ObjInterface` (`TblObjectInterface`).
3. **File rows**: `DatObjects` (`TblDatObject`), the metadata of the actual `.dat` file(s) on disk:
   `DatName` + `DatChecksum` (an index, *not* unique: a re-packed or hacked object can share the S5 name and
   checksum while being a binary-different file), the unique `xxHash3` whole-file hash that is the
   authoritative file identity, and the required `ObjectId` / `Object` FK back to the header row. This is
   one-to-many: a single object can be made of several files, but every file must reference exactly one object.

A header row can therefore legitimately exist without a sub-object row (a DAT that defines no type-specific
data), but never the other way round. `ObjectsMissing` (`TblObjectMissing`) is *not* an ignore list: it
records objects we know about (e.g. referenced by a scenario) whose DAT file we do not have, and the row is
removed once someone supplies the file and it is indexed.

`DbSubObjectStructureTests` locks this structure: every `ObjectType` maps to exactly one `Obj<ObjectType>`
DbSet (and back), each is backed by a `TblObject<ObjectType>` row deriving from `DbSubObject` with a `Parent`
FK, and each resolves to a real table.
