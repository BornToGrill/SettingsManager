using System;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace SettingsManager {
    /// <summary>
    /// Provides methods for loading and saving binary based settings files. This class is abstract.
    /// </summary>
    /// <typeparam name="T">The class that is inheriting this class.</typeparam>
    /// <example>
    /// Notably all properties that should be saved are to be public and the settings class should be marked as Serializable.
    /// <code>
    /// [Serializable]
    /// public class SettingsClass : BinarySettings&lt;SettingsClass&gt; {
    ///     public int IntegerSetting = 15;
    ///     public int StringSetting { get; set; }
    /// }
    /// </code>
    /// </example>
    [Serializable]
    public abstract class BinarySettings<T> : Settings<T> where T : BinarySettings<T>, new() {

        /// <summary>
        /// Represents the extension that will be used when loading and saving binary settings. This field is constant.
        /// </summary>
        [NonSerialized]
        public const string Extension = ".dat";

        /// <summary>
        /// Gets or sets a value indicating whether to omit the extension when saving to a settings file.
        /// </summary>
        public bool OmitExtension { get; set; }

        /// <summary>
        /// Gets a value indicating what the file path of the currently loaded <see cref="BinarySettings{T}"/> is.
        /// </summary>
        public sealed override string SavePath { get; internal set; }


        #region Public Methods
        /// <summary>
        /// Loads a binary settings file using the specified path.
        /// </summary>
        /// <param name="path">The relative or absolute path to the settings file.</param>
        /// <returns>Returns a new instance of the class <see cref="T"/> with variables loaded from the settings file.</returns>
        public static T Load(string path) {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            string binaryPath = FixPathExtension(path);
            bool omitExtension = false;
            if (!File.Exists(binaryPath)) {
                binaryPath = path;
                omitExtension = true;
            }
            if (!File.Exists(binaryPath))
                throw new FileNotFoundException(string.Format(Resources.SettingsExceptionStrings.SettingsNotFound, binaryPath), binaryPath);

            using (FileStream stream = new FileStream(binaryPath, FileMode.Open)) {
                BinaryFormatter format = new BinaryFormatter() {
                    AssemblyFormat = FormatterAssemblyStyle.Simple
                };

                T instance = (T)format.Deserialize(stream);
                instance.SavePath = binaryPath;
                instance.OmitExtension = omitExtension;
                return instance;
            }
        }

        /// <summary>
        /// Reloads the binary settings file by returning a new instance.
        /// </summary>
        /// <returns>Returns a new instance of the class <see cref="T"/> with variables loaded from the settings file.</returns>
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

            string binaryPath = OmitExtension ? savePath : FixPathExtension(savePath);

            using (FileStream stream = new FileStream(binaryPath, FileMode.Create)) {
                BinaryFormatter formatter = new BinaryFormatter() {
                    AssemblyFormat = FormatterAssemblyStyle.Simple
                };
                formatter.Serialize(stream, this);
            }

            if (overrideInstance)
                SavePath = binaryPath;
        }

        /// <summary>
        /// Saves a <see cref="JsonSettings{T}"/> object to the specified path.
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
