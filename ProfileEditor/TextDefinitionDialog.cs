using ProfileEditor.Core;
using System.Windows.Forms;

namespace ProfileEditor
{
    [IgnoreFormRegistration]
    public partial class TextDefinitionDialog : Form
    {
        public TextDefinitionDialog()
        {
            InitializeComponent();
        }

        public string TextValue
        {
            get => txtDefinition.Text;
            set => txtDefinition.Text = value;
        }
    }
}
