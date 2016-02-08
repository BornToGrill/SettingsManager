using System.Text;

namespace SettingsManager.Serialization {

    /// <summary>
    /// Specifies a set of features to support on the <see cref="PlainTextWriter"/> object.
    /// </summary>
    public class PlainTextWriterSettings {

        /// <summary>
        /// Gets or sets a value indicating which <see cref="System.Text.Encoding"/> the <see cref="PlainTextWriter"/> should use.
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// Gets or sets a value indicating what seperator character the <see cref="PlainTextWriter"/> should use.
        /// </summary>
        public char Separator { get; set; } = '=';

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="PlainTextWriter"/> should write whitespace to either side of the separator.
        /// </summary>
        public bool SeparatorWhitespace { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="PlainTextWriter"/> should not write the header.
        /// </summary>
        public bool OmitHeader { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating how the <see cref="PlainTextWriter"/> should represent commented lines.
        /// </summary>
        public string CommentIndicator { get; set; } = "//";
    }
}
