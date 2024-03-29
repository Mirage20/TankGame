using System;
using System.Collections.Generic;
using System.Text;

using System.Threading;
using System.Drawing;
using System.IO;
using System.Configuration;

using lk.ac.mrt.cse.pc11.bean;
using lk.ac.mrt.cse.pc11.util;
using lk.ac.mrt.cse.pc11.AI;
using GUI;

namespace lk.ac.mrt.cse.pc11
{
    #region "Delegates"
    //Delegate to signal the start event.
    public delegate void GameStartingDelegate(object sender, EventArgs e);
    //Delegate to signal the game has started event.
    public delegate void GameJustStartedDelegate(object sender, EventArgs e);
    //Delegate to signal the game has finished event.
    public delegate void GameOverDelegate(object sender, EventArgs e);
    #endregion

    class GameEngine
    {

        #region Variables

        private static volatile List<Contestant> playerList;
        private static volatile List<string> playerIPList;
        private static volatile List<int> playerPort;


        private static volatile List<CoinPile> coinPileList;
        private static volatile List<CoinPile> availableCoinPileList;
        private static volatile List<CoinPile> disappearCoinPileList;
        private static int coinPilesToDisclose = -1;
        private static int nextCoinPileSend = 0;

        private static volatile List<LifePack> lifePackList;
        private static volatile List<LifePack> availableLifePackList;
        private static volatile List<LifePack> disappearLifePackList;
        private static int lifePacksToDisclose = -1;
        private static int nextLifePackSend = 0;

        private static volatile List<CoinPile> plunderCoinPileList;



        private static List<Point> obstacles;
        private static List<Point> brickLocations;
        private static List<BrickWall> brickWalls;
        private static List<Point> water;
        private static List<Bullet> activeBullets;

        private static int maxPlayerNumber;
        private int minPlayerNumber = 1;
        private static int mapSize;
        private static string mapDetails;
        private static int obstaclePenalty;

        private static bool gameStarted = false;
        private static bool gameFinished = false;

        private static Communicator com = null;
        private static GameEngine gameEng = new GameEngine();

        #endregion

        #region "Event Related"

        //Event to notify that the game should start.
        public event GameStartingDelegate startEvent;
        //Event to notify that the game has started.
        public event GameJustStartedDelegate justStartedEvent;
        //Event to notify that the game has finished.
        public event GameOverDelegate gameOverEvent;

        private void RaiseGameStartingEvent()
        {
            // Safely invoke an event:
            GameStartingDelegate temp = startEvent;
            if (temp != null)
            {
                temp(this.startEvent, new EventArgs());
            }
        }

        private void RaiseGameJustStartedEvent()
        {
            // Safely invoke an event:
            GameJustStartedDelegate temp = justStartedEvent;
            if (temp != null)
            {
                temp(this.justStartedEvent, new EventArgs());
            }

        }

        private void RaiseGameOverEvent()
        {
            GameOverDelegate temp = gameOverEvent;
            if (temp != null)
                temp(this.gameOverEvent, new EventArgs());
        }

        #endregion

        #region "Properties"

        public List<Contestant> PlayerList
        {
            get { return GameEngine.playerList; }
        }

        public List<CoinPile> CoinPileList
        {
            get { return GameEngine.coinPileList; }
        }

        public List<CoinPile> AvailableCoinPileList
        {
            get { return GameEngine.availableCoinPileList; }
            set { GameEngine.availableCoinPileList = value; }
        }

        public List<CoinPile> DisappearCoinPileList
        {
            get { return GameEngine.disappearCoinPileList; }
        }

        public int NextCoinPileSend
        {
            get { return nextCoinPileSend; }
            set { nextCoinPileSend = value; }
        }

        public int CoinPilesToDisclose
        {
            get { return GameEngine.coinPilesToDisclose; }
        }

        public List<LifePack> LifePackList
        {
            get { return GameEngine.lifePackList; }
        }

        public List<LifePack> AvailableLifePackList
        {
            get { return GameEngine.availableLifePackList; }
            set { GameEngine.availableLifePackList = value; }
        }

        public List<LifePack> DisappearLifePackList
        {
            get { return GameEngine.disappearLifePackList; }
        }

        public int NextLifePackSend
        {
            get { return GameEngine.nextLifePackSend; }
            set { GameEngine.nextLifePackSend = value; }
        }

        public int LifePacksToDisclose
        {
            get { return GameEngine.lifePacksToDisclose; }
        }

        public List<CoinPile> PlunderCoinPileList
        {
            get { return GameEngine.plunderCoinPileList; }
            set { GameEngine.plunderCoinPileList = value; }
        }

        public bool GameFinished
        {
            get { return gameFinished; }
        }

        #endregion

        public static GameEngine GetInstance()
        {
            return gameEng;
        }

        private GameEngine()
        {
            GameEngine.brickLocations = new List<Point>();
            GameEngine.obstacles = new List<Point>();
            GameEngine.water = new List<Point>();
            GameEngine.brickWalls = new List<BrickWall>();
            GameEngine.activeBullets = new List<Bullet>();

            GameEngine.coinPileList = new List<CoinPile>();
            GameEngine.availableCoinPileList = new List<CoinPile>();
            GameEngine.disappearCoinPileList = new List<CoinPile>();

            GameEngine.lifePackList = new List<LifePack>();
            GameEngine.availableLifePackList = new List<LifePack>();
            GameEngine.disappearLifePackList = new List<LifePack>();

            GameEngine.plunderCoinPileList = new List<CoinPile>();

            GameEngine.playerIPList = new List<string>();
            GameEngine.playerPort = new List<int>();
            GameEngine.playerList = new List<Contestant>();
            GameEngine.com = Communicator.GetInstance();

            joinTim.Elapsed += new System.Timers.ElapsedEventHandler(joinTim_Elapsed);


            this.Initialize();
            this.createMap();
            // gui.InitializeMap(Constant.MAP_SIZE, GameEngine.mapDetails);
        }

        private List<String> consoleLines = new List<String>();

        private void consoleLinesDoProgress()
        {
            consoleLines[0] = consoleLines[0] + ".";
        }

        private void writeConsoleLines(String s, int i)
        {
            while (consoleLines.Count <= i)
            {
                writeConsoleLines("");
            }
            consoleLines[i] = s;
            writeConsoleLines();
        }

        private void writeConsoleLines(String s)
        {
            consoleLines.Add(s);
            writeConsoleLines();
        }

