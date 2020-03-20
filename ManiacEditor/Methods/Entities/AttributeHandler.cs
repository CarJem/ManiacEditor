using System;
using RSDKv5;
using ManiacEditor.Classes.Scene;

namespace ManiacEditor.Methods.Entities
{
    [Serializable]
    public static class AttributeHandler
    {
        static Position no_position = new Position(0, 0);

        public static byte AttributesMapUint8(string name, EditorEntity entity)
        {
            if (entity.attributesMap.ContainsKey(name))
            {
               byte value = entity.attributesMap[name].ValueUInt8;
               return value;
            }
            else
            {
                return 0;
            }

        }
        public static ushort AttributesMapUint16(string name, EditorEntity entity)
        {
            if (entity.attributesMap.ContainsKey(name))
            {
                ushort value = entity.attributesMap[name].ValueUInt16;
                return value;
            }
            else
            {
                return 0;
            }
        }
        public static uint AttributesMapUint32(string name, EditorEntity entity)
        {
            if (entity.attributesMap.ContainsKey(name))
            {
                uint value = entity.attributesMap[name].ValueUInt32;
                return value;
            }
            else
            {
                return 0;
            }
        }
        public static sbyte AttributesMapInt8(string name, EditorEntity entity)
        {
            if (entity.attributesMap.ContainsKey(name))
            {
                sbyte value = entity.attributesMap[name].ValueInt8;
                return value;
            }
            else
            {
                return 0;
            }
        }
        public static short AttributesMapInt16(string name, EditorEntity entity)
        {
            if (entity.attributesMap.ContainsKey(name))
            {
                short value = entity.attributesMap[name].ValueInt16;
                return value;
            }
            else
            {
                return 0;
            }
        }
        public static int AttributesMapInt32(string name, EditorEntity entity)
        {
            if (entity.attributesMap.ContainsKey(name))
            {
                int value = entity.attributesMap[name].ValueInt32;
                return value;
            }
            else
            {
                return 0;
            }
        }
        public static int AttributesMapVar(string name, EditorEntity entity)
        {
            if (entity.attributesMap.ContainsKey(name))
            {
                int value = entity.attributesMap[name].ValueEnum;
                return value;
            }
            else
            {
                return 0;
            }
        }
        public static bool AttributesMapBool(string name, EditorEntity entity, bool failReturnValue = false)
        {
            if (entity.attributesMap.ContainsKey(name))
            {
                bool value = entity.attributesMap[name].ValueBool;
                return value;
            }
            else
            {
                // Allows the user to be able to set this they want
                return failReturnValue;
            }
        }

        public static string AttributesMapString(string name, EditorEntity entity)
        {
            if (entity.attributesMap.ContainsKey(name))
            {
                string value = entity.attributesMap[name].ValueString;
                return value;
            }
            else
            {
                return "";
            }
        }

        public static RSDKv5.Color AttributesMapColor(string name, EditorEntity entity)
        {
            if (entity.attributesMap.ContainsKey(name))
            {
                RSDKv5.Color value = entity.attributesMap[name].ValueColor;
                return value;
            }
            else
            {
                return RSDKv5.Color.EMPTY;
            }
        }

        public static Position AttributesMapPosition(string name, EditorEntity entity)
        {
            if (entity.attributesMap.ContainsKey(name))
            {
                Position value = entity.attributesMap[name].ValueVector2;
                return value;
            }
            else
            {
                return no_position;
            }
        }

        public static int AttributesMapPositionHighX(string name, EditorEntity entity)
        {
            if (entity.attributesMap.ContainsKey(name))
            {
                int value = entity.attributesMap[name].ValueVector2.X.High;
                return value;
            }
            else
            {
                return 0;
            }
        }
        public static int AttributesMapPositionLowX(string name, EditorEntity entity)
        {
            if (entity.attributesMap.ContainsKey(name))
            {
                int value = entity.attributesMap[name].ValueVector2.X.Low;
                return value;
            }
            else
            {
                return 0;
            }
        }

        public static int AttributesMapPositionHighY(string name, EditorEntity entity)
        {
            if (entity.attributesMap.ContainsKey(name))
            {
                int value = entity.attributesMap[name].ValueVector2.Y.High;
                return value;
            }
            else
            {
                return 0;
            }
        }

        public static int AttributesMapPositionLowY(string name, EditorEntity entity)
        {
            if (entity.attributesMap.ContainsKey(name))
            {
                int value = entity.attributesMap[name].ValueVector2.Y.Low;
                return value;
            }
            else
            {
                return 0;
            }
        }

        public static bool PlaneFilterCheck(EditorEntity entity, int prority)
        {
            if (entity.attributesMap.ContainsKey("priority"))
            {
                int plane = (int)entity.attributesMap["priority"].ValueEnum;
                if (plane == 0)
                {
                    plane = (int)entity.attributesMap["priority"].ValueUInt8;
                }
                switch (plane)
                {
                    case 3:
                        plane = 0;
                        break;
                    case 2:
                        plane = 1;
                        break;
                    case 1:
                        plane = 2;
                        break;
                    case 0:
                        plane = 3;
                        break;
                }
                if (plane == prority)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (entity.attributesMap.ContainsKey("targetLayer"))
            {
                int plane = (int)entity.attributesMap["targetLayer"].ValueUInt16;
                switch (plane)
                {
                    case 3:
                        plane = 0;
                        break;
                    case 2:
                        plane = 1;
                        break;
                    case 1:
                        plane = 2;
                        break;
                    case 0:
                        plane = 3;
                        break;
                }
                if (plane == prority)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (entity.attributesMap.ContainsKey("planeFilter"))
            {
                int plane = (int)entity.attributesMap["planeFilter"].ValueEnum;
                if (plane == 0)
                {
                    plane = (int)entity.attributesMap["planeFilter"].ValueUInt8;
                }

                if (plane == prority)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                int manualPriority = GetObjectPriority(entity);
                if (manualPriority == prority) return true;
                return false;
            }
        }

        public static int GetObjectPriority(EditorEntity e)
        {
            
            if (e.Object.Name.Name == "UIOptionPanel") return -1;
            else if (e.Object.Name.Name == "UIPicture") return 1;
            else if (e.Object.Name.Name == "UIChoice") return 1;
            else if(e.Object.Name.Name == "UIButton") return 1;
            else if (e.Object.Name.Name == "UIDiorama") return 1;
            else if (e.Object.Name.Name == "UIKeyBinder") return 1;
            else if (e.Object.Name.Name == "UIButtonLabel") return 1;
            else return 0;
        }


    }
}
