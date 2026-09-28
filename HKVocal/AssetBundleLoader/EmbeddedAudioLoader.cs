using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Modding;
using Modding.Menu;
using UnityEngine;
using UnityEngine.UI;

namespace HKVocals;

public static class EmbeddedAudioLoader
{
    private static AssetBundle audioBundle;
    private static readonly Dictionary<string, AudioClip> audioCache = new(StringComparer.OrdinalIgnoreCase);

    public static List<string> AudioNames { get; } = new();

    private static AssetBundleCreateRequest loadRequest;
    private static NonBouncer CoroutineHolder;
    private static GameObject TextCanvas;
    private static Text TextPanelText;
    private const string WaitText = "Por favor espera mientras se cargan los audios...";

    public static bool AudioLoadSuccess { get; private set; } = false;

    // Método para inicializar los hooks e iniciar la carga
    public static void Initialize()
    {
        if (CoroutineHolder == null)
        {
            CoroutineHolder = new GameObject("HKVocals_AudioLoader_CoroutineHolder").AddComponent<NonBouncer>();
            Object.DontDestroyOnLoad(CoroutineHolder);
        }

        // Suscribirse a los eventos del juego para bloquear el inicio/continuación mientras carga
        On.GameManager.StartNewGame += StopStartNewGame;
        On.GameManager.ContinueGame += StopContinueGame;

        LoadAssetBundleAsync();
    }

    private static void StopContinueGame(On.GameManager.orig_ContinueGame orig, GameManager self)
    {
        CoroutineHolder.StartCoroutine(BlockContinueGame(orig, self));
    }

    private static void StopStartNewGame(On.GameManager.orig_StartNewGame orig, GameManager self, bool permadeathmode, bool bossrushmode)
    {
        CoroutineHolder.StartCoroutine(BlockStartNewGame(orig, self, permadeathmode, bossrushmode));
    }

    private static IEnumerator BlockContinueGame(On.GameManager.orig_ContinueGame orig, GameManager self)
    {
        yield return WaitForBundleToLoad();
        orig(self);
    }

    private static IEnumerator BlockStartNewGame(On.GameManager.orig_StartNewGame orig, GameManager self, bool p, bool b)
    {
        yield return WaitForBundleToLoad();
        orig(self, p, b);
    }

    private static IEnumerator WaitForBundleToLoad()
    {
        if (loadRequest == null || loadRequest.isDone) yield break;

        CreateTextPanel();
        string prevText = "";
        while (!loadRequest.isDone)
        {
            yield return null;
            string newText = WaitText + $" ({Math.Round(loadRequest.progress, 1) * 100}%)";
            if (newText != prevText)
            {
                if (TextPanelText != null)
                {
                    TextPanelText.text = newText;
                }
                prevText = newText;
            }
        }
        if (TextCanvas != null)
        {
            Object.Destroy(TextCanvas);
        }
    }

    public static void LoadAssetBundleAsync()
    {
        if (audioBundle != null || loadRequest != null) return;

        try
        {
            string bundlePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "audiobundle");

            if (!File.Exists(bundlePath))
            {
                bundlePath = Path.Combine(Application.persistentDataPath, "Mods", "VocesDelVacio", "audiobundle");
            }

            if (File.Exists(bundlePath))
            {
                loadRequest = AssetBundle.LoadFromFileAsync(bundlePath);
                HKVocals.DoLog($"Cargando AudioBundle asíncronamente desde archivo: {bundlePath}");
            }
            else
            {
                byte[] bundleBytes = Satchel.AssemblyUtils.GetBytesFromResources("audiobundle");
                if (bundleBytes != null && bundleBytes.Length > 0)
                {
                    loadRequest = AssetBundle.LoadFromMemoryAsync(bundleBytes);
                    HKVocals.DoLog("Cargando AudioBundle asíncronamente desde recursos incrustados.");
                }
            }

            if (loadRequest != null)
            {
                loadRequest.completed += SaveLoadedBundle;
            }
            else
            {
                HKVocals.DoLog("[ERROR] No se encontró la ruta del archivo ni el recurso para el audiobundle.");
            }
        }
        catch (Exception ex)
        {
            HKVocals.DoLog($"[ERROR] Excepción al iniciar la carga del AudioBundle: {ex.Message}");
        }
    }

    private static void SaveLoadedBundle(AsyncOperation operation)
    {
        audioBundle = loadRequest.assetBundle;

        if (audioBundle != null)
        {
            AudioNames.Clear();
            foreach (var audioName in audioBundle.GetAllAssetNames())
            {
                if (new[] { ".mp3", ".wav", ".ogg" }.Any(ext => audioName.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                {
                    AudioNames.Add(audioName.ToLower().Trim());
                }
            }
            AudioLoadSuccess = true;
            HKVocals.DoLog($"AudioBundle cargado correctamente. Total registrados: {AudioNames.Count}");
        }
        else
        {
            HKVocals.DoLog("[ERROR] No se pudo cargar el assetBundle desde la solicitud asíncrona.");
        }
    }

    private static void CreateTextPanel()
    {
        if (TextCanvas != null) return;

        TextCanvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1920, 1080));
        TextCanvas.name = "HKVocals Wait message";
        CanvasGroup cg = TextCanvas.GetComponent<CanvasGroup>();
        cg.interactable = false;
        cg.blocksRaycasts = false;

        GameObject background = CanvasUtil.CreateImagePanel
        (
            TextCanvas,
            CanvasUtil.NullSprite(new byte[] { 0x80, 0x00, 0x00, 0x00 }),
            new CanvasUtil.RectData(Vector2.zero, Vector2.zero, Vector2.zero, Vector2.one)
        );

        var TextPanel = CanvasUtil.CreateTextPanel
        (
            background,
            WaitText,
            60,
            TextAnchor.MiddleCenter,
            new CanvasUtil.RectData(new Vector2(-5, -5), Vector2.zero, Vector2.zero, Vector2.one),
            MenuResources.Perpetua
        );

        TextPanelText = TextPanel.GetComponent<Text>();
    }

    public static bool HasAudioFor(string key)
    {
        if (string.IsNullOrEmpty(key) || audioBundle == null) return false;

        string cleanKey = key.Trim();
        return audioBundle.Contains(cleanKey) || AudioNames.Any(name => name.Contains(cleanKey.ToLower()));
    }

    public static AudioClip GetAudioFor(string key)
    {
        if (string.IsNullOrEmpty(key) || audioBundle == null) return null;

        string cleanKey = key.Trim();

        if (audioCache.TryGetValue(cleanKey, out AudioClip cachedClip) && cachedClip != null)
        {
            if (cachedClip.loadState != AudioDataLoadState.Loaded) cachedClip.LoadAudioData();
            return cachedClip;
        }

        AudioClip clip = audioBundle.LoadAsset<AudioClip>(cleanKey);

        if (clip == null)
        {
            string matchingPath = AudioNames.FirstOrDefault(n => n.EndsWith($"/{cleanKey.ToLower()}.mp3") ||
                                                                n.EndsWith($"/{cleanKey.ToLower()}.wav") ||
                                                                n.EndsWith($"/{cleanKey.ToLower()}.ogg") ||
                                                                n.Contains(cleanKey.ToLower()));
            if (!string.IsNullOrEmpty(matchingPath))
            {
                clip = audioBundle.LoadAsset<AudioClip>(matchingPath);
            }
        }

        if (clip != null)
        {
            clip.LoadAudioData();
            audioCache[cleanKey] = clip;
            return clip;
        }

        return null;
    }
}