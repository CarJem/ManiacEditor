using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using RSDKv5;
using System.Runtime.CompilerServices;

namespace ManiacEditor
{
    public partial class ObjectIDHealer
    {
        static Scene _scene;
        static IList<SceneEntity> _entities;

        public void startHealing(string fixFile)
        {
            Editor.ShowConsoleWindow();
            string fileName = fixFile;

            if (!File.Exists(fileName))
            {
                ColorConsole.WriteLine("File {0} doesn't exist.", ConsoleColor.Red, fileName);
                CloseConsole(-2);
            }

            try
            {
                FixFile(fileName);
            }
            catch (Exception ex)
            {
                ColorConsole.WriteLine("A bad thing happened.", ConsoleColor.Red);
                Console.WriteLine(ex);
                CloseConsole(-3);
            }


            CloseConsole();
        }

        private void FixFile(string fileName)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ManiacEditor.Resources.objects_attributes.ini"))
            {
                Objects.InitObjects(stream, true);
            }

            _scene = new Scene(fileName);
            _entities = GetEntities();

            var duplicateGroups = GetDuplicates();

            if (duplicateGroups == null || !duplicateGroups.Any())
            {
                CloseConsole(1);
            }

            foreach (var duplicateGroup in duplicateGroups)
            {
                HandleDuplicateGroup(duplicateGroup);
            }

            Console.WriteLine("All duplicate groups handled");
            string newFileName = Path.Combine(Path.GetDirectoryName(fileName),
                Path.GetFileNameWithoutExtension(fileName) +
                "-" + DateTime.UtcNow.Ticks + Path.GetExtension(fileName));
            _scene.Write(newFileName);
            Console.WriteLine("Wrote a new file here: " + newFileName);
            ColorConsole.WriteLine("Done! ☻", ConsoleColor.Green);
        }

        private static IList<SceneEntity> GetEntities()
        {
            var entities = _scene.Objects.SelectMany(so => so.Entities).ToList();
            Console.Write("There are ");
            ColorConsole.Write(entities.Count.ToString(), ConsoleColor.Cyan);
            Console.WriteLine(" entities in this scene.");

            return entities;
        }

        private static IList<IGrouping<ushort, SceneEntity>> GetDuplicates()
        {
            var groupedEntities = _entities.GroupBy(e => e.SlotID)
                                           .Where(g => g.Count() > 1)
                                           .ToList();

            if (groupedEntities.Any())
            {
                ColorConsole.WriteLine("There are {0} sets of duplicates.", ConsoleColor.Yellow, groupedEntities.Count());
                return groupedEntities;
            }

            ColorConsole.WriteLine("There are NO duplicates! ☻", ConsoleColor.Green);
            return null;
        }

        private void HandleDuplicateGroup(IGrouping<ushort, SceneEntity> duplicateGroup)
        {
            ushort id = duplicateGroup.Key;
            int duplicateCount = duplicateGroup.Count();
            Console.Write("There are ");
            ColorConsole.Write(duplicateCount.ToString(), ConsoleColor.Cyan);
            Console.Write(" entities with the id: ");
            ColorConsole.Write(id.ToString(), ConsoleColor.Cyan);
            Console.WriteLine(".");

            for (int i = 0; i < duplicateCount; i++)
            {
                PrintEntitySummary(duplicateGroup.ElementAt(i), i);
            }

            bool handled = false;
            while (!handled)
            {
                DuplicateAction duplicateAction = GetAction();
                switch (duplicateAction)
                {
                    case DuplicateAction.Info:
                        PrintExtendedInfo(duplicateGroup);
                        break;
                    case DuplicateAction.Keep:
                        PerformKeep(duplicateGroup);
                        handled = true;
                        break;
                    default:
                        Console.WriteLine("All changes aborted!");
                        CloseConsole(-2);
                        break;
                }
            }
        }

