
namespace Fields
{
    /// <summary>
    /// Содержит информацию о влиянии черной дыры на луч. Входная, выходная точки, направление и т.д.
    /// </summary>
    public struct BlackHoleInfluenceData
    {
        public BlackHoleInfo blackHole;

        /// <summary>
        /// Точка начала искривления луча под воздействием черной дыры. В "прямых" направлениях находится дальше от ЧД, чем в направлениях "наискосок".
        /// </summary>
        public (int X, int Y) inputPoint;

        /// <summary>
        /// Точка окончания искривления луча под воздействием черной дыры (после которой луч опять идет прямо).
        /// </summary>
        public (int X, int Y) outputPoint;

        /// <summary>
        /// Свободные точки после влияния этой ЧД (до следующей ЧД, либо до столкновения с объектом).
        /// </summary>
        public System.Collections.Generic.List<(int X, int Y)> freeCellsAfterBH;

        /// <summary>
        /// Направление луча до воздействия.
        /// </summary>
        public Direction directionBefore;

        /// <summary>
        /// Направление луча после воздействия.
        /// </summary>
        public Direction directionAfter;

        /// <summary>
        /// Луч не искажается, а идет прямо в черную дыру.
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
