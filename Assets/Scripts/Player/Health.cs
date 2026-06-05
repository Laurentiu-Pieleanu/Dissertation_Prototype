using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int _currentHealth;
    
    public HealthBar healthBar;

    public bool isPlayer;
    public LevelEndManager levelEndManager;
    
    private CombatAgent combatAgent;
    public bool useTrainingEpisodeEnd = false;
    public CombatLogManager combatLogManager;
    
    // Start is called before the first frame update
    void Start()
    {
        _currentHealth = maxHealth;
        combatAgent = GetComponent<CombatAgent>();

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    public void ResetHealth()
    {
        _currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.SetHealth(_currentHealth);
        }
    }

    public void TakeDamage(int damage, GameObject attacker = null)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth); 
        
        //Debug.Log(gameObject.name + " took damage. Remaining HP: " + _currentHealth);

        if (healthBar != null)
        {
            healthBar.SetHealth(_currentHealth);
        }
        
        //punish agent for taking damage
        if (combatAgent != null)
        {
            combatAgent.AddReward(-0.3f);
        }
        
        //reward attacker if it is the ML-Agent
        if (attacker != null)
        {
            CombatAgent attackerAgent = attacker.GetComponent<CombatAgent>();
        }

        if (_currentHealth <= 0)
        {
            Debug.Log(gameObject.name + " DIED - export section reached");
            
            Die();
            
            //losing punishment
            if (combatAgent != null)
            {
                combatAgent.AddReward(-5f);
                if (useTrainingEpisodeEnd)
                {
                    combatAgent.EndEpisode();
                }
            }
            
            //winner reward
            if (attacker != null)
            {
                CombatAgent attackerAgent = attacker.GetComponent<CombatAgent>();

                if (attackerAgent != null)
                {
                    attackerAgent.AddReward(5f);
                    if (useTrainingEpisodeEnd)
                    {
                        attackerAgent.EndEpisode();
                    }
                }
            }
        }
        
        //Debug.Log(gameObject.name + " took " + damage + " damage from " + attacker.name);
    }

    private void Die()
    {
        StartCoroutine(HandleDeath());
    }

    private IEnumerator HandleDeath()
    {
        Animator animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        BT bt = GetComponent<BT>();
        if (bt != null)
        {
            bt.enabled = false;
        }
        
        if (combatAgent != null)
        {
            combatAgent.enabled = false;
        }

        if (combatLogManager != null)
        {
            combatLogManager.ExportLog();
        }

        yield return new WaitForSeconds(2f);

        if (levelEndManager != null)
        {
            levelEndManager.EndLevel(!isPlayer);
        }
    }
}
