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
                // Détection automatique du serveur SMTP selon l'adresse
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

