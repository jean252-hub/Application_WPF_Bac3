using System.ComponentModel;

namespace Application_Bac3_WPF
{
    public class Tache : INotifyPropertyChanged
    {
        private string titre;
        private bool isDone;

        public string Titre
        {
            get => titre;
            set { titre = value; OnPropertyChanged(nameof(Titre)); }
        }

        public bool IsDone
        {
            get => isDone;
            set { isDone = value; OnPropertyChanged(nameof(IsDone)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}