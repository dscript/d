﻿using System.Linq;

using Xunit;

namespace D.Parsing.Tests
{
    using Syntax;

    public class PrimitiveTests : TestBase
    {
        [Fact]
        public void All()
        {
            var text = ReadDocument("base/primitives.d");
            
            var a = new Parser(text).Enumerate().ToArray();

            Assert.Equal(18, a.Length);
        }

        [Fact]
        public void A()
        {
            var a = Parse<TypeDeclarationSyntax>("Float32 primitive");

            Assert.Equal("Float32", a.Name);
            Assert.True(a.Flags.HasFlag(TypeFlags.Primitive));
        }

        [Fact]
        public void B()
        {
            var a = Parse<TypeDeclarationSyntax>("Int64 primitive { size = 8 }");

            Assert.Equal("Int64", a.Name);
            Assert.Equal(1, a.Attributes.Length);
            Assert.Equal("size", a.Attributes[0].Name);
            Assert.Equal(8, (NumberLiteral)a.Attributes[0].Value);
        }

        [Fact]
        public void PrimitiveConstraints()
        {
            // type jsnum = i64 between (-2**53 - 1).. (2**53 - 1);

            /*
            var a = Parse<TypeDeclaration>("clamped  type : Float");                    // between 0..1                    // inclusive
            var b = Parse<TypeDeclaration>("positive type : Float >= 0");
            var c = Parse<TypeDeclaration>("evens    type : Int32 % 2 == 0");
            var d = Parse<TypeDeclaration>("notempty type : String where length > 0");

            Assert.Equal("clamped", a.Name.ToString());
            */   
        }
    }       
}