using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel;
using System.Timers;
using System.Threading;

using lk.ac.mrt.cse.pc11.util;
using lk.ac.mrt.cse.pc11.bean;

namespace lk.ac.mrt.cse.pc11
{
    /// <summary>
    /// Management of the game happens here
    /// </summary>
    class GameManager
    {
        #region "Variables"
        private Communicator com; //Communicator instance to be used throughout
        private GameEngine gameEng; //Engine instance to be used throughout
        private static GameManager manager = new GameManager();
        private CoinPile nextCoinPileToDisappear = null;
        private LifePack nextLifePackToDisappear = null;
        #endregion

        #region "Timers:For Timely Events"
        private System.Timers.Timer playerUpdateTimer;
        private System.Timers.Timer lifeTimer;

        private System.Timers.Timer updateCoinPileTimer;
        private System.Timers.Timer CoinPileDisappearTimer;

        private System.Timers.Timer updateLifePackTimer;
        private System.Timers.Timer lifePackDisappearTimer;

        private System.Timers.Timer plunderCoinPileDisappearTimer;

        int updateCounter = Constant.BULLET_MULTI;
        #endregion

        #region "Background Threads"
        private BackgroundWorker listenerThread = new BackgroundWorker();
        private BackgroundWorker gameStarterThread = new BackgroundWorker();
        private BackgroundWorker gameFinisherThread = new BackgroundWorker();
        #endregion

        #region "Methods"

        #region "General"

        private GameManager()
        {
            this.com = Communicator.GetInstance();
            this.gameEng = GameEngine.GetInstance();
            
            SubscribeToGameJustStartedEvent();
            SubscribeToGameStartingEvent();
            SubscribeToGameOverEvent();

            InitializeBackGroundThreads();
            listenerThread.RunWorkerAsync();
            InitializeTimers();
        }

        public static GameManager GetInstance()
        {
            return manager;
        }

        #endregion

        #region "To Handle Events"
        /// <summary>
        /// To subscribe to GameStartingEvent
        /// </summary>
        private void SubscribeToGameStartingEvent()
        {
            GameStartingDelegate temp = new GameStartingDelegate(ReceiveGameStartingEvent);
            gameEng.startEvent += temp;
        }

        /// <summary>
        /// To subscribe to GameJustStartedEvent
        /// </summary>
        private void SubscribeToGameJustStartedEvent()
        {
            GameJustStartedDelegate temp = new GameJustStartedDelegate(ReceiveGameJustStartedEvent);
            gameEng.justStartedEvent += temp;
        }

        /// <summary>
        /// To subscribe to GameOverEvent
        /// </summary>
        private void SubscribeToGameOverEvent()
        {
            GameOverDelegate temp = new GameOverDelegate(ReceiveGameOverEvent);
            gameEng.gameOverEvent += temp;
        }

        /// <summary>
        /// What should be done when GameStartingEvent is received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ReceiveGameStartingEvent(object sender, EventArgs e)
        {
            gameStarterThread.RunWorkerAsync();
        }

        /// <summary>
        /// What needs to be done when GameJustStartedEvent is received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ReceiveGameJustStartedEvent(object sender, EventArgs e)
        {
            ResetTimers();
            Console.WriteLine("\n*******************************************************");
            Console.WriteLine("------------------GAME JUST STARTED--------------------");
            Console.WriteLine("*******************************************************\n\n");
        }

        /// <summary>
        /// What needs to be done when GameOverEvent is received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ReceiveGameOverEvent(object sender, EventArgs e)
        {
            gameFinisherThread.RunWorkerAsync();
        }

        #endregion

        #region "Background Thread Methods"

        /// <summary>
        /// To initialize all Background Threads used
        /// </summary>
        private void InitializeBackGroundThreads()
        {
            listenerThread.DoWork += new DoWorkEventHandler(listenerThread_DoWork);
            gameStarterThread.DoWork += new DoWorkEventHandler(gameStarterThread_DoWork);
            gameFinisherThread.DoWork += new DoWorkEventHandler(gameFinisherThread_DoWork);

            listenerThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(listenerThread_RunWorkerCompleted);

            listenerThread.WorkerSupportsCancellation = true;
            gameStarterThread.WorkerSupportsCancellation = true;
            gameFinisherThread.WorkerSupportsCancellation = true;
        }

        /// <summary>
        /// Work done by ListenerThread:- 
        /// Starts Listening for connections
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listenerThread_DoWork(object sender, DoWorkEventArgs e)
        {
            com.ReceiveData();
        }

