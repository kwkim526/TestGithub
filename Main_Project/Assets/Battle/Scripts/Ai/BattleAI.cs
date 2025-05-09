using Battle.Ai.State;
using Battle.Ai.Weapon;
using Battle.Scripts.Value;
using Battle.Value;
using Pathfinding;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using StateMachine = Battle.Scripts.StateCore.StateMachine;
namespace Battle.Ai
{
    public enum TeamType { Player, Enemy }
    public enum WeaponType { Sword = 0, LongSpear = 66, TwoHanded = 100, shortSword = 83 }

    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(CharacterValue))]
    public class BattleAI : MonoBehaviour
    {
        public StateMachine StateMachine { get; private set; }
        
        public Transform CurrentTarget;
        public Transform tempTarget;
        public Transform Retreater;

        public TeamType team;
        public WeaponType weaponType;

        public float sightRange = 20f;
        public float attackRange = 0.5f;
        public float moveSpeed = 2f;
        public float hp = 100f;
        public float damage = 1f;
        public float retreatDistance = 3f;
        public float waitTime = 0.25f;
        public float stunTime = 0.25f;

        [Min(0.25f)] public float AttackDelay = 0.5f;

        private float lastAttackTime;

        public AiAnimator aiAnimator;
        public WeaponTrigger weaponTrigger;
        public GameObject HealthBar;

        public Targeting Targeting;
        public Rigidbody2D rb;
        private CircleCollider2D PlayerCollider;
        private CharacterValue characterValue;

        private Transform child;
        private Transform Weapon;
        
        public SpriteRenderer[] renderers;
        public Color[] originalColors;
        private Coroutine flashCoroutine;
        
        public AIPath aiPath;
        public AIDestinationSetter destinationSetter;
        
        public LayerMask obstacleMask;
        
        public IsWinner isWinner;

        private void Awake()
        {
            Targeting = new Targeting(this);
            StateMachine = new StateMachine();
            characterValue = GetComponent<CharacterValue>();
            isWinner = IsWinner.Instance;

            // HealthBar 자동 연결
            Transform hb = transform.Find("UnitRoot/HealthBar");
            if (hb != null)
            {
                HealthBar = hb.gameObject;
            }
            else
            {
                Debug.LogWarning($"{name}의 HealthBar를 찾을 수 없습니다.");
            }
        }

        private void Start()
        {
            StartCoroutine(DelayedStart());
        }

        private IEnumerator DelayedStart()
        {
            yield return new WaitForSeconds(Random.Range(0f, 0.5f)); // 0~0.5초 사이 랜덤 대기
            if (weaponTrigger != null)
            {
                weaponTrigger.Initialize(this);
            }
            StateMachine.ChangeState(new IdleState(this, true, 0f));
        }

        private void Update()
        {
            StateMachine.Update();
        }
        
        

        public bool HasEnemyInSight()
        {
            if (CurrentTarget != null) return CurrentTarget;
            return Targeting.FindNearestEnemy();
        }

        public bool CanAttack()
        {
            aiAnimator.StopMove();
            return Time.time >= lastAttackTime + AttackDelay;
        }
        public void RecordAttackTime() => lastAttackTime = Time.time;

        public bool IsInAttackRange()
        {
            if (CurrentTarget == null) return false;
            return Vector2.Distance(transform.position, CurrentTarget.position) <= attackRange;
        }

        public void MoveTo(Vector3 target)
        {
            aiPath.canMove = true;
            ResumeMoving(target);
        }
        
        public void ResumeMoving(Vector3 target)
        {
            Vector2 direction = (target - transform.position).normalized;
            transform.localScale = new Vector3(direction.x > 0 ? -1f : 1f, 1f, 1f);
            HealthBar?.GetComponent<HealthBarFixDirection>()?.ForceFix();
        }

        public void StopMoving()
        {
            destinationSetter.target = gameObject.transform;
            rb.velocity = Vector2.zero;
        }

        // 근처 벽과의 거리 탐지
        public bool IsWall()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.2f, obstacleMask);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Wall"))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsInRetreatDistance() {
            return Vector2.Distance(transform.position, Retreater.position) <= 0.1f;
        }

        public void UseSkill() => Debug.Log($"{gameObject.name} UseSkill");

        public void TakeDamage(float amount)
        {
            characterValue.TakeDamage(amount);
        }
        
        public void FlashRedTransparent(float alpha = 0.5f, float duration = 0.2f)
        {
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }
            flashCoroutine = StartCoroutine(FlashColorBlend(new Color(1f, 0f, 0f, alpha), duration));
        }

        private IEnumerator FlashColorBlend(Color flashColor, float duration)
        {
            // 붉게 + 반투명하게 덧씌움
            for (int i = 0; i < renderers.Length; i++)
            {
                Color blended = Color.Lerp(originalColors[i], flashColor, 0.6f); // 붉은 기조합
                blended.a = flashColor.a;
                renderers[i].color = blended;
            }

            yield return new WaitForSeconds(duration);

            // 정확하게 원래 색으로 복원
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].color = originalColors[i];
            }

            flashCoroutine = null;
        }
        
        public bool IsDead() => characterValue.currentHp <= 0;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, sightRange);
        }

        public void Kill()
        {
            isWinner.Winner(gameObject);
            Destroy(Retreater.gameObject);
            gameObject.SetActive(false);
            Destroy(gameObject);
        }

        // AI 설정창
        [ContextMenu("Setup AI Components Automatically")]
        public void SetupComponents()
        {
            characterValue = GetComponent<CharacterValue>();
            characterValue.StartSetting();

            rb = GetComponent<Rigidbody2D>();
            PlayerCollider = GetComponent<CircleCollider2D>();

            // Weapon 연결
            Weapon = transform.Find("Weapon");
            if (Weapon != null)
            {
                if (!Weapon.TryGetComponent(out weaponTrigger))
                {
                    weaponTrigger = Weapon.gameObject.AddComponent<WeaponTrigger>();
                }
            }
            // Retreater 생성 및 연결
            if (Retreater == null)
            {
                GameObject retreatObj = new GameObject(this.gameObject.name + "Retreat");
                var retreatTarget = retreatObj.AddComponent<RetreatTarget>();
                retreatTarget.ai = this;
                Retreater = retreatObj.transform;
            }
            
            // UnitRoot 연결
            child = transform.Find("UnitRoot");
            if (!child.TryGetComponent(out aiAnimator))
            {
                aiAnimator = child.gameObject.AddComponent<AiAnimator>();
            }
            
            // 캐릭터 전체 렌더러와 원래 색상 저장
            renderers = transform.Find("UnitRoot/Root").GetComponentsInChildren<SpriteRenderer>();
            originalColors = new Color[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                originalColors[i] = renderers[i].color;
            }

            // Rigidbody 설정
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            // Collider 설정
            PlayerCollider.isTrigger = true;
            PlayerCollider.radius = 0.1f;
            PlayerCollider.offset = new Vector2(0f, 0f);

            // HealthBar 연결
            Transform hb = transform.Find("UnitRoot/HealthBar");
            if (hb != null) HealthBar = hb.gameObject;

            // A* 알고리즘 연결
            aiPath = GetComponent<AIPath>();
            destinationSetter = GetComponent<AIDestinationSetter>();
            
            //A* 알고리즘 설정
            aiPath.maxSpeed = moveSpeed;
            aiPath.orientation = OrientationMode.YAxisForward;
            aiPath.whenCloseToDestination = CloseToDestinationMode.ContinueToExactDestination;
            aiPath.enableRotation = false;
            
            // 태그 설정
            gameObject.tag = team.ToString();
            Debug.Log("Warrior 설정이 완료되었습니다.");
        }
    }
}