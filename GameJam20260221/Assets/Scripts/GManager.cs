using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;
 public class GManager : MonoBehaviour
 {
    public static GManager instance = null;

    public int lineScore;

    public int killScore;

    public int flappyScore;

    public int funeScore;

     private void Awake()
     {
          if (instance == null)
          {
              instance = this;
              DontDestroyOnLoad(this.gameObject);
          }
          else
          {
              Destroy(this.gameObject);
          }
     }
 }