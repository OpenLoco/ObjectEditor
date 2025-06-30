# Database Relationships

```mermaid
erDiagram

TblObject }o--o{ TblAuthor : many-to-many
TblObject }o--o{ TblTag : many-to-many
TblObject }o--|| TblLicence : one-to-many

TblObjectPack }o--o{ TblAuthor : many-to-many
TblObjectPack }o--o{ TblTag : many-to-many
TblObjectPack }o--|| TblLicence : one-to-many

TblSC5File }o--o{ TblAuthor : many-to-many
TblSC5File }o--o{ TblTag : many-to-many
TblSC5File }o--|| TblLicence : one-to-many

TblSC5FilePack }o--o{ TblAuthor : many-to-many
TblSC5FilePack }o--o{ TblTag : many-to-many
TblSC5FilePack }o--|| TblLicence : one-to-many

TblObject ||--o{ TblStringTableRow : many-to-one
TblObject ||--|| TblObjectT : one-to-one
TblObject ||--o{ TblDatObject : many-to-one

TblObjectPack ||--|| TblObject : many-to-many
TblSC5FilePack ||--|| TblSC5File : many-to-many

TblObjectT {
  ulong Id
}

TblObject {
  ulong Id
  string Name
  string Description
  DateTimeOffset CreateDate
  DateTimeOffset ModifiedDate
  DateTimeOffset UploadedDate
  ObjectSource ObjectSource
  ObjectType ObjectType
  VehicleType VehicleType
  ObjectAvailability Availability
}

TblObjectPack {
  ulong Id
  string Name
  string Description
  DateTimeOffset CreateDate
  DateTimeOffset ModifiedDate
  DateTimeOffset UploadedDate
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

TblSC5File {
  ulong Id
  string Name
  string Description
  DateTimeOffset CreateDate
  DateTimeOffset ModifiedDate
  DateTimeOffset UploadedDate
  ObjectSource ObjectSource
}

TblSC5FilePack {
  ulong Id
  string Name
  string Description
  DateTimeOffset CreateDate
  DateTimeOffset ModifiedDate
  DateTimeOffset UploadedDate
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