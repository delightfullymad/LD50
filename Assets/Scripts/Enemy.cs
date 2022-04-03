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
    public float speed = 1f;
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.speed = speed;
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

    public void Damage(float dam)
    {
        health -= dam;
        anim.SetTrigger("Hurt");
        GameManager.gameManager.SFX.PlayOneShot(GameManager.gameManager.hitSounds[Random.Range(0,GameManager.gameManager.hitSounds.Length)]);
        if (health <= 0)
        {
            GameManager.gameManager.SFX.PlayOneShot(GameManager.gameManager.kill);
            anim.SetTrigger("Kill");
        }
    }

    void Kill()
    {
        GameManager.gameManager.currentEnemies.Remove(this);
        Destroy(gameObject);
    }

    public void Drain()
    {
        GameManager.gameManager.player.GetComponent<Animator>().SetTrigger("Attack");
        float temp = GameManager.gameManager.bloodCurve.Evaluate(health / maxHealth) * (maxHealth);
        if (temp <= 0) { temp = 0; }
        GameManager.gameManager.collectedBlood += temp;
        GameManager.gameManager.actionMode = mode.Normal;
        GameManager.gameManager.particles[GameManager.gameManager.currentEnemies.IndexOf(this)].emission.SetBurst(0, new ParticleSystem.Burst(0f,temp*10));
        GameManager.gameManager.particles[GameManager.gameManager.currentEnemies.IndexOf(this)].Play();
        GameManager.gameManager.SFX.PlayOneShot(GameManager.gameManager.drain);
        anim.SetTrigger("Drain");
    }

    public void Click()
    {
        if (GameManager.gameManager.actionMode == mode.Attack)
        {
            GameManager.gameManager.player.GetComponent<Animator>().SetTrigger("Attack");
            Damage(GameManager.gameManager.getDamage(GameManager.gameManager.weapon));
            GameManager.gameManager.actions--;
            if(GameManager.gameManager.actions <= 0)
            {
            GameManager.gameManager.acted = true;
            GameManager.gameManager.BlockCards();
            }
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
        GameManager.gameManager.SFX.PlayOneShot(GameManager.gameManager.hitSounds[Random.Range(0, GameManager.gameManager.hitSounds.Length)]);
        GameManager.gameManager.TakeDamage(Random.Range(minDamage, maxDamage));
    }

}
