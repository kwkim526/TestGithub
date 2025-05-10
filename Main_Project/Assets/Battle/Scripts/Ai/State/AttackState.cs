using System.Collections;
using Battle.Scripts.StateCore;
using UnityEngine;

namespace Battle.Scripts.Ai.State
{
    public class AttackState : IState
    {
        private BattleAI ai;
        public AttackState(BattleAI ai) { this.ai = ai; }

        public void EnterState()
        {
            ai.StopMoving();
            ai.Flip(ai.CurrentTarget.position);
            Debug.Log($"{ai} : {ai.StateMachine.currentState}");
            ai.aiAnimator.Reset();
            ai.aiAnimator.Attack();
            if (ai.weaponType == WeaponType.bow)
            {
                RangedAttack();
            }
            else
            {
                MeleeAttack();
            }
        }

        void MeleeAttack()
        {
            ai.weaponTrigger.ActivateCollider();
            ai.RecordAttackTime();
            ai.StartCoroutine(AttackDelay());
        }

        void RangedAttack()
        {
            ai.arrowWeaponTrigger.FireArrow();
            ai.StartCoroutine(RangedAttackDelay());
            ai.StateMachine.ChangeState(new IdleState(ai,false,ai.waitTime));
        }

        private IEnumerator RangedAttackDelay()
        {
            yield return new WaitForSeconds(ai.AttackDelay);
        }

        private IEnumerator AttackDelay()
        {
            yield return new WaitForSeconds(ai.AttackDelay / 2); // 이 숫자 변수로 받을수도?
            ai.aiAnimator.StopMove();
            ai.weaponTrigger.ColliderMove();
            ai.weaponTrigger.DeactivateCollider();
            yield return new WaitForSeconds(ai.AttackDelay / 2);
            ai.StateMachine.ChangeState(new IdleState(ai,false,ai.waitTime));
        }

        public void UpdateState() { }

        public void ExitState() { }
    }
}