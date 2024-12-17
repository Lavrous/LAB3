﻿using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;



namespace Game
{

    public class Program
    {
        const int N = 10;
        const int M = 16;
        const int Chance = 2; // higher the number - lower the chance to obtain a bonus
        // "-" - nothing "b" - ball "w" - wall "c" - crushable
        [Serializable]
        public class Field
        {
            public int b_hor { get; set; } // horizontal ball direction
            public int b_ver { get; set; } // vertical ball direction
            public double speed { get; set; } // higher the number - lower the speed to obtain a bonus
            public int pl_pos { get; set; } // platform position
            public int pl_size { get; set; } // platform size
            public string[,] field { get; set; }
            public string[,] buff_field { get; set; }
            public Field(int b_h = 1, int b_v = -1, int pl_p = 0, int pl_s = 3, double spd = 400)
            {
                b_hor = b_h;
                b_ver = b_v;
                pl_pos = pl_p;
                pl_size = pl_s;
                speed = spd;
                field = new string[N, M];
                for (int j = 0; j < M; j++)
                {
                    field[0, j] = "w";
                }
                for (int i = 1; i < N - 1; i++)
                {
                    field[i, 0] = "w";
                    for (int j = 1; j < M - 1; j++)
                    {
                        field[i, j] = "-";
                    }
                    field[i, M - 1] = "w";
                }
                for (int j = 0; j < M; j++)
                {
                    field[N - 1, j] = "-";
                }

                for (int j = 1; j < M - 1; j++)
                {
                    field[N / 2 - 2, j] = "c";
                }

                for (int j = 1; j < M - 1; j++)
                {
                    field[N / 2 - 1, j] = "c";
                }

                field[N - 2, 1] = "b";

                buff_field = new string[N, M];
                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < M; j++)
                    {
                        buff_field[i, j] = "-";
                    }
                }
            }
            public void FieldPrint()
            {
                string tmp = "";
                Console.Write("╔");
                for (int j = 1; j < M - 1; j++)
                {
                    Console.Write("═");
                }
                Console.WriteLine("╗");
                for (int i = 1; i < N - 1; i++)
                {
                    for (int j = 0; j < M; j++)
                    {
                        switch (field[i, j])
                        {
                            case "w":
                                tmp = "║";
                                break;
                            case "b":
                                tmp = "o";
                                break;
                            case "c":
                                tmp = "H";
                                break;
                            default:
                                tmp = "-";
                                break;
                        }
                        if (buff_field[i, j] != "-")
                        {
                            tmp = buff_field[i, j];
                        }
                        Console.Write(tmp);
                    }
                    Console.WriteLine();
                }
                for (int j = 0; j < M; j++)
                {
                    if ((j >= pl_pos) && (j <= pl_pos + pl_size - 1))
                    {
                        Console.Write("p");
                    }
                    else
                    {
                        Console.Write("-");
                    }
                }
                Console.WriteLine();
            }
            public void PlatformMove(int dir)
            {
                if ((pl_pos + dir >= 0) && (pl_size + pl_pos + dir <= M))
                {
                    pl_pos = pl_pos + dir;
                }
            }
            public bool BallMove()
            {
                int n = 0;
                int m = 0;
                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < M; j++)
                    {
                        if (field[i, j] == "b")
                        {
                            n = i;
                            m = j;
                        }
                    }
                }
                if (n == N - 1)
                {
                    return false;
                }
                if (!BallCollision(n, m))
                {
                    field[n, m] = "-";
                    field[n + b_ver, m + b_hor] = "b";
                }
                return true;
            }

            public void CrushBlock(int n, int m)
            {
                if (field[n, m] == "c")
                {
                    field[n, m] = "-";
                    Random rnd = new Random();
                    int dice;
                    if (rnd.Next(Chance) == 0)
                    {
                        dice = rnd.Next(1, 5); // 1-platform_size_up 2-platform_size_down 3-game_speed_up 4-game_speed_down
                        buff_field[n, m] = dice.ToString();
                    }
                }
            }

            public bool WinCheck()
            {
                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < M; j++)
                    {
                        if (field[i, j] == "c")
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            public void BuffFalling()
            {
                for (int j = 0; j < M; j++)
                {
                    switch (buff_field[N - 1, j])
                    {
                        case "1":
                            pl_size = pl_size + 1;
                            break;
                        case "2":
                            if (pl_size - 1 >= 2)
                            {
                                pl_size = pl_size - 1;
                            }
                            break;
                        case "3":
                            speed = speed / 1.4;
                            if (speed < 300)
                            {
                                speed = 300;
                            }
                            break;
                        case "4":
                            speed = speed * 1.4;
                            if (speed > 600)
                            {
                                speed = 600;
                            }
                            break;
                        default:
                            break;
                    }
                }
                for (int i = N - 1; i > 0; i--)
                {
                    for (int j = 0; j < M; j++)
                    {
                        buff_field[i, j] = buff_field[i - 1, j];
                    }
                }
            }

            public bool BallCollision(int n, int m)
            {
                bool diagonal_check = true;
                if (field[n + b_ver, m] != "-")
                {
                    diagonal_check = false;
                    CrushBlock(n + b_ver, m);
                    b_ver = b_ver * -1;
                    return true;
                }
                else
                {
                    if ((n + b_ver == N - 1) && (pl_pos <= m) && (pl_pos + pl_size >= m))
                    {
                        diagonal_check = false;
                        b_ver = b_ver * -1;
                        return true;
                    }
                }
                if (field[n, m + b_hor] != "-")
                {
                    diagonal_check = false;
                    CrushBlock(n, m + b_hor);
                    b_hor = b_hor * -1;
                    return true;
                }
                if (diagonal_check && field[n + b_ver, m + b_hor] != "-")
                {
                    CrushBlock(n + b_ver, m + b_hor);
                    b_ver = b_ver * -1;
                    b_hor = b_hor * -1;
                    return true;
                }
                else
                {
                    if (diagonal_check && (n + b_ver == N - 1) && (pl_pos <= m + b_hor) && (pl_pos + pl_size >= m + b_hor))
                    {
                        b_ver = b_ver * -1;
                        b_hor = b_hor * -1;
                        return true;
                    }
                }
                return false;

            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Controls. Arrow keys: left-right. ESC: finish game session. LMB: pause");
            Console.WriteLine("Do you want to load the game? Write 'y' or another letter");
            string loadflag = Console.ReadLine();
            Console.CursorVisible = false;
            Field game_field = new Field();
            BinaryFormatter formatter = new BinaryFormatter();
            if (loadflag == "y")
            {
                // десериализация из файла gmamelevel.dat
                using (FileStream fs = new FileStream("gmamelevel.dat", FileMode.OpenOrCreate))
                {
                    game_field = (Field)formatter.Deserialize(fs);

                    Console.WriteLine("Объект десериализован");
                }
            }
            Console.Clear();
            bool winflag = false;
            bool lostflag = false;
            System.ConsoleKey command_key;
            do
            {
                while (!Console.KeyAvailable)
                {
                    Thread.Sleep((int)game_field.speed);
                    game_field.BuffFalling();
                    lostflag = !game_field.BallMove();
                    winflag = game_field.WinCheck();
                    Console.SetCursorPosition(0, 0);
                    game_field.FieldPrint();
                    if (lostflag)
                    {
                        Console.CursorVisible = true;
                        Console.WriteLine("You Lost! Press any key to continue");
                        break;
                    }
                    if (winflag)
                    {
                        Console.CursorVisible = true;
                        Console.WriteLine("You Won! Press any key to continue");
                        break;
                    }
                }
                command_key = Console.ReadKey(true).Key;
                if (command_key == ConsoleKey.LeftArrow)
                {
                    game_field.PlatformMove(-1);
                }
                if (command_key == ConsoleKey.RightArrow)
                {
                    game_field.PlatformMove(1);
                }
                if (lostflag || winflag)
                {
                    break;
                }
            } while (command_key != ConsoleKey.Escape);

            Console.CursorVisible = true;
            Console.WriteLine("Do you want to save the game? Write 'y' or another letter");
            string saveflag = Console.ReadLine();
            if (saveflag == "y")
            {

                // получаем поток, куда будем записывать сериализованный объект
                using (FileStream fs = new FileStream("gmamelevel.dat", FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, game_field);
                    Console.WriteLine("Объект сериализован");
                }

            }

            Console.WriteLine();
        }
    }
}