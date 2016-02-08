using System;
using System.Globalization;
using System.Threading;
using SettingsManager.Serialization.Cryptography.Helpers;

namespace SettingsManager.Serialization.Cryptography {
    public class PlainTextEncoder : EncoderBase {

        /// <summary>
        /// Encodes a primitive <see cref="object"/> into an encoded <see cref="string"/>.
        /// </summary>
        /// <param name="obj"><see cref="object"/> to be encoded.</param>
        /// <returns>The <see cref="string"/> containing the encoded data.</returns>
        public override string EncodePrimitive(object obj) {
            CultureInfo tempInfo = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            string encoded = obj.ToString();
            Thread.CurrentThread.CurrentCulture = tempInfo;
            return encoded;
        }

        /// <summary>
        /// Encodes a null value into an encoded <see cref="string"/>.
        /// </summary>
        /// <returns>The <see cref="string"/> containing the null data.</returns>
        public override string EncodeNull() {
            return "Null";
        }

        /// <summary>
        /// Encodes a <see cref="char"/> into an encoded <see cref="string"/>.
        /// </summary>
        /// <param name="obj">The <see cref="char"/> to be encoded.</param>
        /// <returns>A string containing the encoded data.</returns>
        public override string Encode(char obj) {
            return $"'{obj}'";
        }

        /// <summary>
        /// Encodes a <see cref="string"/> into an encoded <see cref="string"/>.
        /// </summary>
        /// <param name="obj">The <see cref="string"/> to be encoded.</param>
        /// <returns>A string containing the encoded data.</returns>
        public override string Encode(string obj) {
            return $"\"{obj}\"";
        }

        /// <summary>
        /// Encodes a <see cref="bool"/> into an encoded <see cref="string"/>.
        /// </summary>
        /// <param name="obj">The <see cref="bool"/> to be encoded.</param>
        /// <returns>A string containing the encoded data.</returns>
        public override string Encode(bool obj) {
            return obj ? "True" : "False";
        }

        /// <summary>
        /// If overriden in a derived class, Encodes an <see cref="object"/> into an encoded <see cref="string"/>.
        /// </summary>
        /// <remarks>
        /// Check for Interfaces!
        /// Check for abstract classes !
        /// </remarks>
        public override string EncodeObject(object obj) {
            throw new NotImplementedException();
        }
    }
}
