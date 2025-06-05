using System.Windows.Forms;

namespace FlappyBird
{
  public class GameController
  {
    private GameStateManager stateManager;

    public GameController(GameStateManager stateManager)
    {
      this.stateManager = stateManager;
    }

    public void ProcessKeyboardInput(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Space)
      {
        switch (stateManager.CurrentState)
        {
          case GameStateManager.GameState.WaitingToStart:
            stateManager.StartGame();
            break;
          case GameStateManager.GameState.Playing:
            stateManager.Jump();
            break;
          case GameStateManager.GameState.GameOver:
            stateManager.RestartGame();
            break;
        }
      }
    }
  }
}