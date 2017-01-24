﻿using System.Collections.Generic;
using System.Text;

using Xunit;

namespace D.Parsing.Tests
{
    using static Operator;

    using Expressions;
    using Units;

    public class BinaryExpressionTests : TestBase
    {
        public static IEnumerable<object[]> ComparisonOperators
        {
            get
            {
                yield return new object[] { "==", Equal };
                yield return new object[] { "!=", NotEqual };
                yield return new object[] { ">",  GreaterThan };
                yield return new object[] { ">=", GreaterOrEqual };
                yield return new object[] { "<=", LessOrEqual };
                yield return new object[] { "<",  LessThan };
            }
        }

        public static IEnumerable<object[]> LogicalOperators
        {
            get
            {
                yield return new object[] { "&&", LogicalAnd };
                yield return new object[] { "||", LogicalOr };
            }

        }
        public static IEnumerable<object[]> ArithmeticOperators
        {
            get
            {
                yield return new object[] { "+"  , Addition };
                yield return new object[] { "-"  , Subtraction };
                yield return new object[] { "*"  , Multiplication };
                yield return new object[] { "/"  , Division };
                yield return new object[] { "**" , Power };
                yield return new object[] { "%"  , Remainder };
            }
        }

        public static IEnumerable<object[]> BitwiseOperators
        {
            get
            {
                yield return new object[] { "<<",  LeftShift };
                yield return new object[] { ">>",  SignedRightShift };
                yield return new object[] { ">>>", UnsignedRightShift };
                yield return new object[] { "^",   BitwiseXor };
                yield return new object[] { "&",   BitwiseAnd };
                // yield return new object[] { "|",   BitwiseOr };      --- last one....
            }
        }

        [Fact]
        public void Assign1()
        {
            var assignment = Parse<BinaryExpression>($"a *= 3");

            Assert.Equal("a", (Symbol)assignment.Left);

            Assert.Equal(Assign, assignment.Operator);

            Assert.Equal("a * 3", assignment.Right.ToString());
        }

        [Fact]
        public void ModulusOperator()
        {
            Assert.False(char.IsLetter('%'));

            var expression = Parse<BinaryExpression>("a % 3");

            Assert.Equal("a", (Symbol)expression.Left);

            Assert.Equal(3,   (Integer)expression.Right);

            Assert.Equal(Remainder, expression.Operator);

            Assert.Equal("a % 3", expression.ToString());
        }

        [Theory]
        [MemberData(nameof(ArithmeticOperators))]
        [MemberData(nameof(BitwiseOperators))]
        public void CompoundAssignment(string symbol, Operator op)
        {
            var assignment = Parse<BinaryExpression>($"a {symbol}= b");

            var expression = (BinaryExpression)assignment.Right;

            Assert.Equal("a", (Symbol)assignment.Left);

            Assert.Equal(Assign, assignment.Operator);
      
            Assert.Equal("a", (Symbol)expression.Left);

            Assert.Equal(op, expression.Operator);
        }

        [Theory]
        [MemberData(nameof(ArithmeticOperators))]
        public void ArithmeticOps(string symbol, Operator op)
        {
            Assert.Equal(op, Parse<BinaryExpression>($"a {symbol} b").Operator);
            Assert.Equal(op, Parse<BinaryExpression>($"a{symbol}b").Operator);      // whitespace optional
        }

        [Theory]
        [MemberData(nameof(LogicalOperators))]
        public void LogicalOps(string symbol, Operator op)
        {
            Assert.Equal(op, Parse<BinaryExpression>($"a {symbol} b").Operator);
            Assert.Equal(op, Parse<BinaryExpression>($"a{symbol}b").Operator);      // whitespace optional
        }

        [Theory]
        [MemberData(nameof(ComparisonOperators))]
        public void Comparisions(string symbol, Operator op)
        {
            Assert.Equal(op, Parse<BinaryExpression>($"a {symbol} b").Operator);
        }

        [Theory]
        [MemberData(nameof(BitwiseOperators))]
        public void BitwiseOps(string symbol, Operator op)
        {
            Assert.Equal(op, Parse<BinaryExpression>($"a {symbol} b").Operator);
        }

