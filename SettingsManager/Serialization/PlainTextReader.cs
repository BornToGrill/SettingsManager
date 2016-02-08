using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SettingsManager.Serialization.Helpers;

namespace SettingsManager.Serialization {

    /// <summary>
    /// Represents a reader that provides forward-only access to plain text data.
    /// </summary>
    public class PlainTextReader : PlainReader {

        /// <summary>
        /// Gets a value indicating whether the <see cref="PlainTextReader"/> object has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets a value indicating what the position of the underlying <see cref="TextReader"/> is.
        /// </summary>
        public int Position { get; private set; }

        /// <summary>
        /// Gets a value indicating which <see cref="TextReader"/> is being used by the <see cref="PlainTextReader"/>.
        /// </summary>
        protected override TextReader Reader { get; }

        /// <summary>
        /// Gets a value indicating which <see cref="PlainTextReaderSettings"/> is being used by the <see cref="PlainTextReader"/>.
        /// </summary>
        public PlainTextReaderSettings Settings { get; private set; }

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PlainTextReader"/> class using the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The stream which contains the data to be read.</param>
        /// <exception cref="ArgumentNullException"/>
        public PlainTextReader(Stream stream) : this(new StreamReader(stream)) { }

        /// <summary>
        /// Initializes a new instance of the class <see cref="PlainTextReader"/> class using the specified <see cref="TextReader"/>.
        /// </summary>
        /// <param name="reader">The TextReader which contains the data to be read.</param>
        /// <exception cref="ArgumentNullException"/>
        public PlainTextReader(TextReader reader) {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            Reader = reader;
            Settings = new PlainTextReaderSettings();
        }

        /// <summary>
        /// Creates a new <see cref="PlainTextReader"/> instance using the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> from which the <see cref="PlainTextReader"/> should read.</param>
        /// <returns>An instance of the <see cref="PlainTextReader"/> class.</returns>
        /// <exception cref="ArgumentNullException"/>
        public static PlainTextReader Create(Stream stream) {
            return Create(stream, null);
        }

        /// <summary>
        /// Creates a new <see cref="PlainTextReader"/> instance using the specified <see cref="Stream"/> and <see cref="PlainTextReaderSettings"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> from which the <see cref="PlainTextReader"/> should read.</param>
        /// <param name="settings">The <see cref="PlainTextReaderSettings"/> containing the settings that the <see cref="PlainTextReader"/> should use when reading.</param>
        /// <returns>An instance of the <see cref="PlainTextReader"/> class.</returns>
        /// <exception cref="ArgumentNullException"/>
        public static PlainTextReader Create(Stream stream, PlainTextReaderSettings settings) {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            settings = settings ?? new PlainTextReaderSettings();
            StreamReader reader = new StreamReader(stream, settings.Encoding);

            return new PlainTextReader(reader) {
                Settings = settings
            };
        }

        /// <summary>
        /// Creates a new <see cref="PlainTextReader"/> instance using the specified <see cref="TextReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="TextReader"/> from which the <see cref="PlainTextReader"/> should read.</param>
        /// <returns>An instance of the <see cref="PlainTextReader"/> class.</returns>
        /// <exception cref="ArgumentNullException"/>
        public static PlainTextReader Create(TextReader reader) {
            return Create(reader, null);
        }

