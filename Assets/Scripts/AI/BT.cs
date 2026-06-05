using System.Collections.Generic;
using BehaviorTree;

public class BT : Tree
{
    public static float speed = 10f;
    public static float fovRange = 12f;
    public static float attackRange = 2f;
    protected Node root;
    protected override Node SetupTree()
    {
        root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckEnemyInAttackRange(transform),
                new RandomSelector(new List<Node>
                {
                    new LightAttack(transform),
                    new HeavyAttack(transform),
                    new Block(transform)
                },1f)
            }),
            new Sequence(new List<Node>
            {
                new CheckEnemyInFOVRange(transform),
                new GoToTarget(transform),
            }),
        });
        
        return root; 
    }

    private void OnDrawGizmosSelected()
    {
        UnityEngine.Gizmos.color = UnityEngine.Color.yellow;
        UnityEngine.Gizmos.DrawWireSphere(transform.position, fovRange);
        
        UnityEngine.Gizmos.color = UnityEngine.Color.red;
        UnityEngine.Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
