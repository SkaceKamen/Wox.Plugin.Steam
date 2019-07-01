namespace NeXt.Vdf
{
    /// <summary>
    /// A VdfValue that represents a string
    /// </summary>
    public sealed class VdfString : VdfValue
    {
        public VdfString(string name, string value) : base(name)
        {
            Content = value;
        }

        public string Content { get; set; }
        public override VdfValueType Type => VdfValueType.String;
    }
}