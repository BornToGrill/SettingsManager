
namespace SettingsManager.Serialization.Helpers {

    /// <summary>
    /// Represents an object containing data read from a <see cref="System.IO.TextReader"/>.
    /// </summary>
    public class ReadValueObject {
        /// <summary>
        /// Gets or sets a value indiciting the name of a property/field.
        /// </summary>
        internal string Name { get; set; }
        /// <summary>
        /// Gets or sets a value indiciting the value of a property/field.
        /// </summary>
        internal string Value { get; set; }
    }
}
