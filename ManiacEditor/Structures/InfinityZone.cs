using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ManiacEditor.Structures
{
    public class InfinityConfig
    {
        public List<IZStage> Stages { get; set; }
        public List<IZCategory> Categories { get; set; }

        public void LoadStages(string filepath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filepath);

            Stages = new List<IZStage>();
            Categories = new List<IZCategory>();

            if (doc.FirstChild.Name == "InfinityZone")
            {
                XmlElement coreElement = (XmlElement)doc.FirstChild;
                var nodes = new List<XmlNode>(coreElement.ChildNodes.Cast<XmlNode>());
                if (nodes.Exists(x => x.Name == "Stages"))
                {
                    XmlElement xmlStages = (XmlElement)nodes.Where(x => x.Name == "Stages").FirstOrDefault();
                    for (var child = xmlStages.FirstChild; child != null; child = child.NextSibling)
                    {
                        IZStage stage = new IZStage();
                        stage.LoadXML((XmlElement)child);
                        Stages.Add(stage);
                    }
                }
                if (nodes.Exists(x => x.Name == "Categories"))
                {
                    XmlElement xmlCatagories = (XmlElement)nodes.Where(x => x.Name == "Categories").FirstOrDefault();
                    for (var child = xmlCatagories.FirstChild; child != null; child = child.NextSibling)
                    {
                        IZCategory category = new IZCategory();
                        category.LoadXML((XmlElement)child, Stages);
                        Categories.Add(category);
                    }
                }
            }
        }
        public InfinityConfig(string stages_filepath)
        {
            Stages = new List<IZStage>();

            LoadStages(stages_filepath);
        }
        public InfinityConfig()
        {
            Stages = new List<IZStage>();
        }
    }

    public class IZCategory
    {
        public string CategoryName { get; set; }
        public IList<IZGroup> Groups { get; set; }

        public void LoadXML(XmlElement xmlCatagory, List<IZStage> Stages)
        {
            CategoryName = xmlCatagory.GetAttribute("categoryName"); // Required
            Groups = new List<IZGroup>();
            for (var child = xmlCatagory.FirstChild; child != null; child = child.NextSibling)
            {
                IZGroup group = new IZGroup();
                group.LoadXML((XmlElement)child, Stages);
                Groups.Add(group);
            }
        }
    }
    public class IZGroup
    {
        public string GroupName { get; set; }
        public IList<IZScene> Scenes { get; set; }

        public void LoadXML(XmlElement xmlGroup, List<IZStage> Stages)
        {
            GroupName = xmlGroup.GetAttribute("groupName"); // Required
            Scenes = new List<IZScene>();
            for (var child = xmlGroup.FirstChild; child != null; child = child.NextSibling)
            {
                IZScene scene = new IZScene();
                scene.LoadXML((XmlElement)child, Stages);
                if (scene.Stage != null) Scenes.Add(scene);
            }

        }
    }
    public class IZScene
    {
        [JsonIgnore]
        // The Stage Data For This Scene
        public IZStage Stage { get; set; }
        // The name of the stage to refrence.
        public string StageKey { get; set; }
        // The name of this scene. Name should be short
        public string SceneKey { get; set; }
        // The id of this scene (Scene%s.bin)
        public string SceneID { get; set; }
        // The scene flags for this scene
        public int SceneFlags { get; set; }
        // The name of this scene.
        public string SceneName { get; set; }

        public void LoadXML(XmlElement xmlScene, List<IZStage> Stages)
        {
            StageKey = xmlScene.GetAttribute("stageKey"); // Required
            SceneKey = xmlScene.GetAttribute("sceneKey");     // Required
            SceneID = xmlScene.GetAttribute("sceneID");   // Required
            SceneName = xmlScene.GetAttribute("sceneName");   // Required

            if (xmlScene.Attributes.GetNamedItem("sceneFlags") != null)
            {
                int flag = 3;
                int.TryParse(xmlScene.GetAttribute("sceneFlags"), out flag);
                SceneFlags = flag;
            }

            if (Stages.Exists(x => x.StageKey == StageKey))
            {
                Stage = Stages.Where(x => x.StageKey == StageKey).FirstOrDefault();
            }
        }

    }
    public class IZStage
    {
        // The name of this stage;
        public string StageName { get; set; }
        // The name of the folder for this stage
        public string StageDir { get; set; }
        // The name of this scene. (must be unique)
        public string StageKey { get; set; }

        // List of unlocks to enable for a given stage
        public List<string> Unlocks { get; set; }
        // List of paths to assets that needs to be replaced. old => new
        public List<IZAsset> Assets { get; set; }

        public IZStage() { }

        public void LoadXML(XmlElement xmlStage)
        {
            StageName = xmlStage.GetAttribute("stageName"); // Required
            StageDir = xmlStage.GetAttribute("stageDir");     // Required
            StageKey = xmlStage.GetAttribute("stageKey");   // Required

            XmlNode xmlUnlocks = xmlStage.GetElementsByTagName("StageUnlocks").Item(0);
            if (xmlUnlocks != null)
            {
                for (var child = xmlUnlocks.FirstChild; child != null; child = child.NextSibling)
                {
                    if (Unlocks == null) Unlocks = new List<string>();
                    Unlocks.Add(child.Name);
                }
            }

            XmlNode xmlAssets = xmlStage.GetElementsByTagName("StageAssets").Item(0);
            if (xmlAssets != null)
            {
                for (var child = xmlAssets.FirstChild; child != null; child = child.NextSibling)
                {
                    if (Assets == null) Assets = new List<IZAsset>();
                    Assets.Add(new IZAsset(child.Attributes["basePath"].Value, child.Attributes["newPath"].Value));
                }
            }
        }
    }
    public class InfinityUnlocks
    {
        public Dictionary<string, List<string>> UnlockSets { get; set; }
        public void LoadUnlockSets(string filepath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filepath);

            UnlockSets = new Dictionary<string, List<string>>();

            if (doc.FirstChild.Name == "UnlockSets")
            {
                XmlElement unlockSets = (XmlElement)doc.FirstChild;
                for (var unlockSet = unlockSets.FirstChild; unlockSet != null; unlockSet = unlockSet.NextSibling)
                {
                    string name = unlockSet.Name;
                    if (!UnlockSets.ContainsKey(name))
                    {
                        List<string> Unlocks = new List<string>();
                        for (var unlock = unlockSet.FirstChild; unlock != null; unlock = unlock.NextSibling)
                        {
                            Unlocks.Add(unlock.Name);
                        }
                        UnlockSets.Add(name, Unlocks);
                    }

                }
            }
        }
        public InfinityUnlocks(string unlocksets_filepath)
        {
            LoadUnlockSets(unlocksets_filepath);
        }
    }
    public class IZAsset
    {
        public string BasePath { get; set; }
        public string NewPath { get; set; }


        public override string ToString()
        {
            return "BasePath: " + BasePath + Environment.NewLine + "NewPath: " + NewPath;
        }

        public IZAsset(string _basePath, string _newPath)
        {
            BasePath = _basePath;
            NewPath = _newPath;
        }

        public IZAsset() { }
    }


    public class IZStageUnlocks
    {
        public readonly static List<string> AllUnlocks = new List<string>()
        {
            // Green Hill Zone (GHZ)
             "GHZ_Batbrain",      
             "GHZ_Bridge",        
             "GHZ_BuzzBomber",    
             "GHZ_CheckerBall",   
             "GHZ_Chopper",       
             "GHZ_Crabmeat",      
             "GHZ_Decoration",    
             "GHZ_Fireball",      
             "GHZ_Motobug",       
             "GHZ_Newtron",       
             "GHZ_Platform",      
             "GHZ_SpikeLog",      
             "GHZ_Splats",        
             "GHZ_ZipLine",       

            // Chemical Plant Zone (CPZ)
             "CPZ_Ball",          
             "CPZ_Bubbler",       
             "CPZ_CaterkillerJr", 
             "CPZ_OneWayDoor",    
             "CPZ_SpeedBooster",  
             "CPZ_Sweep",         
             "CPZ_SpringTube",    
             "CPZ_Decoration",    
             "CPZ_Platform",      

            // Studiopolis Zone (SPZ)
             "SPZ1_Boss",         
             "SPZ1_Canista",      
             "SPZ1_Circlebumper", 
             "SPZ1_Clapperboard", 
             "SPZ1_Directorchair",
             "SPZ1_Filmreel",     
             "SPZ1_Micdrop",      
             "SPZ1_Rockemsockem", 
             "SPZ1_Shutterbug",   
             "SPZ1_Turbinaut",    
             "SPZ2_Canista",      
             "SPZ2_Pathinverter", 
             "SPZ1_Decoration",   
             "SPZ1_Platform",     
             "SPZ2_Platform",     

            // Flying Battery Zone (FBZ)
             "FBZ_Decoration",    
             "FBZ_Platform",      
             "FBZ_Blaster",       
             "FBZ_Button",        
             "FBZ_Current",       
             "FBZ_Electromagnet", 
             "FBZ_Flamespring",   
             "FBZ_HangGlider",    
             "FBZ_HangPoint",     
             "FBZ_MagSpikeball",  
             "FBZ_Spikes",        
             "FBZ_Technosqueek",  
             "FBZ_TubeSpring",    
             "FBZ_Tuesday",       
             "FBZ_TwistingDoor",  

            // Press Garden Zone (PSZ)
             "PSZ1_Platform",     
             "PSZ2_Platform",     
             "PSZ1_Acetone",      
             "PSZ1_Crate",        
             "PSZ1_DoorTrigger",  
             "PSZ1_Dragonfly",    
             "PSZ1_FrostThrower", 
             "PSZ1_Ice",          
             "PSZ1_IceBomba",     
             "PSZ1_Ink",          
             "PSZ1_InkWipe",      
             "PSZ1_JuggleSaw",    
             "PSZ1_PSZDoor",      
             "PSZ1_Petal",        
             "PSZ1_Press",        
             "PSZ1_PrintBlock",   
             "PSZ1_SP500",        
             "PSZ1_Splats",       
             "PSZ1_Woodrow",      
             "PSZ2_Shinobi",      
             "PSZ2_Spikes",       
             "PSZ2_FrostThrower", 

            // Stardust Speedway Zone (SSZ)
             "SSZ1_SDashWheel",    
             "SSZ1_Platform",     
             "SSZ2_Platform",     
             "SSZ1_LaunchSpring",  
             "SSZ1_SpeedBooster", 
             "SSZ2_SpeedBooster", 
             "SSZ1_Dango",        
             "SSZ1_Flowerpod",    
             "SSZ1_Hotaru",       
             "SSZ1_HotauraMKII",  
             "SSZ1_JunctionWheel",
             "SSZ1_Kabasira",     
             "SSZ1_Kanabun",      
             "SSZ1_MSHologram",   
             "SSZ1_RotatingSpike",
             "SSZ1_RTeleporter",  
             "SSZ1_SDashWheel",   
             "SSZ1_Spark",        
             "SSZ1_SpikeBall",    
             "SSZ2_MetalSonic",   

            // Hydrocity Zone (HCZ)
             "HCZ_Decoration",    
             "HCZ_Bridge",        
             "HCZ_Platform",      
             "HCZ_Blastoid",      
             "HCZ_Button",        
             "HCZ_ButtonDoor",    
             "HCZ_DiveEggman",    
             "HCZ_Fan",           
             "HCZ_Jaws",          
             "HCZ_Jellygnite",    
             "HCZ_MegaChopper",   
             "HCZ_PointDexter",   
             "HCZ_ScrewMobile",   
             "HCZ_TurboSpiker",   
             "HCZ_Wake",          

            // Mirage Saloon Zone (MSZ)
             "MSZ_Decoration",    
             "MSZ_Platform",      
             "MSZ_Armadilloid",   
             "MSZ_Bumpalo",       
             "MSZ_Cactula",       
             "MSZ_Vultron",       
             "MSZ_Ending",        
             "MSZ_HeavyMystic",   
             "MSZ_Rogues",        
             "MSZ_RollerMKII",    
             "MSZ_RotatingSpikes",
             "MSZ_SwingRope",     
             "MSZ_Tornado",       

            // Oil Ocean Zone (OOZ)
             "OOZ_Platform",      
             "OOZ_Aquis",         
             "OOZ_Fan",           
             "OOZ_Hatch",         
             "OOZ_Octus",         
             "OOZ_PullSwitch",    
             "OOZ_PushSpring",    
             "OOZ_Sol",           
             "OOZ_Valve",         

            // Lava Reef Zone (LRZ)
             "LRZ1_Platform",     
             "LRZ2_Platform",     
             "LRZ1_Bridge",       
             "LRZ1_Button",       
             "LRZ1_ButoonDoor",   
             "LRZ1_DrillerDroid", 
             "LRZ1_Fireworm",     
             "LRZ1_Iwamodoki",    
             "LRZ1_LRZFireball",  
             "LRZ1_LRZRockPile",  
             "LRZ1_LavaFall",     
             "LRZ1_LavaGeyser",   
             "LRZ1_OrbitSpike",   
             "LRZ1_Rexon",        
             "LRZ1_Rockdrill",    
             "LRZ1_Stalactite",   
             "LRZ1_Toxomister",   
             "LRZ1_WalkerLegs",   
             "LRZ2_Flamethrower", 
             "LRZ2_ParallaxSprite",
             "LRZ2_Turbine",      
             "LRZ3_HeavyKing",    
             "LRZ3_HeavyRider",   

            // Metallic Madness Zone (MMZ)
             "MMZ_Platform",      
             "MMZ_Bomb",          
             "MMZ_Button",        
             "MMZ_Caterkiller",   
             "MMZ_ConveyorWheel", 
             "MMZ_Matryoshkabom", 
             "MMZ_Mechabu",       
             "MMZ_OneWayDoor",    
             "MMZ_Orbinaut",      
             "MMZ_PohBee",        
             "MMZ_Scarab",        

            // Titanic Monarch Zone
             "TMZ1_Platform",     
             "TMZ1_Decoration",   
             "TMZ1_Ballhog",       
             "TMZ1_Button",        
             "TMZ1_FlasherMKII",   
             "TMZ1_GymBar",        
             "TMZ1_MagnetSphere",  
             "TMZ1_Portal",        
             "TMZ1_SentryBug",     
             "TMZ1_TeeterTotter",  
             "TMZ1_WallBumper",    

            // Angel Island Zone (AIZ)
             "AIZ_Decoration",    
             "AIZ_Platform",      
             "AIZ_Claw",          
             "AIZ_Bloominator",   
             "AIZ_CaterkillerJr", 
             "AIZ_Rhinobot",      
             "AIZ_Sweep",         
             "AIZ_SwingRope",     
             "AIZ_AIZTornado",    

            // Hidden Palace Zone (HPZ)
             "HPZ_Jellygnite",    
             "HPZ_Redz",          
             "HPZ_Stegway",       
             "HPZ_Batbot",        

            // UI/Internal
             "Summary_UIPicture"
        };
    }
}