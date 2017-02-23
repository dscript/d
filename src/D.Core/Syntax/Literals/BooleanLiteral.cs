﻿namespace D.Syntax
{
    public class BooleanLiteralSyntax : SyntaxNode
    {
        public static readonly BooleanLiteralSyntax True = new BooleanLiteralSyntax(true);
        public static readonly BooleanLiteralSyntax False = new BooleanLiteralSyntax(false);

        public BooleanLiteralSyntax(bool value)
        {
            Value = value;
        }

        public bool Value { get; }

        Kind IObject.Kind => Kind.Boolean;

        public override string ToString() => Value ? "true": "false";
    }
}