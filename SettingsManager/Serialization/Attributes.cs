using System;

namespace SettingsManager.Serialization {

    /// <summary>
    /// Instructs the <see cref="PlainTextSerializer.Serialize(System.IO.TextWriter, object)"/> method of the <see cref="PlainTextSerializer"/> not to serialize the public field or public read/write property value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property , AllowMultiple = false)]
    public sealed class PlainTextIgnoreAttribute : Attribute { }

    /// <summary>
    /// Indicates that a public field or property represents a plain text element when the <see cref="PlainTextSerializer"/> serializes or deserializes the object that contains it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class PlainTextElementAttribute : Attribute {

        private string _name;

        /// <summary>
        /// Gets or sets a value indicating what the name of the field or property should be represented as in serialized form.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value.Trim(' '); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether assigning the field or property is optional. If set to true the propery or field should have a default value.
        /// </summary>
        public bool Optional { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the field or property should be serialized on a new line.
        /// </summary>
        /// <remarks>
        /// This value is only important when using a <see cref="System.IO.TextWriter"/> and should write an extra empty line above the serialized element.
        /// </remarks>
        public bool OnNewLine { get; set; }

        /// <summary>
        /// Gets or sets a value indicating what the priority of the element is.
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlainTextElementAttribute"/> class, which will be used by the <see cref="PlainTextSerializer"/> when serializing or deserializing the object.
        /// </summary>
        public PlainTextElementAttribute() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlainTextElementAttribute"/> class and specifies the name of the plain text element, which will be used by the <see cref="PlainTextSerializer"/> when serializing or deserializing the object.
        /// </summary>
        /// <param name="name">The name that will be representing the field or property.</param>
        public PlainTextElementAttribute(string name) : this(name, 0) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlainTextElementAttribute"/> class and specifies the priority of the plain text element, which will be used by the <see cref="PlainTextSerializer"/> when serializing or deserializing the object.
        /// </summary>
        /// <param name="priority">The priority of the that the plain text element should have over other elements.</param>
        public PlainTextElementAttribute(int priority) : this(null, priority) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlainTextElementAttribute"/> class and specifies the name and the priority of the plain text element, which will be used by the <see cref="PlainTextSerializer"/> when serializing or deserializing the object.
        /// </summary>
        /// <param name="name">The name that will be representing the field or property.</param>
        /// <param name="priority">The priority of the that the plain text element should have over other elements.</param>
        public PlainTextElementAttribute(string name, int priority) {
            Name = name;
            Priority = priority;
        }


    }

}
