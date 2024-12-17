using System;
using Game;
using System.Security.Principal;
using System.IO;

namespace GameTest
{
    [TestClass]
    public class GameTests
    {
        const int N = 10;
        const int M = 16;
        [TestMethod]
        public void TestPlatformMove()
        {
            // Arrange
            Program.Field TestField1 = new Program.Field(1, -1, 0, 3, 300);
            Program.Field TestField2 = new Program.Field(1, -1, 1, 3, 300);
            // Act
            TestField1.PlatformMove(1);
            // Assert
            Assert.AreEqual(TestField1.pl_pos, TestField2.pl_pos);
            // попытка подвинуть находящуюся в левом углу платформу вправо должна привести к тому, что платформа сдвинется
        }
        [TestMethod]
        public void TestPlatformMoveExeption()
        {
            // Arrange
            Program.Field TestField1 = new Program.Field(1, -1, 0, 3, 300);
            Program.Field TestField2 = new Program.Field(1, -1, 0, 3, 300);
            // Act
            TestField1.PlatformMove(-1);
            // Assert
            Assert.AreEqual(TestField1.pl_pos, TestField2.pl_pos);
            // попытка подвинуть находящуюся в левом углу платформу влево должна привести к тому, что платформа останется на месте
        }
        [TestMethod]
        public void TestBallMove()
        {
            // Arrange
            Program.Field TestField1 = new Program.Field(1, 1, 0, 3, 300);
            TestField1.field = new string[N, M];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    TestField1.field[i, j] = "-";
                }
            }
            TestField1.field[5, 5] = "b";

            Program.Field TestField2 = new Program.Field(1, 1, 0, 3, 300);
            TestField2.field = new string[N, M];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    TestField2.field[i, j] = "-";
                }
            }
            TestField2.field[6, 6] = "b";
            // Act
            TestField1.BallMove();
            // Assert
            Assert.AreEqual(TestField1.field[6, 6], TestField2.field[6, 6]);
            // проверка нормального перемещения мяча
        }
        [TestMethod]
        public void TestCrushBlock()
        {
            // Arrange
            Program.Field TestField1 = new Program.Field(1, 1, 0, 3, 300);
            TestField1.field = new string[N, M];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    TestField1.field[i, j] = "-";
                }
            }
            TestField1.field[5, 5] = "c";

            // Act
            TestField1.CrushBlock(5, 5);
            // Assert
            Assert.AreEqual(TestField1.field[5, 5], "-");
            // проверка разрушаемости блока
        }
        [TestMethod]
        public void TestWin()
        {
            // Arrange
            Program.Field TestField1 = new Program.Field(1, 1, 0, 3, 300);
            TestField1.field = new string[N, M];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    TestField1.field[i, j] = "-";
                }
            }

            // Assert
            Assert.AreEqual(TestField1.WinCheck(), true);
            // проверка на условие победы (полное отсутствие разрушаемых блоков)
        }
        [TestMethod]
        public void TestImplement()
        {
            // Arrange
            Program.Field TestField1 = new Program.Field(1, 1, 0, 3, 300);
            TestField1.buff_field[N - 1, 1] = "1";

            Program.Field TestField2 = new Program.Field(1, 1, 0, 4, 300);

            // Act
            TestField1.BuffFalling();
            // Assert
            Assert.AreEqual(TestField1.pl_size, TestField2.pl_size);
            // проверка эффекта от усиления
        }
        [TestMethod]
        public void TestDiagonalCollision()
        {
            // Arrange
            Program.Field TestField1 = new Program.Field(1, 1, 0, 3, 300);
            TestField1.field = new string[N, M];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    TestField1.field[i, j] = "-";
                }
            }
            TestField1.field[5, 5] = "c";
            TestField1.field[4, 4] = "b";

            // Act
            TestField1.BallCollision(4, 4);
            // Assert
            Assert.AreEqual(TestField1.field[5, 5], "-");
            // проверка обработки столкновения с разрушаемым объектом по диагонали
        }
        [TestMethod]
        public void TestPrint()
        {
            // Arrange
            var stringWriter1 = new StringWriter();
            Console.SetOut(stringWriter1);

            Program.Field TestField = new Program.Field();
            for (int i = 1; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    TestField.field[i, j] = "-";
                }
            }
            for (int j = 0; j < M; j++)
            {
                TestField.field[0, j] = "w";
            }
            TestField.field[1, 0] = "w";
            TestField.field[1, 1] = "c";
            TestField.field[1, 2] = "b";

            // Act
            TestField.FieldPrint();
            var TrueOut = stringWriter1.ToString();

            var stringWriter2 = new StringWriter();
            Console.SetOut(stringWriter2);

            Console.Write("╔");
            for (int j = 1; j < M - 1; j++)
            {
                Console.Write("═");
            }
            Console.WriteLine("╗");
            Console.Write("║");
            Console.Write("H");
            Console.Write("o");
            for (int j = 3; j < M; j++)
            {
                Console.Write("-");
            }
            Console.WriteLine();

            for (int i = 2; i < N - 1; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    Console.Write("-");
                }
                Console.WriteLine();
            }
            Console.Write("p");
            Console.Write("p");
            Console.Write("p");
            for (int j = 3; j < M; j++)
            {
                Console.Write("-");
            }
            Console.WriteLine();
            var ExpectedOut = stringWriter2.ToString();

            // Assert
            Assert.AreEqual(TrueOut, ExpectedOut);
        }
    }
}