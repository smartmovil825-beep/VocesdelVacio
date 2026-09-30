using System;
using System.Collections.Generic;
using System.Linq;
using Language;
using Newtonsoft.Json;
using Satchel;
using UnityEngine;
using UnityEngine.Audio;

namespace HKVocals;

public sealed class HKVocals : Mod, IGlobalSettings<GlobalSettings>, ILocalSettings<SaveSettings>, ICustomMenuMod
{
    public static GlobalSettings _globalSettings { get; private set; } = new();
    public void OnLoadGlobal(GlobalSettings s) => _globalSettings = s;
    public GlobalSettings OnSaveGlobal() => _globalSettings;
    public static SaveSettings _saveSettings { get; private set; } = new();
    public void OnLoadLocal(SaveSettings s) => _saveSettings = s;
    public SaveSettings OnSaveLocal() => _saveSettings;

    public AudioSource audioSource;
    internal static HKVocals instance;
    public static NonBouncer CoroutineHolder;

    public override List<ValueTuple<string, string>> GetPreloadNames()
    {
        return new List<(string, string)>()
        {
            ("Room_shop", "_SceneManager")
        };
    }

    public HKVocals() : base("Voces del Vacio")
    {
        OnMenuStyleTitle.AfterOrig.SetTitle += AddCustomBanner;
        On.UIManager.Start += AddIcon;

        SFCore.MenuStyleHelper.AddMenuStyleHook += MajorFeatures.MenuTheme.AddTheme;
        MajorFeatures.Achievements.Hook();
    }

    private static string Version = "1.0.0.0";
    public override string GetVersion() => $"{Version}";

    public override void Initialize()
    {
        instance = this;

        // 1. Cargar el AssetBundle interno
        EmbeddedAudioLoader.Initialize();
        MixerLoader.LoadAssetBundle();
        CreditsLoader.LoadAssetBundle();
        StyleLoader.LoadAssetBundle();

        // 2. Inicializar subsistemas
        MajorFeatures.SpecialAudio.Hook();
        MajorFeatures.NPCDialogue.Hook();
        MajorFeatures.MuteOriginalAudio.Hook();
        MajorFeatures.DampenAudio.Hook();
        MajorFeatures.DreamNailDialogue.Hook();
        MajorFeatures.AutoScroll.Hook();
        MajorFeatures.ScrollLock.Hook();
        MajorFeatures.AutomaticBossDialogue.Hook();
        MajorFeatures.UITextAudio.Hook();
        MajorFeatures.RollCredits.Hook();
        MajorFeatures.Patches.Hook();

        EasterEggs.Lapidas.Hook();
        EasterEggs.EternalOrdeal.Hook();
        EasterEggs.SpecialGrub.Hook();

        // Protección contra NullReferenceException en PaleFlower
        try
        {
            EasterEggs.PaleFlower.Hook();
        }
        catch (System.Exception ex)
        {
            LogWarn($"No se pudo registrar el EasterEgg PaleFlower: {ex.Message}");
        }

        UIManager.EditMenus += UI.AudioMenu.AddAudioSliderAndSettingsButton;
        UIManager.EditMenus += UI.ExtrasMenu.AddCreditsButton;
        UIManager.EditMenus += UI.SettingsPrompt.CreatePrompt;

        UI.SettingsPrompt.HookRemoveButton();

        Hooks.PmFsmBeforeStartHook += AddFSMEdits;

        CoroutineHolder = new GameObject("HK Vocals Coroutine Holder").AddComponent<NonBouncer>();
        Object.DontDestroyOnLoad(CoroutineHolder);
        CreateAudioSource();

        var tmpStyle = MenuStyles.Instance.styles.FirstOrDefault(x => x.styleObject.name.Contains("HKVStyle"));
        if (tmpStyle != null)
        {
            MenuStyles.Instance.SetStyle(MenuStyles.Instance.styles.ToList().IndexOf(tmpStyle), false);
        }

        InitAchievements();

        Log("Voces del Vacio inicializado con éxito.");
    }

    public void CreateAudioSource()
    {
        LogDebug("Creando AudioSource principal");
        GameObject audioGO = new GameObject("HK Vocals Audio");
        audioSource = audioGO.AddComponent<AudioSource>();

        audioSource.SetMixerGroup();
        Object.DontDestroyOnLoad(audioGO);
    }

