using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Char : MonoBehaviour
{
    //-----------------------------------------//
    // Adding generic data for all entities
    public string entityName = "EntityName";
    public int entityLvl = 1;
    public int entityXP = 1;
    public bool isNPC = true;
    public bool canBeDamaged = true;
    public bool canBeHealed = true;
    public bool canUseMana = false;
    public bool canRestoreMana = false;
    public bool invisible = false;
    public bool canFly = false;
    //-----------------------------------------//
    // Adding basic stats for generic character
    public int health = 10, mana = 10, stamina = 10;
    public int maxHealth = 10, maxMana = 10, maxStamina = 10;
    public float moveSpeed = 1f;
    //0 - North, 1 - South(to screen), 2 - West, 3 - East
    protected int facing = 1;
    protected Rigidbody2D RB;
    protected Animator AN;
    protected bool isGrounded = false;
    public void Start()
    {
        //grabbing gameObjects components
        RB = GetComponent<Rigidbody2D>();
        AN = GetComponent<Animator>();
    }
    void Awake()
    {
    }
    //health,mana,stamina functions
    public bool takeDamage(int amount)
    {
        //TODO add resists calculation
        if (canBeDamaged)
        {
            health -= amount;
            if (health <= 0)
                die();
            return true;
        }
        return false;
    }

    public bool addHealth(int amount)
    {
        if (canBeHealed)
        {
            health += amount;
            if (health >= maxHealth)
                health = maxHealth;
            return true;
        }
        return false;
    }

    public bool removeMana(int amount)
    {
        if (!canRestoreMana)
        {
            mana -= amount;
            if (mana <= 0)
                mana = 0;
            return true;
        }
        return false;
    }
    public bool addMana(int amount)
    {
        if (canRestoreMana)
        {
            mana += amount;
            if (mana >= maxMana)
                mana = maxMana;
            return true;
        }
        return false;
    }

    public void die()
    {
        // throw new notImplementedException();
    }


    public virtual void FixedUpdate()
    {

    }
}
