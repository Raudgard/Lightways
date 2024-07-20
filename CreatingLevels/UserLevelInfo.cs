using System.Text;

namespace UserLevels
{
    /// <summary>
    /// ���������� � ����������� �� ������� PIO ������ ������.
    /// </summary>
    public class UserLevelInfo
    {
        /// <summary>
        /// ������� ������ ������ �� �������.
        /// </summary>
        public float rating;

        /// <summary>
        /// ������� ��� ������� ��� �������.
        /// </summary>
        public uint launched;

        /// <summary>
        /// ������� ��� ������� ��� �������.
        /// </summary>
        public uint passed;

        /// <summary>
        /// ���������� ������, ������������ ��������.
        /// </summary>
        public uint rated;

        /// <summary>
        /// ���������� ���������� ����� ������. ���������� � 0, ������ ��������� +1, ������������� ����� ����������.
        /// </summary>
        public uint levelNumber;

        /// <summary>
        /// ���������� ���������� ����� ������. ���������� � 0, ������ ��������� +1, ������������� ����� ����������.
        /// </summary>
        public uint reward;

        /// <summary>
        /// �������� ������.
        /// </summary>
        public string levelName;

        /// <summary>
        /// ��� ������, ���������� �������.
        /// </summary>
        public string userName;

        /// <summary>
        /// ID ������, ���������� �������.
        /// </summary>
        public string userID;

        /// <summary>
        /// ��� ������� � JSON.
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