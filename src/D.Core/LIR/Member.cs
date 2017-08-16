﻿using System;

namespace D
{
    public abstract class Member
    {
        public Member(string name, Type type, ObjectFlags modifiers)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Modifiers = modifiers;
        }

        public string Name { get; }

        // String
        // String | Number
        // A & B
        public Type Type { get; }

        // mutable
        public ObjectFlags Modifiers { get; }

        #region Helpers

        public bool IsMutable => Modifiers.HasFlag(ObjectFlags.Mutable);
        
        #endregion

    }
}