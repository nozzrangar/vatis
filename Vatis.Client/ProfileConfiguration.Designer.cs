
namespace Vatsim.Vatis.Client
{
    partial class ProfileConfiguration
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProfileConfiguration));
            this.TlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.pagePresets = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtAtisTemplate = new System.Windows.Forms.TextBox();
            this.btnRenamePreset = new System.Windows.Forms.Button();
            this.btnDeletePreset = new System.Windows.Forms.Button();
            this.btnCopyPreset = new System.Windows.Forms.Button();
            this.btnNewPreset = new System.Windows.Forms.Button();
            this.ddlPresets = new System.Windows.Forms.ComboBox();
            this.pageContractions = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.btnDeleteContraction = new System.Windows.Forms.Button();
            this.gridContractions = new System.Windows.Forms.DataGridView();
            this.ColumnFind = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnReplace = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pageConfiguration = new System.Windows.Forms.TabPage();
            this.vhfFrequency = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.observationTime = new System.Windows.Forms.NumericUpDown();
            this.chkObservationTime = new System.Windows.Forms.CheckBox();
            this.radioVoiceRecorded = new System.Windows.Forms.RadioButton();
            this.radioTextToSpeech = new System.Windows.Forms.RadioButton();
            this.ddlVoices = new System.Windows.Forms.ComboBox();
            this.txtIdsEndpoint = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkMagneticVar = new System.Windows.Forms.CheckBox();
            this.magneticVar = new System.Windows.Forms.NumericUpDown();
            this.pageTransitionLevel = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.btnDeleteTransitionLevel = new System.Windows.Forms.Button();
            this.gridTransitionLevels = new System.Windows.Forms.DataGridView();
            this.low = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.high = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.transitionLevel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnManageComposite = new System.Windows.Forms.Button();
            this.ctxOptions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxNew = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxRename = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ctxImport = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxExport = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panelCompositeList = new Vatsim.Vatis.Client.UI.HitTestPanel();
            this.TreeMenu = new System.Windows.Forms.TreeView();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.compositeTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TlpMain.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.mainTabControl.SuspendLayout();
            this.pagePresets.SuspendLayout();
            this.panel2.SuspendLayout();
            this.pageContractions.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridContractions)).BeginInit();
            this.pageConfiguration.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.vhfFrequency)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.observationTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.magneticVar)).BeginInit();
            this.pageTransitionLevel.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridTransitionLevels)).BeginInit();
            this.ctxOptions.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelCompositeList.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // TlpMain
            // 
            this.TlpMain.ColumnCount = 3;
            this.TlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 726F));
            this.TlpMain.Controls.Add(this.tableLayoutPanel1, 2, 0);
            this.TlpMain.Controls.Add(this.btnManageComposite, 1, 1);
            this.TlpMain.Controls.Add(this.panel1, 1, 0);
            this.TlpMain.Controls.Add(this.flowLayoutPanel1, 2, 1);
            this.TlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TlpMain.Location = new System.Drawing.Point(15, 15);
            this.TlpMain.Margin = new System.Windows.Forms.Padding(0);
            this.TlpMain.Name = "TlpMain";
            this.TlpMain.RowCount = 2;
            this.TlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.TlpMain.Size = new System.Drawing.Size(934, 431);
            this.TlpMain.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.mainTabControl, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(212, 3);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(718, 390);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // mainTabControl
            // 
            this.mainTabControl.Controls.Add(this.pagePresets);
            this.mainTabControl.Controls.Add(this.pageContractions);
            this.mainTabControl.Controls.Add(this.pageConfiguration);
            this.mainTabControl.Controls.Add(this.pageTransitionLevel);
            this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabControl.Enabled = false;
            this.mainTabControl.Location = new System.Drawing.Point(0, 0);
            this.mainTabControl.Margin = new System.Windows.Forms.Padding(0);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(718, 390);
            this.mainTabControl.TabIndex = 1;
            // 
            // pagePresets
            // 
            this.pagePresets.Controls.Add(this.label5);
            this.pagePresets.Controls.Add(this.panel2);
            this.pagePresets.Controls.Add(this.btnRenamePreset);
            this.pagePresets.Controls.Add(this.btnDeletePreset);
            this.pagePresets.Controls.Add(this.btnCopyPreset);
            this.pagePresets.Controls.Add(this.btnNewPreset);
            this.pagePresets.Controls.Add(this.ddlPresets);
            this.pagePresets.Location = new System.Drawing.Point(4, 24);
            this.pagePresets.Name = "pagePresets";
            this.pagePresets.Padding = new System.Windows.Forms.Padding(10);
            this.pagePresets.Size = new System.Drawing.Size(710, 362);
            this.pagePresets.TabIndex = 0;
            this.pagePresets.Text = "Presets";
            this.pagePresets.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 43);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 15);
            this.label5.TabIndex = 9;
            this.label5.Text = "ATIS Template:";
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.txtAtisTemplate);
            this.panel2.Location = new System.Drawing.Point(14, 63);
            this.panel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(4, 6, 6, 6);
            this.panel2.Size = new System.Drawing.Size(680, 286);
            this.panel2.TabIndex = 6;
            // 
            // txtAtisTemplate
            // 
            this.txtAtisTemplate.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtAtisTemplate.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtAtisTemplate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAtisTemplate.Enabled = false;
            this.txtAtisTemplate.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtAtisTemplate.Location = new System.Drawing.Point(4, 6);
            this.txtAtisTemplate.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtAtisTemplate.Multiline = true;
            this.txtAtisTemplate.Name = "txtAtisTemplate";
            this.txtAtisTemplate.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAtisTemplate.Size = new System.Drawing.Size(668, 272);
            this.txtAtisTemplate.TabIndex = 4;
            this.txtAtisTemplate.TextChanged += new System.EventHandler(this.txtTemplate_TextChanged);
            // 
            // btnRenamePreset
            // 
            this.btnRenamePreset.Enabled = false;
            this.btnRenamePreset.Location = new System.Drawing.Point(564, 14);
            this.btnRenamePreset.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnRenamePreset.Name = "btnRenamePreset";
            this.btnRenamePreset.Size = new System.Drawing.Size(64, 24);
            this.btnRenamePreset.TabIndex = 5;
            this.btnRenamePreset.Text = "Rename";
            this.btnRenamePreset.UseVisualStyleBackColor = true;
            this.btnRenamePreset.Click += new System.EventHandler(this.btnRenamePreset_Click);
            // 
            // btnDeletePreset
            // 
            this.btnDeletePreset.Enabled = false;
            this.btnDeletePreset.Location = new System.Drawing.Point(631, 14);
            this.btnDeletePreset.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnDeletePreset.Name = "btnDeletePreset";
            this.btnDeletePreset.Size = new System.Drawing.Size(64, 24);
            this.btnDeletePreset.TabIndex = 3;
            this.btnDeletePreset.Text = "Delete";
            this.btnDeletePreset.UseVisualStyleBackColor = true;
            this.btnDeletePreset.Click += new System.EventHandler(this.btnDeletePreset_Click);
            // 
            // btnCopyPreset
            // 
            this.btnCopyPreset.Enabled = false;
            this.btnCopyPreset.Location = new System.Drawing.Point(496, 14);
            this.btnCopyPreset.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnCopyPreset.Name = "btnCopyPreset";
            this.btnCopyPreset.Size = new System.Drawing.Size(64, 24);
            this.btnCopyPreset.TabIndex = 2;
            this.btnCopyPreset.Text = "Copy";
            this.btnCopyPreset.UseVisualStyleBackColor = true;
            this.btnCopyPreset.Click += new System.EventHandler(this.btnCopyPreset_Click);
            // 
            // btnNewPreset
            // 
            this.btnNewPreset.Location = new System.Drawing.Point(428, 14);
            this.btnNewPreset.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnNewPreset.Name = "btnNewPreset";
            this.btnNewPreset.Size = new System.Drawing.Size(64, 24);
            this.btnNewPreset.TabIndex = 1;
            this.btnNewPreset.Text = "New";
            this.btnNewPreset.UseVisualStyleBackColor = true;
            this.btnNewPreset.Click += new System.EventHandler(this.btnNewPreset_Click);
            // 
            // ddlPresets
            // 
            this.ddlPresets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlPresets.FormattingEnabled = true;
            this.ddlPresets.Location = new System.Drawing.Point(14, 15);
            this.ddlPresets.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ddlPresets.Name = "ddlPresets";
            this.ddlPresets.Size = new System.Drawing.Size(408, 23);
            this.ddlPresets.TabIndex = 0;
            this.ddlPresets.SelectedIndexChanged += new System.EventHandler(this.ddlPresets_SelectedIndexChanged);
            // 
            // pageContractions
            // 
            this.pageContractions.Controls.Add(this.tableLayoutPanel3);
            this.pageContractions.Location = new System.Drawing.Point(4, 24);
            this.pageContractions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pageContractions.Name = "pageContractions";
            this.pageContractions.Padding = new System.Windows.Forms.Padding(12);
            this.pageContractions.Size = new System.Drawing.Size(710, 362);
            this.pageContractions.TabIndex = 1;
            this.pageContractions.Text = "Contractions";
            this.pageContractions.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.btnDeleteContraction, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.gridContractions, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(686, 338);
            this.tableLayoutPanel3.TabIndex = 2;
            // 
            // btnDeleteContraction
            // 
            this.btnDeleteContraction.Location = new System.Drawing.Point(562, 306);
            this.btnDeleteContraction.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnDeleteContraction.Name = "btnDeleteContraction";
            this.btnDeleteContraction.Size = new System.Drawing.Size(120, 27);
            this.btnDeleteContraction.TabIndex = 1;
            this.btnDeleteContraction.Text = "Delete Selected";
            this.btnDeleteContraction.UseVisualStyleBackColor = true;
            this.btnDeleteContraction.Click += new System.EventHandler(this.btnDeleteContraction_Click);
            // 
            // gridContractions
            // 
            this.gridContractions.AllowUserToResizeColumns = false;
            this.gridContractions.AllowUserToResizeRows = false;
            this.gridContractions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridContractions.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridContractions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridContractions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnFind,
            this.ColumnReplace});
            this.gridContractions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridContractions.Location = new System.Drawing.Point(4, 3);
            this.gridContractions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gridContractions.Name = "gridContractions";
            this.gridContractions.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.gridContractions.RowHeadersVisible = false;
            this.gridContractions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridContractions.Size = new System.Drawing.Size(678, 297);
            this.gridContractions.TabIndex = 2;
            this.gridContractions.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridContractions_CellEndEdit);
            this.gridContractions.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.gridContractions_CellValidating);
            this.gridContractions.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridContractions_CellValueChanged);
            this.gridContractions.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.gridContractions_EditingControlShowing);
            this.gridContractions.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.gridContractions_UserDeletedRow);
            this.gridContractions.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.gridContractions_UserDeletingRow);
            // 
            // ColumnFind
            // 
            this.ColumnFind.HeaderText = "String";
            this.ColumnFind.Name = "ColumnFind";
            // 
            // ColumnReplace
            // 
            this.ColumnReplace.HeaderText = "Spoken";
            this.ColumnReplace.Name = "ColumnReplace";
            // 
            // pageConfiguration
            // 
            this.pageConfiguration.Controls.Add(this.vhfFrequency);
            this.pageConfiguration.Controls.Add(this.label3);
            this.pageConfiguration.Controls.Add(this.observationTime);
            this.pageConfiguration.Controls.Add(this.chkObservationTime);
            this.pageConfiguration.Controls.Add(this.radioVoiceRecorded);
            this.pageConfiguration.Controls.Add(this.radioTextToSpeech);
            this.pageConfiguration.Controls.Add(this.ddlVoices);
            this.pageConfiguration.Controls.Add(this.txtIdsEndpoint);
            this.pageConfiguration.Controls.Add(this.label2);
            this.pageConfiguration.Controls.Add(this.chkMagneticVar);
            this.pageConfiguration.Controls.Add(this.magneticVar);
            this.pageConfiguration.Location = new System.Drawing.Point(4, 24);
            this.pageConfiguration.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pageConfiguration.Name = "pageConfiguration";
            this.pageConfiguration.Size = new System.Drawing.Size(710, 362);
            this.pageConfiguration.TabIndex = 2;
            this.pageConfiguration.Text = "Configuration";
            this.pageConfiguration.UseVisualStyleBackColor = true;
            // 
            // vhfFrequency
            // 
            this.vhfFrequency.DecimalPlaces = 3;
            this.vhfFrequency.Location = new System.Drawing.Point(100, 39);
            this.vhfFrequency.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.vhfFrequency.Maximum = new decimal(new int[] {
            135975,
            0,
            0,
            196608});
            this.vhfFrequency.Minimum = new decimal(new int[] {
            118000,
            0,
            0,
            196608});
            this.vhfFrequency.Name = "vhfFrequency";
            this.vhfFrequency.Size = new System.Drawing.Size(96, 23);
            this.vhfFrequency.TabIndex = 15;
            this.vhfFrequency.Value = new decimal(new int[] {
            118000,
            0,
            0,
            196608});
            this.vhfFrequency.ValueChanged += new System.EventHandler(this.vhfFrequency_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 43);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 15);
            this.label3.TabIndex = 14;
            this.label3.Text = "Frequency:";
            this.compositeTooltip.SetToolTip(this.label3, "The VHF frequency of the ATIS station.");
            // 
            // observationTime
            // 
            this.observationTime.Enabled = false;
            this.observationTime.Location = new System.Drawing.Point(201, 103);
            this.observationTime.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.observationTime.Name = "observationTime";
            this.observationTime.Size = new System.Drawing.Size(74, 23);
            this.observationTime.TabIndex = 13;
            this.observationTime.ValueChanged += new System.EventHandler(this.observationTime_ValueChanged);
            // 
            // chkObservationTime
            // 
            this.chkObservationTime.AutoSize = true;
            this.chkObservationTime.Location = new System.Drawing.Point(27, 105);
            this.chkObservationTime.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkObservationTime.Name = "chkObservationTime";
            this.chkObservationTime.Size = new System.Drawing.Size(160, 19);
            this.chkObservationTime.TabIndex = 12;
            this.chkObservationTime.Text = "Offical Observation Time:";
            this.compositeTooltip.SetToolTip(this.chkObservationTime, resources.GetString("chkObservationTime.ToolTip"));
            this.chkObservationTime.UseVisualStyleBackColor = true;
            this.chkObservationTime.CheckedChanged += new System.EventHandler(this.chkObservationTime_CheckedChanged);
            // 
            // radioVoiceRecorded
            // 
            this.radioVoiceRecorded.AutoSize = true;
            this.radioVoiceRecorded.Location = new System.Drawing.Point(439, 239);
            this.radioVoiceRecorded.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.radioVoiceRecorded.Name = "radioVoiceRecorded";
            this.radioVoiceRecorded.Size = new System.Drawing.Size(106, 19);
            this.radioVoiceRecorded.TabIndex = 11;
            this.radioVoiceRecorded.Text = "Voice Recorded";
            this.compositeTooltip.SetToolTip(this.radioVoiceRecorded, "Manually voice record the ATIS using a microphone device.");
            this.radioVoiceRecorded.UseVisualStyleBackColor = true;
            this.radioVoiceRecorded.CheckedChanged += new System.EventHandler(this.radioVoiceRecorded_CheckedChanged);
            // 
            // radioTextToSpeech
            // 
            this.radioTextToSpeech.AutoSize = true;
            this.radioTextToSpeech.Checked = true;
            this.radioTextToSpeech.Location = new System.Drawing.Point(27, 239);
            this.radioTextToSpeech.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.radioTextToSpeech.Name = "radioTextToSpeech";
            this.radioTextToSpeech.Size = new System.Drawing.Size(104, 19);
            this.radioTextToSpeech.TabIndex = 10;
            this.radioTextToSpeech.TabStop = true;
            this.radioTextToSpeech.Text = "Text to Speech:";
            this.compositeTooltip.SetToolTip(this.radioTextToSpeech, "Use synthesized text to speech to generate the ATIS voice.");
            this.radioTextToSpeech.UseVisualStyleBackColor = true;
            this.radioTextToSpeech.CheckedChanged += new System.EventHandler(this.radioTextToSpeech_CheckedChanged);
            // 
            // ddlVoices
            // 
            this.ddlVoices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlVoices.FormattingEnabled = true;
            this.ddlVoices.Items.AddRange(new object[] {
            "Default",
            "US Male",
            "US Female",
            "UK Male",
            "UK Female"});
            this.ddlVoices.Location = new System.Drawing.Point(146, 237);
            this.ddlVoices.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ddlVoices.Name = "ddlVoices";
            this.ddlVoices.Size = new System.Drawing.Size(264, 23);
            this.ddlVoices.TabIndex = 9;
            this.ddlVoices.SelectedIndexChanged += new System.EventHandler(this.ddlVoices_SelectedIndexChanged);
            // 
            // txtIdsEndpoint
            // 
            this.txtIdsEndpoint.Location = new System.Drawing.Point(119, 301);
            this.txtIdsEndpoint.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtIdsEndpoint.Name = "txtIdsEndpoint";
            this.txtIdsEndpoint.Size = new System.Drawing.Size(438, 23);
            this.txtIdsEndpoint.TabIndex = 7;
            this.txtIdsEndpoint.TextChanged += new System.EventHandler(this.txtIdsEndpoint_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 306);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "IDS Endpoint:";
            this.compositeTooltip.SetToolTip(this.label2, "Specify the URL endpoint of an IDS. \r\nWhen an ATIS update occurs, vATIS will post" +
        " the update to the IDS to automatically update it.");
            // 
            // chkMagneticVar
            // 
            this.chkMagneticVar.AutoSize = true;
            this.chkMagneticVar.Location = new System.Drawing.Point(27, 172);
            this.chkMagneticVar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkMagneticVar.Name = "chkMagneticVar";
            this.chkMagneticVar.Size = new System.Drawing.Size(128, 19);
            this.chkMagneticVar.TabIndex = 4;
            this.chkMagneticVar.Text = "Magnetic Variation:";
            this.compositeTooltip.SetToolTip(this.chkMagneticVar, "Add or subtract the specified number of degrees from the wind direction.");
            this.chkMagneticVar.UseVisualStyleBackColor = true;
            this.chkMagneticVar.CheckedChanged += new System.EventHandler(this.chkMagneticVar_CheckedChanged);
            // 
            // magneticVar
            // 
            this.magneticVar.Enabled = false;
            this.magneticVar.Location = new System.Drawing.Point(173, 170);
            this.magneticVar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.magneticVar.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.magneticVar.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.magneticVar.Name = "magneticVar";
            this.magneticVar.Size = new System.Drawing.Size(79, 23);
            this.magneticVar.TabIndex = 1;
            this.magneticVar.ValueChanged += new System.EventHandler(this.magneticVar_ValueChanged);
            // 
            // pageTransitionLevel
            // 
            this.pageTransitionLevel.Controls.Add(this.tableLayoutPanel5);
            this.pageTransitionLevel.Location = new System.Drawing.Point(4, 24);
            this.pageTransitionLevel.Name = "pageTransitionLevel";
            this.pageTransitionLevel.Padding = new System.Windows.Forms.Padding(12);
            this.pageTransitionLevel.Size = new System.Drawing.Size(710, 362);
            this.pageTransitionLevel.TabIndex = 3;
            this.pageTransitionLevel.Text = "Transition Level";
            this.pageTransitionLevel.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Controls.Add(this.btnDeleteTransitionLevel, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.gridTransitionLevels, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 99.99999F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(686, 338);
            this.tableLayoutPanel5.TabIndex = 3;
            // 
            // btnDeleteTransitionLevel
            // 
            this.btnDeleteTransitionLevel.Location = new System.Drawing.Point(562, 306);
            this.btnDeleteTransitionLevel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnDeleteTransitionLevel.Name = "btnDeleteTransitionLevel";
            this.btnDeleteTransitionLevel.Size = new System.Drawing.Size(120, 27);
            this.btnDeleteTransitionLevel.TabIndex = 1;
            this.btnDeleteTransitionLevel.Text = "Delete Selected";
            this.btnDeleteTransitionLevel.UseVisualStyleBackColor = true;
            this.btnDeleteTransitionLevel.Click += new System.EventHandler(this.btnDeleteTransitionLevel_Click);
            // 
            // gridTransitionLevels
            // 
            this.gridTransitionLevels.AllowUserToResizeColumns = false;
            this.gridTransitionLevels.AllowUserToResizeRows = false;
            this.gridTransitionLevels.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridTransitionLevels.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridTransitionLevels.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridTransitionLevels.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.low,
            this.high,
            this.transitionLevel});
            this.gridTransitionLevels.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridTransitionLevels.Location = new System.Drawing.Point(4, 3);
            this.gridTransitionLevels.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.gridTransitionLevels.Name = "gridTransitionLevels";
            this.gridTransitionLevels.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.gridTransitionLevels.RowHeadersVisible = false;
            this.gridTransitionLevels.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridTransitionLevels.Size = new System.Drawing.Size(678, 297);
            this.gridTransitionLevels.TabIndex = 2;
            this.gridTransitionLevels.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridTransitionLevels_CellEndEdit);
            this.gridTransitionLevels.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.gridTransitionLevels_CellValidating);
            this.gridTransitionLevels.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridTransitionLevels_CellValueChanged);
            this.gridTransitionLevels.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.gridTransitionLevels_EditingControlShowing);
            // 
            // low
            // 
            this.low.HeaderText = "QNH Low";
            this.low.Name = "low";
            // 
            // high
            // 
            this.high.HeaderText = "QNH High";
            this.high.Name = "high";
            // 
            // transitionLevel
            // 
            this.transitionLevel.HeaderText = "Transition Level";
            this.transitionLevel.Name = "transitionLevel";
            // 
            // btnManageComposite
            // 
            this.btnManageComposite.AutoSize = true;
            this.btnManageComposite.ContextMenuStrip = this.ctxOptions;
            this.btnManageComposite.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnManageComposite.Location = new System.Drawing.Point(4, 399);
            this.btnManageComposite.Margin = new System.Windows.Forms.Padding(4, 3, 3, 3);
            this.btnManageComposite.Name = "btnManageComposite";
            this.btnManageComposite.Size = new System.Drawing.Size(201, 29);
            this.btnManageComposite.TabIndex = 2;
            this.btnManageComposite.Text = "Manage Composite...";
            this.btnManageComposite.UseVisualStyleBackColor = true;
            this.btnManageComposite.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnManageComposite_MouseClick);
            // 
            // ctxOptions
            // 
            this.ctxOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxNew,
            this.ctxCopy,
            this.ctxRename,
            this.ctxDelete,
            this.toolStripSeparator1,
            this.ctxImport,
            this.ctxExport});
            this.ctxOptions.Name = "ctxOptions";
            this.ctxOptions.Size = new System.Drawing.Size(120, 142);
            // 
            // ctxNew
            // 
            this.ctxNew.Name = "ctxNew";
            this.ctxNew.Size = new System.Drawing.Size(119, 22);
            this.ctxNew.Text = "New";
            this.ctxNew.Click += new System.EventHandler(this.ctxNew_Click);
            // 
            // ctxCopy
            // 
            this.ctxCopy.Enabled = false;
            this.ctxCopy.Name = "ctxCopy";
            this.ctxCopy.Size = new System.Drawing.Size(119, 22);
            this.ctxCopy.Text = "Copy";
            this.ctxCopy.Click += new System.EventHandler(this.ctxCopy_Click);
            // 
            // ctxRename
            // 
            this.ctxRename.Enabled = false;
            this.ctxRename.Name = "ctxRename";
            this.ctxRename.Size = new System.Drawing.Size(119, 22);
            this.ctxRename.Text = "Rename";
            this.ctxRename.Click += new System.EventHandler(this.ctxRename_Click);
            // 
            // ctxDelete
            // 
            this.ctxDelete.Enabled = false;
            this.ctxDelete.Name = "ctxDelete";
            this.ctxDelete.Size = new System.Drawing.Size(119, 22);
            this.ctxDelete.Text = "Delete";
            this.ctxDelete.Click += new System.EventHandler(this.ctxDelete_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(116, 6);
            // 
            // ctxImport
            // 
            this.ctxImport.Name = "ctxImport";
            this.ctxImport.Size = new System.Drawing.Size(119, 22);
            this.ctxImport.Text = "Import...";
            this.ctxImport.Click += new System.EventHandler(this.ctxImport_Click);
            // 
            // ctxExport
            // 
            this.ctxExport.Enabled = false;
            this.ctxExport.Name = "ctxExport";
            this.ctxExport.Size = new System.Drawing.Size(119, 22);
            this.ctxExport.Text = "Export...";
            this.ctxExport.Click += new System.EventHandler(this.ctxExport_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.panelCompositeList);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(202, 390);
            this.panel1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(202, 24);
            this.label1.TabIndex = 3;
            this.label1.Text = "Composites";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelCompositeList
            // 
            this.panelCompositeList.BackColor = System.Drawing.Color.White;
            this.panelCompositeList.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(217)))), ((int)(((byte)(217)))));
            this.panelCompositeList.Controls.Add(this.TreeMenu);
            this.panelCompositeList.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelCompositeList.Location = new System.Drawing.Point(0, 24);
            this.panelCompositeList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panelCompositeList.Name = "panelCompositeList";
            this.panelCompositeList.Padding = new System.Windows.Forms.Padding(6);
            this.panelCompositeList.ShowBorder = true;
            this.panelCompositeList.Size = new System.Drawing.Size(202, 366);
            this.panelCompositeList.TabIndex = 2;
            // 
            // TreeMenu
            // 
            this.TreeMenu.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TreeMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TreeMenu.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.TreeMenu.FullRowSelect = true;
            this.TreeMenu.HideSelection = false;
            this.TreeMenu.Indent = 5;
            this.TreeMenu.Location = new System.Drawing.Point(6, 6);
            this.TreeMenu.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.TreeMenu.Name = "TreeMenu";
            this.TreeMenu.ShowLines = false;
            this.TreeMenu.Size = new System.Drawing.Size(190, 354);
            this.TreeMenu.TabIndex = 0;
            this.TreeMenu.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.TreeMenu_DrawNode);
            this.TreeMenu.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeMenu_AfterSelect);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnCancel);
            this.flowLayoutPanel1.Controls.Add(this.btnApply);
            this.flowLayoutPanel1.Controls.Add(this.btnOK);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(208, 396);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(723, 35);
            this.flowLayoutPanel1.TabIndex = 4;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(632, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 29);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(556, 3);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(70, 29);
            this.btnApply.TabIndex = 1;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(480, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(70, 29);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // compositeTooltip
            // 
            this.compositeTooltip.AutomaticDelay = 250;
            this.compositeTooltip.AutoPopDelay = 10000;
            this.compositeTooltip.InitialDelay = 250;
            this.compositeTooltip.ReshowDelay = 50;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.button1, 0, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.Size = new System.Drawing.Size(200, 100);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(4, 3);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(192, 27);
            this.button1.TabIndex = 1;
            this.button1.Text = "Delete Selected Contraction";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(4, 3);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(192, 290);
            this.dataGridView1.TabIndex = 2;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "String";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Spoken";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // ProfileConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(964, 461);
            this.ControlBox = false;
            this.Controls.Add(this.TlpMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(954, 456);
            this.Name = "ProfileConfiguration";
            this.Padding = new System.Windows.Forms.Padding(15);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Profile Configuration";
            this.TopMost = true;
            this.TlpMain.ResumeLayout(false);
            this.TlpMain.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.mainTabControl.ResumeLayout(false);
            this.pagePresets.ResumeLayout(false);
            this.pagePresets.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.pageContractions.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridContractions)).EndInit();
            this.pageConfiguration.ResumeLayout(false);
            this.pageConfiguration.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.vhfFrequency)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.observationTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.magneticVar)).EndInit();
            this.pageTransitionLevel.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridTransitionLevels)).EndInit();
            this.ctxOptions.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panelCompositeList.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TlpMain;
        private System.Windows.Forms.TreeView TreeMenu;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnApply;
        private UI.HitTestPanel panelCompositeList;
        private System.Windows.Forms.Button btnManageComposite;
        private System.Windows.Forms.ContextMenuStrip ctxOptions;
        private System.Windows.Forms.ToolStripMenuItem ctxNew;
        private System.Windows.Forms.ToolStripMenuItem ctxRename;
        private System.Windows.Forms.ToolStripMenuItem ctxDelete;
        private System.Windows.Forms.ToolStripMenuItem ctxCopy;
        private System.Windows.Forms.ToolTip compositeTooltip;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem ctxImport;
        private System.Windows.Forms.ToolStripMenuItem ctxExport;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.TabPage pagePresets;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox txtAtisTemplate;
        private System.Windows.Forms.Button btnRenamePreset;
        private System.Windows.Forms.Button btnDeletePreset;
        private System.Windows.Forms.Button btnCopyPreset;
        private System.Windows.Forms.Button btnNewPreset;
        private System.Windows.Forms.ComboBox ddlPresets;
        private System.Windows.Forms.TabPage pageContractions;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button btnDeleteContraction;
        private System.Windows.Forms.DataGridView gridContractions;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnFind;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnReplace;
        private System.Windows.Forms.TabPage pageConfiguration;
        private System.Windows.Forms.NumericUpDown vhfFrequency;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown observationTime;
        private System.Windows.Forms.CheckBox chkObservationTime;
        private System.Windows.Forms.RadioButton radioVoiceRecorded;
        private System.Windows.Forms.RadioButton radioTextToSpeech;
        private System.Windows.Forms.ComboBox ddlVoices;
        private System.Windows.Forms.TextBox txtIdsEndpoint;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkMagneticVar;
        private System.Windows.Forms.NumericUpDown magneticVar;
        private System.Windows.Forms.TabPage pageTransitionLevel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Button btnDeleteTransitionLevel;
        private System.Windows.Forms.DataGridView gridTransitionLevels;
        private System.Windows.Forms.DataGridViewTextBoxColumn low;
        private System.Windows.Forms.DataGridViewTextBoxColumn high;
        private System.Windows.Forms.DataGridViewTextBoxColumn transitionLevel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label5;
    }
}