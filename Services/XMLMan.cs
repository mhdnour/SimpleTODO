using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;
using SimpleTODO.SimpleTask.Data;


namespace SimpleTODO.Services
{
    class XMLMan
    {
        // current available tasks
        private List<UIElement> myTasks = new List<UIElement>();
        private SaveFileType savrType;

        public XMLMan(List<UIElement> tasks, SaveFileType savetype)
        {
            myTasks = tasks;
            savrType = savetype;
        }

        private XElement SerializeWorks()
        {

            XElement xTask = new XElement("Tasks");
            
            foreach (UIElement taskToSave in myTasks)
            {                
                XElement MyXGates = new XElement("Task");
                MyXGates.SetAttributeValue("Done", ((TaskControl)taskToSave).isCheck_Done.Value);
                MyXGates.SetAttributeValue("Star", ((TaskControl)taskToSave).isCheck_Star.Value);
                MyXGates.SetAttributeValue("Text", ((TaskControl)taskToSave).taskText);
                MyXGates.SetAttributeValue("Color", ((TaskControl)taskToSave).colorID);
                xTask.Add(MyXGates);
            }
            return xTask;            
        }

        public void SaveFile()
        {
            try
            {
                XElement toFile = new XElement("SimpleToDolist.Data");
                toFile.Add(SerializeWorks());
                toFile.SetAttributeValue("About", "Simple TODO v3.1");
                if (savrType == SaveFileType.SaveToSameFile)
                {
                    toFile.Save("AppData\\data.xml");                    
                }
                else
                    if (savrType == SaveFileType.SaveToSameFile)
                    {
                        toFile.Save("AppData\\data.xml.BackupOfReset"); 
                    }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

    }
}
