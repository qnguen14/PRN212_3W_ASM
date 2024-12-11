using Snake.BLL.Services;
using Snake.DAL.Models;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Snake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public User CurrentUser { get; set; } 

        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
        {
            {GridValue.Empty, Images.Empty },
            {GridValue.Snake, Images.Body },
            {GridValue.Food, Images.Food },
        };

        private readonly Dictionary<Direction, int> dirToRotation = new()
        {
            {Direction.Up, 0 },
            {Direction.Right, 90 },
            {Direction.Down, 180 },
            {Direction.Left, 270 }
        };
        private readonly int rows = 15, cols = 15;
        private readonly Image[,] gridImages;
        private GameState gameState;
        private bool gameRunning;
        // Store the highest score in memory
        private int highestScoreInMemory = 0; 
        //Call Service of LeaderBoard
        private LeaderboardService _LService = new();
        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid();
            gameState = new GameState(rows, cols);
        }

        private async Task RunGame()
        {
            Draw();
            await ShowCountDown();
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();
            await ShowGameOver();
            gameState = new GameState(rows, cols);
        }

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }
            if(!gameRunning)
            {
                gameRunning = true;
                await RunGame();
                gameRunning = false;
            }
        }

        private async void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return;
            }
            switch (e.Key)
            {
                case Key.Left: 
                    gameState.ChangeDirection(Direction.Left); 
                    break;
                case Key.Right: 
                    gameState.ChangeDirection(Direction.Right); 
                    break;
                case Key.Up: 
                    gameState.ChangeDirection(Direction.Up); 
                    break;
                case Key.Down: 
                    gameState.ChangeDirection(Direction.Down); 
                    break;

            }
        }

        private async Task GameLoop()
        {
            while (!gameState.GameOver)
            {
                await Task.Delay(100);
                gameState.Move();
                Draw();
            }
        }
        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;
            GameGrid.Width= GameGrid.Height * (cols / (double)rows);
            for(int r = 0; r < rows; r++)
            {
                for(int c = 0; c < cols; c++)
                {
                    Image image = new Image
                    {
                        Source = Images.Empty,
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    };

                    images[r, c] = image;
                    GameGrid.Children.Add(image);
                }
            }
            return images;
        }

        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();
            ScoreText.Text = $"CORE {gameState.Score}";
            // Update score in memory during gameplay
            UpdateScoreInMemory(CurrentUser.UserId, gameState.Score);
        }
        private void DrawGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    GridValue gridValue = gameState.Grid[r, c];
                    gridImages[r, c].Source = gridValToImage[gridValue];
                    gridImages[r, c].RenderTransform = Transform.Identity;
                }
            }
        }

        private async Task ShowCountDown()
        {
            for( int i = 3; i >= 1; i--)
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(500);
            }
        }

        private async Task DrawDeadSnake()
        {
            List<Position> positions = new List<Position>(gameState.SnakePosition());
            for(int i = 0; i < positions.Count; i++)
            {
                Position position = positions[i];
                ImageSource source = (i == 0) ? Images.DeadHead : Images.DeadBody;
                gridImages[position.Row, position.Col].Source = source;
                await Task.Delay(50);
            }
        }
        private void DrawSnakeHead()
        {
            Position headPos = gameState.HeadPosition();
            Image image = gridImages[headPos.Row, headPos.Col];
            image.Source = Images.Head;

            int rotation = dirToRotation[gameState.Dir];
            image.RenderTransform = new RotateTransform(rotation);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        { 
            if (CurrentUser == null)
            {
                LoggedIn.Text = "Hello, NOT LOGGED IN";
                return;
            }


            LoggedIn.Text = $"Hello, {CurrentUser.Username}";
            //Load RankingBoard
            FillRankBoard(_LService.GetAllScores());
        }
        private void FillRankBoard(List<Leaderboard> arr)
        {
            var sortedList = arr.OrderByDescending(item => item.Score).Take(5).ToList();

            
            Ranking.ItemsSource = null;
            Ranking.ItemsSource = sortedList;
        }
        private void SaveScoreToDatabase(int UserId, int Score)
        {
            if (CurrentUser != null)
            {
                try
                {
                    var allScores = _LService.GetAllScores();
                    var existingScore = allScores.FirstOrDefault(x => x.UserId == UserId);

                    if (existingScore != null)
                    {
                        if (Score > existingScore.Score)
                        {
                            existingScore.Score = Score;
                            _LService.UpdateScore(existingScore);
                            ShowNewRecordOnScreen();
                        }
                    }
                    else
                    {
                        var newScore = new Leaderboard
                        {
                            UserId = UserId,
                            Score = Score,
                            AchievedAt = DateTime.Now
                        };

                        _LService.SaveScore(newScore);
                    }
                    FillRankBoard(_LService.GetAllScores());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to save the score: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void UpdateScoreInMemory(int UserId, int Score)
        {
            var allScores = _LService.GetAllScores();
            var userScore = allScores.FirstOrDefault(x => x.UserId == UserId);
            if (userScore != null) highestScoreInMemory = userScore.Score;

            var leaderboard = (List<Leaderboard>)Ranking.ItemsSource;
            if (Score > highestScoreInMemory)
            {
                highestScoreInMemory = Score; // Update in-memory score

                // Find the specific user's record in DataGrid

                var existingScore = leaderboard.FirstOrDefault(x => x.UserId == UserId);

                if (existingScore != null)
                {
                    // Update the score directly in DataGrid's source
                    existingScore.Score = Score;
                }
                else
                {
                    // Add new record if not exist
                    leaderboard.Add(new Leaderboard
                    {
                        UserId = UserId,
                        Score = Score,
                        AchievedAt = DateTime.Now,
                        User = CurrentUser
                    });
                }

                // Refresh DataGrid
                FillRankBoard(leaderboard);
            }

        }

        private void ShowNewRecordOnScreen()
        {
            NewRecordText.Visibility = Visibility.Visible;

            // hide the message after 1.5 seconds
            Task.Delay(1500).ContinueWith(_ =>
            {
                Dispatcher.Invoke(() =>
                {
                    NewRecordText.Visibility = Visibility.Collapsed;
                });
            });
        }

        private async Task ShowGameOver()
        {
            await DrawDeadSnake();
            await Task.Delay(1000);
            //save score when game over
            SaveScoreToDatabase(CurrentUser.UserId, highestScoreInMemory);
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "PRESS ANY KEY TO START";
        }
    }
}