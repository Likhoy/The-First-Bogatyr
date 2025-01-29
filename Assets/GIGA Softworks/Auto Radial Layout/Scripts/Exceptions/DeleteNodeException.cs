using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GIGA.AutoRadialLayout.Exceptions
{
    public class DeleteNodeException : Exception
    {
        public RadialLayoutNode Node { get; private set; }

        public DeleteNodeException(RadialLayoutNode node, string message) : base(message)
        {
            this.Node = node;    
        }
    }
}
