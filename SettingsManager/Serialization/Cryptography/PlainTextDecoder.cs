using System;
using System.Globalization;
using System.Threading;
using SettingsManager.Serialization.Cryptography.Helpers;

namespace SettingsManager.Serialization.Cryptography {

    /// <summary>
    /// Provides methods for Decoding PlainText strings into objects.
    /// </summary>
    public class PlainTextDecoder : DecoderBase {

        /// <summary>
        /// Gets or sets the string value that indicates a property to be null.
        /// </summary>
        public override string NullIndicator { get; protected set; } = "Null";

        /// <summary>
        /// Decodes an encoded <see cref="string"/> containing a primitive <see cref="object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="string"/> to be decoded.</param>
        /// <returns>The <see cref="object"/> containing the decoded data.</returns>
        /// <exception cref="FormatException">Occurs when the encoded string was not in a correct convertible format.</exception>
        /// <exception cref="OverflowException">Occurs when an object does not fit in the target type.</exception>
        public override T DecodePrimitive<T>(string obj) {
            CultureInfo tempInfo = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            T decoded = (T)Convert.ChangeType(obj, typeof (T));
            Thread.CurrentThread.CurrentCulture = tempInfo;
            return decoded;
        }

        /// <summary>
        /// Decodes an encoded <see cref="string"/> into an <see cref="object"/>.
        /// </summary>
        /// <param name="obj"><see cref="string"/> containing the <see cref="object"/> data.</param>
        /// <returns>An <see cref="object"/> containing the encoded data.</returns>
        public override object DecodeObject(string obj) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Decodes an encoded <see cref="string"/> into a <see cref="char"/>.
        /// </summary>
        /// <param name="obj">The <see cref="string"/> to be decoded.</param>
        /// <returns>The decoded <see cref="char"/>.</returns>
        /// <exception cref="FormatException">Occurs when the encoded string could not be decoded by the decoder or if the decoded value is not of the type char.</exception>
        public override char DecodeChar(string obj) {
            if (obj.Length < 2 || obj[0] != '\'' || obj[obj.Length - 1] != '\'')
                throw new FormatException(string.Format(Resources.SerializationExceptionStrings.InvalidObjectString, obj, typeof(char).Name));

            char value;
            if (!char.TryParse(obj.Substring(1, obj.Length - 2), out value))
                throw new FormatException(string.Format(Resources.SerializationExceptionStrings.InvalidObjectString, obj, typeof(char).Name));
            return value;
        }

        /// <summary>
        /// Decodes an encoded <see cref="string"/> into a <see cref="string"/>.
        /// </summary>
        /// <param name="obj">The <see cref="string"/> to be decoded.</param>
        /// <returns>The decoded <see cref="string"/>.</returns>
        /// <exception cref="FormatException">Occurs when the encoded string could not be decoded by the decoder.</exception>
        public override string DecodeString(string obj) {
            if (obj.Length < 2 || obj[0] != '"' || obj[obj.Length - 1] != '"')
                throw new FormatException(string.Format(Resources.SerializationExceptionStrings.InvalidObjectString, obj, typeof(string).Name));
            return obj.Substring(1, obj.Length - 2);
        }

    }
}
