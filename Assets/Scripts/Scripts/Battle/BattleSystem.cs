using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { Start, ActionSelection, MoveSelection, RunningTurn, Busy, PartyScreen, BattleOver, AboutToUse, MoveForget, Bag}
public enum BattleAction { Move, SwitchPokemon, UseItem, Run }
public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] Image playerImage;
    [SerializeField] Image trainerImage;
    [SerializeField] GameObject pokeballSprite;
    [SerializeField] MoveSelectionUI moveSelectionUI;
    [SerializeField] InventoryUI inventoryUI;
    
    [Header("Audio")]
    [SerializeField] AudioClip wildBattleBGM;
    [SerializeField] AudioClip trainerBattleBGM;
    [SerializeField] AudioClip victoryBGM;
    
    public event Action<bool> OnBattleOver;
    
    BattleState state;
    int currentAction;
    int currentMove;
    bool aboutToUseChoice = true;
    
    PokemonParty playerParty;
    PokemonParty trainerParty;
    Pokemon wildPokemon;
    
    bool isTrainerBattle = false;
    PlayerController player;
    TrainerController trainer;
    
    int escapeAttempts;
    MoveBase moveToLearn;
    
    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        player = playerParty.GetComponent<PlayerController>();
        isTrainerBattle = false;
        
        AudioManager.i.PlayMusic(wildBattleBGM);
        
        StartCoroutine(SetupBattle());
    }
    
    public void StartTrainerBattle(PokemonParty playerParty, PokemonParty trainerParty)
    {
        this.playerParty = playerParty;
        this.trainerParty = trainerParty;
        
        isTrainerBattle = true;
        player = playerParty.GetComponent<PlayerController>();
        trainer = trainerParty.GetComponent<TrainerController>();
        
        AudioManager.i.PlayMusic(trainerBattleBGM);
        
        StartCoroutine(SetupBattle());
    }
    
    public IEnumerator SetupBattle(){

        playerUnit.Clear();
        enemyUnit.Clear();
        
        if (!isTrainerBattle)
        {
            playerUnit.Setup(playerParty.GetHealthyPokemon());
            enemyUnit.Setup(wildPokemon);
            
            dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
            yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared.");
        }
        else
        {
            playerUnit.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(false);
            
            playerImage.gameObject.SetActive(true);
            trainerImage.gameObject.SetActive(true);
            playerImage.sprite = player.Sprite;
            trainerImage.sprite = trainer.Sprite;
            
            yield return dialogBox.TypeDialog($"{trainer.Name} wants to battle!");
            
            //pokemon adverse rentre
            trainerImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);
            var enemyPokemon = trainerParty.GetHealthyPokemon();
            enemyUnit.Setup(enemyPokemon);
            yield return dialogBox.TypeDialog($"{trainer.Name} sends out {enemyPokemon.Base.Name}!");
            
            //pokemon joueur rentre
            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);
            var playerPokemon = playerParty.GetHealthyPokemon();
            playerUnit.Setup(playerPokemon);
            yield return dialogBox.TypeDialog($"Go {playerPokemon.Base.Name}!");
            dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        }
        escapeAttempts = 0;
        partyScreen.Init();
        ActionSelection();
    }
    
    
    void BattleOver(bool won){
        state = BattleState.BattleOver;
        playerParty.Pokemons.ForEach(p => p.OnBattleOver());
        playerUnit.Hud.ClearData();
        enemyUnit.Hud.ClearData();
        OnBattleOver(won);
    }
    
    void ActionSelection(){
        state = BattleState.ActionSelection;
        dialogBox.SetDialog("Choose an action");
        dialogBox.EnableActionSelector(true);
    }

    void OpenPartyScreen()
    {
        partyScreen.CalledFrom = state;
        state = BattleState.PartyScreen;
        partyScreen.gameObject.SetActive(true);
    }

    void OpenBag()
    {
        inventoryUI.gameObject.SetActive(true);
        state = BattleState.Bag;
    }
    
    void MoveSelection(){
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    IEnumerator AboutToUse(Pokemon newPokemon)
    {
        state = BattleState.Busy;
        yield return dialogBox.TypeDialog( $"{trainer.Name} is about to use {newPokemon.Base.Name}. Do you want to change Pokemon?");
        
        state = BattleState.AboutToUse;
        dialogBox.EnableChoiceBox(true);
    }

    IEnumerator ChooseMoveToForget(Pokemon pokemon, MoveBase newMove)
    {
        state = BattleState.Busy;
        yield return dialogBox.TypeDialog($"Choose a move you want to forget");
        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(pokemon.Moves.Select(x => x.Base).ToList(), newMove);
        moveToLearn = newMove;
        
        state = BattleState.MoveForget;
    }

    IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;

        if (playerAction == BattleAction.Move)
        {
            playerUnit.Pokemon.CurrentMove = playerUnit.Pokemon.Moves[currentMove];
            enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetRandomMove();
            
            int playerMovePriority = playerUnit.Pokemon.CurrentMove.Base.Priority;
            int enemyMovePriority = enemyUnit.Pokemon.CurrentMove.Base.Priority;
            
            //Choix premier joueur
            bool playerGoesFirst = true;
            if(enemyMovePriority > playerMovePriority)
                playerGoesFirst = false;
            else if(enemyMovePriority == playerMovePriority)
                playerGoesFirst = playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed;

            var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;
            
            var secondPokemon = secondUnit.Pokemon;
            
            //Premier tour
            yield return RunMove(firstUnit, secondUnit, firstUnit.Pokemon.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BattleOver) yield break;
             
            if(secondPokemon.HP > 0){
                //Second tour
                yield return RunMove(secondUnit, firstUnit, secondUnit.Pokemon.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BattleOver) yield break;
            }
        }
        else
        {
            if (playerAction == BattleAction.SwitchPokemon)
            {
                var selectedMember = partyScreen.SelectedMember;
                state = BattleState.Busy;
                yield return SwitchPokemon(selectedMember);
            }
            else if (playerAction == BattleAction.UseItem)
            {
                dialogBox.EnableActionSelector(false);
            }
            else if (playerAction == BattleAction.Run)
            {
                yield return TryToEscape();
            }
            
            //enemy turn
            var enemyMove = enemyUnit.Pokemon.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if (state == BattleState.BattleOver) yield break;
        }
        
        if (state != BattleState.BattleOver)
            ActionSelection();
    }
    
    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        bool canRunMove = sourceUnit.Pokemon.OnBeforeMove();
        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.Pokemon);
            yield return sourceUnit.Hud.WaitForHpUpdate();
            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.Pokemon);
        
        move.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} used {move.Base.Name}");
        
        if(CheckIfMoveHits(move,sourceUnit.Pokemon,targetUnit.Pokemon))
        {
            sourceUnit.PlayAttackAnimation();
            AudioManager.i.PlaySFX(move.Base.Sound);
            
            yield return new WaitForSeconds(1f);
            targetUnit.PlayHitAnimation();
            AudioManager.i.PlaySFX(AudioId.Hit);

            if (move.Base.Category == MoveCategory.Status)
            {
                yield return RunMoveEffects(move.Base.Effects, sourceUnit.Pokemon, targetUnit.Pokemon,move.Base.Target);
            }
            else
            {
                var damageDetails  = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
                yield return targetUnit.Hud.WaitForHpUpdate();
                yield return ShowDamageDetails(damageDetails);
            }

            if (move.Base.Secondaries != null && move.Base.Secondaries.Count > 0 && targetUnit.Pokemon.HP > 0)
            {
                foreach (var secondary in move.Base.Secondaries)
                {
                    var rnd = UnityEngine.Random.Range(1, 101);
                    if (rnd <= secondary.Chance)
                        yield return RunMoveEffects(secondary, sourceUnit.Pokemon, targetUnit.Pokemon,secondary.Target);
                }
            }

            if (targetUnit.Pokemon.HP <= 0)
            {
                yield return HandlePokemonFainted(targetUnit);
            }
        }
        else
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name}'s attack missed");
        }
        
        
    }
    
    IEnumerator RunMoveEffects(MoveEffects effects, Pokemon source,Pokemon target,MoveTarget moveTarget)
    {
        //Stat Boosting
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
                source.ApplyBoosts(effects.Boosts);
            else
                target.ApplyBoosts(effects.Boosts);
        }

        //Status Condition
        if (effects.Status != ConditionID.none)
        {
            target.SetStatus(effects.Status);
        }
        
        //Volatile Status Condition
        if (effects.VolatileStatus != ConditionID.none)
        {
            target.SetVolatileStatus(effects.VolatileStatus);
        }
        
        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
       if(state == BattleState.BattleOver) yield break;
       yield return new WaitUntil(() => state == BattleState.RunningTurn);
        
        //status damage after turn
        sourceUnit.Pokemon.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Pokemon);
        yield return sourceUnit.Hud.WaitForHpUpdate();
        if (sourceUnit.Pokemon.HP <= 0)
        {
            yield return HandlePokemonFainted(sourceUnit);
            yield return new WaitUntil(() => state == BattleState.RunningTurn);
        }
    }
    
    bool CheckIfMoveHits(Move move, Pokemon source, Pokemon target)
    {
        if (move.Base.AlwaysHits)
            return true;
        
        float moveAccuracy = move.Base.Accuracy;
        int accuracy = source.StatBoosts[Stat.Accuracy];
        int evasion = target.StatBoosts[Stat.Evasion];
        var boostValues = new float[] {1f, 4f/3f, 5f/3f, 2f, 7f/3f, 8f/3f, 3f};
        
        if (accuracy > 0)
            moveAccuracy *= boostValues[accuracy];
        else
            moveAccuracy /= boostValues[-accuracy];
        
        if (evasion > 0)
            moveAccuracy /= boostValues[evasion];
        else
            moveAccuracy *= boostValues[-evasion];
        
        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
    }

    IEnumerator ShowStatusChanges(Pokemon pokemon)
    {
        while (pokemon.StatusChanges.Count > 0)
        {
            var message = pokemon.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }

    IEnumerator HandlePokemonFainted(BattleUnit faintedUnit)
    {
        yield return dialogBox.TypeDialog($"{faintedUnit.Pokemon.Base.Name} fainted");
        faintedUnit.PlayFaintAnimation();
        yield return new WaitForSeconds(2f);

        if (!faintedUnit.IsPlayerUnit)
        {
            bool battleWon = true;
            if (isTrainerBattle)
                battleWon = trainerParty.GetHealthyPokemon() == null;
            
            if (battleWon)
                AudioManager.i.PlayMusic(victoryBGM);
            
            int expYield = faintedUnit.Pokemon.Base.ExpYield;
            int enemyLevel = faintedUnit.Pokemon.Level;
            float trainerBonus = (isTrainerBattle) ? 1.5f : 1f;
            
            int expGain = Mathf.FloorToInt((expYield * enemyLevel * trainerBonus) / 7);
            playerUnit.Pokemon.Exp += expGain;
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} gained {expGain} exp.");
            yield return playerUnit.Hud.SetExpSmooth();


            while(playerUnit.Pokemon.CheckForLevelUp())
            {
                playerUnit.Hud.SetLevel();
                yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} grew to level {playerUnit.Pokemon.Level}.");

                var newMove = playerUnit.Pokemon.GetLearnableMovesAtCurrentLevel();
                if (newMove != null)
                {
                    if (playerUnit.Pokemon.Moves.Count < PokemonBase.MaxNumOfMoves)
                    {
                        playerUnit.Pokemon.LearnMove(newMove);
                        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} learned {newMove.Base.Name}.");
                        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
                    }
                    else
                    {
                        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} wants to learn {newMove.Base.Name}.");
                        yield return dialogBox.TypeDialog($"But it can't learn more than {PokemonBase.MaxNumOfMoves} moves.");
                        yield return ChooseMoveToForget(playerUnit.Pokemon, newMove.Base);
                        yield return new WaitUntil(() => state != BattleState.MoveForget);
                        yield return new WaitForSeconds(2f);
                    }
                }
                
                yield return playerUnit.Hud.SetExpSmooth(true);
            }



            yield return new WaitForSeconds(1f);
        }
        
        CheckForBattleOver(faintedUnit);
    }
    
    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            var nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null)
                OpenPartyScreen();
            else
                BattleOver(false);
        }
        else
        {
            if (!isTrainerBattle)
                BattleOver(true);
            else
            {
                var nextPokemon = trainerParty.GetHealthyPokemon();
                if (nextPokemon != null)
                    StartCoroutine(AboutToUse(nextPokemon));
                else
                    BattleOver(true);
            }
           
        }
    }
    
    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
        {
            yield return dialogBox.TypeDialog("A critical hit!");
        }

        if (damageDetails.TypeEffectiveness > 1f)
        {
            yield return dialogBox.TypeDialog("It's super effective!");
        }
        else if (damageDetails.TypeEffectiveness < 1f)
        {
            yield return dialogBox.TypeDialog("It's not very effective!");
        }
    }

    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }
        else if (state == BattleState.Bag)
        {
            Action onBack = () =>
            {
                inventoryUI.gameObject.SetActive(false);
                state = BattleState.ActionSelection;
            };
            
            Action<ItemBase> onItemUsed = (ItemBase usedItem) =>
            {
                StartCoroutine(OnItemUsed(usedItem));
            };
            
            inventoryUI.HandleUpdate(onBack,onItemUsed);
        }
        else if (state == BattleState.AboutToUse)
        {
            HandleAboutToUse();
        }
        else if (state == BattleState.MoveForget)
        {
            Action<int> onMoveSelected = (moveIndex) =>
            {
                moveSelectionUI.gameObject.SetActive(false);
                if (moveIndex == PokemonBase.MaxNumOfMoves)
                {
                    StartCoroutine(dialogBox.TypeDialog($" {playerUnit.Pokemon.Base.Name} did not learn {moveToLearn.Name}"));
                }
                else
                {
                    var selectedMove = playerUnit.Pokemon.Moves[moveIndex].Base;
                    StartCoroutine(dialogBox.TypeDialog($" {playerUnit.Pokemon.Base.Name} forgot {selectedMove.Name} and learned {moveToLearn.Name}"));
                    playerUnit.Pokemon.Moves[moveIndex] = new Move(moveToLearn);
                }
                
                moveToLearn = null;
                state = BattleState.RunningTurn;
            };
            
            moveSelectionUI.HandleMoveSelection(onMoveSelected);
        }
        
    }

    void HandleActionSelection() {
        
        if (Input.GetKeyDown(KeyCode.RightArrow))
        { 
            ++currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            --currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentAction += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentAction -= 2;
        }
        
        currentAction = Mathf.Clamp(currentAction, 0, 3);
        
        dialogBox.UpdateActionSelection(currentAction);
        
        if(Input.GetKeyDown(KeyCode.Z)){
            if(currentAction == 0){
                //Fight
                MoveSelection();
            }
            else if(currentAction == 1){
                //Bag
                OpenBag();
            }
            else if(currentAction == 2){
                //Pokemon
                OpenPartyScreen();
            }
            else if(currentAction == 3){
                //Run
                StartCoroutine(RunTurns(BattleAction.Run));
            }
        }
    }
    
    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        { 
            ++currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            --currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentMove -= 2;
        }
        
        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Pokemon.Moves.Count - 1);
        
        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var move = playerUnit.Pokemon.Moves[currentMove];
            if (move.PP == 0)
                return;
            
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(RunTurns(BattleAction.Move));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }
        
    }

    IEnumerator OnItemUsed(ItemBase usedItem)
    {
        state = BattleState.Busy;
        inventoryUI.gameObject.SetActive(false);

        if (usedItem is PokeballItem)
        {
            yield return ThrowPokeball((PokeballItem)usedItem );
        }
                
        StartCoroutine(RunTurns(BattleAction.UseItem));
    }

    void HandlePartySelection()
    {

        Action onSelected = () =>
        {
            var selectedMember = partyScreen.SelectedMember;
            if(selectedMember.HP <= 0){
                partyScreen.SetMessageText("You can't send out a fainted Pokemon");
                return;
            }
            if(selectedMember == playerUnit.Pokemon){
                partyScreen.SetMessageText("You can't switch with the same Pokemon");
                return;
            }
            partyScreen.gameObject.SetActive(false);

            if (partyScreen.CalledFrom == BattleState.ActionSelection){
                StartCoroutine(RunTurns(BattleAction.SwitchPokemon));
            }
            else
            {
                state = BattleState.Busy;
                bool isTrainerAboutToUse = (partyScreen.CalledFrom == BattleState.AboutToUse);
                StartCoroutine(SwitchPokemon(selectedMember, isTrainerAboutToUse));
            }
            
            partyScreen.CalledFrom = null;
        };
        
        Action onBack = () =>
        {
            if(playerUnit.Pokemon.HP <= 0)
            {
                partyScreen.SetMessageText("You have to choose a Pokemon to switch out");
                return;
            }
            
            partyScreen.gameObject.SetActive(false);

            if (partyScreen.CalledFrom == BattleState.AboutToUse){
                
                StartCoroutine(SendNextTrainerPokemon());
            }
            else
                ActionSelection();
            
            partyScreen.CalledFrom = null;
        };
        
        partyScreen.HandleUpdate(onSelected, onBack);
    }

    void HandleAboutToUse()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            aboutToUseChoice = !aboutToUseChoice;
        }
        
        dialogBox.UpdateChoiceBox(aboutToUseChoice);
        
        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableChoiceBox(false);
            if (aboutToUseChoice)
            {
                OpenPartyScreen();
            }
            else
            {
                StartCoroutine(SendNextTrainerPokemon());
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableChoiceBox(false);
            StartCoroutine(SendNextTrainerPokemon());
        }
    }
    
    IEnumerator SwitchPokemon(Pokemon newPokemon, bool isTrainerAboutToUse = false)
    {
        if (playerUnit.Pokemon.HP > 0)
        {
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Pokemon.Base.Name}");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }
        
        playerUnit.Setup(newPokemon);
        dialogBox.SetMoveNames(newPokemon.Moves);
        yield return dialogBox.TypeDialog($"Go {newPokemon.Base.Name}!");

        if (isTrainerAboutToUse)
            StartCoroutine(SendNextTrainerPokemon());
        else
            state = BattleState.RunningTurn;
        
    }
    
    IEnumerator SendNextTrainerPokemon()
    {
        state = BattleState.Busy;
        
        var nextPokemon = trainerParty.GetHealthyPokemon();
        enemyUnit.Setup(nextPokemon);
        yield return dialogBox.TypeDialog($"{trainer.Name} sent out {nextPokemon.Base.Name}!");
        
        state = BattleState.RunningTurn;
    }

    IEnumerator ThrowPokeball(PokeballItem pokeballItem)
    {
        state = BattleState.Busy;

        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog("You can't steal a trainer's Pokemon!");
            state = BattleState.RunningTurn;
            yield break;
        }
        
        yield return dialogBox.TypeDialog($"{player.Name} used a {pokeballItem.name}!");
        
        var pokeballObj = Instantiate(pokeballSprite, playerUnit.transform.position - new Vector3(2, 0), Quaternion.identity);
        var pokeball = pokeballObj.GetComponent<SpriteRenderer>();
        pokeball.sprite = pokeballItem.Icon;
        
        yield return pokeball.transform.DOJump(enemyUnit.transform.position + new Vector3(0, 2), 2f, 1, 1f).WaitForCompletion();
        yield return enemyUnit.PlayCaptureAnimation();
        yield return pokeball.transform.DOMoveY(enemyUnit.transform.position.y - 1.5f, 0.5f).WaitForCompletion();

        int shakeCount = TryToCatch(enemyUnit.Pokemon, pokeballItem);
        
        for (int i = 0; i < Mathf.Min(shakeCount,3); ++i)
        {
            yield return new WaitForSeconds(0.5f);
            yield return pokeball.transform.DOPunchRotation(new Vector3(0, 0, 10f), 0.8f).WaitForCompletion();
        }

        if (shakeCount == 4)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} was caught!");
            yield return pokeball.DOFade(0, 1.5f).WaitForCompletion();
            
            playerParty.AddPokemon(enemyUnit.Pokemon);
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} has been added to your party");
            
            Destroy(pokeballObj);
            BattleOver(true);
        }
        else
        {
            yield return new WaitForSeconds(1f);
            pokeball.DOFade(0, 0.2f);
            yield return enemyUnit.PlayBreakOutAnimation();
            yield return new WaitForSeconds(2f);
            
            Destroy(pokeballObj);
            if (shakeCount < 2)
                yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} broke free!");
            else
                yield return dialogBox.TypeDialog($"Almost caught it!");
            
            state = BattleState.RunningTurn;
        }
    }

    int TryToCatch(Pokemon pokemon, PokeballItem pokeballItem)
    {
        float a = (3 * pokemon.MaxHp - 2 * pokemon.HP) * pokemon.Base.CatchRate * pokeballItem.CatchRate * ConditionsDB.GetStatusBonus(pokemon.Status) / (3 * pokemon.MaxHp);
        
        if (a >= 255)
            return 4;
        
        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));
        
        int shakeCount = 0;
        while (shakeCount < 4)
        {
            if (UnityEngine.Random.Range(0, 65535) >= b)
                break;
            
            ++shakeCount;
        }
        
        return shakeCount;
    }

    IEnumerator TryToEscape()
    {
        state = BattleState.Busy;
        
        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog("You can't escape from a trainer battle!");
            state = BattleState.RunningTurn;
            yield break;
        }
        
        
        int playerSpeed = playerUnit.Pokemon.Speed;
        int enemySpeed = enemyUnit.Pokemon.Speed;
        
        if (enemySpeed < playerSpeed)
        {
            yield return dialogBox.TypeDialog("Ran away safely!");
            BattleOver(true);
        }
        else
        {
            int odds = Mathf.FloorToInt((playerSpeed * 128) / enemySpeed) + 30 * escapeAttempts;
            escapeAttempts++;
            if (UnityEngine.Random.Range(0, 256) < odds)
            {
                yield return dialogBox.TypeDialog("Ran away safely!");
                BattleOver(true);
            }
            else
            {
                yield return dialogBox.TypeDialog("Can't escape!");
                state = BattleState.RunningTurn;
            }
        }
    }
}
