using System;
using System.IO;
using System.Text;
using SettingsManager.Serialization;

namespace SettingsManager {

    /// <summary>
    /// Provides methods for loading and saving plain text based settings files. This class is abstract.
    /// </summary>
    /// <typeparam name="T">The class that is inheriting this class.</typeparam>
    /// <example>
    /// Notably all properties that should be saved are to be public.
    /// <code>
    /// public class SettingsClass : PlainTextSettings&lt;SettingsClass&gt; {
    ///     public int IntegerSetting = 15;
    ///     public int StringSetting = { get; set; }
    /// }
    /// </code>
    /// </example>
    public abstract class PlainTextSettings<T> : Settings<T> where T: PlainTextSettings<T>, new() {

        /// <summary>
        /// Represents the extension that will be used when loading and saving plain text settings. This field is constant.
        /// </summary>
        [PlainTextIgnore]
        public const string Extension = ".txt";

        /// <summary>
        /// Gets or sets a value indicating whether to omit the extension when saving to a settings file.
        /// </summary>
        [PlainTextIgnore]
        public bool OmitExtension { get; set; }

        /// <summary>
        /// Gets a value indicating what the file path of the currently loaded <see cref="PlainTextSettings{T}"/> is.
        /// </summary>
        [PlainTextIgnore]
        public sealed override string SavePath { get; internal set; }


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

        /// <summary>
        /// Loads a plain text settings file using the specified path.
        /// </summary>
        /// <param name="path">The relative or absolute path to the settings file.</param>
        /// <param name="settings">The <see cref="PlainTextReaderSettings"/> that the <see cref="PlainTextReader"/> should use when loading a settings file.</param>
        /// <returns>Returns a new instance of the class <see cref="T"/> with variables loaded from the settings file.</returns>
        public static T Load(string path, PlainTextReaderSettings settings) {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            string textPath = FixPathExtension(path);
            bool omitExtension = false;
            if (!File.Exists(textPath)) {
                textPath = path;
                omitExtension = true;
            }
            if(!File.Exists(textPath))
                throw new FileNotFoundException(string.Format(Resources.SettingsExceptionStrings.SettingsNotFound, textPath), textPath);

            using (FileStream stream = new FileStream(path, FileMode.Open)) {
                using (PlainTextReader reader = PlainTextReader.Create(stream, settings)) {
                    T instance = new PlainTextSerializer().Deserialize<T>(reader);
                    instance.SavePath = path;
                    instance.OmitExtension = omitExtension;
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

        /// <summary>
        /// Serializes the <see cref="T"/> object and saves it to the file it was loaded from.
        /// </summary>
        public override void Save() {
            if (SavePath == null)
                throw new NullReferenceException(Resources.SettingsExceptionStrings.SaveWithoutLoad);
            SaveAs(SavePath, false);
        }

        /// <summary>
        /// Serializes the specified <see cref="T"/> object and saves it to the file it was loaded from.
        /// </summary>
        /// <param name="obj">The object to be serialized and saved.</param>
        public static void Save(T obj) {
            obj.Save();
        }

        /// <summary>
        /// Serializes the object and saves it to the specified path.
        /// </summary>
        /// <param name="savePath">The relative or absolute path where the settings should be saved to.</param>
        /// <param name="overrideInstance">True to use the specified file path for future Save() method calls, otherwise false.</param>
        public override void SaveAs(string savePath, bool overrideInstance = true) {
            if (savePath == null)
                throw new ArgumentNullException(nameof(savePath));

            string textPath = OmitExtension ? savePath : FixPathExtension(savePath);
        
            using (FileStream stream = new FileStream(textPath, FileMode.Create))
                using (PlainTextWriter writer = PlainTextWriter.Create(stream, Settings))
                    new PlainTextSerializer().Serialize(writer, this);

            if (overrideInstance)
                SavePath = savePath;
        }

        /// <summary>
        /// Saves a <see cref="PlainTextSettings{T}"/> object to the specified path.
        /// </summary>
        /// <param name="obj"><see cref="T"/> object to be serialized and saved.</param>
        /// <param name="savePath">Path that the settings should be saved to.</param>
        public static void SaveAs(T obj, string savePath) {
            obj.SaveAs(savePath);
        }
        #endregion

        #region Private Methods
        private static string FixPathExtension(string filePath) {
            return Path.GetExtension(filePath) == Extension ? filePath : filePath + Extension;
        }
        #endregion
    }
}
