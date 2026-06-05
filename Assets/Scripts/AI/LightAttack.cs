using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class LightAttack : Node
{
    private CombatController combatController;

    public LightAttack(Transform transform)
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
        
        bool attackStarted = combatController.StartLightAttack(target);
        
        state = attackStarted ? NodeState.SUCCESS : NodeState.RUNNING;
        return state;
    }

}
