using Battle.Scripts.StateCore;
using UnityEngine;
namespace Battle.Ai.State
{
    public class ChaseState : IState
    {
        private BattleAI ai;

        public ChaseState(BattleAI ai) { this.ai = ai; }

        public void EnterState()
        {
            ai.aiAnimator.Reset();
            ai.aiAnimator.Move();
        }

        public void UpdateState()
        {
            if (ai.CurrentTarget != null)
            {
                ai.destinationSetter.target = ai.CurrentTarget;
                ai.MoveTo(ai.CurrentTarget.position);
            }
            else
            {
                ai.Targeting.FindNearestEnemy();
                ai.StateMachine.ChangeState(new IdleState(ai, true));
            }
            if (ai.IsInAttackRange() && ai.CanAttack()) ai.StateMachine.ChangeState(new AttackState(ai));
        }

        public void ExitState(){}
    }
}