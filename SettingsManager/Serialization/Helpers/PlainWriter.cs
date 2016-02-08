using System;
using System.IO;

namespace SettingsManager.Serialization.Helpers {
    public abstract class PlainWriter : IDisposable {

        /// <summary>
        /// If overriden in a derived class, gets a value indicating which <see cref="TextWriter"/> is being used by the <see cref="PlainWriter"/>.
        /// </summary>
        protected abstract TextWriter Writer { get; }

        /// <summary>
        /// If overriden in a derived class, writes a header containing all specified strings.
        /// </summary>
        /// <param name="args">The <see cref="string"/> to be written to the header.</param>
        public abstract void WriteHeader(params string[] args);

        /// <summary>
        /// If overriden in a derived class, writes a property to the underlying <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="value">Value of the property.</param>
        /// <param name="optional">Whether the propery is optional or not.</param>
        /// <param name="newLine">Whether the propery should be writen on a new line.</param>
        public abstract void WriteProperty(string name, string value, bool optional = false, bool newLine = false);

        /// <summary>
        /// If overriden in a derived class, writes a comment to the underlying <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="comment"></param>
        public abstract void WriteComment(string comment);

        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="PlainWriter"/> class.
        /// </summary>
        public virtual void Close() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region IDisposable Implementation Members

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="PlainWriter"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing) { }

        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="PlainWriter"/> class.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
