using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SimpleTODO.SimpleTask.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using SimpleTODO.Services;
using System.Xml.Linq;
using System.IO;

namespace SimpleTODO
{
    /// <summary>
    /// Interaction logic for SimpleModWindow.xaml
    /// </summary>
    public partial class SimpleModWindow : Window
    {

        private ObservableCollection<TaskControl> tasksobsTodo = new ObservableCollection<TaskControl>();
        private ObservableCollection<TaskControl> tasksobsDone = new ObservableCollection<TaskControl>();
        private List<TaskControl> stared = new List<TaskControl>();

        public SimpleModWindow()
        {
            InitializeComponent();
            try
            {
                // with listbox ...
                listBoxTODO.ItemsSource = tasksobsTodo;                
                //countStar.DataContext = stared;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        #region MD
        private void CloseLbl_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {            
            Application.Current.Shutdown();
        }

        private void MiniMizLbl_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void CloseLbl_MouseEnter(object sender, MouseEventArgs e)
        {
            CloseLbl.Foreground = Brushes.Gray;
        }

        private void CloseLbl_MouseLeave(object sender, MouseEventArgs e)
        {
            CloseLbl.Foreground = this.WindowTheme.Background;
        }

        private void CloseLbl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CloseLbl.Foreground = Brushes.White;
        }

        private void MiniMizLbl_MouseEnter(object sender, MouseEventArgs e)
        {
            MiniMizLbl.Foreground = Brushes.Gray;
        }

        private void MiniMizLbl_MouseLeave(object sender, MouseEventArgs e)
        {
            MiniMizLbl.Foreground = this.WindowTheme.Background;
        }

        private void MiniMizLbl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MiniMizLbl.Foreground = Brushes.White;
        }

