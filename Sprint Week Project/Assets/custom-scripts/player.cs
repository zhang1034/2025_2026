using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
  public float BestLapTime { get; private set; } = Mathf.Infinity;

  public float LastLapTime { get; private set; } = 0;
  public float CurrentLapTime { get; private set; } = 0;
  public int CurrentLap { get; private set; } = 0;

  private float lapTimerTimestamp;
  private int lastCheckpointPassed = 0;

  private Transform checkpointsParent;
  private int checkpointCount;
  private int checkpointLayer;
  // Start is called before the first frame update
  void Awake() {
    checkpointsParent = GameObject.Find("Checkpoints").transform;
    checkpointCount = checkpointsParent.childCount;
    checkpointLayer = LayerMask.NameToLayer("Checkpoint");
    // carController = GetComponent<carController>();
  }

  void StartLap() {
    Debug.Log("StartLap!");
    CurrentLap++;
    lastCheckpointPassed = 1;
    lapTimerTimestamp = Time.time;
  }

  void EndLap() {
    LastLapTime = Time.time - lapTimerTimestamp;
    BestLapTime = Mathf.Min(LastLapTime, BestLapTime);
    Debug.Log("Lap time was " + LastLapTime);
    Debug.Log("Best time is " + BestLapTime);
  }

  void OnTriggerEnter(Collider collider) {
    if(collider.gameObject.layer == checkpointLayer) {
      // only care about colliding with the layer that has checkpoints
      if(collider.gameObject.name == "1") {
        // crossed the finish line
        if(lastCheckpointPassed == checkpointCount)
          EndLap();
        // regardless of how the lap is finished, a new lap should start
        StartLap();
      }
      // any of the other checkpoints, make sure they're in order
      else if(collider.gameObject.name == (lastCheckpointPassed+1).ToString())
        lastCheckpointPassed++;
    }
  }

  // Update is called once per frame
  void Update() {
    CurrentLapTime = lapTimerTimestamp > 0 ? Time.time - lapTimerTimestamp : 0;

  }
}
