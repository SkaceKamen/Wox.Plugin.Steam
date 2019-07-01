namespace NeXt.Vdf
{
    /// <summary>
    /// A VdfValue that represents a double
    /// </summary>
    public sealed class VdfDouble : VdfValue
    {
        public override VdfValueType Type => VdfValueType.Double;

        public VdfDouble(string name, double value) : base(name)
        {
            Content = value;
        }

        public double Content { get; }
    }
}