using UnityEngine.UI;  // required for text
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
  public GameObject UIRacePanel;

  public Text UITextCurrentLap;
  public Text UITextCurrentTime;
  public Text UITextLastLapTime;
  public Text UITextBestLapTime;

  public player UpdateUIForPlayer;

  private int currentLap = -1;
  private float currentLapTime;
  private float lastLapTime;
  private float bestLapTime;

  void Update() {
    if(UpdateUIForPlayer != null) {
      if(UpdateUIForPlayer.CurrentLap != currentLap) {
        currentLap = UpdateUIForPlayer.CurrentLap;
        UITextCurrentLap.text = $"LAP: {currentLap}";
      }

      // update the times
      if(UpdateUIForPlayer.CurrentLapTime != currentLapTime) {
        currentLapTime = UpdateUIForPlayer.CurrentLapTime;
        UITextCurrentTime.text = $"TIME: {(int)currentLapTime / 60}:{(currentLapTime % 60) :00.00}";
      }
      if(UpdateUIForPlayer.LastLapTime != lastLapTime) {
        lastLapTime = UpdateUIForPlayer.LastLapTime;
        UITextLastLapTime.text = $"LAST: {(int)lastLapTime / 60}:{(lastLapTime % 60):00.00}";
      }
      if(UpdateUIForPlayer.BestLapTime != bestLapTime) {
        bestLapTime = UpdateUIForPlayer.BestLapTime;
        UITextBestLapTime.text = bestLapTime < 1000000 ? $"BEST: {(int)bestLapTime / 60}:{(bestLapTime % 60):00.00}" : "BEST: ---";
      }
    }


  }
}
