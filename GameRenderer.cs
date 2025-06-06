using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FlappyBird
{
  public class GameRenderer : IDisposable
  {
    private GameStateManager stateManager;
    private List<Bitmap> birdFrames = new List<Bitmap>();
    private Bitmap background;
    private Bitmap upperPipe;
    private Bitmap lowerPipe;
    private Font font = new Font("Calibri", 16, FontStyle.Bold);

    public GameRenderer(GameStateManager stateManager)
    {
      this.stateManager = stateManager;
      LoadResources();
    }

    private void LoadResources()
    {
      birdFrames.Add(new Bitmap("bird1.png"));
      birdFrames.Add(new Bitmap("bird2.png"));
      birdFrames.Add(new Bitmap("bird3.png"));
      background = new Bitmap("background.png");
      upperPipe = new Bitmap("pipe_top.png");
      lowerPipe = new Bitmap("pipe_bottom.png");
    }

    public void Render(Graphics g, Size clientSize)
    {
      g.DrawImage(background, 0, 0, clientSize.Width, clientSize.Height);

      if (stateManager.CurrentState == GameStateManager.GameState.Playing)
      {
        g.DrawImage(upperPipe, new Rectangle(stateManager.PipeXPosition, 0, 70, stateManager.UpperPipeHeight), new Rectangle(0, 0, upperPipe.Width, upperPipe.Height), GraphicsUnit.Pixel);
        g.DrawImage(lowerPipe, new Rectangle(stateManager.PipeXPosition, clientSize.Height - stateManager.LowerPipeHeight, 70, stateManager.LowerPipeHeight), new Rectangle(0, 0, lowerPipe.Width, lowerPipe.Height), GraphicsUnit.Pixel);
        g.DrawImage(birdFrames[stateManager.CurrentFrameIndex], 100, stateManager.BirdYPosition, 50, 40);
      }

      DrawScore(g, clientSize);
      DrawMessages(g, clientSize);
    }

    private void DrawScore(Graphics g, Size clientSize)
    {
      string scoreText = $"Счет: {stateManager.CurrentScore}";
      string highScoreText = $"Рекорд: {stateManager.HighScore}";

      SizeF scoreSize = g.MeasureString(scoreText, font);
      SizeF highScoreSize = g.MeasureString(highScoreText, font);

      float margin = 20;
      float scoreY = 20;
      float highScoreY = 50;

      g.DrawString(scoreText, font, Brushes.White, clientSize.Width - scoreSize.Width - margin, scoreY);
      g.DrawString(highScoreText, font, Brushes.White, clientSize.Width - highScoreSize.Width - margin, highScoreY);
    }

    private void DrawMessages(Graphics g, Size clientSize)
    {
      string message = "";
      Color color = Color.White;

      switch (stateManager.CurrentState)
      {
        case GameStateManager.GameState.WaitingToStart:
          message = "Нажмите пробел, чтобы начать!";
          break;
        case GameStateManager.GameState.GameOver:
          message = "Конец игры! Нажмите пробел для перезапуска!";
          break;
      }

      if (!string.IsNullOrEmpty(message))
      {
        StringFormat format = new StringFormat();
        format.Alignment = StringAlignment.Center;
        format.LineAlignment = StringAlignment.Center;

        g.DrawString(message, font, new SolidBrush(color),
          new RectangleF(0, 0, clientSize.Width, clientSize.Height), format);
      }
    }

    public void Dispose()
    {
      foreach (var frame in birdFrames) frame?.Dispose();
      background?.Dispose();
      upperPipe?.Dispose();
      lowerPipe?.Dispose();
      font?.Dispose();
    }
  }
}