using ExternEngine;
using UnityEngine;

namespace Framework.Base
{
    public class ConstDef
    {
        public static float PROGRESS_END_SNAP = 0.99f;
        public static Vector3 INVAILD_POS = new Vector3(-9000, -9000, -9000);
        public static FFloat STEP_HEIGHT_LOWER = 0.5f;
        public static FFloat JUMP_HEIGHT_LOWER = 1.0f;
        public static FFloat AUTO_JUMP_HEIGHT_LOWER = 1.5f;
        public static FFloat GTRAVITY_VALUE = new FFloat(9.8f);
        public static FFloat TERRAIN_HIT_DISTANCE = 100.0f;

        public static Texture TransparencyTexture;
        public static Sprite TransparencySprite;
    }
}
