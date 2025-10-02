using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
    /// Logique d'interaction pour Formulaire_Mail.xaml
    /// </summary>
    public partial class Formulaire_Mail : Window
    {
        public Formulaire_Mail()
        {
            InitializeComponent();
        }
        private void SenderEmailTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string senderEmail = SenderEmailTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string recipientEmail = RecipientEmailTextBox.Text.Trim();
            string subject = SubjectTextBox.Text.Trim();
            string body = BodyTextBox.Text;

            if (string.IsNullOrWhiteSpace(senderEmail) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(recipientEmail))
            {
                MessageBox.Show("Veuillez remplir tous les champs obligatoires.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                string smtpHost;
                int smtpPort;
                bool enableSsl = true;

                if (senderEmail.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
                {
                    smtpHost = "smtp.gmail.com";
                    smtpPort = 587;
                }
                else if (senderEmail.EndsWith("@outlook.com", StringComparison.OrdinalIgnoreCase) ||
                         senderEmail.EndsWith("@hotmail.com", StringComparison.OrdinalIgnoreCase) ||
                         senderEmail.EndsWith("@live.com", StringComparison.OrdinalIgnoreCase))
                {
                    smtpHost = "smtp.office365.com";
                    smtpPort = 587;
                }
                else
                {
                    MessageBox.Show("Fournisseur non reconnu. Veuillez configurer manuellement le serveur SMTP.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var smtpClient = new SmtpClient(smtpHost)
                {
                    Port = smtpPort,
                    Credentials = new NetworkCredential(senderEmail, password),
                    EnableSsl = enableSsl
                };

                var mailMessage = new MailMessage(senderEmail, recipientEmail, subject, body);

                smtpClient.Send(mailMessage);

                MessageBox.Show("Mail envoyé avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'envoi : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
 }
