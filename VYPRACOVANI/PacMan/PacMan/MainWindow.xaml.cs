using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PacMan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Zakladaní proměných
        Ellipse player;
        Ellipse monster;
        Point start;
        Point startMonster;
        List<Point> walls = new List<Point>();
        List<Ellipse> food = new List<Ellipse>();
        List<Ellipse> superFood = new List<Ellipse>();
        // 3 životy
        int life = 3;
        // Skóre 0
        int score = 0;
        //Nesmrtelnost + časovač na ni
        static bool invicible = false;
        Thread timing = new Thread(() => { invicible = true; Thread.Sleep(10000); invicible = false; });
        public MainWindow()
        {
            InitializeComponent();
            //Hledaní souboru s mapou
            OpenFileDialog fd = new OpenFileDialog()
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false,
                Title = "Vyberte mapu",
                Filter = "*.map|*.map"
            };
            fd.ShowDialog();
            int radek = 0;
            //načtení souboru
            using (StreamReader sr = new StreamReader(fd.FileName))
                while (!sr.EndOfStream)
                {
                    //přečtění řádku
                    string line = sr.ReadLine();
                    for (int i = 0; i < line.Length; i++)
                    {
                        //Hledaní Zdi
                        if (line[i] == 'x' || line[i] == 'X')
                        {
                            walls.Add(new Point(i * 10, radek + 10));
                            grid.Children.Add(new Rectangle() { Width = 10, Height = 10, Fill = Brushes.Black, Margin = new Thickness(i * 10, radek + 10, 0, 0), VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left });
                            continue;
                        }
                        //Hledaní žrádla
                        if (line[i] == '.')
                        {
                            Ellipse el = new Ellipse() { Width = 10, Height = 10, Fill = Brushes.Green, Margin = new Thickness(i * 10, radek + 10, 0, 0), VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left };
                            food.Add(el);
                            grid.Children.Add(el);
                            continue;
                        }
                        //Hledaní super Žrádla
                        if (line[i] == 'S' || line[i] == 's')
                        {
                            Ellipse el = new Ellipse() { Width = 10, Height = 10, Fill = Brushes.Blue, Margin = new Thickness(i * 10, radek + 10, 0, 0), VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left };
                            superFood.Add(el);
                            grid.Children.Add(el);
                            continue;
                        }
                        //Hledaní Pacmana
                        if (line[i] == 'P' || line[i] == 'p')
                        {
                            player = new Ellipse() { Width = 10, Height = 10, Fill = Brushes.Black, Stroke = Brushes.Wheat, StrokeThickness = 1, Margin = new Thickness(i * 10, radek + 10, 0, 0), VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left };
                            grid.Children.Add(player);
                            start = new Point(i * 10, radek + 10);
                            continue;
                        }
                        //Hledaní Monstra
                        if (line[i] == 'M' || line[i] == 'm')
                        {
                            monster = new Ellipse() { Width = 10, Height = 10, Fill = Brushes.Purple, Margin = new Thickness(i * 10, radek + 10, 0, 0), VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left };
                            grid.Children.Add(monster);
                            startMonster = new Point(i * 10, radek + 10);
                            continue;
                        }
                    }
                    //Odsazení
                    radek += 10;
                }
            //Thead kontrolujicí výhru
            new Thread(() => { while (true) { if (food.Count == 0 && superFood.Count == 0) { MessageBox.Show($"Pacman vyhrál! Skóre: {score}"); break; } Thread.Sleep(500); } }).Start();
        }

        private void window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //pozice hráče
            Point position = new Point(player.Margin.Left, player.Margin.Top);
            if (e.Key == Key.Up)
            {
                //Kontrola zdí
                foreach (var item in walls)
                    if (item.X == position.X && item.Y == position.Y - 10)
                        return;
                //posunutí hráče
                player.Margin = new Thickness(position.X, position.Y - 10, 0, 0);
                //Kontrola žrádla
                foreach (var item in food)
                    if (item.Margin.Left == position.X && item.Margin.Top == position.Y - 10)
                    {
                        score += 10;
                        grid.Children.Remove(item);
                        food.Remove(item);
                        break;
                    }
                //kontrola superžrádla + časovač na nesmrtelnost
                foreach (var item in superFood)
                    if (item.Margin.Left == position.X && item.Margin.Top == position.Y - 10)
                    {
                        score += 50;
                        grid.Children.Remove(item);
                        superFood.Remove(item);
                        if (timing.IsAlive)
                            timing.Abort();
                        timing = new Thread(() => { invicible = true; Thread.Sleep(10000); invicible = false; }); timing.Start();
                        break;
                    }
                //Kolize s duchem
                if (position.X == monster.Margin.Left && position.Y - 10 == monster.Margin.Top)
                {
                    if (invicible)
                    {
                        //Respawn ducha
                        monster.Margin = new Thickness(startMonster.X, startMonster.Y, 0, 0);
                        score += 150;
                    }
                    else
                    {
                        life -= 1;
                        //konec hry
                        if (life == 0)
                            MessageBox.Show($"Duch vyhrál! Blahopřejeme. Skóre pacmana: {score}");
                        else
                        {
                            //Respawn
                            player.Margin = new Thickness(start.X, start.Y, 0, 0);
                        }
                    }
                }
                return;
            }
            if (e.Key == Key.Down)
            {
                //kontrola zdí
                foreach (var item in walls)
                    if (item.X == position.X && item.Y == position.Y + 10)
                        return;
                //posunutí hráče
                player.Margin = new Thickness(position.X, position.Y + 10, 0, 0);
                //kontrola žrádla
                foreach (var item in food)
                    if (item.Margin.Left == position.X && item.Margin.Top == position.Y + 10)
                    {
                        score += 10;
                        grid.Children.Remove(item);
                        food.Remove(item);
                        break;
                    }
                //kontrola superžřádla + časvoač na nesmrtelnost
                foreach (var item in superFood)
                    if (item.Margin.Left == position.X && item.Margin.Top == position.Y + 10)
                    {
                        score += 50;
                        grid.Children.Remove(item);
                        superFood.Remove(item);
                        if (timing.IsAlive)
                            timing.Abort();
                        timing = new Thread(() => { invicible = true; Thread.Sleep(10000); invicible = false; }); timing.Start();
                        break;
                    }
                //Kontrola kolize s duchem
                if (position.X == monster.Margin.Left && position.Y + 10 == monster.Margin.Top)
                {
                    if (invicible)
                    {
                        //Smrt ducha a respawn
                        monster.Margin = new Thickness(startMonster.X, startMonster.Y, 0, 0);
                        score += 150;
                    }
                    else
                    {
                        life -= 1;
                        //smrt hráče
                        if (life == 0)
                            MessageBox.Show($"Duch vyhrál! Blahopřejeme. Skóre pacmana: {score}");
                        else
                        {
                            //Respawn hráče
                            player.Margin = new Thickness(start.X, start.Y, 0, 0);
                        }
                    }
                }
                return;
            }
            if (e.Key == Key.Left)
            {
                //Kontrola zdí
                foreach (var item in walls)
                    if (item.X == position.X - 10 && item.Y == position.Y)
                        return;
                //Posunutí hráče
                player.Margin = new Thickness(position.X - 10, position.Y, 0, 0);
                //Kontrola žrádla
                foreach (var item in food)
                    if (item.Margin.Left == position.X - 10 && item.Margin.Top == position.Y)
                    {
                        score += 10;
                        grid.Children.Remove(item);
                        food.Remove(item);
                        break;
                    }
                //Kontrola superžrádla + časovač na nesmrtelnost
                foreach (var item in superFood)
                    if (item.Margin.Left == position.X - 10 && item.Margin.Top == position.Y)
                    {
                        score += 50;
                        grid.Children.Remove(item);
                        superFood.Remove(item);
                        if (timing.IsAlive)
                            timing.Abort();
                        timing = new Thread(() => { invicible = true; Thread.Sleep(10000); invicible = false; }); timing.Start();
                        break;
                    }
                //Kolize s duchem
                if (position.X - 10 == monster.Margin.Left && position.Y == monster.Margin.Top)
                {
                    if (invicible)
                    {
                        //Respawn ducha
                        monster.Margin = new Thickness(startMonster.X, startMonster.Y, 0, 0);
                        score += 150;
                    }
                    else
                    {      
                        life -= 1;
                        //Smrt hráče
                        if (life == 0)
                            MessageBox.Show($"Duch vyhrál! Blahopřejeme. Skóre pacmana: {score}");
                        else
                        {
                            //Respawn hráče
                            player.Margin = new Thickness(start.X, start.Y, 0, 0);
                        }
                    }
                }
                return;
            }
            if (e.Key == Key.Right)
            {
                //Kontrola zdí
                foreach (var item in walls)
                    if (item.X == position.X + 10 && item.Y == position.Y)
                        return;
                //Posunutí hráče
                player.Margin = new Thickness(position.X + 10, position.Y, 0, 0);
                //Kontrola žrádla
                foreach (var item in food)
                    if (item.Margin.Left == position.X + 10 && item.Margin.Top == position.Y)
                    {
                        score += 10;
                        grid.Children.Remove(item);
                        food.Remove(item);
                        break;
                    }
                //Kontrola superžádla + časovač na nesmrtelnost
                foreach (var item in superFood)
                    if (item.Margin.Left == position.X + 10 && item.Margin.Top == position.Y)
                    {
                        score += 50;
                        grid.Children.Remove(item);
                        superFood.Remove(item);
                        if (timing.IsAlive)
                            timing.Abort();
                        timing = new Thread(() => { invicible = true; Thread.Sleep(10000); invicible = false; }); timing.Start();
                        break;
                    }
                //Kontrola kolize s duchem
                if (position.X + 10 == monster.Margin.Left && position.Y == monster.Margin.Top)
                {
                    if (invicible)
                    {
                        //Smrt ducha
                        monster.Margin = new Thickness(startMonster.X, startMonster.Y, 0, 0);
                        score += 150;
                    }
                    else
                    {
                        life -= 1;
                        //Smrt hráče
                        if (life == 0)
                            MessageBox.Show($"Duch vyhrál! Blahopřejeme. Skóre pacmana: {score}");
                        else
                        {
                            //Respawn hráče
                            player.Margin = new Thickness(start.X, start.Y, 0, 0);
                        }
                    }
                }
                return;
            }
            position = new Point(monster.Margin.Left, monster.Margin.Top);
            if (e.Key == Key.W)
            {
                //Kontrola zdí
                foreach (var item in walls)
                    if (item.X == position.X && item.Y == position.Y - 10)
                        return;
                //Posunutí ducha
                monster.Margin = new Thickness(position.X, position.Y - 10, 0, 0);
                //Kontrola kolize s hráčem
                if (position.X == player.Margin.Left && position.Y - 10 == player.Margin.Top)
                {
                    if (invicible)
                    {
                        //Smrt ducha
                        monster.Margin = new Thickness(startMonster.X, startMonster.Y, 0, 0);
                        score += 150;
                    }
                    else
                    {
                        life -= 1;
                        //smrt hráče
                        if (life == 0)
                            MessageBox.Show($"Duch vyhrál! Blahopřejeme. Skóre pacmana: {score}");
                        else
                        {
                            //Respawn hráče
                            player.Margin = new Thickness(start.X, start.Y, 0, 0);
                        }
                    }
                }
                return;
            }
            if (e.Key == Key.S)
            {
                //Kontrola zdí
                foreach (var item in walls)
                    if (item.X == position.X && item.Y == position.Y + 10)
                        return;
                //Posunutí ducha
                monster.Margin = new Thickness(position.X, position.Y + 10, 0, 0);
                //Kontrola kolize s hráčem
                if (position.X == player.Margin.Left && position.Y + 10 == player.Margin.Top)
                {
                    if (invicible)
                    {
                        //Respawn ducha
                        monster.Margin = new Thickness(startMonster.X, startMonster.Y, 0, 0);
                        score += 150;
                    }
                    else
                    {
                        life -= 1;
                        //Smrt hráče
                        if (life == 0)
                            MessageBox.Show($"Duch vyhrál! Blahopřejeme. Skóre pacmana: {score}");
                        else
                        {
                            //Respawn hráče
                            player.Margin = new Thickness(start.X, start.Y, 0, 0);
                        }
                    }
                }
                return;
            }
            if (e.Key == Key.A)
            {
                //Kontrola zdí
                foreach (var item in walls)
                    if (item.X == position.X - 10 && item.Y == position.Y)
                        return;
                //Posunutí monstra
                monster.Margin = new Thickness(position.X - 10, position.Y, 0, 0);
                //Kontrola kolize s hráčem
                if (position.X - 10 == player.Margin.Left && position.Y == player.Margin.Top)
                {
                    if (invicible)
                    {
                        //Respawn ducha
                        monster.Margin = new Thickness(startMonster.X, startMonster.Y, 0, 0);
                        score += 150;
                    }
                    else
                    {
                        life -= 1;
                        //Smrt hráče
                        if (life == 0)
                            MessageBox.Show($"Duch vyhrál! Blahopřejeme. Skóre pacmana: {score}");
                        else
                        {
                            //Respawn Hráče
                            player.Margin = new Thickness(start.X, start.Y, 0, 0);
                        }
                    }
                }
                return;
            }
            if (e.Key == Key.D)
            {
                //Kontrola kolize se zdí
                foreach (var item in walls)
                    if (item.X == position.X + 10 && item.Y == position.Y)
                        return;
                //Posunutí ducha
                monster.Margin = new Thickness(position.X + 10, position.Y, 0, 0);
                //Kontrola kolize s hráčem
                if (position.X + 10 == player.Margin.Left && position.Y == player.Margin.Top)
                {
                    if (invicible)
                    {
                        //Respawn ducha
                        monster.Margin = new Thickness(startMonster.X, startMonster.Y, 0, 0);
                        score += 150;
                    }
                    else
                    {
                        life -= 1;
                        //smrt hráče
                        if (life == 0)
                            MessageBox.Show($"Duch vyhrál! Blahopřejeme. Skóre pacmana: {score}");
                        else
                        {
                            //respawn hráče
                            player.Margin = new Thickness(start.X, start.Y, 0, 0);
                        }
                    }
                }
                return;
            }
        }
    }
}
