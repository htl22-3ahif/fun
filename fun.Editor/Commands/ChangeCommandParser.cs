using fun.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Environment = fun.Core.Environment;
using fun.Core;
using System.Text.RegularExpressions;

namespace fun.Editor.Commands
{
    internal sealed class ChangeCommandParser : CommandParser
    {
        public override string Keyword
        {
            get
            {
                return "change";
            }
        }

        protected override void Do(string[] args)
        {
            var err = Console.Error;

            if (args.Length < 5)
            {
                err.WriteLine("Invalid arguments");
                err.WriteLine("Arguments: <environment_file> <entity_name> <element_type> <field> <field_new_value>");
                return;
            }

            Environment env;
            var envPath = args[0];
            string[] libaries;

            try
            {
                using (var file = new FileStream(envPath, FileMode.Open, FileAccess.Read))
                {
                    env = new EnvironmentXmlReader().Load(file, out libaries)[0];
                }
            }
            catch (Exception)
            {
                err.WriteLine("File could not be found");
                return;
            }

            string ent = args[1];
            string elem = args[2];

            Element element;
            try
            {
                element = env.GetEntity(ent).Elements.First(e => e.GetType().Name == elem);
            }
            catch (Exception)
            {
                err.WriteLine("Element could not be found.");
                return;
            }

            int depth = Regex.Matches(args[4], ".").Count;

            for (int i = 0; i < depth; i++)
            {
                
            }

            var fields = element.GetType().GetFields(
                BindingFlags.Instance | BindingFlags.Public);

            if (fields == null)
            {
                err.WriteLine("There are no fields");
                return;
            }

            //fields.First(f => f.Name == args[3]).SetValue(env.GetEntity(ent).Elements.First(e => e.GetType().Name == elem), args[4]);
        }

        //protected FieldInfo[] GetCurrentFields(Environment env, string ent, string elem, int depth, FieldInfo[] currentFields)
        //{
        //    currentFields.First(f => f.Name == args[3]).SetValue(env.GetEntity(ent).Elements.First(e => e.GetType().Name == elem), args[4])
        //    return new FieldInfo[] { };
        //}
    }
}
