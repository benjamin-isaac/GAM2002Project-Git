using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMY1TURN, ENEMY2TURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{

    public GameObject playerPrefab;
    public GameObject enemy1Prefab;
    public GameObject enemy2Prefab;

    public Transform playerBattleStation;
    public Transform enemy1BattleStation;
    public Transform enemy2BattleStation;

    Unit playerUnit;
    Unit enemy1Unit;
    Unit enemy2Unit;

    public Text dialogueText;

    public BattleHUD playerHUD;
    public BattleHUD enemy1HUD;
    public BattleHUD enemy2HUD;

    public BattleState state;

    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<Unit>();

        GameObject enemy1GO = Instantiate(enemy1Prefab, enemy1BattleStation);
        enemy1Unit = enemy1GO.GetComponent<Unit>();

        GameObject enemy2GO = Instantiate(enemy2Prefab, enemy2BattleStation);
        enemy2Unit = enemy2GO.GetComponent<Unit>();

        playerHUD.SetHUD(playerUnit);
        enemy1HUD.SetHUD(enemy1Unit);
        enemy2HUD.SetHUD(enemy2Unit);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack()
    {
        bool isDead = enemy1Unit.TakeDamage(playerUnit.damage);

        enemy1HUD.SetHP(enemy1Unit.currentHP);
        dialogueText.text = "You hit enemy " + enemy1Unit.unitName + "!";

        yield return new WaitForSeconds(2f);

        if(isDead)
        {
            dialogueText.text = "You killed enemy " + enemy1Unit.unitName + "!";
            yield return new WaitForSeconds(2f);
            state = BattleState.WON;
            EndBattle();
        } else
        {
            state = BattleState.ENEMY1TURN;
            StartCoroutine(Enemy1Turn());
        }
    }

    IEnumerator PlayerAttack2()
    {
        bool isDead = enemy2Unit.TakeDamage(playerUnit.damage);

        enemy2HUD.SetHP(enemy2Unit.currentHP);
        dialogueText.text = "You hit enemy " + enemy2Unit.unitName + "!";

        yield return new WaitForSeconds(2f);

        if(isDead)
        {
            dialogueText.text = "You killed enemy " + enemy2Unit.unitName + "!";
            yield return new WaitForSeconds(2f);
            state = BattleState.ENEMY1TURN;
            StartCoroutine(Enemy1Turn());
        } else
        {
            state = BattleState.ENEMY2TURN;
            StartCoroutine(Enemy2Turn());
        }
    }

    IEnumerator Enemy2Turn()
    {
        dialogueText.text = enemy2Unit.unitName + " attacks!";

        yield return new WaitForSeconds(1f);

        bool isDead = playerUnit.TakeDamage(enemy2Unit.damage);

        playerHUD.SetHP(playerUnit.currentHP);

        yield return new WaitForSeconds(1f);

        if(isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        } else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    IEnumerator Enemy1Turn()
    {
        dialogueText.text = enemy1Unit.unitName + " attacks!";

        yield return new WaitForSeconds(1f);

        bool isDead = playerUnit.TakeDamage(enemy1Unit.damage);

        playerHUD.SetHP(playerUnit.currentHP);

        yield return new WaitForSeconds(1f);

        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        } else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }


    void EndBattle()
    {
        if(state == BattleState.WON)
        {
            dialogueText.text = "Enemies defeated!";
        } else if (state == BattleState.LOST)
        {
            dialogueText.text = "You were defeated.";
        }
    }

    void PlayerTurn()
    {
        dialogueText.text = "Choose an action.";
    }

    IEnumerator PlayerHeal()
    {
        playerUnit.Heal(10);

        playerHUD.SetHP(playerUnit.currentHP);
        dialogueText.text = "You gained 10HP!";

        yield return new WaitForSeconds(2f);

        state = BattleState.ENEMY1TURN;
        StartCoroutine(Enemy1Turn());
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerAttack());
    }

    public void OnAttack2Button()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerAttack2());
    }

    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerHeal());
    }
}
