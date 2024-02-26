// Vaishnavi Barot
// Student ID - 8975398

using System;

// Represents the position of a player on the game board
public class Position
{
    public int X { get; private set; }
    public int Y { get; private set; }

    // Constructor to initialize the position with X and Y coordinates
    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }

    // Move the position based on the given direction
    public void Move(char direction)
    {
        switch (direction)
        {
            case 'U':
                Y--;
                break;
            case 'D':
                Y++;
                break;
            case 'L':
                X--;
                break;
            case 'R':
                X++;
                break;
        }
    }
}

// Represents a player in the game
public class Player
{
    public string Name { get; } // Name of the player
    public Position Position { get; set; } // Current position of the player on the board
    public int GemCount { get; set; } // Number of gems collected by the player
    public int MoveCount { get; private set; } // Number of moves made by the player

    // Constructor to initialize the player with a name and starting position
    public Player(string name, Position position)
    {
        Name = name;
        Position = position;
        GemCount = 0;
        MoveCount = 0;
    }

    // Move the player in a direction
    public void Move(char direction)
    {
        Position.Move(direction);
        MoveCount++;
    }
}

// Represents a cell on the game board
public class Cell
{
    public char Occupant { get; set; } // The occupant of the cell (player, gem, obstacle, or empty)

    // Constructor to initialize the cell with a default occupant of '-'
    public Cell()
    {
        Occupant = '-';
    }
}

// Represents the game board
public class Board
{
    public const int Size = 6; // Size of the game board (6x6)
    public const int MaxTurns = 30; // Maximum number of turns allowed in the game

    private readonly Cell[,] _grid; // 2D array to represent the grid of cells on the board

    // Constructor to initialize the game board and populate it with players, gems, and obstacles
    public Board()
    {
        _grid = new Cell[Size, Size]; // Initialize the grid

        // Populate the board with players, gems, and obstacles
        PopulateBoard();
    }

    // Populate the game board with players, gems, and obstacles
    private void PopulateBoard()
    {
        // Place players at the starting and ending positions
        _grid[0, 0] = new Cell { Occupant = 'P' }; // Player 1 starting position
        _grid[Size - 1, Size - 1] = new Cell { Occupant = 'P' }; // Player 2 starting position

        // Randomly place gems on the board
        Random random = new Random();
        for (int i = 0; i < Size; i++)
        {
            int x = random.Next(Size);
            int y = random.Next(Size);
            while (_grid[y, x] != null)
            {
                x = random.Next(Size);
                y = random.Next(Size);
            }
            _grid[y, x] = new Cell { Occupant = 'G' }; // Place a gem in the cell
        }

        // Randomly place obstacles on the board
        for (int i = 0; i < Size; i++)
        {
            int x = random.Next(Size);
            int y = random.Next(Size);
            while (_grid[y, x] != null)
            {
                x = random.Next(Size);
                y = random.Next(Size);
            }
            _grid[y, x] = new Cell { Occupant = 'O' }; // Place an obstacle in the cell
        }
    }

    // Display the current state of the game board
    public void Display(Player player1, Player player2)
    {
        Console.Clear();
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                if (player1.Position.X == j && player1.Position.Y == i)
                {
                    Console.Write('1'); // Display Player 1
                }
                else if (player2.Position.X == j && player2.Position.Y == i)
                {
                    Console.Write('2'); // Display Player 2
                }
                else
                {
                    Console.Write(_grid[i, j]?.Occupant ?? '-'); // Display the occupant of the cell
                }
                Console.Write(" ");
            }
            Console.WriteLine();
        }
        Console.WriteLine($"Player 1 Gems: {player1.GemCount}"); // Display Player 1's collected gems
        Console.WriteLine($"Player 2 Gems: {player2.GemCount}"); // Display Player 2's collected gems
        Console.WriteLine($"Player 1 Moves: {player1.MoveCount}"); // Display Player 1's move count
        Console.WriteLine($"Player 2 Moves: {player2.MoveCount}"); // Display Player 2's move count
    }

    // Check if a move is valid for the given player in the specified direction
    public bool IsValidMove(Player player, char direction)
    {
        int newX = player.Position.X;
        int newY = player.Position.Y;

        switch (direction)
        {
            case 'U':
                newY--;
                break;
            case 'D':
                newY++;
                break;
            case 'L':
                newX--;
                break;
            case 'R':
                newX++;
                break;
        }

        // Check if the new position is within the bounds of the board and not occupied by an obstacle
        if (newX < 0 || newX >= Size || newY < 0 || newY >= Size)
            return false;

        return _grid[newY, newX]?.Occupant != 'O';
    }

    // Collect a gem from the player's current position if there is one
    public void CollectGem(Player player)
    {
        if (_grid[player.Position.Y, player.Position.X]?.Occupant == 'G')
        {
            player.GemCount++;
            _grid[player.Position.Y, player.Position.X].Occupant = '-'; // Remove the collected gem from the cell
        }
    }
}

