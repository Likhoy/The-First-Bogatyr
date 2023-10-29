using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDetails_", menuName = "Scriptable Objects/Enemy/EnemyDetails")]
public class EnemyDetailsSO : ScriptableObject
{
    #region Header BASE ENEMY DETAILS
    [Space(10)]
    [Header("BASE ENEMY DETAILS")]
    #endregion

    #region Tooltip
    [Tooltip("The name of the enemy")]
    #endregion
    public string enemyName;

    #region Tooltip
    [Tooltip("Prefabs for the enemy (first for storyline, second for endless mode)")]
    #endregion
    public GameObject[] enemyPrefabs;

    #region Tooltip
    [Tooltip("Distance to the player maximum for the enemy to start attacking him")]
    #endregion
    public float aggressionDistance = 30f;

    #region Tooltip
    [Tooltip("Distance to the player before enemy starts chasing")]
    #endregion
    public float chaseDistance = 50f;

    public float shootDistance;

    #region Tooltip
    [Tooltip("Distance to the player before enemy starts using melee weapon")]
    #endregion
    public float handDistance = 3f;

    public float strikeDistance = 1f;

    /*#region Header ENEMY MATERIAL
    [Space(10)]
    [Header("ENEMY MATERIAL")]
    #endregion
    #region Tooltip
    [Tooltip("This is the standard lit shader material for the enemy (used after the enemy materializes")]
    #endregion
    public Material enemyStandardMaterial;*/

    /*#region Header ENEMY MATERIALIZE SETTINGS
    [Space(10)]
    [Header("ENEMY MATERIALIZE SETTINGS")]
    #endregion
    #region Tooltip
    [Tooltip("The time in seconds that it takes the enemy to materialize")]
    #endregion
    public float enemyMaterializeTime;
    #region Tooltip
    [Tooltip("The shader to be used when the enemy materializes")]
    #endregion
    public Shader enemyMaterializeShader;
    [ColorUsage(true, true)]
    #region Tooltip
    [Tooltip("The colour to use when the enemy materializes.  This is an HDR color so intensity can be set to cause glowing / bloom")]
    #endregion
    public Color enemyMaterializeColor;*/

    #region Header ENEMY WEAPON SETTINGS
    [Space(10)]
    [Header("ENEMY WEAPON SETTINGS")]
    #endregion

    #region Tooltip
    [Tooltip("The melee weapon for the enemy")]
    #endregion
    public MeleeWeaponDetailsSO enemyMeleeWeapon;

    // TODO

    #region Tooltip
    [Tooltip("The ranged weapon for the enemy - none if the enemy doesn't have a ranged weapon")]
    #endregion
    public RangedWeaponDetailsSO enemyRangedWeapon;
    #region Tooltip
    [Tooltip("The minimum time delay interval in seconds between bursts of enemy shooting.  This value should be greater than 0. A random value will be selected between the minimum value and the maximum value")]
    #endregion
    public float firingIntervalMin = 0.1f;
    #region Tooltip
    [Tooltip("The maximum time delay interval in seconds between bursts of enemy shooting.  A random value will be selected between the minimum value and the maximum value")]
    #endregion
    public float firingIntervalMax = 1f;
    #region Tooltip
    [Tooltip("The minimum firing duration that the enemy shoots for during a firing burst.  This value should be greater than zero.  A random value will be selected between the minimum value and the maximum value.")]
    #endregion
    public float firingDurationMin = 1f;
    #region Tooltip
    [Tooltip("The maximum firing duration that the enemy shoots for during a firing burst.  A random value will be selected between the minimum value and the maximum value.")]
    #endregion
    public float firingDurationMax = 2f;
    #region Tooltip
    [Tooltip("Select this if line of sight is required of the player before the enemy fires.  If line of sight isn't selected the enemy will fire regardless of obstacles whenever the player is 'in range'")]
    #endregion
    public bool firingLineOfSightRequired;

    #region Header ENEMY HEALTH
    [Space(10)]
    [Header("ENEMY HEALTH")]
    #endregion
    /*#region Tooltip
    [Tooltip("The health of the enemy for each level")]
    #endregion
    public EnemyHealthDetails[] enemyHealthDetailsArray;*/

    public int startingHealthAmount = Settings.defaultEnemyHealth;
    #region Tooltip
    [Tooltip("Select if has some effect period immediately after being hit.  If so specify the effect time in seconds in the other field")]
    #endregion
    public bool hasHitEffect = false;
    #region Tooltip
    [Tooltip("Hit effect time in seconds after being hit")]
    #endregion
    public float hitEffectTime;
    #region Tooltip
    [Tooltip("Select to display a health bar for the enemy")]
    #endregion
    public bool isHealthBarDisplayed = false;

    #region Header REWARD FOR ENEMY DEATH
    [Space(10)]
    [Header("REWARD FOR ENEMY DEATH")]
    #endregion

    public int moneyReward = 0;

    public float moneyDropRadius = 5f;

    #region Header ENDLESS MODE
    [Space(10)]
    [Header("ENDLESS MODE")]
    #endregion // maybe will be placed in a different SO

    #region Tooltip
    [Tooltip("Percent of enemy health growth each 3 waves")]
    #endregion
    public float baseHealthModifier = 0f;
    #region Tooltip
    [Tooltip("Percent of enemy damage growth each 3 waves")]
    #endregion
    public float baseDamageModifier = 0f;

    #region Header BOSS SETTINGS
    [Space(10)]
    [Header("BOSS SETTINGS")]
    #endregion

    public bool isBoss = false;

    public bool spawningImmediately = false;

    public LittleEnemySpawnData[] littleEnemySpawnDatas;

    public float spawnRadius;

    #region Tooltip
    [Tooltip("Count of experience that drops after death")]
    #endregion
    public int experienceDrop = 0;
    
    #region Validation
#if UNITY_EDITOR
    // Validate the scriptable object details entered
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(enemyName), enemyName);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemyPrefabs), enemyPrefabs);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(aggressionDistance), aggressionDistance, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(chaseDistance), chaseDistance, false);
        //HelperUtilities.ValidateCheckNullValue(this, nameof(enemyStandardMaterial), enemyStandardMaterial);
        //HelperUtilities.ValidateCheckPositiveValue(this, nameof(enemyMaterializeTime), enemyMaterializeTime, true);
        //HelperUtilities.ValidateCheckNullValue(this, nameof(enemyMaterializeShader), enemyMaterializeShader);
        //HelperUtilities.ValidateCheckNullValue(this, nameof(enemyMeleeWeapon), enemyMeleeWeapon);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(firingIntervalMin), firingIntervalMin, nameof(firingIntervalMax), firingIntervalMax, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(firingDurationMin), firingDurationMin, nameof(firingDurationMax), firingDurationMax, false);
        // HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemyHealthDetailsArray), enemyHealthDetailsArray);
        if (hasHitEffect)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(hitEffectTime), hitEffectTime, false);
        }
    }

#endif
    #endregion

}
