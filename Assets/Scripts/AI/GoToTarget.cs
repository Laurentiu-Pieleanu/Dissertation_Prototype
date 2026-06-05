using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class GoToTarget : Node
{
    private Transform _transform;
    private Rigidbody _rb;
    
    public GoToTarget(Transform transform)
    {
        _transform = transform;
        _rb = _transform.GetComponent<Rigidbody>();
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");

        if (target == null)
        {
            state = NodeState.FAILURE;
            return state;
        }
        
        if (Vector3.Distance(_transform.position, target.position) > BT.attackRange)
        {
            Vector3 direction = (target.position - _transform.position).normalized;

            Vector3 newPosition = _transform.position + direction * BT.speed * Time.deltaTime;

            if (_rb != null)
            {
                _rb.MovePosition(newPosition);
            }
            else
            {
                _transform.position = newPosition;
            }
            
            _transform.LookAt(target.position);
        }
        state = NodeState.RUNNING;
        return state;
    }
}
