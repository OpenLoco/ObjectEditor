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
| 2     | WS2 — Object delete                     | ⊘      |
| 2     | WS14 — Route registration               | ☑      |
| 2     | WS8 — Identity prefix                   | ☑      |
| 3     | WS5 — EF migrations (+ WS5b)            | ☐      |
| 4     | WS1 — Sub-object identity               | ☐      |
| 4     | WS6 — `SC5Files` → `Scenarios`          | ☐      |
| 5     | WS10 — Credentials                      | ☐      |
| 5     | WS-DeadCode — Experiment cleanup        | ☐      |

---

## Root causes

Two root causes sit behind most of the findings:

1. **Schema management is split-brain.**
   `Definitions/Migrations` exists (25 migrations, latest `20260918063013_DropBridgeVar03`, and
   `Definitions.csproj` references `Microsoft.EntityFrameworkCore.Design`), but nothing ever calls
   `Migrate()` / `MigrateAsync()`. Runtime uses `EnsureCreatedAsync()` plus three hand-written
   upgrader classes (`DatabaseInitializer`, `GameDataFileTableInitializer`, `ScenarioPackTableInitializer`).
   The model snapshot is **stale**: it still models `TblSC5FilePack` and has no
   `TblMusic` / `TblSoundEffect` / `TblTutorial` / `TblGraphics`, which is why the runtime table
   initialiser was written. Any new column needs another bespoke patch.

2. **The object / sub-object model is a workaround.**
   TPT was attempted and reverted (`20250630043418_SubObjectTPT` migration contains commented-out FK
   operations), leaving `TblObject.SubObjectId` as an unconstrained scalar that duplicates the
   `Parent` FK each sub-table already has. There is no referential integrity and rows can orphan.

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

## Phase 2 — API surface & auth alignment — ✅ complete 2026-09-20 (WS2 deferred to Phase 4)

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

**Status:** ⊘ blocked &nbsp; **Size:** L &nbsp; **Depends on:** D3, WS5, WS1

> **Deferred to after WS1/WS5.** Object deletion must remove the sub-object row, and the sub-object model
> currently has no relationship to cascade from (`TblObject.SubObjectId` is an unconstrained scalar). Doing
> it now would need a throwaway 34-case per-`DbSet` switch that WS1 deletes again — so it is queued behind
> WS1 (sub-object identity) as planned. D3a (implement delete) is still the agreed direction.

- [ ] Add `IObjectQueryService.DeleteObjectAsync`
- [ ] Delete sub-object row (via `Parent` FK after WS1), `StringTable` rows, `DatObject` rows, `ObjectPacks` links, then the `TblObject`
- [ ] Delete the DAT file only for `ObjectSource.Custom`; remove the `ObjectIndex` entry and `SaveIndexAsync`
- [ ] Refuse vanilla (Steam/GoG) and `Unavailable` objects (mirror the update guards)
- [ ] Update `Tests/.../ObjectRoutesTest.DeleteAsync` (currently asserts delete is a **no-op**) and add cascade coverage
- [ ] Update `Definitions/Web/Client.cs` / Gui if a delete client method is wanted

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

## Phase 3 — Schema management (prerequisite for Phase 4)

### WS5 — Adopt EF migrations

**Status:** ☐ &nbsp; **Size:** L &nbsp; **Depends on:** D1

- [ ] Regenerate a **baseline**: one migration that matches the _current_ model (also captures the removed duplicate indexes, the game-data tables, and the `TblScenarioPack` rename)
- [ ] Add a **baseline-journal step**: if a DB has tables but no `__EFMigrationsHistory`, insert the baseline row _before_ `MigrateAsync()` — otherwise every existing deployment fails
- [ ] Replace `EnsureCreatedAsync()` with `MigrateAsync()` in `DatabaseInitializer`
- [ ] Delete `GameDataFileTableInitializer.cs` and `ScenarioPackTableInitializer.cs` (+ their tests); express the scenario-pack rename as a migration
- [ ] Keep `ObjectService:DeleteDatabaseOnStartup` for dev; let tests opt into either path
- [ ] Add a CI gate: `dotnet ef migrations has-pending-model-changes` must be clean
- [ ] Tests: `DatabaseInitializerTests` for (i) fresh DB, (ii) legacy DB with no history, (iii) already-migrated DB

