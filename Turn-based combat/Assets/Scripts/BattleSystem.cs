﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{

	public GameObject playerPrefab;
	public GameObject enemyPrefab;

	public Transform playerBattleStation;
	public Transform enemyBattleStation;

	Unit playerUnit;
	Unit enemyUnit;

	public Text dialogueText;

	public BattleHUD playerHUD;
	public BattleHUD enemyHUD;

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

		GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
		enemyUnit = enemyGO.GetComponent<Unit>();

		dialogueText.text = "A wild " + enemyUnit.unitName + " approaches...";

		playerHUD.SetHUD(playerUnit);
		enemyHUD.SetHUD(enemyUnit);

		yield return new WaitForSeconds(2f);

		state = BattleState.PLAYERTURN;
		PlayerTurn();
	}

	IEnumerator LevelUp() 
	{
		playerUnit.maxHP = playerUnit.maxHP  + (int) ((double) playerUnit.maxHP * 1.05);
		playerUnit.currentHP = playerUnit.maxHP;
		playerUnit.unitLevel += 1;

		enemyUnit.maxHP = enemyUnit.maxHP + (int) ((double) enemyUnit.maxHP * 1.05);
		enemyUnit.currentHP = enemyUnit.maxHP;
		enemyUnit.unitLevel += 1;

		dialogueText.text = "Level up!";
		
		playerHUD.SetHUD(playerUnit);
		enemyHUD.SetHUD(enemyUnit);

		yield return new WaitForSeconds(2f);

		state = BattleState.PLAYERTURN;
		PlayerTurn();
	}

	IEnumerator PlayerAttack()
	{
		bool isDead = enemyUnit.TakeDamage(playerUnit.damage);

		enemyHUD.SetHP(enemyUnit.currentHP);
		dialogueText.text = "The attack is successful!";

		yield return new WaitForSeconds(2f);

		if(isDead)
		{
			state = BattleState.WON;
			StartCoroutine(EndBattle());
		} else
		{
			state = BattleState.ENEMYTURN;
			StartCoroutine(EnemyTurn());
		}
	}

	IEnumerator PlayerSpecialAttack()
	{
		bool isDead = enemyUnit.TakeDamage(playerUnit.specialDamage);
		playerUnit.ConsumeSpecialMove();

		enemyHUD.SetHP(enemyUnit.currentHP);
		dialogueText.text = "You used a special move!";

		yield return new WaitForSeconds(2f);

		dialogueText.text = "You have " + playerUnit.specialMovesCount + " special moves left";
		yield return new WaitForSeconds(2f);

		if(isDead)
		{
			state = BattleState.WON;
			StartCoroutine(EndBattle());
		} else
		{
			state = BattleState.ENEMYTURN;
			StartCoroutine(EnemyTurn());
		}
	}

	IEnumerator EnemyTurn()
	{
		dialogueText.text = enemyUnit.unitName + " attacks!";

		yield return new WaitForSeconds(1f);

		bool isDead = playerUnit.TakeDamage(enemyUnit.damage);

		playerHUD.SetHP(playerUnit.currentHP);

		yield return new WaitForSeconds(1f);

		if(isDead)
		{
			state = BattleState.LOST;
			StartCoroutine(EndBattle());
		} else
		{
			state = BattleState.PLAYERTURN;
			PlayerTurn();
		}

	}

	IEnumerator EndBattle()
	{
		if(state == BattleState.WON)
		{
	
			dialogueText.text = "You won the battle!";
			yield return new WaitForSeconds(1f);
			StartCoroutine(LevelUp());
		} else if (state == BattleState.LOST)
		{
			dialogueText.text = "You were defeated.";
		}
	}

	void PlayerTurn()
	{
		dialogueText.text = "Choose an action:";
	}

	IEnumerator PlayerHeal()
	{
		playerUnit.Heal(5);

		playerHUD.SetHP(playerUnit.currentHP);
		dialogueText.text = "You feel renewed strength!";

		yield return new WaitForSeconds(2f);

		state = BattleState.ENEMYTURN;
		StartCoroutine(EnemyTurn());
	}

	public void OnAttackButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;

		StartCoroutine(PlayerAttack());
	}

	public void OnHealButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;

		StartCoroutine(PlayerHeal());
	}

	public void OnSpecialAttack() 
	{
		if (state != BattleState.PLAYERTURN)
			return;
		
		StartCoroutine(PlayerSpecialAttack());
	}
	

}
