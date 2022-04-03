using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UI : MonoBehaviour
{
    public TMP_Text weaponDmg;
    public TMP_Text armourVal;
    public Slider healthbar;
    public Slider healthbarLimit;
    public Slider collectedBlood;
    public Slider collectedBlood2;


    public Slider healthbar2;
    public Slider healthbarLimit2;
    public Slider childBlood2;

    public TMP_Text hunger;

    public Slider timer;

    // Start is called before the first frame update
    void Start()
    {
        timer.maxValue = GameManager.gameManager.dayLength;
        healthbar.value = GameManager.gameManager.health;
        healthbarLimit.value = 100 - GameManager.gameManager.maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        timer.value = GameManager.gameManager.timeLeft;
        if(GameManager.gameManager.health != healthbar.value)
        {
            healthbar.value = GameManager.gameManager.health;
            healthbar2.value = GameManager.gameManager.health;
        }

        healthbarLimit.value = 100 - GameManager.gameManager.maxHealth;
        healthbarLimit2.value = 100 - GameManager.gameManager.maxHealth;

        collectedBlood.value = GameManager.gameManager.collectedBlood;
        collectedBlood2.value = GameManager.gameManager.collectedBlood;

        childBlood2.value = GameManager.gameManager.childHealth;

        if (GameManager.gameManager.weapon != null)
        {
            weaponDmg.text = GameManager.gameManager.weapon.minDamage.ToString() + " - " + GameManager.gameManager.weapon.maxDamage.ToString();
        }
        if (GameManager.gameManager.armour != null && armourVal.text != GameManager.gameManager.armour.value.ToString())
        {
            armourVal.text = GameManager.gameManager.armour.value.ToString();
        }
    }
}
