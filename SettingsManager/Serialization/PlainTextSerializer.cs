using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SettingsManager.Serialization.Cryptography;
using SettingsManager.Serialization.Cryptography.Helpers;
using SettingsManager.Serialization.Helpers;

namespace SettingsManager.Serialization {

    /// <summary>
    /// Serializes and deserializes objects into and from plain text documents.
    /// The <see cref="PlainTextSerializer"/> enables you to control how objects are encoded into plain text.
    /// </summary>
    public class PlainTextSerializer {

        private EncoderBase _encoder;
        private DecoderBase _decoder;

        /// <summary>
        /// Gets the <see cref="EncoderBase"/> used to encode object field and property values.
        /// </summary>
        protected virtual EncoderBase Encoder => _encoder ?? (_encoder = new PlainTextEncoder());

        /// <summary>
        /// Gets the <see cref="DecoderBase"/> used to decode string values into objects.
        /// </summary>
        protected virtual DecoderBase Decoder => _decoder ?? (_decoder = new PlainTextDecoder());

        /// <summary>
        /// Provides data for the <see cref="UnknownElement"/> event.
        /// </summary>
        public delegate void UnknownElementEventHandler(ReadValueObject obj);
        
        /// <summary>
        /// Occurs when a unknown element was found in the deserialized data.
        /// </summary>
        public event UnknownElementEventHandler UnknownElement;


        #region Public Methods
        /// <summary>
        /// Serializes the specified <see cref="object"/> to the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream"><see cref="Stream"/> that the the <see cref="PlainTextSerializer"/> should write to.</param>
        /// <param name="obj"><see cref="object"/> that should be serialized.</param>
        /// <exception cref="ArgumentNullException"/><exception cref="TypeAccessException">Occurs when the Type of the object passed to the method is not public.</exception><exception cref="InvalidOperationException">Occurs when the object passed to the method : Is not public, Is not a class, Does not have a parameterless constructor, contains complex data structures, contains field/properties with duplicate names (attribute included).</exception><exception cref="MemberAccessException">Occures when a property does not have a public set method.</exception>
        public void Serialize(Stream stream, object obj) {
            Serialize(new PlainTextWriter(stream), obj);
        }

        /// <summary>
        /// Serializes the specified object to the specified <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="writer"><see cref="TextWriter"/> that the the <see cref="PlainTextSerializer"/> should write to.</param>
        /// <param name="obj"><see cref="object"/> that should be serialized.</param>
        /// <exception cref="ArgumentNullException"/><exception cref="TypeAccessException">Occurs when the Type of the object passed to the method is not public.</exception><exception cref="InvalidOperationException">Occurs when the object passed to the method : Is not public, Is not a class, Does not have a parameterless constructor, contains complex data structures, contains field/properties with duplicate names (attribute included).</exception><exception cref="MemberAccessException">Occures when a property does not have a public set method.</exception>
        public void Serialize(TextWriter writer, object obj) {
            Serialize(new PlainTextWriter(writer), obj);
        }