        [Fact]
        public void G1()
        {
            Assert.Equal("1 + 1",                                 Debug("1 + 1"));
            Assert.Equal("1 + (1 * 3)",                           Debug("1 + 1 * 3"));
            Assert.Equal("1 + (4 ** 3)",                          Debug("1 + 4 ** 3"));
            Assert.Equal("1 + (1 - (2 * (3 / (4 ** (5 % 6)))))",  Debug("1 + 1 - 2 * 3 / 4 ** 5 % 6"));
            Assert.Equal("1 + ((4 ** 3) + 8)",                    Debug("1 + (4 ** 3) + 8"));
            Assert.Equal("(1 + 4) ** (3 + 8)",                    Debug("(1 + 4) ** (3 + 8)"));
            Assert.Equal("((1 + 4) ** (3 + 8)) * 15",             Debug("((1 + 4) ** (3 + 8)) * 15"));
            Assert.Equal("3 + (4 * 5)",                           Debug("3 + 4 * 5"));
            Assert.Equal("2 * (2 * 3)",                           Debug("2 * 2 * 3"));
            Assert.Equal("1 * (2 * (3 * 4))",                     Debug("1 * 2 * 3 * 4"));
            Assert.Equal("1 + ((4 ** 3) + 8)",                    Debug("1 + 4 ** 3 + 8"));
            Assert.Equal("3 + ((4 * 5) + 3)",                     Debug("3 + 4 * 5 + 3"));
        }

        [Fact]
        public void IsOperator()
        {
            Assert.Equal(Is, Parse<BinaryExpression>("a is Integer").Operator);
            Assert.Equal(Is, Parse<BinaryExpression>("b is String").Operator);
        }

        [Fact]
        public void AsOperator()
        {
            Assert.Equal(As, Parse<BinaryExpression>("a as Integer").Operator);
        }

        [Fact]
        public void WithMemberAccess()
        {
            Assert.Equal("(a * b) * c", Debug("(a * b) * c"));
            Assert.Equal("(this.a * b) * c", Debug("(this.a * b) * c"));
        }

        [Fact]
        public void G2()
        {
            Assert.Equal("(1 + (1 + 3)) * 3",       Debug("(1 + 1 + 3) * 3"));
            Assert.Equal("(1 + (1 + 3)) * (3 * 4)", Debug("(1 + 1 + 3) * 3 * 4"));
            Assert.Equal("(1 + (1 + (3 * 5))) * a", Debug("(1 + 1 + 3 * 5) * a"));
            Assert.Equal("(a * b) * c",             Debug("(a * b) * c"));


            Assert.Equal("((this.n24 * (this.n33 * this.n41)) - ((this.n23 * (this.n34 * this.n41)) - ((this.n24 * (this.n31 * this.n43)) + ((this.n21 * (this.n34 * this.n43)) + ((this.n23 * (this.n31 * this.n44)) - (this.n21 * (this.n33 * this.n44))))))) * d", 
                
                Debug("(this.n24 * this.n33 * this.n41 - this.n23 * this.n34 * this.n41 - this.n24 * this.n31 * this.n43 + this.n21 * this.n34 * this.n43 + this.n23 * this.n31 * this.n44 - this.n21 * this.n33 * this.n44) * d"));


        }

        private string Debug(string text)
        {
            return BEWriter.Write(Parse<BinaryExpression>(text));
        }

        [Fact]
        public void Groupings1()
        {
            Assert.True(Parse<BinaryExpression>("(4 ** 3)").Grouped);
            Assert.False(Parse<BinaryExpression>("4 ** 3").Grouped);

            var a = Parse<BinaryExpression>("4 ** (3 * 16)");

            Assert.False(a.Grouped);
            Assert.True(((BinaryExpression)a.Right).Grouped);

            Assert.Equal("4 ** (3 * 16)", a.ToString());
        }


        private string Debug2(string text)
            => BEWriter2.Write(Parse<BinaryExpression>(text));

        [Fact]
        public void PrecedenceTests()
        {
            Assert.True(GreaterThan.Precedence > LogicalAnd.Precedence);

            Assert.True(Multiplication.Precedence > Addition.Precedence);

            Assert.True(Power.Precedence > Addition.Precedence);

            Assert.True(Division.Precedence == Multiplication.Precedence);

            Assert.True(Addition.Precedence == Subtraction.Precedence);
        }

        [Fact]
        public void NestedGrouping()
        {
            var statement = Parse<BinaryExpression>("(a * (b - 1)) + (d * c)");

            Assert.Equal(Kind.MultiplyExpression, statement.Left.Kind);

            Assert.Equal("(a * (b - 1)) + (d * c)", statement.ToString());
        }

        [Fact]
        public void Grouping()
        {
            var statement = Parse<BinaryExpression>("(1 * 5) + 3");

            var left = (BinaryExpression)statement.Left;

            Assert.Equal(1L, (Integer)left.Left);
            Assert.Equal(5L, (Integer)left.Right);

            Assert.Equal(3L, (Integer)statement.Right);
        }

