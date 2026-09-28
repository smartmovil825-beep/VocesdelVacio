using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKVocals.MajorFeatures
{
    public class Patches
    {
        internal static void Hook()
        {
            ModHooks.LanguageGetHook += LanguageGetPatches;
        }

        private static string LanguageGetPatches(string key, string sheetTitle, string orig)
        {
            if (key == "SHOP_DESC_NOTCH_4")
                return orig.Replace("more charms", "más de tus amuletos");
            if (key == "HORNET_FOUNTAIN_1")
                return "Nos volvemos a encontrar, pequeña sombra.<page>Normalmente soy bastante perceptiva. Sin embargo, te subestimé, aunque desde entonces he adivinado la verdad.<page>Has visto más allá de los límites de este reino. La tuya es una resiliencia nacida de dos vacíos.<page>No es de extrañar entonces que hayas logrado alcanzar el corazón de este mundo. Al hacerlo, conocerás el sacrificio que lo mantiene en pie.";
            if (key == "WITCH_REWARD_2A")
                return "Ahhh... Tu Aguijón Feérico contiene más de 200 de Esencia. Estás demostrando tu talento para recolectarla.<page>¿Has visto esa gran puerta justo ahí fuera? Mi tribu la cerró hace mucho tiempo y prohibió su apertura.<page>Ah, pero hasta donde sé, soy el único miembro de mi tribu que sigue con vida. Eso significa que no debo sentirme mal por romper un tabú. Como prueba de mi fe en ti, abriré la puerta.";
            if (key == "WATERWAYS_GREET")
                return "¡Ho ho! ¿Acaso no son apasionantes estas vías de agua? Un laberinto de tuberías y túneles.<page>No podría haber pedido un lugar mejor para emplear mis talentos. Todo es tan ordenado, tan pensado... nada que ver con la burda irregularidad de esas cavernas.<page>Ahh, pero qué lástima, mi olfato me dice que los Páramos Fúngicos están cerca y presiento que mi húmeda aventura puede haber llegado a su fin. Supongo que daré esta por terminada.";
            if (key == "WATERWAYS_BOUGHT")
                return "Apostaría a que estas tuberías y cámaras se usaban antes para transportar los desechos de la ciudad. Debe de haber habido un hedor horrible aquí abajo. Por suerte, la lluvia constante las ha dejado limpias.";
            return orig;
        }
    }
}
