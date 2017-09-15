﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KekeDreamLand
{
    /// <summary>
    /// The GameManager permit to switch easily between scenes.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Inspector attributes

        [Header("Build indexes :")]
        public int mainMenuIndex = 0;
        public int worldMapIndex = 1;

        [Header("Only for test")]
        public bool skipIntro = false;

        #endregion

        #region Game Manager attributes

        /// <summary>
        /// Singleton of the gamemanager.
        /// </summary>
        public static GameManager instance = null;

        /// <summary>
        /// Level manager. Ready only.
        /// </summary>
        public LevelManager CurrentLevel { get; private set; }

        // Store the current level index to simplify save.
        private int currentLevelIndex;

        // Animation transition script.
        private GameObject ui;
        private TransitionManager transitionManager;

        /// <summary>
        /// Return true if the current screen is the world map.
        /// </summary>
        public bool IsWorldMapScreen
        {
            get { return isWorldMap; }
        }
        private bool isWorldMap = false;
        private WorldMapManager worldmap;

        // TODO add is Saving or useless ?

        // Load/Save system.
        private PlayerProgress playerProgress;

        #endregion

        #region Unity methods

        private void Awake()
        {
            SingletonThis();

            // Load data
            playerProgress = SaveLoadManager.LoadPlayerProgress();

            // Reset
            //playerProgress = new PlayerProgress();
            //SaveLoadManager.SavePlayerProgress(playerProgress);
        }

        private void OnEnable()
        {
            // Called only once because it's singleton.
            SceneManager.sceneLoaded += NewSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= NewSceneLoaded;
        }

        #endregion

        #region GameManager methods

        // Delegate method triggered when a new scene is loaded.
        private void NewSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            // Reset information.
            CurrentLevel = null;

            isWorldMap = false;
            worldmap = null;

            // Setup transition manager of the current scene.
            ui = GameObject.FindGameObjectWithTag("UI");
            if (ui)
                transitionManager = ui.transform.Find("TransitionPanel").GetComponent<TransitionManager>();

            // Load main menu :
            if (arg0.buildIndex == mainMenuIndex)
            {
                // Here we can change settings. Create new game or load existing game.
            }

            // Load world map :
            else if (arg0.buildIndex == worldMapIndex)
            {
                // Here we can move on the world map and enter in a level or access to an another world.

                isWorldMap = true;
                
                worldmap = GameObject.Find("WorldMap").GetComponent<WorldMapManager>();
                worldmap.SetupMap(playerProgress);
            }

            // Load a level :
            else
            {
                // Try to get the level manager of this level.
                GameObject levelManager = GameObject.FindGameObjectWithTag("LevelManager");
                if (levelManager)
                {
                    CurrentLevel = levelManager.GetComponent<LevelManager>();
                    CurrentLevel.StartCoroutine(CurrentLevel.DisplayLevelIntro(arg0.name, skipIntro));
                    
                    skipIntro = false;
                }
            }
        }

        // Singleton this class.
        private void SingletonThis()
        {
            if (instance == null)
                instance = this;

            else
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }
        
        #endregion

        #region Load and Save System
        
        /// <summary>
        /// Store all level progress and save them.
        /// </summary>
        /// <param name="feathersCollected"></param>
        /// <param name="itemsFound"></param>
        public void SaveLevelProgress(int feathersCollected, bool[] itemsFound)
        {
            // Create key for the dictionnary.
            string key = playerProgress.currentWorldIndex + "-" + currentLevelIndex;
            
            // Remove old entry to avoid duplicate entries.
            if (playerProgress.finishedLevels.ContainsKey(key))
                playerProgress.finishedLevels.Remove(key);

            // Create and add the new entry.
            LevelProgress levelProgress = new LevelProgress(feathersCollected, itemsFound);
            playerProgress.finishedLevels.Add(key, levelProgress);

            // TODO update world progress.

            // Save.
            SaveLoadManager.SavePlayerProgress(playerProgress);
        }

        #endregion

        #region Scene management

        /// <summary>
        /// Switch to the main menu scene.
        /// </summary>
        public void LoadMainMenu()
        {
            SceneManager.LoadScene(mainMenuIndex);
        }

        /// <summary>
        /// Switch to the world map scene.
        /// </summary>
        public void LoadWorldMap()
        {
            SceneManager.LoadScene(worldMapIndex);
        }

        /// <summary>
        /// Switch to the new specified level.
        /// </summary>
        /// <param name="world">world index</param>
        /// <param name="level">level index of this world</param>
        public void LoadNewLevel(int world, int level)
        {
            // Update current level index.
            currentLevelIndex = worldmap.GetLevelIndex(playerProgress.currentNodeIndex);

            SceneManager.LoadScene("Level " + (world + 1) + "-" + (level + 1));
        }

        /// <summary>
        /// Reset current loaded scene.
        /// </summary>
        public void ResetCurrentScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        #endregion

        #region Level management

        /// <summary>
        /// Use this when you want to finish the current level.
        /// </summary>
        public void FinishCurrentLevel()
        {
            CurrentLevel.IsDisplayLevelOutro = true;

            if(CurrentLevel.HasCollectAllFeathers())
            {
                Debug.Log("All feathers have been collected.");
                CurrentLevel.PickSpecialItem(1);
            }
            
            transitionManager.FadeIn();
        }

        #endregion

        #region World map management

        public void InteractWithCurrentNode()
        {
            worldmap.InteractWithCurrentNode(playerProgress);
        }

        public void MoveOnWorldMap(InputDirection directionPressed)
        {
            worldmap.TryToMove(playerProgress, directionPressed);
        }

        /// <summary>
        /// Update the current position of Boing on the current world.
        /// </summary>
        /// <param name="nodeIndex"></param>
        public void UpdateCurrentNodeOnWorld(int nodeIndex)
        {
            playerProgress.currentNodeIndex = nodeIndex;
        }

        #endregion

        #region Transition management

        /// <summary>
        /// Activate transition animator.
        /// </summary>
        public void ActivateAnimator()
        {
            transitionManager.ActivateAnimator();
        }

        /// <summary>
        /// Use this when you want to fade in and reload the current scene.
        /// </summary>
        public void TriggerFadeIn()
        {
            transitionManager.FadeIn();
        }

        public void TriggerFadeOut()
        {
            transitionManager.FadeOut();
        }

        // TODO use delegate to easily done what we want when fadeIn is finished ?

        /// <summary>
        /// Event triggered when a fadeIn transition animation finished.
        /// </summary>
        public void FadeInFinished()
        {
            // Case of a level scene.
            if(CurrentLevel)
            {
                // Case of an end of level.
                if (CurrentLevel.IsDisplayLevelOutro)
                {
                    CurrentLevel.LevelOutro();
                    return;
                }

                // Case of an internal transition.
                else if (CurrentLevel.IsInternalTransition)
                {
                    // Move Boing and Camera view when user don't see.
                    CurrentLevel.MoveBoingToNewArea();

                    transitionManager.FadeOut();
                    return;
                }

                // Case of a restart of the level.
                else
                {
                    ResetCurrentScene();
                    skipIntro = true;
                }
            }

            else if(isWorldMap)
            {
                
            }
            
            else
            {
                LoadWorldMap();
            }
        }

        #endregion
    }

}