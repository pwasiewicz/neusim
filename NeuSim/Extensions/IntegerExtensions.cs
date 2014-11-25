namespace NeuSim.Extensions
{
    public static class IntegerExtensions
    {
        public static bool IsOutOfRange(this int? value, int minValueInclusive, int maxValueExclusive)
        {
            if (!value.HasValue)
            {
                return true;
            }

            return !(minValueInclusive <= value) || !(value < maxValueExclusive);
        }
    }
}
