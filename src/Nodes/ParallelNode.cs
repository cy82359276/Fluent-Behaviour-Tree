using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentBehaviourTree
{
    /// <summary>
    /// Runs childs nodes in parallel.
    /// </summary>
    public class ParallelNode : IParentBehaviourTreeNode
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

        /// <summary>
        /// Number of child failures required to terminate with failure.
        /// </summary>
        private int numRequiredToFail;

        /// <summary>
        /// Number of child successess require to terminate with success.
        /// </summary>
        private int numRequiredToSucceed;

        public ParallelNode(string name, int numRequiredToFail, int numRequiredToSucceed)
        {
            this.name = name;
            this.numRequiredToFail = numRequiredToFail;
            this.numRequiredToSucceed = numRequiredToSucceed;

            this.iscompleted = false;
            this.children = new LinkedList<NodeStatusPair>();
        }

        public BehaviourTreeStatus Tick(TimeData time)
        {
            var numChildrenSuceeded = 0;
            var numChildrenFailed = 0;


            if (!iscompleted)
            {
                while (current != null)
                {
                    var status = current.Value.Action.Tick(time);
                    switch (status)
                    {
                        case BehaviourTreeStatus.Success: ++numChildrenSuceeded; break;
                        case BehaviourTreeStatus.Failure: ++numChildrenFailed; break;
                    }

                    current = current.Next;
                }

                iscompleted = true;
            }

            if (numRequiredToSucceed > 0 && numChildrenSuceeded >= numRequiredToSucceed)
            {
                return BehaviourTreeStatus.Success;
            }

            if (numRequiredToFail > 0 && numChildrenFailed >= numRequiredToFail)
            {
                return BehaviourTreeStatus.Failure;
            }

            return BehaviourTreeStatus.Failure;
        }

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
