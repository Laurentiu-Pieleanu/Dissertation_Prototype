using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    private Animator _animator;

    public CombatLogManager combatLogManager;
    
    public int lightDamage = 10;
    public int heavyDamage = 20;
    public float lightAttackCD = 1.0f;
    public float heavyAttackCD = 1.5f;
    public float blockCD = 1.0f;
    public float attackRange = 2f;
    
    private float lastLightAttackTime = -999f;
    private float lastHeavyAttackTime = -999f;
    private float lastBlockTime = -999f;

    private bool isActionLocked = false;

    private Transform currentTarget;
    private bool usingHeavyAttack = false;
    private bool isBlocking = false;
    
    public string agentType = "Unknown";

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public bool StartLightAttack(Transform target)
    {
        if(isActionLocked)
            return false;
        
        if(Time.time - lastLightAttackTime < lightAttackCD)
            return false;
        
        currentTarget = target;
        usingHeavyAttack = false;
        
        isActionLocked = true;
        lastLightAttackTime = Time.time;
        
        _animator.SetTrigger("LightAttack");
        
        LogAction("Light");
        
        return true;
    }

    public bool StartHeavyAttack(Transform target)
    {
        if(isActionLocked)
            return false;
        
        if(Time.time - lastHeavyAttackTime < heavyAttackCD)
            return false;
        
        currentTarget = target;
        usingHeavyAttack = true;
        
        isActionLocked = true;
        lastHeavyAttackTime = Time.time;
        
        _animator.SetTrigger("HeavyAttack");
        
        LogAction("Heavy");
        
        return true;
    }

    public bool Block()
    {
        if(isActionLocked)
            return false;
        
        if(Time.time - lastBlockTime < blockCD)
            return false;
        
        isBlocking = true;
        isActionLocked = true;
        lastBlockTime = Time.time;
        
        _animator.SetTrigger("Block");
        
        LogAction("Block");

        return true;
    }

    public void StopBlocking()
    {
        isBlocking = false;
    }

    public bool IsBlocking()
    {
        return isBlocking;
    }

    public void UnlockAction()
    {
        isActionLocked = false;
    }

    public void ResetCombatState()
    {
        currentTarget = null;
        usingHeavyAttack = false;
        isBlocking = false;
    }
    
    public void ApplyAttackDamage()
    {
        if (currentTarget == null)
        {
            return;
        }
        float distance = Vector3.Distance(transform.position, currentTarget.position);

        if (distance > attackRange)
        {
            Debug.Log("Attack Missed");
            
            CombatAgent agent = GetComponent<CombatAgent>();

            if (agent != null)
            {
                agent.AddReward(-0.2f);
            }

            return;
        }
        
        Health health = currentTarget.GetComponent<Health>();
        
        if (health != null)
        {
            int damage = usingHeavyAttack? heavyDamage : lightDamage;
            
            CombatController targetCombat = currentTarget.GetComponent<CombatController>();

            if (targetCombat != null && targetCombat.IsBlocking())
            {
                CombatAgent blockerAgent = currentTarget.GetComponent<CombatAgent>();

                if (blockerAgent != null)
                {
                    blockerAgent.AddReward(0.3f);
                }

                CombatAgent attackerAgent = GetComponent<CombatAgent>();

                if (attackerAgent != null)
                {
                    attackerAgent.AddReward(-0.2f);
                }
                
                return;
            }
            
            health.TakeDamage(damage, gameObject);
        }
    }

    public void LogAction(string actionType)
    {
        ActionLogEntry entry = new ActionLogEntry(
            agentType,
            actionType,
            Time.time,
            Time.frameCount,
            transform);

        if (combatLogManager != null)
        {
            combatLogManager.LogAction(entry);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
