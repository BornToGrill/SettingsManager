using System;
using System.IO;
using Newtonsoft.Json;

namespace SettingsManager {
    /// <summary>
    /// Provides methods for loading and saving Json based settings files. This class is abstract.
    /// </summary>
    /// <typeparam name="T">The class that is inheriting this class.</typeparam>
    /// <example>
    /// Notably all properties that should be saved are to be public.
    /// <code>
    /// class SettingsClass : JsonSettings&lt;SettingsClass&gt; {
    ///     public int IntegerSetting = 15;
    ///     public int StringSetting { get; set; }
    /// }
    /// </code>
    /// </example>
    public abstract class JsonSettings<T> : Settings<T> where T : JsonSettings<T>, new() {

        /// <summary>
        /// Represents the extension that will be used when loading and saving json settings. This field is constant.
        /// </summary>
        [JsonIgnore]
        public const string Extension = ".json";

        /// <summary>
        /// Gets a value indicating what the file path of the currently loaded <see cref="JsonSettings{T}"/> is.
        /// </summary>
        [JsonIgnore]
        public sealed override string SavePath { get; internal set; }

        #region Public Methods
        /// <summary>
        /// Loads a json settings file using the specified path.
        /// </summary>
        /// <param name="path">The relative or absolute path to the settings file.</param>
        /// <returns>Returns a new instance of the class <see cref="T"/> with variables loaded from the settings file.</returns>
        public static T Load(string path) {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            string jsonPath = FixPathExtension(path);
            if (!File.Exists(jsonPath))
                throw new FileNotFoundException(string.Format(Resources.SettingsExceptionStrings.SettingsNotFound, jsonPath), jsonPath);

            using (FileStream stream = new FileStream(jsonPath, FileMode.Open))
                using (StreamReader reader = new StreamReader(stream))
                    using (JsonTextReader json = new JsonTextReader(reader)) {
                        T instance = new JsonSerializer().Deserialize<T>(json);
                        instance.SavePath = jsonPath;
                        return instance;
                    }
        }

        /// <summary>
        /// Reloads the json settings file by returning a new instance.
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
        /// Serializes the object and saves it to the specified path.
        /// </summary>
        /// <param name="savePath">The relative or absolute path where the settings should be saved to.</param>
        /// <param name="overrideInstance">True to use the specified file path for future Save() method calls, otherwise false.</param>
        public override void SaveAs(string savePath, bool overrideInstance = true) {
            if (savePath == null)
                throw new ArgumentNullException(nameof(savePath));

            string jsonPath = FixPathExtension(savePath);

            using (StreamWriter writer = new StreamWriter(jsonPath))
                new JsonSerializer().Serialize(writer, this);
            if (overrideInstance)
                SavePath = jsonPath;
        }

        /// <summary>
        /// Saves a <see cref="JsonSettings{T}"/> object to the specified path.
        /// </summary>
        /// <param name="obj"><see cref="T"/> object to be serialized and saved.</param>
        /// <param name="savePath">Path that the settings should be saved to.</param>
        public static void Save(T obj, string savePath) {
            if (savePath == null)
                throw new ArgumentNullException(nameof(savePath));

            string jsonPath = FixPathExtension(savePath);
            using (StreamWriter writer = new StreamWriter(jsonPath))
                new JsonSerializer().Serialize(writer, obj);
        }

        #endregion

        #region Private Methods
        private static string FixPathExtension(string filePath) {
            return Path.GetExtension(filePath) == Extension ? filePath : filePath + Extension;
        }
        #endregion
    }
}
