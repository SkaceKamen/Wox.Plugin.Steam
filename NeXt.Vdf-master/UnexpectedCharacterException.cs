namespace NeXt.Vdf
{
    /// <summary>
    /// A character was unexpected during deserialization
    /// </summary>
    public class UnexpectedCharacterException : VdfDeserializationException
    {
        public UnexpectedCharacterException(string message, char c) : base($"Unexpected character: {c}. {message}")
        {
        }
    }
}