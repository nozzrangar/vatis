using Appccelerate.EventBroker;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Serialization;
using Vatsim.Vatis.Client.Args;
using Vatsim.Vatis.Client.Common;
using Vatsim.Vatis.Client.Config;
using Vatsim.Vatis.Client.Core;

namespace Vatsim.Vatis.Client
{
    internal partial class ProfileConfiguration : Form
    {
        private readonly IAppConfig mAppConfig;
        private readonly IUserInterface mUserInterface;
        private readonly IEventBroker mEventBroker;
        private readonly INavaidDatabase mNavaidDatabase;
        private string mPreviousInputValue = "";
        private AtisComposite mCurrentComposite = null;
        private AtisPreset mCurrentPreset = null;
        private bool mFrequencyChanged = false;
        private bool mContractionsChanged = false;
        private bool mTransitionLevelsChanged = false;

        [EventPublication(EventTopics.RefreshAtisComposites)]
        public event EventHandler RefreshAtisComposites;

        [EventPublication(EventTopics.AtisCompositeDeleted)]
        public event EventHandler<AtisCompositeDeletedEventArgs> AtisCompositeDeleted;

        public ProfileConfiguration(IEventBroker eventBroker, IUserInterface userInterface, IAppConfig appConfig, INavaidDatabase navaidDatabase)
        {
            InitializeComponent();

            mAppConfig = appConfig;
            mUserInterface = userInterface;
            mNavaidDatabase = navaidDatabase;
            mEventBroker = eventBroker;
            mEventBroker.Register(this);

            vhfFrequency.TextChanged += vhfFrequency_ValueChanged;
            observationTime.TextChanged += observationTime_ValueChanged;
            magneticVar.TextChanged += magneticVar_ValueChanged;

            RefreshCompositeList();

            btnApply.Enabled = false;
        }

        private TreeNode CreateTreeMenuNode(string name, string tag)
        {
            return new TreeNode
            {
                Tag = tag,
                Text = name,
            };
        }

