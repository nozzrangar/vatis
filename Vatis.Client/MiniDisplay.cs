using Appccelerate.EventBroker;
using Appccelerate.EventBroker.Handlers;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Vatsim.Vatis.Client.Config;
using Vatsim.Vatis.Client.Core;
using Vatsim.Vatis.Client.UI;

namespace Vatsim.Vatis.Client
{
    public partial class MiniDisplay : Form
    {
        [EventPublication(EventTopics.MinifiedWindowClosed)]
        public event EventHandler<EventArgs> MinifiedWindowClosed;

        private readonly IEventBroker mEventBroker;
        private readonly IAppConfig mAppConfig;
        private readonly System.Windows.Forms.Timer mUtcClock;
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        public MiniDisplay(IEventBroker eventBroker, IAppConfig appConfig)
        {
            InitializeComponent();

            mEventBroker = eventBroker;
            mEventBroker.Register(this);
            mAppConfig = appConfig;

            utcClock.Text = DateTime.UtcNow.ToString("HH:mm/ss");
            mUtcClock = new System.Windows.Forms.Timer
            {
                Interval = 500
            };
            mUtcClock.Tick += (s, e) =>
            {
                utcClock.Text = DateTime.UtcNow.ToString("HH:mm/ss");
            };
            mUtcClock.Start();
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams handleParam = base.CreateParams;
                handleParam.ExStyle |= 0x02000000;
                return handleParam;
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x84)
            {
                int x = ((short)((long)m.LParam));
                int y = ((short)((long)m.LParam >> 16));
                Point point = PointToClient(new Point(x, y));
                if (point.X >= ClientSize.Width - 6 && point.Y >= ClientSize.Height - 5)
                {
                    m.Result = (IntPtr)(IsMirrored ? 16 : 17);
                }
                else if (point.X <= 6 && point.Y >= ClientSize.Height - 5)
                {
                    m.Result = (IntPtr)(IsMirrored ? 17 : 16);
                }
                else if (point.X <= 6 && point.Y <= 5)
                {
                    m.Result = (IntPtr)(IsMirrored ? 14 : 13);
                }
                else if (point.X >= ClientSize.Width - 6 && point.Y <= 5)
                {
                    m.Result = (IntPtr)(IsMirrored ? 13 : 14);
                }
                else if (point.Y <= 5)
                {
                    m.Result = (IntPtr)12;
                }
                else if (point.Y >= ClientSize.Height - 5)
                {
                    m.Result = (IntPtr)15;
                }
                else if (point.X <= 6)
                {
                    m.Result = (IntPtr)10;
                }
                else if (point.X >= ClientSize.Width - 6)
                {
                    m.Result = (IntPtr)11;
                }
                else
                {
                    base.WndProc(ref m);
                    if ((int)m.Result == 1)
                    {
                        m.Result = (IntPtr)2;
                    }
                }
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            RefreshDisplay();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            Rectangle rect = new Rectangle(ClientRectangle.Left, ClientRectangle.Top, ClientRectangle.Width - 1, ClientRectangle.Height - 1);
            using Pen pen = new Pen(Color.FromArgb(0, 0, 0));
            pevent.Graphics.DrawRectangle(pen, rect);
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            MinifiedWindowClosed?.Invoke(this, EventArgs.Empty);
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            MinifiedWindowClosed?.Invoke(this, EventArgs.Empty);
            Close();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RefreshDisplay();
            utcClock.Visible = ClientSize.Width > 120;
            Invalidate();
        }

        [EventSubscription(EventTopics.RefreshMinifiedWindow, typeof(OnUserInterfaceAsync))]
        public void OnRefreshMinifiedWindow(object sender, EventArgs e)
        {
            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            tlpMain.Controls.Clear();

            if (mAppConfig == null || !mAppConfig.CurrentProfile.Composites.Any(x => x.Connection.IsConnected))
            {
                using Font font = new Font(Font.FontFamily, 14, FontStyle.Bold);
                tlpMain.Controls.Add(new Label
                {
                    Text = "No Composites Connected",
                    Dock = DockStyle.Fill,
                    Font = font,
                    ForeColor = Color.White,
                    TextAlign = ContentAlignment.MiddleCenter
                });
                MinimumSize = new Size(310, 75);
                return;
            }

            int colWidth = 75;
            int colCount = ClientSize.Width / colWidth;

            int rowHeight = 40;
            int rowCount = ClientSize.Height / rowHeight;

            tlpMain.RowCount = rowCount;
            for (int i = 0; i < rowCount; i++)
            {
                tlpMain.RowStyles.Add(new RowStyle
                {
                    SizeType = SizeType.Absolute,
                    Height = rowHeight,
                });
            }

            tlpMain.ColumnCount = colCount;
            for (int i = 0; i < colCount; i++)
            {
                tlpMain.ColumnStyles.Add(new ColumnStyle
                {
                    SizeType = SizeType.Absolute,
                    Width = colWidth
                });
            }

            if (mAppConfig != null)
            {
                int row = 0;
                int col = 0;
                foreach (var composite in mAppConfig.CurrentProfile.Composites)
                {
                    if (composite.Connection.IsConnected)
                    {
                        var item = new MiniDisplayItem
                        {
                            Icao = composite.Identifier,
                            AtisLetter = composite.CurrentAtisLetter,
                            Metar = composite?.DecodedMetar?.RawMetar ?? "",
                            Composite = composite,
                            Dock = DockStyle.Fill
                        };

                        composite.MetarReceived += (sender, args) =>
                        {
                            item.Metar = args.Value;
                        };
                        composite.NewAtisUpdate += (sender, args) =>
                        {
                            item.IsNewAtis = true;
                        };

                        tlpMain.Controls.Add(item, col, row);

                        if (col > colCount)
                        {
                            col = 0;
                            row++;
                        }

                        col++;
                    }
                }
            }
        }
    }
}
