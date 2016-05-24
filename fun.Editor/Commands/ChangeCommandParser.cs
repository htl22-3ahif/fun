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

            // get the specified environment
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

            // get an object that contains all fields of another object with all its values
            string ent = args[1];
            string elem = args[2];
            string[] fieldTree = args[3].Split('.');

            object obj;
            try
            {
                obj = env.GetEntity(ent).Elements
                    .First(e => e.GetType().Name == elem);
            }
            catch (Exception)
            {
                err.WriteLine("Element does not exist");
                return;
            }

            for (int i = 3; i < args.Length; i++)
            {
                try
                {
                    obj = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)
                        .First(f => f.Name == fieldTree[i - 3]).GetValue(obj);
                }
                catch (Exception)
                {
                    err.Write("There is no {0} field", fieldTree[i - 3]);
                    return;
                }
            }

            //// get the "depth" of the field tree (e.g. Position.X => depth = 1), used whenever a more complex field tree is specified
            //int depth = args[3].Count(s => s == '.');

            //for (int i = 0; i < depth; i++)
            //{
            //    try
            //    {
            //       // check for relevant field
            //       // get subfields for relevant field
            //    }
            //    catch (Exception)
            //    {
            //        err.Write("<error message>");
            //        return;
            //    }
            //}

            // set field to value (args[4])

            //fields.First(f => f.Name == args[3]).SetValue(env.GetEntity(ent).Elements.First(e => e.GetType().Name == elem), args[4]);


        }

        //protected FieldInfo GetRelevantField(FieldInfo field, object obj)
        //{
        //    
        //    return field;
        //}
    }
}
