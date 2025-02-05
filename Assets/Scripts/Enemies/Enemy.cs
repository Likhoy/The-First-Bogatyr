using TheKiwiCoder;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

#region REQUIRE COMPONENTS
// [RequireComponent(typeof(BehaviourTreeInstance))]
//[RequireComponent(typeof(AnimateEnemy))]
// [RequireComponent(typeof(SpriteRenderer))]
// [RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(NavMeshAgent))]
#endregion REQUIRE COMPONENTS

[DisallowMultipleComponent]
public class Enemy : MonoBehaviour
{
    [HideInInspector] public EnemyDetailsSO enemyDetails;
    // private MaterializeEffect materializeEffect;
    private NavMeshAgent agent;
    private BoxCollider2D boxCollider2D;
    private CapsuleCollider2D capsuleCollider2D;
    [HideInInspector] public SpriteRenderer[] spriteRendererArray;
    [HideInInspector] public Animator animator;
    [HideInInspector] public MeleeAttackEvent meleeAttackEvent;
    [HideInInspector] public ActiveWeapon activeWeapon;
    [HideInInspector] public WeaponFiredEvent weaponFiredEvent;
    [HideInInspector] public DefendingStageStartedEvent defendingStageStartedEvent;
    [HideInInspector] public DefendingStageEndedEvent defendingStageEndedEvent;
    [HideInInspector] public LookAtEvent lookAtEvent;

    private MonsterAi monsterAi;

    private BehaviourTreeInstance behaviourTree;

    private AudioSource audioSource;
    private AudioSource audioEffects;
    [SerializeField] private AudioClip CGetDamage;
    [SerializeField] private AudioClip[] CMoney;
    private Vector3 before;
    private Vector3 after;
    private bool costil = true;

    public MeleeWeapon MeleeWeapon { get; private set; }
    public RangedWeapon RangedWeapon { get; private set; }

