using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog, Cutscene, Paused, Menu, PartyScreen, Bag, Evolution,Shop }

public class GameController : MonoBehaviour
{
   [SerializeField] PlayerController playerController;
   [SerializeField] BattleSystem battleSystem;
   [SerializeField] Camera worldCamera;
   [SerializeField] PartyScreen partyScreen;
   [SerializeField] InventoryUI inventoryUI;
   
   GameState state;
   GameState stateBeforePause;
   GameState StateBeforeEvolution;
   
   public GameState State
   {
      get => state;
      set => state = value;
   }
   
   public SceneDetails currentScene { get; private set; }
   public SceneDetails previousScene { get; private set; }
   
   MenuController menuController;
   
   public static GameController Instance { get; private set; }

   private void Awake()
   {
      Instance = this;
      
      menuController = GetComponent<MenuController>();
      
      //Cursor.lockState = CursorLockMode.Locked;
      //Cursor.visible = false;
      
      ConditionsDB.Init();
      PokemonDB.Init();
      MoveDB.Init();
      ItemDB.Init();
      QuestDB.Init();
   }

   private void Start()
   {
      battleSystem.OnBattleOver += EndBattle;
      partyScreen.Init();
      DialogManager.Instance.OnShowDialog += () => state = GameState.Dialog;
      DialogManager.Instance.OnCloseDialog += () =>
      {
         if(state == GameState.Dialog)
            state = GameState.FreeRoam;
      };
      
      menuController.onBack += () =>
      {
         state = GameState.FreeRoam;
         playerController.enabled = true;
      };
      
      menuController.onMenuSelected += OnMenuSelected;

      EvolutionManager.i.OnStartEvolution += () =>
      {
         StateBeforeEvolution = state;
         state = GameState.Evolution;
      };
      EvolutionManager.i.OnCompleteEvolution += () =>
      {
         state = stateBeforePause;
      };

      ShopController.i.OnStart += () => state = GameState.Shop;
      ShopController.i.OnFinish += () => state = GameState.FreeRoam;
   }

   
  

   public void PausedGame(bool pause){
      if (pause)
      {
         stateBeforePause = state;
         state = GameState.Paused;
      }
      else
      {
         state = stateBeforePause;
      }
   }
   
   public void StartBattle(){
      
      state = GameState.Battle;
      battleSystem.gameObject.SetActive(true);
      worldCamera.gameObject.SetActive(false);
      
      var playerParty = playerController.GetComponent<PokemonParty>();
      var wildPokemon = currentScene.GetComponent<MapArea>().GetRandomWildPokemon();
      
      var wildPokemonCopy = new Pokemon(wildPokemon.Base, wildPokemon.Level);
      
      battleSystem.StartBattle(playerParty, wildPokemonCopy);
   }
   
   TrainerController trainer;
   
   public void StartTrainerBattle(TrainerController trainer){
      
      state = GameState.Battle;
      battleSystem.gameObject.SetActive(true);
      worldCamera.gameObject.SetActive(false);
      
      this.trainer = trainer;
      var playerParty = playerController.GetComponent<PokemonParty>();
      var trainerParty = trainer.GetComponent<PokemonParty>();
      
      battleSystem.StartTrainerBattle(playerParty, trainerParty);
   }

   public void OnEnterTrainerView(TrainerController trainer)
   {
      state = GameState.Cutscene;
      StartCoroutine(trainer.TriggerTrainerBattle(playerController));
   }
   
   void EndBattle(bool won){

      if (trainer != null && won)
      {
         trainer.BattleLost();
         trainer = null;
      }
      
      state = GameState.FreeRoam;
      battleSystem.gameObject.SetActive(false);
      worldCamera.gameObject.SetActive(true);

      var PlayerParty = playerController.GetComponent<PokemonParty>();
      StartCoroutine(PlayerParty.CheckForEvolutions());
      AudioManager.i.PlayMusic(currentScene.BackgroundMusic, fade: true);
   }

   private void Update()
   {
      if (state == GameState.FreeRoam)
      {
         playerController.HandleUpdate();
         
         if(Input.GetKeyDown(KeyCode.Return))
         {
            menuController.OpenMenu();
            state = GameState.Menu;
         }
        
      }
      else if (state == GameState.Battle)
      {
         battleSystem.HandleUpdate();
      }
      else if (state == GameState.Dialog)
      {
         DialogManager.Instance.HandleUpdate();
      }
      else if (state == GameState.Menu)
      {
         menuController.HandleUpdate();
      }
      else if (state == GameState.PartyScreen)
      {
         Action onSelected = () =>
         {
            //sommaire
         };
         
         Action onBack = () =>
         {
            partyScreen.gameObject.SetActive(false);
            state = GameState.FreeRoam;
         };
         
         partyScreen.HandleUpdate(onSelected, onBack);
      }
      else if(state == GameState.Bag)
      {
         Action onBack = () =>
         {
            inventoryUI.gameObject.SetActive(false);
            state = GameState.FreeRoam;
         };
         
         inventoryUI.HandleUpdate(onBack);
      }
      else if (state == GameState.Shop)
      {
         ShopController.i.HandleUpdate();
      }
     
   }
   
   public void SetCurrentScene(SceneDetails currScene)
   {
      previousScene = currentScene;
      currentScene = currScene;
   }
   
   private void OnMenuSelected(int selectedItem)
   {
      if (selectedItem == 0)
      {
         // Pokemon
         partyScreen.gameObject.SetActive(true);
         state = GameState.PartyScreen;
      }
      else if (selectedItem == 1)
      {
         // Bag
         inventoryUI.gameObject.SetActive(true);
         state = GameState.Bag;
      }
      else if (selectedItem == 2)
      {
         // Save
         SavingSystem.i.Save("saveSlot1");
         state = GameState.FreeRoam;
      }
      else if (selectedItem == 3)
      {
         // Load
         SavingSystem.i.Load("saveSlot1");
         state = GameState.FreeRoam;
      }
      
      
      
   }
}
   