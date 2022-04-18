using System.Windows.Forms;
using Vatsim.Vatis.Client.Core;

namespace Vatsim.Vatis.Client
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