        private void grid1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch { }
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBox1.Text == "")
                textBlock1.Visibility = System.Windows.Visibility.Visible;
            else
                textBlock1.Visibility = System.Windows.Visibility.Hidden;
        }

        #endregion

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (textBox1.Text != "" && e.Key == Key.Enter)
            {

                CreateTask(false, false, textBox1.Text, ColorTasks.Blue);
                scrollViewer1.ScrollToEnd();
                textBox1.Text = "";
                
            }
        }

        private void CreateTask(bool isDone, bool isStar, string text, SimpleTODO.SimpleTask.Data.ColorTasks taskColor)
        {
            // create the task
            TaskControl newTask = new TaskControl();
            newTask.taskText = text;
            newTask.isCheck_Done = isDone;
            newTask.isCheck_Star = isStar;
            newTask.colorID = taskColor;
            // needed when open from xml
            if (newTask.isCheck_Done == true)
                tasksobsDone.Add(newTask);
            else// change it to another list .
                tasksobsTodo.Add(newTask);
            if (newTask.isCheck_Star == true)
                stared.Add(newTask);
            // add clickin behavior to handle the visual changes
            newTask.chkbox.Click += new RoutedEventHandler(checkBox_Status_Click);
            newTask.starcheck.Click += new RoutedEventHandler(starcheck_Click);
            newTask.Remove.Click += new RoutedEventHandler(Remove_Click);
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            TaskControl tasky = ((((sender as Control).Parent as Grid).Parent as Border).Parent as Border).Parent as TaskControl;
            if (tasksobsDone.Contains(tasky))
                tasksobsDone.Remove(tasky);
            else
                tasksobsTodo.Remove(tasky);
        }

        private void starcheck_Click(object sender, RoutedEventArgs e)
        {
            // just for counting
            TaskControl tasky = ((((sender as Control).Parent as Grid).Parent as Border).Parent as Border).Parent as TaskControl;

            if ((sender as CheckBox).IsChecked == true)
                stared.Add(tasky);
            else
                stared.Remove(tasky);
        }

        private void checkBox_Status_Click(object sender, RoutedEventArgs e)
        {
            TaskControl tasky = ((((sender as Control).Parent as Grid).Parent as Border).Parent as Border).Parent as TaskControl;
            
            if ((sender as CheckBox).IsChecked == true)
            {
                tasksobsTodo.Remove(tasky);
                tasksobsDone.Add(tasky);
            }
            else
            {
                tasksobsDone.Remove(tasky);
                tasksobsTodo.Add(tasky);
            }
        }

        private void window_Closing(object sender, CancelEventArgs e)
        {
            //IsSave = true;
            //SaveLoadInBG();

            List<UIElement> taskInCurrent = new List<UIElement>();
            foreach (TaskControl tc in tasksobsTodo)
            {
                taskInCurrent.Add(tc);
            }
            foreach (TaskControl tc in tasksobsDone)
            {
                taskInCurrent.Add(tc);
            }
            // call my service XMLMan to save my works
            XMLMan saveChanges = new XMLMan(taskInCurrent, SaveFileType.SaveToSameFile);
            saveChanges.SaveFile();
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            textBox1.Focusable = true;
            textBox1.Focus();
            try
            {
                tasksobsDone.Clear();
                tasksobsTodo.Clear();

                string path = "AppData\\data.xml";

                // Create the subfolder
                System.IO.Directory.CreateDirectory("AppData");

                if (File.Exists(path))
                {
                    XElement Open = XElement.Load(path);
                    foreach (XElement task in Open.Element("Tasks").Elements())
                    {
                        DeserializeGates(task);
                    }
                }
                else
                {
                    //Create the file.
                    FileStream fs = File.Create(path);
                    // and nothing to load yet
                }
            }
            catch { } 
        }

        private void DeserializeGates(XElement task)
        {
            // convert value
            bool stared = false;
            if (task.Attribute("Star").Value == "true")
                stared = true;

            bool done = false;
            if (task.Attribute("Done").Value == "true")
                done = true;

            //color to enum Converter
            SimpleTODO.SimpleTask.Data.ColorTasks tskcolor = ColorTasks.Black;
            string colorValu = task.Attribute("Color").Value;
            switch (colorValu)
            {
                case "Black":
                    tskcolor = ColorTasks.Black;
                    break;
                case "Blue":
                    tskcolor = ColorTasks.Blue;
                    break;
                case "Silver":
                    tskcolor = ColorTasks.Silver;
                    break;
                case "DoneGreen":
                    tskcolor = ColorTasks.DoneGreen;
                    break;
                case "Brown":
                    tskcolor = ColorTasks.Brown;
                    break;
            }

            // check for correct container
            CreateTask(done, stared, task.Attribute("Text").Value, tskcolor);
        }

        private void border1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                switch (border1.SelectedIndex)
                {
                    case 0:
                        listBoxTODO.ItemsSource = tasksobsTodo;
                        break;
                    case 1:
                        listBoxTODO.ItemsSource = tasksobsDone;
                        break;
                    case 2:
                        listBoxTODO.ItemsSource = stared;
                        break;
                }
            }
            catch { }
        }

        private void settingsLbl_MouseLeave(object sender, MouseEventArgs e)
        {
            settingsLbl.Foreground = this.WindowTheme.Background;
        }

        private void settingsLbl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            gridSettings.Visibility = System.Windows.Visibility.Visible;
        }

        private void settingsLbl_MouseEnter(object sender, MouseEventArgs e)
        {
            settingsLbl.Foreground = Brushes.Gray;
        }

        private void settingsLbl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            settingsLbl.Foreground = Brushes.White;
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            gridSettings.Visibility = System.Windows.Visibility.Hidden;

        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("This action will remove all your tasks! \n Todo, Done, stared ... etc sure you want continue !", "Ooopps", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                tasksobsDone.Clear();
                tasksobsTodo.Clear();
                stared.Clear();
            }            
            gridSettings.Visibility = System.Windows.Visibility.Hidden;

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Are you sure!","Info",MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                tasksobsDone.Clear();            
            gridSettings.Visibility = System.Windows.Visibility.Hidden;
        }

        private void btnArchive_Click(object sender, RoutedEventArgs e)
        {

        }

        private void listBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            WindowTheme.Background = (listBoxtheme.SelectedItem as ListBoxItem).Background;

            // save changes
        }

        private void listBox_Copy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //window.Background = (listBoxwindow.SelectedItem as ListBoxItem).Background; 

            /*
             * change: font color, the icon color, selection in listboxs
             * 
             * etc etc
             * 
             */
   
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://mhdnour.github.io/SimpleTODO");
        }

    }


}