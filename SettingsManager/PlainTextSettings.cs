using System;
using System.IO;
using SettingsManager.Serialization;

namespace SettingsManager {

    /// <summary>
    /// Provides methods for loading and saving plain text based settings files. This class is abstract.
    /// </summary>
    /// <typeparam name="T">The class that is inheriting this class.</typeparam>
    /// <example>
    /// Notably all properties that should be saved are to be public.
    /// <code>
    /// class SettingsClass : PlainTextSettings&lt;SettingsClass&gt; {
    ///     public int IntegerSetting = 15;
    ///     public int StringSetting = { get; set; }
    /// }
    /// </code>
    /// </example>
    public abstract class PlainTextSettings<T> : Settings<T> where T: PlainTextSettings<T>, new() {

        // TODO: Check if reader, writer settings are working.

        /// <summary>
        /// Gets a value indicating what the file path of the currently loaded <see cref="PlainTextSettings{T}"/> is.
        /// </summary>
        [PlainTextIgnore]
        public sealed override string SavePath { get; internal set; }

        //TODO: Change settings when loading.

        /// <summary>
        /// Gets or sets a value indicating what <see cref="PlainTextWriterSettings"/> should be used for serializing the settings.
        /// </summary>
        [PlainTextIgnore]
        public PlainTextWriterSettings Settings { get; set; } = new PlainTextWriterSettings();

        #region Public Methods
        /// <summary>
        /// Loads a plain text settings file using the specified path.
        /// </summary>
        /// <param name="path">The relative or absolute path to the settings file.</param>
        /// <returns>Returns a new instance of the class <see cref="T"/> with variables loaded from the settings file.</returns>
        public static T Load(string path) {
            return Load(path, null);
        }

        public static T Load(string path, PlainTextReaderSettings settings) {
            using (FileStream stream = new FileStream(path, FileMode.Open)) {
                using (PlainTextReader reader = PlainTextReader.Create(stream, settings)) {
                    T instance = new PlainTextSerializer().Deserialize<T>(reader);
                    instance.SavePath = path;
                    return instance;
                }
            }
        }

        /// <summary>
        /// Reloads the plain text settings file returning a new instance.
        /// </summary>
        /// <returns></returns>
        public override T Reload() {
            if (SavePath == null)
                throw new NullReferenceException(Resources.SettingsExceptionStrings.ReloadWithoutLoad);
            return Load(SavePath);
        }

        public override void Save() {
            if (SavePath == null)
                throw new NullReferenceException(Resources.SettingsExceptionStrings.SaveWithoutLoad);
            SaveAs(SavePath, false);
        }

        public override void SaveAs(string savePath, bool overrideInstance = true) {
            if (savePath == null)
                throw new ArgumentNullException(nameof(savePath));

            using (FileStream stream = new FileStream(savePath, FileMode.Create)) {
                using (PlainTextWriter writer = PlainTextWriter.Create(stream, Settings))
                    new PlainTextSerializer().Serialize(writer, this);
            }
            if (overrideInstance)
                SavePath = savePath;
        }
        #endregion
    }
}
