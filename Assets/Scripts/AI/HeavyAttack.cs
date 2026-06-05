using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class HeavyAttack : Node
{
    private CombatController combatController;

    public HeavyAttack(Transform transform)
    {
        combatController = transform.GetComponent<CombatController>();
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");

        if (target == null)
        {
            state = NodeState.FAILURE;
            return state;
        }
        
        bool attackSarted = combatController.StartHeavyAttack(target);
        
        state = attackSarted ? NodeState.SUCCESS : NodeState.RUNNING;
        return state;
    }
}