        /// <summary>
        /// Serializes the specified <see cref="object"/> to the specified <see cref="PlainTextWriter"/>.
        /// </summary>
        /// <param name="writer"><see cref="PlainTextWriter"/> that the <see cref="PlainTextSerializer"/> should write to.</param>
        /// <param name="obj"><see cref="object"/> that should be serialized.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="TypeAccessException">Occurs when the Type of the object passed to the method is not public.</exception>
        /// <exception cref="InvalidOperationException">Occurs when the object passed to the method : Is not public, Is not a class, Does not have a parameterless constructor, contains complex data structures, contains field/properties with duplicate names (attribute included).</exception>
        /// <exception cref="MemberAccessException">Occures when a property does not have a public set method.</exception>
        public void Serialize(PlainTextWriter writer, object obj) {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            Type objType = obj.GetType();
            if (!objType.IsPublic)
                throw new TypeAccessException(string.Format(Resources.SerializationExceptionStrings.TypeNotAccessible,
                    objType.FullName));
            if (!objType.IsClass)
                throw new InvalidOperationException(Resources.SerializationExceptionStrings.InvalidSerializationType);
            if (objType.GetConstructor(Type.EmptyTypes) == null)
                throw new InvalidOperationException(
                    string.Format(Resources.SerializationExceptionStrings.TypeHasNoParameterlessConstructor, objType));

            var primitives = new List<SerializedObject>();
            object defaultValues = Activator.CreateInstance(objType);

            foreach (FieldInfo field in objType.GetFields().Where(x => x.IsPublic && !IsIgnored(x))) {
                if (field.FieldType.IsPrimitive || field.FieldType == typeof (string))
                    primitives.Add(SerializePrimitive(field, field.GetValue(obj), field.GetValue(defaultValues)));
                else
                    throw new InvalidOperationException(Resources.SerializationExceptionStrings.ComplexDataStructure);
            }
            foreach (PropertyInfo property in objType.GetProperties().Where(x => x.GetMethod.IsPublic && !IsIgnored(x))) {
                if (!property.SetMethod.IsPublic)
                    throw new MemberAccessException(string.Format(
                        Resources.SerializationExceptionStrings.NoPublicSetter, property.Name));
                if (property.PropertyType.IsPrimitive || property.PropertyType == typeof (string))
                    primitives.Add(SerializePrimitive(property, property.GetValue(obj), property.GetValue(defaultValues)));
                else
                    throw new InvalidOperationException(Resources.SerializationExceptionStrings.ComplexDataStructure);
            }
            string[] duplicates =
                primitives.GroupBy(x => x.Name).Where(c => c.Count() > 1).Select(s => s.Key).ToArray();
            if (duplicates.Length > 0)
                throw new InvalidOperationException(string.Format(
                    Resources.SerializationExceptionStrings.DuplicateName, duplicates.First()));

            writer.WriteHeader(
                "This settings file was created using the 'SettingsManager' library",
                "You can find the latest version at : http://daniel-molenaar.com/", //TODO: Latest version
                "",
                $"Encoding = {writer.Settings.Encoding.BodyName}",
                $"Separator = \"{writer.Settings.Separator}\""
                );

            //TODO: Only write optional if the obj.value == default.value
            // Use Type.GetValue(field_name, null) == Type.Getvalue(field_name, obj);
            foreach (SerializedObject primitive in primitives.OrderByDescending(x => x.Priority)) {
                writer.WriteProperty(primitive.Name, primitive.SerializedValue, primitive.Optional, primitive.NewLine);
            }


        }

        /// <summary>
        /// Deserializes the specified <see cref="Stream"/> to the specified type.
        /// </summary>
        /// <typeparam name="T">Type of the deserialized <see cref="object"/>.</typeparam>
        /// <param name="stream"><see cref="Stream"/> that contains the <see cref="object"/> to be deserialized.</param>
        /// <returns>Returns an instance of <see cref="T"/> loaded from the <see cref="Stream"/>.</returns>
        /// <exception cref = "ArgumentNullException"/><exception cref="FormatException">Occurs when a property did not have the correct format.</exception>
        public T Deserialize<T>(Stream stream) {
            return Deserialize<T>(new PlainTextReader(stream));
        }

        /// <summary>
        /// Deserializes the specified <see cref="TextReader"/> to the specified type.
        /// </summary>
        /// <typeparam name="T">Type of the deserialized <see cref="object"/>.</typeparam>
        /// <param name="reader"><see cref="TextReader"/> that contains the <see cref="object"/> to be deserialized.</param>
        /// <returns>Returns an instance of <see cref="T"/> loaded from the <see cref="TextReader"/>.</returns>
        /// <exception cref = "ArgumentNullException"/><exception cref="FormatException">Occurs when a property did not have the correct format.</exception>
        public T Deserialize<T>(TextReader reader) {
            return Deserialize<T>(new PlainTextReader(reader));
        }

        /// <summary>
        /// Deserializes the specified <see cref="PlainTextReader"/> to the specified type.
        /// </summary>
        /// <typeparam name="T">Type of the deserialized <see cref="object"/>.</typeparam>
        /// <param name="reader"><see cref="PlainTextReader"/> that contains the object to be deserialized.</param>
        /// <returns>Returns an instance of <see cref="T"/> loaded from the <see cref="PlainTextReader"/>.</returns>
        /// <exception cref = "ArgumentNullException"/><exception cref="FormatException">Occurs when a property did not have the correct format.</exception>
        public T Deserialize<T>(PlainTextReader reader) {
            Type target = typeof (T);
            return (T)Deserialize(reader, target);
        }

        /// <summary>
        /// Deserializes the specified <see cref="Stream"/> to the specified type.
        /// </summary>
        /// <param name="stream"><see cref="Stream"/> thatcontains the <see cref="object"/> to be deserialized.</param>
        /// <param name="type"><see cref="Type"/> of the deserialized <see cref="object"/>.</param>
        /// <returns>Returns an instance of the specified <see cref="Type"/> loaded from the <see cref="Stream"/>.</returns>
        /// <exception cref = "ArgumentNullException"/><exception cref="FormatException">Occurs when a property did not have the correct format.</exception>
        public object Deserialize(Stream stream, Type type) {
            return Deserialize(new PlainTextReader(stream), type);
        }

