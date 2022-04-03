using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public enum mode { Normal, Attack, Drain}

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    public UI ui;
    public bool acted = false;
    public int actions = 2;
    public bool playerTurn = true;
    public GameObject player;
    
    public AudioMixer room;
    public AudioMixer battle;

    public AudioClip[] hitSounds;
    public AudioClip blood;
    public AudioClip cardSlide;
    public AudioClip discard;
    public AudioClip drain;
    public AudioClip use;
    public AudioClip kill;
    public AudioClip bomb;
    public AudioClip shield;
    public AudioClip heal;
    public AudioSource SFX;


    public float dayLength = 30f;
    public float timeLeft = 30f;
    public bool dayEnded;
    public int day = 0;

    public Card attackCard;
    public int maxCards = 3;
    public int numofCards = 0;
    public Transform cardPanel;
    public GameObject cardPrefab;
    public CardInfo[] allCards;
    public GameObject[] allEnemies;
    public Transform[] spawnPoints;
    public ParticleSystem[] particles;
    public ParticleSystem roomParticles;
    public ParticleSystem sacrificeParticles;


    public List<Enemy> currentEnemies;

    public bool defending = false;
    public float childHealth = 100;
    public float collectedBlood;
    public float health = 100;
    public float maxHealth = 75;
    public Weapon weapon;
    public Armour armour;
    public Transform equipSlot;
    public float damageMod = 1f;

    public mode actionMode = mode.Normal;

    public AnimationCurve bloodCurve;

    public Button endTurnButton;
    public Button drainButton;

    public Button newDayButton;
    public Button sacrificeButton;

    public Transform sky;
    public SpriteRenderer hills;
    public Color hillColourDay;
    public Color hillColourNight;
    public Image eyes;
    public TMP_Text actionsLeft;

    public GameObject gameOver;
    public GameObject menu;


    public void closeMenu()
    {
        menu.GetComponent<Animator>().SetTrigger("Close");
    }

    private void Awake()
    {
        
        gameManager = GetComponent<GameManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        battle.SetFloat("musicVol", -50f);
        //NewDay();
        playerTurn = false;
        endTurnButton.interactable = false;
        drainButton.interactable = false;
        dayEnded = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (menu.activeInHierarchy == true && menu.GetComponent<CanvasGroup>().alpha <= 0)
        {
            menu.SetActive(false);
        }

        actionsLeft.text = actions.ToString();
        if(health <= 0f || childHealth <= 0f)
        {
            gameOver.SetActive(true);
            ui.daysSurvived.text = "You survived for " + day + " days.";
        }
        if(equipSlot.GetComponent<SpriteRenderer>().sprite != weapon.sprite)
        {
            equipSlot.GetComponent<SpriteRenderer>().sprite = weapon.sprite;
        }

        if(acted)
        {
            endTurnButton.GetComponent<Outline>().enabled = true;
        }
        else
        {
            endTurnButton.GetComponent<Outline>().enabled = false;
        }

        timeLeft -= 1 * Time.deltaTime;
        sky.position = Vector3.Lerp(new Vector3(3f, -7f, 0f), new Vector3(3f, 7f, 0f), timeLeft / dayLength);
        hills.color = Color.Lerp(hillColourNight, hillColourDay, timeLeft / dayLength);

        Color eyeTemp = eyes.color;
        eyeTemp.a = (100f - childHealth) / 100f;
        eyes.color = eyeTemp;

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
            player.GetComponent<Animator>().SetTrigger("Defend");
            defending = false;
        }
        else 
        {
            player.GetComponent<Animator>().SetTrigger("Hit");
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
        float dam = Random.Range(weapon.minDamage, weapon.maxDamage) * damageMod; ;
        damageMod = 1;
        return dam;
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
        actions = 2;

        foreach (Transform child in GameManager.gameManager.cardPanel)
        {
            child.GetComponentInChildren<Button>().interactable = true;
        }

        DrawCard();
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
        int maxEnemy = 1;
        if(day == 1)
        {
            maxEnemy = 1;
        }
        else if (day >= 2 && day <= 3)
        {
            maxEnemy = 2;
        }
        else if (day >= 4)
        {
            maxEnemy = 3;
        }

        //Debug.Log(maxEnemy);

        for (int i = 0; i < Random.Range(1, 4); i++)
        {
            GameObject e = Instantiate(allEnemies[Random.Range(0, maxEnemy)], spawnPoints[i]);
            e.transform.position = spawnPoints[i].transform.position;
            currentEnemies.Add(e.GetComponent<Enemy>());
        }
        
    }

    public void PanLeft()
    {
        Camera.main.GetComponent<Animator>().SetBool("Right", false);
        StartCoroutine(TransitionMusic(room, battle, 3f));
        sacrificeButton.interactable = false;
        newDayButton.interactable = false;
    }

    public void PanRight()
    {
        Camera.main.GetComponent<Animator>().SetBool("Right", true);
        StartCoroutine(TransitionMusic(battle, room, 3f));
        timeLeft = dayLength+3f;
    }

    public void EndDay()
    {
        foreach (Transform child in cardPanel)
            Destroy(child.gameObject);

        numofCards = 0;
        StartCoroutine(TransferBlood(3f));
        health += 50;
        



        dayEnded = true;
    }

    public IEnumerator TransitionMusic(AudioMixer var1, AudioMixer var2, float duration)
    {
        float time = 0;

        while (time < duration)
        {
            float vol2 = Mathf.Lerp(0f, -50f, time / duration);
            float vol1 = Mathf.Lerp(-50f, 0f, time / duration);
            time += Time.deltaTime;
            var1.SetFloat("musicVol", vol1);
            var2.SetFloat("musicVol", vol2);
            yield return null;
        }
    }

    public void NewDay()
    {
        DrawCard();
        DrawCard();
        DrawCard();
        //RespawnEnemies();
        GameObject e = Instantiate(allEnemies[0], spawnPoints[0]);
        e.transform.position = spawnPoints[0].transform.position;
        currentEnemies.Add(e.GetComponent<Enemy>());
        timeLeft = dayLength;

        Color col = ui.hunger.color;
        col.a = 0;
        ui.hunger.color = col;

        dayEnded = false;
        day++;
        StartPlayerTurn();
    }

    public void Sacrifice()
    {
        StartCoroutine(SacrificeHealth(2f));
    }

    public IEnumerator SacrificeHealth(float duration)
    {
        if (maxHealth > 10f && childHealth < 100f)
        {
            sacrificeButton.interactable = false;
            newDayButton.interactable = false;
            SFX.PlayOneShot(drain);
            maxHealth -= 10f;
            health = maxHealth;
            childHealth += 25f;

            float time = 0;
            float childBloodStart = childHealth;
            var emission = sacrificeParticles.emission;
            while (time < duration)
            {
                emission.enabled = true;
                childHealth = Mathf.Lerp(childBloodStart, childBloodStart + 25f, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            emission.enabled = false;
            childHealth = childBloodStart + 10f;
            sacrificeButton.interactable = true;
            newDayButton.interactable = true;
        }
    }

    public IEnumerator TransferBlood(float duration)
    {
        float time = 0;
        float collectedStart = collectedBlood;
        float childBloodStart = childHealth;
        var emission = roomParticles.emission;
        emission.enabled = true;
        SFX.PlayOneShot(blood);
        while(time<duration)
        {
            collectedBlood = Mathf.Lerp(collectedStart, 0, time / duration);
            childHealth = Mathf.Lerp(childBloodStart, childBloodStart + collectedStart, time / duration);
            if(collectedBlood == 0)
            {
                emission.enabled = false;
                time = duration;
            }
            time += Time.deltaTime;
            yield return null;
        }
        sacrificeButton.interactable = true;
        newDayButton.interactable = true;
        collectedBlood = 0f;
        childHealth = childBloodStart + collectedStart;
        emission.enabled = false;
        StartCoroutine(Hunger(3f));

    }

    public IEnumerator Hunger(float duration)
    {
        float time = 0;
        float childBloodStart = childHealth;

        while (time < 3f)
        {
            Color col = ui.hunger.color;
            col.a = Mathf.Lerp(0f, 1f, time / 3f);
            ui.hunger.color = col;

            childHealth = Mathf.Lerp(childBloodStart, childBloodStart - (5*day), time / duration);

            time += Time.deltaTime;
            yield return null;
        }

        childHealth = childBloodStart - (5 * day);
        if (childHealth >= 100f)
        {
            childHealth = 100f;
        }
    }

    public void BlockCards()
    {
        foreach (Transform child in GameManager.gameManager.cardPanel)
        {
            child.GetComponentInChildren<Button>().interactable = false;
        }
    }


    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void quitGame()
    {
        Application.Quit();
    }

}
