
using Application_Bac3_WPF.Vue_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Application_Bac3_WPF
{

    public partial class Formulaire_Chronometre : Window
    {
        public Formulaire_Chronometre()
        {
            InitializeComponent();
            DataContext = new VM_Chronometre();
        }
    }
}
