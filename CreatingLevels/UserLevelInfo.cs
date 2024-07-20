using System.Text;

namespace UserLevels
{
    /// <summary>
    /// Информация о сохраненном на сервере PIO уровне игрока.
    /// </summary>
    public class UserLevelInfo
    {
        /// <summary>
        /// Средняя оценка уровня от игроков.
        /// </summary>
        public float rating;

        /// <summary>
        /// Сколько раз уровень был запущен.
        /// </summary>
        public uint launched;

        /// <summary>
        /// Сколько раз уровень был пройден.
        /// </summary>
        public uint passed;

        /// <summary>
        /// Количество оценок, выставленных игроками.
        /// </summary>
        public uint rated;

        /// <summary>
        /// Уникальный порядковый номер уровня. Начинается с 0, каждый следующий +1, присваивается после публикации.
        /// </summary>
        public uint levelNumber;

        /// <summary>
        /// Уникальный порядковый номер уровня. Начинается с 0, каждый следующий +1, присваивается после публикации.
        /// </summary>
        public uint reward;

        /// <summary>
        /// Название уровня.
        /// </summary>
        public string levelName;

        /// <summary>
        /// Имя игрока, создавшего уровень.
        /// </summary>
        public string userName;

        /// <summary>
        /// ID игрока, создавшего уровень.
        /// </summary>
        public string userID;

        /// <summary>
        /// Сам уровень в JSON.
        /// </summary>
        public string level;

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"\nrating: {rating}\n");
            stringBuilder.Append($"launched: {launched}\n");
            stringBuilder.Append($"passed: {passed}\n");
            stringBuilder.Append($"rated: {rated}\n");
            stringBuilder.Append($"levelNumber: {levelNumber}\n");
            stringBuilder.Append($"reward: {reward}\n");
            stringBuilder.Append($"levelName: {levelName}\n");
            stringBuilder.Append($"userName: {userName}\n");
            stringBuilder.Append($"userID: {userID}\n");
            stringBuilder.Append($"level: {level}");
            return stringBuilder.ToString();
        }

    }
}