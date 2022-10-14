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
        public Label[,] datenStrom  = new Label[5,33];              // Array für die Anzeige der LABELS in dem STRO-TAB
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
            balken_lbl[1] = balken1;
            balken_lbl[2] = balken2;
            balken_lbl[3] = balken3;
            balken_lbl[4] = balken4;    
            balken_lbl[5] = balken5;    
            balken_lbl[6] = balken6;
            balken_lbl[7] = balken7;
            balken_lbl[8] = balken8;
            balken_lbl[9] = balken9;
            balken_lbl[10] = balken10;
            balken_lbl[11] = balken11;
            balken_lbl[12] = balken12;
            balken_lbl[13] = balken13;
            balken_lbl[14] = balken14;
            balken_lbl[15] = balken15;
            balken_lbl[16] = balken16;
            balken_lbl[17] = balken17;
            balken_lbl[18] = balken18;
            balken_lbl[19] = balken19;
            balken_lbl[20] = balken20;
            balken_lbl[21] = balken21;
            balken_lbl[22] = balken22;
            balken_lbl[23] = balken23;
            balken_lbl[24] = balken24;
            balken_lbl[25] = balken25;
            balken_lbl[26] = balken26;
            balken_lbl[27] = balken27;
            balken_lbl[28] = balken28;
            balken_lbl[29] = balken29;
            balken_lbl[30] = balken30;
            balken_lbl[31] = balken31;
        }
        public void DatenStromInArray()
        {
            // Datum vom Eintrag
            datenStrom[0, 1] = r1c1;
            datenStrom[0, 2] = r2c1;
            datenStrom[0, 3] = r3c1;
            datenStrom[0, 4] = r4c1;
            datenStrom[0, 5] = r5c1;
            datenStrom[0, 6] = r6c1;
            datenStrom[0, 7] = r7c1;
            datenStrom[0, 8] = r8c1;
            datenStrom[0, 9] = r9c1;
            datenStrom[0, 10] = r10c1;

            datenStrom[0, 11] = r11c1;
            datenStrom[0, 12] = r12c1;
            datenStrom[0, 13] = r13c1;
            datenStrom[0, 14] = r14c1;
            datenStrom[0, 15] = r15c1;
            datenStrom[0, 16] = r16c1;
            datenStrom[0, 17] = r17c1;
            datenStrom[0, 18] = r18c1;
            datenStrom[0, 19] = r19c1;
            datenStrom[0, 20] = r20c1;

            datenStrom[0, 21] = r21c1;
            datenStrom[0, 22] = r22c1;
            datenStrom[0, 23] = r23c1;
            datenStrom[0, 24] = r24c1;
            datenStrom[0, 25] = r25c1;
            datenStrom[0, 26] = r26c1;
            datenStrom[0, 27] = r27c1;
            datenStrom[0, 28] = r28c1;
            datenStrom[0, 29] = r29c1;
            datenStrom[0, 30] = r30c1;

            datenStrom[0, 31] = r31c1;

            // Eintrag in KWH
            datenStrom[1, 1] = r1c2;
            datenStrom[1, 2] = r2c2;
            datenStrom[1, 3] = r3c2;
            datenStrom[1, 4] = r4c2;
            datenStrom[1, 5] = r5c2;
            datenStrom[1, 6] = r6c2;
            datenStrom[1, 7] = r7c2;
            datenStrom[1, 8] = r8c2;
            datenStrom[1, 9] = r9c2;
            datenStrom[1, 10] = r10c2;

            datenStrom[1, 11] = r11c2;
            datenStrom[1, 12] = r12c2;
            datenStrom[1, 13] = r13c2;
            datenStrom[1, 14] = r14c2;
            datenStrom[1, 15] = r15c2;
            datenStrom[1, 16] = r16c2;
            datenStrom[1, 17] = r17c2;
            datenStrom[1, 18] = r18c2;
            datenStrom[1, 19] = r19c2;
            datenStrom[1, 20] = r20c2;

            datenStrom[1, 21] = r21c2;
            datenStrom[1, 22] = r22c2;
            datenStrom[1, 23] = r23c2;
            datenStrom[1, 24] = r24c2;
            datenStrom[1, 25] = r25c2;
            datenStrom[1, 26] = r26c2;
            datenStrom[1, 27] = r27c2;
            datenStrom[1, 28] = r28c2;
            datenStrom[1, 29] = r29c2;
            datenStrom[1, 30] = r30c2;

            datenStrom[1, 31] = r31c2;

            // Differenz letzter Eintrag
            datenStrom[2, 1] = r1c3;
            datenStrom[2, 2] = r2c3;
            datenStrom[2, 3] = r3c3;
            datenStrom[2, 4] = r4c3;
            datenStrom[2, 5] = r5c3;
            datenStrom[2, 6] = r6c3;
            datenStrom[2, 7] = r7c3;
            datenStrom[2, 8] = r8c3;
            datenStrom[2, 9] = r9c3;
            datenStrom[2, 10] = r10c3;

            datenStrom[2, 11] = r11c3;
            datenStrom[2, 12] = r12c3;
            datenStrom[2, 13] = r13c3;
            datenStrom[2, 14] = r14c3;
            datenStrom[2, 15] = r15c3;
            datenStrom[2, 16] = r16c3;
            datenStrom[2, 17] = r17c3;
            datenStrom[2, 18] = r18c3;
            datenStrom[2, 19] = r19c3;
            datenStrom[2, 20] = r20c3;

            datenStrom[2, 21] = r21c3;
            datenStrom[2, 22] = r22c3;
            datenStrom[2, 23] = r23c3;
            datenStrom[2, 24] = r24c3;
            datenStrom[2, 25] = r25c3;
            datenStrom[2, 26] = r26c3;
            datenStrom[2, 27] = r27c3;
            datenStrom[2, 28] = r28c3;
            datenStrom[2, 29] = r29c3;
            datenStrom[2, 30] = r30c3;

            datenStrom[2, 31] = r31c3;


            // Durchschnitt zum letzten Eintrag
            datenStrom[3, 1] = r1c4;
            datenStrom[3, 2] = r2c4;
            datenStrom[3, 3] = r3c4;
            datenStrom[3, 4] = r4c4;
            datenStrom[3, 5] = r5c4;
            datenStrom[3, 6] = r6c4;
            datenStrom[3, 7] = r7c4;
            datenStrom[3, 8] = r8c4;
            datenStrom[3, 9] = r9c4;
            datenStrom[3, 10] = r10c4;

            datenStrom[3, 11] = r11c4;
            datenStrom[3, 12] = r12c4;
            datenStrom[3, 13] = r13c4;
            datenStrom[3, 14] = r14c4;
            datenStrom[3, 15] = r15c4;
            datenStrom[3, 16] = r16c4;
            datenStrom[3, 17] = r17c4;
            datenStrom[3, 18] = r18c4;
            datenStrom[3, 19] = r19c4;
            datenStrom[3, 20] = r20c4;

            datenStrom[3, 21] = r21c4;
            datenStrom[3, 22] = r22c4;
            datenStrom[3, 23] = r23c4;
            datenStrom[3, 24] = r24c4;
            datenStrom[3, 25] = r25c4;
            datenStrom[3, 26] = r26c4;
            datenStrom[3, 27] = r27c4;
            datenStrom[3, 28] = r28c4;
            datenStrom[3, 29] = r29c4;
            datenStrom[3, 30] = r30c4;

            datenStrom[3, 31] = r31c4;


            // Durchschnitt Monat
            datenStrom[4, 1] = r1c5;
            datenStrom[4, 2] = r2c5;
            datenStrom[4, 3] = r3c5;
            datenStrom[4, 4] = r4c5;
            datenStrom[4, 5] = r5c5;
            datenStrom[4, 6] = r6c5;
            datenStrom[4, 7] = r7c5;
            datenStrom[4, 8] = r8c5;
            datenStrom[4, 9] = r9c5;
            datenStrom[4, 10] = r10c5;

            datenStrom[4, 11] = r11c5;
            datenStrom[4, 12] = r12c5;
            datenStrom[4, 13] = r13c5;
            datenStrom[4, 14] = r14c5;
            datenStrom[4, 15] = r15c5;
            datenStrom[4, 16] = r16c5;
            datenStrom[4, 17] = r17c5;
            datenStrom[4, 18] = r18c5;
            datenStrom[4, 19] = r19c5;
            datenStrom[4, 20] = r20c5;

            datenStrom[4, 21] = r21c5;
            datenStrom[4, 22] = r22c5;
            datenStrom[4, 23] = r23c5;
            datenStrom[4, 24] = r24c5;
            datenStrom[4, 25] = r25c5;
            datenStrom[4, 26] = r26c5;
            datenStrom[4, 27] = r27c5;
            datenStrom[4, 28] = r28c5;
            datenStrom[4, 29] = r29c5;
            datenStrom[4, 30] = r30c5;

            datenStrom[4, 31] = r31c5;


            

        }

    }


}
