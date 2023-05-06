using System.Collections;
using UnityEngine;

public class StrengthPotion : Item  {
    private float _strengthDuration;

    private Player _player;
    private float _strengthTimer;

    override protected void Start() {
        base.Start();
        _player = GameManager.Instance.GetPlayer();
        _strengthDuration = 5;
    }

    private void Update() {
        _strengthTimer -= Time.deltaTime;
    }

    private IEnumerator HealerEffectRoutine() {
        while (_strengthTimer >= 0f) {
            // увеличиваю дамаг на 2
            (_player.activeWeapon.GetCurrentWeapon() as MeleeWeapon).weaponDetails.weaponMinDamage *= 2;
            (_player.activeWeapon.GetCurrentWeapon() as MeleeWeapon).weaponDetails.weaponMaxDamage *= 2;
            yield return new WaitForSeconds(1f);
        }
    }

    override public void UseItem() {
        // AnimateDrinking();
        _strengthTimer = _strengthDuration;
        StopAllCoroutines();
        StartCoroutine(HealerEffectRoutine());
    }
}
