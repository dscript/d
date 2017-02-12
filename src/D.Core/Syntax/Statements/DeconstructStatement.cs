﻿using System;

namespace D.Syntax
{
    // let (a, b, c) = point

    public class DestructuringAssignmentSyntax : SyntaxNode
    {
        public DestructuringAssignmentSyntax(AssignmentElementSyntax[] elements, SyntaxNode instance)
        {
            Variables = elements;
            Instance = instance;
        }

        public AssignmentElementSyntax[] Variables { get; }

        public SyntaxNode Instance { get; }

        Kind IObject.Kind => Kind.DestructuringAssignment;
    }

    public struct AssignmentElementSyntax
    {
        public AssignmentElementSyntax(string name, Symbol type)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type;
        }

        public string Name { get; }

        public Symbol Type { get; }
    }
}