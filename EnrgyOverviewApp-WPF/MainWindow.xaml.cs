﻿using System;
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
        public Label[,] datenStrom = new Label[5,33];
        public static string[,] datenS = new string[5, 33];      // Fünf Einträge, 31 Tage im Monat + Eintrag letzer Abschluss + gesamt Monat
        public static int       heute;
        public static string    systemzeit, datum, tag, monat, jahr; // Systemzeit in einzelne Teile zerlegen...
        public static string    fileName;
        public static string[] monate = new string[500];

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
            Files.LoadMonth();
            DatenEintragen();


            AktuellesDatum.Content = Convert.ToString(heute) + "." + monat + "."+ jahr;
            fakeTagHeute.Content = heute;

            TextAktivieren();
        }

        // wenn in er Textbox Enter gedrückt wurde, mache...
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // ist die Taste  = RETURN?
            if (e.Key == Key.Return)
            {
                NeuenWertEintragen();
                DatenEintragen(); // Test..
                Files.SaveMonth();
            }
            
           
        }

        public void NeuenWertEintragen()
        {
            // datenStrom[0, heute].Content = datum;                       // aktuelles Datum
            datenStrom[0, heute].Content = Convert.ToString(heute) + "." + monat + "." + jahr; // Test um an beliebigen Datum eintragen zu können...
            datenS[0, heute] = Convert.ToString(heute) + "." + monat + "." + jahr; // Test um an beliebigen Datum eintragen zu können...]
            datenStrom[1, heute].Content = txt_Box_Zaehlerstand.Text;   // eingetragener Zählerstand

            //datenS[0,heute] = datum;  deaktiviert für Test!
            datenS[1,heute] = txt_Box_Zaehlerstand.Text;

            txt_Box_Zaehlerstand.Text = "Zählerstand?";
            TextAktivieren();


        }

        private void TagMinus(object sender, RoutedEventArgs e)
        {
            heute--;
            if (heute <1) heute = 1;
            fakeTagHeute.Content = heute;
            tag = Convert.ToString(heute);
            AktuellesDatum.Content = Convert.ToString(heute) + "." + monat + "." + jahr;
            TextAktivieren();
        }
        private void TagPlus(object sender, RoutedEventArgs e)
        {
            heute++;
            if(heute > 31) heute = 31;
            fakeTagHeute.Content = heute;
            tag = Convert.ToString(heute);
            AktuellesDatum.Content = Convert.ToString(heute) + "." + monat + "." + jahr;
            TextAktivieren();
        }

        public void DatenEintragen()
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 1; j < 32; j++)
                {
                    datenStrom[i, j].Content = datenS[i, j];

                }
            }
        }

        public void TextAktivieren()
        {
            //TextBox direkt aktiviern und den Text selektieren
            txt_Box_Zaehlerstand.Focus();
            txt_Box_Zaehlerstand.SelectAll();
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
            datenStrom[4, 1] = r1c4;
            datenStrom[4, 2] = r2c4;
            datenStrom[4, 3] = r3c4;
            datenStrom[4, 4] = r4c4;
            datenStrom[4, 5] = r5c4;
            datenStrom[4, 6] = r6c4;
            datenStrom[4, 7] = r7c4;
            datenStrom[4, 8] = r8c4;
            datenStrom[4, 9] = r9c4;
            datenStrom[4, 10] = r10c4;

            datenStrom[4, 11] = r11c4;
            datenStrom[4, 12] = r12c4;
            datenStrom[4, 13] = r13c4;
            datenStrom[4, 14] = r14c4;
            datenStrom[4, 15] = r15c4;
            datenStrom[4, 16] = r16c4;
            datenStrom[4, 17] = r17c4;
            datenStrom[4, 18] = r18c4;
            datenStrom[4, 19] = r19c4;
            datenStrom[4, 20] = r20c4;

            datenStrom[4, 21] = r21c4;
            datenStrom[4, 22] = r22c4;
            datenStrom[4, 23] = r23c4;
            datenStrom[4, 24] = r24c4;
            datenStrom[4, 25] = r25c4;
            datenStrom[4, 26] = r26c4;
            datenStrom[4, 27] = r27c4;
            datenStrom[4, 28] = r28c4;
            datenStrom[4, 29] = r29c4;
            datenStrom[4, 30] = r30c4;

            datenStrom[4, 31] = r31c4;

        }

        private void Monat_Minus(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Monat zurück!");
        }

        private void Monat_Plus(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Monat vor!");
        }
    }


}