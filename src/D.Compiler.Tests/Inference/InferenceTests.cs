﻿using System.Collections.Generic;
using System.Linq;

using D.Expressions;
using D.Parsing;
using D.Syntax;

using Xunit;

namespace D.Compilation.Inference.Tests
{
    public class InferenceTests
    {
        [Fact]
        public void Mutiple()
        {
            var script = "b1 function(t: Number) => t * t * t;";

            var node = ParseFunction(script);

            Assert.Equal("b1", node.Name);
            Assert.Equal("t", node.Parameters[0].Name);
            Assert.Equal("Number", node.Parameters[0].Type.Name);

            var block           = node.Body as BlockExpression;
            var returnStatement = block.Statements[0] as ReturnStatement;
            var binary          = returnStatement.Expression as BinaryExpression;
            
            Assert.Equal("Object", node.ReturnType.Name);
        }

        [Fact]
        public void ParameterFlow()
        {
            var script = "same function(a: Number) { return a }";

            var node = ParseFunction(script);

            Assert.Equal("same",   node.Name);
            Assert.Equal("a",      node.Parameters[0].Name);
            Assert.Equal("Number", node.Parameters[0].Type.Name);

            Assert.Equal("Number", node.ReturnType.Name);
        }

        [Fact]
        public void ParameterFlow2()
        {
            var script = "same function(a: Number, b: Number) => a == b";

            var node = ParseFunction(script);

            Assert.Equal("same", node.Name);
            Assert.Equal("a", node.Parameters[0].Name);
            Assert.Equal("Number", node.Parameters[0].Type.Name);

            Assert.Equal("b", node.Parameters[1].Name);
            Assert.Equal("Number", node.Parameters[1].Type.Name);

            var body = (BlockExpression)node.Body;
            var returnStatement = (ReturnStatement)body[0];

            var b = (BinaryExpression)returnStatement.Expression;

            Assert.Equal(ObjectType.EqualsExpression, b.Operator.OpKind);
            Assert.True(b.Operator.IsComparision);

            Assert.Equal("Boolean", node.ReturnType.Name);
        }

        [Fact]
        public void InlineVariableFlow()
        {
            var script = "one function() { var one = 1; return one; }";

            var node = ParseFunction(script);

            Assert.Equal("one", node.Name);
            Assert.Equal("Int64", node.ReturnType.Name);
        }

        [Fact]
        public void InlineVariableFlow2()
        {
            var script = "one ƒ() { var one: Float32 = 1; let x = one; let y = x; return y; }";

            var node = ParseFunction(script);

            Assert.Equal("Float32", node.ReturnType.Name);
        }

        [Fact]
        public void LambdaLiteralFlow()
        {
            Assert.Equal("String",        ParseFunction("a ƒ() => \"a\"").ReturnType.Name);
            Assert.Equal("Int64",         ParseFunction("a ƒ() => 1").ReturnType.Name);
            Assert.Equal("Array<Int64>",  ParseFunction("a ƒ() => [ 1, 2, 3 ]").ReturnType.ToString());
            Assert.Equal("Array<String>", ParseFunction(@"a ƒ() => [ ""a"", ""b"", ""c"" ]").ReturnType.ToString());
            // Assert.Equal("Array<Object>", ParseFunction(@"a ƒ() => [ ""a"", 1 ]").ReturnType.ToString());
        }

        public static FunctionExpression ParseFunction(string text)
        {
           return new Compiler().VisitFunctionDeclaration(
               syntax: Parse(text).First() as FunctionDeclarationSyntax
           ) as FunctionExpression;
        }

        public static IEnumerable<ISyntaxNode> Parse(string source)
        {
            using var parser = new Parser(source);

            foreach (var node in parser.Enumerate())
            {
                yield return node;

            }
        }
    }
}