        /// <summary>
        /// Work done by GameStartedThread:-
        /// Notifies the Engine to prepare starting the game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gameStarterThread_DoWork(object sender, DoWorkEventArgs e)
        {
            gameEng.PrepareStartingGame();
            lifeTimer.Start();
        }

        /// <summary>
        /// Work done by GameFinisherThread:-
        /// Reset all the resources, Notifies the Engine to prepate finishing the game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gameFinisherThread_DoWork(object sender, DoWorkEventArgs e)
        {
            this.StopTimers();
            gameEng.FinishGame();

            Console.WriteLine("\n*******************************************************");
            Console.WriteLine("-----------------GAME JUST FINISHED--------------------");
            Console.WriteLine("REASON: All players are dead OR All CoinPiles are taken or vanished!\n");

            List<Contestant> players = gameEng.PlayerList;
            foreach (Contestant c in players)
            {
                Console.WriteLine("NAME: " + c.Name + "\tPOINTS: " + c.PointsEarned + "\tALIVE:" + c.IsAlive);
            }
            Console.WriteLine("\n*******************************************************\n\n");
            gameEng.Initialize();

            gameFinisherThread.CancelAsync();

        }

        private void listenerThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                Console.WriteLine(e.Error.Message);
            else if (e.Cancelled)
                Console.WriteLine("CANCELLED");
        }


        #endregion

        #region "Timer Methods"

        /// <summary>
        /// Initializing the timers
        /// </summary>
        private void InitializeTimers()
        {
            this.updateCoinPileTimer = new System.Timers.Timer();
            this.playerUpdateTimer = new System.Timers.Timer();
            this.lifeTimer = new System.Timers.Timer();
            this.CoinPileDisappearTimer = new System.Timers.Timer();
            this.updateLifePackTimer = new System.Timers.Timer();
            this.lifePackDisappearTimer = new System.Timers.Timer();
            this.plunderCoinPileDisappearTimer = new System.Timers.Timer();


            this.updateCoinPileTimer.Elapsed += new ElapsedEventHandler(updateCoinPileTimer_Elapsed);
            this.playerUpdateTimer.Elapsed += new ElapsedEventHandler(playerUpdateTimer_Elapsed);
            this.lifeTimer.Elapsed += new ElapsedEventHandler(lifeTimer_Elapsed);
            this.CoinPileDisappearTimer.Elapsed += new ElapsedEventHandler(CoinPileDisappearTimer_Elapsed);
            this.updateLifePackTimer.Elapsed += new ElapsedEventHandler(updateLifePackTimer_Elapsed);
            this.lifePackDisappearTimer.Elapsed += new ElapsedEventHandler(lifePackDisappearTimer_Elapsed);
            this.plunderCoinPileDisappearTimer.Elapsed += new ElapsedEventHandler(plunderCoinPileDisappearTimer_Elapsed);
           
            this.playerUpdateTimer.Enabled = true;
            this.lifeTimer.Enabled = true;

            this.updateCoinPileTimer.Enabled = true;
            this.CoinPileDisappearTimer.Enabled = true;

            this.updateLifePackTimer.Enabled = true;
            this.lifePackDisappearTimer.Enabled = true;

            this.plunderCoinPileDisappearTimer.Enabled = true;

            this.playerUpdateTimer.Interval = Constant.UPDATE_PERIOD;
            this.lifeTimer.Interval = Constant.LIFE_TIME;

            this.plunderCoinPileDisappearTimer.Interval = 1;

            if (gameEng.CoinPileList.Count > 0)
            {
                gameEng.NextCoinPileSend = gameEng.CoinPileList[gameEng.CoinPilesToDisclose - 1].AppearTime;
                this.updateCoinPileTimer.Interval = gameEng.NextCoinPileSend;  
                this.CoinPileDisappearTimer.Interval = gameEng.DisappearCoinPileList[0].DisappearTime;
                this.nextCoinPileToDisappear = gameEng.DisappearCoinPileList[0];
            }
            else
            {
                this.CoinPileDisappearTimer.Interval = Constant.LIFE_TIME;
                this.nextCoinPileToDisappear = new CoinPile(-1, -1);
                this.nextCoinPileToDisappear.AppearTime = Constant.LIFE_TIME;
                this.nextCoinPileToDisappear.DisappearTime = Constant.LIFE_TIME;
            }

            if (gameEng.LifePackList.Count > 0)
            {
                gameEng.NextLifePackSend = gameEng.LifePackList[gameEng.LifePacksToDisclose - 1].AppearTime;
                this.updateLifePackTimer.Interval = gameEng.NextLifePackSend;
                this.lifePackDisappearTimer.Interval = gameEng.DisappearLifePackList[0].DisappearTime;
                this.nextLifePackToDisappear = gameEng.DisappearLifePackList[0];
            }
            else
            {
                this.lifePackDisappearTimer.Interval = Constant.LIFE_TIME;
                this.nextLifePackToDisappear = new LifePack(-1, -1);
                this.nextLifePackToDisappear.AppearTime = Constant.LIFE_TIME;
                this.nextLifePackToDisappear.DisappearTime = Constant.LIFE_TIME;
            }

            this.updateCoinPileTimer.Stop();
            this.CoinPileDisappearTimer.Stop();

            this.updateLifePackTimer.Stop();
            this.lifePackDisappearTimer.Stop();

            this.playerUpdateTimer.Stop();            
            this.lifeTimer.Stop();
        }

        void plunderCoinPileDisappearTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (gameEng.PlunderCoinPileList.Count > 0)
            {
                gameEng.PlunderCoinPileList[0].DisappearBalance--;
                //Take them out
                while ((gameEng.PlunderCoinPileList.Count > 0) && (gameEng.PlunderCoinPileList[0].DisappearBalance <= 0))
                {
                    Console.WriteLine("Plunder CoinPile @ " + gameEng.PlunderCoinPileList[0].Position + " VANISHED");
                    gameEng.PlunderCoinPileList.RemoveAt(0);
                }

                if (gameEng.PlunderCoinPileList.Count ==0)
                {
                    gameEng.HandleCoinPileGameEnd();
                }
            }
        }
        
        /// <summary>
        /// Resetting the timers
        /// </summary>
        private void ResetTimers()
        {
            this.playerUpdateTimer.Interval = Constant.UPDATE_PERIOD;

            if (gameEng.CoinPileList.Count > 0)
            {
                gameEng.NextCoinPileSend = gameEng.CoinPileList[gameEng.CoinPilesToDisclose - 1].AppearTime;
                this.updateCoinPileTimer.Interval = gameEng.NextCoinPileSend;           
                this.CoinPileDisappearTimer.Interval = gameEng.DisappearCoinPileList[0].DisappearTime;
            }
            else
            {
                CoinPileDisappearTimer.Interval = Constant.LIFE_TIME;
            }

            if (gameEng.LifePackList.Count > 0)
            {
                gameEng.NextLifePackSend = gameEng.LifePackList[gameEng.LifePacksToDisclose - 1].AppearTime;
                this.updateLifePackTimer.Interval = gameEng.NextLifePackSend;            
                this.lifePackDisappearTimer.Interval = gameEng.DisappearLifePackList[0].DisappearTime;
            }
            else
            {
               this.lifePackDisappearTimer.Interval = Constant.LIFE_TIME;
            }

            this.playerUpdateTimer.Start();

            this.updateCoinPileTimer.Start();
            this.CoinPileDisappearTimer.Start();

            this.updateLifePackTimer.Start();
            this.lifePackDisappearTimer.Start();
        }

        /// <summary>
        /// Stopping the timers
        /// </summary>
        private void StopTimers()
        {
            this.playerUpdateTimer.Stop();
            this.lifeTimer.Stop();

            this.updateCoinPileTimer.Stop();
            this.CoinPileDisappearTimer.Stop();

            this.updateLifePackTimer.Stop();
            this.lifePackDisappearTimer.Stop();
        }
         
        /// <summary>
        /// What needs to be done when a CoinPile disappears
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CoinPileDisappearTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (this.nextCoinPileToDisappear != null)
            {
                Monitor.Enter(gameEng.DisappearCoinPileList);
                Monitor.Enter(gameEng.AvailableCoinPileList);
                CoinPile CoinPileNextInLine = null;
                int currentIndex = gameEng.DisappearCoinPileList.IndexOf(this.nextCoinPileToDisappear);

                if (gameEng.AvailableCoinPileList.Contains(this.nextCoinPileToDisappear))
                {
                    Console.WriteLine("CoinPile @ " + this.nextCoinPileToDisappear.Position + " VANISHED");
                    gui.RemoveMapItem(this.nextCoinPileToDisappear.Position.X, this.nextCoinPileToDisappear.Position.Y);
                    gameEng.AvailableCoinPileList.Remove(this.nextCoinPileToDisappear);
                }
                if (currentIndex < gameEng.DisappearCoinPileList.Count - 2)
                {
                    CoinPileNextInLine = gameEng.DisappearCoinPileList[currentIndex + 1];
                    int updateTimeSpan = CoinPileNextInLine.DisappearTime -
                        this.nextCoinPileToDisappear.DisappearTime;
                    if (updateTimeSpan > 0)
                        this.CoinPileDisappearTimer.Interval = updateTimeSpan;
                    else
                        this.CoinPileDisappearTimer.Interval = 500;
                    this.nextCoinPileToDisappear = CoinPileNextInLine;
                }
                else
                {
                    int updateTimeSpan = gameEng.DisappearCoinPileList[gameEng.DisappearCoinPileList.Count - 1].DisappearTime
                        - gameEng.DisappearCoinPileList[gameEng.DisappearCoinPileList.Count - 2].DisappearTime;
                    if (updateTimeSpan != 0)
                        this.CoinPileDisappearTimer.Interval = updateTimeSpan;
                    else
                        this.CoinPileDisappearTimer.Interval = 500;                        
                    this.nextCoinPileToDisappear = null;
                }
                Monitor.Exit(gameEng.AvailableCoinPileList);
                Monitor.Exit(gameEng.DisappearCoinPileList);
            }
            else
            {
                if (gameEng.AvailableCoinPileList.Contains(gameEng.DisappearCoinPileList[gameEng.DisappearCoinPileList.Count-1]))
                {
                    Monitor.Enter(gameEng.DisappearCoinPileList);
                    Monitor.Enter(gameEng.AvailableCoinPileList);
                    Console.WriteLine("CoinPile @ " +
                        gameEng.DisappearCoinPileList[gameEng.DisappearCoinPileList.Count - 1].Position + " VANISHED");
                    gui.RemoveMapItem(gameEng.DisappearCoinPileList[gameEng.DisappearCoinPileList.Count - 1].Position.X, gameEng.DisappearCoinPileList[gameEng.DisappearCoinPileList.Count - 1].Position.Y);
                    gameEng.AvailableCoinPileList.Remove(this.nextCoinPileToDisappear);
                    Monitor.Exit(gameEng.AvailableCoinPileList);
                    Monitor.Exit(gameEng.DisappearCoinPileList);
                    gameFinisherThread.RunWorkerAsync();
                }
            }
        }

        void lifePackDisappearTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (this.nextLifePackToDisappear != null)
            {
                Monitor.Enter(gameEng.DisappearLifePackList);
                Monitor.Enter(gameEng.AvailableLifePackList);
                LifePack lifePackNextInLine = null;
                int currentIndex = gameEng.DisappearLifePackList.IndexOf(this.nextLifePackToDisappear);

                if (gameEng.AvailableLifePackList.Contains(this.nextLifePackToDisappear))
                {
                    Console.WriteLine("Life Pack @ " + this.nextLifePackToDisappear.Position + " VANISHED");
                    gui.RemoveMapItem(this.nextLifePackToDisappear.Position.X, this.nextLifePackToDisappear.Position.Y);
                    gameEng.AvailableLifePackList.Remove(this.nextLifePackToDisappear);
                }

                if (currentIndex < gameEng.DisappearLifePackList.Count - 2)
                {
                    lifePackNextInLine = gameEng.DisappearLifePackList[currentIndex + 1];
                    int updateTimeSpan = lifePackNextInLine.DisappearTime -
                        this.nextLifePackToDisappear.DisappearTime;
                    if (updateTimeSpan > 0)
                        this.lifePackDisappearTimer.Interval = updateTimeSpan;
                    else
                        this.lifePackDisappearTimer.Interval = 500;
                    this.nextLifePackToDisappear = lifePackNextInLine;
                }
                else
                {
                    int updateTimeSpan = gameEng.DisappearLifePackList[gameEng.DisappearLifePackList.Count - 1].DisappearTime
                        - gameEng.DisappearLifePackList[gameEng.DisappearLifePackList.Count - 2].DisappearTime;
                    if (updateTimeSpan != 0)
                        this.lifePackDisappearTimer.Interval = updateTimeSpan;
                    else
                        this.lifePackDisappearTimer.Interval = 500;
                    this.nextLifePackToDisappear = null;
                }
                Monitor.Exit(gameEng.AvailableLifePackList);
                Monitor.Exit(gameEng.DisappearLifePackList);
            }
            else
            {
                if (gameEng.AvailableLifePackList.Contains(gameEng.DisappearLifePackList[gameEng.DisappearLifePackList.Count - 1]))
                {
                    Monitor.Enter(gameEng.DisappearLifePackList);
                    Monitor.Enter(gameEng.AvailableLifePackList);
                    Console.WriteLine("LifePack @ " +
                       gameEng.DisappearLifePackList[gameEng.DisappearLifePackList.Count - 1].Position + " VANISHED");
                    gui.RemoveMapItem(gameEng.DisappearLifePackList[gameEng.DisappearLifePackList.Count - 1].Position.X, gameEng.DisappearLifePackList[gameEng.DisappearLifePackList.Count - 1].Position.Y);
                    gameEng.AvailableLifePackList.Remove(this.nextLifePackToDisappear);
                    Monitor.Exit(gameEng.AvailableLifePackList);
                    Monitor.Exit(gameEng.DisappearLifePackList);
                    //No need to call game finish here since the lifepacks have no impact on the endgame conditions
                }
            }            
        }
                
        /// <summary>
        /// What needs to be done in order to update CoinPiles
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void updateCoinPileTimer_Elapsed(object sender, ElapsedEventArgs e)
        {          
            if (!gameEng.GameFinished && gameEng.CoinPilesToDisclose > 1)
            {
                gameEng.NextCoinPileSend = gameEng.CoinPileList[gameEng.CoinPilesToDisclose - 2].AppearTime -
                                            gameEng.CoinPileList[gameEng.CoinPilesToDisclose - 1].AppearTime;
            }
            else if (!gameEng.GameFinished && gameEng.CoinPilesToDisclose == 1)
            {
                gameEng.NextCoinPileSend = gameEng.CoinPileList[0].AppearTime -
                    gameEng.CoinPileList[1].AppearTime;
            }
            else
            {
                updateCoinPileTimer.Stop();
                return;
            }
            if (gameEng.NextCoinPileSend != 0)
                updateCoinPileTimer.Interval = gameEng.NextCoinPileSend;
            else//A Default value, the correctness should be evaluated
                updateCoinPileTimer.Interval = 500;
            gameEng.SendCoinPileUpdates();
        }

        void updateLifePackTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!gameEng.GameFinished && gameEng.LifePacksToDisclose > 1)
            {
                gameEng.NextLifePackSend = gameEng.LifePackList[gameEng.LifePacksToDisclose - 2].AppearTime -
                                            gameEng.LifePackList[gameEng.LifePacksToDisclose - 1].AppearTime;
            }
            else if (!gameEng.GameFinished && gameEng.LifePacksToDisclose== 1)
            {
                gameEng.NextLifePackSend = gameEng.LifePackList[0].AppearTime -
                    gameEng.LifePackList[1].AppearTime;
            }
            else
            {
                this.updateLifePackTimer.Stop();
                return;
            }
            if (gameEng.NextLifePackSend != 0)
                this.updateLifePackTimer.Interval = gameEng.NextLifePackSend;
            else//A Default value, the correctness should be evaluated
                this.updateLifePackTimer.Interval = 500;
            gameEng.SendLifePackUpdates();           
        }

        /// <summary>
        /// What needs to be done when the full game time is up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void lifeTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (this.gameStarterThread.CancellationPending)
                this.gameStarterThread.Dispose();
            if (!gameEng.GameFinished)
            {
                this.StopTimers();
                gameEng.FinishGame();

                Console.WriteLine("\n*******************************************************");
                Console.WriteLine("-----------------GAME JUST FINISHED--------------------");
                Console.WriteLine("REASON: The Time Is Up!\n");
                foreach (Contestant c in gameEng.PlayerList)
                {
                    Console.WriteLine("NAME: " + c.Name + "\tPOINTS: " + c.PointsEarned + "\tALIVE: " + c.IsAlive);
                }
                Console.WriteLine("\n*******************************************************\n\n");
                gameEng.Initialize();
            }
        }

        /// <summary>
        /// What needs to be done in order to update player status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void playerUpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (gameEng.GameFinished)
            {
                this.playerUpdateTimer.Stop();
            }
            else
            {
                gameEng.UpdateBullets();   
                if (updateCounter == Constant.BULLET_MULTI)
                {
                    gameEng.SendPlayerUpdates();
                    updateCounter = -1;
                }                            
                updateCounter++;
            }
        }
       
        #endregion

        GUI.GUII gui;
        internal void SetGUI(GUI.GUII gui)
        {
            gameEng.SetGUI(gui);
            this.gui = gui;
        }

        #endregion

       
    }
}
