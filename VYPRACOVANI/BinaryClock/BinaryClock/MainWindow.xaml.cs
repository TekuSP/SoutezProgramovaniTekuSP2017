using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BinaryClock
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //FOREACH na projetí všech prvků v gridu
            foreach (var subgrid in grid.Children)
            {
                //Je to grid?
                if (!(subgrid is Grid))
                    continue;
                //Pokud ano, tak mu přidáme čtverečky
                AddRectangles(subgrid as Grid);
                // Pak je nabarvíme na bílo
                foreach (var item in (subgrid as Grid).Children)
                    (item as Rectangle).Fill = Brushes.White;
            }
            //Založíme dvě vlákna které renderuje sekundové ručičky + mění čas dole v textu
            new Thread(() =>{ while (true){ firstRow.Dispatcher.Invoke(() =>{ RenderNumber(int.Parse(DateTime.Now.Second.ToString().Length == 1 ? "0" : DateTime.Now.Second.ToString()[1].ToString()), eightRow);}); Thread.Sleep(1000);}}).Start();
            new Thread(() => { while (true) { firstRow.Dispatcher.Invoke(() => { RenderNumber(int.Parse(DateTime.Now.Second.ToString()[0].ToString()), seventhRow); time.Text = DateTime.Now.ToLongTimeString(); }); Thread.Sleep(1000); } }).Start();
            //Založíme dvě vlákna které renderují minutové ručičky
            new Thread(() => { while (true) { firstRow.Dispatcher.Invoke(() => { RenderNumber(int.Parse(DateTime.Now.Minute.ToString().Length == 1 ? "0" : DateTime.Now.Minute.ToString()[1].ToString()), fifthRow); }); Thread.Sleep(1000); } }).Start();
            new Thread(() => { while (true) { firstRow.Dispatcher.Invoke(() => { RenderNumber(int.Parse(DateTime.Now.Minute.ToString()[0].ToString()), fourthRow); }); Thread.Sleep(1000); } }).Start();
            //Založíme dvě vlákna které renderují hodinové ručičky
            new Thread(() => { while (true) { firstRow.Dispatcher.Invoke(() => { RenderNumber(int.Parse(DateTime.Now.Hour.ToString().Length == 1 ? "0" : DateTime.Now.Hour.ToString()[1].ToString()), secondRow); }); Thread.Sleep(1000); } }).Start();
            new Thread(() => { while (true) { firstRow.Dispatcher.Invoke(() => { RenderNumber(int.Parse(DateTime.Now.Hour.ToString()[0].ToString()), firstRow); }); Thread.Sleep(1000); } }).Start();
        }
        //Vyrkreslovaní čísel podle řádku a čísla
        void RenderNumber(int number, Grid row)
        {
            switch (number)
            {
                case 0:
                    foreach (Rectangle item in row.Children)
                        item.Fill = Brushes.White;
                    break;
                case 1:
                    for (int i = 0; i < row.Children.Count; i++)
                    {
                        if (i == 0)
                            (row.Children[i] as Rectangle).Fill = Brushes.Black;
                        else
                            (row.Children[i] as Rectangle).Fill = Brushes.White;
                    }
                    break;
                case 2:
                    for (int i = 0; i < row.Children.Count; i++)
                    {
                        if (i == 1)
                            (row.Children[i] as Rectangle).Fill = Brushes.Black;
                        else
                            (row.Children[i] as Rectangle).Fill = Brushes.White;
                    }
                    break;
                case 3:
                    for (int i = 0; i < row.Children.Count; i++)
                    {
                        if (i == 0 || i == 1)
                            (row.Children[i] as Rectangle).Fill = Brushes.Black;
                        else
                            (row.Children[i] as Rectangle).Fill = Brushes.White;
                    }
                    break;
                case 4:
                    for (int i = 0; i < row.Children.Count; i++)
                    {
                        if (i == 2)
                            (row.Children[i] as Rectangle).Fill = Brushes.Black;
                        else
                            (row.Children[i] as Rectangle).Fill = Brushes.White;
                    }
                    break;
                case 5:
                    for (int i = 0; i < row.Children.Count; i++)
                    {
                        if (i == 2 || i == 0)
                            (row.Children[i] as Rectangle).Fill = Brushes.Black;
                        else
                            (row.Children[i] as Rectangle).Fill = Brushes.White;
                    }
                    break;
                case 6:
                    for (int i = 0; i < row.Children.Count; i++)
                    {
                        if (i == 2 || i == 1)
                            (row.Children[i] as Rectangle).Fill = Brushes.Black;
                        else
                            (row.Children[i] as Rectangle).Fill = Brushes.White;
                    }
                    break;
                case 7:
                    for (int i = 0; i < row.Children.Count; i++)
                    {
                        if (i == 2 || i == 1 || i == 0)
                            (row.Children[i] as Rectangle).Fill = Brushes.Black;
                        else
                            (row.Children[i] as Rectangle).Fill = Brushes.White;
                    }
                    break;
                case 8:
                    for (int i = 0; i < row.Children.Count; i++)
                    {
                        if (i == 3)
                            (row.Children[i] as Rectangle).Fill = Brushes.Black;
                        else
                            (row.Children[i] as Rectangle).Fill = Brushes.White;
                    }
                    break;
                case 9:
                    for (int i = 0; i < row.Children.Count; i++)
                    {
                        if (i == 3 || i == 0)
                            (row.Children[i] as Rectangle).Fill = Brushes.Black;
                        else
                            (row.Children[i] as Rectangle).Fill = Brushes.White;
                    }
                    break;
            }
        }
        //Přidaní čtverců
        void AddRectangles(Grid gr)
        {
            //Odsazení počáteční
            int top = 10;
            //4 čtverce
            for (int i = 0; i < 4; i++)
            {
                //čtverec
                Rectangle rc = new Rectangle()
                {
                    Margin = new Thickness(0, top, 0, 0),
                    VerticalAlignment = VerticalAlignment.Top,
                    Width = 50,
                    Height = 50
                };
                //Přidáme čtverec do gridu
                gr.Children.Add(rc);
                //Odsadíme
                top += 60;
            }
        }
    }
}
