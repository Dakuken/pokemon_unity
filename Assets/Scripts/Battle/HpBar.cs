using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HpBar : MonoBehaviour
{
   [SerializeField] GameObject health;
   
   public bool IsUpdating { get; set; }
   
   public void SetHP(float hpNormalized){
      health.transform.localScale = new Vector3(hpNormalized, 1f);
   }

   public IEnumerator SetHPSmooth(float newHP)
   {
      IsUpdating = true;
      
      float curHP = health.transform.localScale.x;
      float changeAmt = curHP - newHP;

      while (curHP - newHP > Mathf.Epsilon)
      {
         curHP -= changeAmt * Time.deltaTime;
         SetHP(curHP);
         yield return null;
      }
      health.transform.localScale = new Vector3(newHP, 1f);
      
      IsUpdating = false;
   }
}
