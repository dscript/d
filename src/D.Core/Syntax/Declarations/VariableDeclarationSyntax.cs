﻿using E.Symbols;

namespace E.Syntax
{
    public sealed class VariableDeclarationSyntax : IMemberSyntax, ISyntaxNode
    {
        public VariableDeclarationSyntax(
            Symbol name, 
            TypeSymbol type, 
            ISyntaxNode? value = null, 
            ObjectFlags flags = default)
        {
            Name  = name;
            Type  = type;
            Value = value;
            Flags = flags;
        }

        public Symbol Name { get; }

        // String
        // String | Number
        // A & B
        public TypeSymbol Type { get; }

        // TODO: Condition

        public ISyntaxNode? Value { get; }

        public ObjectFlags Flags { get; }

        SyntaxKind ISyntaxNode.Kind => SyntaxKind.VariableDeclaration;
    }

    // a, b, c: Number

    public sealed class CompoundVariableDeclaration : ISyntaxNode
    {
        public CompoundVariableDeclaration(PropertyDeclarationSyntax[] declarations)
        {
            Members = declarations;
        }

        public PropertyDeclarationSyntax[] Members { get; }

        SyntaxKind ISyntaxNode.Kind => SyntaxKind.CompoundVariableDeclaration;
    }
}

/*
let a: Integer = 1;
let a: Integer > 1 = 5;
let a = 1;
var a = 1
*/