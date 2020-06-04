using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiacEditor.Structures
{
    public struct TileSelectSpecifics
    {
        public bool? SolidTop_A { get; private set; }
        public bool? SolidTop_B { get; private set; }
        public bool? SolidLRB_A { get; private set; }
        public bool? SolidLRB_B { get; private set; }
        public bool? FlipX { get; private set; }
        public bool? FlipY { get; private set; }
        public int? Index { get; private set; }

        public TileSelectSpecifics(int? index)
        {
            Index = index;
            FlipX = null;
            FlipY = null;
            SolidTop_A = null;
            SolidTop_B = null;
            SolidLRB_A = null;
            SolidLRB_B = null;
        }

        public TileSelectSpecifics(int? index, bool? flipX, bool? flipY)
        {
            Index = index;
            FlipX = flipX;
            FlipY = flipY;
            SolidTop_A = null;
            SolidTop_B = null;
            SolidLRB_A = null;
            SolidLRB_B = null;
        }

        public TileSelectSpecifics(int? index, bool? flipX, bool? flipY, bool? solidTop_A, bool? solidTop_B, bool? solidLRB_A, bool? solidLRB_B)
        {
            Index = index;
            FlipX = flipX;
            FlipY = flipY;
            SolidTop_A = solidTop_A;
            SolidTop_B = solidTop_B;
            SolidLRB_A = solidLRB_A;
            SolidLRB_B = solidLRB_B;
        }

        public bool IsMatch(ushort _tile)
        {
            RSDKv5.Tile tile = new RSDKv5.Tile(_tile);
            bool Index_MATCH = (Index != null ? tile.Index == Index : true);
            bool FlipX_MATCH = (FlipX != null ? tile.FlipX == FlipX : true);
            bool FlipY_MATCH = (FlipY != null ? tile.FlipY == FlipY : true);
            bool SolidTop_A_MATCH = (SolidTop_A != null ? tile.SolidTopA == SolidTop_A : true);
            bool SolidTop_B_MATCH = (SolidTop_B != null ? tile.SolidTopB == SolidTop_B : true);
            bool SolidLRB_A_MATCH = (SolidLRB_A != null ? tile.SolidLrbA == SolidLRB_A : true);
            bool SolidLRB_B_MATCH = (SolidLRB_B != null ? tile.SolidLrbB == SolidLRB_B : true);
            return (Index_MATCH && FlipX_MATCH && FlipY_MATCH && SolidTop_A_MATCH && SolidTop_B_MATCH && SolidLRB_A_MATCH && SolidLRB_B_MATCH);
        }
    }
}
