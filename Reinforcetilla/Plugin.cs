
using System;
using System.Threading.Tasks;
using BepInEx;
using GorillaGameModes;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Reinforcetilla
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        GameObject forestButton;
        Transform firstButton;
        bool onModdedPage = false;
        bool wasForestActive;
        GameObject buttonR;
        GameObject buttonL;
        GTZone lastZone;
        void Awake()
        {
            PlayerPrefs.SetString("currentGameMode", PlayerPrefs.GetString("currentGameMode").Replace("MODDED_", ""));
        }
        void OnEnable()
        {
            
            HarmonyPatches.ApplyHarmonyPatches();
            GorillaTagger.OnPlayerSpawned(OnGameInitialized);
        }
        void OnGameInitialized()
        {
            SceneManager.sceneLoaded += delegate (Scene scene, LoadSceneMode mode) { OnEnteredNewMap(); };
            SceneManager.sceneUnloaded += delegate (Scene scene) { OnEnteredNewMap(); };
            buttonR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            buttonR.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
            buttonR.layer = 18;
            buttonR.GetComponent<Renderer>().material = new Material(Shader.Find("GorillaTag/UberShaderNonSRP")) { color = new Color(0.8f, 0.8f, 0.7f) };
            buttonR.AddComponent<GorillaPressableButton>().onPressed += onButtonPressed;
            buttonR.name = "buttonR";
            buttonL = Instantiate(buttonR);
            buttonL.GetComponent<GorillaPressableButton>().onPressed += onButtonPressed;
            buttonL.name = "buttonL";
        }
        async void onButtonPressed(GorillaPressableButton button, bool leftHand)
        {
            button.GetComponent<Renderer>().material.color = Color.red;
            ToggleModdeds(button.gameObject.name == "buttonR");
            await Task.Delay(150);
            button.GetComponent<Renderer>().material.color = new Color(0.8f, 0.8f, 0.7f);
        }
        void Update()
        {
            if (forestButton.activeInHierarchy && !wasForestActive || VRRig.LocalRig.zoneEntity.currentZone != lastZone && VRRig.LocalRig.zoneEntity.currentZone.ToString() == "customMaps") OnEnteredNewMap();
            wasForestActive = forestButton.activeInHierarchy;
            lastZone = VRRig.LocalRig.zoneEntity.currentZone;
        }
        async void OnEnteredNewMap()
        {
            firstButton = null;
            await Task.Delay(250);
            ToggleModdeds(onModdedPage);
            try
            {
                buttonR.transform.rotation = firstButton.rotation;
                buttonR.transform.position = firstButton.position - firstButton.up * 0.18f;
                buttonL.transform.rotation = firstButton.rotation;
                buttonL.transform.position = buttonR.transform.position + buttonR.transform.right * 0.4f;
            }
            catch { }
        }
        void ToggleModdeds(bool setActive)
        {
            try
            {
                foreach (ModeSelectButton gamemodeButton in GameObject.FindObjectsOfType<ModeSelectButton>())
                {
                    forestButton = forestButton ?? gamemodeButton.gameObject;
                    firstButton = firstButton ?? gamemodeButton.transform;
                    gamemodeButton.gameMode = setActive ? gamemodeButton.gameMode.Contains("MODDED_") ? gamemodeButton.gameMode : "MODDED_" + gamemodeButton.gameMode : gamemodeButton.gameMode.Replace("MODDED_", "");
                    var title = gamemodeButton.transform.Find("Title").GetComponent<TMPro.TextMeshPro>();
                    title.text = gamemodeButton.gameMode.Replace(setActive ? "MODDED_" : "Mod. ", setActive ? "Mod. " : "");
                    title.fontSize = setActive ? (975 / gamemodeButton.gameMode.Length) : 80;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Couldn't find the gamemode stand.  : "+e);
            }
            var lastSelectedGM = GorillaNetworking.GorillaComputer.instance.currentGameMode.Value;
            if (onModdedPage ^ setActive) GorillaNetworking.GorillaComputer.instance.currentGameMode.Value = lastSelectedGM.Contains("MODDED_") ? lastSelectedGM.Replace("MODDED_", "") : "MODDED_" + lastSelectedGM;
            onModdedPage = setActive;
        }
    }
}
