using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum mode { Normal, Attack, Drain}

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    public bool acted = false;
    public bool playerTurn = true;

    public float dayLength = 30f;
    public float timeLeft = 30f;
    public bool dayEnded;

    public Card attackCard;
    public int maxCards = 3;
    public int numofCards = 0;
    public Transform cardPanel;
    public GameObject cardPrefab;
    public CardInfo[] allCards;
    public GameObject[] allEnemies;
    public Transform[] spawnPoints;

    public List<Enemy> currentEnemies;

    public bool defending = false;
    public float childHealth = 100;
    public float collectedBlood;
    public float health = 100;
    public float maxHealth = 75;
    public Weapon weapon;
    public Armour armour;
    public mode actionMode = mode.Normal;

    public AnimationCurve bloodCurve;

    public Button endTurnButton;
    public Button drainButton;


    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        NewDay();
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft -= 1 * Time.deltaTime;
        if(health > maxHealth)
        {
            health = maxHealth;
        }
        //Debug.Log(currentEnemies.Count);
        //childHealth -= 1f * Time.deltaTime;

    }

    public void TakeDamage(float damage)
    {
        if (defending)
        {
            defending = false;
        }
        else 
        {
            float temp = (damage - armour.value);
            if (temp > 0f) {
                health -= temp;
            }
            if(health <= 0)
            {
                //GAME OVER
            }
        }
    }

    public void AttackMode()
    {
        actionMode = mode.Attack;
    }
    public void DrainMode()
    {
        actionMode = mode.Drain;
    }
    public void ChangeMode(mode m)
    {
        actionMode = m;
    }

    public float getDamage(Weapon weapon)
    {
        return Random.Range(weapon.minDamage, weapon.maxDamage);
    }

    public void DrawCard()
    {
        if(numofCards < maxCards)
        {
            GameObject c = Instantiate(cardPrefab, cardPanel);
            c.GetComponentInChildren<Card>().SetCard(allCards[Random.Range(0, allCards.Length)]);
            numofCards++;
        }
    }

    public void StartPlayerTurn()
    {
        endTurnButton.interactable = true;
        drainButton.interactable = true;
        acted = false;
        DrawCard();
    }

    public void EndPlayerTurn()
    {
        //childHealth -= 10f;
        playerTurn = false;
        endTurnButton.interactable = false;
        drainButton.interactable = false;
        if (currentEnemies.Count > 0)
        {
            StartCoroutine("StartEnemyTurn");
        }
        else if(timeLeft > 0f)
        {
            RespawnEnemies();
            StartPlayerTurn();  
        }
        else
        {
            PanLeft();
        }
    }

    public IEnumerator StartEnemyTurn()
    {
        for (int i = 0; i < currentEnemies.Count; i++)
        {
            currentEnemies[i].TakeTurn();
            yield return new WaitForSeconds(1f);
        }
        StartPlayerTurn();
    }

    public void RespawnEnemies()
    {
        for (int i = 0; i < Random.Range(1, 3); i++)
        {
            GameObject e = Instantiate(allEnemies[Random.Range(0, allEnemies.Length)], spawnPoints[i]);
            e.transform.position = spawnPoints[i].transform.position;
            currentEnemies.Add(e.GetComponent<Enemy>());
        }
        
    }

    public void PanLeft()
    {
        Camera.main.GetComponent<Animator>().SetBool("Left", true);
    }

    public void PanRight()
    {
        Camera.main.GetComponent<Animator>().SetBool("Left", false);
    }

    public void EndDay()
    {
        foreach (Transform child in cardPanel)
            Destroy(child);

        health = maxHealth;
        dayEnded = true;
    }

    public void NewDay()
    {
        DrawCard();
        DrawCard();
        DrawCard();
        //RespawnEnemies();
        GameObject e = Instantiate(allEnemies[Random.Range(0, allEnemies.Length)], spawnPoints[0]);
        e.transform.position = spawnPoints[0].transform.position;
        currentEnemies.Add(e.GetComponent<Enemy>());
        timeLeft = dayLength;
        dayEnded = false;
        StartPlayerTurn();
    }

    public void SacrificeHealth()
    {
        if (maxHealth > 10f)
        {
            maxHealth -= 10f;
            health = maxHealth;
            childHealth += 25f;
        }
    }

}
