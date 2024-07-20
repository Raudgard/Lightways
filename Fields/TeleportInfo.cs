using System.Collections.Generic;
namespace Fields
{
    /// <summary>
    /// ���������� � ��������� ��� ������������.
    /// </summary>
    public class TeleportInfo : MatrixUnit
    {
        public List<Direction> inputDirections = new List<Direction>();

        public override string ToString()
        {
            return $"{{Teleport on ({X}, {Y})}}";
        }
    }
}