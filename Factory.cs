using LiveSplit.Model;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSplit.UI.Components
{
    public class Factory : IComponentFactory
    {
        public string ComponentName
        {
            get { return "Groove City Auto Splitter"; }
        }

        public IComponent Create(LiveSplitState state)
        {
            return new Component();
        }

        public string UpdateName
        {
            get { return ""; }
        }

        public string UpdateURL
        {
            get { return "http://livesplit.org/update/"; }
        }

        public Version Version
        {
            get { return new Version(); }
        }

        public string XMLURL
        {
            get { return "http://livesplit.org/update/Components/noupdates.xml"; }
        }
    }
}
