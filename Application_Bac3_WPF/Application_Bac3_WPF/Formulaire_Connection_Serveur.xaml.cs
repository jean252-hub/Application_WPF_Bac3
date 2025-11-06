using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;

namespace Application_Bac3_WPF
{
    public partial class Formulaire_Connection_Serveur : Window
    {
        // ================= TCP Listener/Client =================
        private TcpListener tcpListener;
        private List<TcpClient> tcpClients = new List<TcpClient>();
        private TcpClient tcpClient;
        private NetworkStream netStream;
        private BinaryReader reader;
        private BinaryWriter writer;

        // ================= Socket =================
        private TcpListener socketListener;
        private TcpClient socketClient;
        private NetworkStream socketStream;
        private BinaryReader socketReader;
        private BinaryWriter socketWriter;
        private List<TcpClient> socketClients = new List<TcpClient>();

        // ================= UDP =================
        private UdpClient udpClient;

        public Formulaire_Connection_Serveur()
        {
            InitializeComponent();
        }

        #region ===== UDP =====
        private async void UDP_Ecouter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                udpClient = new UdpClient(8080);
                txtEchanges.AppendText("UDP en écoute sur port 8080...\n");

                while (true)
                {
                    var result = await udpClient.ReceiveAsync();
                    string msg = Encoding.UTF8.GetString(result.Buffer);
                    Dispatcher.Invoke(() =>
                    {
                        txtEchanges.AppendText($"Message UDP reçu : {msg}\n");
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur UDP Ecouter : " + ex.Message);
            }
        }

        private async void UDP_Connecter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string serveur = txtServeur.Text.Trim();
                string message = txtMessage.Text.Trim();
                if (string.IsNullOrEmpty(serveur)) return;

                udpClient = new UdpClient();
                udpClient.Connect(serveur, 8080);

                byte[] data = Encoding.UTF8.GetBytes(message);
                await udpClient.SendAsync(data, data.Length);

                txtEchanges.AppendText($"Message UDP envoyé : {message}\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur UDP Connecter : " + ex.Message);
            }
        }
        #endregion

