﻿using Xunit;

namespace D.Parsing.Tests
{
    using Expressions;

    public class TernaryExpressionTests : TestBase
    {
        [Fact]
        public void A()
        {
            var ternary = Parse<TernaryExpression>("a ? 1 : b");

            Assert.Equal(Kind.TernaryExpression, ternary.Kind);

            Assert.Equal("a", (Symbol)ternary.Condition);
            Assert.Equal(1,   (Integer)ternary.Left);
            Assert.Equal("b", (Symbol)ternary.Right);
        }

        [Fact]
        public void B()
        {
            var ternary = Parse<TernaryExpression>("x < 0.5 ? (x * 2) ** 3 / 2 : ((x - 1) * 2) ** 3 + 2) / 2");

            Assert.Equal(Kind.TernaryExpression, ternary.Kind);

            Assert.Equal(Kind.LessThanExpression, ternary.Condition.Kind);
        }
    }
}