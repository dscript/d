﻿using System;

namespace D.Numerics
{
    using Expressions;

    public class Matrix<T> : IObject
        where T : struct, IEquatable<T>, IFormattable
    {
        private MathNet.Numerics.LinearAlgebra.Matrix<T> impl;

        public Matrix(int rows, int columns, T[] elements)
        {
            impl = MathNet.Numerics.LinearAlgebra.Matrix<T>.Build.Dense(rows, columns, elements);
        }

        private Matrix(MathNet.Numerics.LinearAlgebra.Matrix<T> impl)
        {
            this.impl = impl;
        }

        public object this[int x, int y] => impl[x, y];

        #region IArithmetic / Scalars

        public IObject Add(INumber scalar)
            => new Matrix<T>(impl.Add(scalar.As<T>()));

        public IObject Subtract(INumber scalar)
            => new Matrix<T>(impl.Subtract(scalar.As<T>()));

        public IObject Divide(INumber scalar)
            => new Matrix<T>(impl.Divide(scalar.As<T>()));

        public IObject Mutiply(INumber scalar)
            => new Matrix<T>(impl.Multiply(scalar.As<T>()));

        #endregion

        #region IArithmetic / Matrixes

        public Matrix<T> Multiply(Matrix<T> other)
        {
            var result = impl.Multiply(other.impl);

            return new Matrix<T>(result);
        }

        public Matrix<T> Add(Matrix<T> other)
        {
            var result = impl.Add(other.impl);

            return new Matrix<T>(result);
        }

        public Matrix<T> Subtract(Matrix<T> other)
        {
            var result = impl.Subtract(other.impl);

            return new Matrix<T>(result);
        }

        #endregion

        #region Operators

        public static Matrix<T> operator +(Matrix<T> l, T r)
            => new Matrix<T>(l.impl + r);

        public static Matrix<T> operator +(Matrix<T> l, Matrix<T> r)
            => new Matrix<T>(l.impl + r.impl);

        public static Matrix<T> operator +(T l, Matrix<T> r)
           => new Matrix<T>(l + r.impl);

        public static Matrix<T> operator -(Matrix<T> l, Matrix<T> r)
           => new Matrix<T>(l.impl - r.impl);

        public static Matrix<T> operator -(T l, Matrix<T> r)
          => new Matrix<T>(l - r.impl);

        public static Matrix<T> operator -(Matrix<T> l, T r)
        {
            return new Matrix<T>(l.impl - r);
        }

        public static Matrix<T> operator *(Matrix<T> l, Matrix<T> r)
           => new Matrix<T>(l.impl * r.impl);

        public static Matrix<T> operator *(T l, Matrix<T> r)
           => new Matrix<T>(l * r.impl);

        public static Matrix<T> operator *(Matrix<T> l, T r)
            => new Matrix<T>(l.impl * r);

        public static Vector<T> operator *(Matrix<T> l, Vector<T> r)
            => new Vector<T>(l.impl * r.BaseVector);

        public static Matrix<T> operator /(Matrix<T> dividend, T divisor)
            => new Matrix<T>(dividend.impl / divisor);

        public static Matrix<T> operator /(T dividend, Matrix<T> divisor)
        {
            return new Matrix<T>(dividend / divisor.impl);
        }

        public static Matrix<T> operator %(T dividend, Matrix<T> divisor)
           => new Matrix<T>(dividend % divisor.impl);

        public static Matrix<T> operator %(Matrix<T> dividend, Matrix<T> divisor)
            => new Matrix<T>(dividend.impl % divisor.impl);

        public static Matrix<T> operator %(Matrix<T> dividend, T divisor)
            => new Matrix<T>(dividend.impl % divisor);

        #endregion        

        public static Matrix<T> Create(MatrixLiteral expression)
        {
            var elements = new T[expression.Elements.Length];

            for (var i = 0; i < elements.Length; i++)
            {
                var v = (INumber)expression.Elements[i];

                elements[i] = v.As<T>();
            }

            return new Matrix<T>(expression.RowCount, expression.ColumnCount, elements);
        }


        Kind IObject.Kind => Kind.MatrixLiteral;
    }

}