using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace EnrgyOverviewApp_WPF
{
    internal class Files
    {

        public static void SaveLetzterEintrag()
        {
            StreamWriter sw1 = new StreamWriter("Letzter Eintrag.txt");

            for (int i = 0; i < 5; i++)
            {
               sw1.WriteLine(MainWindow.datenS[i,32]);
            }
            sw1.Close();
        }
        public static void LoadLetzterEintrag()
        {
            if (File.Exists("Letzter Eintrag.txt") == true)
            {
                MainWindow.ersterStart = false;     // Das Progamm hat schon Werte in 'Letzter Eintrag.txt' geschrieben...
                StreamReader sr1 = new StreamReader("Letzter Eintrag.txt");

                for (int i = 0; i < 5; i++)
                {
                    MainWindow.datenS[i, 32] = sr1.ReadLine();
                }
                sr1.Close();
            }
        }

        public static void LoadMonth()
        {
            
            if (File.Exists(MainWindow.fileName) == true)
            {
                //MessageBox.Show("Dataei laden!");
                
                StreamReader sr2 = new StreamReader(MainWindow.fileName);

                for (int j = 0; j < 33; j++)
                {
                    for  (int i = 0; i < 5; i++)
                    {
                        MainWindow.datenS[i, j] = sr2.ReadLine();
                        
                    }

                }
                
                sr2.Close();
            }
            else NewMonthFile();

        }

        public static void SaveMonth()
        {
            StreamWriter sw3 = new StreamWriter(MainWindow.fileName);

            for (int j = 0; j < 33; j++)
            {
                    for (int i = 0; i < 5; i++)
                    {
                    sw3.WriteLine(MainWindow.datenS[i, j]);
                    }

                }

                sw3.Close();
        }
        public static void NewMonthFile()   // Neun Monat beginnen und eine Datei anlegen
        {
            StreamWriter sw2 = new StreamWriter(MainWindow.fileName);
            //MessageBox.Show("Neues File wird gespeichert!");
            for (int j = 0; j < 33; j++)
            {
                for  (int i = 0; i < 5; i++)
                {
                    // sw2.WriteLine(MainWindow.datenS[i, j]);
                    sw2.WriteLine("");
                }

            }

            sw2.Close();

            LoadMonth();
            LoadLetzterEintrag();
        }
        public static void LoadMonthList()
        {
            NewMonthList();

            //monate
            if (File.Exists("ML.txt"))
            {
                int i = 0;
               
                StreamReader sr1 = new StreamReader("ML.txt");

                while (true)
                {
                    MainWindow.monate[i] = sr1.ReadLine();
                    if (MainWindow.monate[i] == "END" || i==499) break;
                    i++;
                  
                }

                    sr1.Close();
            }


        }

        public static void NewMonthList()
        {
            string config = "ML.txt";

            if (File.Exists(config) == false)
            {

                StreamWriter sw1 = new StreamWriter(config);
                sw1.WriteLine(MainWindow.fileName);
                sw1.WriteLine("END");
                             
                sw1.Close();
            }

        }
    }
}
