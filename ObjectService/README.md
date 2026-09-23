

# OpenLoco Object Service
An HTTP(S) "minimal API" made with ASP.NET Core. It serves information about the object repository as well as objects from the repository. It is currently hosted at `openloco.leftofzen.dev`

## Terminology
- dat file - the actual byte[] of a dat file
- object - a wrapper object around a dat file containing metadata such as tags, authors, dates, etc.
- S5Header - the first 16 bytes of a dat file, which provide the object name, object type, checksum and source game.

## Overview
### Getting objects
1. First query the server with the `list` route, as seen below. This will return some metadata about each object in the repository, including information about how to query for an entire object.
2. Pick an object you would like more information about and send a request for that object with one of the other 4 GET routes.
That's it. It's real basic right now but it should suffice for most purposes.

### Uploading objects
You can technically manually call the `uploaddat` route but this is intended primarily for the Object Editor to use in an automated fashion. It isn't for individual use.

`PUT /v2/objects/{id}` replaces the object: it is the caller's description of what the object should be. Every column of the `Objects` header row (`Name`, `ObjectType`, `VehicleType`, `Description`, dates, `Availability`, licence, authors, tags, object packs) and the object's sub-object table are applied exactly as sent, and the string table becomes exactly the rows sent (an empty table clears it). An omitted sub-object removes the stored row, and changing `ObjectType` moves the object to the new type's table, dropping the previous type's row — so clients send the whole object, which `POST /v2/objects` does (including an empty string table).

A sub-object that does not belong to the declared `ObjectType` (`DtoObjectVehicle` for `ObjectType.Airport`, say) is a `400`, a name already used by another object is a `409`, and a vanilla Locomotion object is a `403` - all are rejected before anything is written. Deletion is `DELETE` and partial modification would be `PATCH`, which is not implemented.

Two things are **not** the client's to set, because they describe the file rather than the object:

- `ObjectSource` is server-owned. Uploading through `POST /v2/objects` always stores `Custom` (vanilla uploads are refused outright), and `LocomotionSteam`/`LocomotionGoG`/`OpenLoco` objects are placed in the server folders by hand, so no web request can move an object between sources - a request asking for a different source is ignored (and logged).
- `DatObjects` is the DAT file(s) on disk that the object is built from, so nothing reassigns them.

`DisplayName`, `DatChecksum` and `UploadedDate` are projections: the first two are derived from the object's files and the last is set by the database when the row is created.

## Server Admin
- `sudo systemctl start objectservice.service`
- `sudo systemctl restart objectservice.service`
- `sudo systemctl stop objectservice.service`
- `sudo journalctl --disk-usage`
- `sudo journalctl --vacuum-time=5days`
- `sudo journalctl -u objectservice.service`

## API
- See https://openloco.leftofzen.dev/api/
- To check status of the service, query https://openloco.leftofzen.dev/health/

### Game-data file routes
Every entity stored from a `GameData` folder has its own route group, following the same shape as the scenario implementation:
- `GET /v2/<entity>` - list, `GET /v2/<entity>/{id}` - descriptor, `PUT /v2/<entity>/{id}` - update metadata, `DELETE /v2/<entity>/{id}` - delete, `GET /v2/<entity>/{id}/file` - download the file. `POST` is not implemented (these rows are created by the file watcher).
- `DELETE` never destroys data: the file is moved into the category's `Removed` subfolder (see [Runtime file watching](#runtime-file-watching)) and the row either becomes unavailable (objects) or is removed (other entities).
- Route groups: `/v2/scenarios` (scenario and landscape metadata, backed by the `Scenarios` table), `/v2/music`, `/v2/soundeffects`, `/v2/tutorials`, `/v2/graphics`.
- Packs: `/v2/objectpacks` and `/v2/scenariopacks`, where `GET /v2/scenariopacks/{id}/file` returns a zip of the pack's scenario files.
- Only metadata is stored in the database - the files themselves always live on disk under the `GameData` folders, so a database row may or may not have a corresponding file (the file routes return `404` in that case).

## Technical Details

