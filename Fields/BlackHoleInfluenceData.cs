
namespace Fields
{
    /// <summary>
    /// �������� ���������� � ������� ������ ���� �� ���. �������, �������� �����, ����������� � �.�.
    /// </summary>
    public struct BlackHoleInfluenceData
    {
        public BlackHoleInfo blackHole;

        /// <summary>
        /// ����� ������ ����������� ���� ��� ������������ ������ ����. � "������" ������������ ��������� ������ �� ��, ��� � ������������ "���������".
        /// </summary>
        public (int X, int Y) inputPoint;

        /// <summary>
        /// ����� ��������� ����������� ���� ��� ������������ ������ ���� (����� ������� ��� ����� ���� �����).
        /// </summary>
        public (int X, int Y) outputPoint;

        /// <summary>
        /// ��������� ����� ����� ������� ���� �� (�� ��������� ��, ���� �� ������������ � ��������).
        /// </summary>
        public System.Collections.Generic.List<(int X, int Y)> freeCellsAfterBH;

        /// <summary>
        /// ����������� ���� �� �����������.
        /// </summary>
        public Direction directionBefore;

        /// <summary>
        /// ����������� ���� ����� �����������.
        /// </summary>
        public Direction directionAfter;

        /// <summary>
        /// ��� �� ����������, � ���� ����� � ������ ����.
        /// </summary>
        public bool isBeamGoingIntoBlackHole;


        public override string ToString()
        {
            return $"{{ BlackHoleInfluenceData:  blackHole: {blackHole}, " +
                $"inputPoint: {inputPoint}, " +
                $"outputPoint: {outputPoint}, " +
                $"freeCellsAfterBH: {freeCellsAfterBH.ToStringEverythingInARow()}, " +
                $"directionBefore: {directionBefore}, " +
                $"directionAfter: {directionAfter}, " +
                $"isBeamGoingIntoBlackHole: {isBeamGoingIntoBlackHole}.}}";
        }

    }
}
