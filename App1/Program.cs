﻿using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Game
{
    class Program
    {
        static string user;
        static int health = 100;
        static int damage;
        static int money = 100;
        static float level;
        static int n;
        static int Hero_Level = 1;
        static int life = 0;


        static List<string[]> frame = new List<string[]>(); //кадр
        static bool gameStatus = true; // Статус игры

        static char hero = 'x'; // Персонаж
        static char wall = '#'; // Стена
        static char target = '1'; // Цель
        static char lab = '2'; // Лабиринт
        static char tavern = '3'; // Таверна
        static char emptyCell = ' '; // Пустая ячейка
        static char game_enemy = 'S'; // Враг
        static int x1, y1; // координаты персонажа

        delegate void destination();
        static destination[,] locations = {
            {
                Null,
                labyrinth,
                Tavern,
                Null,
            }, //локации
            {
                Sleep,
                Eating,
                roulette,
                Null,
            }, //таверна
            {
                endField,
                Null,
                Null,
                Null,
            }, //лабиринт
            {
                Null,
                Null,
                Null,
                Null,
            } //подарок
        };

        static void Main()
        {
            Start();
            n = 0;
            do
            {
                if (n != 0) // итерация здоровья и денег после каждого хода
                {
                    clear();
                    Console.WriteLine("Поздравляем вас с прохождением круга!");
                    Console.ReadLine();
                    Console.WriteLine($"Денег начислено = {(int)(20 * level)}");
                    Console.WriteLine($"Здоровья прибавилось = {(int)(30 * level)}");
                    money += (int)(20 * level);
                    health += (int)(30 * level);
                    Console.ReadLine();
                }
                PrintInfo(); // вывод характеристик персонажа
                going(); // генерация локации
                clear();
                enemy(ref health, ref damage, ref money); // подбор врага
                Level(); // проверка уровня
                rebirth(); // роверка на наличие дополнительных жизней
                n++;
            } while (health > 0);
            clear();
            Console.WriteLine($"Проведено боев: {n}\n");
            lose();
            Console.WriteLine("Нажмите Enter для выхода...");
            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                danger();

        }

        static void Null() { }
        static void Start()
        {
            clear();
            Console.WriteLine("\t\t\t\tMAGIC WAR\n\n\n");
            Console.WriteLine("Предыстория: Еще вчера все было хорошо и везде царил мир и покой. \nНо тут случилось то, чего не ожидал никто." +
                " На земли сказочных земель\nнапали ужасные Тролли. Без какого-либо повода они разбойничают и нападают на\nкаждого, кто встанет у них на пути." +
                " Пророчество гласит, что однажды появится маг, в чьих силах будет - противостоять многочисленной армии Троллей.\nИ вот, пророчество сбылось...\n");
            Console.WriteLine("Цель игры: Одолеть всех врагов, что встретятся у тебя на пути.\n\n\n");
            Console.WriteLine("Нажмите Enter для продолжения...");
            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                danger();
            clear();
            Console.WriteLine("Примечание №1: Ввод подразумевает русскоязычного пользователя и ответы\nТОЛЬКО в виде \"Да\" и \"Нет\" (за искл. выбора сложности).\n");
            Console.WriteLine("Примечание №2: В игре присутствует защита от неправильного ввода, а так же,\nфункция \"НА РАБОТЕ\".\n" +
                "В случае, если вы заметили приближение начальника, во время ввода решения\nпросто нажмите \"Esc\" - это скроет игру, " +
                "заменив её на информацию, выводимую\nпри вызове консоли (даже разбирающийся в компьютерах человек, увидев ее,\nничего не заподозрит)\n" +
                "Для выхода из режима \"маскировки\" введите следующую комбинацию клавиш - \":)\" \n\n");
            Console.WriteLine("Нажмите Enter для продолжения...");
            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                danger();
            clear();
            EnterName();
            Console.Clear();
            Console.WriteLine($"Добро пожаловать в игру, {user}.\n\n");
            Console.WriteLine("Выберите сложность:");
            Console.WriteLine("1.Легко");
            Console.WriteLine("2.Нормально");
            Console.WriteLine("3.Сложно");
            string v;
            do
            {
                v = Console.ReadLine();
            } while ((v != "1") && (v != "Легко") && (v != "легко") && (v != "2") && (v != "Нормально") && (v != "нормально") && (v != "3") && (v != "Сложно") && (v != "сложно"));
            if ((v == "1") || (v == "Легко") || (v == "легко"))
            {
                level = 0.75F;
                damage = (int)(health * level);
                Console.WriteLine("Выбранный уровень сложности - Легко\n");
            }
            else if ((v == "2") || (v == "Нормально") || (v == "нормально"))
            {
                level = 0.5F;
                damage = (int)(health * level);
                Console.WriteLine("Выбранный уровень сложности - Нормально\n");
            }
            else if ((v == "3") || (v == "Сложно") || (v == "сложно"))
            {
                level = 0.25F;
                damage = (int)(health * level);
                Console.WriteLine("Выбранный уровень сложности - Сложно\n");
            }
            Console.WriteLine("\n\n\nНажмите Enter для продолжения...");
            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                danger();
            clear();
            Console.WriteLine("Нажмите Enter для начала игры...");
            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                danger();
            clear();
        }

        static void PrintInfo()
        {
            clear();
            Console.WriteLine(" ---------------------------------------------");
            Console.WriteLine("|   Ник игрока   | Здоровье | Урон |  Монеты  |");
            Console.WriteLine(" ---------------------------------------------");
            Console.WriteLine("|  {0,-14}|  {1,-8}| {2,-5}|  {3,-8}|", user, health, damage, money);
            Console.WriteLine(" ---------------------------------------------");
            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                danger();
        }

        static void EnterName()
        {
            Console.Write("Введите ваше имя: ");
            user = Console.ReadLine();
            if (user == "" || user == " " ) //пробел не срабатывает, доп код?
            {
                Console.Write("Вы ввели пустое значение, повторите ввод.\n");
                do
                {
                    Console.Write("Введите ваше имя: ");
                    user = Console.ReadLine();
                } while (user == "");
            }

            if (user.Length > 10)
            {
                Console.Write("Вы ввели слишком длинное имя, повторите ввод.\n");
                do
                {
                    Console.Write("Введите ваше имя: ");
                    user = Console.ReadLine();
                } while (user.Length > 10);
            }
        }
        static void Print_Army(string troll, int troll_health, int troll_damage, int troll_money)
        {
            Console.WriteLine(" ---------------------------------------------");
            Console.WriteLine("|   Ник игрока   | Здоровье | Урон |  Монеты  |");
            Console.WriteLine(" ---------------------------------------------");
            Console.WriteLine("|  {0,-14}|  {1,-8}| {2,-5}|  {3,-8}|", user, health, damage, money);
            Console.WriteLine(" ---------------------------------------------");
            Console.WriteLine("                                              ");
            Console.WriteLine("|                   ПРОТИВ!                   |");
            Console.WriteLine("                                              ");
            Console.WriteLine(" ---------------------------------------------");
            Console.WriteLine("|  {0,-14}|  {1,-8}| {2,-5}|  {3,-8}|", troll, troll_health, troll_damage, troll_money);
            Console.WriteLine(" ---------------------------------------------");
            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                danger();
        }

        static void going()
        {
            Random rand = new Random();
            int temp;
            temp = rand.Next(4);
            switch (temp)
            {
                case 1:
                    randomEvent();
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    clear();
                    Console.WriteLine("Герой идёт в поле...\n\n\n");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Field(1);
                    break;
                case 2:
                    randomEvent();
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    clear();
                    Console.WriteLine("Герой перебирается через горы...\n\n\n");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Field(2);
                    break;
                case 3:
                    randomEvent();
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    clear();
                    Console.WriteLine("Герой проходит в лесу...\n\n\n");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Field(3);
                    break;
            }
        }

        static void enemy(ref int health, ref int damage, ref int money)
        {
            Random rand = new Random();
            int temp;
            if (n <= 3) temp = rand.Next(60);
            else temp = rand.Next(103);
            if (temp <= 60)
            {
                Console.WriteLine("На пути тебе встретился тролль\n");
                int troll_health = rand.Next(80, 100);
                int troll_money = rand.Next(30);
                int t_h = troll_health / 2;
                int troll_damage = damage_value(2, troll_health);
                int total_damage = 0;
                Print_Army("Тролль", troll_health, troll_damage, troll_money);
                int pay = troll_health / 3;
                Console.WriteLine($"Будете сражаться ? Цена откупа - {pay}\n");
                string battlefield = Console.ReadLine();
                battlefield = Enter(battlefield);
                Console.WriteLine("\n\nНажимайте Enter для продолжения...\n\n");
                if ((battlefield == "Да") || (battlefield == "да") || (battlefield == "ДА"))
                {
                    int f = rand.Next(2);
                    if (f == 1)
                    {
                        Console.WriteLine("\nВы первее наносите удар\n");
                        damage = damage_value(1, health);
                        troll_health -= damage;
                        Console.WriteLine($"Вы нанесли удар тролю, его здоровье - {troll_health}, урон - {troll_damage}\n");
                    }
                    else
                    {
                        Console.WriteLine("\nВраг первее наносит удар\n");
                        troll_damage = damage_value(2, troll_health);
                        health -= troll_damage;
                        Console.WriteLine($"Троль ударил вас, ваше здоровье - {health}, урон - {damage}\n");
                    }
                    do
                    {
                        total_damage += troll_damage;
                        troll_health -= damage;
                        if (troll_health > 0) Console.WriteLine($"Вы нанесли удар тролю, его здоровье - {troll_health}, урон - {troll_damage}\n");
                        else
                        {
                            Console.WriteLine("Этот удар стал для вашего врага последним...\n");
                            break;

                        }
                        troll_damage = damage_value(2, troll_health);
                        health -= troll_damage;
                        damage = damage_value(1, health);
                        if (health > 0) Console.WriteLine($"Троль ударил вас, ваше здоровье - {health}, урон - {damage}\n");
                        else
                        {
                            Console.WriteLine("Этот удар стал для вас последним...\n");
                            break;

                        }
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                    } while (troll_health >= 0 && health >= 0);
                    if (troll_health <= 0)
                    {
                        Console.WriteLine("\n\nПоздравляем, вы выиграли !!!\n");
                        Console.WriteLine("Вы отняли у врага его жизненные силы и здоровье, и на половину восстановили\nпервоначальные свои.\n");
                        health += t_h + total_damage;
                        damage = damage_value(1, health);
                        money += troll_money;
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        PrintInfo();
                        Console.WriteLine("Нажмите Enter для продолжения...");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                    }
                    if (health < 0)
                    {
                        Console.WriteLine("К сожалению, вы проиграли.\n");
                        Console.WriteLine("Враг оказался сильнее.\n");
                    }
                }
                else
                {
                    if (money >= pay)
                    {
                        money -= pay;
                        Console.WriteLine("\nВы откупились.\n");
                    }
                    else
                    {
                        Console.WriteLine("\nУ вас недостаточно денег для откупа.\n");
                        int z = pay - money;
                        money = 0;
                        health -= z;
                        Console.WriteLine($"\nВраг забрал все ваши деньги и нанес вам урон - {z} xp\n");
                        PrintInfo();
                    }
                }
            }
            else if (60 <= temp && temp <= 90)
            {
                Console.WriteLine("На пути тебе встретился отряд троллей\n");
                int troll_health = rand.Next(250, 600);
                int troll_money = rand.Next(25, 100);
                int t_h = troll_health + health / 2;
                int troll_damage = damage_value(2, troll_health);
                Print_Army("Отряд троллей", troll_health, troll_damage, troll_money);
                int pay = troll_health / 3;
                Console.WriteLine($"Будете сражаться ? Цена откупа - {pay}\n");
                string battlefield = Console.ReadLine();
                battlefield = Enter(battlefield);
                Console.WriteLine("\n\nНажимайте Enter для продолжения...\n\n");
                if ((battlefield == "Да") || (battlefield == "да") || (battlefield == "ДА"))
                {
                    int f = rand.Next(2);
                    if (f == 1)
                    {
                        Console.WriteLine("\nВы первее наносите удар\n");
                        damage = damage_value(1, health);
                        troll_health -= damage;
                        Console.WriteLine($"Вы нанесли удар отряду троллей, их здоровье - {troll_health}, урон - {troll_damage}\n");
                    }
                    else
                    {
                        Console.WriteLine("\nВраг первее наносит удар\n");
                        troll_damage = damage_value(2, troll_health);
                        health -= troll_damage;
                        Console.WriteLine($"Отряд троллей ударил вас, ваше здоровье - {health}, урон - {damage}\n");
                    }
                    do
                    {
                        troll_health -= damage;
                        Console.WriteLine($"Вы нанесли удар отряду троллей, их здоровье - {troll_health}, урон - {troll_damage}\n");
                        troll_damage = damage_value(2, troll_health);
                        health -= troll_damage;
                        damage = damage_value(1, health);
                        if (health > 0) Console.WriteLine($"Отряд троллей ударил вас, ваше здоровье - {health}, урон - {damage}\n");
                        else
                        {
                            Console.WriteLine("Этот удар стал для вас последним...\n");
                            break;
                        }
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                    } while (troll_health >= 0 && health >= 0);
                    if (troll_health <= 0)
                    {
                        Console.WriteLine("\n\nПоздравляем, вы выиграли !!!\n");
                        Console.WriteLine("Вы отняли у врага его жизненные силы и здоровье, и на половину восстановили\nпервоначальные свои.\n");
                        health += t_h;
                        damage = damage_value(1, health);
                        money += troll_money;
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        PrintInfo();
                        Console.WriteLine("Нажмите Enter для продолжения...");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                    }
                    if (health < 0)
                    {
                        Console.WriteLine("К сожалению, вы проиграли.\n");
                        Console.WriteLine("Враг оказался сильнее.\n");
                    }
                }
                else
                {
                    if (money > pay)
                    {
                        money -= pay;
                        Console.WriteLine("\nВы откупились.\n");
                    }
                    else
                    {
                        Console.WriteLine("\nУ вас недостаточно денег для откупа.\n");
                        int z = pay - money;
                        money = 0;
                        health -= z;
                        Console.WriteLine($"\nВраг забрал все ваши деньги и нанес вам урон - {z} xp\n");
                        PrintInfo();
                    }
                }
            }
            else if (90 <= temp && temp <= 100)
            {
                Console.WriteLine("На пути тебе встретилась армия троллей\n");
                int troll_health = rand.Next(1000, 3000);
                int troll_money = rand.Next(100, 400);
                int t_h = troll_health + health / 2;
                int troll_damage = damage_value(2, troll_health);
                Print_Army("Армия троллей", troll_health, troll_damage, troll_money);
                int pay = troll_health / 3;
                Console.WriteLine($"Будете сражаться ? Цена откупа - {pay}\n");
                string battlefield = Console.ReadLine();
                battlefield = Enter(battlefield);
                Console.WriteLine("\n\nНажимайте Enter для продолжения...\n\n");
                if ((battlefield == "Да") || (battlefield == "да") || (battlefield == "ДА"))
                {
                    int f = rand.Next(2);
                    if (f == 1)
                    {
                        Console.WriteLine("\nВы первее наносите удар\n");
                        damage = damage_value(1, health);
                        troll_health -= damage;
                        Console.WriteLine($"Вы нанесли удар армии троллей, их здоровье - {troll_health}, урон - {troll_damage}\n");
                    }
                    else
                    {
                        Console.WriteLine("\nВраг первее наносит удар\n");
                        troll_damage = damage_value(2, troll_health);
                        health -= troll_damage;
                        Console.WriteLine($"Армия троллей ударила вас, ваше здоровье - {health}, урон - {damage}\n");
                    }
                    do
                    {
                        troll_health -= damage;
                        Console.WriteLine($"Вы нанесли удар армии троллей, их здоровье - {troll_health}, урон - {troll_damage}\n");
                        troll_damage = damage_value(2, troll_health);
                        health -= troll_damage;
                        damage = damage_value(1, health);
                        if (health > 0) Console.WriteLine($"Армия троллей ударила вас, ваше здоровье - {health}, урон - {damage}\n");
                        else
                        {
                            Console.WriteLine("Этот удар стал для вас последним...\n");
                            break;
                        }
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                    } while (troll_health >= 0 && health >= 0);
                    if (troll_health <= 0)
                    {
                        Console.WriteLine("\n\nПоздравляем, вы выиграли !!!\n");
                        Console.WriteLine("Вы отняли у врага его жизненные силы и здоровье, и на половину восстановили\nпервоначальные свои.\n");
                        health += t_h;
                        damage = damage_value(1, health);
                        money += troll_money;
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        PrintInfo();
                        Console.WriteLine("Нажмите Enter для продолжения...");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                    }
                    if (health < 0)
                    {
                        Console.WriteLine("К сожалению, вы проиграли.\n");
                        Console.WriteLine("Враг оказался сильнее.\n");
                    }
                }
                else
                {
                    if (money > pay)
                    {
                        money -= pay;
                        Console.WriteLine("\nВы откупились.\n");
                    }
                    else
                    {
                        Console.WriteLine("\nУ вас недостаточно денег для откупа.\n");
                        int z = pay - money;
                        money = 0;
                        health -= z;
                        Console.WriteLine($"\nВраг забрал все ваши деньги и нанес вам урон - {z} xp\n");
                        PrintInfo();
                    }
                }
            }
            else if (101 <= temp && temp <= 103)
            {
                Console.WriteLine("На пути тебе встретился Король троллей\n");
                int troll_health = rand.Next(10000);
                int troll_money = 10000;
                int t_h = troll_health + health / 2;
                int troll_damage = damage_value(2, troll_health);
                Print_Army("Король троллей", troll_health, troll_damage, troll_money);
                int pay = troll_health / 3;
                Console.WriteLine($"Будете сражаться ? Цена откупа - {pay}\n");
                string battlefield = Console.ReadLine();
                battlefield = Enter(battlefield);
                Console.WriteLine("\n\nНажимайте Enter для продолжения...\n\n");
                if ((battlefield == "Да") || (battlefield == "да") || (battlefield == "ДА"))
                {
                    int f = rand.Next(2);
                    if (f == 1)
                    {
                        Console.WriteLine("\nВы первее наносите удар\n");
                        damage = damage_value(1, health);
                        troll_health -= damage;
                        Console.WriteLine($"Вы нанесли удар Королю троллей, его здоровье - {troll_health}, урон - {troll_damage}\n");
                    }
                    else
                    {
                        Console.WriteLine("\nВраг первее наносит удар\n");
                        troll_damage = damage_value(2, troll_health);
                        health -= troll_damage;
                        Console.WriteLine($"Король троллей ударил вас, ваше здоровье - {health}, урон - {damage}\n");
                    }
                    do
                    {
                        troll_health -= damage;
                        Console.WriteLine($"Вы нанесли удар Королю троллей, его здоровье - {troll_health}, урон - {troll_damage}\n");
                        troll_damage = damage_value(2, troll_health);
                        health -= troll_damage;
                        damage = damage_value(1, health);
                        if (health > 0) Console.WriteLine($"Король троллей ударил вас, ваше здоровье - {health}, урон - {damage}\n");
                        else
                        {
                            Console.WriteLine("Этот удар стал для вас последним...\n");
                            break;
                        }
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                    } while (troll_health >= 0 && health >= 0);
                    if (troll_health <= 0)
                    {
                        Console.WriteLine("Поздравляем, вы выиграли !!!\n");
                        win();
                    }
                    if (health < 0)
                    {
                        Console.WriteLine("К сожалению, вы проиграли.\n");
                        Console.WriteLine("Враг оказался сильнее.\n");
                    }
                }
                else
                {
                    Console.WriteLine("Королю троллей не нужны деньги\n");
                    Console.WriteLine("Хотите сдаться в плен ?\n");
                    string v = Console.ReadLine();
                    battlefield = Enter(battlefield);
                    Console.WriteLine("И ему, уж тем более, не нужны пленные !!!\n");
                    health = 0;
                }
            }
        }

        static int damage_value(int x, int health)
        {
            if (x == 1) return ((int)(level * health));
            if (x == 2) return ((int)(0.25F * health));
            else return ((int)(level * health)); ;
        }

        static void roulette()
        {
            clear();
            Console.WriteLine("Желаете сыграть в рулетку ??? Стоимость игры 500 монет.\n");
            string decision = Console.ReadLine();
            decision = Enter(decision);
            if (((decision == "Да") || (decision == "да") || (decision == "ДА")) && (money >= 500))
            {
                do
                {
                    money -= 500;
                    Random rand = new Random();
                    int x;
                    x = rand.Next(10);
                    switch (x)
                    {
                        case 1:
                            {
                                int x1;
                                x1 = rand.Next(500, 650);
                                money += x1;
                                Console.WriteLine($"\nПоздравляем, вам выпало {x1} монет\n");
                                break;
                            }
                        case 2:
                            {
                                int x1;
                                x1 = rand.Next(250, 500);
                                health += x1;
                                Console.WriteLine($"\nПоздравляем, вам выпало {x1} очков жизни\n");
                                break;
                            }
                        default:
                            Console.WriteLine("\nК сожалению, вам ничего не выпало.\n");
                            break;
                    }
                    Console.WriteLine("Желаете еще сыграть ?");
                    string decision_2 = Console.ReadLine();
                    decision_2 = Enter(decision_2);
                    if ((decision_2 == "Нет") || (decision_2 == "нет") || (decision_2 == "НЕТ")) break;
                } while (money >= 500);
            }
            else if ((decision == "Нет") || (decision == "нет") || (decision == "НЕТ"))
            {
                Console.WriteLine("\nВозможно именно в этот раз удача улыбнулась бы вам ...\n");
            }
            else Console.WriteLine("\nК сожалению, у вас не хватает денег.\n");
            Console.WriteLine("Нажмите Enter для продолжения...");
            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                danger();

        }

        static void win()
        {
            Console.WriteLine("\nВы ВЫИГРАЛИ !!!\n");
            Console.WriteLine("\nИгра окончена.\n");
        }

        static void lose()
        {
            Console.WriteLine("\nВы проиграли.\n");
            Console.WriteLine("\nИгра окончена.\n");
        }

        static void danger()
        {
            clear();
            Console.WriteLine("Microsoft Windows [Version 6.1.7601]");
            Console.WriteLine("(c) Корпорация Майкрософт (Microsoft Corp.), 2009 Все права защищены. \n\n");
            Console.Write($"C:Users\\{user}> ");
            string s = Console.ReadLine();
            if (s != ":)")
            {
                do
                {
                    Console.Write($"C:Users\\{user}> ");
                    s = Console.ReadLine();
                } while (s != ":)");
            }
            clear();
            Console.WriteLine("Игра возобновлена.\n");
        }


        //if (Console.ReadKey(true).Key == ConsoleKey.Escape)
        //	danger();
        /*
		 * Console.ReadKey(true).KeyChar == 'F'
		 * 
		 * char ch = 'A';
			ConsoleKey ck = (ConsoleKey) ch;
		 *  ConsoleWriteLine(ConsoleKey.Escape.KeyChar);
		 * 
		 * 
		 */
        static void Enter(ConsoleKey s)
        {
            if (s == ConsoleKey.Escape)
                danger();
            Console.WriteLine("Введите ответ на последнее действие \n");
            Enter("a");

        }

        static string Enter(string s)
        {
            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
            {
                danger();
                Console.WriteLine("Введите ответ на последнее действие \n");
                s = Console.ReadLine();
            }
            if ((s == "Да") || (s == "да") || (s == "ДА") || (s == "Нет") || (s == "нет") || (s == "НЕТ")) return s;
            else
            {
                do
                {
                    Console.WriteLine("Вы ввели неверное значение. Пожалйста, повторите ввод !");
                    s = Console.ReadLine();
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                } while ((s != "Да") && (s != "да") && (s != "ДА") && (s != "Нет") && (s != "нет") && (s != "НЕТ"));
                return s;
            }
        }

        static void Field(int num) //24x79
        {
            gameStatus = true;
            Console.ForegroundColor = ConsoleColor.White; // Цвет текста консоли 
            char[,] field = new char[24, 79];
            /*
            {
                frame.Add(new string[] { "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#" });
                frame.Add(new string[] { "#", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "#" });
                frame.Add(new string[] { "#", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "#" });
                frame.Add(new string[] { "#", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "#" });
                frame.Add(new string[] { "#", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "#" });
                frame.Add(new string[] { "#", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "#" });
                frame.Add(new string[] { "#", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "#" });
                frame.Add(new string[] { "#", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "#" });
                frame.Add(new string[] { "#", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "#" });
                frame.Add(new string[] { "#", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "#" });
                frame.Add(new string[] { "#", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "#" });
                frame.Add(new string[] { "#", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "#" });
                frame.Add(new string[] { "#", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "#" });
                frame.Add(new string[] { "#", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "#" });
                frame.Add(new string[] { "#", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "#" });
                frame.Add(new string[] { "#", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "#" });
                frame.Add(new string[] { "#", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "#" });
                frame.Add(new string[] { "#", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "#" });
                frame.Add(new string[] { "#", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "#" });
                frame.Add(new string[] { "#", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "#" });
                frame.Add(new string[] { "#", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "#" });
                frame.Add(new string[] { "#", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "#" });
                frame.Add(new string[] { "#", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "#" });
                frame.Add(new string[] { "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#" });
            }
               */
            if (num == 1) // поле
            {
                field = new char[,] {
                    { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
                    { '#', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', ' ', '/', '-', '-', '-', '-', '-', '\\', ' ', ' ', ' ', '<', ' ', '1', ' ', '-', ' ', 'П', 'о', 'л', 'е', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                    { '#', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '|', ' ', '/', '-', '-', '-', '\\', ' ', '|', ' ', ' ', '<', ' ', '2', ' ', '-', ' ', 'Л', 'а', 'б', 'и', 'р', 'и', 'н', 'т', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                    { '#', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', ' ', '|', '|', ' ', ' ', '<', ' ', '3', ' ', '-', ' ', 'Т', 'а', 'в', 'е', 'р', 'н', 'а', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                    { '#', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '2', ' ', ' ', '|', '|', ' ', ' ', '<', ' ', 'У', 'п', 'р', 'а', 'в', 'л', 'е', 'н', 'и', 'е', ' ', 'п', 'е', 'р', 'с', 'о', 'н', 'а', 'ж', 'е', 'м', ' ', '(', 'Х', ')', ' ', '#' },
                    { '#', '\\', '/', '|', '|', '\\', '/', '|', '|', '\\', '/', '|', '|', '\\', '/', '|', '|', '\\', '/', '|', '|', '\\', '/', '|', '|', '\\', '/', '|', '|', '\\', '/', '|', '|', '\\', '/', '|', '|', ' ', ' ', '|', '|', '_', ' ', ' ', ' ', '_', '|', '|', ' ', ' ', '<', ' ', 'о', 'с', 'у', 'щ', 'е', 'с', 'т', 'в', 'л', 'я', 'е', 'т', 'с', 'я', ' ', 'с', 'т', 'р', 'е', 'л', 'к', 'а', 'м', 'и', ' ', ' ', '#' },
                    { '#', '\\', '/', '|', '|', '\\', '/', '|', '|', '\\', '/', '|', '|', '\\', '/', '|', '|', '\\', '/', '|', '|', '\\', '/', '|', '|', '\\', '/', '|', '|', '\\', '/', '|', '|', '\\', '/', '|', '|', ' ', '/', '/', '_', ' ', '|', ' ', '|', ' ', '_', '\\', '\\', ' ', '<', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '#' },
                    { '#', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '/', '/', ' ', '#' },
                    { '#', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '\\', '\\', ' ', '#' },
                    { '#', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', '\\', '/', '|', '|', '\\', '/', '|', '|', '\\', '/', '|', '|', '\\', '/', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '/', '/', ' ', '#' },
                    { '#', ' ', '-', '-', '-', '-', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', ' ', ' ', '\\', '/', '|', '|', '\\', '/', '|', '|', '\\', '/', '|', '|', '\\', '/', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '_', ' ', ' ', '#' },
                    { '#', '|', ' ', '1', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '|', ' ', '|', ' ', '#' },
                    { '#', ' ', '_', '_', '_', '_', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', ' ', ' ', '|', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '/', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '\\', ' ', '|', ' ', '#' },
                    { '#', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '|', ' ', '|', ' ', '|', '|', '\\', '/', '|', '|', '\\', '/', '|', '|', '\\', '/', '|', '|', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '/', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '\\', '|', ' ', '#' },
                    { '#', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '|', ' ', '|', ' ', '|', '|', '\\', '/', '|', '|', '\\', '/', '|', '|', '\\', '/', '|', '|', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '/', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '\\', ' ', '#' },
                    { '#', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '|', ' ', '|', ' ', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '|', ' ', '#' },
                    { '#', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', ' ', '|', ' ', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', '_', ' ', ' ', ' ', ' ', '_', ' ', ' ', ' ', ' ', '_', ' ', ' ', '|', ' ', '#' },
                    { '#', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', ' ', '|', ' ', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', '_', '|', ' ', ' ', '|', ' ', '|', ' ', ' ', '|', '_', '|', ' ', '|', ' ', '#' },
                    { '#', '^', '^', '^', '^', '^', '^', '^', '^', '^', '^', '^', '^', '^', '^', '^', '^', '^', '>', '|', ' ', '|', ' ', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', '_', '|', ' ', ' ', '|', '.', '|', ' ', ' ', '|', '_', '|', ' ', '|', ' ', '#' },
                    { '#', ' ', ' ', '_', ' ', ' ', ' ', '_', ' ', ' ', ' ', '_', ' ', ' ', ' ', '_', ' ', ' ', '>', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '/', ' ', ' ', ' ', '\\', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '_', '_', '_', '_', '_', '_', '|', '_', '|', '_', '_', '_', '_', '_', '_', '|', ' ', '#' },
                    { '#', ' ', '|', ' ', '|', ' ', '|', ' ', '|', ' ', '/', ' ', '|', ' ', '|', '_', ' ', ' ', '>', '|', ' ', '|', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '|', ' ', ' ', ' ', ' ', ' ', '|', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '/', ' ', '3', ' ', '\\', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                    { '#', ' ', '|', ' ', '|', ' ', '|', ' ', '|', ' ', '|', ' ', '|', ' ', '|', ' ', ' ', ' ', '>', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                    { '#', ' ', '|', ' ', '|', ' ', '|', '_', '|', ' ', '|', ' ', '|', ' ', '|', '_', ' ', ' ', '>', ' ', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', ' ', 'x', ' ', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                    { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' }
                };
            }

            else if (num == 2) // горы
            {
                field = new char[,] {
                    { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
                    { '#', '-', '-', ' ', ' ', '-', '-', ' ', ' ', '-', '-', '-', '-', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '/', '-', '-', '-', '-', '-', '\\', ' ', ' ', ' ', '<', ' ', '1', '-', 'Г', 'о', 'р', 'ы', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                    { '#', ' ', '/', '^', '\\', ' ', ' ', ' ', '/', '^', '\\', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '/', '-', '-', '-', '\\', ' ', '|', ' ', ' ', '<', ' ', '2', '-', 'Л', 'а', 'б', 'и', 'р', 'и', 'н', 'т', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                    { '#', '/', '/', '|', '\\', '\\', ' ', '/', '/', '|', '\\', '\\', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', ' ', '|', '|', ' ', ' ', '<', ' ', '3', '-', 'Т', 'а', 'в', 'е', 'р', 'н', 'а', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                    { '#', '\\', '/', '|', '\\', '\\', '/', '/', '/', '|', '\\', '\\', '\\', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '|', ' ', ' ', '2', ' ', ' ', '|', '|', ' ', ' ', '<', ' ', 'У', 'п', 'р', 'а', 'в', 'л', 'е', 'н', 'и', 'е', ' ', 'п', 'е', 'р', 'с', 'о', 'н', 'а', 'ж', 'е', 'м', ' ', '(', 'Х', ')', ' ', ' ', ' ', ' ', ' ', '#' },
                    { '#', '\\', '\\', '|', ' ', '/', '_', '/', '-', '\\', '_', '/', '\\', '\\', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '|', '_', ' ', ' ', ' ', '_', '|', '|', ' ', ' ', '<', ' ', 'о', 'с', 'у', 'щ', 'е', 'с', 'т', 'в', 'л', 'я', 'е', 'т', 'с', 'я', ' ', '-', ' ', 'с', 'т', 'р', 'е', 'л', 'к', 'а', 'м', 'и', ' ', ' ', ' ', ' ', '#' },
                    { '#', '\\', '\\', '\\', '/', '\\', '_', '/', '-', '\\', '_', '/', '-', '\\', '\\', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '/', '/', '_', ' ', '|', ' ', '|', ' ', '_', '\\', '\\', ' ', '<', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '#' },
                    { '#', '\\', '\\', '/', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '\\', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '/', '/', ' ', '#' },
                    { '#', '\\', '/', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '\\', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '\\', '\\', ' ', '#' },
                    { '#', '/', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '_', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '\\', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '/', '/', ' ', '#' },
                    { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '1', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '\\', ' ', ' ', ' ', '_', '_', '_', '_', '_', '_', '_', '_', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '_', ' ', ' ', '#' },
                    { '#', ' ', ' ', ' ', ' ', ' ', ' ', '/', ' ', ' ', ' ', '\\', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '|', ' ', '|', ' ', '#' },
                    { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', '-', '-', '-', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', '_', '|', ' ', '|', '_', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '/', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '\\', ' ', '|', ' ', '#' },
                    { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '|', ' ', '|', ' ', ' ', ' ', '|', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', '/', ' ', ' ', ' ', ' ', ' ', '\\', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '/', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '\\', '|', ' ', '#' },
                    { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', '|', ' ', ' ', '|', '_', '_', '_', '_', '-', ' ', ' ', ' ', '_', ' ', ' ', ' ', '-', '_', '_', '_', '_', '_', '_', '_', '_', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '/', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '\\', ' ', '#' },
                    { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '/', ' ', '\\', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '|', ' ', '#' },
                    { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '\\', '_', '/', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', '_', ' ', ' ', ' ', ' ', '_', ' ', ' ', ' ', ' ', '_', ' ', ' ', '|', ' ', '#' },
                    { '#', '^', '^', '^', '^', '^', '^', '^', '^', '^', '^', '^', '^', '^', '^', '^', '^', '^', '^', '^', '^', '>', ' ', ' ', ' ', ' ', ' ', ' ', '-', '-', '-', '-', '-', '-', '-', '-', '_', ' ', ' ', ' ', ' ', ' ', '_', '-', '-', '-', '-', '-', '-', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', '_', '|', ' ', ' ', '|', ' ', '|', ' ', ' ', '|', '_', '|', ' ', '|', ' ', '#' },
                    { '#', ' ', ' ', '_', '_', ' ', ' ', '_', ' ', ' ', ' ', ' ', '_', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '>', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '\\', ' ', ' ', ' ', '/', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', '_', '|', ' ', ' ', '|', '.', '|', ' ', ' ', '|', '_', '|', ' ', '|', ' ', '#' },
                    { '#', ' ', '|', ' ', ' ', ' ', '/', ' ', '\\', ' ', ' ', '|', ' ', '\\', ' ', ' ', '|', ' ', ' ', '|', ' ', '>', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '_', '_', '_', '_', '_', '_', '|', '_', '|', '_', '_', '_', '_', '_', '_', '|', ' ', '#' },
                    { '#', ' ', '|', ' ', ' ', '|', ' ', ' ', ' ', '|', ' ', '|', ' ', ' ', '|', ' ', '|', '\\', ' ', '|', ' ', '>', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', '|', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '/', ' ', '3', ' ', '\\', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                    { '#', ' ', '|', ' ', ' ', '|', ' ', ' ', ' ', '|', ' ', '|', '_', '/', ' ', ' ', '|', ' ', '|', '|', ' ', '>', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                    { '#', ' ', '|', ' ', ' ', ' ', '\\', '_', '/', ' ', ' ', '|', ' ', ' ', ' ', ' ', '|', '/', ' ', '|', ' ', '>', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '/', ' ', 'x', ' ', '\\', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '\\', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '/', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                    { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' }
                };
            }
            else if (num == 3) // лес
            {
                field = new char[,] {
                    { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
                    { '#', ' ', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '/', '-', '-', '-', '-', '-', '\\', ' ', ' ', ' ', '<', ' ', '1', '-', 'Л', 'е', 'с', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                    { '#', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '_', '_', ' ', ' ', '|', ' ', '/', '-', '-', '-', '\\', ' ', '|', ' ', ' ', '<', ' ', '2', '-', 'Л', 'а', 'б', 'и', 'р', 'и', 'н', 'т', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                    { '#', '|', ' ', ' ', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', ' ', '|', '/', ' ', ' ', '\\', ' ', '|', '|', ' ', ' ', ' ', ' ', ' ', '|', '|', ' ', ' ', '<', ' ', '3', '-', 'Т', 'а', 'в', 'е', 'р', 'н', 'а', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                    { '#', '|', ' ', '|', ' ', ' ', '_', '_', ' ', ' ', '_', '_', ' ', ' ', '_', '_', ' ', ' ', '_', '_', ' ', ' ', '_', '_', ' ', ' ', ' ', '|', ' ', '|', '|', ' ', ' ', '|', ' ', '|', '|', ' ', ' ', '2', ' ', ' ', '|', '|', ' ', ' ', '<', ' ', 'У', 'п', 'р', 'а', 'в', 'л', 'е', 'н', 'и', 'е', ' ', 'п', 'е', 'р', 'с', 'о', 'н', 'а', 'ж', 'е', 'м', ' ', '(', 'Х', ')', ' ', ' ', ' ', ' ', ' ', '#' },
                    { '#', '|', ' ', '|', ' ', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', ' ', ' ', '|', ' ', '|', '\\', '|', '|', '/', ' ', '|', '|', '_', ' ', ' ', ' ', '_', '|', '|', ' ', ' ', '<', ' ', 'о', 'с', 'у', 'щ', 'е', 'с', 'т', 'в', 'л', 'я', 'е', 'т', 'с', 'я', ' ', '-', ' ', 'с', 'т', 'р', 'е', 'л', 'к', 'а', 'м', 'и', ' ', ' ', ' ', ' ', '#' },
                    { '#', '|', ' ', '|', ' ', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', '|', ' ', '|', ' ', '|', '|', ' ', '/', '/', '_', ' ', '|', ' ', '|', ' ', '_', '\\', '\\', ' ', '<', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '#' },
                    { '#', '|', ' ', '|', ' ', '\\', '|', '|', '/', '\\', '|', '|', '/', '\\', '|', '|', '/', '\\', '|', '|', '/', '\\', '|', '|', '/', ' ', ' ', '|', ' ', '|', ' ', '_', '_', ' ', ' ', '_', '_', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '/', '/', ' ', '#' },
                    { '#', '|', ' ', '|', ' ', ' ', '|', '|', '_', '_', '|', '|', '_', '_', '|', '|', '_', '_', '|', '|', '_', '_', '|', '|', ' ', ' ', ' ', '|', ' ', '|', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '\\', '\\', ' ', '#' },
                    { '#', '|', ' ', '|', ' ', ' ', ' ', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', ' ', ' ', ' ', ' ', '|', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '/', '/', ' ', '#' },
                    { '#', '|', ' ', '|', ' ', ' ', ' ', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', ' ', ' ', ' ', ' ', '|', ' ', '|', '\\', '|', '|', '/', '\\', '|', '|', '/', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '_', ' ', ' ', '#' },
                    { '#', '|', ' ', '|', ' ', ' ', ' ', '\\', '|', '|', '/', '\\', '|', '|', '/', '\\', '|', '|', '/', '\\', '|', '|', '/', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', '|', '|', ' ', ' ', '|', '|', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '|', ' ', '|', ' ', '#' },
                    { '#', '|', ' ', '|', ' ', ' ', '_', '_', '|', '|', '_', '_', '|', '|', '_', '_', '|', '|', '_', '_', '|', '|', '_', '_', ' ', ' ', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '_', '|', ' ', '|', '_', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '/', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '\\', ' ', '|', ' ', '#' },
                    { '#', '|', ' ', '|', ' ', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', ' ', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '/', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '\\', '|', ' ', '#' },
                    { '#', '|', ' ', '|', ' ', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', ' ', ' ', '|', ' ', '|', '_', '_', '_', '_', '_', '-', ' ', ' ', ' ', '_', ' ', ' ', ' ', '-', '_', '_', '_', '_', '_', '_', '_', '_', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '/', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '\\', ' ', '#' },
                    { '#', '|', '1', '|', ' ', '\\', '|', '|', '/', '\\', '|', '|', '/', '\\', '|', '|', '/', '\\', '|', '|', '/', '\\', '|', '|', '/', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '|', ' ', '#' },
                    { '#', '_', '_', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '_', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', '_', ' ', ' ', ' ', ' ', '_', ' ', ' ', ' ', ' ', '_', ' ', ' ', '|', ' ', '#' },
                    { '#', '^', '^', '^', '^', '^', '^', '^', '^', '^', '^', '^', '^', '^', '^', '^', '^', '>', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '-', '-', '-', '-', '-', '-', '-', '-', '_', ' ', ' ', ' ', ' ', ' ', '_', '-', '-', '-', '-', '-', '-', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', '_', '|', ' ', ' ', '|', ' ', '|', ' ', ' ', '|', '_', '|', ' ', '|', ' ', '#' },
                    { '#', ' ', ' ', ' ', ' ', '_', ' ', ' ', ' ', '_', '_', ' ', ' ', ' ', '_', ' ', ' ', '>', ' ', '_', '_', ' ', ' ', '_', '_', ' ', ' ', '_', '_', ' ', ' ', '_', '_', ' ', ' ', '_', ' ', '|', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', '_', '|', ' ', ' ', '|', '.', '|', ' ', ' ', '|', '_', '|', ' ', '|', ' ', '#' },
                    { '#', ' ', ' ', ' ', '/', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', '/', ' ', '\\', ' ', '>', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '/', ' ', ' ', '\\', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '_', '_', '_', '_', '_', '_', '|', '_', '|', '_', '_', '_', '_', '_', '_', '|', ' ', '#' },
                    { '#', ' ', ' ', '|', ' ', ' ', '|', ' ', '|', '_', '_', ' ', '|', ' ', ' ', ' ', ' ', '>', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', '|', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '/', ' ', '3', ' ', '\\', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                    { '#', ' ', ' ', '|', ' ', ' ', '|', ' ', '|', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', '>', '\\', '|', '|', '/', '\\', '|', '|', '/', '\\', '|', '|', '/', '\\', '|', '|', '/', '\\', '|', '|', '/', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                    { '#', ' ', '_', '/', ' ', ' ', '|', ' ', '|', '_', '_', ' ', ' ', '\\', '_', '/', ' ', '>', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', ' ', ' ', '|', '|', '/', ' ', 'x', ' ', '\\', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '\\', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '/', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                    { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' }
                };
            }
            Render(field);
            while (gameStatus)
            {
                var keyInfo = Console.ReadKey();
                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    danger();
                moveHero(keyInfo, field, 0);

            }
        }

        static void randomEvent()
        {
            clear();
            Random rand = new Random();
            int temp;
            temp = rand.Next(21);
            int t, t2, t3;
            switch (temp)
            {
                case 1:
                    // монеты 10,20
                    t = rand.Next(10, 20);
                    Console.WriteLine($"\nВо время передышки вы присели и нашли в траве около себя {t} монет.\n");
                    money += t;
                    break;

                case 2:
                    // монеты 20,50
                    t = rand.Next(20, 50);
                    Console.WriteLine($"\nПо пути вам захотелось в туалет... ");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine($"В кустах, где вы делали \"свои дела\", вами был обнаружен спрятанный мешочек с монетами - {t} шт.\n");
                    money += t;
                    break;

                case 3:
                    // монеты 50,100
                    t = rand.Next(50, 100);
                    Console.WriteLine($"\nВы давно не ели и не пили, знойное солнце лишь усугубило ситуацию..." +
                        $"Вы стали по-немного сходить с ума. Во время очередного приступа, вы начали копать и обнаружили {t} золотых.\n");
                    money += t;
                    break;

                case 4: //Вы увидели звездопад и загодали увеличение денег или здоровья (желание может сбыться, может нет)
                    Console.WriteLine("\nОстановившись на ночлег и разведя костер, вы принялись готовить ужин...");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("В какой-то момент, в отражении воды, что была в котле, вы увидели звезды. ");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Подняв голову, вашему взору представились тысячи маленьких мерцающих огоньков! ");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Было тихо, только сверчки изредка давали о себе знать, вы уже доедали свою стрепню.");
                    Console.WriteLine(" Как вдруг , казалось бы, неподвижный небосвод оживился! ");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Десятки звезд одновременно устремлялись к горизонту!!!");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Что вы хотите загадать ?");
                    Console.WriteLine("Больше жизни");
                    Console.WriteLine("Больше денег");
                    int z = Console.Read();
                    if (z == 1)
                    {
                        t = rand.Next(2);
                        if (t == 1)
                        {
                            t = rand.Next(20, 60);
                            Console.WriteLine($"Ваше желание сбылось, вы нашли лечебный подорожник," +
                                $" который прибавил вам {t} единиц жизни.");
                            health += t;
                        }
                        else Console.WriteLine("К сожалению, ваше желание не сбылось :(");
                    }
                    else
                    {
                        t = rand.Next(2);
                        if (t == 1)
                        {
                            t = rand.Next(30, 60);
                            Console.WriteLine($"Ваше желание сбылось, вы нашли скелет Робин Гуда," +
                                $" в котором обнаружилось {t} золотых.");
                            health += t;
                        }
                        else Console.WriteLine("К сожалению, ваше желание не сбылось :(");
                    }
                    break;

                case 5:
                    //Кража удачная (получилось украсть деньги/амулеты)                                                               |
                    //Кража неудачная (вас поймали и избили , забрав ваши деньги)  
                    if (money <= 0) break;
                    Console.WriteLine("Не далеко от тропы, по которой вы шли, Вам послышался шум людей.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Подойдя поближе, вы увидели группу торговцев, остановившихся на опушке для ночлега.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Вы подумали, что если дождаться, пока все уснут, то у вас будет хорошая возможность\n" +
                        "чтобы украсть что-то.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Будете ли вы это делать?");
                    string a = Console.ReadLine();
                    a = Enter(a);
                    if ((a == "Да") || (a == "да") || (a == "ДА"))
                    {
                        Console.WriteLine("Вы дождались ночи.");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        Console.WriteLine("Все уснули.");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        Console.WriteLine("Под оглушающие звуки храпа вы пробрались к месту, где лежали вещи торговцев...");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        t = rand.Next(2);
                        if (t == 1)
                        {
                            Console.WriteLine("На цыпочках вы подкрались к месту, куда торговцы свалили все свои вещи.");
                            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                                danger();
                            Console.WriteLine("Один из торговцев начал издавать звуки пробуждения.");
                            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                                danger();
                            Console.WriteLine("Вы быстро схватили небольшой кошель с монетами, который лежал в одном из мешков с вещами.");
                            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                                danger();
                            Console.WriteLine("Так же тихо, как вошли, вы, не привлекая внимания, ушли с опушки\nи устремились куда подальше.");
                            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                                danger();
                            Console.WriteLine("Отойдя достаточно далеко, вы остановились, чтоб подсчитать свою добычу.");
                            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                                danger();
                            int q = (int)((float)money * 0.1F);
                            int w = (int)((float)money * 0.2F);
                            t3 = rand.Next(q, w);
                            Console.WriteLine($"Там было {t3} золотых.");
                            money += t3;
                            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                                danger();
                        }
                        else
                        {
                            t2 = rand.Next(2);
                            if (t2 == 1) Console.WriteLine("Было темно, вы не заметили, и наступили на сухую палку,\n" +
                                "которая громко хрустнула и разбудила всех торговцев...");
                            else Console.WriteLine("Как только ваша нога вступила на поляну, где отдыхали торговцы,\n" +
                                "ваш запах учуяла собака, подняв громкий лай, она разбудила всех торговцев...");
                            Console.WriteLine("Вас поймали и вривязали к дереву.");
                            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                                danger();
                            Console.WriteLine("Единогласно было принято решение избить вас и забрать в отместку часть ваших денег.");
                            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                                danger();
                            int q = (int)((float)money * 0.1F);
                            int w = (int)((float)money * 0.2F);
                            t3 = rand.Next(q, w);
                            q = (int)((float)health * 0.1F);
                            w = (int)((float)health * 0.2F);
                            int u = rand.Next(q, w);
                            Console.WriteLine($"Торговцы сжалились над вами и отняли только {t3} золотых и {u} единиц здоровья.");
                            money -= t3;
                            health -= u;
                            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                                danger();
                        }
                    }
                    else Console.WriteLine("У вас могло получиться... Зато, ваша совесть чиста!");
                    break;

                case 6:
                    Console.WriteLine("Погода была прекрасной, вы решили пройти как можно больше.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Когда вы вспомнили о ночлеге, было совсем темно.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("В крамешной тьме было практически ничего не видно, и решили остановиться\n" +
                        "у первого, попавшегося вам на пути, дерева.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Из-за того, что вчера вы шли исключительно долго, мы особенно сильно устали," +
                        "из-за чего ваш сон был оченнь крепким.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("На утро вы проснулись от сильной боли и обнаружили, что спали на муравейнике.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    t2 = rand.Next(10, 30);
                    Console.WriteLine($"Эта ночь отняла у вас {t2} единиц здоровья.");
                    health -= t2;
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    break;

                case 7:
                    //Нападение диких животных
                    Console.WriteLine("Ваше путешествие завело вас в лес.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Ближе к вечеру по пути вы обнаружили прекрасную поляну прямо у ручья.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Без раздумываний вы принялись готовить ужин и готовиться к ночлегу.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("В этот раз, ужин показался вам особенно вкусным.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Закончив трапезу, вы практически сразу же заснули.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("К сожалению, запах ужина, а так же наличие животной тропы рядом\n" +
                        "привели к вам диких зверей.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    t2 = rand.Next(1, 5);
                    if (t2 == 1)
                    {
                        Console.WriteLine("Этим зверем оказался - Дикий Заяц.");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        int q = (int)((float)health * 0.01F);
                        int w = (int)((float)health * 0.1F);
                        t3 = rand.Next(q, w);
                        Console.WriteLine("Вам повезло, вы не привлекли этого зверя, как добыча,\n" +
                            "и нанеся пару ударов, он ушел.");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        Console.WriteLine($"Урон вашему здоровью составил - {t3} ед. здоровья.");

                    }
                    else if (t2 == 2)
                    {
                        Console.WriteLine("Этим зверем оказался - Бурый медведь.");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        int q = (int)((float)health * 0.4F);
                        int w = (int)((float)health * 0.5F);
                        t3 = rand.Next(q, w);
                        Console.WriteLine("Вам повезло, вы не мед, и не - грибы, вы не привлекли этого зверя, как добыча.\n");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        Console.WriteLine("Пару раз ударив лапой, он убедился, что вы - живой, и в развалочку ушел.");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        Console.WriteLine($"Урон вашему здоровью составил - {t3} ед. здоровья.");
                    }
                    else if (t2 == 3)
                    {
                        Console.WriteLine("Ууу Вам не повезло...");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        Console.WriteLine("Этим зверем оказался - Волк-мать.");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        int q = (int)((float)health * 0.3F);
                        int w = (int)((float)health * 0.4F);
                        t3 = rand.Next(q, w);
                        Console.WriteLine("В лесу нет никого беспощадней - волка-матери.");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        Console.WriteLine("Но, видимо, удача была на вашей стороне, и проснувшись от укуса в шею,\n" +
                            "Вы поспешно залезли на дерево, где смогли укрыться от гнева этого монстра.");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        Console.WriteLine($"Утром вы слезли с дерева и приложили к шее подорожник,\n" +
                            $"Но, не смотря на это, все равно получили урон в {t3} ед. здоровья.");
                    }
                    else if (t2 == 4)
                    {
                        Console.WriteLine("Этим зверем оказалась - Рысь обыкновенная.");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        int q = (int)((float)health * 0.2F);
                        int w = (int)((float)health * 0.3F);
                        t3 = rand.Next(q, w);
                        Console.WriteLine("Вам повезло, вы долго не брились и кошка посчитала, что вы - клубок.\n" +
                            "Рысь игриво поиграла с вами, уталив свою страсть, пометила и пошла прочь.");
                        Console.WriteLine($"Урон вашему здоровью составил всего - {t3} ед. здоровья. Могло быть и хуже.");
                    }
                    else
                    {
                        Console.WriteLine("Этим зверем оказался - Кабан.");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        t3 = 0;
                        Console.WriteLine("Везение это или нет, решать вам, но...");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        Console.WriteLine("Кабан увидел в вас идеального полового партнера...,\n");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        Console.WriteLine("Вы не могли сопротивляться, он был слишком велик, накачен и красив собою.");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        Console.WriteLine($"Урон вашему физическому здоровью составил - {t3} ед. здоровья, моральному - [НЕВОЗМОЖНО ПОДСЧИТАТЬ]");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                    }
                    break;
                case 8:
                    //Простуда из-за плохих погодных условий
                    t2 = rand.Next(3);
                    if (t2 == 1)//много дней шел дождь 
                    {
                        Console.WriteLine("Много дней подряд шел дождь.");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        Console.WriteLine("Из-за густых свинцовых тучь, казалось, что солнце перехотело озарять землю своими лучами, и просто ушло восвояси. ");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        Console.WriteLine("Было темно, сыро и очень холодно. Несмотря, на максимальную осторожность, вы умудрились промокнуть до ниточки.\n");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        Console.WriteLine("В принципе, как и все остальное...\n");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        Console.WriteLine("Наконец дождь утих и погода наладилась, но от этого вам ни чуть не полегчало. У вас поднялся сильный жар и усталость\n" +
                            "была настолько сильна, что вы не могли даже встать...");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        Console.WriteLine("Вас нашла постушка в поле без сознания. Благо она жила недалеко и, не без помощи, притащив вас домой, помогла вам прийти\n" +
                            "в себя и набраться сил.");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        Console.WriteLine("Побыв еще пару дней у женщины, вы окончательно оправились и окрепли, для продолжения путешествия.\n");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        t = rand.Next(30, 70);
                        Console.WriteLine($"В благодарность, вы помогли ей по хозяйству и дали немного денег - {t}\n");
                        money -= t;
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                    }
                    else if (t2 == 2)// конец осени, снильный ветер
                    {
                        Console.WriteLine("Был конец осени.\n");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        Console.WriteLine("Место, где вы шли, славилось сильнейшими шквальными ветрами, из-за которых тут никто надолго не задерживался.\n");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        Console.WriteLine("В какой-то моент он стал настолько сильным, что вы были вынуждены прекратить свой путь и сделать остановку\n");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        Console.WriteLine("Огромный ясень, хоть и устрашающе гудел от ветра, но все же защищал вас от сильной непогоды.\n");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        Console.WriteLine("Кое-как вы развели небольшой костер и принялись готовить ранее пойманную мышку.\n");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        Console.WriteLine("Ваш пир прервал мощнейший ХРУСТ, все произошло быстро и ... больно.\n");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        Console.WriteLine("Вам на ногу упала большая ветка. Перелома, к счастью, не было, но ушиб еще неделю не давал вам ходить в полную силу.\n");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        Console.WriteLine("Урон здоровью - 30 единиц.\n");
                        health -= 30;
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                    }
                    else//зима холод 
                    {
                        Console.WriteLine("В этом году зима пришла рано и застала вас врасплох.\n");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        Console.WriteLine("Вы считали себя достаточно тепло одетым, но эта ночь доказала вам совершенно обратное.\n");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        Console.WriteLine("Проснувшись утром вы не чувствовали конечностей от холода, а ваша борода была полностью покрыта инеем.\n" +
                            "Ночные заморозки были настолько сильными, что даже костер потух.\n");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        Console.WriteLine("Вы были вынуждены отправиться в ближайшее поселение и купить себе там теплую одежду.\n");
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                        Console.WriteLine("Покупка теплой одежды - дело не дешевое. Эта покупка нанесла вам денежный урон в 100 монет.\n");
                        if (money >= 100) money -= 100;
                        else
                        {
                            int c8_dam = rand.Next(health);
                            Console.WriteLine("К сожалению у вас не хватило денег на покупку одежды, что привело к обморожению и физической травме в виде {c8_dam} единиц урона.");
                            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                                danger();
                        }
                    }
                    break;

                case 9:
                    //Нашел свиток со знаниями ... (+ к хп) 
                    Console.WriteLine("Дорога вела вас через заснеженные горы.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Вы остновились, ваше внимание заинтересовала занесенная снегом пещера, неподалеку вас.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Зайдя в нее вы увидели множество настенных рисунков, вославляющих свиток, и сам свиток,\n" +
                        "лежащий на каменном саркофаге");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Свиток оказался магическим и прочитав его, вы наложили на себя защитную печать.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Он так же , открыл вам древние зания и историю о монахе, который его написал и наделил силой.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    //мб добавить историю о монахе, которую игра будет выводить по желанию
                    Console.WriteLine("Ваше здоровье увеличилось на 50 ед.");
                    health += 50;
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    break;

                case 10:
                    //Компания, предлагает вам помощь (удачно) - вам помогают (+ к хп (отдых))                                      
                    //Компания, предлагает вам помощь (неудачно) - вас избили и обокрали
                    break;

                case 11:
                    //Укус змеи 
                    Console.WriteLine("Солнце было в зените.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Вы не прикрыли вовремя голову и заработали солнечный удар.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("У вас началось обильное потоотделение и начала кружиться голова.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Из-за сильной отдышки вы больше не можете продолжать путь и решаете присесть, отдохнуть");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Вы упали на землю...");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("В этот момент из сухостоя поблизости выскочила змея и укусила вас за руку.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Вы отсосали яд и ушли подальше от того места...");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Но все ущерб здоровью равный 15 ед. получили.");
                    health -= 15;
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    break;

                case 12:
                    //Вас обокрали (вор) (минус деньги) 
                    Console.WriteLine("День был тяжелым и вы спали, как младенец.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("К большому сожалению, мимо вас проходил вор, который решил этим воспользоваться.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Он ломал палки и шелестел ветками вблизи места вашего ночлега.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Убедившись, что вы крепко спите он медленно и тихо подошел к вам и забрал все, что только смог унести.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Так же тихо, как пришел, он развернулся и ушел восвояси...");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Не огорчайтесь, все могло быть и хуже... Вы живы! Уже этому должны быть благодарны.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    int th = (int)(health / 3);
                    Console.WriteLine($"Эта неприятность обошлась вам в {th} единиц золота.");
                    health -= th;
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    break;

                case 13:
                    //Вы по пути нашли броню (+ к хп) 
                    Console.WriteLine("В какой-то момент вы вышли на крупную тропу.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Идя по ней вы вышли на большое, не совсем обычное поле.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Необычным в этом поле было то, что оно практически все было увалено трупами воинов, которые, видимо, еще совсем недавно тут бились.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Над полем летали вороны, а у земли дымом стилился тяжелый туман.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Вы решили обойти поле и посмотреть, нет ли выживших.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Неподалеку от вас послушался хлиплый кашель, подбежав, вы увидели раненного воина.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Ранение было очень глубоким и было понятно, что ему осталось совсем недолго.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Перед смертью, мужчина попросил вас дать ему сделать последний, в его жизни, глоток эля.");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("В благодарность, он завещал вам свои доспехи и попросил вас беречь их в память о нем и его павших воинах...");
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    Console.WriteLine("Броня прибавила вам 60 ед. к хп.");
                    health += 60;
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        danger();
                    break;

                case 14:
                    //Сомнительный гриб 
                    Console.WriteLine("Вы уже долгое время ничего не ели");
                    Console.ReadLine();
                    Console.WriteLine("Слабость, головокружение и легкое недомогание преследуют вас уже далеко не один день.");
                    Console.ReadLine();
                    Console.WriteLine("На протяжении всего периода голодания вам встречались только сомнительные грибы, в съедобности которых,\n" +
                        "вы не были уверены.");
                    Console.ReadLine();
                    Console.WriteLine("Наконец вы стали чувствовать себя настолько плохо, что решили, что можете попробовать грибы, т.к. если не съедите,\n" +
                        "то все равно умрете от голода и терять вам нечего.");
                    Console.ReadLine();
                    Console.WriteLine("Вы вырвали глиб из земли и начали медленно подносить ко рту.");
                    Console.ReadLine();
                    t2 = rand.Next(2);
                    if (t2 == 1)
                    {
                        Console.WriteLine("Вдруг, ваше внимание привлекли еле заметные муравьи, которые спешно начали метаться, после того как вы, вырвав гриб,\n" +
                            "разворошили их муравейник.");
                        Console.ReadLine();
                        Console.WriteLine("Все они пытались спасти свои яйца.");
                        Console.ReadLine();
                        Console.WriteLine("Вас эта находка очень обрадовала, т.к. муравьиные яйца содержат в себе огромное количество, так сильно необходимых вам,\n" +
                            "питательных веществ.");
                        Console.ReadLine();
                        Console.WriteLine("Вы не наелись, но это определенно спасло вам жизнь.");
                        Console.ReadLine();
                    }
                    else
                    {
                        Console.WriteLine("Честно говоря, у гриба был отвратный вкус, но выберать не приходилось.");
                        Console.ReadLine();
                        Console.WriteLine("Если первый вы еще нормально съели, то следующие два пришлось запихивать через силу.");
                        Console.ReadLine();
                        Console.WriteLine("Как бы ужасно это не было, но грибы оказались съедобными и спасли вам жизнь, хоть и вызвали небольшие галлюцинации.");
                        Console.ReadLine();
                        Console.WriteLine("Ущерб здоровью - 15 ед.");
                        health -= 15;
                        Console.ReadLine();
                    }
                    break;

                case 15: // вода из ручья плохая/хорошая

                    break;

                case 16: // переходя небольшую реку обнаружил нерест лосося (наелся + к хп)

                    break;

                case 17: // пришлось обходить гору (вражеское поселение или что-то еще), вышел на побережье, обнаружил там останки кораблей (+ к золоту)

                    break;

                case 18: // спасли жизнь незнакомцу, он оказался кузнецом и в благодарность подарил его лучшую юроню (+ к хп)

                    break;

                case 19: // угодил в ловушку для животных (- хп)

                    break;

                case 20: // пробираясь сквозь высокуб траву порезал руки (- хп)

                    break;

                case 21:
                    Console.WriteLine("По пути, ничего необычного не произошло.");
                    break;
            }
            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                danger();

        }
        /*
        Console.WriteLine("");
        Console.ReadLine();
       */
        static void labyrinth()
        { }/*
            {
                frame.Add(new string[] { "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#" });
                frame.Add(new string[] { "#", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "-", "#" });
                frame.Add(new string[] { "#", "x", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "|", "#" });
                frame.Add(new string[] { "#", "_", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "|", "#" });
                frame.Add(new string[] { "#", "|", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "|", "#" });
                frame.Add(new string[] { "#", "|", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "|", "#" });
                frame.Add(new string[] { "#", "|", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "|", "#" });
                frame.Add(new string[] { "#", "|", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "|", "#" });
                frame.Add(new string[] { "#", "|", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "|", "#" });
                frame.Add(new string[] { "#", "|", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "|", "#" });
                frame.Add(new string[] { "#", "|", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "|", "#" });
                frame.Add(new string[] { "#", "|", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "|", "#" });
                frame.Add(new string[] { "#", "|", " ", " ", " ", "S", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "|", "#" });
                frame.Add(new string[] { "#", "|", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "|", "#" });
                frame.Add(new string[] { "#", "|", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "|", "#" });
                frame.Add(new string[] { "#", "|", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "|", "#" });
                frame.Add(new string[] { "#", "|", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "|", "#" });
                frame.Add(new string[] { "#", "|", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "|", "#" });
                frame.Add(new string[] { "#", "|", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "|", "#" });
                frame.Add(new string[] { "#", "|", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "|", "#" });
                frame.Add(new string[] { "#", "|", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "-", "#" });
                frame.Add(new string[] { "#", "|", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", "%", "#" });
                frame.Add(new string[] { "#", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "_", "#" });
                frame.Add(new string[] { "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#", "#" });
            }
            Render();
            while (gameStatus)
            {
				
                var keyInfo = Console.ReadKey();
                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    danger();
                moveHero(keyInfo, 1);
                Render();
            }
        }
        */
        static void Render(char[,] field)
        {
            /*
            clear();
            for (int x = 0; x < frame.Count; x++)
            {
                Console.WriteLine(string.Join("", frame[x]));
            }
			Console.SetCursorPosition (0,0);
            */
            /////////////////////////////////////////////////////////////////////////////
            clear();
            Console.SetCursorPosition(0, 0);
            Console.Clear();
            for (int i = 0; i < 24; i++)
            {
                for (int j = 0; j < 79; j++)
                    Console.Write(field[i, j]);
                Console.WriteLine();
            }
        }
        static void moveHero(ConsoleKeyInfo keyInfo, char[,] field, int loc_num, int value = 0)
        {
            int x = 0, y = 0;
            int x1 = 0, y1 = 0;
            Render(field);
            for (x = 23; x >= 0; x--)
            {
                for (y = 0; y < 78; y++)
                {
                    if (field[x, y] == hero)
                    {
                        x1 = x;
                        y1 = y;
                        if (keyInfo.Key == ConsoleKey.UpArrow)
                        {
                            if ((x - 1) >= 0 && field[x - 1, y] == game_enemy)
                            {
                                field[x, y] = emptyCell;
                                field[x - 1, y] = game_enemy;
                                gameStatus = false;
                                locations[loc_num, 0]();
                                return;
                            }
                            if ((x - 1) >= 0 && field[x - 1, y] == emptyCell)
                            {
                                field[x, y] = emptyCell;
                                field[x - 1, y] = hero;
                                if (value == 1) moveEnemy(x, y);
                                return;
                            }
                            else if ((x - 1) >= 0 && field[x - 1, y] == target)
                            {
                                field[x, y] = emptyCell;
                                field[x - 1, y] = hero;
                                field[x - 1, y] = target;
                                field[x1, y1] = hero;
                                gameStatus = false;
                                locations[loc_num, 0]();
                                return;
                            }
                            else if ((x - 1) >= 0 && field[x - 1, y] == lab)
                            {
                                field[x, y] = emptyCell;
                                field[x - 1, y] = hero;
                                gameStatus = false;
                                locations[loc_num, 1]();
                                labyrinth();
                                return;
                            }
                            else if ((x - 1) >= 0 && field[x - 1, y] == tavern)
                            {
                                field[x, y] = emptyCell;
                                field[x - 1, y] = hero;
                                gameStatus = false;
                                locations[loc_num, 2]();
                                Tavern();
                                return;
                            }
                        }
                        else if (keyInfo.Key == ConsoleKey.DownArrow)
                        {
                            if ((x + 1) <= 23 && field[x + 1, y] == game_enemy)
                            {
                                field[x, y] = emptyCell;
                                field[x + 1, y] = game_enemy;
                                gameStatus = false;
                                locations[loc_num, 0]();
                                endField();
                                return;
                            }
                            if ((x + 1) <= (23) && field[x + 1, y] == emptyCell)
                            {
                                field[x, y] = emptyCell;
                                field[x + 1, y] = hero;
                                if (value == 1) moveEnemy(x, y);
                                return;
                            }
                            else if ((x + 1) <= (23) && field[x + 1, y] == target)
                            {
                                field[x, y] = emptyCell;
                                field[x + 1, y] = hero;
                                field[x + 1, y] = target;
                                field[x1, y1] = hero;
                                gameStatus = false;
                                locations[loc_num, 0]();
                                return;
                            }
                            else if ((x + 1) <= (23) && field[x + 1, y] == lab)
                            {
                                field[x, y] = emptyCell;
                                field[x + 1, y] = hero;
                                gameStatus = false;
                                locations[loc_num, 1]();
                                return;
                            }
                            else if ((x + 1) <= (23) && field[x + 1, y] == tavern)
                            {
                                field[x, y] = emptyCell;
                                field[x + 1, y] = hero;
                                gameStatus = false;
                                locations[loc_num, 2]();
                                return;
                            }

                        }
                        else if (keyInfo.Key == ConsoleKey.LeftArrow)
                        {
                            if ((y - 1) >= 0 && field[x, y - 1] == game_enemy)
                            {
                                field[x, y] = emptyCell;
                                field[x, y - 1] = game_enemy;
                                gameStatus = false;
                                locations[loc_num, 0]();
                                return;
                            }
                            if ((y - 1) >= 0 && field[x, y - 1] == emptyCell)
                            {
                                field[x, y] = emptyCell;
                                field[x, y - 1] = hero;
                                if (value == 1) moveEnemy(x, y);
                                return;
                            }
                            else if ((y - 1) >= 0 && field[x, y - 1] == target)
                            {
                                field[x, y] = emptyCell;
                                field[x, y - 1] = hero;
                                field[x, y - 1] = target;
                                field[x1, y1] = hero;
                                gameStatus = false;
                                locations[loc_num, 0]();
                                return;
                            }
                            else if ((y - 1) >= 0 && field[x, y - 1] == lab)
                            {
                                field[x, y] = emptyCell;
                                field[x, y - 1] = hero;
                                gameStatus = false;
                                locations[loc_num, 1]();
                                return;
                            }
                            else if ((y - 1) >= 0 && field[x, y - 1] == tavern)
                            {
                                field[x, y] = emptyCell;
                                field[x, y - 1] = hero;
                                gameStatus = false;
                                locations[loc_num, 2]();
                                return;
                            }
                            else if ((y - 1) >= 0 && field[x, y - 1] == '4')
                            {
                                field[x, y] = emptyCell;
                                field[x, y - 1] = hero;
                                gameStatus = false;
                                locations[loc_num, 3]();
                                return;
                            }

                        }
                        else if (keyInfo.Key == ConsoleKey.RightArrow)
                        {
                            if ((y + 1) <= 78 && field[x, y + 1] == game_enemy)
                            {
                                field[x, y] = emptyCell;
                                field[x, y + 1] = game_enemy;
                                gameStatus = false;
                                locations[loc_num, 0]();
                                return;
                            }
                            if ((y + 1) <= 78 && field[x, y + 1] == emptyCell)
                            {
                                field[x, y] = emptyCell;
                                field[x, y + 1] = hero;
                                if (value == 1) moveEnemy(x, y);
                                return;
                            }
                            else if ((y + 1) <= 78 && field[x, y + 1] == target)
                            {
                                field[x, y] = emptyCell;
                                field[x, y + 1] = hero;
                                field[x, y + 1] = target;
                                field[x1, y1] = hero;
                                gameStatus = false;
                                locations[loc_num, 0]();
                                return;
                            }
                            else if ((y + 1) <= 78 && field[x, y + 1] == lab)
                            {
                                field[x, y] = emptyCell;
                                field[x, y + 1] = hero;
                                gameStatus = false;
                                gameStatus = false;
                                locations[loc_num, 1]();
                                return;
                            }
                            else if ((y + 1) <= 78 && field[x, y + 1] == tavern)
                            {
                                field[x, y] = emptyCell;
                                field[x, y + 1] = hero;
                                gameStatus = false;
                                locations[loc_num, 2]();
                                return;
                            }
                        }
                    }
                }
            }

        }

        static void endField()
        {
            clear();
            Console.WriteLine("///");
            Console.Beep();
            gameStatus = false;
            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                danger();
        }

        static void moveHero_Present(ConsoleKeyInfo keyInfo) { }/*
        {
            for (int x = frame.Count - 1; x >= 0; x--)
            {
                for (int y = 0; y < frame[x].Length; y++)
                {
                    if (frame[x][y] == hero)
                    {
                        if (keyInfo.Key == ConsoleKey.UpArrow)
                        {
                            if ((x - 1) >= 0 && frame[x - 1][y] == emptyCell)
                            {
                                frame[x][y] = emptyCell;
                                frame[x - 1][y] = hero;
                                return;
                            }
                            else if ((x - 1) >= 0 && frame[x - 1][y] == "1")
                            {
                                frame[x][y] = emptyCell;
                                frame[x - 1][y] = hero;
                                gameStatus = false;
                                return;
                            }
                            else if ((x - 1) >= 0 && frame[x - 1][y] == "2")
                            {
                                frame[x][y] = emptyCell;
                                frame[x - 1][y] = hero;
                                gameStatus = false;
                                return;
                            }
                            else if ((x - 1) >= 0 && frame[x - 1][y] == "3")
                            {
                                frame[x][y] = emptyCell;
                                frame[x - 1][y] = hero;
                                gameStatus = false;
                                return;
                            }
                        }
                        else if (keyInfo.Key == ConsoleKey.DownArrow)
                        {
                            if ((x + 1) <= (frame.Count - 1) && frame[x + 1][y] == emptyCell)
                            {
                                frame[x][y] = emptyCell;
                                frame[x + 1][y] = hero;
                                return;
                            }
                            else if ((x + 1) <= (frame.Count - 1) && frame[x + 1][y] == "1")
                            {
                                frame[x][y] = emptyCell;
                                frame[x + 1][y] = hero;
                                gameStatus = false;
                                return;
                            }
                            else if ((x + 1) <= (frame.Count - 1) && frame[x + 1][y] == "2")
                            {
                                frame[x][y] = emptyCell;
                                frame[x + 1][y] = hero;
                                gameStatus = false;
                                return;
                            }
                            else if ((x + 1) <= (frame.Count - 1) && frame[x + 1][y] == "3")
                            {
                                frame[x][y] = emptyCell;
                                frame[x + 1][y] = hero;
                                gameStatus = false;
                                return;
                            }
                        }
                        else if (keyInfo.Key == ConsoleKey.LeftArrow)
                        {
                            if ((y - 1) >= 0 && frame[x][y - 1] == emptyCell)
                            {
                                frame[x][y] = emptyCell;
                                frame[x][y - 1] = hero;
                                return;
                            }
                            else if ((y - 1) >= 0 && frame[x][y - 1] == "1")
                            {
                                frame[x][y] = emptyCell;
                                frame[x][y - 1] = hero;
                                gameStatus = false;
                                return;
                            }
                            else if ((y - 1) >= 0 && frame[x][y - 1] == "2")
                            {
                                frame[x][y] = emptyCell;
                                frame[x][y - 1] = hero;
                                gameStatus = false;
                                return;
                            }
                            else if ((y - 1) >= 0 && frame[x][y - 1] == "3")
                            {
                                frame[x][y] = emptyCell;
                                frame[x][y - 1] = hero;
                                gameStatus = false;
                                return;
                            }
                        }
                        else if (keyInfo.Key == ConsoleKey.RightArrow)
                        {
                            if ((y + 1) <= (frame[x].Length - 1) && frame[x][y + 1] == emptyCell)
                            {
                                frame[x][y] = emptyCell;
                                frame[x][y + 1] = hero;
                                return;
                            }
                            else if ((y + 1) <= (frame[x].Length - 1) && frame[x][y + 1] == "1")
                            {
                                frame[x][y] = emptyCell;
                                frame[x][y + 1] = hero;
                                gameStatus = false;
                                return;
                            }
                            else if ((y + 1) <= (frame[x].Length - 1) && frame[x][y + 1] == "2")
                            {
                                frame[x][y] = emptyCell;
                                frame[x][y + 1] = hero;
                                gameStatus = false;
                                return;
                            }

                            else if ((y + 1) <= (frame[x].Length - 1) && frame[x][y + 1] == "3")
                            {
                                frame[x][y] = emptyCell;
                                frame[x][y + 1] = hero;
                                gameStatus = false;
                                return;
                            }
                        }

                    }
                }
            }
        }
        */
        static void moveHero_Tavern(ConsoleKeyInfo keyInfo)
        { }/*
            for (int x = frame.Count - 1; x >= 0; x--)
            {
                for (int y = 0; y < frame[x].Length; y++)
                {
                    if (frame[x][y] == hero)
                    {
                        if (keyInfo.Key == ConsoleKey.UpArrow)
                        {
                            if ((x - 1) >= 0 && frame[x - 1][y] == emptyCell)
                            {
                                frame[x][y] = emptyCell;
                                frame[x - 1][y] = hero;
                                return;
                            }
                            else if ((x - 1) >= 0 && frame[x - 1][y] == "1")
                            {
                                frame[x][y] = emptyCell;
                                frame[x - 1][y] = hero;
                                gameStatus = false;
                                Sleep();
                                return;
                            }
                            else if ((x - 1) >= 0 && frame[x - 1][y] == "2")
                            {
                                frame[x][y] = emptyCell;
                                frame[x - 1][y] = hero;
                                gameStatus = false;
                                Eating();
                                return;
                            }
                            else if ((x - 1) >= 0 && frame[x - 1][y] == "3")
                            {
                                frame[x][y] = emptyCell;
                                frame[x - 1][y] = hero;
                                gameStatus = false;
                                roulette(ref money, ref health);
                                return;
                            }
                        }
                        else if (keyInfo.Key == ConsoleKey.DownArrow)
                        {
                            if ((x + 1) <= (frame.Count - 1) && frame[x + 1][y] == emptyCell)
                            {
                                frame[x][y] = emptyCell;
                                frame[x + 1][y] = hero;
                                return;
                            }
                            else if ((x + 1) <= (frame.Count - 1) && frame[x + 1][y] == "1")
                            {
                                frame[x][y] = emptyCell;
                                frame[x + 1][y] = hero;
                                gameStatus = false;
                                Sleep();
                                return;
                            }
                            else if ((x + 1) <= (frame.Count - 1) && frame[x + 1][y] == "2")
                            {
                                frame[x][y] = emptyCell;
                                frame[x + 1][y] = hero;
                                gameStatus = false;
                                Eating();
                                return;
                            }
                            else if ((x + 1) <= (frame.Count - 1) && frame[x + 1][y] == "3")
                            {
                                frame[x][y] = emptyCell;
                                frame[x + 1][y] = hero;
                                gameStatus = false;
                                roulette(ref money, ref health);
                                return;
                            }
                        }
                        else if (keyInfo.Key == ConsoleKey.LeftArrow)
                        {
                            if ((y - 1) >= 0 && frame[x][y - 1] == emptyCell)
                            {
                                frame[x][y] = emptyCell;
                                frame[x][y - 1] = hero;
                                return;
                            }
                            else if ((y - 1) >= 0 && frame[x][y - 1] == "1")
                            {
                                frame[x][y] = emptyCell;
                                frame[x][y - 1] = hero;
                                gameStatus = false;
                                Sleep();
                                return;
                            }
                            else if ((y - 1) >= 0 && frame[x][y - 1] == "2")
                            {
                                frame[x][y] = emptyCell;
                                frame[x][y - 1] = hero;
                                gameStatus = false;
                                Eating();
                                return;
                            }
                            else if ((y - 1) >= 0 && frame[x][y - 1] == "3")
                            {
                                frame[x][y] = emptyCell;
                                frame[x][y - 1] = hero;
                                gameStatus = false;
                                roulette(ref money, ref health);
                                return;
                            }
                            else if ((y - 1) >= 0 && frame[x][y - 1] == "4")
                            {
                                frame[x][y] = emptyCell;
                                frame[x][y - 1] = hero;
                                gameStatus = false;
                                return;
                            }
                        }
                        else if (keyInfo.Key == ConsoleKey.RightArrow)
                        {
                            if ((y + 1) <= (frame[x].Length - 1) && frame[x][y + 1] == emptyCell)
                            {
                                frame[x][y] = emptyCell;
                                frame[x][y + 1] = hero;
                                return;
                            }
                            else if ((y + 1) <= (frame[x].Length - 1) && frame[x][y + 1] == "1")
                            {
                                frame[x][y] = emptyCell;
                                frame[x][y + 1] = hero;
                                gameStatus = false;
                                Sleep();
                                return;
                            }
                            else if ((y + 1) <= (frame[x].Length - 1) && frame[x][y + 1] == "2")
                            {
                                frame[x][y] = emptyCell;
                                frame[x][y + 1] = hero;
                                gameStatus = false;
                                Eating();
                                return;
                            }

                            else if ((y + 1) <= (frame[x].Length - 1) && frame[x][y + 1] == "3")
                            {
                                frame[x][y] = emptyCell;
                                frame[x][y + 1] = hero;
                                gameStatus = false;
                                roulette(ref money, ref health);
                                return;
                            }
                        }

                    }
                }
            }
        }
        */
        static void moveEnemy(int x, int y)
        { }/*
            for (int a = frame.Count - 1; a >= 0; a--)
            {
                for (int b = 0; b < frame[x].Length; b++)
                {
                    if (frame[a][b] == game_enemy)
                    {
                        for (int i = 4; i != -4; i--) //x
                        {
                            for (int j = -4; j != 4; j++) //y
                            {
                                if ((a - i) <= 0 || (b + j) <= 0) continue;
                                if ((a - 1) <= 0 || (b - 1) <= 0) continue;
                                if ((a - i) > frame[a].Length - 1 || (b - j) > frame[b].Length - 1) continue;
                                if (frame[a - i][b + j] == hero)
                                {
                                    Console.Beep();
                                    if ((Math.Abs(x - (a - 1)) + Math.Abs(y - b) < (Math.Abs(x - a) + Math.Abs(y - b))))
                                    {
                                        if ((a - 1) >= 0 && frame[a - 2][b] == hero)
                                        {
                                            frame[a][b] = emptyCell;
                                            frame[a - 2][b] = game_enemy;
                                            endField();
                                            return;
                                        }
                                        if ((a - 1) >= 0 && frame[a - 1][b] == emptyCell)
                                        {
                                            frame[a][b] = emptyCell;
                                            frame[a - 1][b] = game_enemy;
                                            return;
                                        }

                                    }
                                    else if ((Math.Abs(x - (a + 1)) + Math.Abs(y - b) < (Math.Abs(x - a) + Math.Abs(y - b))))
                                    {
                                        if ((a + 1) <= (frame.Count - 1) && frame[a + 2][b] == hero)
                                        {
                                            frame[a][b] = emptyCell;
                                            frame[a + 2][b] = game_enemy;
                                            endField();
                                            return;
                                        }
                                        if ((a + 1) <= (frame.Count - 1) && frame[a + 1][b] == emptyCell)
                                        {
                                            frame[a][b] = emptyCell;
                                            frame[a + 1][b] = game_enemy;
                                            return;
                                        }

                                    }
                                    else if ((Math.Abs(x - a) + Math.Abs(y - (b - 1)) < (Math.Abs(x - a) + Math.Abs(y - b))))
                                    {
                                        if ((b - 1) >= 0 && frame[a][b - 2] == hero)
                                        {
                                            frame[a][b] = emptyCell;
                                            frame[a][b - 2] = game_enemy;
                                            endField();
                                            return;
                                        }
                                        if ((b - 1) >= 0 && frame[a][b - 1] == emptyCell)
                                        {
                                            frame[a][b] = emptyCell;
                                            frame[a][b - 1] = game_enemy;
                                            return;
                                        }
                                    }
                                    else if ((Math.Abs(x - a) + Math.Abs(y - (b + 1)) < (Math.Abs(x - a) + Math.Abs(y - b))))
                                    {
                                        if ((b + 1) <= (frame[a].Length - 1) && frame[a][b + 2] == hero)
                                        {
                                            frame[a][b] = emptyCell;
                                            frame[a][b + 2] = game_enemy;
                                            endField();
                                            return;
                                        }
                                        if ((b + 1) <= (frame[a].Length - 1) && frame[a][b + 1] == emptyCell)
                                        {
                                            frame[a][b] = emptyCell;
                                            frame[a][b + 1] = game_enemy;
                                            return;
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            }
        }
        */
        static void Tavern()
        {

            char[,] tavern = new char[24, 79]
            {
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
                { '#', ' ', ' ', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', '_', '_', '_', ' ', ' ', ' ', ' ', ' ', '_', '_', '_', ' ', ' ', ' ', ' ', ' ', '_', '_', '_', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', '|', ' ', '1', '.', 'Л', 'е', 'ч', 'ь', ' ', 'с', 'п', 'а', 'т', 'ь', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                { '#', ' ', '|', ' ', '\\', '_', '_', '_', '_', '_', '_', '_', '/', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '\\', ' ', '|', ' ', ' ', ' ', '|', ' ', ' ', ' ', '|', ' ', ' ', ' ', '|', ' ', ' ', ' ', '|', ' ', ' ', ' ', '|', ' ', '/', ' ', ' ', ' ', ' ', ' ', '|', ' ', '2', '.', 'П', 'о', 'е', 'с', 'т', 'ь', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                { '#', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '\\', '|', ' ', 'Е', ' ', '|', '\\', '_', '/', '|', ' ', 'Д', ' ', '|', '\\', '_', '/', '|', ' ', 'А', ' ', '|', '/', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '3', '.', 'С', 'ы', 'г', 'р', 'а', 'т', 'ь', ' ', 'в', ' ', 'р', 'у', 'л', 'е', 'т', 'к', 'у', ' ', ' ', '#' },
                { '#', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '_', '_', '_', '|', ' ', ' ', ' ', '|', '_', '_', '_', '|', ' ', ' ', ' ', '|', '_', '_', '_', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ','|', ' ', ' ', ' ', '(', '5', '0', '0', ' ', 'м', 'о', 'н', 'е', 'т', ')', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                { '#', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '4', '.', 'В', 'ы', 'х', 'о', 'д', ' ', 'и', 'з', ' ', 'т', 'а', 'в', 'е', 'р', 'н', 'ы', ' ', ' ', ' ', '#' },
                { '#', ' ', '|', ' ', '/', '-', '-', '-', '-', '-', '-', '-', '\\', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '#' },
                { '#', ' ', '|', ' ', '_', '_', '_', '_', '_', '_', '_', '_', '_', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '/', 'o', 'o', ' ', '\\', '_', '_', '/', 'Y', ' ', '\\', '_', '_', '/', 'Y', ' ', 'O', 'o', '0', ' ', '/', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '_', '_', '_', '_', '_', '_', '_', '_', ' ', ' ', '#' },
                { '#', ' ', '|', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '/', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', ' ', '/', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '/', '|', ' ', ' ', ' ', ' ', '_', ' ', ' ', ' ', '|', ' ', '#' },
                { '#', ' ', '|', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '|', ' ', ' ', '1', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '*', 'Я', 'б', 'л', 'о', 'к', 'о', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', ' ', '/', ' ', '|', ' ', '\\', ' ', '|', ' ', '#' },
                { '#', ' ', '|', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '*', 'С', 'а', 'л', 'а', 'т', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', '|', ' ', ' ', 'O', ' ', ' ', '|', '|', ' ', '#' },
                { '#', ' ', '|', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '*', 'С', 'у', 'п', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', ' ', '\\', ' ', '_', ' ', '/', ' ', '|', ' ', '#' },
                { '#', ' ', '|', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '*', 'К', 'о', 'т', 'л', 'е', 'т', 'а', ' ', 'и', ' ', 'г', 'а', 'р', 'н', 'и', 'р', ' ', '|', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', ' ', '_', ' ', '_', ' ', '_', ' ', '|', ' ', '#' },
                { '#', ' ', '|', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '*', 'Я', 'г', 'о', 'д', 'н', 'ы', 'й', ' ', 'п', 'и', 'р', 'о', 'г', ' ', ' ', ' ', ' ', '|', ' ', '/', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', '|', '_', '|', '_', '|', '_', '|', '|', ' ', '#' },
                { '#', ' ', '|', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '|', '/', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', '|', '_', '|', '_', '|', '_', '|', '|', ' ', '#' },
                { '#', ' ', '|', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '3', ' ', '|', ' ', '|', ' ', '|', '_', '|', '_', '|', '_', '|', '|', ' ', '#' },
                { '#', ' ', '|', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '2', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', '|', '_', '|', '_', '|', '_', '|', '|', ' ', '#' },
                { '#', ' ', '|', '|', '_', '_', '_', '_', '_', '_', '_', '_', '_', '|', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', '|', '_', '|', '_', '|', '_', '|', '|', ' ', '#' },
                { '#', ' ', '|', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', '|', '_', '|', '_', '|', '_', '|', '|', ' ', '#' },
                { '#', ' ', '\\', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '/', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', '|', '_', '|', '_', '|', '_', '|', '|', ' ', '#' },
                { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', ' ', '|', '_', '|', '_', '|', '_', '|', '|', ' ', '#' },
                { '#', ' ', ' ', '_', '_', '_', '_', '_', '_', '_', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '_', '_', '_', '_', '_', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', '|', '_', '_', '_', '_', '_', '_', '_', '_', '|', ' ', '#' },
                { '#', ' ', '|', 'Т', 'А', 'В', 'Е', 'Р', 'Н', 'А', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '4', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'x', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '/', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '/', ' ', '#' },
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' }
            };
            Render(tavern);
            gameStatus = true;
            while (gameStatus)
            {

                var keyInfo = Console.ReadKey();
                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    danger();
                moveHero(keyInfo, tavern, 1);
                Render(tavern);
            }
            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                danger();
        }

        static void Level()
        {
            clear();
            Random rand = new Random();
            int temp;
            clear();
            if ((health >= 300) && (Hero_Level == 1))
            {
                Hero_Level = 2;
                temp = rand.Next(2);
                if (temp == 1) Console.WriteLine($"\t\t\tПоздравляем, вы достигли {Hero_Level}-го уровня !!! ");
                else Console.WriteLine($"\t\t\tПоздравляем, вы перешли на новый - {Hero_Level}-й уровень !!! ");
                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    danger();
                Present(Hero_Level);
            }
            else if ((health >= 500) && (Hero_Level == 2))
            {
                Hero_Level = 3;
                temp = rand.Next(2);
                if (temp == 1) Console.WriteLine($"\t\t\tПоздравляем, вы достигли {Hero_Level}-го уровня !!! ");
                else Console.WriteLine($"\t\t\tПоздравляем, вы перешли на новый - {Hero_Level}-й уровень !!! ");
                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    danger();
                Present(Hero_Level);
            }
            else if ((health >= 1000) && (Hero_Level == 3))
            {
                Hero_Level = 4;
                temp = rand.Next(2);
                if (temp == 1) Console.WriteLine($"\t\t\tПоздравляем, вы достигли {Hero_Level}-го уровня !!! ");
                else Console.WriteLine($"\t\t\tПоздравляем, вы перешли на новый - {Hero_Level}-й уровень !!! ");
                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    danger();
                Present(Hero_Level);
            }
            else if ((health >= 3000) && (Hero_Level == 4))
            {
                Hero_Level = 5;
                temp = rand.Next(2);
                if (temp == 1) Console.WriteLine($"\t\t\tПоздравляем, вы достигли {Hero_Level}-го уровня !!! ");
                else Console.WriteLine($"\t\t\tПоздравляем, вы перешли на новый - {Hero_Level}-й уровень !!! ");
                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    danger();
                Present(Hero_Level);
            }
            else if ((health >= 5000) && (Hero_Level == 5))
            {
                Hero_Level = 6;
                temp = rand.Next(2);
                if (temp == 1) Console.WriteLine($"\t\t\tПоздравляем, вы достигли {Hero_Level}-го уровня !!! ");
                else Console.WriteLine($"\t\t\tПоздравляем, вы перешли на новый - {Hero_Level}-й уровень !!! ");
                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    danger();
                Present(Hero_Level);
            }
            else if ((health >= 8000) && (Hero_Level == 6))
            {
                Hero_Level = 7;
                temp = rand.Next(2);
                if (temp == 1) Console.WriteLine($"\t\t\tПоздравляем, вы достигли {Hero_Level}-го уровня !!! ");
                else Console.WriteLine($"\t\t\tПоздравляем, вы перешли на новый - {Hero_Level}-й уровень !!! ");
                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    danger();
                Present(Hero_Level);
            }

        }

        static void Present(int level)
        {
            clear();
            Random rand = new Random();
            int temp;
            char[,] present = new char[24, 79] {
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
                { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'В', 'ы', 'б', 'е', 'р', 'и', ' ', 'л', 'ю', 'б', 'о', 'й', ' ', 'п', 'о', 'д', 'а', 'р', 'о', 'к', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '_', ' ', ' ', ' ', ' ', '_', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '_', ' ', ' ', ' ', ' ', '_', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '_', ' ', ' ', ' ', ' ', '_', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '/', '\\', '\\', ' ', ' ', '/', '/', '\\', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '/', '\\', '\\', ' ', ' ', '/', '/', '\\', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '/', '\\', '\\', ' ', ' ', '/', '/', '\\', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '\\', '_', '\\', '\\', '/', '/', '_', '/', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '\\', '_', '\\', '\\', '/', '/', '_', '/', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '\\', '_', '\\', '\\', '/', '/', '_', '/', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '-', '-', '-', '-', '|', '|', '-', '-', '-', '-', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '-', '-', '-', '-', '|', '|', '-', '-', '-', '-', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '-', '-', '-', '-', '|', '|', '-', '-', '-', '-', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '-', '-', '-', '-', '|', '|', '-', '-', '-', '-', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '-', '-', '-', '-', '|', '|', '-', '-', '-', '-', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', '-', '-', '-', '-', '|', '|', '-', '-', '-', '-', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', '|', '|', ' ', ' ', ' ', ' ', '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '-', '-', '-', '-', '-', '-', '-', '-', '-', '-', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '1', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '2', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '3', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'x', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
                { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' },
            };
            Render(present);
            gameStatus = true;
            while (gameStatus)
            {
                var keyInfo = Console.ReadKey();
                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    danger();
                moveHero(keyInfo, present, 3);
                Render(present);
            }
            switch (level)
            {
                case 2:
                    {
                        temp = rand.Next(3);
                        if (temp == 1) Console.WriteLine("Ваша награда:\n");
                        else if (temp == 2) Console.WriteLine("В награду вы получаете:");
                        else if (temp == 3) Console.WriteLine("Приз за достижение уровня:");
                        Console.WriteLine("1)50 монет");
                        money += 50;
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();

                    }
                    break;

                case 3:
                    {
                        temp = rand.Next(3);
                        if (temp == 1) Console.WriteLine("Ваша награда:\n");
                        else if (temp == 2) Console.WriteLine("В награду вы получаете:");
                        else if (temp == 3) Console.WriteLine("Приз за достижение уровня:");
                        Console.WriteLine("1)75 монет");
                        money += 50;
                        Console.WriteLine("2)50 ед. жизни");
                        health += 50;
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();

                    }
                    break;

                case 4:
                    {
                        temp = rand.Next(3);
                        if (temp == 1) Console.WriteLine("Ваша награда:\n");
                        else if (temp == 2) Console.WriteLine("В награду вы получаете:");
                        else if (temp == 3) Console.WriteLine("Приз за достижение уровня:");
                        Console.WriteLine("1)Волшебный амулет жизни (+150 к жизни)");
                        health += 200;
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                    }
                    break;

                case 5:
                    {
                        temp = rand.Next(3);
                        if (temp == 1) Console.WriteLine("Ваша награда:\n");
                        else if (temp == 2) Console.WriteLine("В награду вы получаете:");
                        else if (temp == 3) Console.WriteLine("Приз за достижение уровня:");
                        Console.WriteLine("1)Запасная жизнь (в случае, если вы умрете, вы восстановитесь с показателем здоровья - 500)");
                        life = 1;
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                    }
                    break;

                case 6:
                    {
                        temp = rand.Next(3);
                        if (temp == 1) Console.WriteLine("Ваша награда:\n");
                        else if (temp == 2) Console.WriteLine("В награду вы получаете:");
                        else if (temp == 3) Console.WriteLine("Приз за достижение уровня:");
                        Console.WriteLine("1) Сундук золота (2000 монет)");
                        money += 1000;
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                    }
                    break;

                case 7:
                    {
                        temp = rand.Next(3);
                        if (temp == 1) Console.WriteLine("Ваша награда:\n");
                        else if (temp == 2) Console.WriteLine("В награду вы получаете:");
                        else if (temp == 3) Console.WriteLine("Приз за достижение уровня:");
                        Console.WriteLine("1)Улучшенная запасная жизнь (в случае, если вы умрете, вы восстановитесь с показателем здоровья - 2500)");
                        life = 777;
                        if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                            danger();
                    }
                    break;

            }
        }

        static void Sleep()
        {
            Console.WriteLine($"health = {health} money = {money}");
            if (money >= 50)
            {
                Timer t = new Timer(TimerCallback, null, 0, 30000);
            }
            else
            {
                Console.WriteLine("У вас недостаточно денег.");
            }
            Console.ReadLine();
        }

        static void Eating()
        {
            clear();
            Console.WriteLine("Добро пожаловать в таверну!");
            Console.WriteLine("\n\n\n\n\n\n\nНажмите Enter чтобы продолжить...");
            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                danger();
            clear();
            Console.WriteLine(" ____________________________________________________");
            Console.WriteLine("|             МЕНЮ             |  + к xp  |   Цена   | ");
            Console.WriteLine(" ------------------------------|----------|----------| ");
            Console.WriteLine("|   1. Яблоко                  |   20     |    19    | ");
            Console.WriteLine("|   2. Салат                   |   50     |    45    | ");
            Console.WriteLine("|   3. Суп                     |   120    |    120   | ");
            Console.WriteLine("|   4. Котлета с гарниром      |   200    |    250   | ");
            Console.WriteLine("|   5. Ягодный пирог           |   450    |    600   | ");
            Console.WriteLine("|______________________________|__________|__________| ");
            Console.WriteLine("|   0. Выход                   |          |          | ");
            Console.WriteLine("|______________________________|__________|__________| ");
            Console.WriteLine("\n\n\n\n");
            string vibor_eating;
        place:
            Console.Write("Введите необходимый номер: ");
            vibor_eating = Console.ReadLine();
            clear();
            if ((vibor_eating == "1.") || (vibor_eating == "1") || (vibor_eating == "Яблоко") || (vibor_eating == "ЯБЛОКО") || (vibor_eating == "яблоко"))
            {
                Console.WriteLine("Вы съели яблоко.");
                Console.WriteLine("Прибавилось: 20 ед. здоровья");
                Console.WriteLine("Убавилось: 19 монет");
                health += 20;
                money -= 19;
            }
            else if ((vibor_eating == "2.") || (vibor_eating == "2") || (vibor_eating == "Салат") || (vibor_eating == "САЛАТ") || (vibor_eating == "салат"))
            {
                Console.WriteLine("Вы съели салат.");
                Console.WriteLine("Прибавилось: 50 ед. здоровья");
                Console.WriteLine("Убавилось: 45 монет");
                health += 50;
                money -= 45;
            }
            else if ((vibor_eating == "3.") || (vibor_eating == "3") || (vibor_eating == "Суп") || (vibor_eating == "СУП") || (vibor_eating == "суп"))
            {
                Console.WriteLine("Вы съели Суп.");
                Console.WriteLine("Прибавилось: 120 ед. здоровья");
                Console.WriteLine("Убавилось: 120 монет");
                health += 120;
                money -= 120;
            }
            else if ((vibor_eating == "4.") || (vibor_eating == "4") || (vibor_eating == "Котлета с гарниром") || (vibor_eating == "КОТЛЕТА С ГАРНИРОМ") || (vibor_eating == "котлета с гарниром"))
            {
                Console.WriteLine("Вы выбрали котлетку.\n\n\n");
                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    danger();
                Console.WriteLine("Выберите гарнир:");
                Console.WriteLine("1. Макарошки");
                Console.WriteLine("2. Пюрешка");
            place2:
                Console.WriteLine("Введите необходимый номер: ");
                string vibor_eating2 = Console.ReadLine();
                Console.Write("\n\n\n");
                if ((vibor_eating2 == "1.") || (vibor_eating2 == "1") || (vibor_eating2 == "Макарошки") || (vibor_eating2 == "МАКАРОШКИ") || (vibor_eating2 == "макарошки"))
                {
                    Console.WriteLine("Вы выбрали котлетку с макарошками.\n");
                    Console.WriteLine("Прибавилось: 200 ед. здоровья");
                    Console.WriteLine("Убавилось: 250 монет");
                    health += 200;
                    money -= 250;
                }
                else if ((vibor_eating2 == "2.") || (vibor_eating2 == "2") || (vibor_eating2 == "Пюрешка") || (vibor_eating2 == "ПЮРЕШКА") || (vibor_eating2 == "пюрешка"))
                {
                    Console.WriteLine("Вы выбрали котлетку с пюрешкой.\n");
                    Random rand = new Random();
                    int temp;
                    temp = rand.Next(3);
                    if (temp == 3)
                    {
                        Console.WriteLine("Бонус за выбор 50$\n");
                        Console.WriteLine("Прибавилось: 200 ед. здоровья");
                        Console.WriteLine("Убавилось: 250-50=200 монет");
                        health += 200;
                        money -= 200;
                    }
                }
                else
                {
                    clear();
                    goto place2;
                }
            }
            else if ((vibor_eating == "5.") || (vibor_eating == "5") || (vibor_eating == "Ягодный пирог") || (vibor_eating == "ЯГОДНЫЙ ПИРОГ") || (vibor_eating == "ягодный пирог"))
            {
                Console.WriteLine("Вы съели ягодный пирог.");
                Console.WriteLine("Прибавилось: 450 ед. здоровья");
                Console.WriteLine("Убавилось: 600 монет");
            }
            else if ((vibor_eating == "0.") || (vibor_eating == "0")) return;
            else
            {
                clear();
                goto place;
            }
            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                danger();
        }

        static void TimerCallback(object o)
        {
            if (money < 50) return;
            money -= 50;
            health += 30;
            GC.Collect();//это че???
            Console.WriteLine($"health = {health} money = {money}");
        }
        static void rebirth()
        {
            if (health <= 0)
            {
                clear();
                if (life == 1) health = 500;
                else if (life == 777) health = 2500;
                Console.WriteLine($"Вы использовали запасную жизнь, ваше здоровье - {health}");
            }
        }
        static void clear()
        {
            Console.SetCursorPosition(0, 0);
            for (int x = 0; x < Console.WindowHeight; x++)
            {
                Console.Write(new string(' ', Console.WindowWidth));
            }
            Console.SetCursorPosition(0, 0);

        }
    }
}
// сохранение
// добавить врагов (должно быть 5 врагов и 3 удара (удар мечем/магией/щит(щит будет пробиваться, если враг будет использовать сильный удар)
// сон в таверне отрисовать её + там в табличке будет написано "не хотите  ли передохнуть в таверне ?" а ниже так же на баннере будет написана цена за час отдыха .
/* платишь деньги - отдыхаешь
 * минус деньги
 * плюс хп
 * */
// за деньги можно будет узнать, какой враг на каком пути находится 
/*
 * на рисунках с локациями будет 3 пути : на врага , в локацию и обход.
 * когда идешь в обход, с тебя снимаются деньги
 * когда идешь на врага, происходит сражение
 * когда идешь в локацию, ты попадаешь в лабиринт, где ходят монстры , которые хотят тебя съесть, они тебя обнаружат, если ты находишься на 5 шагов от них в любую сторону , если они тебя обнаружат, они побегут на тебя
 * надо пройти лабиринт не попашись на глаза монстрам - "^", персонаж - "*" , цель - "Х"
 *
 */
// Добавить уровни персонажа (к примеру, 5), каждый раз, когда здоровье персонажа будет переходить за какую-то отметку, он будет получать новый уровень 
// дальше будет текстура с 3-мя подарками, к какому он подойдет, то и выиграет (там будут деньги, амулеты с прибавлением здоровья и дополнительные жизни )
/*
 * создать 20 рандомных событий в игре                                                                               
 * 1)Кража удачная (получилось украсть деньги/амулеты)                                                               |+
 * //Кража неудачная (вас поймали и избили , забрав ваши деньги)                                                     |+
 * 2)Муравейник                                                                                                      |+
 * 3)Нападение диких животных                                                                                        |+
 * 4)Простуда из-за плохих погодных условий                                                                          |
 * 5)Нашел свиток со знаниями ... (+ к хп)                                                                           |+
 * 6)Компания, предлагает вам помощь (удачно) - вам помогают (+ к хп (отдых))                                        |
 * //Компания, предлагает вам помощь (неудачно) - вас избили и обокрали                                              |
 * 7)Укус змеи                                                                                                       |+
 * 8)Вас обокрали (вор) (минус деньги)                                                                               |
 * 9)Вы по пути нашли броню (+ к хп)                                                                                 |
 * 10)Монеты до 20                                                                                                   |+
 * 11)Монеты до 50                                                                                                   |+
 * 12)Монеты до 100                                                                                                  |+
 * 13)Вы увидели звездопад и загодали увеличение денег или здоровья (желание может сбыться, может нет)               |+
 * 14)Сомнительный гриб                                                                                              |+
 * 15)                                                                                                     |
 * 16)
 * 17)
 * 18)
 * 19)
 * 20)


//if (enter.Key == ConsoleKey.Escape)danger();

// посмотреть нет ли нигде ошибок с gamestatus
//Оптимизировать функцию перемещения  
// прописать , что уровень напрямую зависит отпрогресса и если человек потерял прогресс (жизнь и урон), то уровень тоже падает
//добавить вывовод победы - зеленым цветом, а поражения\побега - красным


if (Console.ReadKey(true).Key == ConsoleKey.F2)
	Console.WriteLine("F2 Pressed.");

switch (Console.ReadKey(true).Key) {
case ConsoleKey.P:
	play = false;
	break;
case ConsoleKey.W:
	KoorY--;
	break;
case ConsoleKey.S:
	KoorY++;
	break;
case ConsoleKey.D:
	KoorX++;
	break;
case ConsoleKey.A:
	KoorX--;
	break;
default:
	// smth
	break;
}


Console.ReadKey(true).KeyChar

содержит char, так что можете сравнивать
C#
Выделить код

Console.ReadKey(true).KeyChar == 'F'







*/