        [Fact]
        public void A()
        {
            var b = Parse<BinaryExpression>("5 * x * y");

            var l = (Integer)b.Left;
            var r = (BinaryExpression)b.Right;

            Assert.Equal(5, l.Value);

            Assert.Equal("x", r.Left.ToString());
            Assert.Equal("y", r.Right.ToString());
            Assert.Equal(Kind.Symbol, r.Left.Kind);
            Assert.Equal(Kind.Symbol, r.Right.Kind);
        }

        [Fact]
        public void Parse3()
        {
            var statement = Parse<BinaryExpression>("1g * 1g * 2g");

            var l = (IUnit)statement.Left;
            var r = (BinaryExpression)statement.Right;

            Assert.Equal("1g", l.ToString());
            Assert.Equal("1g", r.Left.ToString());
            Assert.Equal("2g", r.Right.ToString());
        }

        [Fact]
        public void Grouping3()
        {
            var statement = Parse<BinaryExpression>("(1 * 5) + (3 + 5)");

            Assert.Equal(Kind.MultiplyExpression, statement.Left.Kind);
            Assert.Equal(Kind.AddExpression, statement.Right.Kind);
        }

        [Fact]
        public void Group()
        {
            var b = Parse<BinaryExpression>("3 * (5 + 5)");

            var l = b.Left;
            var r = b.Right;

            Assert.Equal(3L, (Integer)l);

            Assert.Equal(Multiplication, b.Operator);

            var rr = (BinaryExpression)b.Right;

            Assert.Equal(5L,        (Integer)rr.Left);
            Assert.Equal(Addition,  rr.Operator);
            Assert.Equal(5L,        (Integer)rr.Right);
            
            Assert.Equal("3 * (5 + 5)", b.ToString());
        }

        [Fact]
        public void Read4()
        {
            var statement = Parse<BinaryExpression>(@"5 * 10px");

            Assert.Equal(Multiplication, statement.Operator);

            Assert.Equal("5", statement.Left.ToString());

            var right = (IUnit)statement.Right;
            Assert.Equal(10, (int)right.Real);
            Assert.Equal("px", right.Type.Name);
        }

        [Fact]
        public void Read6()
        {
            var statement = Parse<BinaryExpression>(@"(10, 10) * 5kg");

            Assert.Equal(Multiplication, statement.Operator);

            var left = (TupleExpression)statement.Left;
            var right = (IUnit)statement.Right;

            Assert.Equal(5, (int)right.Real);

            Assert.Equal("k", right.Prefix.Name);
            Assert.Equal("g", right.Type.Name);
            Assert.Equal("5kg", right.ToString());
        }

        [Fact]
        public void X7()
        {
            var a = Parse<BinaryExpression>("1kg + 1000g");

            Assert.Equal("1kg", a.Left.ToString());
            Assert.Equal("1000g", a.Right.ToString());
        }
    }


    public class BEWriter
    {
        public static string Write(BinaryExpression be)
        {
            var sb = new StringBuilder();

            WritePair(sb, be);

            return sb.ToString();
        }

        public static void WritePair(StringBuilder sb, BinaryExpression be, int i = 0)
        {
            if (i > 0)
            {
                sb.Append("(");
            }

            if (be.Left is BinaryExpression)
            {
                WritePair(sb, (BinaryExpression)be.Left, i + 1);
            }
            else
            {
                sb.Append(be.Left.ToString());
            }

            sb.Append(" ");
            sb.Append(be.Operator.Name);
            sb.Append(" ");

            if (be.Right is BinaryExpression)
            {
                WritePair(sb, (BinaryExpression)be.Right, i + 1);
            }
            else
            {
                sb.Append(be.Right.ToString());
            }

            if (i > 0)
            {
                sb.Append(")");
            }
        }
    }

    public class BEWriter2
    {
        public static string Write(BinaryExpression be)
        {
            var sb = new StringBuilder();

            WritePair(sb, be);

            return sb.ToString();
        }

        public static void WritePair(StringBuilder sb, BinaryExpression be, int i = 0)
        {
            var a = ((be.Right as BinaryExpression)?.Operator.Precedence ?? 100) < be.Operator.Precedence;

            if (i > 0 && a)
            {
                sb.Append("(");
            }

            if (be.Left is BinaryExpression)
            {
                WritePair(sb, (BinaryExpression)be.Left, i + 1);
            }
            else
            {
                sb.Append(be.Left.ToString());
            }

            sb.Append(" ");
            sb.Append(be.Operator.Name);
            sb.Append(" ");

            if (be.Right is BinaryExpression)
            {
                WritePair(sb, (BinaryExpression)be.Right, i + 1);
            }
            else
            {
                sb.Append(be.Right.ToString());
            }

            if (i > 0 && a)
            {
                sb.Append(")");
            }
        }
    }
}