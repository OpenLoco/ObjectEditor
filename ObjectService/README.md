

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

### GET
- `/objects/list`
  - Description: Returns a list of all objects indexed/available to view/download from the repository. Primary purpose is for all consumers to do object discovery.
  - Example: `https://openloco.leftofzen.dev/objects/list`
- `/objects/getdat?{objectName}&{checksum}&{returnObjBytes}`
  - Description: Returns a specific object keyed off its object name and checksum. Primary purpose is for the Object Editor to retrieve objects and metadata.
  - Parameters:
      - `objectName`: the name of the object as per it's S5Header::Name property.
      - `checksum`: the checksum of the file as per its S5Header::Checksum property.
     - `returnObjBytes`: an optional parameter. `true` is you want the original byte[] for the object to be included, `false` if not.
  - Example: `https://openloco.leftofzen.dev/objects/getdat?objectName=M6SBO&checksum=2032077333&returnObjBytes=false`
- `/objects/getobject?{uniqueObjectId}&{returnObjBytes}`
  - Description: Returns a specific object keyed off its unique ID in the repository.  Primary purpose is for the Object Editor to retrieve objects and metadata.
  - Parameters:
      - `uniqueObjectId`: the unique ID of an object in the repository.
     - `returnObjBytes`: an optional parameter. `true` is you want the original byte[] for the object to be included, `false` if not.
  - Example: `https://openloco.leftofzen.dev/objects/getobject?uniqueObjectId=1024&returnObjBytes=false`
- `/objects/getdatfile?{objectName}&{checksum}`
  - Description: Returns a dat file keyed off its object name and checksum. Primary purpose is for OpenLoco to download specific objects.
  - Parameters:
      - `objectName`: the name of the object as per it's S5Header::Name property.
      - `checksum`: the checksum of the file as per its S5Header::Checksum property.
  - Example: `https://openloco.leftofzen.dev/objects/getdatfile?objectName=M6SBO&checksum=2032077333`
- `/objects/getobjectfile?{uniqueObjectId}`
  - Description: Returns a dat file keyed off its unique ID in the repository. Primary purpose is for OpenLoco to download specific objects.
  - Parameters:
      - `uniqueObjectId`: the unique ID of an object in the repository.
  - Example: `https://openloco.leftofzen.dev/objects/getobjectfile?uniqueObjectId=1024`

### POST
- `/objects/uploaddat`
  - Uploads a given dat file to the object repository.
  - Content type: `application/json`
  - Parameters:
    -  `datBytesAsBase64`: a string that is the base64 encoding of the raw bytes of the dat file
    - `creationDate`: a `DateTimeOffset` representing the creation date of the dat file. If using the Object Editor, it will use [`File.GetLastWriteTimeUtc()`](https://learn.microsoft.com/en-us/dotnet/api/system.io.file.getlastwritetimeutc?view=net-8.0), which on Windows is the "Modified" time for a file. For historical dat files, this is the only way to get a creation date and it may be incorrect or inaccurate.
  - Example request body:
    ```
    {
      "datBytesAsBase64": "c29tZSBraW5kIG9mIGVhc3RlciBlZ2c=",
      "creationDate": "2024-08-29T08:52:17.372Z"
    }
    ```

## Technical Details

### Database
- The server is backed by a simple SQLite database.
- You can view the current schema for it [here](https://github.com/OpenLoco/ObjectEditor/tree/master/Definitions/Database).
- Whilst the schema is in heavy development and subject to frequent change, instead of the database being the source of truth, for now all the data is stored in JSON files locally. The [DatabaseSeeder](https://github.com/OpenLoco/ObjectEditor/blob/master/DatabaseSeeder/Program.cs) project is what reads these files and populates the database with the existing data. This enables quick iteration or schema and web server without fear of losing data. In future, the database will become the source of truth.

### Web Server
- The API is rate-limited to a burst limit of [20 requests per second](https://github.com/OpenLoco/ObjectEditor/blob/master/ObjectService/ObjectServiceRateLimitOptions.cs) with 10 tokens replenished every second. This is a global limit, regardless of client. This will be [changed in the future](https://github.com/OpenLoco/ObjectEditor/issues/76).
- The server runs on a spare PC I have converted into a Linux (Ubuntu) server. It runs the Object Service as a daemon under systemctl and has an auto-restart configured in case it crashes.
- The domain is registered with Cloudflare. I have a `cloudflared` daemon running on the server that connects Cloudflare's servers and DNS lookup/routing to the server. This tunnel/daemon/Cloudflare hosting setup also acts as a reverse-proxy meaning I don't need to worry about load-balancing or exposing my public IP address to anyone.