        /// <summary>
        /// Creates a new <see cref="PlainTextReader"/> instance using the specified <see cref="TextReader"/> and <see cref="PlainTextReaderSettings"/>.
        /// </summary>
        /// <param name="reader">The <see cref="TextReader"/> from which the <see cref="PlainTextReader"/> should read.</param>
        /// <param name="settings">The <see cref="PlainTextReaderSettings"/> containing the settings that the <see cref="PlainTextReader"/> should use when reading.</param>
        /// <returns>An instance of the <see cref="PlainTextReader"/> class.</returns>
        /// <exception cref="ArgumentNullException"/>
        public static PlainTextReader Create(TextReader reader, PlainTextReaderSettings settings) {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            return new PlainTextReader(reader) {
                Settings = settings ?? new PlainTextReaderSettings()
            };
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Reads the header from the reader.
        /// </summary>
        /// <returns>An <see cref="Array"/> of type <see cref="string"/> containing the header data if succesful, otherwise null.</returns>
        /// <exception cref="ObjectDisposedException"/>
        /// <exception cref="FormatException">Occurs when the header that was being read was in an incorrect format.</exception>
        public override string[] GetHeader() {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName);
            string firstLine = ReadLine();
            if (firstLine == null || !firstLine.StartsWith("///"))
                return null;
            var headerData = new List<string>();
            string current;
            while ((current = ReadLine()) != null && !current.StartsWith("///")) {
                if(current.Length < 2 || !current.StartsWith("//"))
                    throw new FormatException(Resources.SerializationExceptionStrings.HeaderFormat);
                headerData.Add(current.Substring(2).Trim());
            }
            return headerData.ToArray();
        }

        /// <summary>
        /// Reads the next value from the reader.
        /// </summary>
        /// <returns>An instance of <see cref="ReadValueObject"/> containing the read data.</returns>
        /// <exception cref="ObjectDisposedException"/>
        /// <exception cref="FormatException">Occurs when a property name contained a comment or is missing its separator.</exception>
        /// <exception cref="StackOverflowException">Occurs when the ammount of lines the reader has to read becomes greater than the stack size.</exception>
        public override ReadValueObject ReadNextValue() {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName);

            string line = ReadLine();
            if (line == null)
                return null;
            if (Settings.CommentIndicators.Any(x => line.StartsWith(x)) || line == string.Empty)
                return ReadNextValue();

            string[] values = SplitValueString(line);
            if(Settings.CommentIndicators.Any(x => values[0].Contains(x)))
                throw new FormatException(Resources.SerializationExceptionStrings.NameContainedComment);

            values[1] = RemoveComment(values[1]);

            return new ReadValueObject() {
                Name = values[0].Trim(),
                Value = values[1].Trim()
            };

        }

        /// <summary>
        /// Reads the next comment from the reader.
        /// </summary>
        /// <returns>A string with the read comment.</returns>
        /// <remarks>
        /// Only reads comments on lines starting with the comment indicator.
        /// If the comment follows a property it will not be detected.
        /// </remarks>
        /// <exception cref="ObjectDisposedException"/>
        /// <exception cref="StackOverflowException">Occurs when the ammount of lines the reader has to read becomes greater than the stack size.</exception>
        public override string ReadNextComment() {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName);

            string line = ReadLine();
            if (line == null)
                return null;
            if (!Settings.CommentIndicators.Any(x => line.StartsWith(x)))
                return ReadNextComment();
            return line.Substring(2).Trim();
        }
        #endregion

        #region Private Methods

        private string ReadLine() {
            string line = Reader.ReadLine();
            if (line != null)
                Position++;
            return line;
        }

        private string RemoveComment(string input) {
            foreach (string indicator in Settings.CommentIndicators) {
                int indicatorPos = input.IndexOf(indicator, StringComparison.Ordinal);
                if (indicatorPos >= 0)
                    return input.Substring(0, indicatorPos);
            }
            return input;
        }

        private string[] SplitValueString(string input) {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            int splitPos = input.IndexOf(Settings.Seperator);
            if (splitPos < 0)
                throw new FormatException(string.Format(Resources.SerializationExceptionStrings.PropertyMissingSeparator, Settings.Seperator));
            return new[] { input.Substring(0, splitPos), input.Substring(splitPos + 1) };

        }
        #endregion

        #region IDisposable Implementation Members

        protected override void Dispose(bool disposing) {
            if (IsDisposed)
                return;
            if (disposing) {
                Reader.Dispose();
            }
            base.Dispose(disposing);
            IsDisposed = true;
        }

        #endregion
    }
}
