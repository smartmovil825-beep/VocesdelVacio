using SFCore;
using Satchel;

namespace HKVocals.MajorFeatures;

public static class Achievements
{
    public static void Hook() 
    {
        AchievementHelper.AddAchievement("ImpatientLemm",AssemblyUtils.GetSpriteFromResources("HKVocals.Resources.Impateint.png",66f),"Impatient","Leave a relic in Lemm’s deposit box.",true);
        AchievementHelper.AddAchievement("DisdainGrub",AssemblyUtils.GetSpriteFromResources("HKVocals.Resources.Distain.png",66f),"Disdain","Read the dreams of a particularly ungrateful Grub.",true);
        AchievementHelper.AddAchievement("KindnessPaleLurker",AssemblyUtils.GetSpriteFromResources("HKVocals.Resources.Kindness.png",66f),"Kindness","Show the Pale Lurker a new perspective on life.",true);
        AchievementHelper.AddAchievement("LastLaughOrdeal",AssemblyUtils.GetSpriteFromResources("HKVocals.Resources.Last_Laugh.png",66f),"Last Laugh","Hit the lever below The Eternal Ordeal’s Zote statue.",true);
        AchievementHelper.AddAchievement("AlubafarDreamnail",AssemblyUtils.GetSpriteFromResources("HKVocals.Resources.Alubafar.png",66f),"Alubafar","Listen to what an Aluba has to say",true);

        AchievementHelper.AddAchievement("CompendiumVocalization",AssemblyUtils.GetSpriteFromResources("HKVocals.Resources.Full_compendium.png",66f),"Compendium Vocalization","Listen to every line of vocalized dialogue in Hallownest.",false);
        AchievementHelper.AddAchievement("Consideration",AssemblyUtils.GetSpriteFromResources("HKVocals.Resources.All_Dialogue.png",66f),"Consideration","Listen to every word of Hallownest’s living inhabitants.",false);
        AchievementHelper.AddAchievement("Ambition",AssemblyUtils.GetSpriteFromResources("HKVocals.Resources.All_Dreams.png",66f),"Ambition","Uncover every dream, of bug and spirit alike.",false);
        AchievementHelper.AddAchievement("Chronology",AssemblyUtils.GetSpriteFromResources("HKVocals.Resources.All_Lore_Tablets.png",66f),"Chronology","Find every lore tablet buried under this dead Kingdom.",false);
        AchievementHelper.AddAchievement("Acquisition",AssemblyUtils.GetSpriteFromResources("HKVocals.Resources.All_Items_and_Journal.png",66f),"Acquisition","Review every item and journal entry there is to acquire.",false);
        
        ModHooks.LanguageGetHook += AchLang;
        OnEnemyDreamnailReaction.BeforeOrig.RecieveDreamImpact += Aluba;
    }

    private static string AchLang(string key, string sheettitle, string orig)
    {
        return key switch
        {
            "Impatient" => "Impaciente",
            "Leave a relic in Lemm’s deposit box." => "Deja una reliquia en la caja de depósito de Lemm.",
            "Disdain" => "Desdén",
            "Read the dreams of a particularly ungrateful Grub." => "Lee los sueños de una larva particularmente desagradecida.",
            "Kindness" => "Bondad",
            "Show the Pale Lurker a new perspective on life." => "Muéstrale a la Acechadora Pálida una nueva perspectiva de la vida.",
            "Last Laugh" => "Última risa",
            "Hit the lever below The Eternal Ordeal’s Zote statue." => "Golpea la palanca debajo de la estatua de Zote en la Eterna Ordalía.",
            "Alubafar" => "Alubafar",
            "Listen to what an Aluba has to say" => "Escucha lo que un Aluba tiene para decir.",
            "Compendium Vocalization" => "Compendio de vocalización",
            "Listen to every line of vocalized dialogue in Hallownest." => "Escucha cada línea de diálogo doblada en Hallownest.",
            "Consideration" => "Consideración",
            "Listen to every word of Hallownest’s living inhabitants." => "Escucha cada palabra de los habitantes vivos de Hallownest.",
            "Ambition" => "Ambición",
            "Uncover every dream, of bug and spirit alike." => "Descubre cada sueño, tanto de bichos como de espíritus.",
            "Chronology" => "Cronología",
            "Find every lore tablet buried under this dead Kingdom." => "Encuentra cada tabla de historia enterrada bajo este reino muerto.",
            "Acquisition" => "Adquisición",
            "Review every item and journal entry there is to acquire." => "Revisa cada objeto y entrada del diario por adquirir.",
            _ => orig
        };
    }

    private static void Aluba(OnEnemyDreamnailReaction.Delegates.Params_RecieveDreamImpact args)
    {
        if (args.self.gameObject.name == "Aluba")
        {
            GameManager.instance.AwardAchievement("AlubafarDreamnail");
        }
    }
}
