using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentBehaviourTree
{
    /// <summary>
    /// Selects the first node that succeeds. Tries successive nodes until it finds one that doesn't fail.
    /// </summary>
    public class SelectorNode : IParentBehaviourTreeNode
    {
        /// <summary>
        /// The name of the node.
        /// </summary>
        private string name;

        /// <summary>
        /// node sequence is completed.
        /// </summary>
        private bool iscompleted;

        /// <summary>
        /// List of child nodes.
        /// </summary>
        private LinkedList<NodeStatusPair> children;

        /// <summary>
        /// current child position.
        /// </summary>
        private LinkedListNode<NodeStatusPair> current;


        public SelectorNode(string name)
        {
            this.name = name;
            this.iscompleted = false;
            this.children = new LinkedList<NodeStatusPair>();
        }

        public BehaviourTreeStatus Tick(TimeData time)
        {
            if (!iscompleted)
            {
                bool hasrunning = false;

                while (current != null)
                {
                    var status = current.Value.Action.Tick(time);

                    if (status == BehaviourTreeStatus.Success)
                    {
                        iscompleted = true;
                        return status;
                    }
                    else if(status== BehaviourTreeStatus.Running)
                    {
                        hasrunning = true;
                    }

                    current = current.Next;
                }

                //if has running node, continue wait 
                if(hasrunning)
                {
                    //reset current position to first
                    current = children.First;
                    return BehaviourTreeStatus.Running;
                }

                iscompleted = true;
            }

            return BehaviourTreeStatus.Failure;
        }

        /// <summary>
        /// Add a child node to the selector.
        /// </summary>
        public void AddChild(IBehaviourTreeNode child)
        {
            NodeStatusPair nspair = new NodeStatusPair
            {
                Action = child,
                Status = BehaviourTreeStatus.Success
            };

            children.AddLast(nspair);
            current = children.First;
        }
    }
}