    private void AddFSMEdits(PlayMakerFSM fsm)
    {
        string sceneName = MiscUtils.GetCurrentSceneName();
        string gameObjectName = fsm.gameObject.name;
        string fsmName = fsm.FsmName;

        if (FSMEditData.FsmEdits.TryGetValue(new HKVocalsFsmData(sceneName, gameObjectName, fsmName), out var action_1))
        {
            action_1.TryInvokeActions(fsm);
        }
        if (FSMEditData.FsmEdits.TryGetValue(new HKVocalsFsmData(gameObjectName, fsmName), out var action_2))
        {
            action_2.TryInvokeActions(fsm);
        }
        if (FSMEditData.FsmEdits.TryGetValue(new HKVocalsFsmData(fsmName), out var action_3))
        {
            action_3.TryInvokeActions(fsm);
        }
    }
    //Modificacion del Title para que funcione solo cuando el juego esta en español y el tema activo es HKVStyle
    private void AddCustomBanner(OnMenuStyleTitle.Delegates.Params_SetTitle args)
    {
        string activeStyle = string.Empty;
        if (MenuStyles.Instance != null && MenuStyles.Instance.styles != null)
        {
            activeStyle = MenuStyles.Instance.styles[MenuStyles.Instance.CurrentStyle].styleObject.name;
        }

        if (Language.Language.CurrentLanguage() == LanguageCode.ES && activeStyle.Contains("HKVStyle"))
        {
            args.self.Title.sprite = AssemblyUtils.GetSpriteFromResources(
                Random.Range(1, 50) == 1 && _globalSettings.settingsOpened
                    ? "Resources.Title_alt.png"
                    : "Resources.Title.png"
            );
        }
    }

    private void InitAchievements()
    {
        if (_globalSettings.FinishedUIDialoge == null)
        {
            _globalSettings.FinishedUIDialoge = JsonConvert.DeserializeObject<List<string>>(
                System.Text.Encoding.Default.GetString(Satchel.AssemblyUtils.GetBytesFromResources("Resources.AchievementKeys.Inventory_KEYs.json")));
        }

        if (_globalSettings.FinishedNPCDialoge == null)
        {
            _globalSettings.FinishedNPCDialoge = JsonConvert.DeserializeObject<List<string>>(
                System.Text.Encoding.Default.GetString(Satchel.AssemblyUtils.GetBytesFromResources("Resources.AchievementKeys.NPCs_KEYs.json")));
        }

        if (_globalSettings.FinishedDNailDialoge == null)
        {
            _globalSettings.FinishedDNailDialoge = JsonConvert.DeserializeObject<List<string>>(
                System.Text.Encoding.Default.GetString(Satchel.AssemblyUtils.GetBytesFromResources("Resources.AchievementKeys.Dream_Nail_KEYs.json")));
        }

        if (_globalSettings.FinishedLoreTabletDialoge == null)
        {
            _globalSettings.FinishedLoreTabletDialoge = JsonConvert.DeserializeObject<List<string>>(
                System.Text.Encoding.Default.GetString(Satchel.AssemblyUtils.GetBytesFromResources("Resources.AchievementKeys.Lore_Tablet_KEYs.json")));
        }
    }

    private static Sprite icon;
    private void AddIcon(On.UIManager.orig_Start orig, UIManager self)
    {
        orig(self);

        Transform dlcTransform = self.transform.Find("UICanvas/MainMenuScreen/TeamCherryLogo/Hidden_Dreams_Logo");

        if (dlcTransform == null)
        {
            Log("[HKVocals] No se encontró Hidden_Dreams_Logo en la jerarquía.");
            return;
        }

        GameObject dlc = dlcTransform.gameObject;
        GameObject clone = Object.Instantiate(dlc, dlc.transform.parent);
        clone.SetActive(true);

        Vector3 pos = clone.transform.position;
        clone.transform.position = pos + new Vector3(3.2f, -0.111f, 0);
        clone.transform.SetScaleX(233f);
        clone.transform.SetScaleY(233f);

        icon = Satchel.AssemblyUtils.GetSpriteFromResources("Resources.icon.png");

        if (clone.TryGetComponent<SpriteRenderer>(out var sr))
        {
            sr.sprite = icon;
        }
        else if (clone.TryGetComponent<UnityEngine.UI.Image>(out var img))
        {
            img.sprite = icon;
        }
    }

    public static void DoLogDebug(object s) => instance?.LogDebug(s);
    public static void DoLog(object s) => instance?.Log(s);
    public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates) => UI.ModMenu.CreateModMenuScreen(modListMenu);
    public bool ToggleButtonInsideMenu => false;
}