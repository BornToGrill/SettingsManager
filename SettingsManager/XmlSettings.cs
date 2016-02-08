using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SettingsManager {

    /// <summary>
    /// Provides methods for loading and saving Xml based settings files. This class is abstract.
    /// </summary>
    /// <typeparam name="T">The class that is inheriting this class.</typeparam>
    /// <example>
    /// Notably all properties that should be saved are to be public.
    /// <code>
    /// class SettingsClass : XmlSettings&lt;SettingsClass&gt; {
    ///     public int IntegerSetting = 15;
    ///     public int StringSetting = { get; set; }
    /// }
    /// </code>
    /// </example>
    public abstract class XmlSettings<T> : Settings<T> where T: XmlSettings<T>, new() {
        
        /// <summary>
        /// Represents the extension that will be used when loading and saving xml settings. This field is constant.
        /// </summary>
        [XmlIgnore]
        public const string Extension = ".xml";

        /// <summary>
        /// Gets or sets a value indicating whether the settings file should not contain xmlns namespaces.
        /// </summary>
        [XmlIgnore]
        public bool IgnoreNamespaces { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating what <see cref="System.Xml.XmlWriterSettings"/> the <see cref="XmlWriter"/> should use.
        /// </summary>
        [XmlIgnore]
        public XmlWriterSettings XmlWriterSettings { get; set; }

        /// <summary>
        /// Gets or sets a value indicating what <see cref="System.Xml.XmlReaderSettings"/> the <see cref="XmlReader"/> should use.
        /// </summary>
        [XmlIgnore]
        public XmlReaderSettings XmlReaderSettings { get; set; }

        /// <summary>
        /// Gets a value indicating what the file path of the currently loaded <see cref="XmlSettings{T}"/> is.
        /// </summary>
        [XmlIgnore]
        public sealed override string SavePath { get; internal set; }

        #region Public Methods
        /// <summary>
        /// Loads an xml settings file using the specified path.
        /// </summary>
        /// <param name="path">The relative or absolute path to the settings file.</param>
        /// <returns>Returns a new instance of the class <see cref="T"/> with variables loaded from the settings file.</returns>
        public static T Load(string path) {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            string xmlPath = FixPathExtension(path);

            if (!File.Exists(xmlPath))
                throw new FileNotFoundException(string.Format(Resources.SettingsExceptionStrings.SettingsNotFound, xmlPath), xmlPath);

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (FileStream stream = new FileStream(xmlPath, FileMode.Open)) {
                using (XmlTextReader reader = new XmlTextReader(stream)) {
                    reader.MoveToContent();
                    Encoding encoding = reader.Encoding;
                    T instance = (T)serializer.Deserialize(reader);
                    if (encoding != null)
                        instance.XmlWriterSettings.Encoding = encoding;
                    instance.SavePath = xmlPath;
                    return instance;
                }
            }
        }

        /// <summary>
        /// Reloads the xml settings file by returning a new instance.
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
            string xmlPath = FixPathExtension(savePath);

            using (Stream stream = new FileStream(xmlPath, FileMode.Create)) {
                if (XmlWriterSettings == null)
                    XmlWriterSettings = new XmlWriterSettings() {
                        Encoding = Encoding.UTF8, Indent = true,
                    };
                if (XmlWriterSettings.Encoding.Equals(Encoding.UTF8))
                    XmlWriterSettings.OmitXmlDeclaration = true;

                using (XmlWriter writer = XmlWriter.Create(stream, XmlWriterSettings)) {
                    XmlSerializer serializer = new XmlSerializer(GetType());
                    XmlSerializerNamespaces namespaces = null;
                    if (IgnoreNamespaces) {
                        namespaces = new XmlSerializerNamespaces();
                        namespaces.Add("", "");
                    }
                    serializer.Serialize(writer, this, namespaces);
                }
            }
            if (overrideInstance)
                SavePath = xmlPath;
        }

        #endregion
        #region Private Methods

        private static string FixPathExtension(string fileName) {
            return Path.GetExtension(fileName) == Extension ? fileName : fileName + Extension;
        }
        #endregion
    }
}
