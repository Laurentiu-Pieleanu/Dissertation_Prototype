using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class Block : Node
{
    private CombatController combatController;

    public Block(Transform transform)
    {
        combatController = transform.GetComponent<CombatController>();
    }

    public override NodeState Evaluate()
    {
        bool blockStarted = combatController.Block();
        
        state = blockStarted ? NodeState.SUCCESS : NodeState.RUNNING;
        return state;
    }
}
