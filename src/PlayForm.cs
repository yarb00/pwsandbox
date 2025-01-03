// This file is a part of PWSandbox ( https://github.com/yarb00/PWSandbox )

namespace PWSandbox;

public partial class PlayForm : Form
{
    private readonly string[] mapLines;

    private bool isPlayerSpawned = false;
    private int playerX, playerY;

    private const int cellSize = 20;

    private enum MapObjects
    {
        Void = ' ',
        Player = '!',
        Wall = '@',
        Finish = '='
    }

    public PlayForm(string mapFileLocation)
    {
        InitializeComponent();

        try
        {
            mapLines = File.ReadAllLines(mapFileLocation);
        }
        catch (FileNotFoundException)
        {
            MessageBox.Show(
                "ERROR loading PWSandbox:"
                + $"\n\"{mapFileLocation}\" file NOT found!",
                "PWSandbox",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error,
                MessageBoxDefaultButton.Button1
            );

            Close();
            return;
        }

        int maxX = 0;
        for (int y = 0; y < mapLines.Length; y++) if (maxX < mapLines[y].Length) maxX = mapLines[y].Length;
        ClientSize = new Size(maxX * cellSize, mapLines.Length * cellSize);
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Up or Keys.W:
                if (!IsCollision(playerX, playerY - 1)) playerY -= 1;
                break;
            case Keys.Down or Keys.S:
                if (!IsCollision(playerX, playerY + 1)) playerY += 1;
                break;
            case Keys.Left or Keys.A:
                if (!IsCollision(playerX - 1, playerY)) playerX -= 1;
                break;
            case Keys.Right or Keys.D:
                if (!IsCollision(playerX + 1, playerY)) playerX += 1;
                break;
        }

        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        ProcessMap(e.Graphics);
    }

    private void ProcessMap(Graphics graphics)
    {
        for (int y = 0; y < mapLines.Length; y++)
        {
            for (int x = 0; x < mapLines[y].Length; x++)
            {
                switch (mapLines[y][x])
                {
                    default:
                    case (char)MapObjects.Void:
                        DrawCell(graphics, x, y, cellSize, Color.Transparent);
                        break;
                    case (char)MapObjects.Player:
                        if (!isPlayerSpawned)
                        {
                            playerX = x;
                            playerY = y;

                            isPlayerSpawned = true;
                        }
                        DrawCell(graphics, playerX, playerY, cellSize, Color.Yellow);
                        break;
                    case (char)MapObjects.Wall:
                        DrawCell(graphics, x, y, cellSize, Color.Black);
                        break;
                    case (char)MapObjects.Finish:
                        if (playerX == x && playerY == y) MessageBox.Show(
                            "You reached the finish!",
                            "PWSandbox - PLAY mode",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information,
                            MessageBoxDefaultButton.Button1
                        );
                        DrawCell(graphics, x, y, cellSize, Color.Green);
                        break;
                }
            }
        }
    }

    private void DrawCell(Graphics graphics, int x, int y, int cellSize, Color color)
    {
        using (Brush brush = new SolidBrush(color)) graphics.FillRectangle(brush, x * cellSize, y * cellSize, cellSize, cellSize);
    }

    private bool IsCollision(int x, int y)
    {
        if (mapLines[y][x] == (char)MapObjects.Wall) return true;
        return false;
    }
}
