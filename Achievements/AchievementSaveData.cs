using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Achievements;

/// <summary>
///  ласс, содержащий данные дл€ сохранени€ состо€ни€ достижений.
/// </summary>
[Serializable]
public class AchievementSaveData
{
    public Achievement_Type achievementType;

    /// <summary>
    /// –езультат, необходимый дл€ получени€ этого достижени€. ƒл€ разных достижений разный тип. «везды, количество секунд, число искривлений луча и др.
    /// </summary>
    public int resultRequiredToCompleteAchievement;

    ///// <summary>
    /////  оличество звезд, максимально возможное за конкретный уровень. Ќе дл€ всех достижений.
    ///// </summary>
    //public int starsMax;

    /// <summary>
    /// ќсновной результат достижени€.
    /// ƒл€ достижений в режимах - это количество набранных звезд.
    /// ƒл€ скоростных достижений - это количество оставшихс€ секунд.
    /// ƒл€ достижени€ искривлени€ - это максимальное количество искривлений одного луча.
    /// ƒл€ достижений типа "получено / не получено": 1 - получено, 0 - не получено.
    /// </summary>
    public int mainResult;


}
