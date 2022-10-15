using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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



namespace EnrgyOverviewApp_WPF
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Label[,] datenStrom  = new Label[5,33];              // Array für die Anzeige der LABELS in dem STROM-TAB
        public Label[] balken_lbl   = new Label[33];                // Array für die Balkenanzeige
        public static string[,] datenS = new string[5, 33];         // Fünf Einträge, 31 Tage im Monat + Eintrag letzer Abschluss + gesamt Monat
        public static int       heute;
        public static string    systemzeit, datum, tag, monat, jahr;// Systemzeit in einzelne Teile zerlegen...
        public static string    fileName;
        public static string[] monate = new string[500];            // 500 Monate sind eirwas übertrieben ;)
        public static bool ersterStart = true;                      // Wird das Programm zum ersten mal gestartet? ...
        public static int merkerAktuellerWert, durchschnitt,wert1,wert2,ergebnis;
        public static int balkenHoehe , bh2, kwMulti=8;
        

        public MainWindow()
        {
            InitializeComponent();
            DatenStromInArray();

            // Systemzeit in String umwandeln
            systemzeit = System.DateTime.Now.ToString();
            for (int i = 0; i <= 10; i++)
            {
                datum += systemzeit[i];
            }

            for (int i = 6; i <= 9; i++)
            {
                jahr += systemzeit[i];
            }

            tag += systemzeit[0];
            tag += systemzeit[1];

            monat += systemzeit[3];
            monat += systemzeit[4];

            heute = Convert.ToInt32(tag);


            fileName = monat + "-" + jahr + ".txt";     //Name der Monatsberichtsdatei
            Files.LoadMonthList();                      // Liste der vorhandenen Monate laden
            Files.LoadLetzterEintrag();                 // Letzen Eintrag laden - wenn neuer Monat beginnt, bekomme ich so die Abschlsswerte vom Vormonat
            Files.LoadMonth();
            DatenEintragen();


            AktuellesDatum.Content = Convert.ToString(heute) + "." + monat + "." + jahr;
            fakeTagHeute.Content = heute;   // dient zum debuggen
                       
            TextAktivieren();
            Autoscroll();
            FarbeSetzten();
            BalkenZuweisen();
            BalkenBerechnen();
            lbl_KW.Content = Convert.ToString(kwMulti);  // 26-kwMulti



        }


        //********************************************************************************
        // wenn in er Textbox Tasten gedrückt wurden...
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // ist die Taste  = RETURN?
            if (e.Key == Key.Return)
            {
                if (txt_Box_Zaehlerstand.Text != "")
                {
                    NeuenWertEintragen();
                    DatenEintragen(); 
                    BalkenBerechnen();
                    Files.SaveMonth();
                } else MessageBox.Show("Leere Eingabe ist nicht möglich!");
            }
            EingabePruefen();
        }

        //TextBox_KeyUp
        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left) TextAktivieren();     // so bleibt der Cursor am Ende der Eingabe...
            EingabePruefen();
            e.Handled = true;
        } 

        public void EingabePruefen()
        {
            string testString = txt_Box_Zaehlerstand.Text;
            for (int i = 0; i < testString.Length; i++)
            {
                int test1 = (int)testString[i];
                if (test1 < 48 || test1 > 57)                                       // Nur Zahlen sind erlaubt!
                {
                    testString = testString.Substring(0, testString.Length - 1);    // Bei Falscheingabe wird das Zeichen dirkt wieder gelöscht...
                    txt_Box_Zaehlerstand.Text = testString;
                    TextAktivieren();
                }
            }
        }
        // ************************************************************************

        public void FarbeSetzten()    // Die aktuelle Zeile in DarkSlateGray färben - Zeile davor und danach wieder in Grau einfärben
        {
            if (heute > 1)
            {
                for (int i = 0; i < 5; i++)  datenStrom[i, heute - 1].Background = new SolidColorBrush(Colors.Gray);
            }
            if (heute < 31)
            {
                for (int i = 0; i < 5; i++) datenStrom[i, heute + 1].Background = new SolidColorBrush(Colors.Gray);
            }
            for (int i = 0; i < 5; i++) datenStrom[i, heute].Background = new SolidColorBrush(Colors.DarkSlateGray);
            
        }
        public void Autoscroll()    // Den aktellen Tag in die Mitte scollen
        {
            scroll.ScrollToVerticalOffset(heute * 50 - 290);
        }
        public void NeuenWertEintragen()
        {
            // datenStrom[0, heute].Content = datum;                                            // aktuelles Datum
            datenStrom[0, heute].Content = Convert.ToString(heute) + "." + monat + "." + jahr;  // Test um an beliebigen Datum eintragen zu können...
            datenS[0, heute] = Convert.ToString(heute) + "." + monat + "." + jahr;              // Test um an beliebigen Datum eintragen zu können...]
            datenStrom[1, heute].Content = txt_Box_Zaehlerstand.Text;                           // eingetragener Zählerstand

            //datenS[0,heute] = datum;  deaktiviert für Test!
            datenS[1,heute] = txt_Box_Zaehlerstand.Text;
            // merkerAktuellerWert = Convert.ToInt32(txt_Box_Zaehlerstand.Text);                // Aktueller Wert für die Berechnung von den Durchschnittswerten
            if (ersterStart == false) WerteBerechnen();

            //Als letzten Eintrag in das Array füllen
            datenS[0,32] = Convert.ToString(heute) + "." + monat + "." + jahr;
            datenS[1,32] = txt_Box_Zaehlerstand.Text;
            // die weiteren Felder werden in der Methode 'WerteBerechnen' eingetragen
            Files.SaveLetzterEintrag();
            if (ersterStart == true)
            {
                datenS[0, 0] = datenS[0, 32];
                datenS[1, 0] = datenS[1, 32];
                datenS[2, 0] = datenS[2, 32];
                datenS[3, 0] = datenS[3, 32];
                datenS[4, 0] = datenS[4, 32];
                ersterStart = false; 
            }

            txt_Box_Zaehlerstand.Text = "";
            TextAktivieren();
        }


        public void WerteBerechnen()
        {
            if (datenS[1, heute-1] != "")                      // Ausführen, wenn am Vortag ein Wert eingetragen ist
            {
                wert1 = Convert.ToInt32(datenS[1, heute]);     // Aktueller Eintrag
                wert2 = Convert.ToInt32(datenS[1, heute-1]);   // Eintrag vom Vortag
                ergebnis = wert1 - wert2;
                datenS[2, heute] = Convert.ToString(ergebnis); // Differenz zum Vortag eintragen
            }

            // Durchschnitt für den Monat berechnen und eintragen
            wert1 = 0;
            for (int i = 1; i <=heute; i++)
            {
                if (datenS[2, i] != "")
                {
                    wert2 = Convert.ToInt32(datenS[2, i]);
                    wert1 += wert2;
                }
            }
            ergebnis = wert1 / heute;
            for (int i = 1; i <= heute; i++)
            {
                datenS[4, i] = Convert.ToString(ergebnis);
            }


            // Differenz zum letzten Eintrag
            wert1 = 0;
            wert2 = 0;
            int zaehler = 0;
            int merker = 1;

            if (datenS[1, heute - 1] == "")
            {
                
                for(int i = heute-1; i > 0; i--)     // liegt der letzte Eintrag schon ein paar Tage zurück?
                {
                    if (datenS[1, i] == "")
                    {
                        zaehler = i;
                        merker ++;
                    }
                    else break;
                }
                
                wert1 = Convert.ToInt32(datenS[1, heute]);           // Aktueller Eintrag
                wert2 = Convert.ToInt32(datenS[1, heute-merker]);    // letzter Eintrag

                ergebnis = (wert1 - wert2) / (merker);
                
                for (int i = zaehler; i <= heute; i++)
                {
                    datenS[3, i] = Convert.ToString(ergebnis);
                }
                datenS[2, heute] = Convert.ToString(wert1-wert2);
            }
            else datenS[3, heute] = datenS[2, heute];


            // Letzten Eintrag in das Array schreiben
            datenS[2, 32] = datenS[2, heute];
            datenS[3, 32] = datenS[3, heute];
            datenS[4, 32] = datenS[4, heute];

        }

      
        // ACHTUNG - AB HIER FOLGT EIN TEST!!! 
        private void TagMinus(object sender, RoutedEventArgs e)
        {
            heute--;
            if (heute < 1) heute = 1;
            TagSetzen();
            
        }
        private void TagPlus(object sender, RoutedEventArgs e)
        {
            heute++;
            if(heute > 31) heute = 31;
            TagSetzen();

        }

        public void TagSetzen()
        {
            
            fakeTagHeute.Content = heute;
            tag = Convert.ToString(heute);
            AktuellesDatum.Content = Convert.ToString(heute) + "." + monat + "." + jahr;
            TextAktivieren();
            Autoscroll();
            FarbeSetzten();
        }
        // TEST BEENDET !


        public void DatenEintragen()            // Den Labeln in der GUI einen neuen Content zuweisen
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 1; j < 32; j++)
                {
                    datenStrom[i, j].Content = datenS[i, j];
                }
            }
        }

        public void TextAktivieren()            //TextBox direkt aktiviern und den Text selektieren
        {
            txt_Box_Zaehlerstand.Focus();
            txt_Box_Zaehlerstand.Select(txt_Box_Zaehlerstand.Text.Length, 0);
        }

        public void BalkenBerechnen()
        {
            for (int i = 1; i < 32; i++)
            {
                if (datenS[3, i] != "")
                {
                    balken_lbl[i].Height = Convert.ToInt32(datenS[3, i]) * 20;
                    balken_lbl[i].Content = datenS[3, i];
                }                
            }
            
            //  Farbverlauf kwMulti 1-20     
            for (int i = 1; i < 32; i++)
            {
                if (datenS[3, i] != "")
                {
                    double kwOkInListe = (100/(Convert.ToDouble(datenS[3, i])) / 100  * (kwMulti)); 
                    // if (i ==13) MessageBox.Show(Convert.ToString(kwOkInListe));   // Test - Anzeigen vom Wert
                    if (kwOkInListe > 1) kwOkInListe = 1;
                    if (kwOkInListe < 0) kwOkInListe = 0;       // Der Wert liegt nun zischen 0 und 1 - und bei allen Nalken auf der gleichen höhe!

                    // Farbverlauf zuweisen
                    LinearGradientBrush vertikalG = new LinearGradientBrush
                    {
                        StartPoint = new Point(0,0),  
                        EndPoint   = new Point(0, 1- kwOkInListe)
                    };
                    vertikalG.GradientStops.Add(new GradientStop(Colors.Red  ,1-kwOkInListe));  
                    vertikalG.GradientStops.Add(new GradientStop(Colors.Green,1));                    
                    balken_lbl[i].Background = vertikalG;
                } else balken_lbl[i].Background = new SolidColorBrush(Colors.DarkGray);
            }
        }


        //    Monate vor und zurück per Cklick...    Code kommt noch!
        private void Monat_Minus(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Monat zurück!");
        }

        private void Monat_Plus(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Monat vor!");
        }

        // KW-Verbrauch am Tag ok-Wert
        private void KW_Minus(object sender, RoutedEventArgs e)
        {
            kwMulti--;
            if (kwMulti < 1) kwMulti = 1;
            lbl_KW.Content = Convert.ToString(kwMulti);
            Files.SaveLetzterEintrag();
            BalkenBerechnen();
        }

        private void KW_Plus(object sender, RoutedEventArgs e)
        {
            kwMulti++;
            if (kwMulti > 25) kwMulti = 25;
            lbl_KW.Content = Convert.ToString(kwMulti);
            Files.SaveLetzterEintrag();
            BalkenBerechnen();
        }


        //  Ab hier werden die einzellnen Labes in ein Label-Array geschrieben und können so einfacher behandelt werden...
        //****************************************************************************************************************
        public void BalkenZuweisen()
        {
            string zahl;
            for (int i = 1; i < 32; i++)
            {
                zahl = Convert.ToString(i);
                balken_lbl[i] = (Label)FindName($"balken{zahl}");
            }
            
        }
        public void DatenStromInArray()
        {
            string zahl1;
            string zahl2;
                       
            // Label in Array eintragen
            for (int j = 0; j < 5; j++)
            {
                for (int i = 1; i < 32; i++)
                {
                    zahl1 = Convert.ToString(i);
                    zahl2 = Convert.ToString(j+1);
                    datenStrom[j, i] = (Label)FindName($"r{zahl1}c{zahl2}");
                   
                }
            }
           
        }

    }


}