### WS5b — `OwnerUserId` backfill

**Status:** ☐ &nbsp; **Size:** S &nbsp; **Depends on:** WS5

- [ ] Fold the interim `ALTER TABLE ... ADD COLUMN OwnerUserId` loop into the migration baseline; keep it only if the legacy-baseline path still needs it for option (b) of D1

---

## Phase 4 — Data-model refactors (after Phase 3)

### WS1 — Sub-object identity

**Status:** ☐ &nbsp; **Size:** L &nbsp; **Depends on:** WS5, D2

- [ ] Drop `TblObject.SubObjectId`
- [ ] Rewrite `AddOrUpdateCore` to look up the existing row via `x.Parent.Id == parentObj.Id`
- [ ] Ensure `DbSubObject.Parent` is a required FK with `OnDelete(Cascade)` so object deletion cleans up sub-objects
- [ ] Decide whether `ObjectQueryService.UpdateAsync` should also update the sub-object (it currently ignores sub-object changes)
- [ ] Migration: drop the column (and any index on it)
- [ ] Update seeds in `ObjectPackRoutesTest`, `ObjectRoutesTest`, `ObjectsFolderServiceTests`, `DbSubObjectHelperTests`
- [ ] Tests: extend `DbSubObjectHelperTests` (insert/update via `Parent`) + a cascade-delete test

> Note: `DatabaseTools` import/export does not read `SubObjectId`, so it is unaffected.

### WS6 — `SC5Files` → `Scenarios` naming

**Status:** ☐ &nbsp; **Size:** M &nbsp; **Depends on:** WS5, D7

- [ ] Rename the navigation on `TblAuthor` / `TblTag` and the DTO properties in `Definitions/DTO/DtoWeb.cs`
- [ ] Update `ReferenceDataService`, `Pages/{Authors,Tags,Licences}/Details.*`, and route tests
- [ ] Migration: rename the join tables (`TblAuthorTblSC5File` → `TblAuthorTblScenario`, `TblTagTblSC5File` → `TblTagTblScenario`)
- [ ] Update "SC5 Files" headings/strings in the pages

---

## Phase 5 — Security & polish

### WS10 — Hardcoded credentials

**Status:** ☐ &nbsp; **Size:** S

- [ ] Replace the hardcoded admin-password fallback with fail-fast (or force `AdminUser:Password` / user-secrets)
- [ ] Gate and document the dev quick-login password; confirm the dev auth scheme stays excluded from `AdminOnly` / identity endpoints

### WS-DeadCode — Experiment cleanup

**Status:** ☐ &nbsp; **Size:** S

- [ ] Remove the commented-out TPT / relationship experiments in `Definitions/Database/LocoDbContext.cs` and `TblObject.cs`
- [ ] Remove `TestServerFolderManager` / the empty `IServerFolderManager` if unused

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

- Full suite must stay green: `dotnet test Tests/Tests.csproj` (baseline after Phase 2: **2511 passed / 0 failed**, 5 skips).
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

## Discovered during remediation

Issues found while implementing that were not in the original review. Not yet scheduled.

| Date | Area | Finding | Suggested fix |
| ------------ | ------------- | --------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------ |
| 2026-09-20 | Frontend | `DtoInfoResponse.UserName` is never populated: ASP.NET Identity's `/manage/info` only returns `email` / `isEmailConfirmed`, so `Pages/Account/Manage` renders an empty username. | Drop `UserName` from `DtoInfoResponse`, or populate the page from `GET /v2/users/{id}` instead. |