        private void btnManageComposite_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ctxOptions.Show(Cursor.Position.X, Cursor.Position.Y);
            }
        }

        private void TreeMenu_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (TreeMenu.SelectedNode != null)
            {
                mainTabControl.Enabled = true;
                ctxDelete.Enabled = true;
                ctxRename.Enabled = true;
                ctxCopy.Enabled = true;
                ctxExport.Enabled = true;
                txtAtisTemplate.Text = "";
                Text = $"Profile Configuration: {TreeMenu.SelectedNode.Text}";
                if (mAppConfig.CurrentProfile != null)
                {
                    mCurrentComposite = mAppConfig.CurrentProfile.Composites.FirstOrDefault(x => x.Identifier == (string)TreeMenu.SelectedNode.Tag);
                    LoadComposite();
                    RefreshPresetList();
                }
            }
        }

        private void LoadComposite()
        {
            vhfFrequency.Value = (decimal)((mCurrentComposite.AtisFrequency + 100000) / 1000.0);

            if (mCurrentComposite.ObservationTime != null)
            {
                chkObservationTime.Checked = mCurrentComposite.ObservationTime.Enabled;
                observationTime.Value = mCurrentComposite.ObservationTime.Time;
                observationTime.Enabled = chkObservationTime.Checked;
            }

            if (mCurrentComposite.MagneticVariation != null)
            {
                chkMagneticVar.Checked = mCurrentComposite.MagneticVariation.Enabled;
                magneticVar.Value = mCurrentComposite.MagneticVariation.MagneticDegrees;
                magneticVar.Enabled = chkMagneticVar.Checked;
            }

            if (mCurrentComposite.AtisVoice != null)
            {
                radioTextToSpeech.Checked = mCurrentComposite.AtisVoice.UseTextToSpeech == true;
                radioVoiceRecorded.Checked = mCurrentComposite.AtisVoice.UseTextToSpeech == false;

                if (radioTextToSpeech.Checked)
                {
                    ddlVoices.Enabled = true;

                    if (!ddlVoices.Items.Contains(mCurrentComposite.AtisVoice.Voice))
                    {
                        ddlVoices.SelectedItem = "Default";
                    }
                    else
                    {
                        ddlVoices.SelectedItem = mCurrentComposite.AtisVoice.Voice;
                    }

                    var meta = new AtisVoiceMeta
                    {
                        UseTextToSpeech = true,
                        Voice = ddlVoices.SelectedItem.ToString()
                    };
                    mCurrentComposite.AtisVoice = meta;
                    mAppConfig.SaveConfig();
                }
                else
                {
                    ddlVoices.Enabled = false;
                }
            }
            else
            {
                radioTextToSpeech.Checked = true;
                ddlVoices.SelectedItem = "Default";
                var meta = new AtisVoiceMeta
                {
                    UseTextToSpeech = true,
                    Voice = "Default"
                };
                mCurrentComposite.AtisVoice = meta;
                mAppConfig.SaveConfig();
            }

            txtIdsEndpoint.Text = mCurrentComposite.IDSEndpoint;

            gridContractions.Rows.Clear();
            foreach (var contraction in mCurrentComposite.Contractions)
            {
                gridContractions.Rows.Add(new object[]
                {
                    contraction.String,
                    contraction.Spoken
                });
            }

            gridTransitionLevels.Rows.Clear();
            foreach (var tl in mCurrentComposite.TransitionLevels.OrderByDescending(t => t.Low).ThenByDescending(t => t.High))
            {
                gridTransitionLevels.Rows.Add(new object[]
                {
                    tl.Low,
                    tl.High,
                    tl.Altitude
                });
            }

            if (mCurrentComposite.Identifier.StartsWith("K") || mCurrentComposite.Identifier.StartsWith("P"))
            {
                pageTransitionLevel.SetVisible(false);
            }
            else
            {
                pageTransitionLevel.SetVisible(true);
            }
        }

        private void TreeMenu_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if (e.Node == null)
                return;

            var selected = (e.State & TreeNodeStates.Selected) == TreeNodeStates.Selected;
            var unfocused = !e.Node.TreeView.Focused;

            if (selected && unfocused)
            {
                var font = e.Node.NodeFont ?? e.Node.TreeView.Font;
                // override bounds to paint full width
                var bounds = new Rectangle(e.Bounds.X - 6, e.Bounds.Y, TreeMenu.Width, e.Bounds.Height);
                e.Graphics.FillRectangle(SystemBrushes.Highlight, bounds);
                TextRenderer.DrawText(e.Graphics, e.Node.Text, font, e.Bounds, SystemColors.HighlightText, TextFormatFlags.GlyphOverhangPadding);
            }
            else
            {
                e.DrawDefault = true;
            }
        }

        private void ctxNew_Click(object sender, EventArgs e)
        {
            if (mAppConfig.CurrentProfile.Composites.Count >= Constants.MAX_ALLOWED_COMPOSITES)
            {
                MessageBox.Show("The maximum ATIS composite count has been exceeded for this profile.", "Error");
                return;
            }

            bool flag = false;

            while (!flag)
            {
                using (var dlg = mUserInterface.CreateUserInputForm())
                {
                    mPreviousInputValue = "";
                    dlg.PromptLabel = "Enter the facility ICAO identifier (e.g. KLAX)";
                    dlg.WindowTitle = "New ATIS Composite";
                    dlg.ErrorMessage = "Enter a valid facility identifier code that contains only letters or numbers.";
                    dlg.TextUppercase = true;
                    dlg.MaxLength = 4;
                    dlg.RegexExpression = "^([A-Z]\\d\\d|[A-Z]{4})$";
                    dlg.InitialValue = mPreviousInputValue;

                    DialogResult result = dlg.ShowDialog(this);
                    if (result == DialogResult.OK && !string.IsNullOrEmpty(dlg.Value))
                    {
                        string identifier = dlg.Value;
                        mPreviousInputValue = dlg.Value;

                        if (mNavaidDatabase.GetAirport(identifier) == null)
                        {
                            if (MessageBox.Show(this, $"ICAO identifier not found: {identifier}", "Invalid Identifier", MessageBoxButtons.OK, MessageBoxIcon.Hand) == DialogResult.OK)
                            {
                                return;
                            }
                        }

                        if (mAppConfig.CurrentProfile != null && mAppConfig.CurrentProfile.Composites.Any(x => x.Identifier == dlg.Value))
                        {
                            if (MessageBox.Show(this, "A composite with that identifier already exists. Would you like to overwrite it?", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                return;
                            }

                            flag = true;
                        }
                        else
                        {
                            while (!flag)
                            {
                                using (var dlg2 = mUserInterface.CreateUserInputForm())
                                {
                                    mPreviousInputValue = "";
                                    dlg2.PromptLabel = "Enter a name for the ATIS Composite:";
                                    dlg2.WindowTitle = "New ATIS Composite";
                                    dlg2.ErrorMessage = "Invalid composite name. It must consist of only letters, numbers, underscores and spaces.";
                                    dlg2.RegexExpression = "^[a-zA-Z0-9_ ]*$";
                                    dlg2.InitialValue = mPreviousInputValue;

                                    DialogResult result2 = dlg2.ShowDialog(this);
                                    if (result2 == DialogResult.OK && !string.IsNullOrEmpty(dlg2.Value))
                                    {
                                        mPreviousInputValue = dlg2.Value;
                                        var composite = new AtisComposite
                                        {
                                            Identifier = identifier,
                                            Name = dlg2.Value
                                        };
                                        mAppConfig.CurrentProfile.Composites.Add(composite);
                                        mAppConfig.SaveConfig();
                                        RefreshCompositeList();
                                        flag = true;
                                    }
                                    else
                                    {
                                        flag = true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        flag = true;
                    }
                }
            }
        }

        private void ctxCopy_Click(object sender, EventArgs e)
        {
            if (mAppConfig.CurrentProfile.Composites.Count >= Constants.MAX_ALLOWED_COMPOSITES)
            {
                MessageBox.Show("The maximum ATIS composite count has been exceeded for this profile.", "Error");
                return;
            }

            if (TreeMenu.SelectedNode != null)
            {
                var composite = mAppConfig.CurrentProfile.Composites.FirstOrDefault(x => x.Identifier == TreeMenu.SelectedNode.Tag.ToString());

                if (composite != null)
                {
                    bool flag = false;

                    while (!flag)
                    {
                        using (var dlg = mUserInterface.CreateUserInputForm())
                        {
                            mPreviousInputValue = "";
                            dlg.PromptLabel = "Enter the facility ICAO identifier (e.g. KLAX)";
                            dlg.WindowTitle = "New ATIS Composite";
                            dlg.ErrorMessage = "Enter a valid facility identifier code that contains only letters or numbers.";
                            dlg.TextUppercase = true;
                            dlg.MaxLength = 4;
                            dlg.RegexExpression = "^([A-Z]\\d\\d|[A-Z]{4})$";
                            dlg.InitialValue = mPreviousInputValue;

                            DialogResult result = dlg.ShowDialog(this);
                            if (result == DialogResult.OK && !string.IsNullOrEmpty(dlg.Value))
                            {
                                string identifier = dlg.Value;
                                mPreviousInputValue = dlg.Value;

                                if (mAppConfig.CurrentProfile != null && mAppConfig.CurrentProfile.Composites.Any(x => x.Identifier == dlg.Value))
                                {
                                    MessageBox.Show("Another composite already exists with that facility identifier. Please choose a new facility identifier.", "Duplicate Facility Identifier");
                                }
                                else
                                {
                                    while (!flag)
                                    {
                                        using (var dlg2 = mUserInterface.CreateUserInputForm())
                                        {
                                            mPreviousInputValue = "";
                                            dlg2.PromptLabel = "Enter a name for the ATIS Composite:";
                                            dlg2.WindowTitle = "New ATIS Composite";
                                            dlg2.ErrorMessage = "Invalid composite name. It must consist of only letters, numbers, underscores and spaces.";
                                            dlg2.RegexExpression = "^[a-zA-Z0-9_ ]*$";
                                            dlg2.InitialValue = mPreviousInputValue;

                                            DialogResult result2 = dlg2.ShowDialog(this);
                                            if (result2 == DialogResult.OK && !string.IsNullOrEmpty(dlg2.Value))
                                            {
                                                mPreviousInputValue = dlg2.Value;
                                                var clone = composite.Clone();
                                                clone.Identifier = identifier;
                                                clone.Name = dlg2.Value;
                                                mAppConfig.CurrentProfile.Composites.Add(clone);
                                                mAppConfig.SaveConfig();
                                                RefreshCompositeList();
                                                flag = true;
                                            }
                                            else
                                            {
                                                flag = true;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                flag = true;
                            }
                        }
                    }
                }
            }
        }

        private void ctxRename_Click(object sender, EventArgs e)
        {
            bool flag = false;

            if (TreeMenu.SelectedNode != null)
            {
                var composite = mAppConfig.CurrentProfile.Composites.FirstOrDefault(x => x.Identifier == TreeMenu.SelectedNode.Tag.ToString());
                if (composite != null)
                {
                    while (!flag)
                    {
                        using (var dlg = mUserInterface.CreateUserInputForm())
                        {
                            mPreviousInputValue = composite.Name;
                            dlg.PromptLabel = "Enter a new name for the ATIS Composite:";
                            dlg.WindowTitle = "Rename ATIS Composite";
                            dlg.ErrorMessage = "Invalid composite name. It must consist of only letters, numbers, underscores and spaces.";
                            dlg.RegexExpression = "^[a-zA-Z0-9_ ]*$";
                            dlg.InitialValue = mPreviousInputValue;

                            DialogResult result2 = dlg.ShowDialog(this);
                            if (result2 == DialogResult.OK && !string.IsNullOrEmpty(dlg.Value))
                            {
                                mPreviousInputValue = dlg.Value;

                                composite.Name = dlg.Value;
                                mAppConfig.SaveConfig();
                                RefreshCompositeList();
                                flag = true;
                            }
                            else
                            {
                                flag = true;
                            }
                        }
                    }
                }
            }
        }

        private void ctxDelete_Click(object sender, EventArgs e)
        {
            if (TreeMenu.SelectedNode != null)
            {
                if (MessageBox.Show(this, string.Format($"Are you sure you want to delete the selected ATIS Composite? This action will also delete all associated ATIS presets.\r\n\r\n{TreeMenu.SelectedNode.Text}"), "Delete ATIS Composite", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    AtisCompositeDeleted?.Invoke(this, new AtisCompositeDeletedEventArgs(TreeMenu.SelectedNode.Tag.ToString()));

                    mAppConfig.CurrentProfile.Composites.RemoveAll(x => x.Identifier == TreeMenu.SelectedNode.Tag.ToString());
                    mAppConfig.SaveConfig();
                    RefreshCompositeList();
                }
            }
        }

        private void RefreshCompositeList()
        {
            mainTabControl.Enabled = false;
            ctxDelete.Enabled = false;
            ctxRename.Enabled = false;
            ctxCopy.Enabled = false;
            ctxExport.Enabled = false;

            vhfFrequency.Value = 118.000m;
            observationTime.Value = 0;
            magneticVar.Value = 0;
            chkMagneticVar.Checked = false;
            chkObservationTime.Checked = false;
            txtIdsEndpoint.Text = "";

            pageTransitionLevel.SetVisible(false);

            TreeMenu.Nodes.Clear();
            if (mAppConfig.CurrentProfile != null)
            {
                foreach (var composite in mAppConfig.CurrentProfile.Composites.OrderBy(x => x.Identifier))
                {
                    var node = CreateTreeMenuNode($"{composite.Name} ({composite.Identifier})", composite.Identifier);

                    TreeMenu.Nodes.Add(node);

                    if (mAppConfig.CurrentComposite == composite)
                    {
                        TreeMenu.SelectedNode = node;
                    }
                }
            }

            RefreshPresetList();
            RefreshAtisComposites?.Invoke(this, EventArgs.Empty);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (btnApply.Enabled)
            {
                if (MessageBox.Show(this, "You have unsaved changes. Are you sure you want to cancel?", "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    Close();
                }
            }
            else
            {
                Close();
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            ApplyChanges();
        }

        private bool ApplyChanges()
        {
            if (!string.IsNullOrEmpty(txtIdsEndpoint.Text) && !IsValidUrl(txtIdsEndpoint.Text))
            {
                MessageBox.Show("IDS endpoint URL is not a valid hyperlink format.");
                return false;
            }

            if (mFrequencyChanged)
            {
                mCurrentComposite.AtisFrequency = (int)((vhfFrequency.Value - 100m) * 1000m);
                mFrequencyChanged = false;
            }

            if (mCurrentPreset != null && mCurrentPreset.IsTemplateDirty)
            {
                mCurrentPreset.Template = txtAtisTemplate.Text;
            }

            List<string> usedContractions = new List<string>();
            foreach (DataGridViewRow row in gridContractions.Rows)
            {
                if (!row.IsNewRow)
                {
                    try
                    {
                        var stringValue = row.Cells[0].Value.ToString();
                        if (usedContractions.Contains(stringValue))
                        {
                            MessageBox.Show(this, "Duplicate contraction: " + stringValue, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                            gridContractions.Focus();
                            return false;
                        }
                        usedContractions.Add(stringValue);
                    }
                    catch { }
                }
            }

            if (mContractionsChanged)
            {
                mCurrentComposite.Contractions.Clear();
                foreach (DataGridViewRow row in gridContractions.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        if (row.Cells[0].Value != null && row.Cells[1].Value != null)
                        {
                            var stringValue = row.Cells[0].Value.ToString();
                            var spokenValue = row.Cells[1].Value.ToString();
                            if (!string.IsNullOrEmpty(stringValue) && !string.IsNullOrEmpty(spokenValue))
                            {
                                mCurrentComposite.Contractions.Add(new ContractionMeta
                                {
                                    String = stringValue,
                                    Spoken = spokenValue
                                });
                            }
                        }
                    }
                }
            }

            List<Tuple<int, int>> usedTransitionLevels = new List<Tuple<int, int>>();
            foreach (DataGridViewRow row in gridTransitionLevels.Rows)
            {
                if (!row.IsNewRow)
                {
                    try
                    {
                        var low = int.Parse(row.Cells[0].Value.ToString());
                        var high = int.Parse(row.Cells[1].Value.ToString());
                        if (usedTransitionLevels.Any(t => t.Item1 == low && t.Item2 == high))
                        {
                            MessageBox.Show(this, $"Duplicate Transition Level: {low}-{high}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                            gridTransitionLevels.Focus();
                            return false;
                        }
                        usedTransitionLevels.Add(new Tuple<int, int>(low, high));
                    }
                    catch { }
                }
            }

            if (mTransitionLevelsChanged)
            {
                mCurrentComposite.TransitionLevels.Clear();
                foreach (DataGridViewRow row in gridTransitionLevels.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        if (row.Cells[0].Value != null && row.Cells[1].Value != null && row.Cells[2].Value != null)
                        {
                            var low = int.Parse(row.Cells[0].Value.ToString());
                            var high = int.Parse(row.Cells[1].Value.ToString());
                            var tl = int.Parse(row.Cells[2].Value.ToString());
                            mCurrentComposite.TransitionLevels.Add(new TransitionLevel
                            {
                                Low = low,
                                High = high,
                                Altitude = tl
                            });
                        }
                    }
                }
            }

            mTransitionLevelsChanged = false;
            mContractionsChanged = false;
            btnApply.Enabled = false;
            btnOK.Enabled = true;

            mAppConfig.SaveConfig();

            return true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!btnApply.Enabled || ApplyChanges())
            {
                RefreshAtisComposites?.Invoke(this, EventArgs.Empty);
                Close();
            }
        }

        private void vhfFrequency_ValueChanged(object sender, EventArgs e)
        {
            if (!vhfFrequency.Focused)
                return;

            mFrequencyChanged = true;
            btnApply.Enabled = true;
        }

        private void chkObservationTime_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkObservationTime.Focused)
                return;

            observationTime.Enabled = chkObservationTime.Checked;

            var meta = new ObservationTimeMeta
            {
                Enabled = observationTime.Enabled,
                Time = (uint)observationTime.Value
            };
            mCurrentComposite.ObservationTime = meta;

            btnApply.Enabled = true;
        }

        private void observationTime_ValueChanged(object sender, EventArgs e)
        {
            if (!observationTime.Focused)
                return;

            var meta = new ObservationTimeMeta
            {
                Enabled = observationTime.Enabled,
                Time = (uint)observationTime.Value
            };
            mCurrentComposite.ObservationTime = meta;

            btnApply.Enabled = true;
        }

        private void chkMagneticVar_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkMagneticVar.Focused)
                return;

            magneticVar.Enabled = chkMagneticVar.Checked;

            var meta = new MagneticVariationMeta
            {
                Enabled = chkMagneticVar.Enabled,
                MagneticDegrees = (int)magneticVar.Value
            };
            mCurrentComposite.MagneticVariation = meta;

            btnApply.Enabled = true;
        }

        private void magneticVar_ValueChanged(object sender, EventArgs e)
        {
            if (!magneticVar.Focused)
                return;

            var meta = new MagneticVariationMeta
            {
                Enabled = chkMagneticVar.Enabled,
                MagneticDegrees = (int)magneticVar.Value
            };
            mCurrentComposite.MagneticVariation = meta;

            btnApply.Enabled = true;
        }

        private void radioVoiceRecorded_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioVoiceRecorded.Focused)
                return;

            if (mCurrentComposite.AtisVoice != null)
            {
                mCurrentComposite.AtisVoice.UseTextToSpeech = false;
            }
            else
            {
                mCurrentComposite.AtisVoice = new AtisVoiceMeta
                {
                    UseTextToSpeech = false
                };
            }
            ddlVoices.Enabled = false;
            btnApply.Enabled = true;
        }

        private void radioTextToSpeech_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioTextToSpeech.Focused)
                return;

            if (mCurrentComposite.AtisVoice != null)
            {
                mCurrentComposite.AtisVoice.UseTextToSpeech = true;
                if (mCurrentComposite.AtisVoice.Voice != null)
                {
                    if (ddlVoices.Items.Contains(mCurrentComposite.AtisVoice.Voice))
                    {
                        ddlVoices.SelectedItem = mCurrentComposite.AtisVoice.Voice;
                    }
                    else
                    {
                        ddlVoices.SelectedItem = "Default";
                    }
                }
                else
                {
                    ddlVoices.SelectedItem = "Default";
                }
            }
            else
            {
                mCurrentComposite.AtisVoice = new AtisVoiceMeta
                {
                    UseTextToSpeech = true
                };
            }
            ddlVoices.Enabled = true;
            btnApply.Enabled = true;
        }

        private void ddlVoices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!ddlVoices.Focused)
                return;

            if (mCurrentComposite.AtisVoice != null)
            {
                mCurrentComposite.AtisVoice.Voice = ddlVoices.SelectedItem.ToString();
            }
            else
            {
                mCurrentComposite.AtisVoice = new AtisVoiceMeta
                {
                    UseTextToSpeech = true,
                    Voice = ddlVoices.SelectedItem.ToString()
                };
            }

            btnApply.Enabled = true;
        }

        private void txtIdsEndpoint_TextChanged(object sender, EventArgs e)
        {
            if (!txtIdsEndpoint.Focused)
                return;

            mCurrentComposite.IDSEndpoint = txtIdsEndpoint.Text;

            btnApply.Enabled = true;
        }

        public bool IsValidUrl(string value)
        {
            var pattern = new Regex(@"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$");
            return pattern.IsMatch(value.Trim());
        }

        private void btnNewPreset_Click(object sender, EventArgs e)
        {
            bool flag = false;

            while (!flag)
            {
                using (var dlg = mUserInterface.CreateUserInputForm())
                {
                    mPreviousInputValue = "";
                    dlg.PromptLabel = "Enter a name for the preset";
                    dlg.WindowTitle = "New Preset";
                    dlg.InitialValue = mPreviousInputValue;
                    dlg.TextUppercase = true;

                    DialogResult result = dlg.ShowDialog(this);
                    if (result == DialogResult.OK && !string.IsNullOrEmpty(dlg.Value))
                    {
                        mPreviousInputValue = dlg.Value;

                        if (mCurrentComposite.Presets.Any(x => x.Name == dlg.Value))
                        {
                            MessageBox.Show(this, "Another profile already exists with that name. Please choose another.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }

                        var preset = new AtisPreset
                        {
                            Name = dlg.Value,
                            Template = "[FACILITY] ATIS INFO [ATIS_CODE] [OBS_TIME]. [FULL_WX_STRING]. [ARPT_COND] [NOTAMS]. [ADVISE_ON_INIT_CONTACT]."
                        };
                        mCurrentComposite.Presets.Add(preset);
                        mAppConfig.SaveConfig();

                        RefreshPresetList(preset.Name);
                        mPreviousInputValue = "";

                        flag = true;
                    }
                    else
                    {
                        flag = true;
                    }
                }
            }
        }

        private void btnRenamePreset_Click(object sender, EventArgs e)
        {
            bool flag = false;

            mPreviousInputValue = mCurrentPreset.Name;

            while (!flag)
            {
                using (var dlg = mUserInterface.CreateUserInputForm())
                {
                    dlg.PromptLabel = "Enter a new name for the preset";
                    dlg.WindowTitle = "Rename Preset";
                    dlg.InitialValue = mPreviousInputValue;
                    dlg.TextUppercase = true;

                    DialogResult result = dlg.ShowDialog(this);
                    if (result == DialogResult.OK && !string.IsNullOrEmpty(dlg.Value))
                    {
                        mPreviousInputValue = dlg.Value;

                        if (mCurrentComposite.Presets.Any(x => x.Name == dlg.Value))
                        {
                            MessageBox.Show(this, "Another profile already exists with that name. Please choose another.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            flag = false;
                        }
                        else
                        {
                            mCurrentPreset.Name = dlg.Value;
                            mAppConfig.SaveConfig();

                            RefreshPresetList();
                            mPreviousInputValue = "";

                            flag = true;
                        }
                    }
                    else
                    {
                        flag = true;
                    }
                }
            }
        }

        private void btnCopyPreset_Click(object sender, EventArgs e)
        {
            bool flag = false;

            while (!flag)
            {
                using (var dlg = mUserInterface.CreateUserInputForm())
                {
                    dlg.PromptLabel = "Enter a name for the preset";
                    dlg.WindowTitle = "Copy Preset";
                    dlg.InitialValue = mPreviousInputValue;
                    dlg.TextUppercase = true;

                    DialogResult result = dlg.ShowDialog(this);
                    if (result == DialogResult.OK && !string.IsNullOrEmpty(dlg.Value))
                    {
                        mPreviousInputValue = dlg.Value;

                        if (mCurrentComposite.Presets.Any(x => x.Name == dlg.Value))
                        {
                            MessageBox.Show(this, "Another profile already exists with that name. Please choose another.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        else
                        {
                            var clone = mCurrentPreset.Clone();
                            clone.Name = dlg.Value;
                            mCurrentComposite.Presets.Add(clone);
                            mAppConfig.SaveConfig();

                            RefreshPresetList();
                            mPreviousInputValue = "";

                            flag = true;
                        }
                    }
                    else
                    {
                        flag = true;
                    }
                }
            }
        }

        private void btnDeletePreset_Click(object sender, EventArgs e)
        {
            if (ddlPresets.SelectedItem != null)
            {
                if (MessageBox.Show(this, "Are you sure you want to delete the selected preset?", "Delete Preset", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    mCurrentComposite.Presets.RemoveAll(x => x.Name == ddlPresets.SelectedItem.ToString());
                    mAppConfig.SaveConfig();
                    txtAtisTemplate.Text = "";
                    txtAtisTemplate.Enabled = false;
                    RefreshPresetList();
                }
            }
        }

        private void RefreshPresetList(string selectNewPreset = "")
        {
            if (mCurrentComposite != null)
            {
                ddlPresets.Items.Clear();

                foreach (var preset in mCurrentComposite.Presets)
                {
                    ddlPresets.Items.Add(preset.Name);
                }

                if (!string.IsNullOrEmpty(selectNewPreset))
                {
                    ddlPresets.SelectedItem = selectNewPreset;
                }
                else if (mCurrentComposite.CurrentPreset != null)
                {
                    ddlPresets.SelectedItem = mCurrentComposite.CurrentPreset.Name;
                }
                else
                {
                    ddlPresets.SelectedIndex = -1;
                }
            }

            btnCopyPreset.Enabled = ddlPresets.SelectedItem != null;
            btnDeletePreset.Enabled = ddlPresets.SelectedItem != null;
            btnRenamePreset.Enabled = ddlPresets.SelectedItem != null;

            RefreshAtisComposites?.Invoke(this, EventArgs.Empty);
        }

        private void ddlPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnCopyPreset.Enabled = ddlPresets.SelectedItem != null;
            btnDeletePreset.Enabled = ddlPresets.SelectedItem != null;
            btnRenamePreset.Enabled = ddlPresets.SelectedItem != null;
            txtAtisTemplate.Enabled = ddlPresets.SelectedItem != null;

            if (ddlPresets.SelectedItem != null)
            {
                mCurrentPreset = mCurrentComposite.Presets.FirstOrDefault(x => x.Name == ddlPresets.SelectedItem.ToString());

                if (mCurrentPreset != null)
                    txtAtisTemplate.Text = mCurrentPreset.Template;
            }
        }

        private void txtTemplate_TextChanged(object sender, EventArgs e)
        {
            if (!txtAtisTemplate.Focused)
                return;

            if (mCurrentPreset != null)
            {
                if (mCurrentPreset.Template != txtAtisTemplate.Text)
                {
                    btnApply.Enabled = true;
                    mCurrentPreset.IsTemplateDirty = true;
                }
            }
        }

        private void gridContractions_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (base.ActiveControl == btnCancel)
            {
                e.Cancel = true;
                btnCancel_Click(null, EventArgs.Empty);
                return;
            }
        }

        private void gridContractions_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            TextBox textBox = e.Control as TextBox;
            if (textBox != null)
            {
                textBox.CharacterCasing = CharacterCasing.Upper;
            }
        }

        private void btnDeleteContraction_Click(object sender, EventArgs e)
        {
            if (gridContractions.SelectedRows.Count == 1)
            {
                if (!gridContractions.SelectedRows[0].IsNewRow && MessageBox.Show(this, "Are you sure you want to delete the selected contraction?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    gridContractions.Rows.RemoveAt(gridContractions.SelectedRows[0].Index);
                    btnApply.Enabled = true;
                    mContractionsChanged = true;
                }
            }
            else
            {
                MessageBox.Show(this, "No contraction selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void gridContractions_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            gridContractions.Rows[e.RowIndex].ErrorText = "";
        }

        private void gridContractions_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            btnApply.Enabled = true;
            mContractionsChanged = true;
        }

        private void gridContractions_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure you want to delete the selected contraction?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void gridContractions_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            btnApply.Enabled = true;
            mContractionsChanged = true;
        }

        private void ctxImport_Click(object sender, EventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Title = "Import vATIS Composite",
                    CheckFileExists = true,
                    CheckPathExists = true,
                    AddExtension = false,
                    Multiselect = true,
                    Filter = "Legacy vATIS Profile (*.gz)|*.gz|vATIS Composite (*.json)|*.json|All Files (*.*)|*.*",
                    FilterIndex = 1,
                    DefaultExt = "gz",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                    ShowHelp = false
                };
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    foreach (var file in dialog.FileNames.Take(Constants.MAX_ALLOWED_COMPOSITES))
                    {
                        var fileInfo = new FileInfo(file);
                        switch (fileInfo.Extension)
                        {
                            case ".gz":
                                ImportLegacyProfile(fileInfo.FullName);
                                break;
                            case ".json":
                                ImportComposite(fileInfo.FullName);
                                break;
                            default:
                                throw new Exception("Unknown file type");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Composite Import Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void ImportComposite(string fullName)
        {
            try
            {
                var composite = new AtisComposite();

                using (var fs = new FileStream(fullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var sr = new StreamReader(fs))
                    {
                        JsonConvert.PopulateObject(sr.ReadToEnd(), composite);
                    }
                }

                if (mAppConfig.CurrentProfile != null)
                {
                    if (mAppConfig.CurrentProfile.Composites.Any(t => t.Identifier == composite.Identifier))
                    {
                        if (MessageBox.Show(this, $"A composite already exists for {composite.Identifier}. Do you want to overwrite it?", "Duplicate Composite", MessageBoxButtons.YesNo, MessageBoxIcon.Hand) == DialogResult.Yes)
                        {
                            mAppConfig.CurrentProfile.Composites.RemoveAll(t => t.Identifier == composite.Identifier);
                            mAppConfig.CurrentProfile.Composites.Add(composite);
                            mAppConfig.SaveConfig();
                            RefreshCompositeList();
                            LoadComposite();
                        }
                    }
                    else
                    {
                        mAppConfig.CurrentProfile.Composites.Add(composite);
                        mAppConfig.SaveConfig();
                        RefreshCompositeList();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Import Error: " + ex.Message, "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void ImportLegacyProfile(string fullName)
        {
            FileStream fs = new FileStream(fullName, FileMode.Open, FileAccess.Read);
            using (Stream stream = new GZipStream(fs, CompressionMode.Decompress))
            {
                XmlSerializer xml = new XmlSerializer(typeof(LegacyFacility));
                var profile = (LegacyFacility)xml.Deserialize(stream);

                if (mAppConfig.CurrentProfile != null)
                {
                    if (mAppConfig.CurrentProfile.Composites.Any(t => t.Identifier == profile.ID))
                    {
                        if (MessageBox.Show(this, "A composite with that identifier already exists. Would you like to overwrite it?", "Duplicate Composite", MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            return;
                        }

                        var existing = mAppConfig.CurrentProfile.Composites.FirstOrDefault(t => t.Identifier == profile.ID);
                        if (existing != null)
                        {
                            existing.Name = profile.Name;
                            existing.Identifier = profile.ID;
                            if (!string.IsNullOrEmpty(profile.AtisFrequency))
                            {
                                existing.AtisFrequency = (int)(double.Parse(profile.AtisFrequency) * 1000) - 100000;
                            }
                            else
                            {
                                existing.AtisFrequency = profile.Frequency;
                            }
                            existing.IDSEndpoint = profile.InformationDisplaySystemEndpoint;
                            existing.AtisVoice.UseTextToSpeech = !profile.VoiceRecordEnabled;

                            if (profile.MagneticVariation != null)
                            {
                                if (profile.MagneticVariation.Option != LegacyMagneticVariation.LegacyMagneticVariationOption.None)
                                {
                                    existing.MagneticVariation.Enabled = true;
                                }
                                switch (profile.MagneticVariation.Option)
                                {
                                    case LegacyMagneticVariation.LegacyMagneticVariationOption.Add:
                                        existing.MagneticVariation.MagneticDegrees =
                                            Math.Abs(profile.MagneticVariation.MagneticVariationValue);
                                        break;
                                    case LegacyMagneticVariation.LegacyMagneticVariationOption.Subtract:
                                        existing.MagneticVariation.MagneticDegrees =
                                            profile.MagneticVariation.MagneticVariationValue * -1;
                                        break;
                                }
                            }

                            if (profile.MetarObservation != null)
                            {
                                existing.ObservationTime.Enabled = profile.MetarObservation.Enable;
                                existing.ObservationTime.Time = (uint)profile.MetarObservation.ObservationTimeValue;
                            }

                            existing.Contractions.Clear();
                            foreach (var contraction in profile.Contractions)
                            {
                                existing.Contractions.Add(new ContractionMeta
                                {
                                    String = contraction.Key,
                                    Spoken = contraction.Value
                                });
                            }

                            int idx = 1;
                            existing.AirportConditionDefinitions.Clear();
                            foreach (var condition in profile.AirportConditions)
                            {
                                existing.AirportConditionDefinitions.Add(new DefinedText
                                {
                                    Ordinal = idx++,
                                    Text = condition.Message,
                                    Enabled = condition.IsSelected
                                });
                            }

                            idx = 1;
                            existing.NotamDefinitions.Clear();
                            foreach (var condition in profile.Notams)
                            {
                                existing.NotamDefinitions.Add(new DefinedText
                                {
                                    Ordinal = idx++,
                                    Text = condition.Message,
                                    Enabled = condition.IsSelected
                                });
                            }

                            existing.Presets.Clear();
                            foreach (var preset in profile.Profiles)
                            {
                                var p = new AtisPreset
                                {
                                    Name = preset.Name,
                                    Template = preset.AtisTemplate,
                                    AirportConditions = preset.AirportConditions,
                                    Notams = preset.Notams
                                };
                                existing.Presets.Add(p);
                            }

                            mAppConfig.SaveConfig();
                            RefreshCompositeList();
                            LoadComposite();
                        }
                    }
                    else
                    {
                        var composite = new AtisComposite();
                        composite.Name = profile.Name;
                        composite.Identifier = profile.ID;
                        if (!string.IsNullOrEmpty(profile.AtisFrequency))
                        {
                            composite.AtisFrequency = (int)(double.Parse(profile.AtisFrequency) * 1000) - 100000;
                        }
                        else
                        {
                            composite.AtisFrequency = profile.Frequency;
                        }
                        composite.IDSEndpoint = profile.InformationDisplaySystemEndpoint;
                        composite.AtisVoice.UseTextToSpeech = !profile.VoiceRecordEnabled;

                        if (profile.MagneticVariation != null)
                        {
                            if (profile.MagneticVariation.Option != LegacyMagneticVariation.LegacyMagneticVariationOption.None)
                            {
                                composite.MagneticVariation.Enabled = true;
                            }
                            switch (profile.MagneticVariation.Option)
                            {
                                case LegacyMagneticVariation.LegacyMagneticVariationOption.Add:
                                    composite.MagneticVariation.MagneticDegrees =
                                        Math.Abs(profile.MagneticVariation.MagneticVariationValue);
                                    break;
                                case LegacyMagneticVariation.LegacyMagneticVariationOption.Subtract:
                                    composite.MagneticVariation.MagneticDegrees =
                                        profile.MagneticVariation.MagneticVariationValue * -1;
                                    break;
                            }
                        }

                        if (profile.MetarObservation != null)
                        {
                            composite.ObservationTime.Enabled = profile.MetarObservation.Enable;
                            composite.ObservationTime.Time = (uint)profile.MetarObservation.ObservationTimeValue;
                        }

                        foreach (var contraction in profile.Contractions)
                        {
                            composite.Contractions.Add(new ContractionMeta
                            {
                                String = contraction.Key,
                                Spoken = contraction.Value
                            });
                        }

                        int idx = 1;
                        foreach (var condition in profile.AirportConditions)
                        {
                            composite.AirportConditionDefinitions.Add(new DefinedText
                            {
                                Ordinal = idx++,
                                Text = condition.Message,
                                Enabled = condition.IsSelected
                            });
                        }

                        idx = 1;
                        foreach (var condition in profile.Notams)
                        {
                            composite.NotamDefinitions.Add(new DefinedText
                            {
                                Ordinal = idx++,
                                Text = condition.Message,
                                Enabled = condition.IsSelected
                            });
                        }

                        foreach (var preset in profile.Profiles)
                        {
                            var p = new AtisPreset
                            {
                                Name = preset.Name,
                                Template = preset.AtisTemplate,
                                AirportConditions = preset.AirportConditions,
                                Notams = preset.Notams
                            };
                            composite.Presets.Add(p);
                        }

                        mAppConfig.CurrentProfile.Composites.Add(composite);
                        mAppConfig.SaveConfig();
                        RefreshCompositeList();
                    }
                }
            }
        }

        private void ctxExport_Click(object sender, EventArgs e)
        {
            if(mCurrentComposite != null)
            {
                var saveDialog = new SaveFileDialog
                {
                    FileName = $"vATIS Composite - {mCurrentComposite.Name}.json",
                    Filter = "vATIS Composite (*.json)|*.json|All Files (*.*)|*.*",
                    FilterIndex = 1,
                    CheckPathExists = true,
                    DefaultExt = "json",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                    OverwritePrompt = true,
                    ShowHelp = false,
                    SupportMultiDottedExtensions = true,
                    Title = "Export Composite",
                    ValidateNames = true
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveDialog.FileName, JsonConvert.SerializeObject(mCurrentComposite, Formatting.Indented));
                    MessageBox.Show(this, "Composite exported successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
        }

        private void gridTransitionLevels_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            gridTransitionLevels.Rows[e.RowIndex].ErrorText = "";
        }

        private void gridTransitionLevels_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (base.ActiveControl == btnCancel)
            {
                e.Cancel = true;
                btnCancel_Click(null, EventArgs.Empty);
                return;
            }
        }

        private void gridTransitionLevels_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            btnApply.Enabled = true;
            mTransitionLevelsChanged = true;
        }

        private void gridTransitionLevels_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            TextBox textBox = e.Control as TextBox;
            if (textBox != null)
            {
                textBox.KeyPress += (sender, arg) =>
                {
                    if (!char.IsControl(arg.KeyChar) && !char.IsDigit(arg.KeyChar))
                    {
                        arg.Handled = true;
                    }
                };
            }
        }

        private void btnDeleteTransitionLevel_Click(object sender, EventArgs e)
        {
            if (gridTransitionLevels.SelectedRows.Count == 1)
            {
                if (!gridTransitionLevels.SelectedRows[0].IsNewRow && MessageBox.Show(this, "Are you sure you want to delete the selected transition level?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    gridTransitionLevels.Rows.RemoveAt(gridTransitionLevels.SelectedRows[0].Index);
                    btnApply.Enabled = true;
                    mTransitionLevelsChanged = true;
                }
            }
            else
            {
                MessageBox.Show(this, "No transition level selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
    }
}