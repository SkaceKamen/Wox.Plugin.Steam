using System.Collections.Generic;

namespace NeXt.Vdf
{
    /// <summary>
    /// Abstract VdfValue
    /// </summary>
    public abstract class VdfValue
    {
        public VdfValue(string name)
        {
            Name = name;
        }

        /// <summary>
        /// This values name
        /// </summary>
        public string Name { get; private set; }

        private List<string> _comments = new List<string>();

        /// <summary>
        /// This values type, determines how it can be casted
        /// </summary>
        public abstract VdfValueType Type { get; }

        /// <summary>
        /// Comments that where in front of this VdfValue
        /// </summary>
        public ICollection<string> Comments
        {
            get { return _comments; }
        }

        /// <summary>
        /// This values Parent, null for root 
        /// </summary>
        public VdfValue Parent { get; internal set; }
    }
}