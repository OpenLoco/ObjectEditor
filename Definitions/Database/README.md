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
TblObject ||--|| TblObjectT : one-to-one
TblObject ||--o{ TblDatObject : many-to-one

TblObjectPack ||--|| TblObject : many-to-many
TblScenarioPack ||--|| TblSC5File : many-to-many

TblObjectT {
  ulong Id
}

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

TblSC5File {
  ulong Id
  string Name
  string Description
  DateOnly CreateDate
  DateOnly ModifiedDate
  DateOnly UploadedDate
  ObjectSource ObjectSource
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
}

TblSoundEffect {
  ulong Id
  string Name
  string Description
  ObjectSource ObjectSource
  DateOnly CreatedDate
  DateOnly ModifiedDate
  DateOnly UploadedDate
}

TblTutorial {
  ulong Id
  string Name
  string Description
  ObjectSource ObjectSource
  DateOnly CreatedDate
  DateOnly ModifiedDate
  DateOnly UploadedDate
}

TblGraphics {
  ulong Id
  string Name
  string Description
  ObjectSource ObjectSource
  DateOnly CreatedDate
  DateOnly ModifiedDate
  DateOnly UploadedDate
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
