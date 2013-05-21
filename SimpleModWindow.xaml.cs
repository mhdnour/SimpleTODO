﻿using System;
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


/*
 * todo yet:
 * 
 * save+load inside background worker
 * some animations when selecting todo or done or stared lists...
 * save my works to cloud ????
 * adding more color ... but dont wanna 2 use another libraries ?!?!?
 * and finally settings to change the theme color
 * 
 */
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

        private bool IsSave = false;

        //public int StaredCount
        //{
        //    get 
        //    {
        //        return stared.Count;
        //    }

        //    set 
        //    {
        //        // cannot doit it's read only
        //        OnPropertyChanged("StaredCount");
        //    }
        //}

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
                //MessageBox.Show("Ok");
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
            else// change it to another list ........................................??????????
                tasksobsTodo.Add(newTask);
            if (newTask.isCheck_Star == true)
                stared.Add(newTask);
            // add clickin behavior to handle the visual changes
            newTask.chkbox.Click += new RoutedEventHandler(checkBox_Status_Click);
            newTask.starcheck.Click += new RoutedEventHandler(starcheck_Click);
        }

        void starcheck_Click(object sender, RoutedEventArgs e)
        {
            // just for counting
            TaskControl tasky = ((((sender as Control).Parent as Grid).Parent as Border).Parent as Border).Parent as TaskControl;

            if ((sender as CheckBox).IsChecked == true)
                stared.Add(tasky);
            else
                stared.Remove(tasky);
            //MessageBox.Show(StaredCount.ToString());
            countStar.Text = stared.Count.ToString();
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


            //IsSave = false;
            //SaveLoadInBG();
        }

        private void SaveLoadInBG()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bw.RunWorkerAsync();
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;

            if (IsSave)
            {
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
            else
            {
                try
                {                    
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
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                } 
            }
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        { }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
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

        //#region for stars count


        //public event PropertyChangedEventHandler PropertyChanged;
        //protected void OnPropertyChanged(string propertyName)
        //{
        //    if (!string.IsNullOrEmpty(propertyName))
        //    {
        //        if (this.PropertyChanged != null)
        //            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //    //this.OnObjectChanged();
        //}

        //#endregion

    }

    /// <summary>
    /// Who is the caller for the background worker?
    /// </summary>
    public enum BackGroundCaller
    {
        Save,
        Load
    }
}
/*
 *  read from xml file and place all the items in the observlist ... idont know check 
 *  stackoverflow for it
 *  
 * then foreach(item in this list)
 * 
 * if isStar = true
 * 
 * newlist.add(item)
 * 
 * and can change while changin the selections from 
 * Done
 * todo
 * important
 */