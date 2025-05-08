using Battle.Scripts.StateCore;
using System.Collections;
namespace Battle.Ai.State
{
    public class DamageState : IState
    {
        private BattleAI ai;
        private float damage;

        public DamageState(BattleAI ai, float damage)
        {
            this.ai = ai;
            this.damage = damage;
        }

        public void EnterState()
        {
            ai.TakeDamage(damage);
            ai.FlashRedTransparent(0.8f, 0.1f);
            ai.StartCoroutine(EndDamageRoutine());
        }
        
        private IEnumerator EndDamageRoutine()
        {
            if (ai.IsDead()) ai.StateMachine.ChangeState(new DeadState(ai));
            else
            {
                ai.StateMachine.ChangeState(new RetreatState(ai));
            }
            yield return null;
        }

        public void UpdateState()
        {
            
        }

        public void ExitState()
        {
            
        }
    }
}