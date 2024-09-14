using System.Collections.Generic;
﻿using System.Collections;
using UnityEngine;
using Winch.Core;
using Winch.Util;
using System;
using Winch.Core.API;
using UnityEngine.AI;
using Winch.Data.Shop;
using System.Linq;

namespace DeepSubmergence {
    public class DeepSubmergence : USingleton<DeepSubmergence> {

        // [/] Make dialogue only progress with certain buttons (match existing - only mouse)
        
        // V0.4 bugs & feedback fixes
        // [x] Last quest image has no fx (lmfao)
        // [x] docking mechanic jank (not sure if fixable)
        // [x] Immersive text; show/hide quotes and have less direct conversational UI
        // [x] Switch dialogue/pseudoquests to be more systemic, for intro cutscene?
        // [x] Switch over to using addressables for assets
        // [x] Sonar ping system?
        // [x] Update readme with fixed issues/new features
        
        // V0.5: Post-tech improvements
        // [x] Selling pumps and pressure vessels, level up with caught fish
        //    - unlock from progression levels
        //    - sizing objects PITA? How to give them in a way that doesn't suck
        // [x] Balance cost of new parts too
        // [x] Update readme
        // [x] Cover art for game start? Just a fullscreen splash on a canvas, not 3D backgroundy
        // [x] Play it a shitload, bugtest, etc.
        
        public static readonly ModAssembly ModAssembly = ModAssemblyLoader.GetCurrentMod();

        public static readonly string AssetsPath = ModAssembly.AssetsPath;

        public GameObject submarinePlayer;
        public GameObject submarineUI;
        public GameObject underwaterFishableManager;
        public GameObject debugAxes;

        public List<GameObject> managedObjects = new();
        
        private bool setup;

        protected override bool ShouldNotDestroyOnLoad => true;

        protected override void Awake(){
            base.Awake();
            WinchCore.Log.Debug("mod loaded");

            ApplicationEvents.Instance.OnGameLoaded += OnGameLoaded;
            GameManager.Instance.OnGameStarted += OnGameStarted;
            GameManager.Instance.OnGameEnded += OnGameEnded;

            try
            {
                ModelUtil.Initialize();
            }
            catch (Exception ex)
            {
                WinchCore.Log.Error(ex);
            }
        }

        private void OnGameLoaded()
        {
            setup = false;

            try
            {
                // Instantiate all the objects needed for the mod
                SetupTravellingMerchant();
                SetupSubmarinePlayer();
                SetupDebugAxes();
                SetupDiveUI();
                SetupFishableManager();
                SetupSeaBase();

                setup = true;
            }
            catch (Exception e)
            {
                WinchCore.Log.Error(e);
            }
        }

        private void OnGameStarted()
        {
            GameManager.Instance.SaveData.AddMapMarker("deepsubmergence.seabase", false);
        }

        private void OnGameEnded()
        {
            ShutDown();
        }

        private void Update(){
            try {
                if(setup){
                    // Constantly reset the freshness of deepsubmergence fish to prevent them from rotting
                    // This is to facilitate quests (i.e. you can always keep them around) but also a bit spooky
                    List<FishItemInstance> inventoryItems = GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<FishItemInstance>(ItemType.GENERAL, ItemSubtype.FISH);

                    foreach (FishItemInstance inventoryItem in inventoryItems){
                        if(inventoryItem.id.StartsWith("deepsubmergence")){
                            inventoryItem.freshness = 3.0f; // Max freshness value in game
                        }
                    }

                    List<FishItemInstance> storageItems = GameManager.Instance.SaveData.Storage.GetAllItemsOfType<FishItemInstance>(ItemType.GENERAL, ItemSubtype.FISH);
                    
                    foreach (FishItemInstance storageItem in storageItems){
                        if(storageItem.id.StartsWith("deepsubmergence")){
                            storageItem.freshness = 3.0f; // Max freshness value in game
                        }
                    }
                }
            } catch (Exception e){
                WinchCore.Log.Error(e);
            }
        }

        private void ShutDown(){
            setup = false;
            
            WinchCore.Log.Debug("Resetting");
            
            for(int i = 0, count = managedObjects.Count; i < count; ++i){
                Destroy(managedObjects[i]);
            }
        }

