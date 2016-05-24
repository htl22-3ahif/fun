using fun.Core;
using fun.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Environment = fun.Core.Environment;

namespace fun.Editor.Commands
{
    internal sealed class ListCommandParser : CommandParser
    {
        public override string Keyword
        {
            get
            {
                return "ls";
            }
        }

        protected override void Do(string[] args)
        {
            var err = Console.Error;

            if (args.Length == 0)
            {
                err.WriteLine("There are no arguments");
                err.WriteLine("Arguments: <environment_file> [<entity_name> <element_type> <field>...]");
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

            if (args.Length == 1)
            {
                try
                {
                    foreach (var entity in env.Entities)
                        Console.WriteLine(entity.Name);
                }
                catch (Exception)
                {
                    err.WriteLine("There are no entities");
                }
                return;
            }

            var ent = args[1];

            if (args.Length == 2)
            {
                if (!env.Entities.Any(e => e.Name == ent))
                {
                    err.WriteLine("There is no {0} entity", ent);
                    return;
                }

                try
                {
                    foreach (var element in env.GetEntity(ent).Elements)
                    {
                        Console.WriteLine(element.GetType().Name);
                    }
                }
                catch (Exception)
                {
                    err.WriteLine("There are no Elements");
                }
                return;
            }

            var elem = args[2];

            if (args.Length == 3)
            {
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

                var fields = element.GetType().GetFields(
                    BindingFlags.Instance | BindingFlags.Public);

                if (fields == null)
                {
                    err.WriteLine("There are no fields");
                    return;
                }

                foreach (var _field in fields)
                    Console.WriteLine(_field.Name + ":" + _field.GetValue(element));

                return;
            }

            if (args.Length > 3)
            {
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
                            .First(f => f.Name == args[i]).GetValue(obj);
                    }
                    catch (Exception)
                    {
                        err.Write("There is no {0} field", args[i]);
                        return;
                    }
                }

                FieldInfo[] fields;
                try
                {
                    fields = obj.GetType().GetFields(
                        BindingFlags.Instance | BindingFlags.Public);
                }
                catch (Exception)
                {
                    err.WriteLine("There are no fields");
                    return;
                }

                foreach (var _field in fields)
                    Console.WriteLine(_field.Name + ":" + _field.GetValue(obj));
            }
        }
    }
}
