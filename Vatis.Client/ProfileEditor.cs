﻿using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Vatsim.Vatis.Client.Common;
using Vatsim.Vatis.Client.Config;
using Vatsim.Vatis.Client.Core;

namespace Vatsim.Vatis.Client
{
    internal partial class ProfileEditor : Form
    {
        private readonly IProfileEditorConfig mAppConfig;
        private readonly IUserInterface mUserInterface;
        private readonly INavaidDatabase mNavaidDatabase;

        private readonly SynchronizationContext mSyncContext;
        private AtisComposite mCurrentComposite;
        private AtisPreset mCurrentPreset;

        private bool mFrequencyChanged = false;
        private bool mObservationTimeChanged = false;
        private bool mMagneticVariationChanged = false;
        private bool mContractionsChanged = false;
        private bool mTransitionLevelsChanged = false;
        private bool mVoiceOptionsChanged = false;
        private bool mIdsEndpointChanged = false;
        private bool mFaaFormatChanged = false;
        private bool mAtisTypeChanged = false;
        private bool mExternalAtisGeneratorChanged = false;
        private bool mExternalUrlChanged = false;
        private bool mExternalArrivalChanged = false;
        private bool mExternalDepartureChanged = false;
        private bool mExternalApproachesChanged = false;
        private bool mExternalRemarksChanged = false;

        private List<AtisComposite> mSelectedComposites = new();

