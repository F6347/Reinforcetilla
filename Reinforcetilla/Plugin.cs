
using System;
using System.Threading.Tasks;
using BepInEx;
using GorillaNetworking;
using UnityEngine;
namespace Reinforcetilla
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        Transform button;
        float difButton;
        bool onModdedPage;
        GameObject buttonR, buttonL;
        GTZone lastZone;

        void Awake()
        {
            PlayerPrefs.SetString("currentGameMode", "Infection");
            HarmonyPatches.ApplyHarmonyPatches();
        }
        void OnEnable()
        {
            GorillaTagger.OnPlayerSpawned(OnGameInitialized);
        }
        void OnGameInitialized()
        {
            buttonR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            buttonR.layer = 18;
            buttonR.GetComponent<Renderer>().material = new Material(Shader.Find("GorillaTag/UberShaderNonSRP")) { color = new Color(0.8f, 0.8f, 0.7f) };
            buttonR.name = "buttonR";
            buttonR.AddComponent<GorillaPressableButton>().onPressed += OnButtonPressed;

            buttonL = Instantiate(buttonR);
            buttonL.name = "buttonL";
            buttonL.GetComponent<GorillaPressableButton>().onPressed += OnButtonPressed;

            OnEnteredNewMap();
        }

        async void OnButtonPressed(GorillaPressableButton button, bool hand)
        {
            button.GetComponent<Renderer>().material.color = Color.red;
            ToggleModdeds(button.gameObject.name.Contains("R"));
            await Task.Delay(150);
            button.GetComponent<Renderer>().material.color = new Color(0.8f, 0.8f, 0.7f);
        }
        void Update()
        {
            if (VRRig.LocalRig.zoneEntity.currentZone != lastZone) OnEnteredNewMap();
            lastZone = VRRig.LocalRig.zoneEntity.currentZone;
        }

        async void OnEnteredNewMap()
        {
            button = null;
            await Task.Delay(1000);
            ToggleModdeds(onModdedPage);
            try
            {
                buttonR.transform.localScale = button.lossyScale * 1.05f;
                buttonR.transform.rotation = button.rotation;
                buttonR.transform.position = buttonR.transform.position = button.position - button.up * difButton;
                buttonL.transform.localScale = button.lossyScale * 1.05f;
                buttonL.transform.rotation = button.rotation;
                buttonL.transform.position = buttonR.transform.position + buttonR.transform.right * difButton* .86f;
            }
            catch { }
        }
        void ToggleModdeds(bool setActive)
        {
            try
            {
                foreach (ModeSelectButton gamemodeButton in GameObject.FindObjectsOfType<ModeSelectButton>())
                {
                    button = gamemodeButton.transform;
                    gamemodeButton.gameMode = setActive ? gamemodeButton.gameMode.Contains("MODDED_") ? gamemodeButton.gameMode : "MODDED_" + gamemodeButton.gameMode : gamemodeButton.gameMode.Replace("MODDED_", "");
                    var title = gamemodeButton.transform.Find("Title").GetComponent<TMPro.TextMeshPro>();
                    title.text = gamemodeButton.gameMode.Replace(setActive ? "MODDED_" : "Mod. ", setActive ? "Mod. " : "");
                    title.fontSize = setActive ? (975 / gamemodeButton.gameMode.Length) : 80;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Couldn't find the gamemode stand.  Error: " + e);
                return;
            }
            difButton = button.lossyScale.magnitude * 4.1f;
            var lastSelectedGM = GorillaComputer.instance.currentGameMode.Value;
            if (onModdedPage ^ setActive) GorillaComputer.instance.currentGameMode.Value = lastSelectedGM.Contains("MODDED_") ? lastSelectedGM.Replace("MODDED_", "") : "MODDED_" + lastSelectedGM;
            onModdedPage = setActive;
        }
    }
}