        private void SetupTravellingMerchant(){

            var gridShopJunk = GameManager.Instance.SaveData.GetGridByKey(GridKey.TRAVELLING_MERCHANT_MATERIALS);
            var gridConfigurationShopJunk = gridShopJunk.gridConfiguration;
            gridConfigurationShopJunk.mainItemType |= ItemType.EQUIPMENT;
            gridConfigurationShopJunk.mainItemSubtype |= Enums.PUMP;
            gridConfigurationShopJunk.mainItemSubtype |= Enums.PRESSURE_VESSEL;
            gridShopJunk.Init(gridConfigurationShopJunk, false);
            
            var shipyards = DockUtil.GetAllShipyardDestinations().Where(shipyard => shipyard.marketTabs.Any(marketTab => marketTab.gridKey == GridKey.TRAVELLING_MERCHANT_MATERIALS));
            foreach (var shipyard in shipyards)
            {
                shipyard.itemSubtypesBought |= Enums.PUMP;
                shipyard.itemSubtypesBought |= Enums.PRESSURE_VESSEL;
            }
            var shopJunk = ShopUtil.GetShopData("TM_Junk");
            shopJunk.dialogueLinkedShopData.Add(new ShopData.DialogueLinkedShopData
            {
                itemData = new List<ShopData.ShopItemData>{
                    new ModdedShopItemData("deepsubmergence.pumptier1"),
                    new ModdedShopItemData("deepsubmergence.pumptier2"),
                    new ModdedShopItemData("deepsubmergence.pumptier3")
                },
                dialogueNodes = new List<string> { "DeepSubmergence_Diver_Root" },
                requireMode = ShopData.DialogueLinkedShopData.RequireMode.ALL
            });
            shopJunk.dialogueLinkedShopData.Add(new ShopData.DialogueLinkedShopData
            {
                itemData = new List<ShopData.ShopItemData>{
                    new ModdedShopItemData("deepsubmergence.pressurevesseltier1"),
                    new ModdedShopItemData("deepsubmergence.pressurevesseltier2"),
                    new ModdedShopItemData("deepsubmergence.pressurevesseltier3")
                },
                dialogueNodes = new List<string> { "DeepSubmergence_Diver_Root" },
                requireMode = ShopData.DialogueLinkedShopData.RequireMode.ALL
            });
        }
        
        private void SetupSubmarinePlayer(){
            submarinePlayer = Utils.SetupModelTextureAsGameObject(
                "SubmarinePlayer",
                ModelUtil.GetModel("deepsubmergence.submarine"),
                false,
                TextureUtil.GetTexture("deepsubmergence.submarinetexture")
            );

            submarinePlayer.AddComponent<SubmarinePlayer>();
        }
        
        private void SetupDebugAxes(){
            debugAxes = Utils.SetupModelTextureAsGameObject(
                "Debug Axes",
                ModelUtil.GetModel("deepsubmergence.debugaxes")
            );
            
            debugAxes.transform.position = new Vector3(0.0f, -1000.0f, 0.0f);
        }
        
        private void SetupDiveUI(){
            submarineUI = Utils.SetupTextureAsSpriteOnCanvas(
                "Submarine UI",
                TextureUtil.GetSprite("deepsubmergence.uiribbon"),
                new Vector2(75.0f, 50.0f),
                new Vector2(37.5f, 320.0f)
            );
            
            submarineUI.AddComponent<SubmarineUI>();
        }
        
        private void SetupFishableManager(){
            underwaterFishableManager = Utils.SetupGameObject("Underwater Fishable Manager");
            underwaterFishableManager.AddComponent<UnderwaterFishableManager>();
        }

        private void SetupSeaBase(){
            GameObject seaBase = Utils.SetupModelTextureAsGameObject(
                "Sea Base",
                ModelUtil.GetModel("deepsubmergence.seabase"),
                true,
                TextureUtil.GetTexture("deepsubmergence.seabasetexture")
            );
            GameObject seaBaseWindows = Utils.SetupModelTextureAsGameObject(
                "Sea Base Windows",
                ModelUtil.GetModel("deepsubmergence.seabasewindows"),
                true,
                TextureUtil.GetTexture("deepsubmergence.seabasetexture"),
                TextureUtil.GetTexture("deepsubmergence.seabaseemittexture"),
                2
            );
            
            seaBaseWindows.transform.SetParent(seaBase.transform, false);
            seaBaseWindows.transform.localPosition = Vector3.zero;

            // Setup nav mash obstacle
            GameObject navMeshObstacleObject = Utils.SetupGameObject("Sea Base Nav Mesh Obstacle");
            navMeshObstacleObject.transform.SetParent(seaBase.transform, false);
            navMeshObstacleObject.transform.localPosition = new Vector3(2.25f, 0.0f, 0.875f);
            navMeshObstacleObject.transform.localScale = Vector3.one * 18.0f;
            NavMeshObstacle navMeshObstacle = navMeshObstacleObject.AddComponent<NavMeshObstacle>();
            navMeshObstacle.shape = NavMeshObstacleShape.Capsule;

            // Position the sea base model
            seaBase.transform.position = new Vector3(735.0f, -5.7f, -272.0f);
            seaBase.transform.rotation = Quaternion.Euler(0.0f, 125.0f, 0.0f);
        }
    }
}
