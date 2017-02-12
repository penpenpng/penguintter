using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Penguintter
{
    [Serializable()]
    public class Config
    {
        public LimitedInt StatusLifeTime { get; set; } = new LimitedInt(1, 60, 8);
        public bool AllowMultiLines { get; set; } = true;

        public LimitedInt StatusDisplyType_WidthLimit { get; set; } = new LimitedInt(1, 3000, 600);
        public bool StatusDisplyType_ShowAll { get; set; } = true;
        public bool StatusDisplyType_Wrap { get; set; } = false;
        public bool StatusDisplyType_Hide { get; set; } = false;

        public LimitedInt ScreenNameFontSize { get; set; } = new LimitedInt(1, 50, 16);
        public LimitedInt StatusFontSize { get; set; } = new LimitedInt(1, 50, 24);

        public BindableColor NormalColor { get; set; } = new BindableColor(0, 0, 0, 255);
        public BindableColor RetweetColor { get; set; } = new BindableColor(34, 139, 34, 255);
        public BindableColor ReplyColor { get; set; } = new BindableColor(255, 0, 0, 255);

        public BindableColor NormalBackground { get; set; } = new BindableColor(255, 255, 255, 0);
        public BindableColor RetweetBackground { get; set; } = new BindableColor(255, 255, 255, 0);
        public BindableColor ReplyBackground { get; set; } = new BindableColor(255, 255, 255, 0);

        public bool NotShowDetailView { get; set; } = false;
        public LimitedInt DetailLifeTime { get; set; } = new LimitedInt(1, 60, 5);
        public BindableColor DetailBackground { get; set; } = new BindableColor(72, 209, 204, 180);

        public LimitedInt UpdateFontSize { get; set; } = new LimitedInt(1, 50, 12);

        public LimitedInt LogFontSize { get; set; } = new LimitedInt(1, 50, 12);
        public LimitedInt MaxLogCount { get; set; } = new LimitedInt(0, 2000, 200);


        static public string ConfigFileName { get; } = @"penguintter.conf";
        public void SaveConfig()
        {
            try
            {
                using (var st = new System.IO.FileStream(ConfigFileName, System.IO.FileMode.Create))
                {
                    new BinaryFormatter().Serialize(st, this);
                }
            }
            catch(Exception e)
            {
                DevTool.Print(e);
            }
        }

        static public Config LoadConfig()
        {
            using (var st = new System.IO.FileStream(ConfigFileName, System.IO.FileMode.Open))
            {
                return (Config)new BinaryFormatter().Deserialize(st);
            }
        }
    }

    [Serializable()]
    public class BindableColor
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte A { get; set; }

        public BindableColor(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public Color Color
        {
            get {
                return new Color() {
                    R = R,
                    G = G,
                    B = B,
                    A = A,
                };
            }
            set {
                R = value.R;
                G = value.G;
                B = value.B;
                A = value.A;
            }
        }
    }

    [Serializable()]
    public class LimitedInt
    {
        private int _Value;
        private int Max { get; set; }
        private int Min { get; set; }

        public LimitedInt(int min, int max, int value)
        {
            Max = max;
            Min = min;
            Value = value;
        }

        public int Value
        {
            get { return _Value; }
            set
            {
                if (!(Min <= value && value <= Max))
                {
                    throw new ArgumentException();
                }
                _Value = value;
            }
        }
    }
}
