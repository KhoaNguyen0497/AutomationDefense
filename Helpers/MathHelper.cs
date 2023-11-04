using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationDefense.Helpers
{
    public static class MathHelper
    {
        public static Random Random = new Random();
        public static bool Chance(int chance)
        {
            if (chance == 0)
            {
                return false;
            }

            if (chance == 100)
            {
                return true;
            }

            return Random.Next(0, 100) < chance;
        }

        public static T RandomElement<T>(this IEnumerable<T> items)
        {
            return items.ElementAt(Random.Next(0, items.Count()));
        }

        public static IEnumerable<int> DivideEvenly(int numerator, int denominator)
        {
            int rem;
            int div = Math.DivRem(numerator, denominator, out rem);

            for (int i = 0; i < denominator; i++)
            {
                yield return i < rem ? div + 1 : div;
            }
        }

    }
}
