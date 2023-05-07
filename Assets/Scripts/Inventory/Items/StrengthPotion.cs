using UnityEngine;

public class StrengthPotion : Item  {
    private float _strengthDuration;

    private Player _player;
    private float _strengthTimer;

    override protected void Start() {
        base.Start();
        _player = GameManager.Instance.GetPlayer();
        _strengthDuration = 30;

        // увеличиваю дамаг на 2
        if (_player.activeWeapon.GetCurrentWeapon() != null) // если есть оружие - увеличиваем урон
        {
            if (_player.activeWeapon.GetCurrentWeapon() is MeleeWeapon)
            {
                (_player.activeWeapon.GetCurrentWeapon() as MeleeWeapon).weaponDetails.weaponMinDamage *= 2;
                (_player.activeWeapon.GetCurrentWeapon() as MeleeWeapon).weaponDetails.weaponMaxDamage *= 2;
            }
        }

    }

    private void Update() {
        _strengthTimer -= Time.deltaTime;
    }

    override public void UseItem() {
        audioEffects.PlayOneShot(CDrink);
        if (_player.activeWeapon.GetCurrentWeapon() != null)
        {
            _strengthTimer += _strengthDuration;
            StartCoroutine(ItemEffects.DamageEffectRoutine(_strengthDuration));
        }

    }
}