    private void Awake()
    {
        // Load components
        audioSource = GetComponent<AudioSource>();
        audioEffects = GameObject.Find("AudioEffects").GetComponent<AudioSource>();
        //aimWeaponEvent = GetComponent<AimWeaponEvent>();
        // materializeEffect = GetComponent<MaterializeEffect>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        spriteRendererArray = GetComponentsInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
        meleeAttackEvent = GetComponent<MeleeAttackEvent>();
        activeWeapon = GetComponent<ActiveWeapon>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
        defendingStageStartedEvent = GetComponent<DefendingStageStartedEvent>();
        defendingStageEndedEvent = GetComponent<DefendingStageEndedEvent>();
        lookAtEvent = GetComponent<LookAtEvent>();
        before = transform.position;

        agent = GetComponent<NavMeshAgent>();
        behaviourTree = GetComponent<BehaviourTreeInstance>();

        monsterAi = GetComponent<MonsterAi>();

        // в 2D это устанавливается
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    /*private void Start()
    {
        
    }*/

    private void Update()
    {
        // обязательно для NavMeshAgent
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        /*after = transform.position;

        if (before != after)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
            audioSource.Stop();

        before = after;*/
    }

    /// <summary>
    /// Handle health lost event
    /// </summary>
    private void HealthEvent_OnHealthLost(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        if (costil) costil = false;
        else audioEffects.PlayOneShot(CGetDamage, 0.7f);

        if (healthEventArgs.healthAmount <= 0)
        {
            audioEffects.PlayOneShot(CGetDamage, 0.7f);
            EnemyDestroyed();
        }
    }

    /// <summary>
    /// Enemy destroyed
    /// </summary>
    private void EnemyDestroyed()
    {
        DestroyedEvent destroyedEvent = GetComponent<DestroyedEvent>();
        destroyedEvent.CallDestroyedEvent(false, enemyDetails.experienceDrop);

        if (enemyDetails.moneyReward > 0 && SceneManager.GetActiveScene().name != Settings.purpleWorldSceneName)
        {
            for (int i = 0; i < enemyDetails.moneyReward / 100; i++) // needed to add another money values
            {
                float positionXOffset = Random.Range(-enemyDetails.moneyDropRadius, enemyDetails.moneyDropRadius);
                float yMaxOffset = Mathf.Sqrt(enemyDetails.moneyDropRadius * enemyDetails.moneyDropRadius - positionXOffset * positionXOffset);
                float positionYOffset = Random.Range(-yMaxOffset, yMaxOffset);
                Instantiate(GameResources.Instance.coins[0], transform.position + new Vector3(positionYOffset, positionYOffset), Quaternion.identity);
                System.Random rand = new System.Random();
                audioEffects.PlayOneShot(CMoney[rand.Next(CMoney.Length)]);
            }
        }
    }


    /// <summary>
    /// Initialise the enemy
    /// </summary>
    public void EnemyInitialization(EnemyDetailsSO enemyDetails, int enemySpawnNumber, EnemyModifiers enemyModifiers = null)
    {
        this.enemyDetails = enemyDetails;

        SetEnemyMovementUpdateFrame(enemySpawnNumber);

        if (enemyModifiers != null)
        {
            SetEnemyStartingHealth(enemyModifiers.healthModifierEffect);
        }
        SetEnemyStartingHealth();

        SetEnemyStartingWeapon();

        // SetEnemyAnimationSpeed();

        // Materialise enemy
        //StartCoroutine(MaterializeEnemy());
    }

    /// <summary>
    /// Set enemy movement update frame
    /// </summary>
    private void SetEnemyMovementUpdateFrame(int enemySpawnNumber)
    {
        // Set frame number that enemy should process it's updates
        monsterAi.SetUpdateFrameNumber(enemySpawnNumber % Settings.targetFrameRateToSpreadPathfindingOver);
    }


    /// <summary>
    /// Set the starting health for the enemy
    /// </summary>
    private void SetEnemyStartingHealth(int modifierEffect = 0)
    {
        // Get the enemy health for the dungeon level
        /*foreach (EnemyHealthDetails enemyHealthDetails in enemyDetails.enemyHealthDetailsArray)
        {
            if (enemyHealthDetails.dungeonLevel == dungeonLevel)
            {
                health.SetStartingHealth(enemyHealthDetails.enemyHealthAmount);
                return;
            }
        }*/
        // health.SetStartingHealth(enemyDetails.startingHealthAmount + modifierEffect);
    }

    /// <summary>
    /// Set enemy starting weapon as per the weapon details SO
    /// </summary>
    private void SetEnemyStartingWeapon()
    {
        // Process if enemy has a weapon
        /*if (enemyDetails.enemyRangedWeapon != null)
        {
            RangedWeapon weapon = new RangedWeapon() { weaponDetails = enemyDetails.enemyRangedWeapon, 
                weaponCurrentMinDamage = enemyDetails.enemyRangedWeapon.GetWeaponMinDamage(),
                weaponCurrentMaxDamage = enemyDetails.enemyRangedWeapon.GetWeaponMaxDamage(),
                weaponReloadTimer = 0f, 
                weaponClipRemainingAmmo = enemyDetails.enemyRangedWeapon.weaponClipAmmoCapacity, 
                weaponRemainingAmmo = enemyDetails.enemyRangedWeapon.weaponAmmoCapacity, 
                isWeaponReloading = false };

            // Set weapon for enemy
            setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon, true);
            RangedWeapon = weapon;
        }
        
        if (enemyDetails.enemyMeleeWeapon != null)
        {
            MeleeWeapon = new MeleeWeapon() { weaponDetails = enemyDetails.enemyMeleeWeapon,
                weaponCurrentMinDamage = enemyDetails.enemyMeleeWeapon.GetWeaponMinDamage(),
                weaponCurrentMaxDamage = enemyDetails.enemyMeleeWeapon.GetWeaponMaxDamage(),
                weaponListPosition = 1 };
            
            if (activeWeapon.GetCurrentWeapon() == null)
                setActiveWeaponEvent.CallSetActiveWeaponEvent(MeleeWeapon, false);
        }*/
    }

    /// <summary>
    /// Set enemy animator speed to match movement speed
    /// </summary>
    /*private void SetEnemyAnimationSpeed()
    {
        // Set animator speed to match movement speed
        animator.speed = enemyMovementAI.moveSpeed / Settings.baseSpeedForEnemyAnimations;
    }*/

    /*private IEnumerator MaterializeEnemy()
    {
        // Disable collider, Movement AI and Weapon AI
        EnemyEnable(false);

        yield return StartCoroutine(materializeEffect.MaterializeRoutine(enemyDetails.enemyMaterializeShader, enemyDetails.enemyMaterializeColor, enemyDetails.enemyMaterializeTime, spriteRendererArray, enemyDetails.enemyStandardMaterial));

        // Enable collider, Movement AI and Weapon AI
        EnemyEnable(true);

    }*/

    private void EnemyEnable(bool isEnabled)
    {
        // Enable/Disable colliders
        boxCollider2D.enabled = isEnabled;
        capsuleCollider2D.enabled = isEnabled;

        // Enable/Disable enemy behaviour
        behaviourTree.enabled = isEnabled;

        // Enable / Disable Fire Weapon
        // fireWeapon.enabled = isEnabled;

    }
}