### Database
- The server is backed by a simple SQLite database.
- You can view the current schema for it [here](https://github.com/OpenLoco/ObjectEditor/tree/master/Definitions/Database).
- Whilst the schema is in heavy development and subject to frequent change, instead of the database being the source of truth, for now all the data is stored in JSON files locally. The [DatabaseSeeder](https://github.com/OpenLoco/ObjectEditor/blob/master/DatabaseSeeder/Program.cs) project is what reads these files and populates the database with the existing data. This enables quick iteration or schema and web server without fear of losing data. In future, the database will become the source of truth.

### Runtime file watching
- A **single hosted service**, `GameDataWatcherService` (`ObjectService:EnableFileWatcher`, default `true`), watches everything under `GameData`. It owns **one `FileSystemWatcher` per category folder** - `Graphics`, `Landscapes`, `Music`, `Objects`, `Scenarios`, `SoundEffects` and `Tutorials` - so a busy folder can never overflow a shared OS buffer and starve the others, and so events arrive already scoped to the correct folder.
- Every folder has **its own entity-specific service**, because a game object is a different entity from a scenario, landscape, tutorial, sound effect, music file or graphics file. Each service implements `IGameDataFileService` (`ImportAsync` / `RemoveAsync` / `ReconcileAsync`) and is registered separately:
  - `ObjectsFolderService` - maintains `objectIndex.json` and the object tables (including sub-object and string-table data).
  - `ScenariosFolderService`, `LandscapesFolderService` - maintain the `Scenarios` table (they share an implementation because scenarios and landscapes are the same S5 entity type).
  - `TutorialsFolderService`, `SoundEffectsFolderService`, `MusicFolderService`, `GraphicsFolderService` - distinct services for the remaining entity types, each persisting files into its own table (`Tutorials`, `SoundEffects`, `Music`, `Graphics`).
- `GameDataFolderWatcher` is the abstract base class holding all the common watcher machinery (watcher setup, event queueing, waiting for a copy-in to finish, rename/delete handling, temp/index-file filtering, the shared write lock and reconciliation dispatch). The seven concrete watchers (`ObjectsFolderWatcher`, `ScenariosFolderWatcher`, `LandscapesFolderWatcher`, `TutorialsFolderWatcher`, `SoundEffectsFolderWatcher`, `MusicFolderWatcher`, `GraphicsFolderWatcher`) only supply their folder and resolve their own service.
- Deleting/moving a file removes it from the object index and marks the corresponding database object as `Unavailable` (metadata such as authors/tags/packs is preserved).
- Every category folder contains a `Removed` subfolder. Files deleted through the API are **moved** there (never deleted) so a removal is recoverable, and `Removed` is ignored by the watchers and by reconciliation so a parked file is never re-imported. Parked files keep their relative path (`Custom/x.dat` -> `Removed/Custom/x.dat`).
- On startup every folder is reconciled by its own service (files added/removed while the server was offline are picked up).
- A single shared `GameDataWatcherLock` serialises file operations across all seven folders so the object index and SQLite database are never written concurrently.
- Schema changes are delivered as EF migrations (`Definitions/Migrations`). `DatabaseInitializer` calls `Migrate()` on startup; databases created before migrations were adopted have the baseline recorded in `__EFMigrationsHistory` first (see `MigrationInitializer`), so they are never re-created. The file-entity tables (`Music`, `SoundEffects`, `Tutorials`, `Graphics`) are part of the baseline.

### Configuration
- **`AdminUser:Password`** is required to bootstrap the system admin account. It is never defaulted in code: when it is missing the admin is simply not created (an **error** is logged outside Development), so deployments must supply it via user-secrets or environment variables. `AdminUser:Email` / `AdminUser:Username` fall back to a non-secret display identity.
- **`DevAuth:Email` / `DevAuth:Password`** configure the development-only `/dev/quick-login` endpoint and the quick-login page. The endpoint is only mapped when the environment is Development and returns `503` when unset.
- Development-only values for both live in `appsettings.Development.json`; `appsettings.json` intentionally contains neither.
- `ObjectService:DisableAuthentication` enables the dev authentication scheme, which impersonates the admin user for `/v2` requests. It is **only honoured in the Development environment** and is deliberately excluded for `/v2/users`, `/v2/roles` and `/v2/identity` so identity flows are exercised for real.

### Web Server
- The API is rate-limited to a burst limit of [20 requests per second](https://github.com/OpenLoco/ObjectEditor/blob/master/ObjectService/ObjectServiceRateLimitOptions.cs) with 10 tokens replenished every second. This is a global limit, regardless of client. This will be [changed in the future](https://github.com/OpenLoco/ObjectEditor/issues/76).
- The server runs on a spare PC I have converted into a Linux (Ubuntu) server. It runs the Object Service as a daemon under systemctl and has an auto-restart configured in case it crashes.
- The domain is registered with Cloudflare. I have a `cloudflared` daemon running on the server that connects Cloudflare's servers and DNS lookup/routing to the server. This tunnel/daemon/Cloudflare hosting setup also acts as a reverse-proxy meaning I don't need to worry about load-balancing or exposing my public IP address to anyone.
