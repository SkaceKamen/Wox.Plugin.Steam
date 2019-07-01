using System;

namespace NeXt.Vdf
{
    /// <summary>
    /// Base class for al Deserialization exceptions
    /// </summary>
    public class VdfDeserializationException : Exception
    {
        public VdfDeserializationException(string message) : base(message)
        {
        }
    }
}