using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace R1Engine
{
    /// <summary>
    /// Robin's Rayman 1 cage location randomizer
    /// </summary>
    public static class CageLocationRandomizer
    {
        public const float MaxGenDoorFromCageDist = 500;
        public const float MaxCageFromOriginalSpotDist = 1200;

        private static List<R1_EventType> BannedTargets = new List<R1_EventType>()
        {
            R1_EventType.MS_nougat,
            R1_EventType.MS_poing_plate_forme,
            R1_EventType.MS_porte,
            R1_EventType.TYPE_ANNULE_SORT_DARK,
            R1_EventType.TYPE_AUTOJUMP_PLAT,
            R1_EventType.TYPE_BAG3,
            R1_EventType.TYPE_BB1_PLAT,
            R1_EventType.TYPE_BIGSTONE,
            R1_EventType.TYPE_BIG_BOING_PLAT,
            R1_EventType.TYPE_BOING_PLAT,
            R1_EventType.TYPE_BON3,
            R1_EventType.TYPE_BONBON_PLAT,
            R1_EventType.TYPE_BOUEE_JOE,
            R1_EventType.TYPE_BOUT_TOTEM,
            R1_EventType.TYPE_BTBPLAT,
            R1_EventType.TYPE_CAISSE_CLAIRE,
            R1_EventType.TYPE_CFUMEE,
            R1_EventType.TYPE_CORDE,
            R1_EventType.TYPE_CORDEBAS,
            R1_EventType.TYPE_COUTEAU_SUISSE,
            R1_EventType.TYPE_CRAYON_BAS,
            R1_EventType.TYPE_CRAYON_HAUT,
            R1_EventType.TYPE_CRUMBLE_PLAT,
            R1_EventType.TYPE_CYMBAL1,
            R1_EventType.TYPE_CYMBAL2,
            R1_EventType.TYPE_CYMBALE,
            R1_EventType.TYPE_DARK,
            R1_EventType.TYPE_DARK_SORT,
            R1_EventType.TYPE_DUNE,
            R1_EventType.TYPE_EAU,
            R1_EventType.TYPE_ENS,
            R1_EventType.TYPE_FALLING_CRAYON,
            R1_EventType.TYPE_FALLING_OBJ,
            R1_EventType.TYPE_FALLING_OBJ2,
            R1_EventType.TYPE_FALLING_YING,
            R1_EventType.TYPE_FALLING_YING_OUYE,
            R1_EventType.TYPE_FALLPLAT,
            R1_EventType.TYPE_FEE,
            R1_EventType.TYPE_GOMME,
            R1_EventType.TYPE_GRAINE,
            R1_EventType.TYPE_HERSE_HAUT,
            R1_EventType.TYPE_HERSE_HAUT_NEXT,
            R1_EventType.TYPE_INDICATOR,
            R1_EventType.TYPE_INST_PLAT,
            R1_EventType.TYPE_JOE,
            R1_EventType.TYPE_LAVE,
            R1_EventType.TYPE_LEVIER,
            R1_EventType.TYPE_LIFTPLAT,
            R1_EventType.TYPE_MARACAS,
            R1_EventType.TYPE_MARACAS_BAS,
            R1_EventType.TYPE_MARK_AUTOJUMP_PLAT,
            R1_EventType.TYPE_MARTEAU,
            R1_EventType.TYPE_MOVE_AUTOJUMP_PLAT,
            R1_EventType.TYPE_MOVE_MARTEAU,
            R1_EventType.TYPE_MOVE_PLAT,
            R1_EventType.TYPE_MOVE_RUBIS,
            R1_EventType.TYPE_MOVE_START_NUA,
            R1_EventType.TYPE_MOVE_START_PLAT,
            R1_EventType.TYPE_MUS_WAIT,
            R1_EventType.TYPE_ONOFF_PLAT,
            R1_EventType.TYPE_PALETTE_SWAPPER,
            R1_EventType.TYPE_PANCARTE,
            R1_EventType.TYPE_PETIT_COUTEAU,
            R1_EventType.TYPE_PI,
            R1_EventType.TYPE_PIERREACORDE,
            R1_EventType.TYPE_PI_BOUM,
            R1_EventType.TYPE_PI_MUS,
            R1_EventType.TYPE_PLATFORM,
            R1_EventType.TYPE_POELLE,
            R1_EventType.TYPE_PRI,
            R1_EventType.TYPE_PT_GRAPPIN,
            R1_EventType.TYPE_PUNAISE1,
            R1_EventType.TYPE_RAYMAN,
            R1_EventType.TYPE_RAY_ETOILES,
            R1_EventType.TYPE_REDUCTEUR,
            R1_EventType.TYPE_ROULETTE,
            R1_EventType.TYPE_ROULETTE2,
            R1_EventType.TYPE_ROULETTE3,
            R1_EventType.TYPE_RUBIS,
            R1_EventType.TYPE_SCROLL,
            R1_EventType.TYPE_SCROLL_SAX,
            R1_EventType.TYPE_SLOPEY_PLAT,
            R1_EventType.TYPE_SUPERHELICO,
            R1_EventType.TYPE_SWING_PLAT,
            R1_EventType.TYPE_TAMBOUR1,
            R1_EventType.TYPE_TAMBOUR2,
            R1_EventType.TYPE_TARZAN,
            R1_EventType.TYPE_TIBETAIN,
            R1_EventType.TYPE_TIBETAIN_2,
            R1_EventType.TYPE_TIBETAIN_6,
            R1_EventType.TYPE_TOTEM,
            R1_EventType.TYPE_TROMPETTE,
            R1_EventType.TYPE_UFO_IDC,
        };

        /// <summary>
        /// Randomizes the events in a level based on the flags
        /// </summary>
        /// <param name="level">The level</param>
        /// <param name="flags">The flags</param>
        /// <param name="seed">An optional seed to use</param>
        /// <param name="map">The map index</param>
        public static void Randomize(Unity_Level level, int? seed)
        {
            var random = seed != null ? new Random(seed.Value) : new Random();

            List<Unity_Object_R1> events = level.EventData.Select(o => o as Unity_Object_R1).ToList();

            var spawningSpots = events
                .Where(x => (x.ObjCollision == null || x.ObjCollision.Length == 0) && !x.IsAlways && !x.IsEditor && !BannedTargets.Contains(x.EventData.Type))
                .Select(x => x).ToList();

            foreach(var obj in events) {

                if (obj.EventData.Type != R1_EventType.TYPE_CAGE) continue;

                var linkedObjects = level.EventData.Where(o => o != obj && o.EditorLinkGroup == obj.EditorLinkGroup)
                    .Select(o => o as Unity_Object_R1).ToList();

                var singleGenDoor = (linkedObjects.Count() == 1 && linkedObjects.First().EventData.Type == R1_EventType.TYPE_GENERATING_DOOR) ? linkedObjects.First() : null;
                bool canMoveAnywhere = obj.EditorLinkGroup == 0 || singleGenDoor != null;

                var cageSpots = GetCloseSpots(obj, spawningSpots, canMoveAnywhere ? MaxCageFromOriginalSpotDist : MaxGenDoorFromCageDist);

                if (!cageSpots.Any()) continue;

                int tries = 0;

                var originalObjPos = GetCenteredPos(obj);
                var objCenterOffset = GetCenterOffset(obj);

                (Vector2, Unity_Object_R1) spawningSpot;

                do { 
                    spawningSpot = cageSpots[random.Next(cageSpots.Count)];

                    obj.XPosition = (short) (spawningSpot.Item1.x - objCenterOffset.x);
                    obj.YPosition = (short) (spawningSpot.Item1.y - objCenterOffset.y); 

                    tries++;

                } while (!PositionSafe(level, GetCenteredPos(obj)) && tries < 100);

                if (spawningSpot.Item2 != null) { 

                    var ssc = GetCenterOffset(spawningSpot.Item2);
                    spawningSpot.Item2.XPosition = (short)(originalObjPos.x - ssc.x);
                    spawningSpot.Item2.YPosition = (short)(originalObjPos.y - ssc.y);
                }

                Vector3 rayToUnitySpace = new Vector3(1.0f/16.0f, -1.0f/16.0f, 1);

                /*
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = Vector3.Scale(new Vector3(obj.XPosition, obj.YPosition, 0), rayToUnitySpace);
                var cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube2.transform.position = Vector3.Scale(new Vector3(obj.XPosition+objCenterOffset.x, obj.YPosition+objCenterOffset.y, 0), rayToUnitySpace);
                */

                if (singleGenDoor != null) {
                    singleGenDoor.XPosition = obj.XPosition;
                    singleGenDoor.YPosition = obj.YPosition;
                    var genDoorSpots = GetCloseSpots(singleGenDoor, spawningSpots, MaxGenDoorFromCageDist);

                    if (!genDoorSpots.Any()) continue;

                    tries = 0;

                    do {
                        var genDoorSpot = genDoorSpots[random.Next(genDoorSpots.Count)];
                        var genDoorCenterOffset = GetCenterOffset(singleGenDoor);

                        singleGenDoor.XPosition = (short) (genDoorSpot.Item1.x - genDoorCenterOffset.x);
                        singleGenDoor.YPosition = (short) (genDoorSpot.Item1.y - genDoorCenterOffset.y);

                        tries++;

                    } while (!PositionSafe(level, GetCenteredPos(singleGenDoor)) && tries < 100);
                }

            }
        }

        private static bool PositionSafe(Unity_Level level, Vector2 coords)
        {
            int x = (int)coords.x / level.PixelsPerUnit;
            int y = (int)coords.y / level.PixelsPerUnit;

            var cm = level.Maps[level.DefaultCollisionMap];

            var tilemapController = Controller.obj.levelController.controllerTilemap;


            for (int i = -1; i <= 1; i++) {
                for (int j = -1; j <= 0; j++) {

                    if (cm.GetMapTile(x+i, y+j)?.Data?.CollisionType != 0) {

                        return false;
                    }
                }
            }

            for (int i = -1; i <= 1; i++) {
                for (int j = -1; j <= 0; j++) {

                    tilemapController.SetTypeAtPos(x + i, y + j, (ushort)R1_TileCollisionType.Bounce);
                }
            }


            return true;
        }

        private static List<(Vector2,Unity_Object_R1)> GetCloseSpots(Unity_Object_R1 me, List<Unity_Object_R1> spawningSpots, float maxDist)
        {
            var eventPos = GetCenteredPos(me);

            return spawningSpots.Where(spot =>
            {
                var spotPos = GetCenteredPos(spot);
                return spot!=me && (spotPos - eventPos).sqrMagnitude <
                       maxDist * maxDist;
            }).Select(x=>(GetCenteredPos(x),x)).ToList();
        }

        private static Vector2 GetCenteredPos(Unity_Object_R1 o)
        {
            var co = GetCenterOffset(o);
            return new Vector2(o.XPosition + co.x, o.YPosition + co.y);
        }

        private static Vector2 GetCenterOffset(Unity_Object_R1 o)
        {
            var anim = o.CurrentAnimation;
            var frame = anim.Frames[0];
            float minX = -1;
            float minY = -1;
            float maxX = 0;
            float maxY = 0;
             
            foreach (var l in frame.SpriteLayers) {
                var sprite = o.Sprites[l.ImageIndex];

                if (sprite != null) {
                    minX = minX == -1 ? l.XPosition : Mathf.Min(maxX, l.XPosition);
                    minY = minY == -1 ? l.YPosition : Mathf.Min(minY, l.XPosition);
                    maxX = Mathf.Max(maxX, l.XPosition + sprite.rect.width);
                    maxY = Mathf.Max(maxY, l.YPosition + sprite.rect.height);
                }
            }

            return new Vector2(minX + (maxX - minX)*0.5f, minY + (maxY - minY)*0.5f);
        }
    }
}