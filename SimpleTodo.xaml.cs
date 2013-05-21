using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml.Linq;
using MahApps.Metro.Controls;
using SimpleTODO.Services;
using SimpleTODO.SimpleTask.Data;

namespace SimpleTODO
{
    public partial class Simple : MetroWindow
    {
        public Simple()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (textBox1.Text != "")
            {
                CreateTask(false, false, textBox1.Text, ColorTasks.Black);
                scroll_Current.ScrollToEnd();
                textBox1.Text = "";
                countUndone.Content = list_Current.Children.Count;
            }
            else
                MessageBox.Show("Task cannot be empty");
        }
        // Visual
        private void checkBox_Status_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
            {
                list_Current.Children.Remove((((((sender as Control).Parent as Grid).Parent as Border).Parent as Border).Parent as TaskControl));
                list_Done.Children.Add((((((sender as Control).Parent as Grid).Parent as Border).Parent as Border).Parent as TaskControl));
                scroll_Done.ScrollToEnd();
            }
            else
            {
                list_Done.Children.Remove((((((sender as Control).Parent as Grid).Parent as Border).Parent as Border).Parent as TaskControl));
                list_Current.Children.Add((((((sender as Control).Parent as Grid).Parent as Border).Parent as Border).Parent as TaskControl));
                scroll_Current.ScrollToEnd();
            }
            countDone.Content = list_Done.Children.Count;
            countUndone.Content = list_Current.Children.Count;
        }
        // save works at exit
        private void SimpleWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // all the tasks
             
            List<UIElement> taskInCurrent = new List<UIElement>();
            foreach (TaskControl tc in list_Current.Children)
            {
                taskInCurrent.Add(tc);
            }
            foreach (TaskControl tc in list_Done.Children)
            {
                taskInCurrent.Add(tc);
            }
            // call my service XMLMan to save my works
            XMLMan saveChanges = new XMLMan(taskInCurrent, SaveFileType.SaveToSameFile);
            saveChanges.SaveFile();
             
             
            
        }
        // resotre the database :p (last saved works)
        private void SimpleWindow_Loaded(object sender, RoutedEventArgs e)
        {            
            try
            {
                textBox1.Focusable = true;
                textBox1.Focus();

                list_Done.Children.Clear();
                list_Current.Children.Clear();

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
                countDone.Content = list_Done.Children.Count;
                countUndone.Content = list_Current.Children.Count;
            }
            catch {} 
              

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
            switch(colorValu)
            {
                case"Black":
                    tskcolor = ColorTasks.Black;
                    break;
                case"Blue":
                    tskcolor = ColorTasks.Blue;
                    break;
                case"Silver":
                    tskcolor = ColorTasks.Silver;
                    break;
				case"DoneGreen":
                    tskcolor = ColorTasks.DoneGreen;
                    break;
                case "Brown":
                    tskcolor = ColorTasks.Brown;
                    break;
            }
            
            // check for correct container
            CreateTask(done, stared, task.Attribute("Text").Value, tskcolor);           
        }

        private void CreateTask(bool isDone, bool isStar, string text, SimpleTODO.SimpleTask.Data.ColorTasks taskColor)
        {
            // create the task
            TaskControl newTask = new TaskControl();
            newTask.taskText = text;
            newTask.isCheck_Done = isDone;
            newTask.isCheck_Star = isStar;
            newTask.colorID = taskColor;
            if (newTask.isCheck_Done == false)
                list_Current.Children.Add(newTask);
            else
                list_Done.Children.Add(newTask);
            // add clickin behavior to handle the visual changes
            newTask.chkbox.Click += new RoutedEventHandler(checkBox_Status_Click);
			
        }
        // visual
        private void textBox1_GotFocus(object sender, RoutedEventArgs e)
        {
        	button1.IsDefault = true;
        }

        private void textBox1_LostFocus(object sender, RoutedEventArgs e)
        {
			button1.IsDefault = false;
        }
        
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Remove all tasks Done, undone, important", "Warnning !", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                // all the tasks
                List<UIElement> taskInCurrent = new List<UIElement>();
                foreach (TaskControl tc in list_Current.Children)
                {
                    taskInCurrent.Add(tc);
                }
                foreach (TaskControl tc in list_Done.Children)
                {
                    taskInCurrent.Add(tc);
                }
                // call my service XMLMan to save my works
                XMLMan saveChanges = new XMLMan(taskInCurrent, SaveFileType.CreateCopyOfCurrentWorks);
                saveChanges.SaveFile();

                list_Done.Children.Clear();
                list_Current.Children.Clear();

                countDone.Content = countUndone.Content = 0;
            }
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            
        }

        /* 
         * http://stackoverflow.com/questions/2373343/sorting-elements-in-a-stackpanel-wpf
         * 
         * to make filters like: (show stared only, show only selected colors ... etc)
         * and if you want to use move up/down:
         * 
         * 
         * 
         * The answer above is correct, but if you can not change your stackpanel 
         * (if you have not enough time, or have written many codes related to the stackpanel) try this:

            Store the controls in a List or Dictionary
            Sort the List or Dictionary
            Remove controls from stackpanel using : StackPanel.Children.Remove(child)
            Foreach member of List or Dictionary add controls to StackPanel using : StackPanel.Children.Insert(i, child);

            note: the code is working, Remove function removes the control from StackPanel item's (from the tree) 
         * but the control is already on the memory 
         * so that the control is able to inserting in any StackPanel or same of it.
         * 
         * 
         */

    }
}
