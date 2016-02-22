namespace LinQDemo
{
    public static class Extensions
    {
        /// <summary>
        /// Retuns true if odd number
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsOdd(this int value)
        {
            return value%2 != 0;
        }

        /// <summary>
        /// Returns true if even number
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEven(this int value)
        {
            return !IsOdd(value);
        }
    }
}