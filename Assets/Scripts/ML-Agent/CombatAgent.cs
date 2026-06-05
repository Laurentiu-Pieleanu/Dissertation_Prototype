using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class CombatAgent : Agent
{
    private Rigidbody rb;
    public Transform target;
    public CombatController combatController;
    public float moveSpeed = 2f;

    public Transform agentStartPoint;
    //public Transform targetStartPoint;
    
    private Health myHealth;
    private Health targetHealth;
    private Animator animator;

    private int lastAction = -1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        combatController = GetComponent<CombatController>();
        
        myHealth = GetComponent<Health>();
        targetHealth = target.GetComponent<Health>();
    }

    public override void OnEpisodeBegin()
    {
        if (myHealth != null)
        {
            myHealth.ResetHealth();
        }

        if (targetHealth != null)
        {
            targetHealth.ResetHealth();
        }

        if (agentStartPoint != null)
        {
            transform.position = agentStartPoint.position;
            transform.rotation = agentStartPoint.rotation;
        }

        combatController.ResetCombatState();
        
        CombatController targetCombat = target.GetComponent<CombatController>();

        if (targetCombat != null)
        {
            targetCombat.ResetCombatState();
        }
        
        lastAction = -1;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        float distance = Vector3.Distance(transform.position, target.position);
        
        sensor.AddObservation(distance/10f);
        
        sensor.AddObservation((float)myHealth._currentHealth / myHealth.maxHealth);
        sensor.AddObservation((float)targetHealth._currentHealth / targetHealth.maxHealth);
        
        sensor.AddObservation(combatController.IsBlocking() ? 1f : 0f);
        
        CombatController targetCombat = target.GetComponent<CombatController>();
        
        sensor.AddObservation(targetCombat != null && targetCombat.IsBlocking() ? 1f : 0f);
        
        sensor.AddObservation(distance <= combatController.attackRange ? 1f : 0f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int action = actions.DiscreteActions[0];

        float distance = Vector3.Distance(transform.position, target.position);
        bool inRange = distance <= combatController.attackRange;

        if (!inRange)
        {
            MoveTowardTarget();
            AddReward(0.01f);
            return;
        }
        
        if (action == lastAction)
        {
            AddReward(-0.04f);
        }
        
        lastAction = action;

        switch (action)
        {
            case 0:
                animator.SetBool("Walking", false);
                AddReward(-0.005f);
                break;
            
            case 1:
                animator.SetBool("Walking", false);
                if (combatController.StartLightAttack(target))
                {
                    AddReward(0.02f);
                }
                else
                {
                    AddReward(-0.02f);
                }
                break;
            
            case 2:
                animator.SetBool("Walking", false);
                if (combatController.StartHeavyAttack(target))
                {
                    AddReward(0.01f);
                }
                else
                {
                    AddReward(-0.03f);
                }
                break;
            
            case 3:
                animator.SetBool("Walking", false);

                if (combatController.Block())
                {
                    AddReward(0.005f);

                    if ((float)myHealth._currentHealth / myHealth.maxHealth < 0.4f)
                    {
                        AddReward(0.015f);
                    }
                }
                else
                {
                    AddReward(-0.02f);
                }
                break;
        }

        if (inRange && (action == 0))
        {
            AddReward(-0.01f);
        }
        
        AddReward(-0.001f);
    }
    
    private void MoveTowardTarget()
    {
        animator.SetBool("Walking", true);
        
        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
        {
            animator.SetBool("Walking", false);
            return;
        }
        
        direction.Normalize();
        
        Vector3 newPos = transform.position + direction * moveSpeed * Time.deltaTime;
        
        newPos.y = transform.position.y;
        
        rb.MovePosition(newPos);
        
        transform.rotation = Quaternion.LookRotation(direction);
    }
    
}
