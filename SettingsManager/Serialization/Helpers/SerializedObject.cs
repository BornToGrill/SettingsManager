using System;

namespace SettingsManager.Serialization.Helpers {

    /// <summary>
    /// Represents a serialized object.
    /// </summary>
    internal sealed class SerializedObject {

        /// <summary>
        /// Gets or sets the <see cref="Type"/> of the serialized <see cref="object"/>.
        /// </summary>
        internal Type Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the name of the serialized <see cref="object"/>.
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the encoded value of the serialized <see cref="object"/>.
        /// </summary>
        internal string SerializedValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the priority of the serialized <see cref="object"/>.
        /// </summary>
        internal int Priority { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether assigning the serialized <see cref="object"/> is optional.
        /// </summary>
        internal bool Optional { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the serialized <see cref="object"/> should be writen on a new line.
        /// </summary>
        internal bool NewLine { get; set; }
    }
}
