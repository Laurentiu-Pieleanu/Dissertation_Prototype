using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class CheckEnemyInFOVRange : Node
{
    private static int _enemyLayerMask = LayerMask.GetMask("Player");

    private Transform _transform;
    private Animator _animator;

    public CheckEnemyInFOVRange(Transform transform)
    {
        _transform = transform;
        _animator = transform.GetComponent<Animator>();
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");

        if (target != null)
        {
            float distance = Vector3.Distance(_transform.position, target.position);

            if (distance > BT.fovRange)
            {
                //Target outside of FOV range
                ClearData("target");
                _animator.SetBool("Walking", false);
                
                state = NodeState.FAILURE;
                return state;
            }
            state = NodeState.SUCCESS;
            return state;
        }
        
        
        Collider[] colliders = Physics.OverlapSphere(
            _transform.position, BT.fovRange, _enemyLayerMask);

        if (colliders.Length > 0)
        {
            parent.parent.SetData("target", colliders[0].transform);
            _animator.SetBool("Walking", true);
            state = NodeState.SUCCESS;
            return state;
        }
        
        state = NodeState.FAILURE;
        return state;
    }
    
}
