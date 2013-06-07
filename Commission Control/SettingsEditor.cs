using System;
using System.Windows.Forms;

namespace D3.Commission
{
    public partial class SettingsEditor : Form
    {
        public Settings Settings { get; set; }

        public bool IsDirty { get; set; }
        
        public SettingsEditor(Settings settings)
        {
            Settings = settings;
            InitializeComponent();
        }

        private void SettingsEditor_Load(object sender, EventArgs e)
        {
            ServerName.Text = Settings.ServerName;
            DatabaseName.Text = Settings.DatabaseName;
            IntegratedSecurity.Checked = Settings.IntegratedSecurity;
            UserID.Text = Settings.UserID;
            Password.Text = Settings.Password;
        }

        private void IntegratedSecurity_CheckStateChanged(object sender, EventArgs e)
        {
            IsIntegratedSecurity();            
        }

        private void IsIntegratedSecurity()
        {
            UserID.Enabled = (!IntegratedSecurity.Checked);
            Password.Enabled = (!IntegratedSecurity.Checked);
        }

        private void Save_Click(object sender, EventArgs e)
        {
            SaveSettings();
            Close();
        }

        private void SaveSettings()
        {
            if (Settings.ServerName != ServerName.Text)
            {
                Settings.ServerName = ServerName.Text;
                IsDirty = true;
            }
            if (Settings.DatabaseName != DatabaseName.Text)
            {
                Settings.DatabaseName = DatabaseName.Text;
                IsDirty = true;
            }
            if (Settings.IntegratedSecurity != IntegratedSecurity.Checked)
            {
                Settings.IntegratedSecurity = IntegratedSecurity.Checked;
                IsDirty = true;
            }
            if (Settings.UserID != UserID.Text)
            {
                Settings.UserID = UserID.Text;
                IsDirty = true;
            }
            if (Settings.Password != Password.Text)
            {
                Settings.Password = Password.Text;
                IsDirty = true;
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}