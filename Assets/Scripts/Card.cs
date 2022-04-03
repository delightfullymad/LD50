using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    Animator anim;
    public CardInfo card;

    public TMP_Text cardName;
    public Image cardColour;
    public Image cardImage;
    public TMP_Text cardDescription;
    public bool overButton;


    // Start is called before the first frame update
    void Start()
    {
        if(card != null)
        {
            cardName.text = card.cardName;
            cardColour.color = card.cardColour;
            cardImage.sprite = card.cardImage;
            cardDescription.text = card.cardDescription;
        }
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!overButton && Input.GetMouseButtonDown(0) && anim.GetBool("Click")== true && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f)
        {
            anim.SetTrigger("Return");
        }
    }

    public void SetCard(CardInfo c)
    {
        card = c;
        cardName.text = c.cardName;
        cardColour.color = c.cardColour;
        cardImage.sprite = c.cardImage;
        cardDescription.text = c.cardDescription;
    }

    public void Hover()
    {
        if (GameManager.gameManager.acted == false)
        {
            anim.SetBool("Hover", true);
        }
    }
    public void HoverStop()
    {
        anim.SetBool("Hover", false);
    }


    public void Click()
    {
        if (anim.GetBool("Click") == false && anim.GetBool("Hover"))
        {
            anim.SetBool("Click", true);
        }
        
    }
    public void ClickStop()
    {
        anim.SetBool("Click", false);
    }

    public void UseCard()
    {
        GameManager.gameManager.actionMode = mode.Normal;
        GameManager.gameManager.damageMod = 1;


            switch (card.type)
        {
            case cardType.Attack:
                GameManager.gameManager.actionMode = mode.Attack;
                GameManager.gameManager.attackCard = GetComponent<Card>();
                GameManager.gameManager.damageMod = card.attackMod;
                break;

            case cardType.Defence:
                GameManager.gameManager.defending = true;
                GameManager.gameManager.acted = true;
                GameManager.gameManager.BlockCards();
                DestroyCard();
                break;

            case cardType.Heal:
                GameManager.gameManager.health += card.heal;
                GameManager.gameManager.acted = true;
                GameManager.gameManager.BlockCards();
                DestroyCard();
                break;

            case cardType.Weapon:
                GameManager.gameManager.weapon = card.weapon;
                GameManager.gameManager.BlockCards();
                GameManager.gameManager.acted = true;
                DestroyCard();
                break;

            case cardType.Armour:
                GameManager.gameManager.armour = card.armour;
                GameManager.gameManager.acted = true;
                GameManager.gameManager.BlockCards();
                DestroyCard();
                break;
            default:

                break;
        }

    }

    public void DestroyCard()
    {
        GameManager.gameManager.numofCards--;
        Destroy(transform.parent.gameObject);
    }

    public void IsOverButton()
    {
        overButton = true;
    }
    public void NotOverButton()
    {
        overButton = false;
    }

}
