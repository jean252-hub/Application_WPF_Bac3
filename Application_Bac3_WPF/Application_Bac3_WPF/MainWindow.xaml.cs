using System;
using System.Net;
using System.Net.Mail;
using System.Windows;

namespace Application_Bac3_WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public void OpenMailForm_Click(object sender, RoutedEventArgs e)
        {
            Formulaire_Mail mailWindow = new Formulaire_Mail();
            mailWindow.Show();
        }
        public void Open_todolist_Click(object sender, RoutedEventArgs e)
        {
            Formulaire_Todo todoWindow = new Formulaire_Todo();
            todoWindow.Show();
        }
        public void Open_chronometre_click(object sender, RoutedEventArgs e) { 
            Formulaire_Chronometre chronoWindow = new Formulaire_Chronometre();
            chronoWindow.Show();
        }
    }
}