using System;
using System.IO;
using System.Linq;
using SettingsManager.Serialization.Helpers;

namespace SettingsManager.Serialization {

    /// <summary>
    /// Represents a writer that provides methods for writing data to a underlying <see cref="TextWriter"/>.
    /// </summary>
    public class PlainTextWriter : PlainWriter {

        /// <summary>
        /// Gets a value indicating whether the <see cref="PlainTextWriter"/> object has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets a value indicating which <see cref="TextWriter"/> is being used by the <see cref="PlainTextWriter"/>.
        /// </summary>
        protected override TextWriter Writer { get; }

        /// <summary>
        /// Gets a value indicating which <see cref="PlainTextWriterSettings"/> is being used by the <see cref="PlainTextWriter"/>.
        /// </summary>
        public PlainTextWriterSettings Settings { get; private set; }

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PlainTextWriter"/> class using the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="output">The <see cref="Stream"/> to which the <see cref="PlainTextWriter"/> should write.</param>
        /// <exception cref="ArgumentNullException"/><exception cref="ArgumentException"/>
        public PlainTextWriter(Stream output) : this(new StreamWriter(output))  { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlainTextWriter"/> class using the specified <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> to which the <see cref="PlainTextWriter"/> should write.</param>
        /// <exception cref="ArgumentNullException"/>
        public PlainTextWriter(TextWriter writer) {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            Writer = writer;
            Settings = new PlainTextWriterSettings();
        }

        /// <summary>
        /// Creates a new <see cref="PlainTextWriter"/> instance using the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="output">The <see cref="Stream"/> to which the <see cref="PlainTextWriter"/> should write.</param>
        /// <returns>An instance of the <see cref="PlainTextWriter"/> class.</returns>
        /// <exception cref="ArgumentNullException"/><exception cref="ArgumentException"/>
        public static PlainTextWriter Create(Stream output) {
            return Create(output, null);
        }

        /// <summary>
        /// Creates a new <see cref="PlainTextWriter"/> instance using the specified <see cref="Stream"/> and <see cref="PlainTextWriterSettings"/>.
        /// </summary>
        /// <param name="output">The <see cref="Stream"/> to which the <see cref="PlainTextWriter"/> should write.</param>
        /// <param name="settings">The <see cref="PlainTextWriterSettings"/> containing the settings that the <see cref="PlainTextWriter"/> should use when writing.</param>
        /// <returns>An instance of the <see cref="PlainTextWriter"/> class.</returns>
        /// <exception cref="ArgumentNullException"/><exception cref="ArgumentException"/>
        public static PlainTextWriter Create(Stream output, PlainTextWriterSettings settings) {
            if (output == null)
                throw new ArgumentNullException(nameof(output));

            settings = settings ?? new PlainTextWriterSettings();
            StreamWriter stream = new StreamWriter(output, settings.Encoding);

            return new PlainTextWriter(stream) {
                Settings = settings
            };
        }

        /// <summary>
        /// Creates a new <see cref="PlainTextWriter"/> instance using the specified <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> to which the <see cref="PlainTextWriter"/> should write.</param>
        /// <returns>An instance of the <see cref="PlainTextWriter"/> class.</returns>
        /// <exception cref="ArgumentNullException"/>
        public static PlainTextWriter Create(TextWriter writer) {
            return Create(writer, null);
        }

        /// <summary>
        /// Creates a new <see cref="PlainTextWriter"/> instance using the specified <see cref="TextWriter"/> and <see cref="PlainTextWriterSettings"/>.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> to which the <see cref="PlainTextWriter"/> should write.</param>
        /// <param name="settings">The <see cref="PlainTextWriterSettings"/> containing the settings that the <see cref="PlainTextWriter"/> should use when writing.</param>
        /// <returns>An instance of the <see cref="PlainTextWriter"/> class.</returns>
        /// <exception cref="ArgumentNullException"/>
        public static PlainTextWriter Create(TextWriter writer, PlainTextWriterSettings settings) {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            return new PlainTextWriter(writer) {
                Settings = settings ?? new PlainTextWriterSettings()
            };
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Writes a header to the underlying <see cref="TextWriter"/> using the specified strings.
        /// </summary>
        /// <param name="args">The <see cref="string"/> that should be writen to the header.</param>
        /// <exception cref="ObjectDisposedException"/>
        /// <exception cref="IOException">Occurs when the field that was attempted to write to was inaccessible.</exception>
        public override void WriteHeader(params string[] args) {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (Settings.OmitHeader)
                return;
            string[] activeParams = args.Where(x => x != null).ToArray();
            if(activeParams.Length <= 0)
                return;
            int maxLength = activeParams.Max().Length;
            if (maxLength < 3)
                maxLength = 3;

            string terminator = new string('=', maxLength + 2);
            WriteComment(terminator, "///");

            foreach (string element in activeParams) {
                WriteComment(element, "//");
            }
            WriteComment(terminator, "///");
            Writer.WriteLine();
        }

        /// <summary>
        /// Writes a propery to the underlying <see cref="TextWriter"/> using the specified name and value.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <param name="optional">Whether assigning the property is optional or not.</param>
        /// <param name="newLine">Whether the property should be writen on a new line.</param>
        /// <exception cref="ObjectDisposedException"/>
        /// <exception cref="IOException">Occurs when the field that was attempted to write to was inaccessible.</exception>
        public override void WriteProperty(string name, string value, bool optional = false, bool newLine = false) {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName);

            string format = Settings.SeparatorWhitespace ? " " : "";
            string output = string.Join(format, name, Settings.Separator, value);

            if (newLine)
                Writer.WriteLine();
            if (optional)
                WriteComment(output);
            else 
                Writer.WriteLine(output);
        }

        /// <summary>
        /// Writes a comment to the underlying <see cref="TextWriter"/> using the specified comment.
        /// </summary>
        /// <param name="comment">The comment that should be writen.</param>
        /// <exception cref="ObjectDisposedException"/>
        /// <exception cref="IOException">Occurs when the field that was attempted to write to was inaccessible.</exception>
        public override void WriteComment(string comment) {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName);

            Writer.WriteLine($"{Settings.CommentIndicator} {comment}");
        }

        /// <summary>
        /// Writes a comment to the underlying <see cref="TextWriter"/> using the specified comment and comment indicator.
        /// </summary>
        /// <param name="comment">The comment that should be writen.</param>
        /// <param name="commentIndicator">The indicator that will precede the comment.</param>
        /// <exception cref="ObjectDisposedException"/>
        /// <exception cref="IOException">Occurs when the field that was attempted to write to was inaccessible.</exception>
        public void WriteComment(string comment, string commentIndicator) {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName);

            Writer.WriteLine($"{commentIndicator} {comment}");
        }
        #endregion
        #region Private Methods

        #endregion

        #region IDisposable Implementation Members

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="PlainTextWriter"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing) {
            if (IsDisposed)
                return;
            if (disposing) {
                Writer.Dispose();
            }
            base.Dispose(disposing);
            IsDisposed = true;
        }
        #endregion

    }
}
