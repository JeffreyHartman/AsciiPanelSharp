using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AsciiPanelSharp;

namespace rlSharp
{
    /// <summary>
    /// To use AsciiPanelSharp, you must create a form that inherits from AsciiPanel. You can then use all the AsciiPanel methods to draw onto the form.
    /// </summary>
    public partial class rlSharp : AsciiPanel
    {
        public rlSharp()
        {
            InitializeComponent();
            this.Write("test");
        }
    }
}
