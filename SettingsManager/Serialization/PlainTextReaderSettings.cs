
using System.Text;

namespace SettingsManager.Serialization {

    /// <summary>
    /// Specifies a set of features to support on the <see cref="PlainTextReader"/> object.
    /// </summary>
    public class PlainTextReaderSettings {

        /// <summary>
        /// Gets or sets a value indicating which <see cref="System.Text.Encoding"/> the <see cref="PlainTextReader"/> should use.
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// Gets or sets a value indicating what the seperator character will be inbetween the property name and value.
        /// </summary>
        public char Seperator { get; set; } = '=';

        /// <summary>
        /// Gets a value indicating what the comment indicators will be for the <see cref="PlainTextReader"/>.
        /// </summary>
        internal readonly string[] CommentIndicators = { "//", "#" };
    }
}
