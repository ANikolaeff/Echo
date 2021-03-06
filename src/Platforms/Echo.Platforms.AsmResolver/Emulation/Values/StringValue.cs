using System;
using System.Text;
using AsmResolver.DotNet.Signatures.Types;
using Echo.Concrete.Values;
using Echo.Concrete.Values.ReferenceType;
using Echo.Concrete.Values.ValueType;
using Echo.Core;
using Echo.Core.Values;

namespace Echo.Platforms.AsmResolver.Emulation.Values
{
    /// <summary>
    /// Represents an unicode string value. 
    /// </summary>
    public class StringValue : IDotNetValue
    {
        private readonly IMemoryAccessValue _contents;

        /// <summary>
        /// Creates a new string value.
        /// </summary>
        /// <param name="stringType">The string type signature.</param>
        /// <param name="contents">The raw contents of the string.</param>
        /// <exception cref="ArgumentException">
        /// Occurs when the memory block referenced by <paramref name="contents"/> is of an invalid size.
        /// </exception>
        public StringValue(TypeSignature stringType, IMemoryAccessValue contents)
        {
            Type = stringType ?? throw new ArgumentNullException(nameof(stringType));
            _contents = contents ?? throw new ArgumentNullException(nameof(contents));
            if (contents.Size % sizeof(char) != 0)
                throw new ArgumentException($"Length of raw string memory must be a multiple of two.");
        }

        /// <inheritdoc />
        public TypeSignature Type
        {
            get;
        }

        /// <summary>
        /// Gets the number of characters stored in the string.
        /// </summary>
        public int Length => _contents.Size / sizeof(char);
        
        /// <inheritdoc />
        public bool IsKnown => true;

        /// <inheritdoc />
        public int Size => _contents.Size;

        /// <inheritdoc />
        public bool IsValueType => true;

        /// <inheritdoc />
        public Trilean IsZero => false;

        /// <inheritdoc />
        public Trilean IsNonZero => true;

        /// <inheritdoc />
        public Trilean IsPositive => true;

        /// <inheritdoc />
        public Trilean IsNegative => false;

        /// <inheritdoc />
        public IValue Copy() => new StringValue(Type, _contents);

        /// <summary>
        /// Gets a single character stored in the string.
        /// </summary>
        /// <param name="index">The character index.</param>
        /// <returns>The character.</returns>
        public Integer16Value GetChar(int index) => _contents.ReadInteger16(index * sizeof(ushort));

        /// <summary>
        /// Gets the string representation of the (partially) known string. 
        /// </summary>
        /// <param name="unknownChar">The character used for indicating an unknown character in the string.</param>
        /// <returns>The string.</returns>
        public string ToString(char unknownChar)
        {
            var builder = new StringBuilder(Length);
            
            for (int i = 0; i < Length; i++)
            {
                var rawCharValue = GetChar(i);
                builder.Append(rawCharValue.IsKnown ? (char) rawCharValue.U16 : unknownChar);
            }

            return builder.ToString();
        }

        /// <inheritdoc />
        public override string ToString() => ToString('?');
    }
}