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
        public IList<IZStage> Stages { get; set; }

        public void LoadStages(string filepath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filepath);

            Stages = new List<IZStage>();

            if (doc.FirstChild.Name == "Stages")
            {
                XmlElement xmlStages = (XmlElement)doc.FirstChild;
                for (var child = xmlStages.FirstChild; child != null; child = child.NextSibling)
                {
                    IZStage stage = new IZStage();
                    stage.LoadXML((XmlElement)child);
                    Stages.Add(stage);
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
    public class IZStage
    {
        [JsonIgnore]
        // List of scenes
        public List<IZScene> Scenes { get; set; }
        // The name of the stage
        public string StageName { get; set; }
        // The internal name of this stage (Must be unique)
        public string StageKey { get; set; }
        // The name of the folder for this stage
        public string StageID { get; set; }
        // The scene flags for this stage
        public int Flags { get; set; }

        // List of unlocks to enable for a given stage
        public List<string> Unlocks { get; set; }
        // List of paths to assets that needs to be replaced. old => new
        public List<IZAsset> Assets { get; set; }

        public IZStage() { }

        public void LoadXML(XmlElement xmlStage)
        {
            StageName = xmlStage.GetAttribute("stageName"); // Required
            StageID = xmlStage.GetAttribute("stageID");     // Required
            StageKey = xmlStage.GetAttribute("stageKey");   // Required
            if (xmlStage.Attributes.GetNamedItem("flags") != null)
            {
                int flag = 3;
                int.TryParse(xmlStage.GetAttribute("flags"), out flag);
                Flags = flag;
            }

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

            XmlNode xmlScenes = xmlStage.GetElementsByTagName("Scenes").Item(0);
            if (xmlScenes != null)
            {
                for (var child = xmlScenes.FirstChild; child != null; child = child.NextSibling)
                {
                    string sceneID = child.Attributes["id"].Value; // Required
                    string sceneName = "";
                    if (child.Attributes.GetNamedItem("name") != null) sceneName = child.Attributes.GetNamedItem("name").Value;

                    if (sceneID == null) continue;

                    // Set name to ID if a name wasn't given
                    if (sceneName == null) sceneName = sceneID;


                    IZScene scene = new IZScene();
                    scene.Parent = this;
                    scene.SceneID = sceneID;
                    scene.SceneName = sceneName;
                    if (child.Attributes.GetNamedItem("flags") != null)
                    {
                        int flag = 3;
                        int.TryParse(child.Attributes["flags"].Value, out flag);
                        scene.Flags = flag;
                    }
                    else scene.Flags = 3;

                    if (Scenes == null) Scenes = new List<IZScene>();
                    Scenes.Add(scene);
                }
            }
        }
    }
    public class IZScene
    {
        [JsonIgnore]
        // The owner of the scene (Base)
        public IZStage Parent { get; set; }
        // The id of this scene (Scene%s.bin)
        public string SceneID { get; set; }
        // The name of this scene. Name should be short
        public string SceneName { get; set; }
        // The scene flags for this scene
        public int Flags { get; set; }
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