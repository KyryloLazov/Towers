using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Stats;
    public static int Money;
    public int StartMoney = 400;
    public int Rounds;
    public Text LivesText;
    public Text MoneyText;

    public static int Lives;
    public int StartLives = 20;
    private void Awake()
    {
        Stats = this;
        Money = StartMoney;
        Lives = StartLives;

        Rounds = 0;

        LivesText.text = PlayerStats.Lives.ToString() + " LIVES";
        MoneyText.text = PlayerStats.Money.ToString() + "$";
    }

    public void Damage()
    {
        Lives--;
        if (Lives <= 0)
        {
            LivesText.text = "Game Over";
            return;
        }        
        LivesText.text = PlayerStats.Lives.ToString() + " LIVES";
    }

    public void Spend(int amount)
    {
        Money -= amount;
        MoneyText.text = PlayerStats.Money.ToString() + "$";
    }

    public void Add(int amount)
    {
        Money += amount;
        MoneyText.text = PlayerStats.Money.ToString() + "$";
    }
}
