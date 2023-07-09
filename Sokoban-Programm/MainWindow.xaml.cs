using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Security.Principal;
using System.Windows.Media.Media3D;
using System.Timers;

namespace Sokoban_Programm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //Déclaration Variable
        RowDefinition rowDefinition = new RowDefinition();
        ColumnDefinition columnDefinition = new ColumnDefinition();

        List<RowDefinition> rows = new List<RowDefinition>();
        List<ColumnDefinition> columns = new List<ColumnDefinition>();

        DispatcherTimer timer = new DispatcherTimer();

        Level Level;

        UCCell[,] blocs;
        UCCell bloc = new UCCell();

        char[,] grille;

        char lastBloc = ' ';
        char nextbloc;

        int width=0;
        int height=0;

        int nbRow;
        int nbColumn;


        int nbRefreshed;
        int[] playerIndex = new int[2];

        int[] widthAndHeight = new int[3];

        string[] lines;


        //levels
        int level = 1;
        string[] difficulty = new string[3];
        int difficultyLevel = 0;
        int fileCount;



        //UI
        TimeSpan timePlaying = new TimeSpan();
        string[] difficultyName = new string[3];



        //win
        int nbTargets;
        int nbBoxtargets;

        int CurrentTime;

        Int64 nbResets = new Int64();
        int score;

        //int for player movement
        int deltaX;
        int deltaY;

        int playerDirection = 3;

        //int for box movement
        int boxDeltaX;
        int boxDeltaY;

        //Save

        //Bool for special movement

        bool onTarget = false;
        bool boxOntarget = false;
        bool nextBox = false;
        bool isWall = false;
        bool nextBoxWall = false;
        


        public MainWindow()
        {
            InitializeComponent();
            timer.Start();

            timer.Tick += timerEvent;
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            difficulty[0] = "1 First steps - Beginner";
            difficulty[1] = "2 First steps - Advanced";
            difficulty[2] = "3 First steps - Expert";

            difficultyName[0] = "Beginner";
            difficultyName[1] = "Advanced";
            difficultyName[2] = "Expert";

            RBtnBeginner.IsChecked = true;
            LoadSave();

            LblResets.Content = ("Resets: " + nbResets).ToString();
            LblScore.Content = ($"Coins: {score}");

            LevelsLoading();
        }



        private void Reset()
        {
            playerDirection = 3;
            onTarget = false;
            boxOntarget = false;
            nextBox = false;
            isWall = false;
            nextBoxWall = false;
            lastBloc = ' ';
            deltaX = 0;
            deltaY = 0;
        }

        private void timerEvent(object sender, EventArgs e)
        {
            timePlaying += TimeSpan.FromSeconds(1);
            CurrentTime++;

            LblCounter.Content = timePlaying.ToString();
        }
        
        private void LevelsLoading()
        {
            Level = new Level();
            string levelName = $".\\Map\\{difficulty[difficultyLevel]}\\{difficulty[difficultyLevel]}_{level}.txt";

            Level.Load(levelName);
            width = Level.width;
            height = Level.height;
            grille = new char[Level.width, Level.height];
            grille = Level.grille;
            
            Start_Generation();

        }

        private void Start_Generation()
        {

            LblCounter.Content=timePlaying.ToString();
            blocs = new UCCell[width, height];
            LblLevel.Content = ($"Level {level} {difficultyName[difficultyLevel]}").ToString();

            
            
            Reset();

            grdDisplay.Children.Clear();
            grdDisplay.RowDefinitions.Clear();
            grdDisplay.ColumnDefinitions.Clear();
            
            


            for (int y = 0; y < height; y++)
            {
                rowDefinition = new RowDefinition();
                rowDefinition.Height = GridLength.Auto;
                grdDisplay.RowDefinitions.Add(rowDefinition);

            }
            for (int x = 0; x < width; x++)
            {
                columnDefinition = new ColumnDefinition();
                columnDefinition.Width = GridLength.Auto;
                grdDisplay.ColumnDefinitions.Add(columnDefinition);
            }
            

            //Start blocs
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bloc = new UCCell();
                    Grid.SetColumn(bloc, x);
                    Grid.SetRow(bloc, y);

                    grdDisplay.Children.Add(bloc);
                    blocs[x,y] = bloc;
                }
            }

            widthAndHeight[0] = 1900 / width;
            widthAndHeight[1] = 1900 / height;
            widthAndHeight[2] = (widthAndHeight[0] + widthAndHeight[1]) / 5;
            Debug.WriteLine(widthAndHeight[2]);


            //Find Player

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (grille[x, y] == '@' || grille[x,y] == '+')
                    {
                        playerIndex[0] = x;
                        playerIndex[1] = y;
                    }
                    if (grille[x, y] == '+')
                    {
                        lastBloc = '.';
                    }
                }


            }

            FindTargets();

            //refresh
            View();
        }

        private void FindTargets()
        {
            //find targets
            nbTargets = 0;
            nbBoxtargets = 0;
            foreach (var item in grille)
            {
                if (item == '.' | item == '*')
                {
                    nbTargets++;
                }
            }
        }

        private void View()
        {
            
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {

                    blocs[x, y].WidthHeightUpdate(widthAndHeight[2]);

                    blocs[x, y].PlayerDirection(playerDirection);

                    if (grille[x, y] == '#')
                    {
                        blocs[x, y].Wall();
                    }
                    else if (grille[x, y] == '$')
                    {
                        blocs[x, y].Crate();
                    }
                    else if (grille[x, y] == '@')
                    {
                        blocs[x, y].PlayerGround();
                    }
                    else if (grille[x, y] == ' ')
                    {
                        blocs[x, y].Ground();
                    }
                    else if (grille[x, y] == '.')
                    {
                        blocs[x, y].Target();
                    }
                    else if (grille[x, y] == '+')
                    {
                        blocs[x, y].PlayerTarget();
                    }
                    else if (grille[x, y] == '*')
                    {
                        blocs[x, y].CrateTarget();
                    }
                }
            }
        }
        

        
        private void MovementTester()
        {

            //taking and verify next bloc
            nextbloc = grille[deltaX, deltaY];

            //box tester
            if (nextbloc == '$' | nextbloc == '*')
            {
                nextBox = true;
            }
            else
            {
                nextBox = false;
            }

            //target tester
            if (nextbloc == '.' | nextbloc == '*')
            {
                onTarget = true;
            }
            else
            {
                onTarget = false;
            }

            //wall tester
            if (nextbloc == '#')
            {
                isWall = true;
            }
            else
            {
                isWall = false;
            }

            //box target tester
            if (nextBox && grille[boxDeltaX,boxDeltaY] == '.')
            {
                boxOntarget = true;
            }
            else
            {
                boxOntarget = false;
            }


            //box wall tester

            if (!isWall && nextBox)
            {
                if (grille[boxDeltaX, boxDeltaY] == '#' | grille[boxDeltaX, boxDeltaY] == '$' | grille[boxDeltaX, boxDeltaY] == '*')
                {
                    nextBoxWall = true;
                }
                else
                {
                    nextBoxWall = false;
                }
            }
            else
            {
                nextBoxWall = false;
            }

        }
        
        
        
        private void Movement()
        {
            MovementTester();

            if (!isWall && !nextBoxWall)
            {
                
                // replace player by last bloc or ' ' if start
                grille[playerIndex[0], playerIndex[1]] = lastBloc;

                //takes the new last bloc
                lastBloc = grille[deltaX, deltaY];

                //verify if box
                if (lastBloc == '$')
                {
                    lastBloc = ' ';
                }
                else if (lastBloc == '*')
                {
                    lastBloc = '.';
                }

                //condition movement

                //target
                if (!onTarget)
                {
                    grille[deltaX, deltaY] = '@';
                }
                else
                {
                    grille[deltaX, deltaY] = '+';
                }
                //box

                if (nextBox)
                {
                    if (boxOntarget)
                    {
                        nextbloc = '*';
                    }
                    else
                    {
                        nextbloc = '$';
                    }
                    grille[boxDeltaX, boxDeltaY] = nextbloc;
                    
                }

                //update player index
                playerIndex[0] = deltaX;
                playerIndex[1] = deltaY;
            }


        }


        private void KeyboardInput(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.D) || Keyboard.IsKeyDown(Key.A) || Keyboard.IsKeyDown(Key.S) || Keyboard.IsKeyDown(Key.W))
            {

                //Direction selection
                if (Keyboard.IsKeyDown(Key.D))
                {
                    deltaX = playerIndex[0] + 1;
                    deltaY = playerIndex[1];
                    boxDeltaX = playerIndex[0] + 2;
                    boxDeltaY = playerIndex[1];
                    playerDirection = 1;
                }
                else if (Keyboard.IsKeyDown(Key.A))
                {
                    deltaX = playerIndex[0] - 1;
                    deltaY = playerIndex[1];
                    boxDeltaX = playerIndex[0] - 2;
                    boxDeltaY = playerIndex[1];
                    playerDirection = 2;
                }
                else if (Keyboard.IsKeyDown(Key.S))
                {
                    deltaX = playerIndex[0];
                    deltaY = playerIndex[1] + 1;
                    boxDeltaX = playerIndex[0];
                    boxDeltaY = playerIndex[1] + 2; ;
                    playerDirection = 3;
                }
                else if (Keyboard.IsKeyDown(Key.W))
                {
                    deltaX = playerIndex[0];
                    deltaY = playerIndex[1] - 1;
                    boxDeltaX = playerIndex[0];
                    boxDeltaY = playerIndex[1] - 2;
                    playerDirection = 4;
                }

                //Movement
                
                Movement();

                //Win?
                Win();


                //Render
                View();
            }
            else if (Keyboard.IsKeyDown(Key.P))
            {
                Debugger();
            }
            else if (Keyboard.IsKeyDown(Key.Escape))
            {
                nbResets++;
                LblResets.Content = ("Resets: "+nbResets).ToString();
                LevelsLoading();
            }
        }

        
        private void Debugger()
        {
            for (int i = 0; i < height; i++)
            {
                for (int a = 0; a < width; a++)
                {
                    Debug.Write(grille[a,i]);
                }
                Debug.WriteLine("");
            }
        }

       
        private void LevelTester()
        {
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo($".\\Map\\{difficulty[difficultyLevel]}");
            fileCount = dir.GetFiles().Length;
            Debug.WriteLine(fileCount);
        }
        
        private void Nextlevel()
        {

            LevelTester();

            level++;

            if (level >= fileCount && difficultyLevel < 2)
            {
                difficultyLevel++;
                level = 0;
                if(difficultyLevel == 0)
                {
                    RBtnBeginner.IsChecked = true;
                }
                else if (difficultyLevel == 1)
                {
                    RBtnAdvanced.IsChecked = true;
                }
                else if (difficultyLevel == 2)
                {
                    RBtnExpert.IsChecked = true;
                }
            }
            else if (level >= fileCount && difficultyLevel >= 2)
            {
                level--;
            }
            CurrentTime = 0;
            LevelsLoading();
        }

        private void PreviousLevel()
        {
            LevelTester();

            level--;
            
            if (level < 0 && difficultyLevel > 0)
            {
                difficultyLevel--;

                LevelTester();
                
                if (difficultyLevel == 0)
                {
                    RBtnBeginner.IsChecked = true;
                }
                else if (difficultyLevel == 1)
                {
                    RBtnAdvanced.IsChecked = true;
                }
                else if (difficultyLevel == 2)
                {
                    RBtnExpert.IsChecked = true;
                }
                level = fileCount - 1;
            }
            else if (level < 0 && difficultyLevel <= 0)
            {
                level++;
            }

            CurrentTime = 0;

            LevelsLoading();
        }

        private void Win()
        {

            nbBoxtargets = 0;
            foreach (var item in grille)
            {
                if (item == '*')
                {
                    nbBoxtargets++;
                }
            }

            if (nbBoxtargets == nbTargets)
            {
                View();
                MessageBox.Show("YOOUUUUUU WIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIN    (^///^)");


                score += 10 - ((CurrentTime / (((difficultyLevel * 3) + 3) * 10)));

                if(score < 0)
                {
                    score = 0;
                }

                LblScore.Content = ($"Coins: {score}");

                Nextlevel();
                
            }

        }

        private void BtnPrevious_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"testeeee {level}");
            PreviousLevel();

        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            Nextlevel();
        }

        private void RBtnBeginner_Checked(object sender, RoutedEventArgs e)
        {
            difficultyLevel = 0;
            level = 0;
            LevelsLoading();
        }

        private void RBtnAdvanced_Checked(object sender, RoutedEventArgs e)
        {
            difficultyLevel = 1;
            level = 0;
            LevelsLoading();
        }

        private void RBtnExpert_Checked(object sender, RoutedEventArgs e)
        {
            difficultyLevel = 2;
            level = 0;
            LevelsLoading();
        }



        private void Save()
        {
            File.WriteAllText(".\\Saves\\Save.txt",$"TotalTime: +{timePlaying}#\nScore: *{score}#\nTotalResets: @{nbResets}#");
        }


        private void DeleteSave()
        {
            if (File.Exists(".\\Saves\\Save.txt"))
            {
                File.Delete(".\\Saves\\Save.txt");
            }
            LblScore.Content = "0";
        }


        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteSave();
            timePlaying = TimeSpan.Zero;
            score = 0;
            nbResets = 0;
            LblResets.Content = ("Resets: " + nbResets).ToString();
            LevelsLoading();
        }



        private void LoadSave()
        {

            if (!File.Exists(".\\Saves\\Save.txt"))
            {
                Save();
            }

            string[] save = File.ReadAllLines(".\\Saves\\Save.txt");

            int processLine = 0;
            foreach (var line in save)
            {


                int wichLine = 0;
                bool takeValue = false;

                string newLine="";

                foreach (var item in line)
                {
                    if(takeValue && item != '#')
                    {
                        newLine += item;
                    }

                    if (item == '@' || item == '*' || item == '+')
                    {
                        takeValue = true;
                        
                    }
                    else if (item == '#')
                    {
                        takeValue = false;
                    }

                    
                    
                }

                if(processLine == 0)
                {
                    TimeSpan.TryParse(newLine, out timePlaying);
                }
                else if (processLine == 1)
                {
                    int.TryParse(newLine, out score);
                }
                else if (processLine == 2)
                {
                    Int64.TryParse(newLine, out nbResets);
                }


                processLine++;
            }
        }
    }
}