        #region ===== TCP Serveur =====
        private async void Listener_Ecouter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8000);
                tcpListener.Start();
                txtEchanges.AppendText("Serveur TCP en écoute sur port 8000...\n");

                while (true)
                {
                    TcpClient client = await tcpListener.AcceptTcpClientAsync();
                    tcpClients.Add(client);
                    txtEchanges.AppendText($"Client TCP connecté : {client.Client.RemoteEndPoint}\n");

                    NetworkStream stream = client.GetStream();
                    BinaryReader r = new BinaryReader(stream);
                    BinaryWriter w = new BinaryWriter(stream);

                    _ = EcouterClient(client, r, w);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur Listener Écouter : " + ex.Message);
            }
        }

        private async Task EcouterClient(TcpClient client, BinaryReader reader, BinaryWriter writer)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                while (client.Connected)
                {
                    if (stream.DataAvailable)
                    {
                        string msg = reader.ReadString();
                        Dispatcher.Invoke(() =>
                        {
                            txtEchanges.AppendText($"Message TCP reçu ({client.Client.RemoteEndPoint}) : {msg}\n");
                        });

                        // Accusé de réception
                        writer.Write("Message reçu ✅");
                        writer.Flush();
                    }
                    else
                    {
                        await Task.Delay(50);
                    }
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    txtEchanges.AppendText($"Client TCP déconnecté : {ex.Message}\n");
                });
            }
            finally
            {
                tcpClients.Remove(client);
                client.Close();
            }
        }
        #endregion

        #region ===== TCP Client =====
        private async void Listener_Connecter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string serveur = txtServeur.Text.Trim();
                if (string.IsNullOrEmpty(serveur))
                {
                    MessageBox.Show("Aucun serveur défini !");
                    return;
                }

                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(serveur, 8000);
                txtEchanges.AppendText("Connexion TCP au serveur réussie ✅\n");

                netStream = tcpClient.GetStream();
                reader = new BinaryReader(netStream);
                writer = new BinaryWriter(netStream);

                _ = EcouterServeur();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur Listener Connecter : " + ex.Message);
            }
        }

        private async Task EcouterServeur()
        {
            try
            {
                while (tcpClient.Connected)
                {
                    if (netStream.DataAvailable)
                    {
                        string msg = reader.ReadString();
                        Dispatcher.Invoke(() =>
                        {
                            txtEchanges.AppendText($"Message serveur TCP : {msg}\n");
                        });
                    }
                    else
                    {
                        await Task.Delay(50);
                    }
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    txtEchanges.AppendText($"Déconnecté du serveur TCP : {ex.Message}\n");
                });
            }
        }

        private void BtnEnvoyer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string msg = txtMessage.Text;

                // TCP
                if (writer != null && tcpClient.Connected)
                {
                    writer.Write(msg);
                    writer.Flush();
                    txtEchanges.AppendText($"Message TCP envoyé : {msg}\n");
                }

                // Socket
                if (socketWriter != null && socketClient.Connected)
                {
                    socketWriter.Write(msg);
                    socketWriter.Flush();
                    txtEchanges.AppendText($"Message Socket envoyé : {msg}\n");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur Send : " + ex.Message);
            }
        }
        #endregion

        #region ===== SOCKET =====
        private async void Socket_Ecouter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                socketListener = new TcpListener(IPAddress.Any, 9000);
                socketListener.Start();
                txtEchanges.AppendText("Socket en écoute sur port 9000...\n");

                while (true)
                {
                    TcpClient client = await socketListener.AcceptTcpClientAsync();
                    socketClients.Add(client);
                    txtEchanges.AppendText($"Client Socket connecté : {client.Client.RemoteEndPoint}\n");

                    NetworkStream stream = client.GetStream();
                    BinaryReader r = new BinaryReader(stream);
                    BinaryWriter w = new BinaryWriter(stream);

                    _ = EcouterSocketClient(client, r, w);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur Socket Écouter : " + ex.Message);
            }
        }

        private async Task EcouterSocketClient(TcpClient client, BinaryReader reader, BinaryWriter writer)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                while (client.Connected)
                {
                    if (stream.DataAvailable)
                    {
                        string msg = reader.ReadString();
                        Dispatcher.Invoke(() =>
                        {
                            txtEchanges.AppendText($"Message Socket reçu ({client.Client.RemoteEndPoint}) : {msg}\n");
                        });

                        // Réponse automatique
                        writer.Write("Message reçu ✅");
                        writer.Flush();
                    }
                    else
                    {
                        await Task.Delay(50);
                    }
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    txtEchanges.AppendText($"Client Socket déconnecté : {ex.Message}\n");
                });
            }
            finally
            {
                socketClients.Remove(client);
                client.Close();
            }
        }

        private async void Socket_Connecter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string serveur = txtServeur.Text.Trim();
                if (string.IsNullOrEmpty(serveur))
                {
                    MessageBox.Show("Aucun serveur défini !");
                    return;
                }

                socketClient = new TcpClient();
                await socketClient.ConnectAsync(serveur, 9000);
                txtEchanges.AppendText("Connexion Socket au serveur réussie ✅\n");

                socketStream = socketClient.GetStream();
                socketReader = new BinaryReader(socketStream);
                socketWriter = new BinaryWriter(socketStream);

                // Lecture en continu depuis le serveur
                _ = Task.Run(async () =>
                {
                    try
                    {
                        while (socketClient.Connected)
                        {
                            if (socketStream.DataAvailable)
                            {
                                string msgServeur = socketReader.ReadString();
                                Dispatcher.Invoke(() =>
                                {
                                    txtEchanges.AppendText($"Message reçu (Socket) : {msgServeur}\n");
                                });
                            }
                            else
                            {
                                await Task.Delay(50);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(() =>
                            txtEchanges.AppendText($"Déconnecté du serveur Socket : {ex.Message}\n"));
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur Socket Connecter : " + ex.Message);
            }
        }

        private void Socket_Deconnecter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (socketWriter != null) socketWriter.Close();
                if (socketReader != null) socketReader.Close();
                if (socketStream != null) socketStream.Close();
                if (socketClient != null) socketClient.Close();

                foreach (var client in socketClients)
                    client.Close();

                if (socketListener != null) socketListener.Stop();
                socketClients.Clear();

                txtEchanges.AppendText("Socket déconnectée ✅\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur Socket Déconnecter : " + ex.Message);
            }
        }
        #endregion

        #region ===== Utilitaire =====
        private async void Utilitaire_Verifier_Click(object sender, RoutedEventArgs e)
        {
            string serveur = txtServeur.Text.Trim();
            if (string.IsNullOrEmpty(serveur))
            {
                MessageBox.Show("Veuillez entrer un nom de serveur.");
                return;
            }

            try
            {
                IPHostEntry hostEntry = await Dns.GetHostEntryAsync(serveur);
                IPAddress ip = hostEntry.AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);

                if (ip == null)
                {
                    MessageBox.Show("Aucune adresse IPv4 trouvée !");
                    return;
                }

                Ping ping = new Ping();
                PingReply reply = await ping.SendPingAsync(ip.ToString(), 3000);

                if (reply.Status == IPStatus.Success)
                {
                    txtIP.Text = ip.ToString();
                    MessageBox.Show("Serveur valide et ping réussi ✅");
                }
                else
                {
                    MessageBox.Show("Ping échoué ❌");
                }
            }
            catch
            {
                MessageBox.Show("Serveur invalide ou ping impossible ❌");
            }
        }
        #endregion
    }
}
