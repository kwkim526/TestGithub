using Battle.Scripts.StateCore;
using UnityEngine;

namespace Battle.Ai.State
{
    public class RetreatState : IState
    {
        private BattleAI ai;

        public RetreatState(BattleAI Ai) { this.ai = Ai; }

        public void EnterState()
        {
            ai.aiAnimator.Reset();
            ai.aiAnimator.Move();
            Debug.Log($"{ai.gameObject.name} : 후퇴 시작");
            ai.Retreater.GetComponent<RetreatTarget>().SetRetreatTarget();
        }

        public void UpdateState()
        {
            ai.MoveTo(ai.Retreater.position);
            if (ai.IsInRetreatDistance())
            {
                if (ai.CurrentTarget != null)
                {
                    Debug.Log("후퇴 완료");
                    ai.destinationSetter.target = ai.CurrentTarget;
                    ai.StateMachine.ChangeState(new IdleState(ai, true));
                }
                else ai.StateMachine.ChangeState(new IdleState(ai, true));
            }
        }
        public void ExitState()
        {
        }
    }
}