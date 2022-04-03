using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 10f;
    public float health = 10f;
    public Slider healthBar;
    public float minDamage = 2f;
    public float maxDamage = 5f;
    Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
        healthBar.maxValue = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.value = health;
        if(GameManager.gameManager.actionMode == mode.Normal)
        {
            GetComponent<Button>().interactable = false;
        }
        else GetComponent<Button>().interactable = true;
    }

    void Damage(float dam)
    {
        health -= dam;
        anim.SetTrigger("Hurt");
        if(health <= 0)
        {
            Kill();
        }
    }

    void Kill()
    {
        GameManager.gameManager.currentEnemies.Remove(this);
        Destroy(gameObject);
    }

    public void Drain()
    {
        float temp = GameManager.gameManager.bloodCurve.Evaluate(health / maxHealth) * maxHealth;
        if (temp <= 0) { temp = 0; }
        GameManager.gameManager.collectedBlood += temp;
        GameManager.gameManager.actionMode = mode.Normal;
        Kill();
    }

    public void Click()
    {
        if (GameManager.gameManager.actionMode == mode.Attack)
        {
            Damage(GameManager.gameManager.getDamage(GameManager.gameManager.weapon));
            GameManager.gameManager.acted = true;
            GameManager.gameManager.attackCard.DestroyCard();
            GameManager.gameManager.actionMode = mode.Normal;
        }
        if (GameManager.gameManager.actionMode == mode.Drain)
        {
            Drain();
        }
    }

    public void TakeTurn()
    {
        anim.SetTrigger("Attack");
        
    }

    public void Attack()
    {
        GameManager.gameManager.TakeDamage(Random.Range(minDamage, maxDamage));
    }

}
