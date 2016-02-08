using System;
using System.IO;

namespace SettingsManager.Serialization.Helpers {
    public abstract class PlainReader : IDisposable {

        /// <summary>
        /// If overriden in a derived class, gets a value indicating which <see cref="TextReader"/> is being used by the <see cref="PlainReader"/>.
        /// </summary>
        protected abstract TextReader Reader { get; }

        /// <summary>
        /// If overriden in a derived class, reads the header from the reader.
        /// </summary>
        /// <returns>An <see cref="Array"/> of type <see cref="string"/> containing the data read from the header.</returns>
        public abstract string[] GetHeader();

        /// <summary>
        /// Reads the next value from the reader.
        /// </summary>
        /// <returns>An instance of <see cref="ReadValueObject"/> containing the read data.</returns>
        public abstract ReadValueObject ReadNextValue();

        /// <summary>
        /// Reads the next comment from the reader.
        /// </summary>
        /// <returns>A <see cref="string"/> containing the comment.</returns>
        public abstract string ReadNextComment();

        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="PlainReader"/> class.
        /// </summary>
        public virtual void Close() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region IDisposable Implementation Members

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="PlainReader"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing) {
            
        }

        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="PlainReader"/> class.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
