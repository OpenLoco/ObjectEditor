

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

## Technical Details

### Database
- The server is backed by a simple SQLite database.
- You can view the current schema for it [here](https://github.com/OpenLoco/ObjectEditor/tree/master/Definitions/Database).
- Whilst the schema is in heavy development and subject to frequent change, instead of the database being the source of truth, for now all the data is stored in JSON files locally. The [DatabaseSeeder](https://github.com/OpenLoco/ObjectEditor/blob/master/DatabaseSeeder/Program.cs) project is what reads these files and populates the database with the existing data. This enables quick iteration or schema and web server without fear of losing data. In future, the database will become the source of truth.

### Web Server
- The API is rate-limited to a burst limit of [20 requests per second](https://github.com/OpenLoco/ObjectEditor/blob/master/ObjectService/ObjectServiceRateLimitOptions.cs) with 10 tokens replenished every second. This is a global limit, regardless of client. This will be [changed in the future](https://github.com/OpenLoco/ObjectEditor/issues/76).
- The server runs on a spare PC I have converted into a Linux (Ubuntu) server. It runs the Object Service as a daemon under systemctl and has an auto-restart configured in case it crashes.
- The domain is registered with Cloudflare. I have a `cloudflared` daemon running on the server that connects Cloudflare's servers and DNS lookup/routing to the server. This tunnel/daemon/Cloudflare hosting setup also acts as a reverse-proxy meaning I don't need to worry about load-balancing or exposing my public IP address to anyone.
