using System;
using System.Buffers.Binary;
using System.Globalization;
using Echo.Core;
using Echo.Core.Values;

namespace Echo.Concrete.Values.ValueType
{
    /// <summary>
    /// Represents a fully known concrete 64 bit floating point numerical value.
    /// </summary>
    public class Float64Value : FloatValue
    {
        /// <summary>
        /// Wraps a 64 bit floating point number into an instance of <see cref="Float64Value"/>.
        /// </summary>
        /// <param name="value">The 64 bit floating point number to wrap.</param>
        /// <returns>The concrete 64 bit floating point numerical value.</returns>
        public static implicit operator Float64Value(double value)
        {
            return new Float64Value(value);
        }

        /// <summary>
        /// Creates a new fully known concrete 64 bit floating point numerical value.
        /// </summary>
        /// <param name="value">The raw 64 bit value.</param>
        public Float64Value(double value)
        {
            F64 = value;
        }

        /// <summary>
        /// Gets or sets the raw floating point value.
        /// </summary>
        public double F64
        {
            get;
            set;
        }

        /// <inheritdoc />
        public override int Size => sizeof(double);

        /// <inheritdoc />
        public override Trilean Sign
        {
            get => F64 < 0;
            set => throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int ExponentSize => 11;

        /// <inheritdoc />
        public override int SignificandSize => 52;

        /// <inheritdoc />
        public override IValue Copy() => new Float64Value(F64);

        /// <inheritdoc />
        public override unsafe void GetBits(Span<byte> buffer)
        {
            // HACK: .NET standard 2.0 does not provide a method to write floating point numbers to a span.
            
            double value = F64;
            ulong rawBits = *(ulong*) &value;
            BinaryPrimitives.WriteUInt64LittleEndian(buffer, rawBits);
        }

        /// <inheritdoc />
        public override void GetMask(Span<byte> buffer)
        {
            // TODO: support unknown bits in float.
            buffer.Fill(0);
        }

        /// <inheritdoc />
        public override unsafe void SetBits(Span<byte> bits, Span<byte> mask)
        {
            // HACK: .NET standard 2.0 does not provide a method to read floating point numbers from a span.
            // TODO: support unknown bits in float.
            
            ulong rawBits = BinaryPrimitives.ReadUInt64LittleEndian(bits);
            F64 = *(double*) &rawBits;
        }

        /// <inheritdoc />
        public override string ToString() => F64.ToString(CultureInfo.InvariantCulture);

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            
            return obj is Float64Value value && F64.Equals(value.F64);;
        }

        /// <inheritdoc />
        public override int GetHashCode() => F64.GetHashCode();
    }
}