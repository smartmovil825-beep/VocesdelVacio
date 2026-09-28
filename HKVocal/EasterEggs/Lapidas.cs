using Modding;

namespace HKVocals.EasterEggs;

public static class Lapidas
{
    public static void Hook()
    {
        ModHooks.LanguageGetHook += ChangeText;
    }

    public static void Unhook()
    {
        ModHooks.LanguageGetHook -= ChangeText;
    }

    private static string ChangeText(string key, string sheetTitle, string orig)
    {
        switch (key)
        {
            case "BELIEVE_TAB_01":
                return "SilverDubs";
            case "BELIEVE_TAB_02":
                return "Araly Delgado";
            case "BELIEVE_TAB_03":
                return "Thisisdanieldubstep";
            case "BELIEVE_TAB_04":
                return "CaydeOne";
            case "BELIEVE_TAB_05":
                return "Celes";
            case "BELIEVE_TAB_06":
                return "Abraxasdesu";
            case "BELIEVE_TAB_07":
                return "DBDUBS";
            case "BELIEVE_TAB_08":
                return "Mogabs";
            case "BELIEVE_TAB_09":
                return "littlecube";
            case "BELIEVE_TAB_10":
                return "Arturo Trejo";
            case "BELIEVE_TAB_11":
                return "Yun Amane";
            case "BELIEVE_TAB_12":
                return "Kaiju_0";
            case "BELIEVE_TAB_13":
                return "Yumi";
            case "BELIEVE_TAB_14":
                return "https://discord.gg/z-live";
            case "BELIEVE_TAB_15":
                return "Strelon018";
            case "BELIEVE_TAB_16":
                return "Mal Actor";
            case "BELIEVE_TAB_17":
                return "Bloodshot/The Dark Flame";
            case "BELIEVE_TAB_18":
                return "Aklla";
            case "BELIEVE_TAB_19":
                return "AXOLOCHI";
            case "BELIEVE_TAB_20":
                return "Estarosa97";
            case "BELIEVE_TAB_21":
                return "Ragde_Slayer";
            case "BELIEVE_TAB_22":
                return "Takuazin893";
            case "BELIEVE_TAB_23":
                return "Aldevion";
            case "BELIEVE_TAB_24":
                return "Antuan02";
            case "BELIEVE_TAB_25":
                return "GringaDubs";
            case "BELIEVE_TAB_26":
                return "Nazareno Gabriel Noro";
            case "BELIEVE_TAB_27":
                return "Shiondub";
            case "BELIEVE_TAB_28":
                return "Chinito811";
            case "BELIEVE_TAB_29":
                return "Ghostthkg";
            case "BELIEVE_TAB_30":
                return "MasterRD_TCS";
            case "BELIEVE_TAB_31":
                return "Akane";
            case "BELIEVE_TAB_32":
                return "Mailén Zalazar";
            case "BELIEVE_TAB_33":
                return "JamiVoice";
            case "BELIEVE_TAB_34":
                return "Xero";
            case "BELIEVE_TAB_35":
                return "Tapiochi";
            case "BELIEVE_TAB_36":
                return "Alek Figueroa";
            case "BELIEVE_TAB_37":
                return "José Javier Verdegay";
            case "BELIEVE_TAB_38":
                return "Kypirinha";
            case "BELIEVE_TAB_39":
                return "HelpyGRD";
            case "BELIEVE_TAB_40":
                return "Julio Navarro";
            case "BELIEVE_TAB_41":
                return "Liam Gamer";
            case "BELIEVE_TAB_42":
                return "PaoMiaw";
            case "BELIEVE_TAB_43":
                return "ErisCrisp";
            case "BELIEVE_TAB_44":
                return "HorizonJD";
            case "BELIEVE_TAB_45":
                return "BluePlay (Allison)";
            case "BELIEVE_TAB_46":
                return "Yurei tori";
            case "BELIEVE_TAB_47":
                return "Mey Rin";
            case "BELIEVE_TAB_48":
                return "D Pro game";
            case "BELIEVE_TAB_49":
                return "Ismelol";
            case "BELIEVE_TAB_50":
                return "El_Primi";
            case "BELIEVE_TAB_51":
                return "Alex Ramírez Morgan";
            case "BELIEVE_TAB_52":
                return "Raw_Sam";
            case "BELIEVE_TAB_53":
                return "Milo_chocomilk";
            case "BELIEVE_TAB_54":
                return "DrewJK";
            case "BELIEVE_TAB_55":
                return "Akurashi";
            case "BELIEVE_TAB_56":
                return "CrimsonKED";
            case "BELIEVE_TAB_57":
                return "SoyRubenGD";
            case "BELIEVE_TAB_58":
                return "Dario Sandes";
            case "BELIEVE_TAB_59":
                return "Sammael-Orvanys";
            case "BELIEVE_TAB_60":
                return "jlopezr1";
            case "BELIEVE_TAB_61":
                return "ManuerDubs";
            case "BELIEVE_TAB_62":
                return "TioOso";
            case "BELIEVE_TAB_63":
                return "Z-Live";

            default:
                return orig;
        }
    }
}