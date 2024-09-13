using System.Collections.Generic;
﻿using System.Collections;
using UnityEngine;
using Winch.Core;
using Winch.Util;
using System;
using Winch.Core.API;
using UnityEngine.AI;

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

        public GameObject dredgePlayer;
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

            SeaBaseDock.Initialize();
            ModelUtil.Initialize();
        }
        
        IEnumerator Start(){
            
            setup = false;
            
            // spin until we find a player
            while(dredgePlayer == null){
                yield return null;
                dredgePlayer = GameObject.Find("Player");
            }

            try {
                // Instantiate all the objects needed for the mod
                SetupSubmarinePlayer();
                SetupDebugAxes();
                SetupDiveUI();
                SetupFishableManager();
                SetupSeaBase();
                
                setup = true;
            } catch (Exception e){
                WinchCore.Log.Error(e.ToString());
            }
        }
        
        void Update(){
            try {
                if(setup){
                    if(dredgePlayer == null){
                        ShutDown();
                    } else {
                        // Constantly reset the freshness of deepsubmergence fish to prevent them from rotting
                        // This is to facilitate quests (i.e. you can always keep them around) but also a bit spooky
                        List<SpatialItemInstance> inventoryItems = GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<SpatialItemInstance>(ItemType.GENERAL);

                        for(int i = 0, count = inventoryItems.Count; i < count; ++i){
                            if(inventoryItems[i].id.Contains("deepsubmergence") && inventoryItems[i] is FishItemInstance fishy){
                                fishy.freshness = 3.0f; // Max freshness value in game
                            }
                        }

                        List<SpatialItemInstance> storageItems = GameManager.Instance.SaveData.Storage.GetAllItemsOfType<SpatialItemInstance>(ItemType.GENERAL);

                        for(int i = 0, count = storageItems.Count; i < count; ++i){
                            if(storageItems[i].id.Contains("deepsubmergence") && storageItems[i] is FishItemInstance fishy){
                                fishy.freshness = 3.0f; // Max freshness value in game
                            }
                        }
                    }
                }
            } catch (Exception e){
                WinchCore.Log.Error(e.ToString());
            }
        }

        private void ShutDown(){
            setup = false;
            
            WinchCore.Log.Debug("Resetting");
            
            for(int i = 0, count = managedObjects.Count; i < count; ++i){
                Destroy(managedObjects[i]);
            }
            
            // Kick off a restart waiting for player
            StartCoroutine(Start());
        }
        
        private void SetupSubmarinePlayer(){
            submarinePlayer = Utils.SetupModelTextureAsGameObject(
                "SubmarinePlayer",
                ModelUtil.GetModel("deepsubmergence.submarine"),
                TextureUtil.GetTexture("deepsubmergence.submarinetexture")
            );

            submarinePlayer.AddComponent<SubmarinePlayer>();
        }
        
        private void SetupDebugAxes(){
            debugAxes = Utils.SetupModelTextureAsGameObject(
                "Debug Axes",
                ModelUtil.GetModel("deepsubmergence.debugaxes"),
                null
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
                TextureUtil.GetTexture("deepsubmergence.seabasetexture")
            );
            GameObject seaBaseWindows = Utils.SetupModelTextureAsGameObject(
                "Sea Base Windows",
                ModelUtil.GetModel("deepsubmergence.seabasewindows"),
                TextureUtil.GetTexture("deepsubmergence.seabasetexture"),
                TextureUtil.GetTexture("deepsubmergence.seabaseemittexture"),
                2
            );
            
            seaBaseWindows.transform.SetParent(seaBase.transform, false);
            seaBaseWindows.transform.localPosition = Vector3.zero;
            
            // Position the sea base model
            seaBase.transform.position = new Vector3(735.0f, -5.7f, -272.0f);
            seaBase.transform.rotation = Quaternion.Euler(0.0f, 125.0f, 0.0f);
        }
    }
}
