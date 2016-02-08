using System;

namespace SettingsManager.Serialization.Cryptography.Helpers {

    /// <summary>
    /// Provides methods for Encoding objects.
    /// </summary>
    public abstract class EncoderBase {
        /// <summary>
        /// If overriden in a derived class, Encodes a primitive <see cref="object"/> into an encoded <see cref="string"/>.
        /// </summary>
        /// <param name="obj"><see cref="object"/> to be encoded.</param>
        /// <returns>The <see cref="string"/> containing the encoded data.</returns>
        public abstract string EncodePrimitive(object obj);

        /// <summary>
        /// If overriden in a derived class, Encodes an <see cref="object"/> into an encoded <see cref="string"/>.
        /// </summary>
        /// <remarks>
        /// Check for Interfaces!
        /// Check for abstract classes !
        /// </remarks>
        public abstract string EncodeObject(object obj);

        /// <summary>
        /// If overriden in a derived class, Encodes a null value into an encoded <see cref="string"/>.
        /// </summary>
        /// <returns>The <see cref="string"/> containing the null data.</returns>
        public abstract string EncodeNull();

        /// <summary>
        /// Encodes an <see cref="object"/> of unknown type into an encoded <see cref="string"/>.
        /// </summary>
        /// <param name="obj">The object to be encoded.</param>
        /// <returns>The <see cref="string"/> containing the encoded data.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Occurs when the type code of the passed object is not handled.</exception>
        public virtual string EncodeUnknown(object obj) {

            if (obj == null)
                return EncodeNull();
            if (obj is TimeSpan)
                return Encode((TimeSpan)obj);

            switch (Type.GetTypeCode(obj.GetType())) {
                case TypeCode.Byte:
                    return Encode((string) obj);
                case TypeCode.SByte:
                    return Encode((sbyte) obj);
                case TypeCode.Int16:
                    return Encode((short) obj);
                case TypeCode.UInt16:
                    return Encode((ushort) obj);
                case TypeCode.Int32:
                    return Encode((int) obj);
                case TypeCode.UInt32:
                    return Encode((uint) obj);
                case TypeCode.Int64:
                    return Encode((long) obj);
                case TypeCode.UInt64:
                    return Encode((ulong) obj);
                case TypeCode.Single:
                    return Encode((float) obj);
                case TypeCode.Double:
                    return Encode((double) obj);
                case TypeCode.Decimal:
                    return Encode((decimal) obj);
                case TypeCode.Char:
                    return Encode((char)obj);
                case TypeCode.String:
                    return Encode((string) obj);
                case TypeCode.Boolean:
                    return Encode((bool)obj);
                case TypeCode.Object:
                    return EncodeObject(obj);
                case TypeCode.DateTime:
                    return Encode((DateTime)obj);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Encodes a <see cref="byte"/> into an encoded <see cref="string"/>.
        /// </summary>
        /// <param name="obj">The <see cref="byte"/> to be encoded.</param>
        /// <returns>A string containing the encoded data.</returns>
        public virtual string Encode(byte obj) {
            return EncodePrimitive(obj);
        }

        /// <summary>
        /// Encodes a <see cref="sbyte"/> into an encoded <see cref="string"/>.
        /// </summary>
        /// <param name="obj">The <see cref="sbyte"/> to be encoded.</param>
        /// <returns>A string containing the encoded data.</returns>
        public virtual string Encode(sbyte obj) {
            return EncodePrimitive(obj);
        }

        /// <summary>
        /// Encodes a <see cref="short"/> into an encoded <see cref="string"/>.
        /// </summary>
        /// <param name="obj">The <see cref="short"/> to be encoded.</param>
        /// <returns>A string containing the encoded data.</returns>
        public virtual string Encode(short obj) {
            return EncodePrimitive(obj);
        }

        /// <summary>
        /// Encodes an <see cref="ushort"/> into an encoded <see cref="string"/>.
        /// </summary>
        /// <param name="obj">The <see cref="ushort"/> to be encoded.</param>
        /// <returns>A string containing the encoded data.</returns>
        public virtual string Encode(ushort obj) {
            return EncodePrimitive(obj);
        }

        /// <summary>
        /// Encodes an <see cref="int"/> into an encoded <see cref="string"/>.
        /// </summary>
        /// <param name="obj">The <see cref="int"/> to be encoded.</param>
        /// <returns>A string containing the encoded data.</returns>
        public virtual string Encode(int obj) {
            return EncodePrimitive(obj);
        }

        /// <summary>
        /// Encodes an <see cref="uint"/> into an encoded <see cref="string"/>.
        /// </summary>
        /// <param name="obj">The <see cref="uint"/> to be encoded.</param>
        /// <returns>A string containing the encoded data.</returns>
        public virtual string Encode(uint obj) {
            return EncodePrimitive(obj);
        }

        /// <summary>
        /// Encodes a <see cref="long"/> into an encoded <see cref="string"/>.
        /// </summary>
        /// <param name="obj">The <see cref="long"/> to be encoded.</param>
        /// <returns>A string containing the encoded data.</returns>
        public virtual string Encode(long obj) {
            return EncodePrimitive(obj);
        }

        /// <summary>
        /// Encodes an <see cref="ulong"/> into an encoded <see cref="string"/>.
        /// </summary>
        /// <param name="obj">The <see cref="ulong"/> to be encoded.</param>
        /// <returns>A string containing the encoded data.</returns>
        public virtual string Encode(ulong obj) {
            return EncodePrimitive(obj);
        }

        /// <summary>
        /// Encodes a <see cref="float"/> into an encoded <see cref="string"/>.
        /// </summary>
        /// <param name="obj">The <see cref="float"/> to be encoded.</param>
        /// <returns>A string containing the encoded data.</returns>
        public virtual string Encode(float obj) {
            return EncodePrimitive(obj);
        }

        /// <summary>
        /// Encodes a <see cref="double"/> into an encoded <see cref="string"/>.
        /// </summary>
        /// <param name="obj">The <see cref="double"/> to be encoded.</param>
        /// <returns>A string containing the encoded data.</returns>
        public virtual string Encode(double obj) {
            return EncodePrimitive(obj);
        }

        /// <summary>
        /// Encodes a <see cref="decimal"/> into an encoded <see cref="string"/>.
        /// </summary>
        /// <param name="obj">The <see cref="decimal"/> to be encoded.</param>
        /// <returns>A string containing the encoded data.</returns>
        public virtual string Encode(decimal obj) {
            return EncodePrimitive(obj);
        }

        /// <summary>
        /// Encodes a <see cref="bool"/> into an encoded <see cref="string"/>.
        /// </summary>
        /// <param name="obj">The <see cref="bool"/> to be encoded.</param>
        /// <returns>A string containing the encoded data.</returns>
        public virtual string Encode(bool obj) {
            return EncodePrimitive(obj);
        }

        /// <summary>
        /// Encodes a <see cref="char"/> into an encoded <see cref="string"/>.
        /// </summary>
        /// <param name="obj">The <see cref="char"/> to be encoded.</param>
        /// <returns>A string containing the encoded data.</returns>
        public virtual string Encode(char obj) {
            return EncodePrimitive(obj);
        }

        /// <summary>
        /// Encodes a <see cref="string"/> into an encoded <see cref="string"/>.
        /// </summary>
        /// <param name="obj">The <see cref="string"/> to be encoded.</param>
        /// <returns>A string containing the encoded data.</returns>
        public virtual string Encode(string obj) {
            return EncodePrimitive(obj);
        }

        /// <summary>
        /// Encodes a <see cref="DateTime"/> object into an encoded <see cref="string"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DateTime"/> to be encoded.</param>
        /// <returns>A string containing the encoded data.</returns>
        public virtual string Encode(DateTime obj) {
            return EncodePrimitive(obj);
        }

        /// <summary>
        /// Encodes a <see cref="DateTimeOffset"/> object into an encoded <see cref="string"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DateTimeOffset"/> to be encoded.</param>
        /// <returns>A string containing the encoded data.</returns>
        public virtual string Encode(DateTimeOffset obj) {
            return EncodePrimitive(obj);
        }

        /// <summary>
        /// Encodes a <see cref="TimeSpan"/> object into an encoded <see cref="string"/>.
        /// </summary>
        /// <param name="obj">The <see cref="TimeSpan"/> to be encoded.</param>
        /// <returns>A string containing the encoded data.</returns>
        public virtual string Encode(TimeSpan obj) {
            return EncodePrimitive(obj);
        }
    }
}
