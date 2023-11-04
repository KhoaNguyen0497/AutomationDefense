using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationDefense.Helpers
{
    public static class VectorHelper
    {
        public static bool WithinDistance(Vector2 value1, Vector2 value2, float distance)
        {
            float v1 = value1.X - value2.X, v2 = value1.Y - value2.Y;
            return ((v1 * v1) + (v2 * v2)) < distance * distance;
        }

    }
}
