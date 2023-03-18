using UnityEngine;

[CreateAssetMenu(fileName = "MovementDetails_", menuName = "Scriptable Objects/Movement/MovementDetails")]
public class MovementDetailsSO : ScriptableObject
{
    #region Header MOVEMENT DETAILS
    [Space(10)]
    [Header("MOVEMENT DETAILS")]
    #endregion Header
    #region Tooltip
    [Tooltip("The minimum move speed. The GetMoveSpeed method calculates a random value between the minimum and maximum")]
    #endregion Tooltip
    public float minMoveSpeed = 8f;
    #region Tooltip
    [Tooltip("The maximum move speed. The GetMoveSpeed method calculates a random value between the minimum and maximum")]
    #endregion Tooltip
    public float maxMoveSpeed = 8f;
    #region Tooltip
    [Tooltip("if these is a dash movement - this is a dash speed")]
    #endregion
    public float dashSpeed;
    #region Tooltip
    [Tooltip("if there is a dash movement - this is a dash distance")]
    #endregion
    public float dashDistance;
    #region Tooltip
    [Tooltip("if there is a dash movement - this is the cooldown time in seconds between dash actions")]
    #endregion
    public float dashCooldownTime;

    public Vector2 patrolingArea;

    /// <summary>
    /// Get a random movement speed between the minimum and maximum values
    /// </summary>
    public float GetMoveSpeed()
    {
        return Random.Range(minMoveSpeed, maxMoveSpeed);
    }

    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(minMoveSpeed), minMoveSpeed, nameof(maxMoveSpeed), maxMoveSpeed, false);
        if (dashDistance != 0f || dashSpeed != 0 || dashCooldownTime != 0)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(dashDistance), dashDistance, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(dashSpeed), dashSpeed, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(dashCooldownTime), dashCooldownTime, false);
        }
    }

#endif
    #endregion Validation

}