        private void writeConsoleLines()
        {
            Console.Clear();
            foreach (string s in consoleLines)
            {
                Console.WriteLine(s);
            }
        }

        private void createMap()
        {
            try
            {
                StreamReader sr = new StreamReader(ConfigurationManager.AppSettings.Get("MapPath"));
                String line;
                List<String> readInputs = new List<String>();
                while ((line = sr.ReadLine()) != null)
                {
                    readInputs.Add(line);
                }
                GameEngine.mapDetails = "";
                if (readInputs.Count != 0)
                {
                    GameEngine.mapSize = int.Parse(readInputs[0]);
                    GameEngine.mapDetails += readInputs[0] + ":" + readInputs[1] + ":" + readInputs[2];
                    Point tempPoint;
                    string[] tokens = readInputs[1].Split(new char[] { ';' });
                    for (int i = 0; i < tokens.Length; i++)
                    {
                        tempPoint = new Point(int.Parse(tokens[i].Substring(0, tokens[i].IndexOf(","))),
                            int.Parse(tokens[i].Substring(tokens[i].IndexOf(",") + 1, tokens[i].Length - tokens[i].IndexOf(",") - 1)));
                        GameEngine.brickLocations.Add(tempPoint);
                    }

                    tokens = readInputs[2].Split(new char[] { ';' });
                    for (int i = 0; i < tokens.Length; i++)
                    {
                        tempPoint = new Point(int.Parse(tokens[i].Substring(0, tokens[i].IndexOf(","))),
                            int.Parse(tokens[i].Substring(tokens[i].IndexOf(",") + 1, tokens[i].Length - tokens[i].IndexOf(",") - 1)));
                        GameEngine.obstacles.Add(tempPoint);
                    }
                    CoinPile tempCoinPile;
                    for (int i = 3; i < readInputs.Count; i++)
                    {
                        tokens = readInputs[i].Split(new char[] { ',', ';' });
                        tempCoinPile = new CoinPile(int.Parse(tokens[0]), int.Parse(tokens[1]));
                        tempCoinPile.AppearTime = int.Parse(tokens[2]);
                        tempCoinPile.LifeTime = int.Parse(tokens[3]);
                        tempCoinPile.Price = int.Parse(tokens[4]);
                        tempCoinPile.DisappearTime = tempCoinPile.AppearTime + tempCoinPile.LifeTime;
                        GameEngine.coinPileList.Add(tempCoinPile);
                    }
                }
                sortCoinPileLists();
            }
            catch (Exception ex1)
            {
                try
                {
                    writeConsoleLines("Generating the map");
                    GameEngine.mapSize = Constant.MAP_SIZE;
                    consoleLinesDoProgress();
                    RandomGen r = new RandomGen();
                    consoleLinesDoProgress();
                    // GameEngine.mapDetails = mapSize + ":";
                    consoleLinesDoProgress();
                    int maxBrick = Constant.MAX_BRICKS;
                    if (maxBrick < 5)
                    {
                        maxBrick = 5;
                        writeConsoleLines("Didn't I tell you to put at least 5? I am taking it as 5");

                    }
                    consoleLinesDoProgress();
                    int brickCount = r.randomD(maxBrick / 2, maxBrick);
                    consoleLinesDoProgress();
                    int lineCount = consoleLines.Count;
                    for (int i = 0; i < brickCount; i++)
                    {

                        int x = r.randomD(mapSize - 1);
                        int y = r.randomDP(mapSize - 1, mapSize, x);
                        Point p = new Point(x, y);
                        if (!brickLocations.Contains(p))
                        {
                            GameEngine.brickLocations.Add(p);
                            if (i != 0)
                            {
                                GameEngine.mapDetails = GameEngine.mapDetails + ";";
                            }
                            GameEngine.mapDetails = GameEngine.mapDetails + x + "," + y;
                            writeConsoleLines("Bricks:- " + (i + 1) + "/" + brickCount + " created", lineCount);
                        }
                        else
                        {
                            i--;
                        }

                    }
                    GameEngine.mapDetails = GameEngine.mapDetails + ":";

                    int maxObs = Constant.MAX_OBSTACLES;
                    int minObs = 8;
                    consoleLinesDoProgress();
                    if (maxObs < 8)
                    {
                        maxObs = 8;
                        writeConsoleLines("Didn't I tell you to put at least 8? I am taking it as 8");
                    }
                    if (minObs < brickCount)//More obstacals than bricks needed
                    {
                        minObs = brickCount;
                    }
                    if (minObs > maxObs)//But keep in the possible region
                    {
                        minObs = maxObs;
                    }
                    consoleLinesDoProgress();
                    int obsCount = r.randomD(minObs, maxObs);
                    lineCount = consoleLines.Count;

                    for (int i = 0; i < obsCount; i++)
                    {
                        int x = r.randomD(mapSize - 1);
                        int y = r.randomDP(mapSize - 1, mapSize, x);
                        Point p = new Point(x, y);
                        if ((!brickLocations.Contains(p)) & (!obstacles.Contains(p)))
                        {
                            GameEngine.obstacles.Add(p);
                            if (i != 0)
                            {
                                GameEngine.mapDetails = GameEngine.mapDetails + ";";
                            }
                            GameEngine.mapDetails = GameEngine.mapDetails + x + "," + y;
                            writeConsoleLines("Stone :- " + (i + 1) + "/" + obsCount + " created", lineCount);
                        }
                        else
                        {
                            i--;
                        }
                    }
                    GameEngine.mapDetails = GameEngine.mapDetails + ":";

                    consoleLinesDoProgress();
                    int waterCount = 10;
                    lineCount = consoleLines.Count;
                    for (int i = 0; i < waterCount; i++)
                    {

                        int x = r.randomD(mapSize - 1);
                        int y = r.randomDP(mapSize - 1, mapSize, x);
                        Point p = new Point(x, y);
                        if ((!brickLocations.Contains(p)) & (!obstacles.Contains(p)) & (!water.Contains(p)))
                        {
                            GameEngine.water.Add(p);
                            if (i != 0)
                            {
                                GameEngine.mapDetails = GameEngine.mapDetails + ";";
                            }
                            GameEngine.mapDetails = GameEngine.mapDetails + x + "," + y;
                            writeConsoleLines("water:- " + (i + 1) + "/" + waterCount + " created", lineCount);
                        }
                        else
                        {
                            i--;
                        }
                    }



                    //Create brick walls
                    for (int i = 0; i < brickLocations.Count; i++)
                    {
                        brickWalls.Add(new BrickWall(brickLocations[i]));
                    }



                    int tresCount = (Constant.CoinPile_RATE) * (Constant.LIFE_TIME / 60000);// CoinPiles for every 1 min
                    consoleLinesDoProgress();
                    MapItem tempMapItem;
                    consoleLinesDoProgress();



                    List<Point> points = CreatePointList(tresCount);

                    lineCount = consoleLines.Count;

                    for (int i = 0; i < Math.Min(tresCount, points.Count); i++)
                    {

                        try
                        {
                            tempMapItem = CreateMapItem(points[i], true);

                            List<MapItem> conflictPoints = new List<MapItem>();

                            for (int j = 0; j < CoinPileList.Count; j++)
                            {
                                conflictPoints.Add(CoinPileList[j]);
                            }



                            Boolean pass = GetMapItemAcceptance(tempMapItem, conflictPoints);


                            if (pass)
                            {
                                GameEngine.coinPileList.Add((CoinPile)tempMapItem);
                                writeConsoleLines("CoinPiles :- " + (i + 1) + "/" + Math.Min(tresCount, points.Count) + " created", lineCount);
                            }
                            else
                            {
                                i--;//Recalculate
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("...............................");
                            Console.WriteLine(e.StackTrace);
                            Console.WriteLine("...............................");
                        }
                    }



                    int lifePackCount = (Constant.LIFEPACK_RATE) * (Constant.LIFE_TIME / 60000);// CoinPiles for every 1 min
                    consoleLinesDoProgress();




                    points = CreatePointList(lifePackCount);

                    lineCount = consoleLines.Count;

                    for (int i = 0; i < Math.Min(lifePackCount, points.Count); i++)
                    {
                        try
                        {
                            tempMapItem = CreateMapItem(points[i], false);

                            List<MapItem> conflictPoints = new List<MapItem>();

                            for (int j = 0; j < CoinPileList.Count; j++)
                            {
                                conflictPoints.Add(CoinPileList[j]);
                            }
                            for (int j = 0; j < lifePackList.Count; j++)
                            {
                                conflictPoints.Add(lifePackList[j]);
                            }

                            Boolean pass = GetMapItemAcceptance(tempMapItem, conflictPoints);


                            if (pass)
                            {
                                GameEngine.lifePackList.Add((LifePack)tempMapItem);
                                writeConsoleLines("LifePacks :- " + (i + 1) + "/" + Math.Min(lifePackCount, points.Count) + " created", lineCount);
                            }
                            else
                            {
                                i--;//Recalculate
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("...............................");
                            Console.WriteLine(e.StackTrace);
                            Console.WriteLine("...............................");
                        }

                    }





                    writeConsoleLines("\nMap generation was successful!");

                    Console.Write("\n");
                    for (int i = 0; i < mapSize; i++)
                    {
                        for (int j = 0; j < mapSize; j++)
                        {
                            Point p = new Point(j, i);
                            if (brickLocations.Contains(p))
                            {
                                Console.Write("B");
                            }
                            else if (obstacles.Contains(p))
                            {
                                Console.Write("S");
                            }
                            else if (water.Contains(p))
                            {
                                Console.Write("W");
                            }
                            else
                            {
                                Console.Write("N");
                            }
                            Console.Write(" ");
                        }
                        Console.Write("\n");
                    }
                    Console.Write("\n");

                    sortCoinPileLists();



                }
                catch (Exception ex2)
                {
                    Console.WriteLine("Could not load nor generate Map!!!\n\n");
                    Console.WriteLine("Error Message: " + ex1.Message + "\n\n");
                    Console.WriteLine("Error Message: " + ex2.Message + "\n\n");
                    Console.WriteLine("Server not started! Please restart the server!");
                    while (true)
                    {
                        Console.ReadLine();
                    }
                }
            }
        }

        private MapItem CreateMapItem(Point p, Boolean isCoinPile)
        {
            RandomGen r = new RandomGen();

            MapItem tempMapItem;
            if (isCoinPile)
            {
                tempMapItem = new CoinPile(p.X, p.Y);
                ((CoinPile)tempMapItem).Price = r.randomD(700, 2000);
            }
            else
            {
                tempMapItem = new LifePack(p.X, p.Y);
            }
            tempMapItem.LifeTime = r.randomD(Convert.ToInt32((Constant.MAP_SIZE / 4) * Constant.PLAYER_DELAY), Constant.LIFE_TIME / 10);
            tempMapItem.AppearTime = r.randomD(0, (Constant.LIFE_TIME - tempMapItem.LifeTime));


            tempMapItem.DisappearTime = tempMapItem.AppearTime + tempMapItem.LifeTime;
            return tempMapItem;
        }

        private Boolean GetMapItemAcceptance(MapItem tempMapItem, List<MapItem> conflictTestItems)
        {
            Boolean pass = true;

            for (int j = 0; j < /*i*/conflictTestItems.Count; j++)
            {
                if (tempMapItem.Position.Equals(conflictTestItems[j].Position))
                {

                    Boolean tempInDanger = false;

                    if ((tempMapItem.AppearTime >= conflictTestItems[j].AppearTime) & (tempMapItem.AppearTime <= conflictTestItems[j].DisappearTime)) //Temp CoinPile appears after the old CoinPile appears and before the old CoinPile disappears
                    {
                        tempInDanger = true;
                    }
                    else if ((conflictTestItems[j].AppearTime >= tempMapItem.AppearTime) & (conflictTestItems[j].AppearTime <= tempMapItem.DisappearTime))//Old CoinPile appears after the temp CoinPile appears and before the temp CoinPile disappears
                    {
                        tempInDanger = true;
                    }
                    else if ((tempMapItem.AppearTime >= conflictTestItems[j].AppearTime) & (tempMapItem.DisappearTime <= conflictTestItems[j].DisappearTime))//Temp CoinPile appears after the old CoinPile appears and disappears before the old CoinPile does
                    {
                        tempInDanger = true;
                    }
                    else if ((conflictTestItems[j].AppearTime >= tempMapItem.AppearTime) & (conflictTestItems[j].DisappearTime <= tempMapItem.DisappearTime))//Old CoinPile appears after the temp CoinPile appears and disappears before the temp CoinPile does
                    {
                        tempInDanger = true;
                    }



                    if (tempInDanger)
                    {
                        tempMapItem.AppearTime = tempMapItem.AppearTime + 2000;
                        tempMapItem.DisappearTime = tempMapItem.DisappearTime + 2000;
                        if (tempMapItem.DisappearTime <= Constant.LIFE_TIME)
                        {
                            j = 0; //Cheak the compatibility all over again
                        }
                        else
                        {
                            pass = false;//do not add it                           
                            break;
                        }
                    }
                }
            }

            return (pass);
        }

        private List<Point> CreatePointList(int itemCount)
        {
            RandomGen r = new RandomGen();

            int lineCount = consoleLines.Count;

            List<Point> points = new List<Point>();

            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    Point p = new Point(i, j);
                    points.Add(p);
                }
            }


            for (int i = 0; i < points.Count; i++)
            {
                if ((brickLocations.Contains(points[i])) | (obstacles.Contains(points[i])) | (water.Contains(points[i])))
                {
                    points.RemoveAt(i);
                    i--;
                }
            }



            while (points.Count < itemCount)
            {
                writeConsoleLines("Adjusting  point list size " + (points.Count * 100) / itemCount + "% done", lineCount);
                int i = r.randomDD(points.Count - 1);
                points.Add(points[i]);
            }
            writeConsoleLines("Adjusting  point list size 100% done", lineCount);

            return (points);
        }

