using fun.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Environment = fun.Core.Environment;
using System.Reflection.Emit;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace fun.Editor.GUI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.FolderBrowserDialog folderDialog;
        private System.Windows.Forms.OpenFileDialog fileDialog;
        private string EnvPath = "";
        private bool ActionOnEntity = true;

        public MainWindow()
        {

            InitializeComponent();

            Init();
        }

        public void DisplayTree(string env)
        {
            TreeViewItem tvRoot = new TreeViewItem() { Header = env };
            List<string> entities = GetEntities(env);

            foreach(string e in entities)
            {
                TreeViewItem tvEntities = new TreeViewItem() { Header = e };
                List<string> elements = GetElements(env, e);

                foreach(string elem in elements)
                {
                    TreeViewItem tvElements = new TreeViewItem() { Header = elem };
                    List<string> fields = GetFields(env, e, elem);

                    foreach(string f in fields)
                    {
                        //tvElements.Items.Add(f);
                        TreeViewItem tvFields = new TreeViewItem() { Header = f };
                        if (f.Contains(":"))
                        {
                            List<string> args = new List<string>() { f.Split(':')[0] };
                            GetFieldsRec(env, e, elem, args, tvFields);
                        } else
                        {
                            List<string> args = new List<string>() { f };
                            GetFieldsRec(env, e, elem, args, tvFields);
                        }

                        tvElements.Items.Add(tvFields);
                    }

                    tvEntities.Items.Add(tvElements);
                }

                tvRoot.Items.Add(tvEntities);
            }

            treeView.Items.Clear();
            treeView.Items.Add(tvRoot);
        }

        public List<string> GetEntities(string env)
        {
            List<string> lines = new List<string>();
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "fun.Editor.exe",
                    Arguments = "ls " + env,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                string line = proc.StandardOutput.ReadLine();
                lines.Add(line);
            }

            return lines;
        }

        public List<string> GetElements(string env, string entity)
        {
            List<string> lines = new List<string>();
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "fun.Editor.exe",
                    Arguments = "ls " + env + " " + entity,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                string line = proc.StandardOutput.ReadLine();
                lines.Add(line);
            }

            return lines;
        }

        public List<string> GetFields(string env, string entity, string element)
        {
            List<string> lines = new List<string>();
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "fun.Editor.exe",
                    Arguments = "ls " + env + " " + entity + " " + element,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                string line = proc.StandardOutput.ReadLine();
                lines.Add(line);
            }

            return lines;
        }

        public void GetFieldsRec(string env, string entity, string element, List<string> args, TreeViewItem item)
        {
            string strArguments = "ls " + env + " " + entity + " " + element;
            foreach(string s in args)
            {
                strArguments += " " + s;
            }

            List<string> lines = new List<string>();
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "fun.Editor.exe",
                    Arguments = strArguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                string line = proc.StandardOutput.ReadLine();
                lines.Add(line);
            }


            if(lines.Count > 0 && !String.IsNullOrWhiteSpace(lines[0]))
            {
                foreach(string line in lines)
                {
                    TreeViewItem tvItem = new TreeViewItem() { Header = line };

                    if(line.Contains(":"))
                    {
                        args.Add(line.Split(':')[0]);

                        GetFieldsRec(env, entity, element, args, tvItem);

                        args.Remove(line.Split(':')[0]);
                    } else
                    {
                        tvItem.Items.Add(args[0]);
                    }

                    item.Items.Add(tvItem);
                }
            }

        }

        private void Init()
        {
            rbNew.IsChecked = true;

            folderDialog = new System.Windows.Forms.FolderBrowserDialog();

            fileDialog = new System.Windows.Forms.OpenFileDialog();


            fileDialog.Filter = "XML Files|*.xml";

            rbActionOnEntity.IsChecked = true;
            rbActionOnElement.IsChecked = false;
            rbEntityAdd.IsChecked = true;
            rbElementAdd.IsChecked = true;

            ToggleActionUI(true);
        }

        private void rbNew_Checked(object sender, RoutedEventArgs e)
        {
            tbFileOrFolderPath.IsEnabled = true;
            ChangeIcon(btnChoose, MaterialDesignThemes.Wpf.PackIconKind.Folder);
        }

        private void rbChange_Checked(object sender, RoutedEventArgs e)
        {
            tbFileOrFolderPath.IsEnabled = false;
            ChangeIcon(btnChoose, MaterialDesignThemes.Wpf.PackIconKind.File);
        }

        private void ChangeIcon(Button button, MaterialDesignThemes.Wpf.PackIconKind icon)
        {
            MaterialDesignThemes.Wpf.PackIcon pi = new MaterialDesignThemes.Wpf.PackIcon();
            pi.Kind = icon;
            button.Content = pi;
        }

        private void UpdateEntityComboboxes()
        {
            IEnumerable<string> entities = GetEntities(EnvPath);

            cbxEntityName.ItemsSource = entities;

            cbxElementEntity.ItemsSource = entities;
        }
        

        private void UpdateElementCombobox(string selectedEntity)
        {
            cbxElement.ItemsSource = GetElements(EnvPath, selectedEntity);
        }

        private void btnChoose_Click(object sender, RoutedEventArgs e)
        {
            string fileOrFolderPath = "";

            if ((bool)rbChange.IsChecked)
            {
                if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    fileOrFolderPath = fileDialog.FileName;
                    tbFileOrFolderPath.IsReadOnly = false;
                    tbFileOrFolderPath.IsEnabled = true;

                    EnvPath = fileDialog.FileName;

                    tbFileOrFolderPath.Text = fileOrFolderPath;

                    UpdateEntityComboboxes();

                    DisplayTree(EnvPath);
                }
            }
            else
            {
                if (String.IsNullOrWhiteSpace(tbFileOrFolderPath.Text))
                {

                    if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        tbFileOrFolderPath.IsReadOnly = false;
                        tbFileOrFolderPath.IsEnabled = false;
                        fileOrFolderPath = folderDialog.SelectedPath;

                        tbFileOrFolderPath.IsEnabled = true;

                        tbFileOrFolderPath.Text = fileOrFolderPath;
                    }

                } else
                {
                    EnvPath = tbFileOrFolderPath.Text;

                    UpdateEntityComboboxes();

                    CreateEnvironment(EnvPath);

                    MessageBox.Show("Ok, set to new path");
                }
            }

            
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ActionOnEntity)
            {

                if ((bool)rbEntityAdd.IsChecked)
                {
                    // add
                    AddEntity(EnvPath, tbEntityAdd.Text);

                }
                else
                {
                    // remove
                    RemoveEntity(EnvPath, cbxEntityName.SelectedValue.ToString());
                }

            }
            else
            {

                if ((bool)rbElementAdd.IsChecked)
                {
                    AddElement(EnvPath, cbxElementEntity.SelectedValue.ToString(), tbElementName.Text);
                }
                else
                {
                    RemoveElement(EnvPath, cbxElementEntity.SelectedValue.ToString(), cbxElement.SelectedValue.ToString());
                }
            }

            DisplayTree(EnvPath);

            UpdateEntityComboboxes();
        }

        private void cbxEntityName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbxElementName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void tbEntityName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void tbElementName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ToggleActionUI(bool value)
        {
            ActionOnEntity = !ActionOnEntity;

            rbEntityAdd.IsEnabled = value;
            rbEntityRemove.IsEnabled = value;

            tbEntityAdd.IsEnabled = value;
            cbxEntityName.IsEnabled = value;

            lblEntity.IsEnabled = value;



            rbElementAdd.IsEnabled = !value;
            rbElementRemove.IsEnabled = !value;

            lblElement.IsEnabled = !value;
            lblElementEntity.IsEnabled = !value;

            cbxElement.IsEnabled = !value;
            cbxElementEntity.IsEnabled = !value;

            tbElementName.IsEnabled = !value;
        }

        private void rbActionOnEntity_Checked(object sender, RoutedEventArgs e)
        {
            ToggleActionUI(true);
        }

        private void rbActionOnElement_Checked(object sender, RoutedEventArgs e)
        {
            ToggleActionUI(false);
        }




        private void rbEntityAdd_Checked(object sender, RoutedEventArgs e)
        {
            cbxEntityName.Visibility = Visibility.Hidden;
            tbEntityAdd.Visibility = Visibility.Visible;
        }

        private void rbEntityRemove_Checked(object sender, RoutedEventArgs e)
        {
            cbxEntityName.Visibility = Visibility.Visible;
            tbEntityAdd.Visibility = Visibility.Hidden;

            cbxEntityName.ItemsSource = GetEntities(EnvPath);
            cbxEntityName.SelectedIndex = 0;
        }



        private void rbElementAdd_Checked(object sender, RoutedEventArgs e)
        {
            cbxElement.Visibility = Visibility.Hidden;
            tbElementName.Visibility = Visibility.Visible;

            cbxElementEntity.ItemsSource = GetEntities(EnvPath);
            cbxElementEntity.SelectedIndex = 0;
        }

        private void rbElementRemove_Checked(object sender, RoutedEventArgs e)
        {
            cbxElement.Visibility = Visibility.Visible;
            tbElementName.Visibility = Visibility.Hidden;

            cbxElementEntity.ItemsSource = GetEntities(EnvPath);
            cbxElementEntity.SelectedIndex = 0;

            cbxElement.ItemsSource = GetElements(EnvPath, (string)cbxElementEntity.SelectionBoxItem);
            cbxElement.SelectedIndex = 0;
        }

        private void cbxElementEntity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxElementEntity.SelectedValue == null) return;

            UpdateElementCombobox(cbxElementEntity.SelectedValue.ToString());
        }

        public void AddEntity(string env, string entity)
        {
            List<string> lines = new List<string>();
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "fun.Editor.exe",
                    Arguments = "add entity " + env + " " + entity,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            lines = proc.StandardOutput.ReadToEnd().Split('\n').ToList();

            try
            {
                tbCommand.Text = lines[0];
            }
            catch (Exception e) { MessageBox.Show(e.Message); }
        }

        public void RemoveEntity(string env, string entity)
        {
            List<string> lines = new List<string>();
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "fun.Editor.exe",
                    Arguments = "rm entity " + env + " " + entity,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                string line = proc.StandardOutput.ReadLine();
                lines.Add(line);
            }

            try
            {
                tbCommand.Text = lines[0];
            }
            catch (Exception e) { MessageBox.Show(e.Message); }
        }

        public void AddElement(string env, string entity, string element)
        {
            List<string> lines = new List<string>();
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "fun.Editor.exe",
                    Arguments = "add element " + env + " " + entity + " " + element,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                string line = proc.StandardOutput.ReadLine();
                lines.Add(line);
            }

            try
            {
                tbCommand.Text = lines[0];
            }
            catch (Exception e) { MessageBox.Show(e.Message); }
        }

        public void RemoveElement(string env, string entity, string element)
        {
            List<string> lines = new List<string>();
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "fun.Editor.exe",
                    Arguments = "rm entity " + env + " " + entity + " " + element,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                string line = proc.StandardOutput.ReadLine();
                lines.Add(line);
            }
            try
            {
                tbCommand.Text = lines[0];
            }
            catch (Exception e) { MessageBox.Show(e.Message); }
                
                

        }

        public void CreateEnvironment(string env)
        {
            List<string> lines = new List<string>();
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "fun.Editor.exe",
                    Arguments = "create " + env,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                string line = proc.StandardOutput.ReadLine();
                lines.Add(line);
            }

            try
            {
                tbCommand.Text = lines[0];
            }
            catch (Exception e) { MessageBox.Show(e.Message); }
        }
    }
}
