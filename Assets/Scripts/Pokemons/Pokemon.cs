using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Pokemon
{
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;

    public Pokemon(PokemonBase pBase, int pLevel)
    {
        _base = pBase;
        level = pLevel;
        Init();
    }
    
    public PokemonBase Base {
        get {
            return _base;
        } 
    }

    public int Level
    {
        get {
            return level;
        }
    }

    public int Exp { get; set; }
    public int HP { get; set; }
    public List<Move> Moves { get; set; }
    public Move CurrentMove { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }
    public Condition Status { get; private set; }
    public Condition VolatileStatus { get; private set; }
    public int StatusTime { get; set; }
    public int VolatileStatusTime { get; set; }
    public Queue<string> StatusChanges { get; private set; }
    public event System.Action OnStatusChanged; 
    public event System.Action OnHpChanged; 

    

    public void Init(){
        
        Moves = new List<Move>();
        foreach(var move in Base.LearnableMoves){
            if(move.Level <= Level){
                Moves.Add(new Move(move.Base));
            }
            if(Moves.Count >= PokemonBase.MaxNumOfMoves){
                break;
            }
        }
        
        Exp = Base.GetExpForLevel(Level);
        
        CalculateStats();
        HP = MaxHp;
        
        StatusChanges = new Queue<string>();
        ResetStatBoost();
        Status = null;
        VolatileStatus = null;
    }
    
    public Pokemon(PokemonSaveData saveData)
    {
        _base = PokemonDB.GetObjectByName(saveData.name);
        level = saveData.level;
        Exp = saveData.exp;
        HP = saveData.HP;
        
        if(saveData.status != null) 
            Status = ConditionsDB.Conditions[saveData.status.Value];
        else 
            Status = null;
        
        Moves = saveData.moves.Select(s => new Move(s)).ToList();
        
        CalculateStats();
        StatusChanges = new Queue<string>();
        ResetStatBoost();
        VolatileStatus = null;
    }
    
    public PokemonSaveData GetSaveData(){
        var saveData = new PokemonSaveData(){
            name = Base.name,
            HP = HP,
            level = level,
            exp = Exp,
            status = Status?.Id,
            moves = Moves.Select(m => m.GetSaveData()).ToList(),
        };
        
        return saveData;
    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((Base.SpDefense * Level) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5);

        int oldMaxHp = MaxHp;
        MaxHp = Mathf.FloorToInt((Base.MaxHp * Level) / 100f) + 10 + Level;
        
        if(oldMaxHp != 0)
            HP += MaxHp - oldMaxHp;
    }
    

    void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            { Stat.Attack, 0 },
            { Stat.Defense, 0 },
            { Stat.SpAttack, 0 },
            { Stat.SpDefense, 0 },
            { Stat.Speed, 0 },
            { Stat.Accuracy, 0 },
            { Stat.Evasion, 0 },
        };
    }
    
    int GetStat(Stat stat){
        int statVal = Stats[stat];
        
        int boost = StatBoosts[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };
        
        if (boost >= 0)
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        else
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);
        
        return statVal;
    }

    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;
            
            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);
            
            if (boost > 0)
                StatusChanges.Enqueue($"{Base.Name}'s {stat} rose!");
            else
                StatusChanges.Enqueue($"{Base.Name}'s {stat} fell!");
            
        }
    }
    
    public bool CheckForLevelUp()
    {
        if (Exp > Base.GetExpForLevel(level + 1))
        {
            ++level;
            CalculateStats();
            return true;
        }
        return false;
    }
    
    public LearnableMove GetLearnableMovesAtCurrentLevel()
    {
        var currentLevel = Level;
        return Base.LearnableMoves.Where(x => x.Level == currentLevel).FirstOrDefault();
    }
    
    public void LearnMove(LearnableMove moveToLearn)
    {
        if (Moves.Count > PokemonBase.MaxNumOfMoves)
            return;
        
        Moves.Add(new Move(moveToLearn.Base));
    }

    public Evolution CheckForEvolution()
    {
        return Base.Evolutions.FirstOrDefault(e => e.RequiredLevel <= level);
    }

    public void UpdateMoves()
    {
        Moves.Clear();

        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
            {
                Moves.Add(new Move(move.Base));
            }

            if (Moves.Count >= PokemonBase.MaxNumOfMoves)
            {
                break;
            }
        }
    }
    public void Evolve(Evolution evolution)
    {
        _base = evolution.EvolvesInto;
        CalculateStats();
        UpdateMoves();
    }

    public void Heal()
    {
        HP = MaxHp;
        OnHpChanged?.Invoke();
        
        CureStatus();
    }
    
    public float GetNormalizedExp()
    {
        int currentLevelExp = Base.GetExpForLevel(Level);
        int nextLevelExp = Base.GetExpForLevel(Level + 1);
        
        float normalizedExp = (float)(Exp - currentLevelExp) / (nextLevelExp - currentLevelExp);
        return Mathf.Clamp01(normalizedExp);
    }
    public int Attack{
        get { return GetStat(Stat.Attack);}
    }

    public int Defense{
        get { return GetStat(Stat.Defense);}
    }

    public int SpAttack{
        get { return GetStat(Stat.SpAttack);}
    }

    public int SpDefense{
        get { return GetStat(Stat.SpDefense);}
    }

    public int Speed{
        get { return GetStat(Stat.Speed);}
    }

    public int MaxHp{
        get; private set;
    }

    public DamageDetails TakeDamage(Move move, Pokemon attacker)
    {
        float critical = 1f;
        if(Random.value * 100f < 6.25f){
            critical = 1.5f;
        }

        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1) *
                     TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type2);
        
        var damageDetails = new DamageDetails()
        {
            Critical = critical,
            TypeEffectiveness = type,
            Fainted = false
        };
        
        float attack = (move.Base.Category == MoveCategory.Special) ? attacker.SpAttack : attacker.Attack;
        float defense = (move.Base.Category == MoveCategory.Special) ? SpDefense : Defense;
        
        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attack / defense);
        int damage = Mathf.FloorToInt(d * modifiers);

        DecreaseHp(damage);
        
        return damageDetails;
    }
    
    public void DecreaseHp(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHp);
        OnHpChanged?.Invoke();
    }
    
    public void IncreaseHp(int amount)
    {
        HP = Mathf.Clamp(HP + amount, 0, MaxHp);
        OnHpChanged?.Invoke();
    }
    
    public void SetStatus(ConditionID conditionId)
    {
        if(Status != null) return;
        
       Status = ConditionsDB.Conditions[conditionId];
       Status?.OnStart?.Invoke(this);
       StatusChanges.Enqueue($"{Base.Name} {Status.StartMessage}");
       OnStatusChanged?.Invoke();
    }
    
    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }
    
    public void SetVolatileStatus(ConditionID conditionId)
    {
        if(VolatileStatus != null) return;
        
        VolatileStatus = ConditionsDB.Conditions[conditionId];
        VolatileStatus?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {VolatileStatus.StartMessage}");
    }
    
    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }

    public Move GetRandomMove()
    {
        var movesWithPP = Moves.Where(x => x.PP > 0).ToList();
        
        int r = Random.Range(0, movesWithPP.Count);
        return movesWithPP[r];
    }
    
    public bool OnBeforeMove()
    {
        bool canPerformMove = true;
        if (Status?.OnBeforeMove != null)
            if (!Status.OnBeforeMove(this))
                canPerformMove = false;
        
        if (VolatileStatus?.OnBeforeMove != null)
            if (!VolatileStatus.OnBeforeMove(this))
                canPerformMove = false;
        
        return canPerformMove;
    }
    
    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }

    public void OnBattleOver()
    {
        VolatileStatus = null;
        ResetStatBoost();
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    
    public float Critical { get; set; }
    
    public float TypeEffectiveness { get; set; }
}

[System.Serializable]
public class PokemonSaveData
{
    public string name;
    public int HP;
    public int level;
    public int exp;
    public ConditionID? status;
    public List<MoveSaveData> moves;
}