        private void sortCoinPileLists()
        {
            GameEngine.coinPileList.Sort(CompareMapItemsByTime);
            GameEngine.coinPilesToDisclose = GameEngine.coinPileList.Count;
            GameEngine.disappearCoinPileList.AddRange(GameEngine.coinPileList);
            GameEngine.disappearCoinPileList.Sort(CompareMapItemsByDisappearTime);


            GameEngine.lifePackList.Sort(CompareMapItemsByTime);
            GameEngine.lifePacksToDisclose = GameEngine.lifePackList.Count;
            GameEngine.disappearLifePackList.AddRange(GameEngine.lifePackList);
            GameEngine.disappearLifePackList.Sort(CompareMapItemsByDisappearTime);

            List<Point> obstLocations = new List<Point>();
            obstLocations.AddRange(GameEngine.obstacles);
            obstLocations.AddRange(GameEngine.brickLocations);

            AIMain ai = AIMain.GetInstance();
            ai.setMapSize(GameEngine.mapSize);
            ai.setMap(GameEngine.water.ToArray(), obstLocations.ToArray());

        }

        public void Initialize()
        {
            GameEngine.brickLocations.Clear();
            GameEngine.obstacles.Clear();

            GameEngine.coinPileList.Clear();
            GameEngine.availableCoinPileList.Clear();
            GameEngine.disappearCoinPileList.Clear();

            GameEngine.lifePackList.Clear();
            GameEngine.availableLifePackList.Clear();
            GameEngine.disappearLifePackList.Clear();


            GameEngine.playerList.Clear();
            GameEngine.playerIPList.Clear();
            GameEngine.playerPort.Clear();

            GameEngine.maxPlayerNumber = 5;// int.Parse(ConfigurationManager.AppSettings.Get("MaxPlayerCount"));
            GameEngine.obstaclePenalty = int.Parse(ConfigurationManager.AppSettings.Get("ObstaclePenalty"));


        }

