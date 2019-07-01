namespace NeXt.Vdf
{
    internal enum CharacterType
    {
        Whitespace,
        Newline,
        SequenceDelimiter,
        CommentDelimiter,
        TableOpen,
        TableClose,
        EscapeChar,
        Char
    }
}