namespace NeXt.Vdf
{
    /// <summary>
    /// A VdfValue that represents an integer
    /// </summary>
    public sealed class VdfInteger : VdfValue
    {
        public VdfInteger(string name, int value) : base(name)
        {
            Content = value;
        }

        public int Content { get; }
        public override VdfValueType Type => VdfValueType.Integer;
    }
}