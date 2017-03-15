using System;
using System.Collections.Generic;

namespace GeocachingToolbox.GeocachingCom.MapFetch
{


    public static class UTFGrid
    {
        private const int GRID_MAXX = 63;
        private const int GRID_MAXY = 63;

        /** Calculate from a list of positions (x/y) the coords */
        public static UTFGridPosition GetPositionInGrid(List<UTFGridPosition> positions)
        {
            int minX = GRID_MAXX;
            int maxX = 0;
            int minY = GRID_MAXY;
            int maxY = 0;
            foreach (UTFGridPosition pos in positions)
            {
                minX = Math.Min(minX, pos.x);
                maxX = Math.Max(maxX, pos.x);
                minY = Math.Min(minY, pos.y);
                maxY = Math.Max(maxY, pos.y);
            }
            return new UTFGridPosition((minX + maxX) / 2, (minY + maxY) / 2);
        }

    }
}