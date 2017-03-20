using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using temTek.TemTekConsoleGraphics;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace KolemSveta
{
    class Program
    {
        /// <summary>
        /// Vstup programu
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Založím stringy pro začátek a konec cesty
            string from;
            //Založíme města
            City[] cities;
            //Vytvořit Menu pro výběr souboru či ruční zadaní
            if (ConsoleMenu.Menu(new string[] { "Vybrat soubor", "Zadat ručně" }, "Vítejte v programu \"Kolem Světa\"!") == 1)
            {
                //Udělat dialog na výběr souboru s nadpisem "Vyberte si soubor"
                OpenFileDialog fd = new OpenFileDialog()
                {
                    CheckFileExists = true,
                    CheckPathExists = true,
                    Multiselect = false,
                    Title = "Vyberte si soubor"
                };
                //Zobrazit dialog
                fd.ShowDialog();
                //Otevřu soubor
                using (StreamReader sr = new StreamReader(fd.FileName))
                {
                    //Chytám všechny chyby
                    try
                    {
                        //Založím List
                        List<City> cityTmp = new List<City>();
                        //Přečtu odkud jedeme
                        from = sr.ReadLine();
                        //Čtu dokud není konec souboru
                        while (!sr.EndOfStream)
                        {
                            bool found = false;
                            City actualFrom = new City();
                            City actualTo = new City();
                            //Rozeberu řádek který čtu podle mezer do pole
                            string[] temp = sr.ReadLine().Split(' ');
                            //Pokud to nejsou tři věci, tak to není náš soubor
                            if (temp.Length != 3)
                                throw new ArgumentException("Neplatná délka vstupu");
                            //Pokud to není malé nebo velké písmeno, tak to není náš vstup
                            if (!Regex.IsMatch(temp[0], "^[a-žA-Ž]+$"))
                                throw new ArgumentException("Neplatný vstup");
                            //Pokud to není malé nebo velké písmeno, tak to není náš vstup
                            if (!Regex.IsMatch(temp[1], "^[a-žA-Ž]+$"))
                                throw new ArgumentException("Neplatný vstup");
                            foreach (var item in cityTmp)
                                if (item.Name == temp[0])
                                {
                                    found = true;
                                    actualFrom = item;
                                    break;
                                }
                            if(!found)
                            {
                                actualFrom = new City() { Name = temp[0], pathsFrom = new List<Path>(), pathsTo = new List<Path>() };
                                cityTmp.Add(actualFrom);
                            }
                            foreach (var item in cityTmp)
                                if (item.Name == temp[1])
                                {
                                    found = true;
                                    actualTo = item;
                                    break;
                                }
                            if (!found)
                            {
                                actualTo = new City() { Name = temp[1], pathsFrom = new List<Path>(), pathsTo = new List<Path>() };
                                cityTmp.Add(actualTo);
                            }
                            found = false;
                            //Přidám do Listu novou cestu
                            Path tempPath = new Path() { From = actualFrom, To = actualTo, Price = int.Parse(temp[2]) };
                        }
                        //Zbavím se listu aby nežral tolik paměti a převedu to na pole
                        cities = cityTmp.ToArray();
                    }
                    //Tady zpracovavám chyby
                    catch (Exception ex)
                    {
                        //Výpis blábolů pro uživatele
                        Console.WriteLine("Došlo k chybě v programu");
                        Console.WriteLine("Je možné že byl zadán neplatný vstup či neplatný soubor!");
                        Console.WriteLine("Chyba pro programátory: ");
                        Console.WriteLine("------------------------");
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("------------------------");
                        Console.WriteLine("Omlouváme se.");
                        //Ukončení aplikace s chybou 10
                        Environment.Exit(10);
                    }
                }
            }
            else
            {
                while (true)
                {
                    Console.Write("Zadejte název města odkud vyjedeme: ");
                    from = Console.ReadLine();
                    if (!Regex.IsMatch(from, "^[a-žA-Ž]+$"))
                    {

                    }
                }
            }
        }
        /// <summary>
        /// Třída na cestu
        /// </summary>
        class Path
        {
            // Odkud
            public City From { get; set; }
            // Kam
            public City To { get; set; }
            // Za jakou cenu
            public int Price { get; set; }
        }
        /// <summary>
        /// Třída na Město
        /// </summary>
        class City
        {
            //Název města
            public string Name { get; set; }
            //Všechny cesty Z města
            public List<Path> pathsFrom;
            //Všechny cesty DO města
            public List<Path> pathsTo;
            public void AddPath(Path pathToAdd)
            {
                //foreach (var item in pathsFrom)
                //    if (item.From == pathToAdd.From && item.To == pathToAdd.To)
                //    {
                //        found = true;
                //        break;
                //    }
                //if (!found)
                //    actualFrom.pathsFrom.Add(tempPath);
            }
        }
    }
}
