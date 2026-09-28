namespace HKVocals.EasterEggs;
public static class SpecialGrub
{
    public static string SpeicalGrubSceneName = "Abyss_19";
    public static void Hook()
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += EditSpecialGrub;
        ModHooks.LanguageGetHook += GetSpecialGrubDialogue;
    }

    public static void EditSpecialGrub(Scene From, Scene To)
    {
        if (To.name == "Abyss_19")
        {
            GameObject.Find("Grub Bottle").transform.GetChild(0).GetChild(0).gameObject.AddComponent<OnDreamNail>();
        }
    }

    private class OnDreamNail : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D col)
        {
            if(col.tag == "Dream Attack")
            {
                MixerLoader.SetSnapshot(Snapshots.Dream);
                if (HKVocals._saveSettings.GrubConvo < 8) 
                {
                    HKVocals._saveSettings.GrubConvo += 1;
                    GameObject.Find("Grub Bottle").transform.GetChild(0).GetChild(0).GetComponent<PlayMakerFSM>().GetFsmStringVariable("Sheet Name").Value = "Elderbug";
                    GameObject.Find("Grub Bottle").transform.GetChild(0).GetChild(0).GetComponent<PlayMakerFSM>().GetFsmStringVariable("Convo Name").Value = $"GRUB_BOTTLE_DREAM_S_{HKVocals._saveSettings.GrubConvo}";
                    AudioPlayer.TryPlayAudioFor($"GRUB_BOTTLE_DREAM_S_{HKVocals._saveSettings.GrubConvo}");
                }
                else
                {
                    GameManager.instance.AwardAchievement("DisdainGrub");
                    GameObject.Find("Grub Bottle").transform.GetChild(0).GetChild(0).GetComponent<PlayMakerFSM>().GetFsmStringVariable("Convo Name").Value = $"GRUB_BOTTLE_DREAM_S_REPEAT_0";
                    GameObject.Find("Grub Bottle").transform.GetChild(0).GetChild(0).GetComponent<PlayMakerFSM>().GetFsmStringVariable("Sheet Name").Value = "Elderbug";
                }
            }
        }
    }
    public static string GetSpecialGrubDialogue(string key, string sheettitle, string orig)
    {
        return key switch
        {
            "GRUB_BOTTLE_DREAM_S_0" => " ...Hogar...",
            "GRUB_BOTTLE_DREAM_S_1" => "¿Por qué me mira fijamente de esa manera?",
            "GRUB_BOTTLE_DREAM_S_2" => "¿Acaso no ha venido a liberarme? ¿A salvarme de este cruel destino?",
            "GRUB_BOTTLE_DREAM_S_3" => "Una y otra vez retira su puño, como si se preparase para romper esta prisión invisible. Pero solo le da al aire.",
            "GRUB_BOTTLE_DREAM_S_4" => "No desea destruir lo que me confina, sino mi orgullo.",
            "GRUB_BOTTLE_DREAM_S_5" => "¿De verdad pretende burlarse y avergonzar a una indefensa larva como yo? Qué insecto tan malvado debe ser para prolongar esta tortura a sabiendas, alejado de los míos.",
            "GRUB_BOTTLE_DREAM_S_6" => "De mi... Padre Larva.",
            "GRUB_BOTTLE_DREAM_S_7" => "Cuando llegue el momento adecuado y este insecto menos se lo espere...",
            "GRUB_BOTTLE_DREAM_S_8" => "Le devolveré el favor con mucho gusto.",
            "GRUB_BOTTLE_DREAM_S_REPEAT_0" => "...",
            _ => orig
        };
    }
}
