using Battle.Scripts.StateCore;
using System.Collections;
using UnityEngine;
namespace Battle.Ai.State
{
    public class AttackState : IState
    {
        private BattleAI ai;
        private bool isFinished;

        public AttackState(BattleAI ai) { this.ai = ai; }

        public void EnterState()
        {
            ai.aiAnimator.StopMove();
            ai.aiAnimator.Reset();
            ai.aiAnimator.Attack();
            ai.weaponTrigger.ActivateCollider();
            ai.RecordAttackTime();
            ai.StartCoroutine(AttackDelay());
        }

        private IEnumerator AttackDelay()
        {
            yield return new WaitForSeconds(ai.attackRange / 2f); // 이 숫자 변수로 받을수도?
            ai.weaponTrigger.ColliderMove();
            // 무기 활성화 시간만큼 대기
            yield return new WaitForSeconds(ai.AttackDelay/2f); // 이 숫자도 변수로 받을수도?
            ai.weaponTrigger.DeactivateCollider();
            ai.StateMachine.ChangeState(new RetreatState(ai));
        }

        public void UpdateState()
        {
            
        }

        public void ExitState()
        {
        }
    }
}