        public ProfileEditor(IProfileEditorConfig appConfig, IUserInterface userInterface, INavaidDatabase navaidDatabase)
        {
            InitializeComponent();

            mSyncContext = SynchronizationContext.Current;

            mAppConfig = appConfig;
            mUserInterface = userInterface;
            mNavaidDatabase = navaidDatabase;

            RefreshCompositeList();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (btnApply.Enabled)
            {
                if (MessageBox.Show(this, "You have unsaved changes. Are you sure you want to close without saving?", "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void RefreshCompositeList()
        {
            var temp = new List<ListBoxItem>();
            foreach (var composite in mAppConfig.Composites.OrderBy(x => x.Identifier))
            {
                temp.Add(new ListBoxItem { Text = composite.ToString(), Tag = composite });
            }
            listComposites.DataSource = temp.ToList();
            listComposites.SelectedIndex = -1;
            RefreshPresetList();
            ResetUI();
        }

        private void ResetUI()
        {
            Text = "vATIS Profile Editor";

            mainTabControl.Enabled = false;
            ctxDelete.Enabled = false;
            ctxRename.Enabled = false;
            ctxCopy.Enabled = false;
            ctxExport.Enabled = false;
            ctxExportSingleComposite.Enabled = false;

            txtAtisTemplate.Text = "";
            txtAirportCond.Text = "";
            txtNotams.Text = "";
            vhfFrequency.Text = "118.000";
            observationTime.Value = 0;
            magneticVar.Value = 0;
            chkMagneticVar.Checked = false;
            chkObservationTime.Checked = false;
            chkFaaFormat.Checked = false;
            chkExternalAtisGenerator.Checked = false;
            txtIdsEndpoint.Text = "";

            ResetExternalAtis();

            pageExternalAtis.SetVisible(false);
            pageTransitionLevel.SetVisible(false);
        }

        private void ResetExternalAtis()
        {
            txtSelectedPreset.Text = "[NONE]";
            txtExternalUrl.Text = "";
            txtExternalUrl.Enabled = false;
            txtExternalArr.Text = "";
            txtExternalDep.Text = "";
            txtExternalApp.Text = "";
            txtExternalRemarks.Text = "";
            tlpVariables.Enabled = false;
            groupTest.Enabled = false;
            txtMetar.Text = "";
        }

        private void RefreshPresetList()
        {
            if (mCurrentComposite != null)
            {
                ddlPresets.Items.Clear();

                foreach (var preset in mCurrentComposite.Presets)
                {
                    ddlPresets.Items.Add(preset.Name);
                }
            }

            btnCopyPreset.Enabled = ddlPresets.SelectedItem != null;
            btnDeletePreset.Enabled = ddlPresets.SelectedItem != null;
            btnRenamePreset.Enabled = ddlPresets.SelectedItem != null;
        }

        private void LoadComposite()
        {
            if (mCurrentComposite == null)
                return;

            vhfFrequency.Text = ((mCurrentComposite.AtisFrequency + 100000) / 1000.0).ToString("000.000");

            switch (mCurrentComposite.AtisType)
            {
                case AtisType.Combined:
                    typeCombined.Checked = true;
                    break;
                case AtisType.Departure:
                    typeDeparture.Checked = true;
                    break;
                case AtisType.Arrival:
                    typeArrival.Checked = true;
                    break;
            }

            chkFaaFormat.Checked = mCurrentComposite.UseFaaFormat;

            chkExternalAtisGenerator.Checked = mCurrentComposite.UseExternalAtisGenerator;

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
                        Voice = ddlVoices.SelectedItem.ToString() ?? "Default"
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
            foreach (var tl in mCurrentComposite.TransitionLevels.OrderByDescending(t => t.Low)
                .ThenByDescending(t => t.High))
            {
                gridTransitionLevels.Rows.Add(new object[]
                {
                    tl.Low,
                    tl.High,
                    tl.Altitude
                });
            }

            pageExternalAtis.SetVisible(mCurrentComposite.UseExternalAtisGenerator);

            if (mCurrentComposite.Identifier.StartsWith("K") || mCurrentComposite.Identifier.StartsWith("P"))
            {
                pageTransitionLevel.SetVisible(false);
            }
            else
            {
                pageTransitionLevel.SetVisible(true);
            }
        }

        private void ctxNew_Click(object sender, EventArgs e)
        {
            bool flag = false;

            var previousIdentifer = "";
            var previousName = "";
            var previousType = AtisType.Combined;

            while (!flag)
            {
                using var dlg = mUserInterface.CreateNewCompositeDialog();
                dlg.Identifier = previousIdentifer;
                dlg.CompositeName = previousName;
                dlg.Type = previousType;

                DialogResult result = dlg.ShowDialog(this);
                if (result == DialogResult.OK && !string.IsNullOrEmpty(dlg.Identifier))
                {
                    previousIdentifer = dlg.Identifier;
                    previousName = dlg.CompositeName;
                    previousType = dlg.Type;

                    if (mNavaidDatabase.GetAirport(dlg.Identifier) == null)
                    {
                        if (MessageBox.Show(this, $"ICAO identifier not found: {dlg.Identifier}", "Invalid Identifier", MessageBoxButtons.OK, MessageBoxIcon.Hand) == DialogResult.OK)
                        {
                            continue;
                        }
                    }

                    if (mAppConfig.Composites.Any(x => x.Identifier == dlg.Identifier && x.AtisType == dlg.Type))
                    {
                        if (MessageBox.Show(this, $"{dlg.Identifier} ({dlg.Type}) already exists. Would you like to overwrite it?", "Duplicate Composite", MessageBoxButtons.YesNo, MessageBoxIcon.Hand) == DialogResult.No)
                        {
                            continue;
                        }
                        else
                        {
                            mAppConfig.Composites.RemoveAll(x => x.Identifier == dlg.Identifier && x.AtisType == dlg.Type);
                            mAppConfig.SaveConfig();
                        }
                    }

                    var composite = new AtisComposite
                    {
                        Identifier = dlg.Identifier,
                        Name = dlg.CompositeName,
                        AtisType = dlg.Type
                    };

                    mAppConfig.Composites.Add(composite);
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

        private void ctxCopy_Click(object sender, EventArgs e)
        {
            if (listComposites.SelectedItem != null)
            {
                bool flag = false;
                var previousIdentifer = "";
                var previousName = "";
                var previousType = AtisType.Combined;

                var composite = (AtisComposite)((ListBoxItem)listComposites.SelectedItem).Tag;

                while (!flag)
                {
                    using var dlg = mUserInterface.CreateNewCompositeDialog();
                    dlg.Identifier = previousIdentifer;
                    dlg.CompositeName = previousName;
                    dlg.Type = previousType;

                    DialogResult result = dlg.ShowDialog(this);
                    if (result == DialogResult.OK && !string.IsNullOrEmpty(dlg.Identifier))
                    {
                        previousIdentifer = dlg.Identifier;
                        previousName = dlg.CompositeName;
                        previousType = dlg.Type;

                        if (mNavaidDatabase.GetAirport(dlg.Identifier) == null)
                        {
                            if (MessageBox.Show(this, $"ICAO identifier not found: {dlg.Identifier}", "Invalid Identifier", MessageBoxButtons.OK, MessageBoxIcon.Hand) == DialogResult.OK)
                            {
                                continue;
                            }
                        }

                        if (mAppConfig.Composites.Any(x => x.Identifier == dlg.Identifier && x.AtisType == dlg.Type))
                        {
                            if (MessageBox.Show(this, $"{dlg.Identifier} ({dlg.Type}) already exists.", "Duplicate Composite", MessageBoxButtons.OK, MessageBoxIcon.Hand) == DialogResult.OK)
                            {
                                continue;
                            }
                        }

                        var clone = composite.Clone();
                        clone.Identifier = dlg.Identifier;
                        clone.Name = dlg.CompositeName;
                        clone.AtisType = dlg.Type;

                        mAppConfig.Composites.Add(clone);
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

        private void ctxRename_Click(object sender, EventArgs e)
        {
            bool flag = false;

            if (listComposites.SelectedItem != null)
            {
                var composite = (AtisComposite)((ListBoxItem)listComposites.SelectedItem).Tag;
                if (composite != null)
                {
                    var previousValue = "";
                    while (!flag)
                    {
                        using var dlg = mUserInterface.CreateUserInputForm();
                        previousValue = composite.Name;
                        dlg.PromptLabel = "Enter a new name for the ATIS Composite:";
                        dlg.WindowTitle = "Rename ATIS Composite";
                        dlg.ErrorMessage = "Invalid composite name. It must consist of only letters, numbers, underscores and spaces.";
                        dlg.RegexExpression = "^[a-zA-Z0-9_ ]*$";
                        dlg.InitialValue = previousValue;

                        DialogResult result2 = dlg.ShowDialog(this);
                        if (result2 == DialogResult.OK && !string.IsNullOrEmpty(dlg.Value))
                        {
                            previousValue = dlg.Value;
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

        private void ctxDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, string.Format($"Are you sure you want to delete the selected Composites? This action cannot be undone."), "Delete Composites", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                foreach (var item in listComposites.SelectedItems)
                {
                    var composite = (AtisComposite)((ListBoxItem)item).Tag;
                    mAppConfig.Composites.Remove(composite);
                }
                mAppConfig.SaveConfig();
                RefreshCompositeList();
            }
        }

        private void ImportComposite(string fullName)
        {
            try
            {
                var composite = new AtisComposite();

                using var fs = new FileStream(fullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using (var sr = new StreamReader(fs))
                {
                    composite = JsonConvert.DeserializeObject<AtisComposite>(sr.ReadToEnd(), new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Error
                    });
                }

                if (composite != null)
                {
                    if (mAppConfig.Composites.Any(x => x.Identifier == composite.Identifier))
                    {
                        if (MessageBox.Show(this, $"A composite already exists for {composite.Identifier}. Do you want to overwrite it?", "Duplicate Composite", MessageBoxButtons.YesNo, MessageBoxIcon.Hand) == DialogResult.Yes)
                        {
                            mAppConfig.Composites.RemoveAll(t => t.Identifier == composite.Identifier);
                            mAppConfig.Composites.Add(composite);
                            mAppConfig.SaveConfig();
                            RefreshCompositeList();
                            LoadComposite();
                        }
                    }
                    else
                    {
                        mAppConfig.Composites.Add(composite);
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
            using Stream stream = new GZipStream(fs, CompressionMode.Decompress);
            XmlSerializer xml = new XmlSerializer(typeof(LegacyFacility));
            var profile = xml.Deserialize(stream) as LegacyFacility;

            if (profile == null)
                return;

            if (mAppConfig.Composites.Any(x => x.Identifier == profile.ID))
            {
                if (MessageBox.Show(this, $"A composite with that identifier already exists: {profile}\r\n\r\nWould you like to overwrite it?", "Duplicate Composite", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }

                var existing = mAppConfig.Composites.FirstOrDefault(t => t.Identifier == profile.ID);
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

                mAppConfig.Composites.Add(composite);
                mAppConfig.SaveConfig();
                RefreshCompositeList();
            }
        }

        private void ctxExport_Click(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveChanges();
        }

        private bool SaveChanges()
        {
            if (mCurrentComposite == null)
                return false;

            if (!string.IsNullOrEmpty(txtIdsEndpoint.Text) && !IsValidUrl(txtIdsEndpoint.Text))
            {
                MessageBox.Show("IDS endpoint URL is not a valid hyperlink format.");
                return false;
            }

            if(mFrequencyChanged)
            {
                if (decimal.TryParse(vhfFrequency.Text, out var frequency))
                {
                    frequency = frequency.ToVatsimFrequencyFormat();
                    if (frequency < 18000 || frequency > 37000)
                    {
                        MessageBox.Show(this, "Invalid frequency range. The accepted frequency range is 118.000-137.000 MHz", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        return false;
                    }
                    else
                    {
                        mCurrentComposite.AtisFrequency = (int)frequency;
                        mFrequencyChanged = false;
                    }
                }
            }

            if (mAtisTypeChanged)
            {
                if (typeCombined.Checked)
                {
                    if (mAppConfig.Composites.Any(x => x.Identifier == mCurrentComposite.Identifier && x.AtisType == AtisType.Combined && x != mCurrentComposite))
                    {
                        MessageBox.Show(this, $"A Combined ATIS already exists for {mCurrentComposite.Identifier}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        return false;
                    }
                    else
                    {
                        mCurrentComposite.AtisType = AtisType.Combined;
                        mAtisTypeChanged = false;
                    }
                }
                else if (typeDeparture.Checked)
                {
                    if (mAppConfig.Composites.Any(x => x.Identifier == mCurrentComposite.Identifier && x.AtisType == AtisType.Departure && x != mCurrentComposite))
                    {
                        MessageBox.Show(this, $"A Departure ATIS already exists for {mCurrentComposite.Identifier}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        return false;
                    }
                    else
                    {
                        mCurrentComposite.AtisType = AtisType.Departure;
                        mAtisTypeChanged = false;
                    }
                }
                else if (typeArrival.Checked)
                {
                    if (mAppConfig.Composites.Any(x => x.Identifier == mCurrentComposite.Identifier && x.AtisType == AtisType.Arrival && x != mCurrentComposite))
                    {
                        MessageBox.Show(this, $"An Arrival ATIS already exists for {mCurrentComposite.Identifier}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        return false;
                    }
                    else
                    {
                        mCurrentComposite.AtisType = AtisType.Arrival;
                        mAtisTypeChanged = false;
                    }
                }
            }

            if (mFaaFormatChanged)
            {
                mCurrentComposite.UseFaaFormat = chkFaaFormat.Checked;
                mFaaFormatChanged = false;
            }

            if (mObservationTimeChanged)
            {
                var meta = new ObservationTimeMeta
                {
                    Enabled = chkObservationTime.Checked,
                    Time = (uint)observationTime.Value
                };
                mCurrentComposite.ObservationTime = meta;
                mObservationTimeChanged = false;
            }

            if (mMagneticVariationChanged)
            {
                var meta = new MagneticVariationMeta
                {
                    Enabled = chkMagneticVar.Checked,
                    MagneticDegrees = (int)magneticVar.Value
                };
                mCurrentComposite.MagneticVariation = meta;
                mMagneticVariationChanged = false;
            }

            if (mIdsEndpointChanged)
            {
                mCurrentComposite.IDSEndpoint = txtIdsEndpoint.Text;
                mIdsEndpointChanged = false;
            }

            if (mVoiceOptionsChanged)
            {
                mCurrentComposite.AtisVoice.UseTextToSpeech = radioTextToSpeech.Checked;
                mCurrentComposite.AtisVoice.Voice = ddlVoices.SelectedItem.ToString() ?? "Default";
                mVoiceOptionsChanged = false;
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
                        if (!string.IsNullOrEmpty(stringValue))
                        {
                            if (usedContractions.Contains(stringValue))
                            {
                                MessageBox.Show(this, "Duplicate contraction: " + stringValue, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                gridContractions.Focus();
                                return false;
                            }
                            usedContractions.Add(stringValue);
                        }
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
                        if (int.TryParse(row.Cells[0].Value.ToString(), out int low)
                            && int.TryParse(row.Cells[1].Value.ToString(), out int high))
                        {
                            if (usedTransitionLevels.Any(t => t.Item1 == low && t.Item2 == high))
                            {
                                MessageBox.Show(this, $"Duplicate Transition Level: {low}-{high}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                gridTransitionLevels.Focus();
                                return false;
                            }
                            usedTransitionLevels.Add(new Tuple<int, int>(low, high));
                        }
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
                        if (int.TryParse(row.Cells[0].Value.ToString(), out int low)
                            && int.TryParse(row.Cells[1].Value.ToString(), out int high)
                            && int.TryParse(row.Cells[2].Value.ToString(), out int tl))
                        {
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

            if (mExternalAtisGeneratorChanged)
            {
                mCurrentComposite.UseExternalAtisGenerator = chkExternalAtisGenerator.Checked;
                mExternalAtisGeneratorChanged = false;
            }

            if (mExternalUrlChanged)
            {
                mCurrentPreset.ExternalGenerator.Url = txtExternalUrl.Text;
                mExternalUrlChanged = false;
            }

            if (mExternalArrivalChanged)
            {
                mCurrentPreset.ExternalGenerator.Arrival = txtExternalArr.Text;
                mExternalArrivalChanged = false;
            }

            if (mExternalDepartureChanged)
            {
                mCurrentPreset.ExternalGenerator.Departure = txtExternalDep.Text;
                mExternalDepartureChanged = false;
            }

            if (mExternalApproachesChanged)
            {
                mCurrentPreset.ExternalGenerator.Approaches = txtExternalApp.Text;
                mExternalApproachesChanged = false;
            }

            if (mExternalRemarksChanged)
            {
                mCurrentPreset.ExternalGenerator.Remarks = txtExternalRemarks.Text;
                mExternalRemarksChanged = false;
            }

            mTransitionLevelsChanged = false;
            mContractionsChanged = false;
            btnApply.Enabled = false;

            mAppConfig.SaveConfig();

            return true;
        }

        private void ddlPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnCopyPreset.Enabled = ddlPresets.SelectedItem != null;
            btnDeletePreset.Enabled = ddlPresets.SelectedItem != null;
            btnRenamePreset.Enabled = ddlPresets.SelectedItem != null;
            txtAtisTemplate.Enabled = ddlPresets.SelectedItem != null;
            txtAirportCond.Enabled = ddlPresets.SelectedItem != null;
            txtNotams.Enabled = ddlPresets.SelectedItem != null;

            if (ddlPresets.SelectedItem != null && mCurrentComposite != null)
            {
                mCurrentPreset = mCurrentComposite.Presets.FirstOrDefault(x => x.Name == ddlPresets.SelectedItem.ToString());

                if (mCurrentPreset != null)
                {
                    if (mCurrentComposite.UseExternalAtisGenerator)
                    {
                        txtAtisTemplate.Enabled = false;
                        txtAirportCond.Enabled = false;
                        txtNotams.Enabled = false;
                        notams.Enabled = false;
                        airportConditions.Enabled = false;

                        txtSelectedPreset.Text = mCurrentPreset.Name;
                        txtExternalUrl.Enabled = ddlPresets.SelectedItem != null;
                        tlpVariables.Enabled = ddlPresets.SelectedItem != null;
                        groupTest.Enabled = ddlPresets.SelectedItem != null;

                        if (mCurrentPreset.ExternalGenerator != null)
                        {
                            txtExternalUrl.Text = mCurrentPreset.ExternalGenerator.Url;
                            txtExternalArr.Text = mCurrentPreset.ExternalGenerator.Arrival;
                            txtExternalDep.Text = mCurrentPreset.ExternalGenerator.Departure;
                            txtExternalApp.Text = mCurrentPreset.ExternalGenerator.Approaches;
                            txtExternalRemarks.Text = mCurrentPreset.ExternalGenerator.Remarks;
                        }
                    }
                    else
                    {
                        txtAtisTemplate.Text = mCurrentPreset.Template;
                        txtAtisTemplate.Enabled = true;
                        txtAirportCond.Enabled = true;
                        txtNotams.Enabled = true;
                        notams.Enabled = true;
                        airportConditions.Enabled = true;
                    }
                }
            }
        }

        private void txtAtisTemplate_TextChanged(object sender, EventArgs e)
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

        private void gridContractions_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is TextBox textBox)
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

        private void gridContractions_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            btnApply.Enabled = true;
            mContractionsChanged = true;
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

        private void vhfFrequency_TextChanged(object sender, EventArgs e)
        {
            if (mCurrentComposite == null)
                return;

            if (!vhfFrequency.Focused)
                return;

            if (decimal.TryParse(vhfFrequency.Text, out var frequency))
            {
                if (frequency.ToVatsimFrequencyFormat() != mCurrentComposite.AtisFrequency)
                {
                    mFrequencyChanged = true;
                    btnApply.Enabled = true;
                }
                else
                {
                    mFrequencyChanged = false;
                    btnApply.Enabled = false;
                }
            }
        }

        private void chkFaaFormat_CheckedChanged(object sender, EventArgs e)
        {
            if (mCurrentComposite == null)
                return;

            if (!chkFaaFormat.Focused)
                return;

            if (chkFaaFormat.Checked != mCurrentComposite.UseFaaFormat)
            {
                mFaaFormatChanged = true;
                btnApply.Enabled = true;
            }
            else
            {
                btnApply.Enabled = false;
            }
        }

        private void typeCombined_CheckedChanged(object sender, EventArgs e)
        {
            if (mCurrentComposite == null)
                return;

            if (!typeCombined.Focused)
                return;

            if (typeCombined.Checked && mCurrentComposite.AtisType != AtisType.Combined)
            {
                mAtisTypeChanged = true;
                btnApply.Enabled = true;
            }
            else
            {
                mAtisTypeChanged = false;
                btnApply.Enabled = false;
            }
        }

        private void typeDeparture_CheckedChanged(object sender, EventArgs e)
        {
            if (mCurrentComposite == null)
                return;

            if (!typeDeparture.Focused)
                return;

            if (typeDeparture.Checked && mCurrentComposite.AtisType != AtisType.Departure)
            {
                mAtisTypeChanged = true;
                btnApply.Enabled = true;
            }
            else
            {
                mAtisTypeChanged = false;
                btnApply.Enabled = false;
            }
        }

        private void typeArrival_CheckedChanged(object sender, EventArgs e)
        {
            if (mCurrentComposite == null)
                return;

            if (!typeArrival.Focused)
                return;

            if (typeArrival.Checked && mCurrentComposite.AtisType != AtisType.Arrival)
            {
                mAtisTypeChanged = true;
                btnApply.Enabled = true;
            }
            else
            {
                mAtisTypeChanged = false;
                btnApply.Enabled = false;
            }
        }

        private void chkObservationTime_CheckedChanged(object sender, EventArgs e)
        {
            if (mCurrentComposite == null)
                return;

            if (!chkObservationTime.Focused)
                return;

            observationTime.Enabled = chkObservationTime.Checked;

            if (chkObservationTime.Checked != mCurrentComposite.ObservationTime.Enabled)
            {
                mObservationTimeChanged = true;
                btnApply.Enabled = true;
            }
            else
            {
                btnApply.Enabled = false;
            }
        }

        private void observationTime_ValueChanged(object sender, EventArgs e)
        {
            if (mCurrentComposite == null)
                return;

            if (!observationTime.Focused)
                return;

            if (observationTime.Value != mCurrentComposite.ObservationTime.Time)
            {
                mObservationTimeChanged = true;
                btnApply.Enabled = true;
            }
            else
            {
                mObservationTimeChanged = false;
                btnApply.Enabled = false;
            }
        }

        private void observationTime_KeyUp(object sender, KeyEventArgs e)
        {
            observationTime_ValueChanged(sender, e);
        }

        private void chkMagneticVar_CheckedChanged(object sender, EventArgs e)
        {
            if (mCurrentComposite == null)
                return;

            if (!chkMagneticVar.Focused)
                return;

            magneticVar.Enabled = chkMagneticVar.Checked;

            if (chkMagneticVar.Checked != mCurrentComposite.MagneticVariation.Enabled)
            {
                mMagneticVariationChanged = true;
                btnApply.Enabled = true;
            }
            else
            {
                mMagneticVariationChanged = false;
                btnApply.Enabled = false;
            }
        }

        private void magneticVar_ValueChanged(object sender, EventArgs e)
        {
            if (mCurrentComposite == null)
                return;

            if (!magneticVar.Focused)
                return;

            if (magneticVar.Value != mCurrentComposite.MagneticVariation.MagneticDegrees)
            {
                mMagneticVariationChanged = true;
                btnApply.Enabled = true;
            }
            else
            {
                mMagneticVariationChanged = false;
                btnApply.Enabled = false;
            }
        }

        private void magneticVar_KeyUp(object sender, KeyEventArgs e)
        {
            magneticVar_ValueChanged(sender, e);
        }

        private void radioTextToSpeech_CheckedChanged(object sender, EventArgs e)
        {
            if (mCurrentComposite == null)
                return;

            if (!radioTextToSpeech.Focused)
                return;

            ddlVoices.Enabled = radioTextToSpeech.Checked;

            if (mCurrentComposite.AtisVoice != null)
            {
                if (mCurrentComposite.AtisVoice.UseTextToSpeech == false && radioTextToSpeech.Checked)
                {
                    mVoiceOptionsChanged = true;
                    btnApply.Enabled = true;
                }
                else
                {
                    mVoiceOptionsChanged = false;
                    btnApply.Enabled = false;
                }
            }
            else
            {
                mVoiceOptionsChanged = false;
                btnApply.Enabled = false;
            }
        }

        private void ddlVoices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mCurrentComposite == null)
                return;

            if (!ddlVoices.Focused)
                return;

            if (mCurrentComposite.AtisVoice != null)
            {
                if (ddlVoices.SelectedItem.ToString() != mCurrentComposite.AtisVoice.Voice)
                {
                    mVoiceOptionsChanged = true;
                    btnApply.Enabled = true;
                }
            }
            else
            {
                mCurrentComposite.AtisVoice = new AtisVoiceMeta
                {
                    UseTextToSpeech = true,
                    Voice = ddlVoices.SelectedItem.ToString() ?? "Default"
                };

                mVoiceOptionsChanged = true;
                btnApply.Enabled = true;
            }
        }

        private void radioVoiceRecorded_CheckedChanged(object sender, EventArgs e)
        {
            if (mCurrentComposite == null)
                return;

            if (!radioVoiceRecorded.Focused)
                return;

            ddlVoices.Enabled = !radioVoiceRecorded.Checked;

            if (mCurrentComposite.AtisVoice != null)
            {
                if (radioVoiceRecorded.Checked && mCurrentComposite.AtisVoice.UseTextToSpeech)
                {
                    mVoiceOptionsChanged = true;
                    btnApply.Enabled = true;
                }
                else
                {
                    mVoiceOptionsChanged = false;
                    btnApply.Enabled = false;
                }
            }
            else
            {
                mVoiceOptionsChanged = false;
                btnApply.Enabled = false;
            }
        }

        private void txtIdsEndpoint_TextChanged(object sender, EventArgs e)
        {
            if (mCurrentComposite == null)
                return;

            if (!txtIdsEndpoint.Focused)
                return;

            if ((txtIdsEndpoint.Text ?? "") != (mCurrentComposite.IDSEndpoint ?? ""))
            {
                mIdsEndpointChanged = true;
                btnApply.Enabled = true;
            }
            else
            {
                mIdsEndpointChanged = false;
                btnApply.Enabled = false;
            }
        }

        private void btnNewPreset_Click(object sender, EventArgs e)
        {
            if (mCurrentComposite == null)
                return;

            bool flag = false;
            var previousValue = "";

            while (!flag)
            {
                using var dlg = mUserInterface.CreateUserInputForm();
                dlg.PromptLabel = "Enter a name for the preset";
                dlg.WindowTitle = "New Preset";
                dlg.InitialValue = previousValue;
                dlg.TextUppercase = true;

                DialogResult result = dlg.ShowDialog(this);
                if (result == DialogResult.OK && !string.IsNullOrEmpty(dlg.Value))
                {
                    previousValue = dlg.Value;

                    if (mCurrentComposite.Presets.Any(x => x.Name == dlg.Value))
                    {
                        MessageBox.Show(this, "Another profile already exists with that name. Please choose another.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    var preset = new AtisPreset
                    {
                        Name = dlg.Value,
                        Template = "[FACILITY] ATIS INFO [ATIS_LETTER] [TIME]. [WX]. [ARPT_COND] [NOTAMS]."
                    };
                    mCurrentComposite.Presets.Add(preset);
                    mAppConfig.SaveConfig();

                    RefreshPresetList();
                    previousValue = "";

                    flag = true;
                }
                else
                {
                    flag = true;
                }
            }
        }

        private void btnCopyPreset_Click(object sender, EventArgs e)
        {
            if (mCurrentComposite == null || mCurrentPreset == null)
                return;

            bool flag = false;
            var previousValue = "";

            while (!flag)
            {
                using var dlg = mUserInterface.CreateUserInputForm();
                dlg.PromptLabel = "Enter a name for the preset";
                dlg.WindowTitle = "Copy Preset";
                dlg.InitialValue = previousValue;
                dlg.TextUppercase = true;

                DialogResult result = dlg.ShowDialog(this);
                if (result == DialogResult.OK && !string.IsNullOrEmpty(dlg.Value))
                {
                    previousValue = dlg.Value;

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
                        previousValue = "";

                        flag = true;
                    }
                }
                else
                {
                    flag = true;
                }
            }
        }

        private void btnRenamePreset_Click(object sender, EventArgs e)
        {
            if (mCurrentComposite == null || mCurrentPreset == null)
                return;

            bool flag = false;
            var previousValue = mCurrentPreset.Name;

            while (!flag)
            {
                using var dlg = mUserInterface.CreateUserInputForm();
                dlg.PromptLabel = "Enter a new name for the preset";
                dlg.WindowTitle = "Rename Preset";
                dlg.InitialValue = previousValue;
                dlg.TextUppercase = true;

                DialogResult result = dlg.ShowDialog(this);
                if (result == DialogResult.OK && !string.IsNullOrEmpty(dlg.Value))
                {
                    previousValue = dlg.Value;

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
                        ResetExternalAtis();
                        previousValue = "";

                        flag = true;
                    }
                }
                else
                {
                    flag = true;
                }
            }
        }

        private void btnDeletePreset_Click(object sender, EventArgs e)
        {
            if (mCurrentComposite == null)
                return;

            if (ddlPresets.SelectedItem != null)
            {
                if (MessageBox.Show(this, "Are you sure you want to delete the selected preset?", "Delete Preset", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    mCurrentComposite.Presets.RemoveAll(x => x.Name == ddlPresets.SelectedItem.ToString());
                    mAppConfig.SaveConfig();
                    txtAtisTemplate.Text = "";
                    txtAtisTemplate.Enabled = false;
                    RefreshPresetList();
                    ResetExternalAtis();
                }
            }
        }

        private void listComposites_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listComposites.SelectedItems.Count == 1)
            {
                var composite = (AtisComposite)((ListBoxItem)listComposites.SelectedItem).Tag;
                if (composite != null)
                {
                    mainTabControl.Enabled = true;
                    ctxDelete.Enabled = true;
                    ctxRename.Enabled = true;
                    ctxCopy.Enabled = true;
                    ctxExport.Enabled = true;
                    ctxExportSingleComposite.Enabled = true;
                    txtAtisTemplate.Text = "";
                    txtAirportCond.Text = "";
                    txtNotams.Text = "";
                    Text = $"vATIS Profile Editor: {composite}";
                    mCurrentComposite = composite;
                    LoadComposite();
                    RefreshPresetList();
                }
            }

            if (listComposites.SelectedItems != null)
            {
                mSelectedComposites.Clear();

                if (listComposites.SelectedItems.Count > 1)
                {
                    ResetUI();
                    ctxExport.Enabled = true;
                    ctxDelete.Enabled = true;
                    ctxExportToProfile.Enabled = true;
                }

                foreach (var selected in listComposites.SelectedItems)
                {
                    var composite = (AtisComposite)((ListBoxItem)selected).Tag;
                    if (composite != null)
                    {
                        mSelectedComposites.Add(composite);
                    }
                }
            }
        }

        private void airportConditions_Click(object sender, EventArgs e)
        {
            if (mCurrentComposite == null)
                return;

            using var dlg = new AirportConditionsDialog(mCurrentComposite);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                mAppConfig.SaveConfig();
            }
        }

        private void notams_Click(object sender, EventArgs e)
        {
            if (mCurrentComposite == null)
                return;

            using var dlg = new NotamDefinitionsDialog(mCurrentComposite);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                mAppConfig.SaveConfig();
            }
        }

        private void ctxImportLegacyProfile_Click(object sender, EventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Title = "Import Legacy vATIS Profile",
                    CheckFileExists = true,
                    CheckPathExists = true,
                    AddExtension = false,
                    Multiselect = true,
                    Filter = "Legacy vATIS Profile (*.gz)|*.gz",
                    FilterIndex = 1,
                    DefaultExt = "gz",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                    ShowHelp = false
                };
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    foreach (var file in dialog.FileNames)
                    {
                        ImportLegacyProfile(file);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Legacy Profile Import Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void ctxImportComposite_Click(object sender, EventArgs e)
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
                    Filter = "vATIS Composite (*.json)|*.json",
                    FilterIndex = 1,
                    DefaultExt = "gz",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                    ShowHelp = false
                };
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    foreach (var file in dialog.FileNames)
                    {
                        ImportComposite(file);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Composite Import Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void ctxExportToProfile_Click(object sender, EventArgs e)
        {
            if (mSelectedComposites == null)
                return;

            if (mSelectedComposites.Count > 15)
            {
                MessageBox.Show(this, "A profile cannot contain more than 15 composites.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            bool flag = false;
            var previousValue = "";

            while (!flag)
            {
                using var dlg = mUserInterface.CreateUserInputForm();
                dlg.PromptLabel = "Enter a name for the profile:";
                dlg.WindowTitle = "Save Profile As";
                dlg.ErrorMessage = "Invalid profile name. It must consist of only letters, numbers, underscores and spaces.";
                dlg.RegexExpression = "[A-Za-z0-9_ ]+";
                dlg.InitialValue = previousValue;

                DialogResult result = dlg.ShowDialog(this);
                if (result == DialogResult.OK && !string.IsNullOrEmpty(dlg.Value))
                {
                    var profile = new Profile
                    {
                        Name = dlg.Value,
                        Composites = mSelectedComposites
                    };

                    var saveDialog = new SaveFileDialog
                    {
                        FileName = $"vATIS Profile - {dlg.Value}.json",
                        Filter = "vATIS Composite (*.json)|*.json|All Files (*.*)|*.*",
                        FilterIndex = 1,
                        CheckPathExists = true,
                        DefaultExt = "json",
                        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                        OverwritePrompt = true,
                        ShowHelp = false,
                        SupportMultiDottedExtensions = true,
                        Title = "Export Profile",
                        ValidateNames = true
                    };

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        flag = true;
                        File.WriteAllText(saveDialog.FileName, JsonConvert.SerializeObject(profile, Formatting.Indented));
                        MessageBox.Show(this, "Profile exported successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
                else
                {
                    flag = true;
                }
            }
        }

        private void ctxExportSingleComposite_Click(object sender, EventArgs e)
        {
            if (mCurrentComposite != null)
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

        private void chkExternalAtisGenerator_CheckedChanged(object sender, EventArgs e)
        {
            if (mCurrentComposite == null)
                return;

            if (!chkExternalAtisGenerator.Focused)
                return;

            pageExternalAtis.SetVisible(chkExternalAtisGenerator.Checked);
            RefreshPresetList();

            if (chkExternalAtisGenerator.Checked != mCurrentComposite.UseExternalAtisGenerator)
            {
                mExternalAtisGeneratorChanged = true;
                btnApply.Enabled = true;
            }
            else
            {
                btnApply.Enabled = false;
            }
        }

        private void txtExternalUrl_TextChanged(object sender, EventArgs e)
        {
            if (mCurrentPreset == null)
                return;

            if (!txtExternalUrl.Focused)
                return;

            if (txtExternalUrl.Text != mCurrentPreset.ExternalGenerator.Url)
            {
                mExternalUrlChanged = true;
                btnApply.Enabled = true;
            }
            else
            {
                mExternalUrlChanged = false;
                btnApply.Enabled = false;
            }
        }

        private void txtExternalArr_TextChanged(object sender, EventArgs e)
        {
            if (mCurrentPreset == null)
                return;

            if (!txtExternalArr.Focused)
                return;

            if (txtExternalArr.Text != mCurrentPreset.ExternalGenerator.Arrival)
            {
                mExternalArrivalChanged = true;
                btnApply.Enabled = true;
            }
            else
            {
                mExternalArrivalChanged = false;
                btnApply.Enabled = false;
            }
        }

        private void txtExternalDep_TextChanged(object sender, EventArgs e)
        {
            if (mCurrentPreset == null)
                return;

            if (!txtExternalDep.Focused)
                return;

            if (txtExternalDep.Text != mCurrentPreset.ExternalGenerator.Departure)
            {
                mExternalDepartureChanged = true;
                btnApply.Enabled = true;
            }
            else
            {
                mExternalArrivalChanged = false;
                btnApply.Enabled = false;
            }
        }

        private void txtExternalApp_TextChanged(object sender, EventArgs e)
        {
            if (mCurrentPreset == null)
                return;

            if (!txtExternalApp.Focused)
                return;

            if (txtExternalApp.Text != mCurrentPreset.ExternalGenerator.Approaches)
            {
                mExternalApproachesChanged = true;
                btnApply.Enabled = true;
            }
            else
            {
                mExternalApproachesChanged = false;
                btnApply.Enabled = false;
            }
        }

        private void txtExternalRemarks_TextChanged(object sender, EventArgs e)
        {
            if (mCurrentPreset == null)
                return;

            if (!txtExternalRemarks.Focused)
                return;

            if (txtExternalRemarks.Text != mCurrentPreset.ExternalGenerator.Remarks)
            {
                mExternalRemarksChanged = true;
                btnApply.Enabled = true;
            }
            else
            {
                mExternalRemarksChanged = false;
                btnApply.Enabled = false;
            }
        }

        private void txtMetar_TextChanged(object sender, EventArgs e)
        {
            btnTest.Enabled = !string.IsNullOrEmpty(txtMetar.Text);
        }

        private void btnFetchMetar_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {

                mSyncContext.Post(o =>
                {
                    btnFetchMetar.Enabled = false;
                    btnTest.Enabled = false;
                }, null);

                var result = "";

                try
                {
                    var client = new RestClient();
                    var request = new RestRequest("https://metar.vatsim.net/metar.php?id=" + mCurrentComposite.Identifier);
                    result = client.Get<string>(request).Content;
                }
                catch { }

                mSyncContext.Post(o =>
                {
                    txtMetar.Text = result;
                    btnFetchMetar.Enabled = true;
                    btnTest.Enabled = true;
                }, null);
            });
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            var url = txtExternalUrl.Text;
            if (!string.IsNullOrEmpty(url))
            {
                url = url.Replace("$metar", System.Web.HttpUtility.UrlEncode(txtMetar.Text));
                url = url.Replace("$arrrwy", txtExternalArr.Text);
                url = url.Replace("$deprwy", txtExternalDep.Text);
                url = url.Replace("$app", txtExternalApp.Text);
                url = url.Replace("$remarks", txtExternalRemarks.Text);
                url = url.Replace("$atiscode", RandomLetter());

                Task.Run(() =>
                {
                    mSyncContext.Post(o =>
                    {
                        btnTest.Text = "Loading...";
                        btnTest.Enabled = false;
                    }, null);

                    var result = "";

                    try
                    {
                        var client = new RestClient();
                        var request = new RestRequest(url);
                        var response = client.Get<string>(request);

                        result = Regex.Replace(response.Content, @"\[(.*?)\]", " $1 ");
                        result = Regex.Replace(result, @"\s+", " ");
                    }
                    catch { }

                    mSyncContext.Post(o => {
                        txtResponse.Text = result.Trim(' ');
                        btnTest.Text = "Test URL";
                        btnTest.Enabled = true;
                    }, null);
                });
            }
        }

        private static bool IsValidUrl(string value)
        {
            var pattern = new Regex(@"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$");
            return pattern.IsMatch(value.Trim());
        }

        private static string RandomLetter()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var random = new Random();
            return new string(Enumerable.Range(1, 1).Select(_ => chars[random.Next(chars.Length)]).ToArray());
        }
    }

    internal class ListBoxItem
    {
        public object Tag { get; set; }
        public string Text { get; set; }
        public override string ToString() => Text;
    }
}
