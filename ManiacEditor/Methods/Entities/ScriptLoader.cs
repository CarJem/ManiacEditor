using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using ManiacEditor.Entity_Renders;

namespace ManiacEditor.Methods.Entities
{
    public static class ScriptLoader
    {
        public static List<EntityRenderer> LoadRenderers(List<string> filepaths)
        {
            CompilerResults compilationResults = null;
            try
            {
                CSharpCodeProvider codeProvider = new CSharpCodeProvider();
                CompilerParameters parms = new CompilerParameters();
                parms.IncludeDebugInformation = true;
                parms.ReferencedAssemblies.AddRange(GetAssemblyFiles(Assembly.GetExecutingAssembly()).ToArray());
                parms.ReferencedAssemblies.Add("netstandard.dll");
                parms.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
                compilationResults = codeProvider.CompileAssemblyFromFile(parms, filepaths.ToArray());
                Assembly assembly = compilationResults.CompiledAssembly;
                List<EntityRenderer> validRenders = new List<EntityRenderer>();
                foreach (var type in assembly.ExportedTypes)
                {
                    EntityRenderer result = (EntityRenderer)assembly.CreateInstance(type.ToString());
                    if (result != null) validRenders.Add(result);
                }
                return validRenders;
            }
            catch
            {
                if (compilationResults != null && compilationResults.Errors.HasErrors)
                {
                    string errors = string.Join(Environment.NewLine, compilationResults.Errors.Cast<string>());
                    System.Windows.Forms.MessageBox.Show(errors);
                }
                return new List<EntityRenderer>();
            }


        }

        public static IEnumerable<string> GetAssemblyFiles(Assembly assembly)
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            return assembly.GetReferencedAssemblies()
                .Select(name => loadedAssemblies.SingleOrDefault(a => a.FullName == name.FullName)?.Location)
                .Where(l => l != null);
        }

        public static List<LinkedRenderer> LoadLinkedRenderers(List<string> filepaths)
        {
            CompilerResults compilationResults = null;
            try
            {
                CSharpCodeProvider codeProvider = new CSharpCodeProvider();
                CompilerParameters parms = new CompilerParameters();
                parms.ReferencedAssemblies.AddRange(GetAssemblyFiles(Assembly.GetExecutingAssembly()).ToArray());
                parms.ReferencedAssemblies.Add("netstandard.dll");
                parms.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
                compilationResults = codeProvider.CompileAssemblyFromFile(parms, filepaths.ToArray());
                Assembly assembly = compilationResults.CompiledAssembly;
                List<LinkedRenderer> validRenders = new List<LinkedRenderer>();
                foreach (var type in assembly.ExportedTypes)
                {
                    LinkedRenderer result = (LinkedRenderer)assembly.CreateInstance(type.ToString());
                    if (result != null) validRenders.Add(result);
                }
                return validRenders;
            }
            catch
            {
                if (compilationResults != null && compilationResults.Errors.HasErrors)
                {
                    string errors = string.Join(Environment.NewLine, compilationResults.Errors.Cast<string>());
                    System.Windows.Forms.MessageBox.Show(errors);
                }
                return new List<LinkedRenderer>();
            }


        }
    }
}
