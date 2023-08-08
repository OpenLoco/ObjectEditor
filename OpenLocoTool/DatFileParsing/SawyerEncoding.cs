namespace OpenLocoTool.DatFileParsing
{
    public enum SawyerEncoding : byte
    {
        uncompressed = 0,
        runLengthSingle = 1,
        runLengthMulti = 2,
        rotate = 3,
    }
}