        /// <summary>
        /// Deserializes the specified <see cref="TextReader"/> to the specified type.
        /// </summary>
        /// <param name="reader"><see cref="TextReader"/> thatcontains the <see cref="object"/> to be deserialized.</param>
        /// <param name="type"><see cref="Type"/> of the deserialized <see cref="object"/>.</param>
        /// <returns>Returns an instance of the specified <see cref="Type"/> loaded from the <see cref="TextReader"/>.</returns>
        /// <exception cref = "ArgumentNullException"/><exception cref="FormatException">Occurs when a property did not have the correct format.</exception>
        public object Deserialize(TextReader reader, Type type) {
            return Deserialize(new PlainTextReader(reader), type);
        }

        /// <summary>
        /// Deserializes the specified <see cref="PlainTextReader"/> to the specified type.
        /// </summary>
        /// <param name="reader"><see cref="PlainTextReader"/> thatcontains the <see cref="object"/> to be deserialized.</param>
        /// <param name="type"><see cref="Type"/> of the deserialized <see cref="object"/>.</param>
        /// <returns>Returns an instance of the specified <see cref="Type"/> loaded from the <see cref="PlainTextReader"/>.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException">Occurs when a property did not have the correct format.</exception>
        public object Deserialize(PlainTextReader reader, Type type) {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            object instance = Activator.CreateInstance(type);

            try {
                ReadValueObject readItem;
                while ((readItem = reader.ReadNextValue()) != null) {
                    if (string.IsNullOrWhiteSpace(readItem.Name))
                        throw new FormatException(Resources.SerializationExceptionStrings.PropertyNameEmpty);
                    if (string.IsNullOrWhiteSpace(readItem.Value))
                        throw new FormatException(
                            string.Format(Resources.SerializationExceptionStrings.PropertyValueEmpty, readItem.Name));

                    PropertyInfo property = GetProperty(readItem.Name, type);
                    if (property != null) {
                        property.SetValue(instance, Decoder.DecodeUnknown(readItem.Value, property.PropertyType));
                        continue;
                    }
                    FieldInfo field = GetField(readItem.Name, type);
                    if (field != null) {
                        field.SetValue(instance, Decoder.DecodeUnknown(readItem.Value, field.FieldType));
                        continue;
                    }
                    UnknownElement?.Invoke(readItem);
                }
            }
            catch (FormatException ex) {
                throw new FormatException(string.Format(Resources.SerializationExceptionStrings.InvalidSettingFormat,
                    reader.Position) + Environment.NewLine + ex.Message, ex);
            }
            return instance;
        }
        #endregion

        #region Private Methods

        private SerializedObject SerializePrimitive(MemberInfo member, object value, object defaultValue) {

            SerializedObject obj = new SerializedObject() {
                Type = value?.GetType(),
                Name = member.Name,
                SerializedValue = Encoder.EncodeUnknown(value),
                Priority = 0
            };

            object[] attributes = member.GetCustomAttributes(false);
            if (attributes.Length <= 0)
                return obj;

            foreach (object attribute in attributes) {
                if (attribute is PlainTextIgnoreAttribute)
                    return null;
                else if (attribute is PlainTextElementAttribute) {
                    var attr = (PlainTextElementAttribute) attribute;

                    if (attr.Name != null)
                        obj.Name = attr.Name;
                    obj.Priority = attr.Priority;
                    obj.Optional = attr.Optional;
                    if (value != null && obj.Optional && !value.Equals(defaultValue))
                        obj.Optional = false;
                    obj.NewLine = attr.OnNewLine;
                }
            }
            return obj;
        }

        private static bool IsIgnored(MemberInfo member) {
            return member.GetCustomAttribute(typeof (PlainTextIgnoreAttribute)) != null;
        }

        private static PropertyInfo GetProperty(string name, Type type) {
            return
                type.GetProperties()
                    .Where(x => x.SetMethod.IsPublic && !IsIgnored(x))
                    .FirstOrDefault(property => GetMemberName(property) == name);
        }

        private static FieldInfo GetField(string name, Type type) {
            return
                type.GetFields()
                    .Where(x => x.IsPublic && !IsIgnored(x))
                    .FirstOrDefault(field => GetMemberName(field) == name);
        }

        private static string GetMemberName(MemberInfo member) {
            PlainTextElementAttribute attr =
                (PlainTextElementAttribute) member.GetCustomAttribute(typeof (PlainTextElementAttribute));
            return attr?.Name ?? member.Name;
        }
        #endregion
    }

}
