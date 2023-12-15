using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName= "Pokemon", menuName = "Pokemon/Create new pokemon")]
public class PokemonBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite sprite;

    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;

    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;
    
    [SerializeField] int expYield;
    [SerializeField] GrowthRate growthRate;
    
    [SerializeField] int catchRate = 255;
    
    [SerializeField] List<LearnableMove> learnableMoves;
    
    public static int MaxNumOfMoves { get; set; } = 4;
    
    public int GetExpForLevel(int level){
        if (growthRate == GrowthRate.Fast)
        {
            return 4 * (level * level * level) / 5;
        }
        else if(growthRate == GrowthRate.MediumFast)
        {
            return level * level * level;
        }

        return -1;
    }

    public string Name{
        get { return name;}
    }

    public string Description{
        get { return description;}
    }

    public Sprite Sprite{
        get { return sprite;}
    }

    public PokemonType Type1{
        get { return type1;}
    }

    public PokemonType Type2{
        get { return type2;}
    }

    public int MaxHp{
        get { return maxHp;}
    }

    public int Attack{
        get { return attack;}
    }

    public int Defense{
        get { return defense;}
    }

    public int SpAttack{
        get { return spAttack;}
    }

    public int SpDefense{
        get { return spDefense;}
    }

    public int Speed{
        get { return speed;}
    }
    
    public List<LearnableMove> LearnableMoves{
        get { return learnableMoves;}
    }
    
    public int CatchRate{
        get { return catchRate;} 
    }
    
    public int ExpYield{
        get { return expYield;}
    }
    
    public GrowthRate GrowthRate{
        get { return growthRate;}
    }
}

[System.Serializable]

public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;
    
    public MoveBase Base{
        get { return moveBase;}
    }
    
    public int Level{
        get { return level;}
    }
}

public enum PokemonType{
    None,
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fighting,
    Poison,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon,
    Dark,
    Steel,
    Fairy
}

public enum GrowthRate{
    Fast,
    MediumFast,
}

public enum Stat
{
    Attack,
    Defense,
    SpAttack,
    SpDefense,
    Speed,
    Accuracy,
    Evasion
}

public class TypeChart
{
    static float[][] chart =
    {
                /* types                    Nor   Fire   WAT    Elec    Grass    Ice    Fight   Pois    Grou    Fly     Psy     Bug     Rock    Ghost   Drag    Dark    Steel   Fairy
                /*NORMAL*/      new float[] {1f,  1f,    1f,     1f,     1f,     1f,     1f,     1f,    1f,     1f,     1f,     1f,     0.5f,   0f,     1f,     1f,     0.5f,   1f },
                /*FIRE*/        new float[] {1f,  0.5f,  0.5f,   1f,     2f,     2f,     1f,     1f,    1f,     1f,     1f,     2f,     0.5f,   1f,     0.5f,   1f,     2f,     1f },
                /*WATER*/       new float[] {1f,  2f,    0.5f,   1f,     0.5f,   1f,     1f,     1f,    2f,     1f,     1f,     1f,     2f,     1f,     0.5f,   1f,     1f,     1f },
                /*ELECTRIC*/    new float[] {1f,  1f,    2f,     0.5f,   0.5f,   1f,     1f,     1f,    0f,     2f,     1f,     1f,     1f,     1f,     0.5f,   1f,     1f,     1f },
                /*GRASS*/       new float[] {1f,  0.5f,  2f,     1f,     0.5f,   1f,     1f,     0.5f,  2f,     0.5f,   1f,     0.5f,   2f,     1f,     0.5f,   1f,     0.5f,   1f },
                /*ICE*/         new float[] {1f,  0.5f,  0.5f,   1f,     2f,     0.5f,   1f,     1f,    2f,     2f,     1f,     1f,     1f,     1f,     2f,     1f,     0.5f,   1f },
                /*FIGHTING*/    new float[] {2f,  1f,    1f,     1f,     1f,     2f,     1f,     0.5f,  1f,     0.5f,   0.5f,   0.5f,   2f,     0f,     1f,     2f,     2f,     0.5f },
                /*POISON*/      new float[] {1f,  1f,    1f,     1f,     2f,     1f,     1f,     0.5f,  0.5f,   1f,     1f,     1f,     0.5f,   0.5f,   1f,     1f,     0f,     2f },
                /*GROUND*/      new float[] {1f,  2f,    1f,     2f,     0.5f,   1f,     1f,     2f,    1f,     0f,     1f,     0.5f,   2f,     1f,     1f,     1f,     2f,     1f },
                /*FLYING*/      new float[] {1f,  1f,    1f,     0.5f,   2f,     1f,     2f,     1f,    1f,     1f,     1f,     2f,     0.5f,   1f,     1f,     1f,     0.5f,   1f },
                /*PSYCHIC*/     new float[] {1f,  1f,    1f,     1f,     1f,     1f,     2f,     2f,    1f,     1f,     0.5f,   1f,     1f,     1f,     1f,     0f,     0.5f,   1f },
                /*BUG*/         new float[] {1f,  0.5f,  1f,     1f,     2f,     1f,     0.5f,   0.5f,  1f,     0.5f,   2f,     1f,     1f,     0.5f,   1f,     2f,     0.5f,   0.5f },
                /*ROCK*/        new float[] {1f,  2f,    1f,     1f,     1f,     2f,     0.5f,   1f,    0.5f,   2f,     1f,     2f,     1f,     1f,     1f,     1f,     0.5f,   1f },
                /*GHOST*/       new float[] {0f,  1f,    1f,     1f,     1f,     1f,     1f,     1f,    1f,     1f,     2f,     1f,     1f,     2f,     1f,     0.5f,   1f,     1f },
                /*DRAGON*/      new float[] {1f,  1f,    1f,     1f,     1f,     1f,     1f,     1f,    1f,     1f,     1f,     1f,     1f,     1f,     2f,     1f,     0.5f,   0f },
                /*DARK*/        new float[] {1f,  1f,    1f,     1f,     1f,     1f,     0.5f,   1f,    1f,     1f,     2f,     1f,     1f,     2f,     1f,     0.5f,   1f,     0.5f },
                /*STEEL*/       new float[] {1f,  0.5f,  0.5f,   0.5f,   1f,     2f,     1f,     1f,    1f,     1f,     1f,     1f,     2f,     1f,     1f,     1f,     0.5f,   2f },
                /*FAIRY*/       new float[] {1f,  0.5f,  1f,     1f,     1f,     1f,     2f,     0.5f,  1f,     1f,     1f,     1f,     1f,     1f,     2f,     2f,     0.5f,   1f }
    };
    
    public static float GetEffectiveness(PokemonType attackType, PokemonType defenseType){
        if(attackType == PokemonType.None || defenseType == PokemonType.None){
            return 1;
        }
        
        int row = (int)attackType - 1;
        int col = (int)defenseType - 1;
        
        return chart[row][col];
    }
}