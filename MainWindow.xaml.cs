using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WMPLib;
using System.Diagnostics;
using System.Windows.Media.Animation;


namespace BrickBreakerss
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //gestione Lewis
        Ellipse Lewis;
        DispatcherTimer timer_Lewis;
        double LewisX, LewisY;

        double pos_bottom, pos_left;
        int dirY, dirX;

        //gestione gioco
        Utemte utente;
        bool started;
        bool gameOver;
        int punti;
        double speedMercedes;
        List<Utemte> classifica_tot;

        //gestione mercedes
        Rectangle mercedes;

        //gestione Brick
        int nRedBullTot = 48;
        int nRedBullRiga = 8;
        List<Rectangle> elencoRedBull;

        //gestione bonus
        List<Ellipse> elencoBonus;
        DispatcherTimer timer_bonus;
        bool w12;
        int w12Timer;

        //List<Utemte> classifica_tot = new List<Utemte>();
        //List<int> listaInteri = provvisoria.Cast<int>().ToList();

        Random rnd = new Random(Guid.NewGuid().GetHashCode());//per fare random davvero random per usarla per bonus ecc....

        WindowsMediaPlayer GoverSound = null, Win = null, StartSound = null, RankTop = null, BonusSound=null;


            public MainWindow()
            {
                InitializeComponent();
                utente = new Utemte();
                bt_start.Background=new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/BrickBreakerss;component/Resources/scacchi.jpg", UriKind.RelativeOrAbsolute))
                };

                (classifica_tot) = FileManager.Leggi("classifica.bin");

                timer_Lewis = new DispatcherTimer();
                timer_Lewis.Interval = new TimeSpan(0, 0, 0, 0, 25);
                timer_Lewis.Tick += MuoviLewis;

                timer_bonus = new DispatcherTimer();
                timer_bonus.Interval = new TimeSpan(0, 0, 0, 0, 25);
                timer_bonus.Tick += MuoviBonus;

                started = false;

                elencoRedBull = new List<Rectangle>(nRedBullTot);
                elencoBonus = new List<Ellipse>(nRedBullTot);

                contenitore.Background = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/BrickBreakerss;component/Resources/sondoF1.png", UriKind.RelativeOrAbsolute))
                };

                nRedBullTot = 48;
                nRedBullRiga = 8;

                logo.Height = 0;
            }

		    private void viewIstruzioni(object sender, RoutedEventArgs e)
            {
			    MessageBox.Show("Per giocare devi inserire il nick, cliccare start e successivamente, una volta comparsa la macchina dovrai cliccare spazio, Lewis volerà e avrai iniziato a giocare. \nPer giocare devi muovere la Mercedes in basso a destra e sinistra con le freccette, in modo da far rimbalzare Lewis su di essa e successivamente fargli colpire le RedBull. \nIl gioco finisce se Lewis tocca terra o se non ci sono più RedBull. \n\nInoltre ci sono 4 bonus da colpire con la Mercedes: \nFia: si aumentano i punti. \nToto: Lewis diventa più grande. \nW12: Lewis rimbalza solo sui bordi e non sulle macchine per 15 secondi. Bottas: la Mercedes diventa più veloce", "Cattoni Inc.");
		    }
		
            //SetUp
            private void logo_Loaded(object sender, RoutedEventArgs e)
            {
                DoubleAnimation heightAnimation = new DoubleAnimation(500, new Duration(TimeSpan.FromSeconds(1.5)));
                logo.BeginAnimation(HeightProperty, heightAnimation);
                //tb_user.BeginAnimation(HeightProperty, heightAnimation);
            }
            private void bt_start_Click(object sender, RoutedEventArgs e)
            {
                MessageBox.Show("Clicca invio e spazio per iniziare a giocare!","Cattoni Inc.");
                Button b = sender as Button;
                //cl_bt.Visibility = Visibility.Hidden;
                b.Visibility = Visibility.Hidden;
                logo.Visibility = Visibility.Hidden;
                tb_user.Visibility = Visibility.Hidden;
                lb_user.Visibility = Visibility.Hidden;
                istruzioni.Visibility = Visibility.Hidden;

                mercedes = new Rectangle();
                mercedes.Width = 280;
                mercedes.Height = 135;
                mercedes.Fill = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/BrickBreakerss;component/Resources/Piattaforma.png", UriKind.Absolute))
                };

                Lewis = new Ellipse();
                Lewis.Fill = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/BrickBreakerss;component/Resources/Lewis.png", UriKind.Absolute))
                };

                cv_container.Children.Add(mercedes);
                cv_container.Children.Add(Lewis);
                utente.Username= tb_user.Text;

                AudioSetUp();
                SetUp();
            
                //StartSound.controls.stop();
            
            }
            private void AudioSetUp()
            {
                //SoundPlayer sp = new SoundPlayer(@"\\Resources\\Start.wav\");
                //sp.Play();

                //sp.PlayLooping();
                //sp.Stop();

                StartSound = new WindowsMediaPlayer();
                StartSound.URL = @"Resources\Start.wav"; //start
                StartSound.controls.stop();

                GoverSound = new WindowsMediaPlayer();
                GoverSound.URL = @"Resources\Lose.wav"; //gameover
                GoverSound.controls.stop();

                RankTop = new WindowsMediaPlayer();
                RankTop.URL = @"Resources\GetInThere.wav"; //vittoria
                RankTop.controls.stop();

                Win = new WindowsMediaPlayer();
                Win.URL = @"Resources\1posto.wav"; //max punteggio
                Win.controls.stop();

                BonusSound = new WindowsMediaPlayer();
                BonusSound.URL = @"Resources\HitBonus.wav"; //max punteggio
                BonusSound.controls.stop();
            }
            private void SetUp()
            {
                StartSound.controls.play();
                gameOver = false;
                started = false;
                punti = 0;
                dirY = 1;
                dirX = -1;
                LewisX = 10;
                LewisY = 10;
                speedMercedes = 18;
                tb_punti.Visibility = Visibility.Visible;
                tb_punti.Text = "Punti: " + punti;
                tb_punti.FontSize = 24;
                tb_infoMatch.Visibility = Visibility.Visible;
                //w12 = false;
                w12Timer = 0;

                Canvas.SetLeft(mercedes, cv_container.ActualWidth / 2 - mercedes.Width / 2);
                Canvas.SetBottom(mercedes, 10);

                double posB = Canvas.GetBottom(mercedes);

                Lewis.Width = 65;
                Lewis.Height = 50;

                Canvas.SetLeft(Lewis, cv_container.ActualWidth / 2 - Lewis.Width / 2);
                Canvas.SetBottom(Lewis, posB + mercedes.Height);

                pos_bottom = Canvas.GetBottom(Lewis);
                pos_left = Canvas.GetLeft(Lewis);

                CreaRedBulls();
            }
            public void CreaRedBulls()
            {
                int nBr = 0; //numero redBull già messe


                int redBullWidth = 240;
                int redBullHeight = 60;

                int top = 30;
                int left = 0;
                Rectangle redBull;
                ImageBrush sfondo = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/BrickBreakerss;component/Resources/brick.png", UriKind.Absolute))
                };

                for (int i = 0; i < nRedBullTot; i++)
                {
                    redBull = new Rectangle();
                    redBull.Width = redBullWidth;
                    redBull.Height = redBullHeight;
                    redBull.Fill = sfondo;

                    if (nBr == nRedBullRiga)
                    {
                        top += redBullHeight;
                        left = 0;
                        nBr = 0;
                    }
                    if (nBr < nRedBullRiga)
                    {
                        nBr++;

                        cv_container.Children.Add(redBull);
                        elencoRedBull.Add(redBull);

                        Canvas.SetLeft(redBull, left);
                        Canvas.SetTop(redBull, top);

                        left += redBullWidth;
                    }
                }
            }
            public void MuoviLewis(object sender, EventArgs e)//azioni movimento Lewis+gestione win e lose
            {
                pos_bottom += LewisY * dirY;
                pos_left += LewisX * dirX;

                Canvas.SetLeft(Lewis, pos_left);
                Canvas.SetBottom(Lewis, pos_bottom);


                if (pos_bottom <= 0)
                {
                    gameOver = true;
                    started = false;
                    ClearSounds();
                    GoverSound.controls.play();
                    GameOver();
                    tb_infoMatch.Text = "";
                    MessageBox.Show("Hai perso!! Premi due volte invio e poi spazio per riprovare", "Cattoni Inc.");
                    Classificata();
                    return;
                }


                Rect AreaLewis =
                    new Rect(Canvas.GetLeft(Lewis),
                        (cv_container.ActualHeight - Canvas.GetBottom(Lewis)),
                        Lewis.ActualWidth,
                        Lewis.ActualHeight);
                Rect AreaPiattaforma =
                    new Rect(Canvas.GetLeft(mercedes),
                        (cv_container.ActualHeight - Canvas.GetBottom(mercedes)),
                        mercedes.ActualWidth,
                        mercedes.ActualHeight);



                if (pos_bottom >= cv_container.ActualHeight - Lewis.Height) dirY = -1;
                if (pos_left <= 0) dirX = 1;
                if (pos_left >= cv_container.ActualWidth - Lewis.Width && dirX > 0) dirX = -1;
                if (AreaLewis.IntersectsWith(AreaPiattaforma))
                {
                    dirY = 1;
                    if (Canvas.GetLeft(Lewis) + Lewis.Width / 2 < Canvas.GetLeft(mercedes) + mercedes.Width / 4) dirX = -1;
                    else if (Canvas.GetLeft(Lewis) + Lewis.Width / 2 > Canvas.GetLeft(mercedes) + mercedes.Width - (mercedes.Width / 4)) dirX = 1;
                }
                else
                {
                    for (int i = elencoRedBull.Count - 1; i >= 0; i--)
                    {
                        Rectangle redBull = elencoRedBull[i];
                        Rect AreaRedBull = new Rect(
                            Canvas.GetLeft(redBull),
                            Canvas.GetTop(redBull) + redBull.ActualHeight,
                            redBull.ActualWidth, redBull.ActualHeight);

                        if (AreaLewis.IntersectsWith(AreaRedBull))
                        {
                            if (!w12)
                                Lewis_Collision(redBull);
                            else if (w12Timer > 0) w12Timer--;
                            else
                            {
                                w12 = false;
                            }
                            punti++;
                            cv_container.Children.Remove(redBull);
                            elencoRedBull.Remove(redBull);
                            tb_punti.Text = "Punti: " + punti;


                            if (elencoRedBull.Count==0)
                            {
                                gameOver = true;
                                started = false;
                                GameOver();
                                ClearSounds();
                                Win.controls.play();
                                MessageBox.Show("HAI VINTO!!! CLICCA DUE VOLTE INVIO PER RIGIOCARE!", "Cattoni Inc.");
                                Classificata();
                                return;
                            }
                            Create_Bonus(AreaRedBull);
                            break;
                        }
                    }
                }
            }
            private void Lewis_Collision(Rectangle redBull)
            {

                double LewisLeft = Canvas.GetLeft(Lewis);
                double LewisRight = Canvas.GetLeft(Lewis) + Lewis.Width;
                double LewisTop = cv_container.ActualHeight - (Canvas.GetBottom(Lewis) - Lewis.Height);
                double LewisBottom = cv_container.ActualHeight - Canvas.GetBottom(Lewis);

                double redBullLeft = Canvas.GetLeft(redBull);
                double redBullRight = Canvas.GetLeft(redBull) + redBull.Width;
                double redBullTop = Canvas.GetTop(redBull);
                double redBullBottom = Canvas.GetTop(redBull) + redBull.Height;

                double LewisCenterX = Canvas.GetLeft(Lewis) + Lewis.Width / 2;
                double LewisCenterY = Canvas.GetTop(Lewis) + Lewis.Height / 2;
                double redBullCenterX = Canvas.GetLeft(redBull) + redBull.Width / 2;
                double redBullCenterY = Canvas.GetTop(redBull) + redBull.Height / 2;


                if (Math.Abs(LewisCenterX - redBullCenterX) / redBull.Width < 0.5) // colpito sopra o sotto
                {
                    dirY = -dirY;
                }
                else if (Math.Abs(LewisCenterY - redBullCenterY) / redBull.Height < 0.5) // colpito a destra o a sinistra
                {
                    dirX = -dirX;
                }
                else if (LewisLeft < redBullLeft && LewisTop < redBullTop) //in alto a sinistra 
                {
                    if (dirY == 1 && dirX == 1) dirX = -dirX;
                    else if (dirY == -1 && dirX == 1)
                    {
                        dirX = -dirX;
                        dirY = -dirY;
                    }
                    else if (dirY == -1 && dirX == -1) dirY = -dirY;
                    else
                    {
                        dirX = -dirX;
                        dirY = -dirY;
                    }
                }
                else if (LewisLeft < redBullLeft && LewisTop > redBullTop) //in basso a sinistra
                {
                    if (dirY == 1 && dirX == 1)
                    {
                        dirX = -dirX;
                        dirY = -dirY;
                    }
                    else if (dirY == -1 && dirX == 1) dirX = -dirX;
                    else if (dirY == 1 && dirX == -1) dirY = -dirY;
                    else
                    {
                        dirX = -dirX;
                        dirY = -dirY;
                    }
                }
                else if (LewisLeft > redBullLeft && LewisTop > redBullTop) //in alto a destra
                {
                    if (dirY == 1 && dirX == -1) dirX = -dirX;
                    else if (dirY == -1 && dirX == -1)
                    {
                        dirX = -dirX;
                        dirY = -dirY;
                    }
                    else if (dirY == -1 && dirX == 1) dirY = -dirY;
                    else
                    {
                        dirX = -dirX;
                        dirY = -dirY;
                    }
                }
                else if (LewisLeft > redBullLeft && LewisTop < redBullTop) //in basso a destra
                {
                    if (dirY == 1 && dirX == -1)
                    {
                        dirX = -dirX;
                        dirY = -dirY;
                    }
                    else if (dirY == -1 && dirX == -1) dirX = -dirX;
                    else if (dirY == 1 && dirX == 1) dirY = -dirY;
                    else
                    {
                        dirX = -dirX;
                        dirY = -dirY;
                    }
                }
            }
        
        
            //gestione punti
            private void Classificata()
            {
                classifica_tot = FileManager.Leggi("classifica.bin");
                utente.Punteggio=punti;
                classifica_tot.Add(utente);
                (classifica_tot).Sort();
                FileManager.Salva(classifica_tot, "classifica.bin");
                    //int rankTot = classifica_tot.FindIndex(x => (x as Utemte).Punteggio.Equals(punti)) + 1;
                    int rankTot = classifica_tot.Max(x => x.Punteggio);
                if (rankTot == punti)
                {
                    RankTop.controls.play();
                    MessageBox.Show("SEI IL MIGLIORE DELLA STORIA, hai fatto " + (classifica_tot[0]).Punteggio+" punti", "Cattoni Inc.");
                }
        
                    //MessageBox.Show("È il tuo "+rankUt+"° miglior Punteggioo e nella classifica generale "+rankTot+"°");
        
            }
            private void visual_cl(object sender, RoutedEventArgs e)
            {
                contenitore.Visibility = Visibility.Hidden;
                /*contenitore.Children.Clear();
			    contenitore.RowDefinitions.Clear();
			    contenitore.ColumnDefinitions.Clear();*/
                //cont_main.Children.Clear();
                cont_main.Children.Add(new Classifica());
            }
        
            //gestioneBonus
            private void Create_Bonus(Rect RedBullArea)
            {
                rnd = new Random(Guid.NewGuid().GetHashCode());
                int chance = rnd.Next(0, 400);

                if (chance < 100) // 25% chance 
                {
                    Ellipse bonus = new Ellipse();
                    bonus.Width = 50;
                    bonus.Height = 50;
                if (chance < 25)
                    {
                        //bottas (compagno di lewia) lo fa andare più veloce
                        bonus.Fill = new ImageBrush
                        {
                            ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/BrickBreakerss;component/Resources/BonusBottas.png", UriKind.Absolute))
                        };
                        
                        Canvas.SetLeft(bonus, RedBullArea.X + RedBullArea.Width / 2 - bonus.Width / 2);
                        Canvas.SetTop(bonus, RedBullArea.Y + RedBullArea.Height / 2 - bonus.Height / 2);
                        cv_container.Children.Add(bonus);
                        elencoBonus.Add(bonus);
                        bonus.Tag = 1;
                }
                    else if (chance >= 25 && chance < 50)
                    {
                        //Toto wolffaumenta dimensione Lewis
                        bonus.Fill = new ImageBrush
                        {
                            ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/BrickBreakerss;component/Resources/BonusToto.png", UriKind.Absolute))
                        };
                        
                        Canvas.SetLeft(bonus, RedBullArea.X + RedBullArea.Width / 2 - bonus.Width / 2);
                        Canvas.SetTop(bonus, RedBullArea.Y + RedBullArea.Height / 2 - bonus.Height / 2);
                        cv_container.Children.Add(bonus);
                        elencoBonus.Add(bonus);
                        bonus.Tag = 2;

                }
                    else if (chance >= 50 && chance < 75)
                    {
                        bonus.Width = 100;
                        bonus.Height = 20;
                        //la w12 fa passare attraverso blocchi
                        bonus.Fill = new ImageBrush
                        {
                            ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/BrickBreakerss;component/Resources/BonusW12.png", UriKind.Absolute))
                        }; ;

                        Canvas.SetLeft(bonus, RedBullArea.X + RedBullArea.Width / 2 - bonus.Width / 2);
                        Canvas.SetTop(bonus, RedBullArea.Y + RedBullArea.Height / 2 - bonus.Height / 2);
                        cv_container.Children.Add(bonus);
                        elencoBonus.Add(bonus);
                        bonus.Tag = 3;
                }
                    else
                    {
                        //la fia ci da più punti
                        bonus.Fill = new ImageBrush
                        {
                            ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/BrickBreakerss;component/Resources/BonusFia.png", UriKind.Absolute))
                        };
                        Canvas.SetLeft(bonus, RedBullArea.X + RedBullArea.Width / 2 - bonus.Width / 2);
                        Canvas.SetTop(bonus, RedBullArea.Y + RedBullArea.Height / 2 - bonus.Height / 2);
                        cv_container.Children.Add(bonus);
                        elencoBonus.Add(bonus);
                        bonus.Tag = 4;
                    }
                }
            }
            public void MuoviBonus(object sender, EventArgs e)
            {
                for (int i = elencoBonus.Count - 1; i >= 0; i--)
                {
                    double pos_top = Canvas.GetTop(elencoBonus[i]);
                    Rect AreaBonus =
                    new Rect(Canvas.GetLeft(elencoBonus[i]),
                        (Canvas.GetTop(elencoBonus[i])+elencoBonus[i].Height),
                        elencoBonus[i].ActualWidth,
                        elencoBonus[i].ActualHeight);
                    Rect AreaPiattaforma =
                        new Rect(Canvas.GetLeft(mercedes),
                            (cv_container.ActualHeight - Canvas.GetBottom(mercedes)),
                            mercedes.ActualWidth,
                            mercedes.ActualHeight);


                    Canvas.SetTop(elencoBonus[i], pos_top + 5);
                    if (AreaBonus.IntersectsWith(AreaPiattaforma))
                    {
                        BonusHit(elencoBonus[i]);
                        cv_container.Children.Remove(elencoBonus[i]);
                        elencoBonus.Remove(elencoBonus[i]);
                    }
                    else if (pos_top >= cv_container.ActualHeight)
                    {
                        cv_container.Children.Remove(elencoBonus[i]);
                        elencoBonus.Remove(elencoBonus[i]);
                    }
                }
            }
            private void BonusHit(Ellipse bonus) //cosa fanno i bonus
            {
                BonusSound.controls.play();
                if (bonus.Tag.Equals(1))
                {
                    tb_infoMatch.Text = "La Mercedes va più veloce!";
                    //valtterri bottas
                    speedMercedes += 10;
                }
                else if (bonus.Tag.Equals(2))
                {
                    tb_infoMatch.Text = "Lewis è più grande!";
                    //lewis grazie a toto diventa più grande
                    if(Lewis.Width<130) //sennò diventa troppo grande
                    {
                    Lewis.Width = Lewis.Width + Lewis.Width * 0.35;
                    Lewis.Height = Lewis.Height + Lewis.Height * 0.35;
                    }
                    
                }
                else if (bonus.Tag.Equals(3))
                {
                    tb_infoMatch.Text = "Lewis passa attraverso le RedBull!";
                    //lewis torna sulla w12
                    w12 = true;
                    w12Timer = 15;
                }
                else
                {
                    tb_infoMatch.Text = "I tuoi punti sono aumentati del 50%!";
                    //punti=+50%
                    punti = punti+punti/2;
                    tb_punti.Text = "Punti: " + punti;
                }
            }

            //gestione movimenti lewis
            private void window_KeyDown(object sender, KeyEventArgs e)
            {
                double posAttuale;

                switch (e.Key)
                {
                    case Key.Left:
                        if (started)
                        {
                            posAttuale = Canvas.GetLeft(mercedes);
                            if (posAttuale > 0)
                                Canvas.SetLeft(mercedes, posAttuale - speedMercedes);
                        }
                        break;
                    case Key.Right:
                        if (started)
                        {
                            posAttuale = Canvas.GetLeft(mercedes);
                            if (posAttuale < cv_container.ActualWidth - mercedes.Width)
                                Canvas.SetLeft(mercedes, posAttuale + speedMercedes);
                        }
                        break;
                    case Key.Space:
                        if (!started && !gameOver)
                        {

                            rnd = new Random(Guid.NewGuid().GetHashCode());
                            LewisX = rnd.Next(8, 14);
                            LewisY = rnd.Next(8, 14);
                            if (rnd.Next(0, 10) >= 5) dirX = 1;
                            started = true;
                            timer_Lewis.Start();
                            timer_bonus.Start();
                        }
                        break;
                    case Key.Enter:
                        if (gameOver)
                        {
                            foreach (Rectangle redBull in elencoRedBull)
                            {
                                cv_container.Children.Remove(redBull);
                            }
                            elencoRedBull.Clear();
                            foreach (Ellipse bonus in elencoBonus)
                            {
                                cv_container.Children.Remove(bonus);
                            }
                            ClearSounds();
                            elencoBonus.Clear();
                            SetUp();
                        }
                        break;
                }
            }

            //gameover
            private void ClearSounds()
            {
                StartSound.controls.stop();
                GoverSound.controls.stop();
                RankTop.controls.stop();
                Win.controls.stop();
                BonusSound.controls.stop();
            }
            private void GameOver()
            {
                gameOver = true;
                timer_Lewis.Stop();
                timer_bonus.Stop();
            }
    }
}
