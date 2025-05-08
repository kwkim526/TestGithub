using Battle.Scripts.StateCore;
using UnityEngine;
namespace Battle.Ai.State
{
    public class IdleState : IState
    {
        private BattleAI ai;
        private bool isChase;

        public IdleState(BattleAI ai, bool isChase)
        {
            this.ai = ai;
            this.isChase = isChase;
        }

        public void EnterState()
        {
            ai.aiAnimator.Reset();
            ai.aiAnimator.StopMove();
            ai.aiPath.canMove = false;
            ai.StopMoving();
            ai.rb.velocity = Vector2.zero;
        }

        public void UpdateState()
        {
            if (isChase)
            {
                if (ai.HasEnemyInSight())
                {
                    ai.destinationSetter.target = ai.CurrentTarget;
                    if (ai.IsInAttackRange())
                    {
                        if (ai.CanAttack()) ai.StateMachine.ChangeState(new AttackState(ai));
                    }
                    else
                    {
                        ai.StateMachine.ChangeState(new ChaseState(ai));
                    }
                }
            }
            else
            {
                ai.StateMachine.ChangeState(new RetreatState(ai));
            }
        }

        public void ExitState() { }
    }
}