using Battle.Scripts.StateCore;
using UnityEngine;
namespace Battle.Ai.State
{
	public class RetreatState : IState
	{
		private BattleAI ai;

		public RetreatState(BattleAI ai) { this.ai = ai; }

		public void EnterState()
		{
			Debug.Log("후퇴 실행");
			ai.StopMoving();
			ai.retreatTargetmovement.SetRetreatTarget();
			ai.aiAnimator.Move();
		}

		public void UpdateState()
		{
			if (ai.CurrentTarget != null)
			{
				ai.MoveTo(ai.CurrentTarget.position);
				if (ai.IsInRetreatDistance())
				{
					if (ai.tempTarget != null)
					{
						ai.CurrentTarget = ai.tempTarget;
						ai.StateMachine.ChangeState(new ChaseState(ai));
					} else
					{
						ai.StateMachine.ChangeState(new IdleState(ai));
					}
				}
			}
		}

		public void ExitState() => ai.StopMoving();
	}
}