        static void PrintEntitySummary(SceneEntity entity, int index)
        {
            Console.WriteLine("Entity '{0}' is a {1}, at location {2}",
                              index, entity.Object.Name.Name,
                              entity.Position);
        }

        static void PrintExtendedInfo(IGrouping<ushort, SceneEntity> duplicateGroup)
        {
            int duplicateCount = duplicateGroup.Count();
            for (int i = 0; i < duplicateCount; i++)
            {
                SceneEntity entity = duplicateGroup.ElementAt(i);
                Console.WriteLine("Entity '{0}' is a {1}, at location X:{2}, Y:{3}",
                              i, entity.Object.Name.Name,
                              entity.Position.X.High, entity.Position.Y.High);
                foreach (var attribute in entity.attributesMap)
                {
                    Console.WriteLine($"{attribute.Key} = {attribute.Value}");
                }
                Console.WriteLine("**************");
            }
        }

        static void PerformKeep(IGrouping<ushort, SceneEntity> duplicateGroup)
        {
            Console.WriteLine("Enter number between 0 and {0}, to indicate which entity to keep. ",
                          duplicateGroup.Count() - 1);
            ColorConsole.WriteLine("All others in this group will be deleted!", ConsoleColor.Yellow);

            bool gotValidNumber = false;
            byte chosenEntityIndex = 0;
            while (!gotValidNumber)
            {
                Console.Write("Choice: ");
                string input = Console.ReadLine();
                if (Byte.TryParse(input, out chosenEntityIndex))
                {
                    if (chosenEntityIndex >= 0 && chosenEntityIndex < duplicateGroup.Count())
                    {
                        gotValidNumber = true;
                    }
                    else
                    {
                        Console.WriteLine("That is not in the valid range.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid selection!");
                }
            }

            for (byte b = 0; b < duplicateGroup.Count(); b++)
            {
                if (b == chosenEntityIndex) continue;
                var entity = duplicateGroup.ElementAt(b);
                _scene.Objects.Find(o => o == entity.Object).Entities.Remove(entity);
            }

        }

        static DuplicateAction GetAction()
        {
            var action = DuplicateAction.Abort;

            Console.WriteLine("What to do about this? ");
            ColorConsole.Write("Info", ConsoleColor.White);
            Console.WriteLine(", for more info.");
            ColorConsole.Write("Keep", ConsoleColor.White);
            Console.WriteLine(", to select an entity to keep.");
            ColorConsole.Write("Abort", ConsoleColor.White);
            Console.WriteLine(", to cancel all changes and get out of here.");

            bool choiceMade = false;
            while (!choiceMade)
            {
                Console.Write("Choice: ");
                string choice = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(choice))
                {
                    continue; // go around again
                }

                char firstLetter = Char.ToLowerInvariant(choice[0]);
                switch (firstLetter)
                {
                    case 'i':
                        action = DuplicateAction.Info;
                        choiceMade = true;
                        break;
                    case 'k':
                        action = DuplicateAction.Keep;
                        choiceMade = true;
                        break;
                    case 'a':
                        action = DuplicateAction.Abort;
                        choiceMade = true;
                        break;
                    default:
                        break;
                }
            }

            return action;
        }

        static bool CheckArgs(string[] args)
        {
            if (args.Length != 1)
            {
                ColorConsole.WriteLine("Invalid number of arguments.", ConsoleColor.Red);
                return false;
            }

            if (args[0].StartsWith("/h", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if (args[0].StartsWith("-h", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            return true;
        }

        static void PrintHelp()
        {
            Console.WriteLine(@"Usage:
DuplicateObjectIdHealer.exe MySceneFile.bin");
        }

        private void CloseConsole(int exitCode = 0)
        {
            Console.Write("Press any key to exit...");
            Console.ReadKey(false);
            Console.WriteLine();
            Application.Restart();
            Environment.Exit(0);
        }

        private void ObjectIDHealer_Load(object sender, EventArgs e)
        {

        }
    }
}
