using LiveSplit.GrooveCity;
using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.TimeFormatters;
using LiveSplit.UI.Components;
using LiveSplit.Web;
using LiveSplit.Web.Share;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Timers;

namespace LiveSplit.UI.Components
{
    class Component : IComponent
    {
        public ComponentSettings Settings { get; set; }

        public string ComponentName
        {
            get { return "Groove City Auto Splitter"; }
        }

        public float PaddingBottom { get { return 0; } }
        public float PaddingTop { get { return 0; } }
        public float PaddingLeft { get { return 0; } }
        public float PaddingRight { get { return 0; } }

        public Thread RefreshThread { get; set; }
        public bool Refresh { get; set; }

        public IDictionary<string, Action> ContextMenuControls { get; protected set; }

        public Process Game { get; set; }

        protected static readonly DeepPointer LevelTimer = new DeepPointer("groovecity.exe", 0x9E110C, 0x288, 0x568, 0x660, 0x788);
        protected static readonly DeepPointer AliveTimer = new DeepPointer("groovecity.exe", 0x9E1920, 0x3FC);
        protected static readonly DeepPointer DeathCounter = new DeepPointer("groovecity.exe", 0x9DF8E8, 0x5B4, 0x150, 0x790, 0x5F0, 0x28);
        protected static readonly DeepPointer LevelID = new DeepPointer("groovecity.exe", 0x9B2264, 0x164);
        protected static readonly DeepPointer Score = new DeepPointer("groovecity.exe", 0x9E1114, 0xF0, 0x30, 0x580, 0x784, 0x10C);

        public TimeSpan GameTime { get; set; }

        //public bool WasTimeRunning { get; set; }
        public TimeSpan? OldAliveTime { get; set; }
        //public TimeSpan? OldLevelTime { get; set; }
        //public int OldDeathCounter { get; set; }
        public int OldLevelID { get; set; }

        protected TimerModel Model { get; set; }

        public Component()
        {
            Settings = new ComponentSettings();

            //ContextMenuControls = new Dictionary<String, Action>();
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            if (Game == null || Game.HasExited)
            {
                Game = null;
                var process = Process.GetProcessesByName("GrooveCity").FirstOrDefault();
                if (process != null)
                {
                    Game = process;
                }
            }

            if (!state.Run.CustomComparisons.Contains("Game Time"))
                state.Run.CustomComparisons.Add("Game Time");

            if (Model == null)
            {
                Model = new TimerModel() { CurrentState = state };
                state.OnStart += state_OnStart;
            }

            if (Game != null)
            {
                float time;
                //LevelTimer.Deref<float>(Game, out time);
                //var levelTime = TimeSpan.FromSeconds(time);

                AliveTimer.Deref<float>(Game, out time);
                var aliveTime = TimeSpan.FromSeconds(time);

                //int deathCounter;
                //DeathCounter.Deref<int>(Game, out deathCounter);

                int levelID;
                LevelID.Deref<int>(Game, out levelID);

                //bool isTimeRunning = OldLevelTime != levelTime;

                if (OldAliveTime != null)// && OldLevelTime != null)
                {
                    if (levelID != OldLevelID
                        && levelID >= 6
                        && levelID <= 22
                        && levelID != 7
                        && levelID != 8
                        && levelID != 18)
                    {
                        if (state.CurrentPhase == TimerPhase.NotRunning && levelID == 6)
                        {
                            Model.Start();
                        }
                        else if (state.CurrentPhase == TimerPhase.Running)
                        {
                            Model.Split();
                        }
                    }
                    if (levelID == 4 && state.CurrentPhase == TimerPhase.Running)
                    {
                        Model.Reset();
                    }
                    if (OldAliveTime > aliveTime)
                    {
                        //OldDeathCounter = deathCounter;
                        GameTime += OldAliveTime ?? TimeSpan.Zero;
                    }
                }

                state.IsLoading = true;
                state.CurrentGameTime = GameTime + aliveTime;

                //WasTimeRunning = isTimeRunning;
                //OldLevelTime = levelTime;
                OldAliveTime = aliveTime;
                OldLevelID = levelID;
            }
        }

        void state_OnStart(object sender, EventArgs e)
        {
            GameTime = TimeSpan.Zero;
            //OldLevelTime = null;
            OldAliveTime = null;
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
        }

        public float VerticalHeight
        {
            get { return 0; }
        }

        public float MinimumWidth
        {
            get { return 0; }
        }

        public float HorizontalWidth
        {
            get { return 0; }
        }

        public float MinimumHeight
        {
            get { return 0; }
        }

        public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
        {
            return document.CreateElement("x");
        }

        public System.Windows.Forms.Control GetSettingsControl(UI.LayoutMode mode)
        {
            return null;
        }

        public void SetSettings(System.Xml.XmlNode settings)
        {
        }

        public void RenameComparison(string oldName, string newName)
        {
        }
    }
}
