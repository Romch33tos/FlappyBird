using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace FlappyBird
{
  public class FlappyBirdGame : Form
  {
    private const int Gravity = 1;
    private const int JumpForce = -12;
    private const int PipeSpeed = 4;
    private const int PipeGap = 180;
    private const int PipeWidth = 70;
    private const int FrameDelay = 5;
    private const int BirdWidth = 50;
    private const int BirdHeight = 40;
    private const int BirdInitialX = 100;
    private const int BirdInitialY = 250;

    private enum GameState { WaitingToStart, Playing, GameOver }
    private GameState currentGameState = GameState.WaitingToStart;
    private int highScore = 0;
    private int currentScore = 0;
    private string highScoreFilePath = "highscore.txt";

    private Timer gameTimer;
    private List<Bitmap> birdAnimationFrames = new List<Bitmap>();
    private Bitmap backgroundImage;
    private Bitmap pipeTopImage;
    private Bitmap pipeBottomImage;
    private Font uiFont = new Font("Arial", 16, FontStyle.Bold);
    private Random random = new Random();

    private int birdYPosition = BirdInitialY;
    private int birdVerticalVelocity = 0;
    private int pipeXPosition = 0;
    private int topPipeHeight = 0;
    private int bottomPipeHeight = 0;
    private int currentAnimationFrame = 0;
    private int frameCounter = 0;

    public FlappyBirdGame()
    {
      InitializeGameWindow();
      LoadGameAssets();
      LoadHighScore();
      InitializeGameTimer();
      GeneratePipes();
    }

    private void InitializeGameWindow()
    {
      this.Text = "Flappy Bird";
      this.ClientSize = new Size(400, 500);
      this.DoubleBuffered = true;
      this.KeyDown += HandleKeyInput;
      this.BackColor = Color.SkyBlue;
    }

    private void LoadGameAssets()
    {
      try
      {
        birdAnimationFrames.Add(new Bitmap("bird1.png"));
        birdAnimationFrames.Add(new Bitmap("bird2.png"));
        birdAnimationFrames.Add(new Bitmap("bird3.png"));

        for (int frameIndex = 0; frameIndex < birdAnimationFrames.Count; frameIndex++)
        {
          birdAnimationFrames[frameIndex] = new Bitmap(birdAnimationFrames[frameIndex],
              new Size(BirdWidth, BirdHeight));
        }

        backgroundImage = new Bitmap("background.png");
        backgroundImage = new Bitmap(backgroundImage, this.ClientSize);

        pipeTopImage = new Bitmap("pipe_top.png");
        pipeBottomImage = new Bitmap("pipe_bottom.png");
    
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Ошибка загрузки изображений: {ex.Message}\nИспользуются стандартные графические элементы.");
        CreatePlaceholderGraphics();
      }
    }

    private void CreatePlaceholderGraphics()
    {
      birdAnimationFrames.Add(CreateColoredBitmap(BirdWidth, BirdHeight, Color.Red));
      backgroundImage = CreateColoredBitmap(ClientSize.Width, ClientSize.Height, Color.SkyBlue);
      pipeTopImage = CreateColoredBitmap(PipeWidth, 300, Color.Green);
      pipeBottomImage = CreateColoredBitmap(PipeWidth, 300, Color.Green);
    }

    private Bitmap CreateColoredBitmap(int width, int height, Color color)
    {
      Bitmap bmp = new Bitmap(width, height);
      using (Graphics g = Graphics.FromImage(bmp))
      {
        g.Clear(color);
      }
      return bmp;
    }

    private void LoadHighScore()
    {
      try
      {
        if (File.Exists(highScoreFilePath))
        {
          highScore = int.Parse(File.ReadAllText(highScoreFilePath));
        }
      }
      catch
      {
      }
    }

    private void SaveHighScore()
    {
      try
      {
        File.WriteAllText(highScoreFilePath, highScore.ToString());
      }
      catch
      {
      }
    }

    private void InitializeGameTimer()
    {
      gameTimer = new Timer { Interval = 15 };
      gameTimer.Tick += UpdateGame;
      gameTimer.Start();
    }

    private void HandleKeyInput(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Space)
      {
        switch (currentGameState)
        {
          case GameState.WaitingToStart:
            StartGame();
            break;
          case GameState.Playing:
            birdVerticalVelocity = JumpForce;
            break;
          case GameState.GameOver:
            ResetGame();
            break;
        }
      }
    }

    private void StartGame()
    {
      currentGameState = GameState.Playing;
    }

    private void UpdateGame(object sender, EventArgs e)
    {
      switch (currentGameState)
      {
        case GameState.Playing:
          UpdateGameState();
          break;
      }
      UpdateAnimation();
      this.Invalidate();
    }

    private void UpdateGameState()
    {
      birdVerticalVelocity += Gravity;
      birdYPosition += birdVerticalVelocity;

      pipeXPosition -= PipeSpeed;

      CheckCollisions();

      if (pipeXPosition < -PipeWidth)
      {
        pipeXPosition = this.ClientSize.Width;
        GeneratePipes();
        currentScore++;
        if (currentScore > highScore)
        {
          highScore = currentScore;
          SaveHighScore();
        }
      }
    }

    private void UpdateAnimation()
    {
      frameCounter++;
      if (frameCounter >= FrameDelay)
      {
        frameCounter = 0;
        currentAnimationFrame = (currentAnimationFrame + 1) % birdAnimationFrames.Count;
      }
    }

    private void GeneratePipes()
    {
      int minPipeHeight = 50;
      int maxAvailableHeight = this.ClientSize.Height - PipeGap - minPipeHeight;
      topPipeHeight = random.Next(minPipeHeight, maxAvailableHeight);
      bottomPipeHeight = this.ClientSize.Height - topPipeHeight - PipeGap;
    }

    private void CheckCollisions()
    {
      if (birdYPosition < 0 || birdYPosition > this.ClientSize.Height - BirdHeight)
      {
        EndGame();
        return;
      }

      Rectangle birdBounds = new Rectangle(BirdInitialX, birdYPosition, BirdWidth, BirdHeight);
      Rectangle topPipeBounds = new Rectangle(pipeXPosition, 0, PipeWidth, topPipeHeight);
      Rectangle bottomPipeBounds = new Rectangle(pipeXPosition,
          this.ClientSize.Height - bottomPipeHeight, PipeWidth, bottomPipeHeight);

      if (birdBounds.IntersectsWith(topPipeBounds) || birdBounds.IntersectsWith(bottomPipeBounds))
      {
        EndGame();
      }
    }

    private void EndGame()
    {
      currentGameState = GameState.GameOver;
      gameTimer.Stop();
    }

    private void ResetGame()
    {
      birdYPosition = BirdInitialY;
      birdVerticalVelocity = 0;
      pipeXPosition = this.ClientSize.Width;
      currentScore = 0;
      currentGameState = GameState.Playing;
      GeneratePipes();
      gameTimer.Start();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      Graphics graphics = e.Graphics;

      graphics.DrawImage(backgroundImage, 0, 0, this.ClientSize.Width, this.ClientSize.Height);

      graphics.DrawImage(pipeTopImage, pipeXPosition, 0, PipeWidth, topPipeHeight);
      graphics.DrawImage(pipeBottomImage, pipeXPosition,
          this.ClientSize.Height - bottomPipeHeight, PipeWidth, bottomPipeHeight);

      if (birdAnimationFrames.Count > 0)
      {
        graphics.DrawImage(birdAnimationFrames[currentAnimationFrame], BirdInitialX, birdYPosition, BirdWidth, BirdHeight);
      }
      
      SizeF scoreSize = graphics.MeasureString($"Счет: {currentScore}", uiFont);
      SizeF highScoreSize = graphics.MeasureString($"Рекорд: {highScore}", uiFont);

      float rightMargin = 20;
      float scoreY = 20;
      float highScoreY = 50;

      graphics.DrawString($"Счет: {currentScore}", uiFont, Brushes.White,
          this.ClientSize.Width - scoreSize.Width - rightMargin, scoreY);
      graphics.DrawString($"Рекорд: {highScore}", uiFont, Brushes.White,
          this.ClientSize.Width - highScoreSize.Width - rightMargin, highScoreY);

      switch (currentGameState)
      {
        case GameState.WaitingToStart:
          DrawCenteredMessage(graphics, "Нажмите ПРОБЕЛ чтобы начать", Color.White);
          break;
        case GameState.GameOver:
          DrawCenteredMessage(graphics, "Игра окончена! Нажмите ПРОБЕЛ чтобы начать заново", Color.Red);
          break;
      }
    }

    private void DrawCenteredMessage(Graphics graphics, string message, Color color)
    {
      SizeF textSize = graphics.MeasureString(message, uiFont);
      graphics.DrawString(message, uiFont, new SolidBrush(color),
          (this.ClientSize.Width - textSize.Width) / 2,
          (this.ClientSize.Height - textSize.Height) / 2);
    }

    [STAThread]
    public static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new FlappyBirdGame());
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        foreach (var frame in birdAnimationFrames) frame?.Dispose();
        backgroundImage?.Dispose();
        pipeTopImage?.Dispose();
        pipeBottomImage?.Dispose();
        gameTimer?.Dispose();
        uiFont?.Dispose();
      }
      base.Dispose(disposing);
    }
  }
}
