using System;
using RSDKv5;

namespace ManiacEditor
{
    [Serializable]
    public class AttributeValidater
    {
         Position no_position = new Position(0, 0);

        public byte AttributesMapUint8(string name, SceneEntity entity)
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
        public ushort AttributesMapUint16(string name, SceneEntity entity)
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
        public uint AttributesMapUint32(string name, SceneEntity entity)
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
        public sbyte AttributesMapInt8(string name, SceneEntity entity)
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
        public short AttributesMapInt16(string name, SceneEntity entity)
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
        public int AttributesMapInt32(string name, SceneEntity entity)
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
        public int AttributesMapVar(string name, SceneEntity entity)
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
        public bool AttributesMapBool(string name, SceneEntity entity, bool failReturnValue = false)
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

        public string AttributesMapString(string name, SceneEntity entity)
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

        public RSDKv5.Color AttributesMapColor(string name, SceneEntity entity)
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

        public Position AttributesMapPosition(string name, SceneEntity entity)
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

        public int AttributesMapPositionHighX(string name, SceneEntity entity)
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
        public int AttributesMapPositionLowX(string name, SceneEntity entity)
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

        public int AttributesMapPositionHighY(string name, SceneEntity entity)
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

        public int AttributesMapPositionLowY(string name, SceneEntity entity)
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

        public static bool PlaneFilterCheck(SceneEntity entity, int prority)
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

        public static int GetObjectPriority(SceneEntity e)
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
