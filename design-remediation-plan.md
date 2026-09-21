# Design Remediation Plan

Tracking document for the database and web-API design review. Keep the checkboxes and the
[Progress Log](#progress-log) up to date as work lands.

|                  |                                                                                     |
| ---------------- | ----------------------------------------------------------------------------------- |
| **Status**       | Active                                                                              |
| **Created**      | 2026-09-20                                                                          |
| **Owner**        | _unassigned_                                                                        |
| **Related docs** | `ObjectService/README.md`, `Definitions/Database/README.md`, `dat-object-layout.md` |

## How to use this document

- Every workstream has an ID (`WSn`) matching the original review findings, so the two can be cross-referenced.
- Tick a task when **code + tests** are merged, and add a line to the [Progress Log](#progress-log) with the PR/commit.
- Phase 0 decisions must be settled before the phases that depend on them start.
- Status legend: ☐ not started · ◐ in progress · ☑ done · ⊘ deferred / won't do

## Status at a glance

| Phase | Workstream                              | Status |
| ----- | --------------------------------------- | ------ |
| 0     | D1–D7 decisions                         | ☑      |
| 1     | WS3 — Permissions                       | ☑      |
| 1     | WS4 — Validation symmetry               | ☑      |
| 1     | WS12 — Minor consistency                | ☑      |
| 1     | WS11 — Dead code / `ObjectType` mapping | ☑      |
| 1     | WS13 — Index path convention            | ☑      |
| 2     | WS7 — Pack GET-by-id                    | ☑      |
| 2     | WS2 — Object delete                     | ☑      |
| 2     | WS14 — Route registration               | ☑      |
| 2     | WS8 — Identity prefix                   | ☑      |
| 3     | WS5 — EF migrations (+ WS5b)            | ☑      |
| 4     | WS1 — Sub-object identity               | ☑      |
| 4     | WS6 — `SC5Files` → `Scenarios`          | ☑      |
| 5     | WS10 — Credentials                      | ☑      |
| 5     | WS-DeadCode — Experiment cleanup        | ☑      |

---

## Root causes

Two root causes sit behind most of the findings:

1. **Schema management is split-brain.** *(resolved in Phase 3 — see WS5)*
   `Definitions/Migrations` existed (25 migrations, latest `20260918063013_DropBridgeVar03`, and
   `Definitions.csproj` references `Microsoft.EntityFrameworkCore.Design`), but nothing ever called
   `Migrate()` / `MigrateAsync()`. Runtime used `EnsureCreatedAsync()` plus three hand-written
   upgrader classes (`DatabaseInitializer`, `GameDataFileTableInitializer`, `ScenarioPackTableInitializer`).
   The model snapshot was **stale**: it still modelled `TblSC5FilePack` and had no
   `TblMusic` / `TblSoundEffect` / `TblTutorial` / `TblGraphics`, which is why the runtime table
   initialiser was written, and any new column needed another bespoke patch.
   **Resolved in Phase 3 (WS5):** migrations are now the source of truth, the baseline replaced the
   bespoke upgraders, and a CI gate prevents the snapshot from drifting again.

2. **The object / sub-object model is a workaround.** *(resolved in Phase 4 — see WS1)*
   TPT was attempted and reverted (`20250630043418_SubObjectTPT` migration contains commented-out FK
   operations), leaving `TblObject.SubObjectId` as an unconstrained scalar that duplicated the
   `Parent` FK each sub-table already has, with no referential integrity and rows that could orphan.
   **Resolved in Phase 4 (WS1):** `SubObjectId` is dropped and the sub-table's `Parent` FK (required,
   `ON DELETE CASCADE`) is now the single source of truth.

---

## Completed — initial review pass (2026-09-20)

Bug fixes already landed before this plan was written. Full suite: **2487 passed / 0 failed** (5 pre-existing skips).

- [x] **Object-pack downloads dropped uploaded objects.** Uploaded DATs are indexed with an absolute path but `ObjectPackService.GetPackFileAsync` only accepted relative paths.
      → Added `RouteHelpers.TryGetSafePathUnderRoot` and used it in `ObjectPackService`.
- [x] **`GET /v2/objectpacks/{id}` and `/v2/scenariopacks/{id}` returned `200 []` for unknown ids** while every other resource GET 404s.
      → `ObjectPackRouteHandler` / `ScenarioPackRouteHandler` now 404 when the pack is missing.
- [x] **`Location` header on create omitted the `/v2` prefix** (`CrudRouteHandler`, `ObjectRouteHandler`).
- [x] **Legacy DBs missing `OwnerUserId` on game-data tables** (`DatabaseInitializer` only patched 4 of 8 `DbCoreObject` tables).
- [x] **`DbSubObjectHelper.AddOrUpdate` update branch did nothing** — `existingSubObj = subObj;` reassigned a local and set `parentObj.SubObjectId = 0`. Now copies non-key values onto the tracked entity.
- [x] **Redundant duplicate unique indexes** removed from `TblScenario`, `TblMusic`, `TblSoundEffect`, `TblTutorial`, `TblGraphics`, `DbSubObject`.
- [x] Tests added: `TryGetSafePathUnderRoot` cases, absolute-path pack zip, pack 404s, `DbSubObjectHelperTests` (insert + update).

---

## Phase 0 — Decisions to settle first

These block the phases that follow; each is a policy choice, not code.

| #      | Decision                        | Options                                                                                                                                                  | Choice                                                                                                                                                                                                                                                                                |
| ------ | ------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **D1** | Migrations vs. current approach | (a) Adopt EF migrations + baseline existing DBs · (b) keep `EnsureCreated`, consolidate all upgraders into one versioned runner                          | **a**                                                                                                                                                                                                                                                                                 |
| **D2** | Sub-object identity             | (a) Drop `TblObject.SubObjectId`, resolve via sub-table `Parent` FK · (b) configure a real 1:1 relationship · (c) leave as-is + integrity guards         | **a**                                                                                                                                                                                                                                                                                 |
| **D3** | Object delete                   | (a) Implement `DELETE /v2/objects/{id}` incl. disk + index cleanup · (b) unmap the route (405)                                                           | **a**                                                                                                                                                                                                                                                                                 |
| **D4** | Pack list vs. single GET        | (a) `GET /{id}` returns one rich descriptor, keep `/descriptor` as light summary · (b) one shape, delete `/descriptor` · (c) leave as-is                 | **a**                                                                                                                                                                                                                                                                                 |
| **D5** | Permission taxonomy             | Canonical set where _grantable ⇔ enforced_. Who may create/modify packs via the API — any authenticated user (today) or `ObjectPacksCreate` / `*Modify`? | asp.net claims are the only way to determine permissions. the 'author' table is just to store the string name of the author and isn't used for permissions (though an asp.net user may be assigned an 'author' which immediately means they get claim to all objects with that author |
| **D6** | Identity route prefix           | Move `/register`, `/login`, `/manage/*` under `/v2/identity` (breaking), or leave at root                                                                | move them to identity                                                                                                                                                                                                                                                                 |
| **D7** | `SC5Files` rename               | Rename navigation to `Scenarios` (+ join-table rename migration) and update DTOs/pages/DatabaseTools, or keep                                            | rename to Scenarios                                                                                                                                                                                                                                                                   |

---

## Phase 1 — Low-risk consistency (no schema change) — ✅ complete 2026-09-20

### WS3 — Permissions are incoherent

**Status:** ☑ &nbsp; **Size:** M &nbsp; **Depends on:** D5 &nbsp; **Completed:** 2026-09-20

Per D5, ASP.NET claims are the single source of truth for permissions (the `Authors` table only stores
display names; being linked to one grants edit rights over objects crediting that author).

- [x] Made `LocoPermissions.All` the single source of truth (added `ObjectPacksModify` / `ScenarioPacksModify`) and added a matching `LocoPermissions.Curator` set used by the initializer
- [x] Added `CanCreateObjectPacks` / `CanModifyObjectPacks` / `CanModifyScenarioPacks` policies and gated the pack POST/PUT/DELETE endpoints (`RouteBuilderExtensions` + `BaseTableRouteHandler.MapWriteRoutes`)
- [x] Removed the dead `DisplayNameChange` permission and the startup loop that re-granted it to every user
- [x] Extended `ObjectOwnershipHandler` so a user linked to an object's author (`TblUser.AssociatedAuthorId`) may edit it (D5)
- [x] Tests: `LocoPermissionsTests` (All covers every constant, unique, `Curator ⊆ All`), `PackAuthorizationTests` (policies registered), `ObjectOwnershipHandlerTests` (owner / author-linked / unrelated / admin / vanilla)

### WS4 — Validation symmetry

**Status:** ☑ &nbsp; **Size:** M &nbsp; **Completed:** 2026-09-20

- [x] Added `TryValidateUpdate` to `ICrudService` / `CrudService` and called it from `CrudRouteHandler.UpdateAsync`
- [x] Unique-`Name` violations now return **409 Conflict** (`DbExceptionHelpers.IsUniqueConstraintViolation`) instead of 500; the `Accepted` location header also gained the missing `/v2` prefix
- [x] `Name` validated on object-pack and scenario-pack create/update (blank → 400)
- [x] Decision: mapped-but-501 endpoints (`POST /v2/roles`, `/v2/users`, `/v2/scenarios`, game-data `POST`) **stay 501** — they remain self-documenting "not implemented" rather than 405
- [x] Tests: `ObjectPackRoutesTest` (blank name → 400, duplicate name → 409), `DbExceptionHelpersTests`

### WS12 — Minor consistency cleanup

**Status:** ☑ &nbsp; **Size:** S &nbsp; **Completed:** 2026-09-20

- [x] Collapsed the duplicate `Routes.Roles` / `Routes.RolesSubRoute` constants (only `Routes.Roles` remains; `Client.cs` and `UserRouteHandler` updated)
- [x] Dropped the unused `HttpContext` from `ICrudService.ListAsync` (and from `IObjectQueryService.ListAsync`)
- [x] `TblObjectMissing` now uses the same `IsDescending` unique-index shape as `TblDatObject` (the resulting model change is captured by the WS5 baseline migration)

### WS11 — Duplicate `ObjectType` switches / dead code

**Status:** ☑ &nbsp; **Size:** S &nbsp; **Completed:** 2026-09-20

- [x] Deleted `ObjectService/RouteHandlers/ObjectTypeMapping.cs` (no callers) and the dead `DbSubObjectHelper.GetDbSetForType`
- [x] `DatabaseTools/Services/DatabaseHelperScripts.cs` now uses the canonical `ObjectTypeMapping.StructTypeToObjectType` instead of its own 34-case copy
- [x] `Definitions/ObjectModels/ObjectTypeMapping.cs` is the single source of truth for the ObjectType ⇄ CLR struct mapping. `DbSubObjectHelper`'s `AddOrUpdate`/`GetDbSubForType` switches are inherently per-`DbSet` and stay as-is
- [x] Tests: `ObjectTypeMappingTests` (round-trip for every `ObjectType`, unknown-type throws)

### WS13 — Mixed object-index path convention

**Status:** ☑ &nbsp; **Size:** S–M &nbsp; **Completed:** 2026-09-20

- [x] `ObjectQueryService.UploadDatAsync` stores a **relative** `FileName` via `ServerFolderManager.GetCustomObjectRelativeFileName`, matching scanned entries
- [x] Kept `RouteHelpers.TryGetSafePathUnderRoot` as a defensive fallback for already-persisted absolute entries
- [x] Tests: `ServerFolderManagerTests` (relative name), `ObjectRoutesTest` (uploaded entry indexed relatively, and resolvable from the Objects folder), `ObjectPackRoutesTest` (absolute-entry pack zip, added in the initial pass)

---

## Phase 2 — API surface & auth alignment — ✅ complete 2026-09-21 (WS2 finished after Phase 4/WS1)

### WS7 — Pack GET-by-id

**Status:** ☑ &nbsp; **Size:** M &nbsp; **Depends on:** D4 &nbsp; **Completed:** 2026-09-20

`GET /v2/objectpacks/{id}` used to return a collection with 0-or-1 rich descriptors, and `/descriptor`
duplicated it with a lighter shape; `Client.GetObjectPackAsync` hid this behind `FirstOrDefault()`.

- [x] `GET /v2/objectpacks/{id}` and `/v2/scenariopacks/{id}` now return a **single** rich descriptor (or 404)
- [x] Updated `Definitions/Web/Client.cs` (`GetObjectPackAsync`, `GetScenarioPackAsync` now read a single descriptor)
- [x] `Gui/ObjectServiceClient.cs` / `FolderTreeViewModel` need no change (identical client signatures)
- [x] `/descriptor` kept as the light summary shape (per D4a)
- [x] Tests: `ObjectPackRoutesTest.GetAsync` / `ScenarioPackRoutesTests.GetAsync` now exercise the single-object shape, plus the 404 cases

### WS2 — Object delete

**Status:** ☑ &nbsp; **Size:** L &nbsp; **Depends on:** D3, WS5, WS1 &nbsp; **Completed:** 2026-09-21

Removal is **recoverable and non-destructive**: file(s) are moved into a per-category `Removed`
subfolder and the database row is kept (objects are marked `ObjectAvailability.Unavailable`).

- [x] Added the `Removed` subfolder to every category folder (`ServerFolderManager.RemovedFolderName` + `...RemovedFolder` properties), created alongside `Original`/`Custom`/`OpenLoco`
- [x] `ServerFolderManager.MoveToRemovedFolder` moves a file into `Removed` preserving its relative path (`Custom/x.dat` → `Removed/Custom/x.dat`), never overwriting an earlier removal, and refuses paths outside the category / already-removed / missing files
- [x] `Removed` is ignored by the file watchers (`GameDataFolderWatcher.IsIgnored`) **and** by reconciliation (`GameDataFolderServiceBase.EnumerateFiles`), so a parked file is never re-imported
- [x] `IObjectQueryService.DeleteObjectAsync`: parks the DAT file(s), drops the object-index entries, sets `Availability = Unavailable` and keeps the row (metadata, packs and scenario references survive). Refuses vanilla (`LocomotionSteam`/`LocomotionGoG`). Sub-object/string-table/DAT rows are cleaned up by the FK cascades proven in WS1
- [x] `ObjectRouteHandler.DeleteAsync` replaced its 501 stub with the real handler (200 / 404 / 403)
- [x] Applied the same file policy to the other game-data deletes (`GameDataFileQueryService.DeleteAsync`, `ScenarioService.DeleteAsync`) which previously deleted the row but left the file on disk, so the watcher re-imported it
- [x] Tests: `ServerFolderManagerTests` (Removed folder exists for all categories, move preserves the relative path, no overwrite, rejects outside/removed/missing, `IsUnderRemovedFolder`), `ObjectsFolderServiceTests` (reconciliation ignores parked files; remove→restore flips availability), and `ObjectRoutesTest.DeleteAsync` rewritten for the soft-delete semantics
- [x] **Round trip:** re-importing a file that reappears (i.e. restored from `Removed`) now sets `Availability = Available` again, so a removal is fully recoverable
- [x] `ObjectService/README.md` + the `ServerFolderManager` structure comment document the `Removed` convention

> **Domain note (corrected):** `ObjectsMissing` is **not** an ignore list. It lists objects we know about
> (e.g. referenced by a scenario) whose DAT file we do not have; when someone later supplies the file it is
> indexed and the row is removed from `ObjectsMissing`. It is therefore not used for removals.

### WS14 — Route registration cleanup

**Status:** ☑ &nbsp; **Size:** S &nbsp; **Completed:** 2026-09-20

- [x] `UserRouteHandler.MapAdditionalWriteRoutes` now registers the `/v2/users/me` PUT/DELETE from the write path (called from `RouteBuilderExtensions`), not the read path
- [x] `RolesSubRoute` collapsed into `Routes.Roles` (done in WS12/Phase 1)
- [x] The admin maintenance POSTs stay on the read-path registration on purpose, so identity management keeps working when `ObjectService:BackendReadOnly` disables game-data writes (documented in code)
- [x] Test: `IdentityRoutesTest.UpdateCurrentUserDisplayName_WithAuthentication_ShouldSucceed` covers `PUT /v2/users/me`

### WS8 — Identity endpoint prefix

**Status:** ☑ &nbsp; **Size:** S–M &nbsp; **Depends on:** D6 &nbsp; **Completed:** 2026-09-20

- [x] Identity endpoints are mounted under `/v2/identity` (`Routes.Identity*` constants)
- [x] Updated `Pages/Account/{Login,Register,Manage}` and `Pages/Dev/QuickLogin` to the versioned paths
- [x] `DevAuthenticationHandler` now also excludes `/v2/identity`, so identity flows are never impersonated in dev
- [x] Documented as a breaking API change (see Progress Log)

## Phase 3 — Schema management (prerequisite for Phase 4) — ✅ complete 2026-09-21

### WS5 — Adopt EF migrations

**Status:** ☑ &nbsp; **Size:** L &nbsp; **Depends on:** D1 &nbsp; **Completed:** 2026-09-21

The 25 stale migrations were squashed into a single **`20260921012153_InitialBaseline`** generated from
the current model (65 tables; `TblSC5FilePack` is gone and the game-data tables are present, so the
snapshot is finally in sync).

- [x] Regenerated the baseline from the current model (captures the removed duplicate indexes, the game-data file tables and the `TblScenarioPack` rename)
- [x] Added `MigrationInitializer.EnsureBaselineHistoryAsync`: creates `__EFMigrationsHistory` and records the baseline for a non-empty database that has no history; no-op for brand-new/already-migrated databases
- [x] `DatabaseInitializer` now calls `EnsureBaselineHistoryAsync` + `MigrateAsync()` instead of `EnsureCreatedAsync()`
- [x] Deleted `GameDataFileTableInitializer.cs` and `ScenarioPackTableInitializer.cs` (+ their tests). The scenario-pack rename is inherent in the baseline (`ScenarioPacks` is created directly); the game-data tables likewise
- [x] `ObjectService:DeleteDatabaseOnStartup` retained for dev; `TestWebApplicationFactory` now uses `Migrate()` so the whole suite runs on the migrated schema
- [x] CI gate added: `.github/workflows/db-migrations.yml` runs `dotnet ef migrations has-pending-model-changes` and `migrations script --idempotent`
- [x] Tests: `DatabaseMigrationTests` covers fresh DB, brand-new (unseeded) DB, legacy DB with no history, idempotency and already-migrated

> **Known limitation (accepted):** the baseline assumes an existing database already has the *current*
> schema, because `EnsureCreated` builds from the model. Databases older than the last `EnsureCreated`
> release must be rebuilt (delete `loco.db` or set `ObjectService:DeleteDatabaseOnStartup`); the file
> watcher/reconcile jobs repopulate the data from the `GameData` folders.

### WS5b — `OwnerUserId` backfill

**Status:** ☑ &nbsp; **Size:** S &nbsp; **Depends on:** WS5 &nbsp; **Completed:** 2026-09-21

- [x] The interim `ALTER TABLE ... ADD COLUMN OwnerUserId` loop was deleted: the column is part of the migration baseline, and pre-migration databases already have it from `EnsureCreated`

---

## Phase 4 — Data-model refactors ✅ complete 2026-09-21

### WS1 — Sub-object identity

**Status:** ☑ &nbsp; **Size:** L &nbsp; **Depends on:** WS5, D2 &nbsp; **Completed:** 2026-09-21

`TblObject.SubObjectId` was a second, unconstrained pointer to the same row that the sub-table's
`Parent` FK already identifies. It is gone: the sub-table's `Parent` FK is the single source of truth,
and it is already a **required FK with `ON DELETE CASCADE`** (verified in the baseline migration and by
a new cascade test).

- [x] Dropped `TblObject.SubObjectId` (plus the dead TPT experiment comments around it and the commented `HasAlternateKey` in `LocoDbContext`)
- [x] `AddOrUpdateCore` now finds the existing row via `x.Parent.Id == parentObj.Id` and no longer writes anything back to the object
- [x] `DbSubObject.Parent` cascade delete confirmed (required FK + `ReferentialAction.Cascade` in the baseline; no model change needed). `DbSubObjectHelperTests.DeletingParentObject_CascadesToSubObjectRow` proves a delete of the object removes the sub-object row at the database level
- [x] Migration `20260921015814_DropObjectSubObjectId` drops the column (no index existed on it)
- [x] Updated `ObjectQueryService.UploadDatAsync` / `ObjectsFolderService` (removed `SubObjectId = 0`) and the seeds in `ObjectPackRoutesTest` / `ObjectRoutesTest` / `DbSubObjectHelperTests`
- [x] Tests: `DbSubObjectHelperTests` reworked for the `Parent`-based lookup (update keeps the id, insert works) + the cascade-delete test
- [ ] **Decision — object update still ignores the sub-object.** Implementing it would need DTO→DAT reverse mappers for all 34 sub-object types (`ToObject`/`FromDto` do not exist). Out of scope for WS1; recorded under "Discovered during remediation"

> Note: `DatabaseTools` import/export does not read `SubObjectId`, so it is unaffected.

### WS6 — `SC5Files` → `Scenarios` naming

**Status:** ☑ &nbsp; **Size:** M &nbsp; **Depends on:** WS5, D7 &nbsp; **Completed:** 2026-09-21

- [x] Renamed the navigation on `TblAuthor` / `TblTag` to `Scenarios`, and the corresponding properties on `DtoAuthorDescriptor`, `DtoTagDescriptor`, `DtoLicenceDescriptor` and `DtoScenarioPackDescriptor`
- [x] Updated `ReferenceDataService`, `Pages/{Authors,Tags,Licences}/Details.*`, `ScenarioPacks/Details.*` (`SelectedSC5FileIds` → `SelectedScenarioIds`), `Scenarios/Details.*`, and the route tests
- [x] Migration `20260921024756_RenameSc5FilesNavigationToScenarios`: the join *tables* were already `TblAuthorTblScenario` / `TblScenarioTblTag` (EF names them after the entity types, and the entity was renamed to `TblScenario` earlier), so this renames their **columns and indexes** (`SC5FilesId` → `ScenariosId`) and recreates the FKs. Verified end-to-end against a baseline-era database by the legacy-upgrade test
- [x] Renamed `SC5FilePackService.cs` → `ScenarioPackService.cs` (the class inside was already `ScenarioPackService`)
- [x] Updated the user-facing strings: "SC5 Files" → "Scenarios", "SC5 File Packs" → "Scenario Packs", plus the Gui folder-tree labels and the `Dto`/README/mermaid naming

**Two latent bugs fixed while here:**

1. **`DatabaseTools` export/import filename mismatch** — export wrote `sc5FilePacks.json` but import reads `scenarioPacks.json`, so an export could never be imported back. Export now writes `scenarioPacks.json`.
2. **Scenarios pages linked to a dead category** — `Pages/Scenarios/Details.*` used `asp-route-category="sc5files"` while `Pages/Index` switches on `scenarios`, so the "Back to scenarios" links and the post-delete redirect landed on the default view.

> **Breaking change (API/UI):** the JSON field `SC5Files` on the author/tag/licence/scenario-pack descriptors is now `Scenarios`. `Definitions/Web/Client.cs`, the Gui and the Razor pages were all updated together.

> Also normalised stray U+202F (narrow no-break space) characters in `Pages/Index.cshtml` headings that had masked some of these strings.

---

## Phase 5 — Security & polish ✅ complete 2026-09-21

### WS10 — Hardcoded credentials

**Status:** ☑ &nbsp; **Size:** S &nbsp; **Completed:** 2026-09-21

The admin password is **never defaulted in code**: it must come from configuration, and without it the
admin account is simply not bootstrapped — so a deployment can never ship a known-password admin.

- [x] New `AdminUserSettings.FromConfiguration` returns `null` unless `AdminUser:Password` is configured; `DatabaseInitializer` logs a **warning** in Development and an **error** elsewhere, then skips admin creation, role assignment and the unowned-object backfill
- [x] Removed `DatabaseInitializer.DefaultAdminPassword`; the email/username defaults moved into `AdminUserSettings` as non-secret display defaults
- [x] `/dev/quick-login` and `Pages/Dev/QuickLogin` now read `DevAuth:Email` / `DevAuth:UserName` / `DevAuth:Password` from configuration and return 503/400 when unset — no code default. The endpoint was already Development-only + `AllowAnonymous`
- [x] `appsettings.Development.json` carries the development-only `AdminUser` and `DevAuth` values (with an explanatory comment); `appsettings.json` has neither, so production must supply them via user-secrets or environment variables
- [x] Dev-auth exclusions confirmed unchanged: `/v2/users`, `/v2/roles` and `/v2/identity` are never impersonated
- [x] `TestWebApplicationFactory` now supplies `AdminUser:Password` explicitly (keeping the built-in username so `DevAuthenticationHandler` finds the admin deterministically)
- [x] Tests: `AdminUserSettingsTests` (null when missing/blank, configured values win, identity defaults)

### WS-DeadCode — Experiment cleanup

**Status:** ☑ &nbsp; **Size:** S &nbsp; **Completed:** 2026-09-21

- [x] Removed the commented-out TPT / relationship experiments: the `TblObject` leftovers (WS1), the `OrderItem` sample in `LocoDbContext.OnModelCreating`, and the commented `ToDto`/`ToTbl` members in `DbSubObject`
- [x] Removed the empty `IServerFolderManager` interface and the unused `TestServerFolderManager` (neither had any callers)

---

## Suggested sequencing

| Order | Workstream(s)              | Size | Depends on   |
| ----- | -------------------------- | ---- | ------------ |
| 1     | Phase 0 decisions          | S    | —            |
| 2     | WS3, WS4, WS12, WS11, WS13 | M    | D5           |
| 3     | WS7, WS14                  | M    | D4           |
| 4     | WS5 (+ WS5b)               | L    | D1           |
| 5     | WS1, WS2                   | L    | WS5, D2 / D3 |
| 6     | WS6                        | M    | WS5, D7      |
| 7     | WS8                        | S–M  | D6           |
| 8     | WS10, WS-DeadCode          | S    | —            |

---

## Verification strategy

- Full suite must stay green: `dotnet test Tests/Tests.csproj` (baseline after Phase 5: **2520 passed / 0 failed**, 5 skips).
- Migration drift must stay clean: `dotnet ef migrations has-pending-model-changes --project Definitions` (also enforced by `.github/workflows/db-migrations.yml`).
- The whole solution must build: `dotnet build ObjectEditor.slnx` (covers `Gui`, `DatabaseTools`, `DatabaseToolsConsole`).
- Every schema change needs: fresh-DB test + legacy-DB upgrade test + the migration-drift CI gate.
- Every API shape change must update, in the same PR: `Definitions/Web/Client.cs`, `Gui/ObjectServiceClient.cs`, the Razor pages, and the integration tests.
- No workstream merges without a test that fails before and passes after.
- Keep `DatabaseTools` (`Services/DatabaseImportService.cs`, `Services/DatabaseExportService.cs`) and `DatabaseToolsConsole` compiling against any model/naming change.

---

## Progress Log

Add a row whenever a task or workstream is completed, with the PR/commit.

| Date       | Workstream     | Summary                                                                                                                                                               | PR / commit   |
| ---------- | -------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------- |
| 2026-09-20 | Initial review | Object-pack absolute-path fix, pack 404s, `/v2` Location prefix, game-data `OwnerUserId` backfill, `DbSubObjectHelper` update fix, duplicate index removal, +12 tests | _uncommitted_ |
| 2026-09-20 | Phase 1 (WS3, WS4, WS12, WS11, WS13) | Permissions reconciled + pack policies + author-based ownership; update validation + 409 on duplicate name; route/constant + `HttpContext` cleanup; dead `ObjectTypeMapping`/`GetDbSetForType` removed and mapping consolidated; uploads index relative paths. +23 tests (2510 green) | _uncommitted_ |
| 2026-09-20 | Phase 2 (WS7, WS14, WS8) | Pack GET-by-id returns a single descriptor; `/users/me` writes registered from the write path; Identity API moved to `/v2/identity` (**breaking change** for clients using `/register`, `/login`, `/manage/*`, `/logout`). WS2 (object delete) deferred behind WS1 per plan. +1 test (2511 green) | _uncommitted_ |
| 2026-09-21 | Phase 3 (WS5, WS5b) | 25 stale migrations squashed into `InitialBaseline`; `MigrationInitializer` journals the baseline for pre-migration databases; `DatabaseInitializer` uses `MigrateAsync`; `GameDataFileTableInitializer`/`ScenarioPackTableInitializer` (+ tests) deleted; `TestWebApplicationFactory` uses `Migrate()`; CI drift gate added. See "Known limitation" above for pre-baseline databases. 2508 green | _uncommitted_ |
| 2026-09-21 | Phase 4 (WS1) | `TblObject.SubObjectId` dropped (migration `20260921015814_DropObjectSubObjectId`); `DbSubObjectHelper` resolves the sub-object via the `Parent` FK; cascade delete proven by test; dead TPT comments removed. WS2 (object delete) is now unblocked. 2509 green | _uncommitted_ |
| 2026-09-21 | WS2 (object delete) | New per-category `Removed` folder (moved-to, never deleted; ignored by watchers + reconciliation); `DELETE /v2/objects/{id}` parks the DAT, drops the index entry and marks the row `Unavailable` (vanilla refused); the same file policy applied to the other game-data deletes; restoring a parked file flips the object back to `Available`; README + `ServerFolderManager` docs updated. 2515 green | _uncommitted_ |
| 2026-09-21 | Phase 4 (WS6) | `SC5Files` → `Scenarios` navigation + DTO rename (migration `20260921024756_RenameSc5FilesNavigationToScenarios` renames the join columns/indexes/FKs); `SC5FilePackService.cs` renamed; page strings/Gui labels updated. Fixed two latent bugs: `DatabaseTools` export/import filename mismatch (`sc5FilePacks.json` vs `scenarioPacks.json`) and the dead `category=sc5files` links. **Breaking:** descriptor JSON field `SC5Files` → `Scenarios`. 2515 green | _uncommitted_ |
| 2026-09-21 | Phase 5 (WS10, WS-DeadCode) | Admin password no longer defaulted in code (`AdminUserSettings`; admin is skipped + logged when unconfigured); dev quick-login credentials moved to `DevAuth` config; `appsettings.Development.json` holds dev-only values; dead TPT/relationship comments and the unused `IServerFolderManager`/`TestServerFolderManager` removed; README config section added. +5 tests (2520 green) | _uncommitted_ |

## Discovered during remediation

Issues found while implementing that were not in the original review. Not yet scheduled.

| Date | Area | Finding | Suggested fix |
| ------------ | ------------- | --------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------ |
| 2026-09-20 | Frontend | `DtoInfoResponse.UserName` is never populated: ASP.NET Identity's `/manage/info` only returns `email` / `isEmailConfirmed`, so `Pages/Account/Manage` renders an empty username. | Drop `UserName` from `DtoInfoResponse`, or populate the page from `GET /v2/users/{id}` instead. |
| 2026-09-21 | API | `PUT /v2/objects/{id}` accepts `DtoObjectPostResponse.SubObject` but silently ignores it — sub-object edits via the API are discarded. | Add DTO→DAT reverse mappers for the sub-object types and call `DbSubObjectHelper.AddOrUpdate` from `ObjectQueryService.UpdateAsync` when `SubObject` is supplied. |
