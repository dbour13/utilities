using System;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Text;

internal class Program
{
    public enum WinningLine { None, FirstHorizontal, SecondHorizontal, ThirdHorizontal, FirstVertical, SecondVertical, ThirdVertical, LeftRightDiagonal, RightLeftDiagonal, Draw};
    public enum Piece { None, X, O};

    public class TicTacToeBoardResult
    {
        public WinningLine Line { get; set; } = WinningLine.None;
        public Piece Winner { get; set; } = Piece.None;
    }

    public class TicTacToeBoard
    {
        public Piece[,] BoardState;
        private delegate bool IsPiece(int x, int y);

        public TicTacToeBoardResult IsWinner()
        {
            Piece winner = Piece.None;

            WinningLine winningLine = IsWinner(IsX);

            if (winningLine == WinningLine.None)
            {
                winningLine = IsWinner(IsO);

                if (winningLine != WinningLine.None
                    && winningLine != WinningLine.Draw)
                {
                    winner = Piece.O;
                }
            }
            else if (winningLine != WinningLine.Draw)
            {
                winner = Piece.X;
            }

            return new TicTacToeBoardResult()
            {
                Line = winningLine, 
                Winner = winner
            };
        }

        private WinningLine IsWinner(IsPiece isPiece)
        {
            // Horizontal 
            for (int i = 0; i < 3; i++)
            {
                if (isPiece(0, i) && isPiece(1, i) && isPiece(2, i))
                {
                    return (WinningLine)i+1;
                }
            }

            // Vertical
            for (int i = 0; i < 3; i++)
            {
                if (isPiece(i, 0) && isPiece(i, 1) && isPiece(i, 2))
                {
                    return (WinningLine)i + 4;
                }
            }

            // Diagonals
            if (isPiece(0,0) && isPiece(1, 1) && isPiece(2, 2))
            {
                return WinningLine.LeftRightDiagonal;
            }

            if (isPiece(2, 0) && isPiece(1, 1) && isPiece(0, 2))
            {
                return WinningLine.RightLeftDiagonal;
            }

            bool draw = true;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (BoardState[i,j] == Piece.None)
                    {
                        draw = false;
                        break;
                    }
                }

                if (!draw)
                {
                    break;
                }
            }

