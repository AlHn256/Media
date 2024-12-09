using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.VideoPanels
{
    public class MainControlInfo
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public object Value { get; set; }
        public object Tag { get; set; }
        public int WindowIndex { get; set; }

        public MainControlInfo() { }
    }

    public class ContainerControlInfo
    {
        public string Name { get; set; }
        public object Tag { get; set; }
        public int WindowIndex { get; set; }
        public object Value { get; set; }
        public bool Valid { get; set; }
        List<MainControlInfo> ChildItems { get; set; }
        public AlfaPribor.VideoPanels.VideoPanels.ToolbarButton ToolbarButtonType { get; set; }
        
        public ContainerControlInfo()
        {
            ChildItems = new List<MainControlInfo>();
            Valid = true;
        }
    }
    public class CustomButtonItemDictionary
    {
        public Dictionary<string, ContainerControlInfo> _customButtonInfoDictionary;
        public CustomButtonItemDictionary()
        {
            _customButtonInfoDictionary = new Dictionary<string, ContainerControlInfo>();
        }
        
        public bool AddItem(string key, ContainerControlInfo containerInfo)
        {
            bool result = true;
            try
            {
                _customButtonInfoDictionary.Add(key, containerInfo);
            }
            catch
            {
                result = false;
            }
            return result;
        }
        public ContainerControlInfo GetItem(string key)
        {
            ContainerControlInfo info = null;
            try
            {
                info = _customButtonInfoDictionary[key];
            }
            catch
            {
                info = new ContainerControlInfo{Valid=false};
            }
            return info;
        }
    }

}
