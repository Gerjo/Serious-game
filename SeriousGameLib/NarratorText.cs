using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeriousGameLib
{
    public static class NarratorText
    {
        public static string[] AfleveringBenzine =
           new string[] {  "OH NEE!", 
                            "De benzine is op en je zit voorlopig vast.."
            };
        public static string[] AfleveringWinScreenCaptions =
           new string[] {  "Een Gouden Trofee!",
                            "Een Zilveren Trofee!",
                            "Probeer het nog eens!",
            };


        // Shown when the minigame is first loaded.
        public static string SchilderWelcomeText = "Welkom bij de schilder, meng het verf, en kleur de tekening na!";

        public static string SchilderButtonUndo = "Ongedaan Maken";
        public static string SchilderButtonSend = "Ik ben klaar!";

        // The new score is better than the previous:
        public static string[] SchilderMotivational =
            new string[] {  "Goed gedaan! Je tekening is nu voor {0}% gelijk.", 
                            "Netjes! De tekening is al voor {0}% gelijk.",
                            "Chapeau! De tekening is al voor {0}% gelijk." 
            };

        // The new score is worse than the previous:
        public static string[] SchilderDemotivational =
            new string[] {  "Een verkeerde kleur! Je tekening is nu voor {0}% gelijk.", 
                            "Probeer het nog een keer! Je tekening is voor {0}% gelijk." 
            };

        // The caption title at the winscreen, one for each trophy:
        public static string[] SchilderWinScreenCaptions =
            new string[] {  "Een Gouden Trofee!",
                            "Een Zilveren Trofee!",
                            "Volgende keer beter",
            };

        // The body text at the winscreen, one for each trophy:
        public static string[] SchilderWinScreenText =
            new string[] {  "De klant is tevreden met het schilderij, \njij verdient een trofee!",
                            "De klant is tevreden, dat verdient een trofee!",
                            "De tekening is nog niet helemaal af, \nmaar je verdient toch een trofee!",
            };

        // This is printed on the "winscreen" button.
        public static string ButtonReturnToOffice = "Ga terug naar kantoor";

        public static string OntwerpenWelcomeText = "Welkom bij de ontwerpen! Zoek de verschillende kaarten in zo min mogelijk beurten bij elkaar!";


        // The caption title at the winscreen, one for each trophy:
        public static string[] OntwerpenWinScreenCaptions =
            new string[] {  "Een gouden Trofee!",
                            "Een zilveren Trofee!",
                            "Nog even oefenen",
            };

        // The body text at the winscreen, one for each trophy:
        public static string[] OntwerpenWinScreenText =
            new string[] {  "Jij hebt heel snel alle dieren bij elkaar \ngezocht, dat verdient een trofee!",
                            "Goed gedaan! Nog even oefenen en je \nontvangt ook een gouden trofee!",
                            "Het duurde even, maar je hebt als nog \neen bronzen trofee verdient!",
            };

        // The new score is better than the previous:
        public static string[] OntwerpenMotivational =
            new string[] {  "Je hebt een nieuwe set gevonden! Dit is beurt nummer {0}.", 
            };

        // The new score is worse than the previous:
        public static string[] OntwerpenDemotivational =
            new string[] {  "Probeer het nog een keer! Dit is beurt nummer {0}.", 
                            "Die twee dieren zien niet gelijk! Dit is beurt nummer {0}.", 
            };

        // Shown when the minigame is first loaded.
        public static string UitnodigingWelcomeText = "Versleep de stukjes naar de juiste plek, los the puzzel zo snel mogelijk op!";


        // The caption title at the winscreen, one for each trophy:
        public static string[] UitnodigingWinScreenCaptions =
            new string[] {  "Een Gouden Trofee!",
                            "Een zilveren Trofee!",
                            "Nog even oefenen",
            };

        // The body text at the winscreen, one for each trophy:
        public static string[] UitnodigingWinScreenText =
            new string[] {  "Dat heb je snel gedaan! Je verdient \neen gouden trofee",
                            "Goed werk! Nog ietje sneller werken \nen je krijgt een gouden trofee.",
                            "Nog even oefenen, maar toch krijg je een trofee",
            };

        public static string UitnodigingTimer = "Los de puzzle op! Jij bent nu {0} bezig.";

        // In order: AFLEVERING = 0, FOTOGRAAF = 1, ONTWERPEN = 2, SCHILDER = 3, UITNODIGING = 4, NONE = 5, MENU = 6
        public static string[] HoverTexts =
            new string[] {  "Dit is de aflevering.", 
                            "Dit is de fotograaf.",
                            "Dit zijn de ontwerpen.",
                            "Dit is de schilder.",
                            "Dit is de uitnodiging.",
                            "Not used.",
                            "Ga naar het menu.",
               
            };

        public static string ConfirmGotoOfficeText      = "Weet je zeker dat je terug wilt? \nJe verdient dan geen trofee!";
        public static string ButtonConfirmGotoOfficeYes = "Naar kantoor";
        public static string ButtonConfirmGotoOfficeNo  = "Blijven Spelen";


        // ORDER: 0 =>"fotograaf", "schilder", "aflevering", "uitnodiging", "ontwerpen", "kantoor"
        public static string[] MenuHover = new String[] {
            "Start de fotograaf!",
            "Start het schilder spel!",
            "Start de aflevering, een racespel!",
            "Start de uitnodiging, een leg puzzel spel!",
            "Start de ontwerpen, een memory spel.",
            "Ga naar het interactieve kantoor.",
            };

        // Shown when the minigame is first loaded.
        public static string FotograafWelcomeText = "Maak vier mooie foto's van de kat! Gebruik je scrollwiel of pijltjes om te zoomen!";

        // The caption title at the winscreen, one for each trophy:
        public static string[] FotograafWinScreenCaptions =
            new string[] {  "Een Gouden Trofee!",
                            "Een zilveren Trofee!",
                            "Nog even oefenen",
            };

        // The body text at the winscreen, one for each trophy:
        public static string[] FotograafWinScreenText =
            new string[] {  "Dat zijn de beste foto's ooit! \nDat verdient een gouden trofee!",
                            "Goed gedaan! Nog even oefenen en je \nontvangt ook een gouden trofee!",
                            "Het zijn niet de beste foto's ooit,\nmaar toch verdien je een trofee.",
            };
    }
}
