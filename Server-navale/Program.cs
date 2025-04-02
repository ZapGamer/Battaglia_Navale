using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server_navale
{
    internal class Program
    {
        private static TcpListener server;
        private static TcpClient client1, client2;
        private static NetworkStream stream1, stream2;
        private static int count1 = 0, count2 = 0;
        private static bool isPlayer1Turn = true;  
        private static bool gameStarted = false;
        private static string[,] grid1 = new string[10, 10];  
        private static string[,] grid2 = new string[10, 10];  
        private static Random rand = new Random();

        static void Main(string[] args)
        {
            StartServer();
        }
        //funzione per stampare le griglie su console
        private static void PrintGrid(string[,] grid, int n)
        {
            Console.WriteLine($"Griglia {n}:");
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    Console.Write(grid[row, col] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();  
        }
        //funzione principale che attende la connessione dei client
        static void StartServer()
        {
            server = new TcpListener(IPAddress.Any, 50000);
            server.Start();
            Console.WriteLine("Server in ascolto...");

            client1 = server.AcceptTcpClient();  
            Console.WriteLine("Giocatore 1 connesso.");

            client2 = server.AcceptTcpClient();  
            Console.WriteLine("Giocatore 2 connesso.");

            stream1 = client1.GetStream();
            stream2 = client2.GetStream();

            InitializeGrid(grid1);
            InitializeGrid(grid2);
            AggiornaTabelle();

            PrintGrid(grid1, 1);
            PrintGrid(grid2, 2);

            gameStarted = true;
            Console.WriteLine("Server Startato!\n");
            StartGame();
        }
        //funzione per spedire ai client le tabelle aggiornate
        static void AggiornaTabelle()
        {
            string json1 = JsonConvert.SerializeObject(grid1);
            string json2 = JsonConvert.SerializeObject(grid2);

            SendMessage(stream1, json1);
            SendMessage(stream2, json2);
        }
        //funzione principale dove vengono startati i due thread (per ogni client)
        static void StartGame()
        {
            Thread player1Thread = new Thread(() => HandlePlayerTurn(stream1, stream2, 1));
            Thread player2Thread = new Thread(() => HandlePlayerTurn(stream2, stream1, 2));

            SendMessage(stream1, "Benvenuto! E' il tuo turno.");
            SendMessage(stream2, "Benvenuto! Attendi il tuo turno...");
            Console.WriteLine("Player connessi!");
           
            player1Thread.Start();
            player2Thread.Start();
        }
        //funzione che gestisce il gioco
        static void HandlePlayerTurn(NetworkStream stream, NetworkStream stream_opp, int playerNumber)
        {
            while (gameStarted)
            {
                string playerMove = ReceiveMove(stream);
                if (string.IsNullOrEmpty(playerMove)) continue;

                string result = ProcessMove(playerMove, playerNumber); 
                if (result.Split('|')[0] == "MESSAGGIO") //controllo per stabilire se il messaggio ricevuto è una mossa o un messaggio della chat
                {
                    result = $"Giocatore {playerNumber}:|" + result;
                    SendMessage(stream, result);
                    SendMessage(stream_opp, result); 
                } 
                else { SendMessage(stream, result); }

                //i due controlli per stabilire il vincitore
                if(count1 == 10)
                {
                    AggiornaTabelle();
                    Thread.Sleep(100);
                    SendMessage(stream1, "Hai vinto!!!");
                    SendMessage(stream2, "Hai perso!!!");
                    count1 = 0;
                    count2 = 0;
                    gameStarted = false;
                } 
                else if (count2 == 10)
                {
                    AggiornaTabelle();
                    Thread.Sleep(100);
                    SendMessage(stream2, "Hai vinto!!!");
                    SendMessage(stream1, "Hai perso!!!");
                    count1 = 0;
                    count2 = 0;
                    gameStarted = false;
                }

                //aggiorna le tabelle se viene confermato il messaggio come mossa
                if((result == "Mancato!" || result == "Colpito!") && gameStarted)
                {
                    isPlayer1Turn = !isPlayer1Turn; //cambio dei turni
                    AggiornaTabelle();
                    Thread.Sleep(100);
                    if (isPlayer1Turn)
                    {
                        SendMessage(stream1, "E' il tuo turno.");
                        SendMessage(stream2, "Attendi il tuo turno...");
                    }
                    else
                    {
                        SendMessage(stream2, "E' il tuo turno.");
                        SendMessage(stream1, "Attendi il tuo turno...");
                    }
                }
                Console.WriteLine();
            }
        }
        //funzione che riceve messaggi dai client
        static string ReceiveMove(NetworkStream stream)
        {
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            return Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();
        }
        //funzione che controlla la risposta e restituisce un risultato se è una mossa
        static string ProcessMove(string move, int playerNumber)
        {
            string[,] opponentGrid = (playerNumber == 1) ? grid2 : grid1;
            if (move.Replace("MESSAGGIO|", "") == move) {
                int row = move[0] - 'A';
                int col = int.Parse(move.Substring(1)) - 1;

                if (opponentGrid[row, col] == "X" || opponentGrid[row, col] == "O")
                {
                    return "Colpo gia' effettuato! Ripeti.";
                }

                if (opponentGrid[row, col] == "N")
                {
                    opponentGrid[row, col] = "O";
                    return "Mancato!";
                }
                else if (opponentGrid[row, col] == "S")
                {
                    opponentGrid[row, col] = "X";
                    if (playerNumber == 1) { ++count1; }
                    else if (playerNumber == 2) { ++count2; }
                    return "Colpito!";
                }
                return "Mancato"; 
            }
            else {
                return move;
            }
        }
        //funzione che spedisce messaggi ai client e li stampa a console
        static void SendMessage(NetworkStream stream, string message)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            stream.Write(buffer, 0, buffer.Length);
            Console.WriteLine($"Messaggio inviato: {message}");
        }
        //funzione che inizializza le due grid dei giocatori
        static void InitializeGrid(string[,] grid)
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    grid[i, j] = "N";
                }
            }
            PositionShips(grid);
        }
        //funzione per posizionare le navi sulle grid
        static void PositionShips(string[,] grid)
        {
            int[] shipLengths = { 1, 1, 1, 2, 2, 3 };
            bool shipPlaced = false;

            bool CanPlaceShip(int row, int col, int length, bool isHorizontal) //funzione che verifica se si può piazzare in una cella una parte di nave
            {
                if (isHorizontal)
                {
                    if (col + length > 10) return false;
                    for (int i = -1; i <= length; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            int checkRow = row + j;
                            int checkCol = col + i;
                            if (checkRow >= 0 && checkRow < 10 && checkCol >= 0 && checkCol < 10)
                            {
                                if (i >= 0 && i < length && j == 0 && grid[row, col + i] != "N") { return false; }
                                if (grid[checkRow, checkCol] != "N") { return false; }
                            }
                        }
                    }
                }
                else
                {
                    if (row + length > 10) return false;
                    for (int i = -1; i <= length; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            int checkRow = row + i;
                            int checkCol = col + j;
                            if (checkRow >= 0 && checkRow < 10 && checkCol >= 0 && checkCol < 10)
                            {
                                if (i >= 0 && i < length && j == 0 && grid[row + i, col] != "N") { return false; }
                                if (grid[checkRow, checkCol] != "N") { return false; }
                            }
                        }
                    }
                }
                return true;
            }
            void PlaceShip(int row, int col, int length, bool isHorizontal) //funzione che piazza la nave
            {
                if (isHorizontal)
                {
                    for (int i = 0; i < length; i++)
                    {
                        grid[row, col + i] = "S";
                    }
                }
                else
                {
                    for (int i = 0; i < length; i++)
                    {
                        grid[row + i, col] = "S";
                    }
                }
            }
            foreach (int shipLength in shipLengths) //qui si sceglie la posizione e la direzione in maniera randomica
            {
                shipPlaced = false;
                while (!shipPlaced)
                {
                    int row = rand.Next(0, 10);
                    int col = rand.Next(0, 10);
                    bool isHorizontal = rand.Next(0, 2) == 0;
                    if (CanPlaceShip(row, col, shipLength, isHorizontal))
                    {
                        PlaceShip(row, col, shipLength, isHorizontal);
                        shipPlaced = true;
                    }
                }
            }
        }

    }
}