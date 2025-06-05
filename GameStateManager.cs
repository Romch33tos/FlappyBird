using System;
using System.Drawing;
using System.IO;

namespace FlappyBird
{
  public class GameStateManager
  {
    public enum GameState { WaitingToStart, Playing, GameOver }
    public GameState CurrentState { get; private set; } = GameState.WaitingToStart;

    private const int GravityValue = 1;
    private const int JumpForceValue = -10;
    private const float GravityMultiplier = 0.7f;
    private const int PipeMovementSpeed = 4;
    private const int VerticalGapBetweenPipes = 180;
    private const int PipeWidthPixels = 70;
    private const int AnimationFrameDelay = 5;
    private const int BirdWidthPixels = 40;
    private const int BirdHeightPixels = 50;
    private const int BirdStartXPosition = 100;
    private const int BirdStartYPosition = 250;
    private const int InitialPipeSpawnOffset = 150;

    private Size gameAreaSize;
    private Random random = new Random();
    private string highScoreFileName = "highscore.txt";

    public int HighScore { get; private set; }
    public int CurrentScore { get; private set; }
    public int BirdYPosition { get; private set; } = BirdStartYPosition;
    public float BirdVerticalSpeed { get; private set; }
    public int PipeXPosition { get; private set; }
    public int UpperPipeHeight { get; private set; }
    public int LowerPipeHeight { get; private set; }
    public int CurrentFrameIndex { get; private set; }
    public int FrameCounter { get; private set; }

    public GameStateManager(Size gameAreaSize)
    {
      this.gameAreaSize = gameAreaSize;
      LoadHighScore();
    }

    public void Update()
    {
      switch (CurrentState)
      {
        case GameState.Playing:
          UpdateGameLogic();
          break;
      }
      UpdateAnimation();
    }

    private void UpdateGameLogic()
    {
      BirdVerticalSpeed += GravityValue * GravityMultiplier;
      BirdYPosition += (int)BirdVerticalSpeed;
      PipeXPosition -= PipeMovementSpeed;

      CheckCollisions();

      if (PipeXPosition < -PipeWidthPixels)
      {
        PipeXPosition = gameAreaSize.Width;
        GenerateNewPipes();
        CurrentScore++;
        if (CurrentScore > HighScore)
        {
          HighScore = CurrentScore;
          SaveHighScore();
        }
      }
    }

    private void UpdateAnimation()
    {
      FrameCounter++;
      if (FrameCounter >= AnimationFrameDelay)
      {
        FrameCounter = 0;
        CurrentFrameIndex = (CurrentFrameIndex + 1) % 3;
      }
    }

    private void CheckCollisions()
    {
      if (BirdYPosition < 0 || BirdYPosition > gameAreaSize.Height - BirdHeightPixels)
      {
        GameOver();
        return;
      }

      Rectangle birdRect = new Rectangle(BirdStartXPosition, BirdYPosition, BirdWidthPixels, BirdHeightPixels);
      Rectangle upperPipeRect = new Rectangle(PipeXPosition, 0, PipeWidthPixels, UpperPipeHeight);
      Rectangle lowerPipeRect = new Rectangle(PipeXPosition, gameAreaSize.Height - LowerPipeHeight, PipeWidthPixels, LowerPipeHeight);

      if (birdRect.IntersectsWith(upperPipeRect) || birdRect.IntersectsWith(lowerPipeRect))
      {
        GameOver();
      }
    }

    public void StartGame()
    {
      CurrentState = GameState.Playing;
      PipeXPosition = gameAreaSize.Width + InitialPipeSpawnOffset;
      GenerateNewPipes();
      CurrentScore = 0;
      BirdYPosition = BirdStartYPosition;
      BirdVerticalSpeed = 0;
    }

    public void RestartGame()
    {
      CurrentState = GameState.Playing;
      PipeXPosition = gameAreaSize.Width + InitialPipeSpawnOffset;
      BirdYPosition = BirdStartYPosition;
      BirdVerticalSpeed = 0;
      CurrentScore = 0;
      GenerateNewPipes();
    }

    public void Jump()
    {
      BirdVerticalSpeed = JumpForceValue;
    }

    private void GameOver()
    {
      CurrentState = GameState.GameOver;
    }

    private void GenerateNewPipes()
    {
      int minPipeHeight = 50;
      int maxPossibleHeight = gameAreaSize.Height - VerticalGapBetweenPipes - minPipeHeight;
      UpperPipeHeight = random.Next(minPipeHeight, maxPossibleHeight);
      LowerPipeHeight = gameAreaSize.Height - UpperPipeHeight - VerticalGapBetweenPipes;
    }

    private void LoadHighScore()
    {
      if (File.Exists(highScoreFileName))
      {
        HighScore = int.Parse(File.ReadAllText(highScoreFileName));
      }
    }

    private void SaveHighScore()
    {
      File.WriteAllText(highScoreFileName, HighScore.ToString());
    }
  }
}