// Represents the game logic and flow
public class Game
{
    private readonly Board _board; // The game board
    private readonly Player _player1; // Player 1
    private readonly Player _player2; // Player 2
    private Player _currentTurn; // Current player's turn
    private int _totalTurns; // Total number of turns taken in the game

    // Constructor to initialize the game with the board and players
    public Game()
    {
        _board = new Board(); // Initialize the game board
        _player1 = new Player("P1", new Position(0, 0)); // Initialize Player 1 with starting position
        _player2 = new Player("P2", new Position(Board.Size - 1, Board.Size - 1)); // Initialize Player 2 with starting position
        _currentTurn = _player1; // Set Player 1 to start the game
        _totalTurns = 0; // Initialize total turns taken to 0
    }

    // Start the game loop
    public void Start()
    {
        // Continue the game loop until the game is over
        while (!IsGameOver())
        {
            _board.Display(_player1, _player2); // Display the current state of the game board
            Console.WriteLine($"It's {_currentTurn.Name}'s turn."); // Display whose turn it is
            Console.WriteLine($"Move count for {_currentTurn.Name}: {_currentTurn.MoveCount}"); // Display move count for the current player
            char direction;
            // Prompt the current player to enter a valid move direction
            do
            {
                Console.Write("Enter direction (U/D/L/R): ");
                direction = char.ToUpper(Console.ReadKey().KeyChar); // Read the player's input
                Console.WriteLine();
                Console.ReadLine(); // Consume newline character
            } while (direction != 'U' && direction != 'D' && direction != 'L' && direction != 'R');

            // Execute the player's move if it's valid
            if (_board.IsValidMove(_currentTurn, direction))
            {
                _currentTurn.Move(direction); // Move the player
                _board.CollectGem(_currentTurn); // Collect gem if present at the player's position
                _totalTurns++; // Increment total turns taken
                SwitchTurn(); // Switch to the next player's turn
            }
            else
            {
                Console.WriteLine("Invalid move. Try again."); // Inform the player of an invalid move
            }
        }

        AnnounceWinner(); // Announce the winner of the game
    }

    // Switch to the next player's turn
    private void SwitchTurn()
    {
        _currentTurn = (_currentTurn == _player1) ? _player2 : _player1; // Toggle between Player 1 and Player 2
    }

    // Check if the game is over (reached maximum turns)
    private bool IsGameOver()
    {
        return _totalTurns >= Board.MaxTurns; // Game is over if total turns taken reaches the maximum turns allowed
    }

    // Announce the winner of the game
    private void AnnounceWinner()
    {
        if (_totalTurns >= Board.MaxTurns) // Check if the game ended due to maximum turns reached
        {
            if (_player1.GemCount > _player2.GemCount)
                Console.WriteLine("Player 1 wins!"); // Player 1 has more gems collected
            else if (_player1.GemCount < _player2.GemCount)
                Console.WriteLine("Player 2 wins!"); // Player 2 has more gems collected
            else
                Console.WriteLine("It's a tie!"); // Equal number of gems collected by both players
        }
        else
        {
            Console.WriteLine("Maximum turns reached. No winner based on gem count."); // Inform that maximum turns were reached without a winner
        }
    }
}

// Entry point of the program
class Program
{
    static void Main(string[] args)
    {
        bool playAgain; // Variable to track if the player wants to play again
        // Start a new game and prompt the player to play again if desired
        do
        {
            Game game = new Game(); // Initialize a new game
            game.Start(); // Start the game

            // Ask the player if they want to play again
            Console.WriteLine("Do you want to play again? (yes/no): ");
            string input = Console.ReadLine().ToLower(); // Read the player's input
            playAgain = input == "yes"; // Set playAgain based on the player's input
        } while (playAgain); // Continue playing if the player wants to play again
    }
}
