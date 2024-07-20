using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Saving
{
    /// <summary>
    /// Static class for Player Prefs
    /// </summary>
    public class SCFPP
    {
        public static class Miscellaneous
        {
            /// <summary>
            /// ����� ������ ��������� ��������. -1 - ������ �� ��������, 0 - �������� ������ , 1 - �������� ������, ... , 4 - �������� �����.
            /// </summary>
            public static string difficultiesPassed = "difficultysPassed";
            public static string levelsPassed = "levelsPassed";
            public static string timeLastAddingKeys = "timeLastAddingKeys";
            public static string timeNextAddingKeys = "timeNextAddingKeys";

            /// <summary>
            /// ���������� ����� � ������� �� ��� �����.
            /// </summary>
            public static string winLevelsCount = "winLevelsCount";

            /// <summary>
            /// ������������ ��� ������� �����.
            /// </summary>
            public static string userHasAlreadyReview = "userHasAlreadyReview";

            /// <summary>
            /// ��������� ������� ������ In-App-Review.
            /// </summary>
            public static string inAppReviewCalledTime = "inAppReviewCalledTime";


            public static string playerName = "playerName";

            /// <summary>
            /// ������ �� ��� ������������� database �� PlayerIO.
            /// </summary>
            public static string isFirstTimeToGetDB = "isFirstTimeToGetDB";

            ///// <summary>
            ///// �������� ������� ������.
            ///// </summary>
            //public static string userLevelsNames = "userLevelsNames";

        }



        //public static string randomPhotonsLevelSize = "randomPhotonsLevelSize";
        //public static string randomPhotonsTeleportsCount = "randomPhotonsTeleportsCount";
        //public static string randomPhotonsBlackHolesCount = "randomPhotonsBlackHolesCount";
        //public static string randomPhotonsDarkness = "randomPhotonsDarkness";


        public static class ModesSliders
        {
            public static class RandomPhotons
            {
                public static string levelSize = "randomPhotonsLevelSize";
                public static string teleportsCount = "randomPhotonsTeleportsCount";
                public static string blackHolesCount = "randomPhotonsBlackHolesCount";
                public static string darkness = "randomPhotonsDarkness";
            }

            public static class FasterThanLight
            {
                public static string levelSize = "fasterThanLightLevelSize";
                public static string blackHolesCount = "fasterThanLightBlackHolesCount";
                public static string darkness = "fasterThanLightDarkness";
            }

            public static class BlackHolesRiddles
            {
                public static string levelSize = "blackHolesRiddlesLevelSize";
                public static string teleportsCount = "blackHolesRiddlesTeleportsCount";
                public static string blackHolesCount = "blackHolesRiddlesBlackHolesCount";
                
            }

        }


        public static class Options
        {
            public static string language = "language";
            /// <summary>
            /// ���� ��� ������� ��� ������ �������.
            /// </summary>
            public static string languageHasAlreadyBeenChanged = "languageHasAlreadyBeenChanged";
            public static string languageDropdownValue = "languageDropdownValue";

            public static string fullScreenMode = "fullScreenMode";
            public static string showBHSize = "showBHSize";
            public static string showBHArea = "showBHArea";


            public static string isMusicOn = "isMusicOn";
            public static string isSoundsOn = "isSoundsOn";

            public static string musicVolume = "musicVolume";
            public static string soundsVolume = "soundsVolume";

            public static string colorTheme = "colorTheme";

        }








    }
}