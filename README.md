# OpenLocoTool
A modern implementation of 'LocoTool' for dat file parsing

# GUI

1. open UI, no loading
2. user selects a directory
3. if no open-loco-tool index file exists, open-loco-tool fully loads all dat files in directory, creates an index and writes it to `objectIndex.json` in that folder. this is SLOW (currently)
4. next time that directory is opened, the index is read instead of loading all files. this is FAST

# Dat Object Layout

|-File-------------------------------|
|-S5Header-|-DatHeader--|-ObjectData-|

======================================================================================

|-S5Header----------------|
|-Flags-|-Name-|-Checksum-|

|-DatHeader-------------|
|-Encoding-|-Datalength-|

|-ObjectData-----------------------------------------|
|-Object-|-StringTable-|-VariableData-|-GraphicsData-|

======================================================================================

|-Object-|
-- <per-object>

|-StringTable-|
|-String{n}---|

|-VariableData-|
-- <per-object>

|-GraphicsData------------------------------|
|-G1Header-|-G1Element32{n}-|-ImageBytes{n}-|

======================================================================================

|-String-----------------|
|-Language-|-StringBytes-|

|-G1Header---------------|
|-NumEntries-|-TotalSize-|

|-G1Element32------------------------------------------------------|
|-Offset-|-Width-|-Height-|-xOffset-|-yOffset-|-Flags-|-ZoomOffset-|

|-ImageBytes-|
-- offset by G1Element32.Offset

