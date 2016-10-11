using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentBehaviourTree
{
    /// <summary>
    /// Runs child nodes in sequence, until one fails.
    /// </summary>
    public class SequenceNode : IParentBehaviourTreeNode
    {
        /// <summary>
        /// Name of the node.
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


        public SequenceNode(string name)
        {
            this.name = name;
            this.iscompleted = false;
            this.children = new LinkedList<NodeStatusPair>();
        }

        public BehaviourTreeStatus Tick(TimeData time)
        {

            if (!iscompleted)
            {
                while (current != null)
                {
                    var status = current.Value.Action.Tick(time);

                    switch (status)
                    {
                        case BehaviourTreeStatus.Failure:
                            {
                                iscompleted = true;
                                return status;
                            }
                        case BehaviourTreeStatus.Running:
                            {
                                iscompleted = false;
                                return status;
                            }
                    }

                    current = current.Next;
                }

                iscompleted = true;
            }

            return BehaviourTreeStatus.Success;
        }

        /// <summary>
        /// Add a child to the sequence.
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