        public static void Resolve(object stateInfo)
        {
            DataObject dataObj = (DataObject)stateInfo;
            string conIP = dataObj.ClientMachine;
            int conPort = dataObj.ClientPort;

            string request = dataObj.MSG;
            if (request.Contains(";"))
            {
                String[] tokens = request.Split(new char[] { ';' });
                request = tokens[1];
            }


            string result = "";
            if (Constant.UP.Equals(request) || Constant.DOWN.Equals(request)
                || Constant.LEFT.Equals(request) || Constant.RIGHT.Equals(request) || Constant.SHOOT.Equals(request))
                result = gameEng.UpdateStatus(conIP, conPort, request);
            else if (Constant.C2S_INITIALREQUEST.Equals(request) && !GameEngine.gameStarted)
                result = gameEng.AddContestant(conIP, conPort);
            else if (Constant.C2S_INITIALREQUEST.Equals(request) && GameEngine.gameStarted)
                result = Constant.S2C_GAMESTARTED + Constant.S2C_DEL;
            else
                result = Constant.S2C_REQUESTERROR + Constant.S2C_DEL;
            dataObj.MSG = result;

            if (result != null)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(com.SendData), (object)dataObj);
            }
        }

        private string AddContestant(string conIP, int port)
        {
            if (playerList.Count < GameEngine.maxPlayerNumber && !(playerIPList.Contains(conIP) && playerPort.Contains(port)))
            {
                Monitor.Enter(playerIPList);
                Monitor.Enter(playerPort);
                Monitor.Enter(playerList);
                int index = playerList.Count;
                Contestant con = new Contestant("P" + index, conIP, port);
                con.UpdatedTime = DateTime.Now;

                playerList.Add(con);
                playerIPList.Add(conIP);
                playerPort.Add(port);

                //Engine.joinedPlayers += 1;
                //Engine.alivePlayers += 1;

                switch (playerList.Count)
                {
                    case (1):
                        con.StartP = new Point(0, 0);
                        this.RaiseGameStartingEvent();
                        break;
                    case (2):
                        con.StartP = new Point(0, mapSize - 1);
                        break;
                    case (3):
                        con.StartP = new Point(mapSize - 1, 0);
                        break;
                    case (4):
                        con.StartP = new Point(mapSize - 1, mapSize - 1);
                        break;
                    default: // LIMITATION: Max Number of Contestants should be equal or less to the width of the map
                        con.StartP = new Point(playerList.Count, playerList.Count);
                        break;
                }
                con.CurrentP = con.StartP;
                if (playerList.Count == GameEngine.maxPlayerNumber && GameEngine.gameStarted)
                {
                    GameEngine.gameStarted = true;
                    GameEngine.gameFinished = false;
                    this.RaiseGameJustStartedEvent();
                }
                Monitor.Exit(playerList);
                Monitor.Exit(playerPort);
                Monitor.Exit(playerIPList);
                //If added then initial map is sent.
                gui.AddPlayer(con.CurrentP.X, con.CurrentP.Y, con.Direction, index);
                //    gui.InitializeMap(Constant.MAP_SIZE, GameEngine.mapDetails);

                return "I:" + con.Name + ":" + mapDetails + Constant.S2C_DEL;

            }
            else if (playerList.Count >= GameEngine.maxPlayerNumber && !playerIPList.Contains(conIP))
            {
                return Constant.S2C_CONTESTANTSFULL + Constant.S2C_DEL;
            }
            else if (playerIPList.Contains(conIP) && playerPort.Contains(port))
            {
                return Constant.S2C_ALREADYADDED + Constant.S2C_DEL;
            }
            else
            {
                return Constant.S2C_REQUESTERROR + Constant.S2C_DEL;
            }
        }

        private string UpdateStatus(string conIP, int conPort, string request)
        {
            if (GameEngine.gameFinished)
                return Constant.S2C_GAMEOVER + Constant.S2C_DEL;
            else if (!GameEngine.gameStarted)
                return Constant.S2C_NOTSTARTED + Constant.S2C_DEL;
            else if (!playerIPList.Contains(conIP))
                return Constant.S2C_NOTACONTESTANT + Constant.S2C_DEL;
            else
            {

                int index = GameEngine.playerIPList.IndexOf(conIP);
                if (index != GameEngine.playerIPList.LastIndexOf(conIP))
                {
                    index = GameEngine.playerPort.IndexOf(conPort);
                }
                //Catering for multithreading.
                Monitor.Enter(GameEngine.playerList);
                Contestant con = GameEngine.playerList[index];
                Monitor.Exit(GameEngine.playerList);
                return (updateContestant(con, request));


            }
        }

        public string updateContestant(Contestant con, string request)
        {
            string pointsEarned = "";
            if (!con.IsAlive) //Player is not alive
            {
                return Constant.S2C_NOTALIVE + Constant.S2C_DEL;
            }
            TimeSpan dr = DateTime.Now - con.UpdatedTime;
            if (dr.TotalMilliseconds < Constant.PLAYER_DELAY)
            {
                con.UpdatedTime = DateTime.Now; //No flooding the server is accepted ;)
                return Constant.S2C_TOOEARLY + Constant.S2C_DEL;
            }
            //Console.WriteLine(dr.TotalSeconds);
            con.InvalidCell = false;
            Point reqPoint = new Point();
            Boolean shoot = false;
            Boolean turn = false;
            //Calculating the next cell requested.
            if (Constant.UP.Equals(request.ToUpperInvariant()))
            {
                if (con.Direction == 0) //If it is facing North already move
                {
                    reqPoint.X = con.CurrentP.X;
                    reqPoint.Y = con.CurrentP.Y - 1;
                }
                else //Turn to face North
                {
                    reqPoint.X = con.CurrentP.X;
                    reqPoint.Y = con.CurrentP.Y;
                    con.Direction = 0;
                    turn = true;
                }
            }
            else if (Constant.DOWN.Equals(request.ToUpperInvariant()))
            {
                if (con.Direction == 2)
                {
                    reqPoint.X = con.CurrentP.X;
                    reqPoint.Y = con.CurrentP.Y + 1;
                }
                else
                {
                    reqPoint.X = con.CurrentP.X;
                    reqPoint.Y = con.CurrentP.Y;
                    con.Direction = 2;
                    turn = true;
                }
            }
            else if (Constant.LEFT.Equals(request.ToUpperInvariant()))
            {
                if (con.Direction == 3)
                {
                    reqPoint.X = con.CurrentP.X - 1;
                    reqPoint.Y = con.CurrentP.Y;
                }
                else
                {
                    reqPoint.X = con.CurrentP.X;
                    reqPoint.Y = con.CurrentP.Y;
                    con.Direction = 3;
                    turn = true;
                }
            }
            else if (Constant.RIGHT.Equals(request.ToUpperInvariant()))
            {
                if (con.Direction == 1)
                {
                    reqPoint.X = con.CurrentP.X + 1;
                    reqPoint.Y = con.CurrentP.Y;
                }
                else
                {
                    reqPoint.X = con.CurrentP.X;
                    reqPoint.Y = con.CurrentP.Y;
                    con.Direction = 1;
                    turn = true;
                }
            }
            else if (Constant.SHOOT.Equals(request.ToUpperInvariant()))
            {
                shoot = true;
            }

            if (!shoot)
            {
                con.Shot = false;
                //Checking for errors in the cell requested.
                if (IsInvalid(reqPoint))
                {
                    //out of the map.
                    con.InvalidCell = true;
                    con.UpdatedTime = DateTime.Now;
                    //Releasing the players.                   
                    return Constant.S2C_INVALIDCELL + Constant.S2C_DEL;
                }
                else //Cell requested is in the map.
                {
                    if (GameEngine.obstacles.Contains(reqPoint))
                    { //Hits on an obstacle.
                        return ObstacleHit(con);
                    }
                    else if (GameEngine.brickLocations.Contains(reqPoint))
                    {
                        int brickIndex = brickLocations.IndexOf(reqPoint);
                        if (brickWalls[brickIndex].DamageLevel < 4) //consider as obstacle
                        {
                            return ObstacleHit(con);
                        }
                        else
                        {
                            con.CurrentP = reqPoint;
                            con.UpdatedTime = DateTime.Now;
                        }
                    }
                    else if (GameEngine.water.Contains(reqPoint))
                    {   //Falls in a pit.
                        con.IsAlive = false;
                        con.CurrentP = reqPoint;
                        con.Health = 0;
                        con.UpdatedTime = DateTime.Now;
                        HandlePlayerDeath();
                        return Constant.S2C_FALLENTOPIT + Constant.S2C_DEL;
                    }
                    else //Not an obstacle or pitfall. May be an already occupied cell.
                    {
                        Monitor.Enter(GameEngine.playerList);
                        for (int i = 0; i < GameEngine.playerList.Count; i++)
                        {
                            if (!turn && i != con.Index && reqPoint == GameEngine.playerList[i].CurrentP)
                            {
                                if (GameEngine.playerList[i].IsAlive)
                                {
                                    con.UpdatedTime = DateTime.Now;
                                    // con.CurrentP = reqPoint;                                
                                    return Constant.S2C_CELLOCCUPIED + Constant.S2C_DEL;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        Monitor.Exit(GameEngine.playerList);
                        //Non occupied cell, May be a regular CoinPile is here.
                        //Monitor.Enter(GameEngine.availableCoinPileList);
                        foreach (CoinPile t in GameEngine.availableCoinPileList)
                        {
                            if (t.Position == reqPoint)
                            {
                                con.PointsEarned += t.Price;
                                con.Coins += t.Price;
                                pointsEarned += t.Price;
                                int tIndex = gameEng.DisappearCoinPileList.IndexOf(t);
                                GameEngine.availableCoinPileList.Remove(t);
                                GameEngine.availableCoinPileList.Sort(CompareMapItemsByDisappearTime);
                                HandleCoinPileGameEnd();
                                gui.RemoveMapItem(t.Position.X, t.Position.Y);
                                break;
                            }
                        }

                        foreach (CoinPile c in plunderCoinPileList)
                        {
                            if (c.Position == reqPoint)
                            {
                                con.PointsEarned += c.Price;
                                con.Coins += c.Price;
                                pointsEarned += c.Price;
                                plunderCoinPileList.Remove(c);
                                HandleCoinPileGameEnd();
                                gui.RemoveMapItem(c.Position.X, c.Position.Y);
                                break;
                            }
                        }

                        //Non occupied cell, May be a lifepack is here.
                        foreach (LifePack l in GameEngine.availableLifePackList)
                        {
                            if (l.Position == reqPoint)
                            {
                                con.Health += (int)(0.1f * Constant.PLAYER_HEALTH);
                                int lIndex = gameEng.DisappearLifePackList.IndexOf(l);
                                GameEngine.availableLifePackList.Remove(l);
                                GameEngine.availableLifePackList.Sort(CompareMapItemsByDisappearTime);
                                gui.RemoveMapItem(l.Position.X, l.Position.Y);
                                break;
                            }
                        }

                        con.UpdatedTime = DateTime.Now;
                        con.CurrentP = reqPoint;
                    }
                }
            }
            else
            {
                con.Shot = true;
                activeBullets.Add(new Bullet(con, con.CurrentP, con.Direction));
            }
            return null;


        }

        public void HandleCoinPileGameEnd()
        {
            if (GameEngine.coinPilesToDisclose == 0 &&
                    GameEngine.availableCoinPileList.Count == 0 && plunderCoinPileList.Count == 0)
            {
                GameEngine.gameFinished = true;
                GameEngine.gameStarted = false;
                this.RaiseGameOverEvent();
            }
        }

        private void HandlePlayerDeath()
        {
            Contestant c;
            int deadPlayers = 0;
            for (int i = 0; i < GameEngine.playerList.Count; i++)
            {
                c = GameEngine.playerList[i];
                if (!c.IsAlive)
                {
                    deadPlayers++;
                }
            }
            if (deadPlayers == GameEngine.playerList.Count)
            {
                GameEngine.gameFinished = true;
                GameEngine.gameStarted = false;
                this.RaiseGameOverEvent();
            }
        }

        private String ObstacleHit(Contestant con)
        {
            con.PointsEarned -= GameEngine.obstaclePenalty;
            con.UpdatedTime = DateTime.Now;
            return Constant.S2C_HITONOBSTACLE + ";" + GameEngine.obstaclePenalty + Constant.S2C_DEL;
        }

        private Boolean IsInvalid(Point reqPoint)
        {
            return (reqPoint.X > GameEngine.mapSize - 1 || reqPoint.X < 0 ||
                                    reqPoint.Y > GameEngine.mapSize - 1 || reqPoint.Y < 0);
        }

        private int CompareMapItemsByTime(MapItem t1, MapItem t2)
        {
            if (t1.AppearTime > t2.AppearTime)
                return -1;
            else if (t1.AppearTime == t2.AppearTime)
                return 0;
            else
                return 1;
        }

        private int CompareMapItemsByDisappearTime(MapItem t1, MapItem t2)
        {
            if (t1.DisappearTime > t2.DisappearTime)
                return 1;
            else if (t1.DisappearTime == t2.DisappearTime)
                return 0;
            else
                return -1;
        }

        public void SendCoinPileUpdates()
        {
            //  System.Windows.Forms.MessageBox.Show("Test");
            Boolean showTress = true;
            foreach (Contestant c in playerList)
            {
                if (c.CurrentP.Equals(GameEngine.coinPileList[GameEngine.coinPilesToDisclose - 1].Position))
                {
                    showTress = false;
                    break;
                }
            }
            if (showTress)
            {
                int x = GameEngine.coinPileList[GameEngine.coinPilesToDisclose - 1].Position.X;
                int y = GameEngine.coinPileList[GameEngine.coinPilesToDisclose - 1].Position.Y;
                int lifeT = GameEngine.coinPileList[GameEngine.coinPilesToDisclose - 1].LifeTime;
                int price = GameEngine.coinPileList[GameEngine.coinPilesToDisclose - 1].Price;

                string tStatus = "C:" + x + "," +
                    y + ":" +
                   lifeT + ":" +
                    price + Constant.S2C_DEL;

                Monitor.Enter(GameEngine.availableCoinPileList);
                GameEngine.availableCoinPileList.Add(GameEngine.coinPileList[GameEngine.coinPilesToDisclose - 1]);
                GameEngine.availableCoinPileList.Sort(this.CompareMapItemsByDisappearTime);
                Monitor.Exit(GameEngine.availableCoinPileList);

                GameEngine.coinPilesToDisclose--;
                DataObject dO = new DataObject(tStatus, GameEngine.playerIPList, GameEngine.playerPort);
                ThreadPool.QueueUserWorkItem(new WaitCallback(com.BroadCast), (object)dO);
                Console.WriteLine("............................................................................");
                Console.WriteLine("NEXT CoinPile IN :" + GameEngine.nextCoinPileSend + " :: T-To-Disclose :" + GameEngine.coinPilesToDisclose);
                Console.WriteLine(tStatus);

                gui.AddCoin(x, y, price, lifeT);
            }
            else
            {
                GameEngine.coinPileList[GameEngine.coinPilesToDisclose - 1].AppearTime += 2000;
                GameEngine.coinPileList[GameEngine.coinPilesToDisclose - 1].DisappearTime += 2000;
                GameEngine.coinPileList.Sort(CompareMapItemsByTime);
            }




        }

        public void SendLifePackUpdates()
        {
            Boolean showLifePack = true;
            foreach (Contestant c in playerList)
            {
                if (c.CurrentP.Equals(GameEngine.lifePackList[GameEngine.lifePacksToDisclose - 1].Position))
                {
                    showLifePack = false;
                    break;
                }
            }
            if (showLifePack)
            {

                int x = GameEngine.lifePackList[GameEngine.lifePacksToDisclose - 1].Position.X;
                int y = GameEngine.lifePackList[GameEngine.lifePacksToDisclose - 1].Position.Y;
                int lifeTime = GameEngine.lifePackList[GameEngine.lifePacksToDisclose - 1].LifeTime;

                string tStatus = "L:" + x + "," +
                   y + ":" +
                  lifeTime + Constant.S2C_DEL;

                Monitor.Enter(GameEngine.availableLifePackList);
                GameEngine.availableLifePackList.Add(GameEngine.lifePackList[GameEngine.lifePacksToDisclose - 1]);
                GameEngine.availableLifePackList.Sort(this.CompareMapItemsByDisappearTime);
                Monitor.Exit(GameEngine.availableLifePackList);

                GameEngine.lifePacksToDisclose--;
                DataObject dO = new DataObject(tStatus, GameEngine.playerIPList, GameEngine.playerPort);
                ThreadPool.QueueUserWorkItem(new WaitCallback(com.BroadCast), (object)dO);
                Console.WriteLine("............................................................................");
                Console.WriteLine("NEXT Life Packet IN :" + GameEngine.nextLifePackSend + " :: T-To-Disclose :" + GameEngine.lifePacksToDisclose);
                Console.WriteLine(tStatus);


                gui.AddLifePack(x, y, lifeTime);
            }
            else
            {
                GameEngine.lifePackList[GameEngine.lifePacksToDisclose - 1].AppearTime += 2000;
                GameEngine.lifePackList[GameEngine.lifePacksToDisclose - 1].DisappearTime += 2000;
                GameEngine.lifePackList.Sort(CompareMapItemsByTime);
            }
        }

        public void SendPlayerUpdates()
        {
            if (!GameEngine.gameFinished)
            {
                Monitor.Enter(GameEngine.playerList);

                string msg = "G:";
                foreach (Contestant p in GameEngine.playerList)
                {
                    int shotNum = 0;
                    if (p.Shot)
                    {
                        shotNum = 1;
                        p.Shot = false; //reset the shoot variable.
                    }
                    msg += p.Name + ";" + p.CurrentP.X + "," + p.CurrentP.Y + ";" + p.Direction + ";" + shotNum + ";" + p.Health + ";" + p.Coins + ";" + p.PointsEarned + ":";

                }

                foreach (BrickWall b in GameEngine.brickWalls)
                {
                    msg += b.Pos.X + "," + b.Pos.Y + "," + b.DamageLevel + ";";
                }
                msg = msg.Substring(0, msg.Length - 1);

                gui.UpdateMap(msg);

                msg += Constant.S2C_DEL;
                Console.WriteLine(msg);
                DataObject dO = new DataObject(msg, GameEngine.playerIPList, GameEngine.playerPort);
                ThreadPool.QueueUserWorkItem(new WaitCallback(com.BroadCast), (object)dO);




                List<CoinPile> currentCoinPiles = new List<CoinPile>();
                currentCoinPiles.AddRange(availableCoinPileList);
                currentCoinPiles.AddRange(plunderCoinPileList);

                AIMain.GetInstance().AvailableCoinPileList = currentCoinPiles;
                AIMain.GetInstance().moveShips();
                List<AIMessage> nextAIupdates = AIMain.GetInstance().NextAIupdates;
                for (int i = 0; i < nextAIupdates.Count; i++)
                {
                    updateContestant(nextAIupdates[i].Contestant, nextAIupdates[i].Command);
                }


            }
        }

        public void PrepareStartingGame()
        {
            Thread.Sleep(int.Parse(ConfigurationManager.AppSettings.Get("StartDelay")));
            while (playerList.Count < minPlayerNumber)
            {
                Thread.Sleep(100);//Looping delay. To reduce the burden on the processor
            }
            Thread.Sleep(1000);//Real player delay. Let the message go through the network
            Monitor.Enter(GameEngine.playerList);
            Monitor.Enter(GameEngine.playerPort);
            Monitor.Enter(GameEngine.playerIPList);
            string msg = "S:";
            foreach (Contestant p in GameEngine.playerList)
            {
                msg += p.Name + ";" + p.StartP.X + "," + p.StartP.Y + ";" + p.Direction + ":";
                p.UpdatedTime = DateTime.Now;
            }
            msg = msg.Substring(0, msg.Length - 1);
            msg += Constant.S2C_DEL;
            Console.WriteLine(msg);
            DataObject dO = new DataObject(msg, GameEngine.playerIPList, GameEngine.playerPort);
            ThreadPool.QueueUserWorkItem(new WaitCallback(com.BroadCast), (object)dO);
            if (!GameEngine.gameStarted)
            {
                gameStarted = true;
                gameFinished = false;
                this.RaiseGameJustStartedEvent();
            }
            Monitor.Exit(GameEngine.playerIPList);
            Monitor.Exit(GameEngine.playerPort);
            Monitor.Exit(GameEngine.playerList);
        }

        public void FinishGame()
        {
            GameEngine.gameStarted = false;
            GameEngine.gameFinished = true;
            GameEngine.coinPilesToDisclose = 0;
            DataObject dO = new DataObject(Constant.S2C_GAMEJUSTFINISHED + Constant.S2C_DEL, GameEngine.playerIPList, GameEngine.playerPort);
            ThreadPool.QueueUserWorkItem(new WaitCallback(com.BroadCast), (object)dO);
        }












        private void IssuePlunderCoinPile(Point pos, int value)
        {
            string tStatus = "C:" + pos.X + "," + pos.Y + ":" + Constant.PLUNDER_TREASUR_LIFE + ":" + value + Constant.S2C_DEL;

            DataObject dO = new DataObject(tStatus, GameEngine.playerIPList, GameEngine.playerPort);
            ThreadPool.QueueUserWorkItem(new WaitCallback(com.BroadCast), (object)dO);
            Console.WriteLine("NEXT CoinPile IN :" + GameEngine.nextCoinPileSend + " :: T-To-Disclose :" + GameEngine.coinPilesToDisclose);
            Console.WriteLine(tStatus);


            CoinPile c = new CoinPile(pos.X, pos.Y);
            c.Price = value;
            int waitAmout = 0;
            for (int i = 0; i < plunderCoinPileList.Count; i++)
            {
                waitAmout += plunderCoinPileList[i].DisappearBalance;
            }
            c.DisappearBalance = Math.Max(0, (Constant.PLUNDER_TREASUR_LIFE - waitAmout));
            plunderCoinPileList.Add(c);
            gui.AddCoin(pos.X, pos.Y, value, Constant.PLUNDER_TREASUR_LIFE);
        }




        internal void UpdateBullets()
        {
            for (int j = 0; j < activeBullets.Count; j++)
            {
                // Console.WriteLine("apple at " + activeBullets[j].Pos.X + " , " + activeBullets[j].Pos.Y + " , " + activeBullets[j].Direction);

                int[] dirData = activeBullets[j].DirData;
                Point newP = new Point(activeBullets[j].Pos.X + dirData[0], activeBullets[j].Pos.Y + dirData[1]);
                Boolean removeBullet = false;


                if (!IsInvalid(newP))
                {
                    if (brickLocations.Contains(newP))
                    {
                        int wallIndex = brickLocations.IndexOf(newP);
                        if (brickWalls[wallIndex].DamageLevel < 4)
                        {
                            brickWalls[wallIndex].DamageLevel++;
                            Console.WriteLine("Wall " + brickWalls[wallIndex].Pos.X + "," + brickWalls[wallIndex].Pos.Y + "," + brickWalls[wallIndex].DamageLevel);
                            removeBullet = true;
                            activeBullets[j].Shooter.PointsEarned += Constant.BRICK_POINTS;
                            //   hit = true;
                        }
                        // else
                        // {
                        //      activeBullets[j].Pos = newP; //Bypass the brick
                        //  }
                    }

                    if (obstacles.Contains(newP))
                    {
                        removeBullet = true;
                    }

                    //player hit
                    if (!removeBullet)
                    {
                        for (int i = 0; i < playerList.Count; i++)
                        {
                            if ((playerList[i].CurrentP == newP) && (playerList[i].IsAlive))
                            {
                                playerList[i].Health -= (int)(0.1f * Constant.PLAYER_HEALTH);
                                if (playerList[i].Health <= 0)
                                {
                                    playerList[i].IsAlive = false;
                                    HandlePlayerDeath();
                                    activeBullets[j].Shooter.PointsEarned += (int)(0.25f * playerList[i].PointsEarned);
                                    IssuePlunderCoinPile(playerList[i].CurrentP, playerList[i].Coins);

                                }
                                removeBullet = true;
                                break;
                            }
                        }
                    }




                    if (!removeBullet)
                    {
                        activeBullets[j].Pos = newP;
                    }

                }
                else
                {
                    removeBullet = true;
                }



                /*
                                List<Point> candidatePoints = new List<Point>();
                                candidatePoints.Add(activeBullets[j].Pos);

                                for (int i = 0; i < 3; i++)
                                {
                                    Point oldP = new Point(candidatePoints[candidatePoints.Count - 1].X, candidatePoints[candidatePoints.Count - 1].Y);
                                    Point newP = new Point(oldP.X + dirData[0], oldP.Y + dirData[1]);
                                    if (!IsInvalid(newP))
                                    {
                                        candidatePoints.Add(newP);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                candidatePoints.RemoveAt(0);
                                Console.WriteLine(candidatePoints.Count);

                                Boolean hit = false;
                
                                for (int i = 0; i < candidatePoints.Count; i++)
                                {
                    
                                }

                                if ((!hit) && (candidatePoints.Count > 0))
                                {
                                    if (!IsInvalid(candidatePoints[candidatePoints.Count - 1]))
                                    {
                                        activeBullets[j].Pos = candidatePoints[candidatePoints.Count - 1];
                                    }
                                    else
                                    {
                                        removeBullet = true;
                                    }
                                }*/

                if (removeBullet)
                {
                    gui.MoveBullet(activeBullets[j].Id, -1, -1, activeBullets[j].Direction);
                    activeBullets.RemoveAt(j);
                    j--;
                }
                else
                {
                    gui.MoveBullet(activeBullets[j].Id, newP.X, newP.Y, activeBullets[j].Direction);
                }


            }
        }


        GUII gui;
        internal void SetGUI(GUII gui)
        {
            this.gui = gui;
            gui.InitializeMap(Constant.MAP_SIZE, GameEngine.mapDetails);

            joinTim.Enabled = true;

        }

        Boolean[] dummyStatus;
        System.Timers.Timer joinTim = new System.Timers.Timer(1);
        Boolean firstBatchOfDummysAdded = false;

        private void joinTim_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            dummyStatus = gui.getJoinBeforeRealContestant();
            if (dummyStatus != null)
            {
                if (!firstBatchOfDummysAdded)
                {
                    if (dummyStatus.Length == 5)
                    {
                        minPlayerNumber = 5;
                    }
                    else
                    {
                        minPlayerNumber = dummyStatus.Length + 1;
                    }

                    for (int i = 0; i < dummyStatus.Length; i++)
                    {
                        if (dummyStatus[i])
                        {
                            AddContestant("", 7001 + i);
                        }
                    }

                    if (dummyStatus.Length == 5)
                    {
                        attachAIsToDummys(false);
                    }
                    firstBatchOfDummysAdded = true;
                }
                else
                {

                    Boolean realPlayerJoined = false;
                    foreach (Contestant c in playerList)
                    {
                        if (c.Port == 7000)
                        {
                            realPlayerJoined = true;
                            joinTim.Enabled = false;
                            break;
                        }
                    }

                    if (realPlayerJoined)
                    {
                        for (int i = 0; i < dummyStatus.Length; i++)
                        {
                            if (!dummyStatus[i])
                            {
                                AddContestant("", 7001 + i);
                            }
                        }
                        attachAIsToDummys(true);
                    }

                }

            }
        }

        private void attachAIsToDummys(Boolean isARealPiayerThere)
        {
            List<DummyAI> dummys = new List<DummyAI>();
            if (!isARealPiayerThere)
            {
                for (int i = 0; i < playerList.Count; i++)
                {
                    dummys.Add(new DummyAI(playerList[i], 7001 + i));
                }
            }
            else
            {
                int realIndex = -1;
                int dummyIndex = 0;
                for (int i = 0; i < playerList.Count; i++)
                {
                    if (playerList[i].Port == 7000)
                    {
                        realIndex = i;
                        break;
                    }
                }
                for (dummyIndex = 0; dummyIndex < realIndex; dummyIndex++)
                {
                    dummys.Add(new DummyAI(playerList[dummyIndex], 7001 + dummyIndex));
                }
                for (int i = (realIndex + 1); i < playerList.Count; i++)
                {
                    dummys.Add(new DummyAI(playerList[i], 7001 + i));
                }
            }
            AIMain.GetInstance().Dummys = dummys;
        }





    }
}
