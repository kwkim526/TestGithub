using UnityEngine;

namespace Battle.Ai
{
    public class RetreatTarget : MonoBehaviour
    {
        public BattleAI ai;

        public void SetRetreatTarget()
        {
            if(ai.IsInRetreatDistance() || ai.CurrentTarget == null) return;

            if (ai.retreatDistance == 0)
            {
                ai.Retreater.transform.position = ai.transform.position;
            }
            else
            {
                ai.tempTarget = ai.CurrentTarget;
                if (ai.IsWall())
                {
                    Vector2 baseDir = (ai.transform.position - ai.tempTarget.position).normalized;
                    Vector2 randomOffset = new Vector2(Random.Range(-0.2f, -0.01f), Random.Range(-0.2f, -0.01f));
                    Vector2 dir = (baseDir + randomOffset).normalized;
                    Vector2 retreatPos = (Vector2)ai.transform.position + dir * ai.retreatDistance;

                    ai.Retreater.position = retreatPos;
                }
                else
                {
                    Debug.Log("벽 감지됨");
                    ai.Retreater.position = new Vector2(Random.Range(-5f, 5f), Random.Range(-3f, 3f));
                }
            }

            ai.destinationSetter.target = ai.Retreater;
        }
    }
}