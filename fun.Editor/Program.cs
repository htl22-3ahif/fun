using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Specialized;
using System.Reflection;
using System.IO;
using fun.Editor.Commands;
using fun.IO;
using Environment = fun.Core.Environment;

namespace fun.Editor
{
    class Program
    {
        static void Main(string[] args)
        {
            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string configFile = Path.Combine(appPath, "editor.config");
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = configFile;
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

            Environment environment = null;

            try
            {
                using (var file = new FileStream(config.AppSettings.Settings["envPath"].Value, FileMode.Open, FileAccess.Read))
                {
                    environment = new EnvironmentXmlReader().Load(file)[0];
                }
            }
            catch (FileNotFoundException) { }


            CommandManager commands = new CommandManager(
                new CreateCommandParser());

            commands.Parse(args);
        }
    }
}
