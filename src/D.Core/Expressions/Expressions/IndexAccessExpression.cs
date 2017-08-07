﻿namespace D.Expressions
{
    // [index]
    public class IndexAccessExpression : IExpression
    {
        public IndexAccessExpression(IExpression left, IArguments arguments)
        {
            Left = left;
            Arguments = arguments;
        }

        public IExpression Left { get; set; }

        // [1]
        // [1, 2]
        public IArguments Arguments { get; set; }

        Kind IObject.Kind => Kind.IndexAccessExpression;
    }
}