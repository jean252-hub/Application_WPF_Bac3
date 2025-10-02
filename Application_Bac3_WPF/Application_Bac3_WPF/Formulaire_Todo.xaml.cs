using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// <summary>
    /// Logique d'interaction pour Formulaire_Todo.xaml
    /// </summary>
    public partial class Formulaire_Todo : Window
    {
        public ObservableCollection<Tache> Taches 
        { get; set; } = new();

        public Formulaire_Todo()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            var texte = NewTaskTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(texte))
            {
                Taches.Add(new Tache { Titre = texte, IsDone = false });
                NewTaskTextBox.Clear();
            }
        }
        private void ModifyTask_Click(object sender, RoutedEventArgs e)
        {
            if (TasksListBox.SelectedItem is Tache tache)
            {
                var texte = ModifyTextbox.Text.Trim();
                if (!string.IsNullOrEmpty(texte))
                {
                    tache.Titre = texte;
                    
                }
            }
        }

        private void RemoveTask_Click(object sender, RoutedEventArgs e)
        {
            if (TasksListBox.SelectedItem is Tache tache)
            {
                Taches.Remove(tache);
            }
        }
    }
}
