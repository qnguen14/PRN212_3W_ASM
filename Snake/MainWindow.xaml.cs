using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Snake
{
    public partial class MainWindow : Window
    {
        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
        {
            { GridValue.Empty, Images.Empty },
            { GridValue.Snake, Images.Body },
            { GridValue.Food, Images.Food },
        };

        private readonly Dictionary<Direction, int> dirToRotation = new()
        {
            { Direction.Up, 0 },
            { Direction.Right, 90 },
            { Direction.Down, 180 },
            { Direction.Left, 270 },
        };

        private const int rows = 15, cols = 15;
        private readonly Image[,] gridImages;
        private GameState gameState;
        private bool gameRunning;
        private bool isTimedMode;

        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid();
            ResetGame();
        }

        private void ResetGame()
        {
            gameState = new GameState(rows, cols);
            gameRunning = false;
            Overlay.Visibility = Visibility.Visible;
            MenuPanel.Visibility = Visibility.Visible;
            OverlayText.Text = "SNAKE GAME";
            TimeText.Text = "";
        }

        private async Task RunGame()
        {
            gameState.Reset();
            Draw();
            await ShowCountDown();

            Overlay.Visibility = Visibility.Hidden;

            await GameLoop();

            await ShowGameOver();
            ResetGame();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }
            else if (!gameRunning)
            {
                StartGame();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver) return;

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

        private async void StartGame()
        {
            gameRunning = true;
            await RunGame();
            gameRunning = false;
        }

        private async Task GameLoop()
        {
            while (!gameState.GameOver)
            {
                await Task.Delay(100);
                gameState.Move();

                if (isTimedMode && !gameState.UpdateTimer())
                {
                    break; // End game if time runs out
                }

                UpdateUI();
            }
        }

        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;
            GameGrid.Width = GameGrid.Height * (cols / (double)rows);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
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

        private void UpdateUI()
        {
            Draw();
            if (isTimedMode)
            {
                TimeText.Text = $"Time: {gameState.RemainingTime}s";
            }
        }

        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();
            ScoreText.Text = $"SCORE: {gameState.Score}";
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
            for (int i = 3; i >= 1; i--)
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(500);
            }
        }

        private async Task DrawDeadSnake()
        {
            foreach (var position in gameState.SnakePosition())
            {
                ImageSource source = position == gameState.HeadPosition() ? Images.DeadHead : Images.DeadBody;
                gridImages[position.Row, position.Col].Source = source;
                await Task.Delay(50);
            }
        }

        private void DrawSnakeHead()
        {
            Position headPos = gameState.HeadPosition();
            Image image = gridImages[headPos.Row, headPos.Col];
            image.Source = Images.Head;
            image.RenderTransform = new RotateTransform(dirToRotation[gameState.Dir]);
        }

        private async Task ShowGameOver()
        {
            await DrawDeadSnake();
            await Task.Delay(1000);
            Overlay.Visibility = Visibility.Visible;

            OverlayText.Text = $"GAME OVER\nScore: {gameState.Score}";
        }

        private async void ClassicMode_Click(object sender, RoutedEventArgs e)
        {
            isTimedMode = false;
            StartGame();
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void TimedMode_Click(object sender, RoutedEventArgs e)
        {
            isTimedMode = true;
            TimeText.Text = $"Time: {GameState.TOTAL_GAME_TIME}s";
            StartGame();
        }
    }
}
