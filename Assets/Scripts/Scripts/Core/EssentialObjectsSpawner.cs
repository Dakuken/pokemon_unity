using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialObjectsSpawner : MonoBehaviour
{
   [SerializeField] GameObject essentialObjectsPrefab;

   private void Awake()
   {
      var existingObjects = FindObjectsOfType<EssentialObjects>();
      if (existingObjects.Length == 0)
      {
         var spawnPosition = new Vector3(0, 0, 0);
         
         var grid = FindObjectOfType<Grid>();
         if (grid != null)
            spawnPosition = grid.transform.position;
            
         Instantiate(essentialObjectsPrefab, spawnPosition, Quaternion.identity);
      }
   }
}
