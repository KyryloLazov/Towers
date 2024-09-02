using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundsSurvived : MonoBehaviour
{
    public Text roundsText;

    private void OnEnable()
    {
        roundsText.text = PlayerStats.Stats.Rounds.ToString();
    }
}
