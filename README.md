# OpenLocoTool
A modern implementation of 'LocoTool' for dat file parsing

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

