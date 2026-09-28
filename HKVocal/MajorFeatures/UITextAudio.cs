using SFCore.Utils;
using MonoMod.RuntimeDetour;
namespace HKVocals.MajorFeatures;
public class UITextAudio
{
    private static bool HunterNotesUnlocked = false;
    public static bool OpenShopMenu = false;
    public static bool OpenInvMenu = false;
    public static bool ShopMenuClosed = true;
    public static bool InvMenuClosed = true;
    public static bool AudioQueued = false;

    public static void Hook()
    {
        FSMEditData.AddGameObjectFsmEdit("Enemy List", "Item List Control", PlayJournalText );
        FSMEditData.AddGameObjectFsmEdit ("Inv", "Update Text", PlayInventoryText );
        FSMEditData.AddGameObjectFsmEdit ("Charms", "Update Text", PlayCharmText );
        FSMEditData.AddGameObjectFsmEdit ("Item List", "Item List Control", PlayShopText );
        FSMEditData.AddGameObjectFsmEdit ("Shop Menu", "shop_control", ShopMenuOpenClose );
        FSMEditData.AddGameObjectFsmEdit ("Inventory", "Inventory Control", InventoryOpenClose );
        FSMEditData.AddGameObjectFsmEdit("Enemy List", "Item List Control Custom", PlayEquipmentText);
        //new Hook(typeof(SFCore.ItemHelper).GetMethod("CreateEquipmentPane", BindingFlags.Static | BindingFlags.NonPublic), PlayEquipmentText);
    }

    static IEnumerator AudioStop()
    {
        yield return new WaitUntil(AudioPlayer.IsPlaying);
        AudioPlayer.StopPlaying();
    }

    public static void PlayCharmText(PlayMakerFSM fsm)
    {
        Core.FsmUtil.FsmUtil.AddFsmMethod(fsm,"Change Text", () => { MixerLoader.SetSnapshot(Snapshots.Cliffs); fsm.PlayUIText("Convo Desc", UIAudioType.Other); });
    }

    public static void PlayInventoryText(PlayMakerFSM fsm)
    {
        Core.FsmUtil.FsmUtil.AddFsmMethod(fsm,"Change Text", () => { MixerLoader.SetSnapshot(Snapshots.Cliffs); fsm.PlayUIText("Convo Desc", UIAudioType.Other); });
    }

    public static void PlayJournalText(PlayMakerFSM fsm)
    {
        Core.FsmUtil.FsmUtil.AddFsmMethod(fsm,"Display Kills", () => { HunterNotesUnlocked = false; });
        Core.FsmUtil.FsmUtil.AddFsmMethod(fsm,"Get Notes", () => { HunterNotesUnlocked = true; });

        Core.FsmUtil.FsmUtil.AddFsmMethod(fsm,"Get Details", () =>
        {
            HKVocals.CoroutineHolder.StartCoroutine(HunterWait(fsm));
        });
    }

    static IEnumerator HunterWait(PlayMakerFSM fsm)
    {
        yield return new WaitForSeconds(0.3f);
        HKVocals.CoroutineHolder.StartCoroutine(HunterCode(fsm));
    }

    static IEnumerator HunterCode(PlayMakerFSM fsm)
    {
        var audio = "";

        if (HunterNotesUnlocked == true)
        {
            audio = "Item Notes Convo";
        }
        else if (HunterNotesUnlocked == false)
        {
            audio = "Item Desc Convo";
        }

        MixerLoader.SetSnapshot(Snapshots.Cliffs);
        fsm.PlayUIText(audio, UIAudioType.Other);
        yield return audio;
    }

    //public static void PlayEquipmentText(PlayMakerFSM fsmAction<GameObject> orig, GameObject newPaneGo)
    public static void PlayEquipmentText(PlayMakerFSM fsm)
    {
        // orig(newPaneGo);
        // PlayMakerFSM fsm = newPaneGo.FindGameObjectInChildren("Enemy List").LocateMyFSM("Item List Control Custom");
        Core.FsmUtil.FsmUtil.AddFsmMethod(fsm,"Get Details", () => { MixerLoader.SetSnapshot(Snapshots.Cliffs); fsm.PlayUIText("Item Notes Convo", UIAudioType.Other); });
    }


    public static void ShopMenuOpenClose(PlayMakerFSM fsm)
    {
        //Checks when you open the shop keeper menu
        Core.FsmUtil.FsmUtil.AddFsmMethod(fsm,"Open Window", () => {
            if (HKVocals._globalSettings.dampenAudio == true)
            {
                DampenAudio.StartDampingAudioNormal();
            }
            OpenShopMenu = true;
            ShopMenuClosed = false;
        });

        //Checks when you close a shop keeper menu
        Core.FsmUtil.FsmUtil.AddFsmMethod(fsm,"Down", () => {
            if (HKVocals._globalSettings.dampenAudio == true)
            {
                DampenAudio.ForceStopDampenAudio();
            }
            if (AudioQueued == true)
            {
                HKVocals.CoroutineHolder.StartCoroutine(AudioStop());
            }
            AudioPlayer.StopPlaying();
            ShopMenuClosed = true;
            OpenShopMenu = false;
        });
    }

    public static void InventoryOpenClose(PlayMakerFSM fsm)
    {
        Core.FsmUtil.FsmUtil.AddFsmMethod(fsm,"Open", () => {
            if (HKVocals._globalSettings.dampenAudio == true)
            {
                DampenAudio.StartDampingAudioNormal();
            }
            OpenInvMenu = true;
            InvMenuClosed = false;
        });

        Core.FsmUtil.FsmUtil.AddFsmMethod(fsm,"Close", () => {
            if (HKVocals._globalSettings.dampenAudio == true)
            {
                DampenAudio.ForceStopDampenAudio();
            }
            if (AudioQueued == true)
            {
                HKVocals.CoroutineHolder.StartCoroutine(AudioStop());
            }
            AudioPlayer.StopPlaying();
            InvMenuClosed = true;
            OpenInvMenu = false;
        });
    }

    public static void PlayShopText(PlayMakerFSM fsm)
    {
        Core.FsmUtil.FsmUtil.AddFsmMethod(fsm,"Get Details Init", () => { fsm.PlayUIText("Item Desc Convo", UIAudioType.Shop); });
        Core.FsmUtil.FsmUtil.AddFsmMethod(fsm,"Get Details", () => { fsm.PlayUIText("Item Desc Convo", UIAudioType.Shop); });
    }
}

public enum UIAudioType
{
    Shop,
    Other
}