            return draw ? WinningLine.Draw : WinningLine.None;
        }

        private bool IsX(int x, int y)
        {
            return BoardState[x, y] == Piece.X;
        }

        private bool IsO(int x, int y)
        {
            return BoardState[x, y] == Piece.O;
        }

        public TicTacToeBoard()
        {
            BoardState = new Piece[3,3];
        }
    }

    private static void DrawTitle(string[] title)
    {
        int x = Console.CursorLeft;
        int y = Console.CursorTop;

        for (int i = 0; i < title.Length; i++)
        {
            Console.SetCursorPosition(x, y + i);
            Console.Write(title[i]);
        }
    }

    private static void DrawTitleOutline(string[] title, ConsoleColor spaceColour, ConsoleColor otherColour)
    {
        int x = Console.CursorLeft;
        int y = Console.CursorTop;

        int cnt = 0;
        for (int i = 0; i < title.Length; i++)
        {
            for (int j = 0; j < title[i].Length; j++)
            {
                Console.SetCursorPosition(x + j, y + i);

                if (title[i][j] == ' ')
                {
                    Console.BackgroundColor = spaceColour;
                    Console.ForegroundColor = spaceColour;
                }
                else
                {
                    Console.BackgroundColor = otherColour;
                    Console.ForegroundColor = otherColour;
                }

                Console.Write(" ");
                cnt++;
            }
        }
    }

    /// <summary>
    /// Returns position of top left of board
    /// </summary>
    /// <returns></returns>
    private static Point DrawBoard()
    {
        Console.Clear();

        string line = new string(' ', Console.WindowWidth);
        string[] board = new string[] 
        { 
            "╔═══╦═══╦═══╗",
            "║   ║   ║   ║",
            "╠═══╬═══╬═══╣",
            "║   ║   ║   ║",
            "╠═══╬═══╬═══╣",
            "║   ║   ║   ║",
            "╚═══╩═══╩═══╝"
        };

        string[] title =
        {
            "██████████████████████████████████████████████████████████████████████████████████████████████████████",
            "█        ██        ███      █████████        ███      ████      █████████        ███      ███        █",
            "████  ████████  █████  ████  ███████████  █████  ████  ██  ████  ███████████  █████  ████  ██  ███████",
            "████  ████████  █████  █████████████████  █████  ████  ██  █████████████████  █████  ████  ██      ███",
            "████  ████████  █████  ████  ███████████  █████        ██  ████  ███████████  █████  ████  ██  ███████",
            "████  █████        ███      ████████████  █████  ████  ███      ████████████  ██████      ███        █",
            "██████████████████████████████████████████████████████████████████████████████████████████████████████" 
        };

        // Gray out the screen
        Console.BackgroundColor = ConsoleColor.DarkGray;
        Console.ForegroundColor = ConsoleColor.White;

        for (int i = 0; i < Console.WindowHeight; i++)
        {
            Console.SetCursorPosition(0, i);
            Console.Write(line);
        }

        // Draw the title
        Console.SetCursorPosition(2, 1);

        DrawTitleOutline(title, ConsoleColor.White, ConsoleColor.Black);

        Console.BackgroundColor = ConsoleColor.DarkGray;
        Console.ForegroundColor = ConsoleColor.White;

        Console.SetCursorPosition(2, 10);
        Console.WriteLine("Use arrow keys to select a square and Enter to place your piece");
        Console.SetCursorPosition(2, 11);
        Console.WriteLine("Press ESC to exit");

        // Draw the game board centered
        Console.SetCursorPosition((Console.WindowWidth/2)-6, 12);
        DrawTitle(board);

        // Return position of top left of gameboard
        return new Point((Console.WindowWidth / 2)-4, 13);
    }

    private static void ClearBoard(Point topLeftPos)
    {
        Console.BackgroundColor = ConsoleColor.DarkGray;
        Console.ForegroundColor = ConsoleColor.DarkGray;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Console.SetCursorPosition(topLeftPos.X + j*4-1, topLeftPos.Y + i*2);
                Console.Write(' ');
                Console.Write(' ');
                Console.Write(' ');
            }
        }
    }

    private static void DrawLine(Point topLeftPos, Piece winner, WinningLine line)
    {
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = winner == Piece.X ? ConsoleColor.Blue : ConsoleColor.Red;
        char pieceChar = winner == Piece.X ? 'X' : 'O';

        switch (line)
        {
            case WinningLine.FirstHorizontal:
            case WinningLine.SecondHorizontal:
            case WinningLine.ThirdHorizontal:
                for (int i = 0; i < 3; i++)
                {
                    Console.SetCursorPosition(topLeftPos.X+(i*4)-1, topLeftPos.Y + (((int)line)-(int)WinningLine.FirstHorizontal)*2);
                    Console.Write(' ');
                    Console.Write(pieceChar);
                    Console.Write(' ');
                }
                break;
            case WinningLine.FirstVertical:
            case WinningLine.SecondVertical:
            case WinningLine.ThirdVertical:
                for (int i = 0; i < 3; i++)
                {
                    Console.SetCursorPosition(topLeftPos.X + (((int)line) - (int)WinningLine.FirstVertical)*4-1, topLeftPos.Y + (i * 2));
                    Console.Write(' ');
                    Console.Write(pieceChar);
                    Console.Write(' ');
                }
                break;
            case WinningLine.LeftRightDiagonal:
                for (int i = 0; i < 3; i++)
                {
                    Console.SetCursorPosition(topLeftPos.X + (i * 4) - 1, topLeftPos.Y + (i * 2));
                    Console.Write(' ');
                    Console.Write(pieceChar);
                    Console.Write(' ');
                }
                break;
            case WinningLine.RightLeftDiagonal:
                for (int i = 0; i < 3; i++)
                {
                    Console.SetCursorPosition(topLeftPos.X + ((2-i) * 4) - 1, topLeftPos.Y + (i * 2));
                    Console.Write(' ');
                    Console.Write(pieceChar);
                    Console.Write(' ');
                }
                break;
            default:
                break;
        }
    }

    private static void Main(string[] args)
    {
        var originalEncoding = Console.OutputEncoding;
        Console.OutputEncoding = Encoding.UTF8;
        Console.CursorVisible = false;

        TicTacToeBoard ticTacBoard = new TicTacToeBoard();
        int posX = 1;
        int posY = 1;
        int oldX = 1;
        int oldY = 1;
        bool xTurn = true;
        bool lastXTurn = false;

        Point topLeftPos = DrawBoard();
        Console.SetCursorPosition(topLeftPos.X+4, topLeftPos.Y+2);

        bool gameOver = false;
        bool gameStart = true;

        while (!gameOver)
        {
            ConsoleKeyInfo key;
            if (gameStart)
            {
                key = new ConsoleKeyInfo('s', ConsoleKey.S,false, false, false);
                gameStart = false;
            }
            else
            {
                key = Console.ReadKey(true);
                lastXTurn = xTurn;
            }

            oldX = posX;
            oldY = posY;

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    posY--;
                    break;
                case ConsoleKey.DownArrow:
                    posY++;
                    break;
                case ConsoleKey.LeftArrow:
                    posX--;
                    break;
                case ConsoleKey.RightArrow:
                    posX++;
                break;
                case ConsoleKey.Escape:
                    gameOver = true;
                    break;
                case ConsoleKey.Enter:
                    if (ticTacBoard.BoardState[posX, posY] == Piece.None)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.ForegroundColor = xTurn ? ConsoleColor.Blue : ConsoleColor.Red;

                        Console.Write(xTurn ? 'X' : 'O');
                        ticTacBoard.BoardState[posX, posY] = xTurn ? Piece.X : Piece.O;
                        xTurn = !xTurn;

                        TicTacToeBoardResult result = ticTacBoard.IsWinner();
                        bool needsReset = false;
                        if (result.Winner == Piece.X)
                        {
                            DrawLine(topLeftPos, result.Winner, result.Line);

                            Console.BackgroundColor = ConsoleColor.DarkGray;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.SetCursorPosition((Console.WindowWidth / 2) - 6, 20);
                            Console.WriteLine("X Wins! ");
                            needsReset = true;
                        }
                        else if (result.Winner == Piece.O)
                        {
                            DrawLine(topLeftPos, result.Winner, result.Line);

                            Console.BackgroundColor = ConsoleColor.DarkGray;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.SetCursorPosition((Console.WindowWidth / 2) - 6, 20);
                            Console.WriteLine("O Wins! ");
                            needsReset = true;
                        }
                        else if (result.Line == WinningLine.Draw)
                        {
                            Console.BackgroundColor = ConsoleColor.DarkGray;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.SetCursorPosition((Console.WindowWidth / 2) - 6, 20);
                            Console.WriteLine("Draw!   ");
                            needsReset = true;
                        }

                        if (needsReset)
                        {
                            System.Threading.Thread.Sleep(2000);
                            ClearBoard(topLeftPos);

                            ticTacBoard = new TicTacToeBoard();
                            posX = 1;
                            posY = 1;
                            oldX = 1;
                            oldY = 1;
                            xTurn = true;
                            lastXTurn = false;
                        }
                    }
                    break;
                default:
                    break;
            }

            if (posX < 0)
            {
                posX = 0;
            }
            if (posX > 2)
            {
                posX = 2;
            }
            if (posY < 0)
            {
                posY = 0;
            }
            if (posY > 2)
            { 
                posY = 2; 
            }


            // Get rid of outline if there was one
            if (ticTacBoard.BoardState[oldX, oldY] == Piece.None)
            {
                Console.SetCursorPosition(topLeftPos.X + oldX * 4, topLeftPos.Y + oldY * 2);
                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(' ');
            }

            // Print whose turn it is
            if (xTurn != lastXTurn)
            {
                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition((Console.WindowWidth / 2) - 6, 20);
                Console.WriteLine(xTurn ? "X's Turn" : "O's Turn");
            }

            // Set cursor to new position
            Console.SetCursorPosition(topLeftPos.X + posX * 4, topLeftPos.Y + posY * 2);

            // Draw outline of piece if there isn't already one on this square
            if (ticTacBoard.BoardState[posX, posY] == Piece.None)
            {
                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(xTurn ? 'X' : 'O');

                Console.SetCursorPosition(topLeftPos.X + posX * 4, topLeftPos.Y + posY * 2);
            }
        }

        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;
        Console.Clear();
        Console.OutputEncoding = originalEncoding;
        Console.CursorVisible = true;
    }
}