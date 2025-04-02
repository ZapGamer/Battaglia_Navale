using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client_navale
{
    public partial class Form1 : Form
    {
        private static Thread tr, trRicevi;
        private delegate void delegato();
        private delegate void delegato2(object obj);
        private delegate void delegato3(string s);
        private static TcpClient client;
        private static NetworkStream stream;
        private Button[,] buttons, griglia;
        private static string[,] grid;
        private const int gridSize = 10;
        private Label LabelDati;
        private string risposta;
        private PictureBox Sfondo;
        public Form1()
        {
            InitializeComponent();
            InitializeElements();
            LabelDati = (Label)this.Controls["LabelDati"];
            listBoxMessaggi.Items.Add("Chat:");
            AggiornaLabels();
        }
        //generazione elementi
        private void InitializeElements()
        {
            buttons = new Button[gridSize, gridSize];

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    buttons[i, j] = new Button();
                    buttons[i, j].Width = 40;
                    buttons[i, j].Height = 40;
                    buttons[i, j].Top = 50 + (i * 45);
                    buttons[i, j].Left = 50 + (j * 45);
                    buttons[i, j].Tag = $"{(char)('A' + i)}{j + 1}";
                    buttons[i, j].Text = Convert.ToString(buttons[i, j].Tag);
                    buttons[i, j].BackColor = Color.FromArgb(29, 162, 216); ;
                    buttons[i, j].FlatStyle = FlatStyle.Flat;
                    buttons[i, j].FlatAppearance.BorderSize = 0;
                    buttons[i, j].Click += Button_Click;
                    this.Controls.Add(buttons[i, j]);
                }
            }

            griglia = new Button[gridSize, gridSize];

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    griglia[i, j] = new Button();
                    griglia[i, j].Width = 35;
                    griglia[i, j].Height = 35;
                    griglia[i, j].Top = 50 + (i * 35);
                    griglia[i, j].Left = 600 + (j * 35);
                    griglia[i, j].Tag = $"{(char)('A' + i)}{j + 1}";
                    griglia[i, j].Text = "";
                    griglia[i, j].FlatStyle = FlatStyle.Flat;
                    griglia[i, j].FlatAppearance.BorderSize = 0;
                    this.Controls.Add(griglia[i, j]);
                }
            }

            Label LabelDati = new Label
            {
                Name = "LabelDati",
                Top = gridSize * 45 + 100,
                Left = 50,
                Width = 280,
                Height = 22,
                Text = "Attendi il tuo turno...",
                Font = new Font("Arial", 14),
                BackColor = Color.Transparent,
            };
            this.Controls.Add(LabelDati);

            Sfondo = new PictureBox();
            Sfondo.Dock = DockStyle.Fill;
            Sfondo.Image = getImageFromAssets("sfondo.gif");
            Sfondo.SizeMode = PictureBoxSizeMode.StretchImage;
            this.Controls.Add(Sfondo);            
        }
        //funzione che aggiorna le label con lo sfondo animato
        private void AggiornaLabels()
        {
            LabelRisultati.Parent = Sfondo;
            LabelDati.Parent = Sfondo;
            label1.Parent = Sfondo;
            label2.Parent = Sfondo;
            label3.Parent = Sfondo;
            LabelRisultati.ForeColor = Color.White;
            LabelDati.ForeColor = Color.White;
            label1.ForeColor = Color.White;
            label2.ForeColor = Color.White;
            label3.ForeColor = Color.White;
        }
        //funzione per ottenere la tabella del proprio campo all'inizio
        private void GetGrids()
        {
            byte[] bufferGrid = new byte[1024];
            int bytesRead = stream.Read(bufferGrid, 0, bufferGrid.Length);
            string json = Encoding.UTF8.GetString(bufferGrid, 0, bytesRead);
            grid = JsonConvert.DeserializeObject<string[,]>(json);
        }
        //funzione che utilizza la libreria System.IO per ottenere la path degli assets del progetto
        Image getImageFromAssets(string imageName)
        {
            return Image.FromFile(Path.GetDirectoryName(Path.GetDirectoryName(Application.StartupPath)) + "/assets/" + imageName);
        }
        //funzione che aggiorna i colori del proprio campo 
        private void AggiornaGrigliaPulsanti(string[,] datiRicevuti, Button[,] grigliaPulsanti)
        {
            int righe = datiRicevuti.GetLength(0);
            int colonne = datiRicevuti.GetLength(1);

            for (int i = 0; i < righe; i++)
            {
                for (int j = 0; j < colonne; j++)
                {
                    if (datiRicevuti[i, j] == "S")
                    {
                        grigliaPulsanti[i, j].BackColor = Color.FromArgb(29, 162, 216);
                    }
                    else if (datiRicevuti[i, j] == "N")
                    {
                        grigliaPulsanti[i, j].BackColor = Color.FromArgb(29, 162, 216);
                        grigliaPulsanti[i, j].BackgroundImage = getImageFromAssets("waves.png");
                        grigliaPulsanti[i, j].BackgroundImageLayout = ImageLayout.Stretch;
                    }
                    else if (datiRicevuti[i, j] == "O")
                    {
                        grigliaPulsanti[i, j].BackColor = Color.RoyalBlue;
                        grigliaPulsanti[i, j].BackgroundImage = getImageFromAssets("waves.png");
                        grigliaPulsanti[i, j].BackgroundImageLayout = ImageLayout.Stretch;
                    }
                    else if (datiRicevuti[i, j] == "X")
                    {
                        grigliaPulsanti[i, j].BackgroundImage = getImageFromAssets("explosion.png");
                        grigliaPulsanti[i, j].BackgroundImageLayout = ImageLayout.Stretch;
                    }
                }
            }
        }
        //override della funzione che aggiorna le immagini sulla griglia, questa aggiunge le navi e le orienta
        private void AggiornaGrigliaPulsanti(string[,] datiRicevuti, Button[,] grigliaPulsanti, List<(int, int, int, int, int, string)> shipDetails)
        {
            int righe = datiRicevuti.GetLength(0);
            int colonne = datiRicevuti.GetLength(1);

            for (int i = 0; i < righe; i++)
            {
                for (int j = 0; j < colonne; j++)
                {
                    if (datiRicevuti[i, j] == "S")
                    {
                        grigliaPulsanti[i, j].BackColor = Color.FromArgb(29, 162, 216);
                    }
                    else if (datiRicevuti[i, j] == "N")
                    {
                        grigliaPulsanti[i, j].BackColor = Color.FromArgb(29, 162, 216);
                        grigliaPulsanti[i, j].BackgroundImage = getImageFromAssets("waves.png");
                        grigliaPulsanti[i, j].BackgroundImageLayout = ImageLayout.Stretch;
                    }
                    else if (datiRicevuti[i, j] == "O")
                    {
                        grigliaPulsanti[i, j].BackColor = Color.RoyalBlue;
                        grigliaPulsanti[i, j].BackgroundImage = getImageFromAssets("waves.png");
                        grigliaPulsanti[i, j].BackgroundImageLayout = ImageLayout.Stretch;
                    }
                    else if (datiRicevuti[i, j] == "X")
                    {
                        grigliaPulsanti[i, j].BackgroundImage = getImageFromAssets("explosion.png");
                        grigliaPulsanti[i, j].BackgroundImageLayout = ImageLayout.Stretch;
                    }
                }
            }

            foreach ((int, int, int, int, int, string) ship in shipDetails)
            {
                int size = ship.Item5;
                if (size == 1)
                {
                    int row = ship.Item1;
                    int col = ship.Item2;
                    string direction = ship.Item6;
                    if (direction == "horizontal")
                    {
                        grigliaPulsanti[row, col].BackgroundImage = getImageFromAssets("NaveDa1/fronte.png");
                    }
                    else //ruoto l'immagine in modo che sia verticale
                    {
                        grigliaPulsanti[row, col].BackgroundImage = getImageFromAssets("NaveDa1/fronte.png");
                        grigliaPulsanti[row, col].BackgroundImage.RotateFlip(RotateFlipType.Rotate270FlipNone); 
                    }
                    grigliaPulsanti[row, col].BackgroundImageLayout = ImageLayout.Stretch;
                }
                else if (size == 2)
                {
                    int rowFront = ship.Item1;
                    int colFront = ship.Item2;
                    int rowBack = ship.Item3;
                    int colBack = ship.Item4;
                    string direction = ship.Item6;

                    if (direction == "horizontal") //qua scambio fronte e centro perchè l'algoritmo vede il davanti della nave come la parte più a sinistra, mentre l'immagine del fronte punta verso destra
                    {
                        grigliaPulsanti[rowFront, colFront].BackgroundImage = getImageFromAssets("NaveDa2/centro.png"); 
                        grigliaPulsanti[rowBack, colBack].BackgroundImage = getImageFromAssets("NaveDa2/fronte.png");
                    }
                    else //qua non c'è bisogno di scambiare perchè l'algoritmo vede il davanti della nave come la parte più in alto
                    {
                        grigliaPulsanti[rowFront, colFront].BackgroundImage = getImageFromAssets("NaveDa2/fronte.png"); 
                        grigliaPulsanti[rowFront, colFront].BackgroundImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        grigliaPulsanti[rowBack, colBack].BackgroundImage = getImageFromAssets("NaveDa2/centro.png");
                        grigliaPulsanti[rowBack, colBack].BackgroundImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    }
                    grigliaPulsanti[rowFront, colFront].BackgroundImageLayout = ImageLayout.Stretch;
                    grigliaPulsanti[rowBack, colBack].BackgroundImageLayout = ImageLayout.Stretch;
                }
                else
                {
                    int rowFront = ship.Item1;
                    int colFront = ship.Item2;
                    int rowBack = ship.Item3;
                    int colBack = ship.Item4;
                    int rowMiddle = (rowFront + rowBack) / 2;
                    int colMiddle = (colFront + colBack) / 2;
                    string direction = ship.Item6;

                    if (direction == "horizontal") //anche qui fronte e retro sono scambiati a causa di come l'algoritmo rileva avanti e dietro
                    {
                        grigliaPulsanti[rowFront, colFront].BackgroundImage = getImageFromAssets("NaveDa3/retro.png"); 
                        grigliaPulsanti[rowMiddle, colMiddle].BackgroundImage = getImageFromAssets("NaveDa3/centro.png");
                        grigliaPulsanti[rowBack, colBack].BackgroundImage = getImageFromAssets("NaveDa3/fronte.png");
                    }
                    else
                    {
                        grigliaPulsanti[rowFront, colFront].BackgroundImage = getImageFromAssets("NaveDa3/fronte.png");
                        grigliaPulsanti[rowFront, colFront].BackgroundImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        grigliaPulsanti[rowMiddle, colMiddle].BackgroundImage = getImageFromAssets("NaveDa3/centro.png");
                        grigliaPulsanti[rowMiddle, colMiddle].BackgroundImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        grigliaPulsanti[rowBack, colBack].BackgroundImage = getImageFromAssets("NaveDa3/retro.png");
                        grigliaPulsanti[rowBack, colBack].BackgroundImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    }
                    grigliaPulsanti[rowFront, colFront].BackgroundImageLayout = ImageLayout.Stretch;
                    grigliaPulsanti[rowMiddle, colMiddle].BackgroundImageLayout = ImageLayout.Stretch;
                    grigliaPulsanti[rowBack, colBack].BackgroundImageLayout = ImageLayout.Stretch;
                }
            }
        }

        //funzione per ottenere informazioni utili sulle navi per posizionare le immagini
        private List<(int, int, int, int, int, string)> GetShipDetails(string[,] grid)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);
            bool[,] visited = new bool[rows, cols];
            var shipDetails = new List<(int, int, int, int, int, string)>();

            (int size, int backRow, int backCol) ExploreShip(int frontRow, int frontCol)
            {
                visited[frontRow, frontCol] = true;
                int size = 1;
                int row = frontRow, col = frontCol;

                if (col + 1 < cols && grid[row, col + 1] == "S" && !visited[row, col + 1])
                {
                    while (col + 1 < cols && grid[row, col + 1] == "S")
                    {
                        visited[row, ++col] = true;
                        size++;
                    }
                    return (size, row, col);
                }
                if (row + 1 < rows && grid[row + 1, col] == "S" && !visited[row + 1, col])
                {
                    while (row + 1 < rows && grid[row + 1, col] == "S")
                    {
                        visited[++row, col] = true;
                        size++;
                    }
                    return (size, row, col); 
                }
                return (size, row, col);
            }
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (grid[row, col] == "S" && !visited[row, col])
                    {
                        var (size, backRow, backCol) = ExploreShip(row, col);
                        string orientation = (row == backRow) ? "horizontal" : "vertical";
                        shipDetails.Add((row, col, backRow, backCol, size, orientation));
                    }
                }
            }
            return shipDetails;
        }
        //funzione per creare il client e stabilire la connessione con esso
        private void ConnectToServer()
        {
            try
            {
                if (client != null && client.Connected)
                {
                    MessageBox.Show("Già connesso al server.");
                    return;
                }

                client = new TcpClient("127.0.0.1", 50000);
                stream = client.GetStream();

                GetGrids();
                AggiornaGrigliaPulsanti(grid, griglia, GetShipDetails(grid));
                trRicevi = new Thread(Ricevi_Risposte);
                trRicevi.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore: " + ex.Message);
            }
        }

        //thread di background che gestisce la ricezione di messaggi
        private void Ricevi_Risposte()
        {
            delegato del1 = new delegato(ChiudiForm);
            delegato del2 = new delegato(ScriviDato1);
            delegato del3 = new delegato(ScriviDato2);
            delegato3 del4 = new delegato3(ScriviChat);
            while (true)
            {
                try
                {
                    byte[] rispostaByte = new byte[1024];
                    int lunghezza = stream.Read(rispostaByte, 0, rispostaByte.Length);
                    risposta = Encoding.UTF8.GetString(rispostaByte, 0, lunghezza);

                    grid = JsonConvert.DeserializeObject<string[,]>(risposta);
                    AggiornaGrigliaPulsanti(grid, griglia);
                } 
                catch { }
                
                string rispostaMessaggio = risposta.Replace("MESSAGGIO|", "");
                if (rispostaMessaggio != risposta) 
                { 
                    Invoke(del4, rispostaMessaggio); 
                }
                else if (risposta == "Hai vinto!!!" || risposta == "Hai perso!!!") 
                {
                    Invoke(del1); 
                } 
                else if (risposta == "Colpo gia' effettuato! Ripeti." || risposta == "Mancato!" || risposta == "Colpito!")
                {
                    Invoke(del3);
                }
                else if (risposta == "Benvenuto! E' il tuo turno." || risposta == "E' il tuo turno." || risposta == "Benvenuto! Attendi il tuo turno..." || risposta == "Attendi il tuo turno...")
                {
                    Invoke(del2);
                }
            }
        }

        //invio di un input della griglia di pulsanti
        private void Button_Click(object sender, EventArgs e)
        {
            if (LabelRisultati.Text == "Colpo gia' effettuato! Ripeti." || LabelDati.Text == "Attendi il tuo turno..." || LabelDati.Text == "Benvenuto! Attendi il tuo turno...")
            {
                MessageBox.Show(risposta);
                LabelRisultati.Text = "";
                return;
            }
            tr = new Thread(ControllaColpo);
            tr.Start(sender);
        }
        //thread che manda i risultati del colpo e avvia il delegato per i colori
        private void ControllaColpo(object sender)
        {
            delegato2 del1 = new delegato2(CambiaColore);
            Button button = (Button)sender;
            string colpo = button.Tag.ToString();
            byte[] buffer = Encoding.UTF8.GetBytes(colpo);
            stream.Write(buffer, 0, buffer.Length);
            Thread.Sleep(100);
            Invoke(del1, sender);
        }
        //cambio colore della griglia di pulsanti
        private void CambiaColore(object sender)
        {
            Button button = (Button)sender;
            button.Enabled = false;
            if (LabelRisultati.Text == "Mancato!") { button.BackColor = Color.RoyalBlue; }
            else if (LabelRisultati.Text == "Colpito!") { button.BackColor = Color.Red; }
        }
        //funzioni di gestione risposte (utilizzate dai delegati)
        private void ChiudiForm()
        {
            LabelDati.Text = risposta;
            MessageBox.Show(this, risposta);
            this.Close();
            System.Environment.Exit(0);
        }
        private void ScriviDato1()
        { 
            LabelDati.Text = risposta;
        }
        private void ScriviDato2()
        {
            LabelRisultati.Text = risposta;
        }

        //funzioni per scrivere un messaggio in chat
        private void ScriviChat(string rispostaMessaggio)
        {
            string messaggio = $"{rispostaMessaggio.Split('|')[0]} {rispostaMessaggio.Replace($"{rispostaMessaggio.Split('|')[0]}|", "")}";
            listBoxMessaggi.Items.Add(messaggio);
            listBoxMessaggi.SelectedIndex = listBoxMessaggi.Items.Count - 1;
            listBoxMessaggi.SelectedIndex = -1;
        }
        //funzione collegata all'invio dei messaggi in console
        private void textBoxMessaggi_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true; //sopprimo i suoni di windows
                if (textBoxMessaggi.Text != "")
                {
                    string messaggino = $"MESSAGGIO|{textBoxMessaggi.Text}";
                    byte[] buffer = Encoding.UTF8.GetBytes(messaggino);
                    stream.Write(buffer, 0, buffer.Length);
                    textBoxMessaggi.Text = "";
                }
            }
        }
        private void textBoxMessaggi_Enter(object sender, EventArgs e)
        {
            if (textBoxMessaggi.Text == "Scrivi un messaggio...")
            {
                textBoxMessaggi.Text = "";   
            }
            textBoxMessaggi.ForeColor = Color.Black;
        }
        private void textBoxMessaggi_Leave(object sender, EventArgs e)
        {
            if (textBoxMessaggi.Text == "")
            {
                textBoxMessaggi.Text = "Scrivi un messaggio...";
                textBoxMessaggi.ForeColor = Color.LightGray;
            }
        }

        //funzioni di apertura e chiusura della form
        private void Form1_Load(object sender, EventArgs e)
        {
            ConnectToServer();
        }
        private void Form1_Closing(object sender, FormClosingEventArgs e)
        {
            if (client != null)
            {
                client.Close();
            }
        }
    }
}