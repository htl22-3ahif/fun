using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Specialized;
using System.Reflection;
using System.IO;

namespace fun.Editor
{
    class Program
    {
        static void Main(string[] args)
        {
            // code we will use for create command
            //string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //string configFile = Path.Combine(appPath, "editor.config");
            //ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
            //configFileMap.ExeConfigFilename = configFile;
            //Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

            //config.AppSettings.Settings["envPath"].Value = "asdfasfdasf";
            //config.Save();

            Commands commands = new Commands(); // we dont have commands now :c

            commands.Parse(args);

            // this will probably be the whole code in main
        }
    }
}
