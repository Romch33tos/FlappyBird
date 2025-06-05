using System;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace FlappyBird
{
  public class FlappyBirdGame : Form
  {
    private readonly Timer gameTimer;
    private readonly Bitmap birdImage;
    private readonly Bitmap pipeTopImage;
    private readonly Bitmap pipeBottomImage;

    private int birdY = 250;
    private int birdVelocity = 0;
    private const int Gravity = 1;
    private const int PipeSpeed = 4;
    private int score = 0;
    private const int PipeGap = 180;
    private const int PipeWidth = 70;
    private int pipeX = 400;
    private int pipeHeightTop;
    private int pipeHeightBottom;
    private bool gameOver = false;
    private readonly Font scoreFont = new Font("Arial", 16, FontStyle.Bold);
    private readonly Brush skyBrush = new SolidBrush(Color.SkyBlue);

    private const int BirdWidth = 60;
    private const int BirdHeight = 60;
    private const int BirdX = 100;

    public FlappyBirdGame()
    {
      try
      {
        birdImage = new Bitmap("bird.png");
        pipeTopImage = new Bitmap("pipe_top.png");
        pipeBottomImage = new Bitmap("pipe_bottom.png");

        birdImage = new Bitmap(birdImage, new Size(BirdWidth, BirdHeight));
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Ошибка загрузки изображений: {ex.Message}\nИгра будет использовать стандартные фигуры.");
        birdImage = CreateColoredBitmap(BirdWidth, BirdHeight, Color.Red);
        pipeTopImage = CreateColoredBitmap(PipeWidth, 300, Color.Green);
        pipeBottomImage = CreateColoredBitmap(PipeWidth, 300, Color.Green);
      }

      this.Text = "Flappy Bird";
      this.ClientSize = new Size(400, 500);
      this.DoubleBuffered = true;
      this.KeyDown += OnKeyDown;
      this.BackColor = Color.SkyBlue;

      gameTimer = new Timer();
      gameTimer.Interval = 15;
      gameTimer.Tick += GameLoop;
      gameTimer.Start();

      GeneratePipes();
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

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Space)
      {
        if (gameOver)
        {
          ResetGame();
        }
        else
        {
          birdVelocity = -12;
        }
      }
    }

    private void GameLoop(object sender, EventArgs e)
    {
      if (!gameOver)
      {
        birdVelocity += Gravity;
        birdY += birdVelocity;

        pipeX -= PipeSpeed;

        CheckCollisions();

        if (pipeX < -PipeWidth)
        {
          pipeX = this.ClientSize.Width;
          GeneratePipes();
          score++;
        }
      }

      this.Invalidate();
    }

    private void GeneratePipes()
    {
      Random random = new Random();
      int minHeight = 50;
      int maxHeight = this.ClientSize.Height - PipeGap - minHeight;
      pipeHeightTop = random.Next(minHeight, maxHeight);
      pipeHeightBottom = this.ClientSize.Height - pipeHeightTop - PipeGap;
    }

    private void CheckCollisions()
    {
      if (birdY < 0 || birdY > this.ClientSize.Height - BirdHeight)
      {
        GameOver();
      }

      Rectangle birdRect = new Rectangle(BirdX, birdY, BirdWidth, BirdHeight);
      Rectangle topPipeRect = new Rectangle(pipeX, 0, PipeWidth, pipeHeightTop);
      Rectangle bottomPipeRect = new Rectangle(pipeX, this.ClientSize.Height - pipeHeightBottom, PipeWidth, pipeHeightBottom);

      if (birdRect.IntersectsWith(topPipeRect) || birdRect.IntersectsWith(bottomPipeRect))
      {
        GameOver();
      }
    }

    private void GameOver()
    {
      gameOver = true;
      gameTimer.Stop();
    }

    private void ResetGame()
    {
      birdY = 250;
      birdVelocity = 0;
      pipeX = this.ClientSize.Width;
      score = 0;
      gameOver = false;
      GeneratePipes();
      gameTimer.Start();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      Graphics g = e.Graphics;

      g.FillRectangle(skyBrush, 0, 0, this.ClientSize.Width, this.ClientSize.Height);

      g.DrawImage(pipeTopImage, pipeX, 0, PipeWidth, pipeHeightTop);
      g.DrawImage(pipeBottomImage, pipeX, this.ClientSize.Height - pipeHeightBottom, PipeWidth, pipeHeightBottom);

      g.DrawImage(birdImage, BirdX, birdY, BirdWidth, BirdHeight);

      g.DrawString($"Score: {score}", scoreFont, Brushes.Black, 20, 20);

      if (gameOver)
      {
        string gameOverText = "Game Over! Press Space to restart";
        SizeF textSize = g.MeasureString(gameOverText, scoreFont);
        g.DrawString(gameOverText, scoreFont, Brushes.Red,
            (this.ClientSize.Width - textSize.Width) / 2,
            (this.ClientSize.Height - textSize.Height) / 2);
      }
    }

    [STAThread]
    public static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new FlappyBirdGame());
    }
  }
}