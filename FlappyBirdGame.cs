using System;
using System.Drawing;
using System.Windows.Forms;

namespace FlappyBird
{
  public class FlappyBirdGame : Form
  {
    private GameStateManager stateManager;
    private GameRenderer renderer;
    private GameController controller;
    private System.Windows.Forms.Timer gameUpdateTimer;

    public FlappyBirdGame()
    {
      InitializeGame();
    }

    private void InitializeGame()
    {
      this.Text = "Flappy Bird";
      this.ClientSize = new Size(400, 500);
      this.DoubleBuffered = true;
      this.BackColor = Color.SkyBlue;
      this.MinimizeBox = false;
      this.FormBorderStyle = FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;

      stateManager = new GameStateManager(this.ClientSize);
      renderer = new GameRenderer(stateManager);
      controller = new GameController(stateManager);

      this.KeyDown += controller.ProcessKeyboardInput;
      InitializeGameTimer();
    }

    private void InitializeGameTimer()
    {
      gameUpdateTimer = new System.Windows.Forms.Timer { Interval = 15 };
      gameUpdateTimer.Tick += (sender, e) =>
      {
        stateManager.Update();
        this.Invalidate();
      };
      gameUpdateTimer.Start();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      renderer.Render(e.Graphics, this.ClientSize);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        renderer.Dispose();
        gameUpdateTimer?.Dispose();
      }
      base.Dispose(disposing);
    }
  }
}
