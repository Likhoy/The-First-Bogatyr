using Unity.VisualScripting;
using UnityEngine;

public class StrengthPotion : Item  
{
    private float _strengthDuration;

    private Player _player;
    private float _strengthTimer;

    override protected void Start() 
    {
        base.Start();
        _player = GameManager.Instance.GetPlayer();
        _strengthDuration = 15;
    }

    override public void UseItem() 
    {
        /*audioEffects.PlayOneShot(CDrink);
        if (_player.activeWeapon.GetCurrentWeapon() != null)
        {
            StartCoroutine(ItemEffects.DamageEffectRoutine(_strengthDuration, effectImage, effectSprite));
        }*/

    }
}
