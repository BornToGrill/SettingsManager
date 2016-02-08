using System.Xml.Serialization;

namespace SettingsManager {

    /// <summary>
    /// Provides methods for loading and saving settings files. This class is abstract.
    /// </summary>
    /// <typeparam name="T">The <see cref="T"/> passed to the derived class.</typeparam>
    public abstract class Settings<T> { 

        /// <summary>
        /// If overriden in a derived class, gets a value indicating what the save path of the <see cref="Settings{T}"/> is.
        /// </summary>
        [XmlIgnore]
        public abstract string SavePath { get; internal set; }

        /// <summary>
        /// If overriden in a derived class, reloads the settings into a new instance.
        /// </summary>
        /// <returns>A new instance of <see cref="T"/> with data loaded from a settings file.</returns>
        public abstract T Reload();

        /// <summary>
        /// If overriden in a derived class, saves the current <see cref="Settings{T}"/> to the specified <see cref="SavePath"/>.
        /// </summary>
        public abstract void Save();

        /// <summary>
        /// If overriden in a derived class, saves the current <see cref="Settings{T}"/> to the specified save path.
        /// </summary>
        /// <param name="savePath">The path that the settings should be saved to.</param>
        /// <param name="overrideInstance">true if future <see cref="Save"/> calls should save to the new save path; otherwise false.</param>
        public abstract void SaveAs(string savePath, bool overrideInstance = true);


    }
}
