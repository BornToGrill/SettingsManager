using System;

namespace SettingsManager.Serialization.Cryptography.Helpers {

    /// <summary>
    /// Provides methods for Decoding strings.
    /// </summary>
    public abstract class DecoderBase {

        /// <summary>
        /// If overriden in a derived class, Sets or gets the string value indicating what an encoded null string is.
        /// </summary>
        public abstract string NullIndicator { get; protected set; }

        /// <summary>
        /// If overriden in a derived class, Decodes an encoded <see cref="string"/> containing a primitive <see cref="object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="string"/> to be decoded.</param>
        /// <returns>The <see cref="object"/> containing the decoded data.</returns>
        public abstract T DecodePrimitive<T>(string obj);

        /// <summary>
        /// If overriden in a derived class, Decodes an encoded <see cref="string"/> into an <see cref="object"/>.
        /// </summary>
        public abstract object DecodeObject(string obj);

        /// <summary>
        /// Returns null.
        /// </summary>
        public virtual object DecodeNull() {
            return null;
        }

        /// <summary>
        /// Decodes an encoded <see cref="string"/> into an <see cref="object"/> of unknown type.
        /// </summary>
        /// <param name="obj">The string to be decoded.</param>
        /// <param name="type">The type of the encoded object.</param>
        /// <returns>The <see cref="object"/> containing the decoded data.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="NullReferenceException">Occurs when the NullIndicator has not been set in a derived class.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Occurs when the type passed to the method can not be decoded.</exception>
        public virtual object DecodeUnknown(string obj, Type type) {

            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (type == typeof(TimeSpan))
                return DecodeTimeSpan(obj);
            if (obj == NullIndicator)
                return DecodeNull();

            switch (Type.GetTypeCode(type)) {
                case TypeCode.Byte:
                    return DecodeByte(obj);
                case TypeCode.SByte:
                    return DecodeSByte(obj);
                case TypeCode.Int16:
                    return DecodeInt16(obj);
                case TypeCode.UInt16:
                    return DecodeUInt16(obj);
                case TypeCode.Int32:
                    return DecodeInt32(obj);
                case TypeCode.UInt32:
                    return DecodeUInt32(obj);
                case TypeCode.Int64:
                    return DecodeInt64(obj);
                case TypeCode.UInt64:
                    return DecodeUInt64(obj);
                case TypeCode.Single:
                    return DecodeSingle(obj);
                case TypeCode.Double:
                    return DecodeDouble(obj);
                case TypeCode.Decimal:
                    return DecodeDecimal(obj);
                case TypeCode.Char:
                    return DecodeChar(obj);
                case TypeCode.String:
                    return DecodeString(obj);
                case TypeCode.Boolean:
                    return DecodeBoolean(obj);
                case TypeCode.DateTime:
                    return DecodeDateTime(obj);
                case TypeCode.Object:
                    return DecodeObject(obj);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), string.Format(Resources.SerializationExceptionStrings.TypeCouldNotBeDecoded, type.FullName));
            }
        }

        /// <summary>
        /// Decodes an encoded <see cref="string"/> into a <see cref="byte"/>.
        /// </summary>
        /// <param name="obj">The <see cref="string"/> containing the <see cref="byte"/> data.</param>
        /// <returns>The decoded object.</returns>
        public virtual byte DecodeByte(string obj) {
            return DecodePrimitive<byte>(obj);
        }

        /// <summary>
        /// Decodes an encoded <see cref="string"/> into a <see cref="sbyte"/>.
        /// </summary>
        /// <param name="obj">The <see cref="string"/> containing the <see cref="sbyte"/> data.</param>
        /// <returns>The decoded object.</returns>
        public virtual sbyte DecodeSByte(string obj) {
            return DecodePrimitive<sbyte>(obj);
        }

        /// <summary>
        /// Decodes an encoded <see cref="string"/> into a <see cref="short"/>.
        /// </summary>
        /// <param name="obj">The <see cref="string"/> containing the <see cref="short"/> data.</param>
        /// <returns>The decoded object.</returns>
        public virtual short DecodeInt16(string obj) {
            return DecodePrimitive<short>(obj);
        }

        /// <summary>
        /// Decodes an encoded <see cref="string"/> into an <see cref="ushort"/>.
        /// </summary>
        /// <param name="obj">The <see cref="string"/> containing the <see cref="ushort"/> data.</param>
        /// <returns>The decoded object.</returns>
        public virtual ushort DecodeUInt16(string obj) {
            return DecodePrimitive<ushort>(obj);
        }

        /// <summary>
        /// Decodes an encoded <see cref="string"/> into an <see cref="int"/>.
        /// </summary>
        /// <param name="obj">The <see cref="string"/> containing the <see cref="int"/> data.</param>
        /// <returns>The decoded object.</returns>
        public virtual int DecodeInt32(string obj) {
            return DecodePrimitive<int>(obj);
        }

        /// <summary>
        /// Decodes an encoded <see cref="string"/> into an <see cref="uint"/>.
        /// </summary>
        /// <param name="obj">The <see cref="string"/> containing the <see cref="uint"/> data.</param>
        /// <returns>The decoded object.</returns>
        public virtual uint DecodeUInt32(string obj) {
            return DecodePrimitive<uint>(obj);
        }

        /// <summary>
        /// Decodes an encoded <see cref="string"/> into a <see cref="long"/>.
        /// </summary>
        /// <param name="obj">The <see cref="string"/> containing the <see cref="long"/> data.</param>
        /// <returns>The decoded object.</returns>
        public virtual long DecodeInt64(string obj) {
            return DecodePrimitive<long>(obj);
        }

        /// <summary>
        /// Decodes an encoded <see cref="string"/> into an <see cref="ulong"/>.
        /// </summary>
        /// <param name="obj">The <see cref="string"/> containing the <see cref="ulong"/> data.</param>
        /// <returns>The decoded object.</returns>
        public virtual ulong DecodeUInt64(string obj) {
            return DecodePrimitive<ulong>(obj);
        }

        /// <summary>
        /// Decodes an encoded <see cref="string"/> into a <see cref="float"/>.
        /// </summary>
        /// <param name="obj">The <see cref="string"/> containing the <see cref="float"/> data.</param>
        /// <returns>The decoded object.</returns>
        public virtual float DecodeSingle(string obj) {
            return DecodePrimitive<float>(obj);
        }

        /// <summary>
        /// Decodes an encoded <see cref="string"/> into a <see cref="double"/>.
        /// </summary>
        /// <param name="obj">The <see cref="string"/> containing the <see cref="double"/> data.</param>
        /// <returns>The decoded object.</returns>
        public virtual double DecodeDouble(string obj) {
            return DecodePrimitive<double>(obj);
        }

        /// <summary>
        /// Decodes an encoded <see cref="string"/> into a <see cref="decimal"/>.
        /// </summary>
        /// <param name="obj">The <see cref="string"/> containing the <see cref="decimal"/> data.</param>
        /// <returns>The decoded object.</returns>
        public virtual decimal DecodeDecimal(string obj) {
            return DecodePrimitive<decimal>(obj);
        }

        /// <summary>
        /// Decodes an encoded <see cref="string"/> into a <see cref="bool"/>.
        /// </summary>
        /// <param name="obj">The <see cref="string"/> containing the <see cref="bool"/> data.</param>
        /// <returns>The decoded object.</returns>
        public virtual bool DecodeBoolean(string obj) {
            return DecodePrimitive<bool>(obj);
        }

        /// <summary>
        /// Decodes an encoded <see cref="string"/> into a <see cref="char"/>.
        /// </summary>
        /// <param name="obj">The <see cref="string"/> containing the <see cref="char"/> data.</param>
        /// <returns>The decoded object.</returns>
        public virtual char DecodeChar(string obj) {
            return DecodePrimitive<char>(obj);
        }

        /// <summary>
        /// Decodes an encoded <see cref="string"/> into a <see cref="string"/>.
        /// </summary>
        /// <param name="obj">The <see cref="string"/> containing the <see cref="string"/> data.</param>
        /// <returns>The decoded object.</returns>
        public virtual string DecodeString(string obj) {
            return DecodePrimitive<string>(obj);
        }

        /// <summary>
        /// Decodes an encoded <see cref="string"/> into a <see cref="DateTime"/> object.
        /// </summary>
        /// <param name="obj">The <see cref="string"/> containing the <see cref="DateTime"/> data.</param>
        /// <returns>The decoded object.</returns>
        public virtual DateTime DecodeDateTime(string obj) {
            return DecodePrimitive<DateTime>(obj);
        }

        /// <summary>
        /// Decodes an encoded <see cref="string"/> into a <see cref="DateTimeOffset"/> object.
        /// </summary>
        /// <param name="obj">The <see cref="string"/> containing the <see cref="DateTimeOffset"/> data.</param>
        /// <returns>The decoded object.</returns>
        public virtual DateTimeOffset DecodeDateTimeOffset(string obj) {
            return DecodePrimitive<DateTimeOffset>(obj);
        }

        /// <summary>
        /// Decodes an encoded <see cref="string"/> into a <see cref="TimeSpan"/> object.
        /// </summary>
        /// <param name="obj">The <see cref="string"/> containing the <see cref="TimeSpan"/> data.</param>
        /// <returns>The decoded object.</returns>
        public virtual TimeSpan DecodeTimeSpan(string obj) {
            return DecodePrimitive<TimeSpan>(obj);
        }
    }
}
