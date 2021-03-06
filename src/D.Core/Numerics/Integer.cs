﻿using System;
using System.Runtime.InteropServices;

namespace E
{
    [StructLayout(LayoutKind.Sequential, Size = 8)]
    public readonly struct Integer : IObject, INumber
    {
        public Integer(long value)
        {
            Value = value;
        }

        public long Value { get; }

        public int BitCount => 64;

        readonly ObjectType IObject.Kind => ObjectType.Int64;

        public static implicit operator int(Integer value) => (int)value.Value;

        public static implicit operator long(Integer value) => value.Value;

        #region INumeric

        readonly T INumber.As<T>() => (T)Convert.ChangeType(Value, typeof(T));

        readonly double INumber.Real => Value;

        #endregion

        public readonly override string ToString() => Value.ToString();
    }
}

// The LLVM language specifies integer types as iN, where N is the bit-width of the integer, and ranges from 1 to 2^23-1

// no distiction between signed and unsigned
