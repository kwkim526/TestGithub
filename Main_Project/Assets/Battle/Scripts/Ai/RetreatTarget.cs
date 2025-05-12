using UnityEngine;

namespace Battle.Scripts.Ai
{
    public class RetreatTarget : MonoBehaviour
    {
        public BattleAI ai;
        public Vector2 baseDir;
        public Vector2 retreatPos;
        public Vector2 randomPos;

        public void SetRetreatTarget()
        {
            if (ai.weaponType == WeaponType.bow)
            {
                Debug.LogWarning("활잡이");
                //랜덤 좌표 생성 (범위 제한 포함)
                randomPos = new Vector2(
                    Random.Range(ai.retreatAreaMin.x, ai.retreatAreaMax.x),
                    Random.Range(ai.retreatAreaMin.y, ai.retreatAreaMax.y)
                );
                ai.Retreater.position = randomPos;
                ai.aiPath.canMove = true;
            }
            else
            {
                if (ai.IsInRetreatDistance() || ai.CurrentTarget == null)
                    return;
                if (ai.retreatDistance == 0)
                {
                    ai.Retreater.position = ai.transform.position;
                }
                else
                {
                    ai.tempTarget = ai.CurrentTarget;

                    // 후퇴 방향 계산
                    baseDir = (ai.transform.position - ai.tempTarget.position).normalized;
                    retreatPos = (Vector2)ai.transform.position + baseDir * ai.retreatDistance;

                    // 벽 내부 영역으로 제한
                    retreatPos = new Vector2(
                        Mathf.Clamp(retreatPos.x, ai.retreatAreaMin.x, ai.retreatAreaMax.x),
                        Mathf.Clamp(retreatPos.y, ai.retreatAreaMin.y, ai.retreatAreaMax.y)
                    );
                

                    // 벽 판정
                    if (!IsWall())
                    {
                        ai.Retreater.position = retreatPos;
                    }
                    else
                    {
                        //랜덤 좌표 생성 (범위 제한 포함)
                        Vector2 randomPos = new Vector2(
                            Random.Range(ai.retreatAreaMin.x, ai.retreatAreaMax.x),
                            Random.Range(ai.retreatAreaMin.y, ai.retreatAreaMax.y)
                        );
                        ai.Retreater.position = randomPos;
                        ai.aiPath.canMove = true;
                    }
                
                }
            }
            // 목적지 지정
            ai.destinationSetter.target = ai.Retreater;
        }
        private bool IsWall()
        {
            Vector2 origin = ai.transform.position;
            Vector2 direction = retreatPos - origin;
            float distance = direction.magnitude;

            RaycastHit2D hit = Physics2D.Raycast(origin, direction.normalized, distance, ai.obstacleMask);
            return hit.collider != null;
        }
    }
}
