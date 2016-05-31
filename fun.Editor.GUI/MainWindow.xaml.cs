using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace fun.Editor.GUI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Envionment environment;

        public MainWindow()
        {
            //var editor = new Process();
            //editor.StartInfo = new ProcessStartInfo("fun.Editor.exe", "ls environment.xml");
            //var output = editor.StandardOutput;
            //editor.Start();
            //var entities = output.ReadToEnd().Split('\n');
            
            InitializeComponent();

            if(tb_FolderOrFile.Text.Split('.').Last() == "xml")
            {
                environment = new Envionment(tb_FolderOrFile.Text.Split('/').Last());
                EnvironmentEntitiesAndElements();
            }
        }

        public void EnvironmentEntitiesAndElements()
        {
            #region Entity
            var editor = new Process();
            editor.StartInfo = new ProcessStartInfo("fun.Editor.exe", "ls " + tb_FolderOrFile.Text.Split('/').Last());
            var output = editor.StandardOutput;
            editor.Start();
            var entities = output.ReadToEnd().Split('\n');
            foreach (var entity in entities)
            {
                environment.Entities.Add(new Entity(entity));
            }
            #endregion
            #region Element
            foreach (var item in environment.Entities)
            {
                editor.StartInfo = new ProcessStartInfo("fun.Editor.exe", "ls " + tb_FolderOrFile.Text.Split('/').Last() + " " + item.EntityName);
                output = editor.StandardOutput;
                editor.Start();
                var elements = output.ReadToEnd().Split('\n');
                int counter = 0;
                foreach (var element in elements)
                {
                    environment.Entities[counter].Elements.Add(new Elements(element));
                    counter++;
                }
            }
            #endregion
            #region Field1
            int entitycounter = 0;
            foreach (var entity in environment.Entities)
            {
                int elementcounter = 0;
                foreach (var element in entity.Elements)
                {

                    editor.StartInfo = new ProcessStartInfo("fun.Editor.exe", "ls " + tb_FolderOrFile.Text.Split('/').Last() + " " + entity.EntityName + " " + element.ElementName);
                    output = editor.StandardOutput;
                    editor.Start();
                    var fields = output.ReadToEnd().Split('\n');
                    foreach (var field in fields)
                    {
                        environment.Entities[entitycounter].Elements[elementcounter].Fields.Add(new Field_n(field));
                    }
                    elementcounter++;
                }
                entitycounter++;
            }
            #endregion
            #region More Fields
            entitycounter = 0;
            foreach (var entity in environment.Entities)
            {
                int elementcounter = 0;
                foreach (var element in entity.Elements)
                {
                    int fieldcounter = 0;
                    foreach (var field in element.Fields)
                    {
                        editor.StartInfo = new ProcessStartInfo("fun.Editor.exe", "ls " + tb_FolderOrFile.Text.Split('/').Last() + " " + entity.EntityName + " " + element.ElementName + " " + field.FieldName);
                        output = editor.StandardOutput;
                        editor.Start();
                        var morefields = output.ReadToEnd().Split('\n');
                        foreach (var morefield in morefields)
                        {
                            environment.Entities[entitycounter].Elements[elementcounter].Fields[fieldcounter].Fields.Add(new Field_n(morefield));

                        }
                        fieldcounter++;
                    }
                    elementcounter++;
                }
                entitycounter++;
            }
            #endregion

        }
    }

    #region Environment - Fields
    public class Envionment
    {
        public string EnvironmentName { get; set; }
        public List<Entity> Entities { get; set; }

        public Envionment()
        {
            this.EnvironmentName = "";
            this.Entities = new List<Entity>();
        }

        public Envionment(string EnvironmentName)
        {
            this.EnvironmentName = EnvironmentName;
            this.Entities = new List<Entity>();
        }

        public Envionment(string EnvironmentName, List<Entity> Entities)
        {
            this.EnvironmentName = EnvironmentName;
            this.Entities = Entities;
        }


        public override string ToString()
        {
            string ReturnString = "";
            ReturnString += EnvironmentName;
            foreach (var item in Entities)
            {
                ReturnString += "\r\n  - " + Entities;
            }
            return ReturnString;
        }
    }
    public class Entity
    {
        public string EntityName { get; set; }
        public List<Elements> Elements { get; set; }

        public Entity()
        {
            this.EntityName = "";
            this.Elements = new List<GUI.Elements>();
        }
        public Entity(string EntityName)
        {
            this.EntityName = EntityName;
            this.Elements = new List<GUI.Elements>();
        }
        public Entity(string EntityName, List<Elements> Elements)
        {
            this.Elements = Elements;
        }

        public override string ToString()
        {
            string Returnstring = "";
            Returnstring += this.EntityName;
            foreach (var item in Elements)
            {
                Returnstring += "\r\n  - " + item;
            }
            return Returnstring;
        }
    }
    public class Elements
    {
        public string ElementName { get; set; }
        public List<Field_n> Fields { get; set; }

        public Elements()
        {
            this.ElementName = "";
            this.Fields = new List<Field_n>();
        }
        public Elements(string ElementName)
        {
            this.ElementName = ElementName;
            this.Fields = new List<Field_n>();
        }
        public Elements(string ElementName, List<Field_n> Fields)
        {
            this.ElementName = ElementName;
            this.Fields = Fields;
        }

        public override string ToString()
        {
            string ReturnString = "";
            ReturnString += ElementName;
            foreach (var item in Fields)
            {
                ReturnString += "\r\n  - " + item;
            }
            return ReturnString;
        }
    }
    public class Field_n
    {
        public string FieldName { get; set; }
        public List<Field_n> Fields { get; set; }

        public Field_n()
        {
            this.FieldName = "";
            this.Fields = new List<Field_n>();
        }
        public Field_n(string FieldName)
        {
            this.FieldName = FieldName;
            this.Fields = new List<Field_n>();
        }
        public Field_n(string FieldName, List<Field_n> Fields)
        {
            this.FieldName = FieldName;
            this.Fields = Fields;
        }

        public override string ToString()
        {
            string ReturnString = "";
            ReturnString += FieldName;
            foreach (var item in Fields)
            {
                ReturnString += "\r\n  - " + item;
            }
            return ReturnString;
        }
    }
    #endregion
}
