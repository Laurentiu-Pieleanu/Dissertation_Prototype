using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class RandomSelector : Node
    {
        private float _decisionDelay;
        private float _decisionTime;

        public RandomSelector(List<Node> children, float delay = 1f) : base(children)
        {
            _decisionDelay = delay;
            _decisionTime = -Mathf.Infinity;
        }

        public override NodeState Evaluate()
        {
            if (children == null || children.Count == 0)
            {
                state = NodeState.FAILURE;
                return state;
            }
            
            //Wait for cooldown
            if (Time.time - _decisionTime < _decisionDelay)
            {
                state = NodeState.RUNNING;
                return state;
            }
            
            //Pick random action from children
            int randomIndex = Random.Range(0, children.Count);
            Node selectNode = children[randomIndex];
            NodeState result = selectNode.Evaluate();
            
            //Reset timer when action is executed
            if (result == NodeState.SUCCESS)
            {
                _decisionTime = Time.time;
                state = NodeState.RUNNING;
                return state;
            }
            
            return result;
        }
    }
}

