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

namespace ManiacEditor.Methods.Runtime
{
    public static class ScriptLoader
    {
        #region Compilation
        public static List<EntityRenderer> LoadRenderers(List<string> filepaths)
        {
            CompilerResults compilationResults = null;
            List<EntityRenderer> validRenders = new List<EntityRenderer>();
            CodeDomProvider codeProvider = GetCodeDomProvider();

            try
            {
                CompilerParameters parms = GetParameters("Renders.dll");
                compilationResults = codeProvider.CompileAssemblyFromFile(parms, filepaths.ToArray());
                Assembly assembly = compilationResults.CompiledAssembly;
                foreach (var type in assembly.ExportedTypes)
                {
                    var rawResult = assembly.CreateInstance(type.ToString());
                    if (rawResult is EntityRenderer)
                    {
                        EntityRenderer result = (EntityRenderer)rawResult;
                        if (result != null) validRenders.Add(result);
                    }

                }
                codeProvider.Dispose();
                return validRenders;
            }
            catch (Exception ex)
            {
                InterpretException(ex, compilationResults, codeProvider);
                codeProvider.Dispose();
                return validRenders;
            }


        }
        public static List<LinkedRenderer> LoadLinkedRenderers(List<string> filepaths)
        {
            CompilerResults compilationResults = null;
            List<LinkedRenderer> validRenders = new List<LinkedRenderer>();
            CodeDomProvider codeProvider = GetCodeDomProvider();

            try
            {
                CompilerParameters parms = GetParameters("LinkedRenders.dll");
                compilationResults = codeProvider.CompileAssemblyFromFile(parms, filepaths.ToArray());
                Assembly assembly = compilationResults.CompiledAssembly;
                foreach (var type in assembly.ExportedTypes)
                {
                    var rawResult = assembly.CreateInstance(type.ToString());
                    if (rawResult is LinkedRenderer)
                    {
                        LinkedRenderer result = (LinkedRenderer)rawResult;
                        if (result != null) validRenders.Add(result);
                    }
                }
                codeProvider.Dispose();
                return validRenders;
            }
            catch (Exception ex)
            {
                InterpretException(ex, compilationResults, codeProvider);
                codeProvider.Dispose();
                return validRenders;
            }


        }
        #endregion

        #region Helpers

        private static CodeDomProvider GetCodeDomProvider()
        {
            CodeDomProvider codeProvider = new Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider();
            return codeProvider;
        }
        public static IEnumerable<string> GetAssemblyFiles(Assembly assembly)
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            return assembly.GetReferencedAssemblies()
                .Select(name => loadedAssemblies.SingleOrDefault(a => a.FullName == name.FullName)?.Location)
                .Where(l => l != null);
        }
        private static CompilerParameters GetParameters(string assemblyName)
        {
            string path = System.IO.Path.Combine(ProgramPaths.GetExecutingDirectoryName(), "Lib", assemblyName);
            CompilerParameters parms = new CompilerParameters();
            parms.TreatWarningsAsErrors = false;
            parms.OutputAssembly = path;
            parms.GenerateInMemory = true;
            parms.ReferencedAssemblies.AddRange(GetAssemblyFiles(Assembly.GetExecutingAssembly()).ToArray());
            parms.ReferencedAssemblies.Add("netstandard.dll");
            parms.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
            return parms;
        }
        private static void InterpretException(Exception ex, CompilerResults compilationResults, CodeDomProvider codeProvider)
        {
            if (compilationResults != null && compilationResults.Errors.HasErrors)
            {
                string errors = string.Join(Environment.NewLine, compilationResults.Errors.Cast<CompilerError>().ToList().Where(x => x.IsWarning == false).ToList().ConvertAll(z => z.ToString()));
                System.Windows.Forms.MessageBox.Show(errors);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            codeProvider.Dispose();
        }

        #endregion
    }
}
