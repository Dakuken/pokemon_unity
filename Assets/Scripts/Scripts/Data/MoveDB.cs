using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDB 
{
    static Dictionary<string, MoveBase> moves;

    public static void Init()
    {
        moves = new Dictionary<string, MoveBase>();
        
        var MoveArray= Resources.LoadAll<MoveBase>("");
        foreach (var move in MoveArray)
        {
            if (moves.ContainsKey(move.Name))
            {
                Debug.LogError("There are two or more moves with the same name: " + move.Name);
                continue;
            }
            
            moves[move.Name] = move;
        }
        
    }
    
    public static MoveBase GetMoveByName(string name)
    {
        if (!moves.ContainsKey(name))
        {
            Debug.LogError("Couldn't find move with name: " + name);
            return null;
        }
        return moves[name];
    }
}
