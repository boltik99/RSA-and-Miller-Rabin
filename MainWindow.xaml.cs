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
using System.Security.Cryptography;
using System.Collections;
using System.Diagnostics;
using System.Numerics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media.Animation;

namespace Rsa_and_Miller_Rabin
{
    public partial class MainWindow : Window
    {
        BigInteger n;
        BigInteger d;
        BigInteger r_e;

        public MainWindow()
        {
            InitializeComponent();
        }

        public static BigInteger Rd_int(int dlina)
        {
            dlina = dlina / 8;
            BigInteger rd;
            using (RNGCryptoServiceProvider rg = new RNGCryptoServiceProvider())
            {
                byte[] rno = new byte[dlina];
                rg.GetBytes(rno);
                rd = new BigInteger(rno);
            }
            if (rd < 0)
            {
                return rd * (-1);
            }
            else
                return rd;
        }//генерация случайного числа

        public static BigInteger Random_4islo(int dlina)
        {
            int dl = dlina - 1;
            BigInteger res = 0;
            Random r = new Random();
            int[] dv_res = new int[dlina];
            dv_res[0] = 1;
            int j = 1;
            while (dl != 0)
            {
                int rn = r.Next(0, 2);
                dv_res[j] = rn;
                dl--;
                j++;
            }


            BigInteger g = 0;
            int s = dv_res.Length - 1;
            for (int i = 0; i < dv_res.Length; i++)
            {
                if (dv_res[i] == 1)
                {
                    //g = BigInteger.Pow(2, s);
                    g = stepen_(2, s);
                    res += g;
                }
                s--;
            }
            return res;
        }

        public static int Dlina_e(BigInteger n)
        {
            BigInteger nn = n;
            string dl_n = Convert.ToString(nn);
            
            int dlina_e = (int)(dl_n.Length * 1.4);

            return dlina_e;
        } // берем длину е такой что была равна 1/3 длине n

        public static BigInteger RSA_E(TextBox tx_RSA_d, BigInteger fn, BigInteger n, int dlina) //вычисляем е
        {
            BigInteger x, y;
            BigInteger rsa_e = 0;
            int dlina_rsa_e = Dlina_e(n);

            if (dlina < 50)
                rsa_e = Random_4islo(dlina_rsa_e);
            else
                rsa_e = Rd_int(dlina_rsa_e);

            while (rsa_e > fn || rsa_e < 1)
            {
                rsa_e = Random_4islo(dlina_rsa_e);
            }
            
            while(rsa_e % 2 == 0)
            {
                rsa_e++;
            }

            while (MillerRabinTest(rsa_e) != true)
            {
                rsa_e += 2;
            }


            if (Evklid(fn, rsa_e, out x, out y) == 1)
            {
                x = 0;
                y = 0;
                BigInteger nod = Evklid(fn, rsa_e, out x, out y);
                BigInteger d = (y % fn + fn) % fn;
                if (d < 0)
                    d = d * (-1);

                if (((rsa_e * d) % fn) != 1)
                {
                    return RSA_E(tx_RSA_d, fn, n, dlina);
                }
                else
                {
                    tx_RSA_d.Text = Convert.ToString(d);
                    return rsa_e;
                }
            }
            else
                return RSA_E(tx_RSA_d, fn, n, dlina);
            
        }

        private void Gen_Click(object sender, RoutedEventArgs e)//кнопка генерации p q n f итд  
        {
            int dlina;
            try
            {
                dlina = Convert.ToInt32(tx_RSA_kol_bit.Text);
            }
            catch
            {
                System.Windows.MessageBox.Show("Введите количество бит");
                return;
            }
            //############################################################

            List<BigInteger> p_and_q = gen_p_and_q(dlina);
            BigInteger p = p_and_q[0];
            BigInteger q = p_and_q[1];


            while (MillerRabinTest(p) != true)
            {
                p += 2;
            }

            while (MillerRabinTest(q) != true)
            {
                q += 2;
            }
            //############################################################
            BigInteger fn = (p - 1) * (q - 1);
            n = p * q;
            tx_RSA_n.Text = Convert.ToString(n);

            r_e = RSA_E(tx_RSA_d, fn, n, dlina);
            //############################################################
            d = BigInteger.Parse(tx_RSA_d.Text);

            tx_RSA_p.Text = Convert.ToString(p);
            tx_RSA_q.Text = Convert.ToString(q);
            tx_RSA_f.Text = Convert.ToString(fn);
            tx_RSA_e.Text = Convert.ToString(r_e);
            tx_RSA_d.Text = Convert.ToString(d);
        }

        public static BigInteger Evklid(BigInteger A, BigInteger B, out BigInteger e_x, out BigInteger e_y)
        {
            BigInteger hod = 0;
            List<BigInteger> A_ls = new List<BigInteger>();
            List<BigInteger> B_ls = new List<BigInteger>();
            List<BigInteger> A_mod_B = new List<BigInteger>();
            List<BigInteger> A_div_B = new List<BigInteger>();
            List<BigInteger> X_ls = new List<BigInteger>();
            List<BigInteger> Y_ls = new List<BigInteger>();

            A_ls.Add(A);
            B_ls.Add(B);
            A_mod_B.Add(A % B);
            A_div_B.Add(A / B);

            while (A_ls.Last() % B_ls.Last() != 0)
            {
                A_ls.Add(B_ls.Last());
                B_ls.Add(A_mod_B.Last());
                A_mod_B.Add(A_ls.Last() % B_ls.Last());
                A_div_B.Add(A_ls.Last() / B_ls.Last());
            }
            hod = B_ls.Last();

            X_ls.Add(0);
            Y_ls.Add(1);
            A_div_B.Remove(A_div_B.Last());
            A_div_B.Reverse();
            foreach (var d in A_div_B)
            {
                BigInteger y = X_ls.Last() - Y_ls.Last() * d;
                X_ls.Add(Y_ls.Last());
                Y_ls.Add(y);
            }

            e_x = X_ls.Last();
            e_y = Y_ls.Last();


            return hod;
        }//НОД евклид 

        public static List<int> perevod(BigInteger b)
        {
            BigInteger os = 0;
            List<int> dv = new List<int>();
            while (b > 0)
            {
                os = b % 2;
                b /= 2;
                dv.Add((int)os);
            }
            dv.Reverse();
            
            return dv;
        }//перевод из десятичного в двоичный

        public static BigInteger stepen_po_mod(BigInteger a, BigInteger b, BigInteger n)
        {
            BigInteger res;
            List<int> b_dv = perevod(b);

            BigInteger[] a_i = new BigInteger[b_dv.Count];
            a_i[0] = a;
            BigInteger dlina = b_dv.Count - 1;
            int i = 0;
            while (dlina != 0)
            {
                if (b_dv[i + 1] == 0)
                    a_i[i + 1] = (a_i[i] * a_i[i]) % n;
                else
                    a_i[i + 1] = (a_i[i] * a_i[i] * a) % n;
                i++;
                dlina--;
            }
            res = a_i.Last();
            return res;
        }//степень по модулю
        public static BigInteger stepen_(BigInteger a, BigInteger b)
        {
            BigInteger res;
            List<int> b_dv = perevod(b);

            if (b_dv.Count == 0)
            {
                return 1;
            }
            BigInteger[] a_i = new BigInteger[b_dv.Count];
            a_i[0] = a;
            BigInteger dlina = b_dv.Count - 1;
            int i = 0;
            while (dlina != 0)
            {
                if (b_dv[i + 1] == 0)
                    a_i[i + 1] = (a_i[i] * a_i[i]);
                else
                    a_i[i + 1] = (a_i[i] * a_i[i] * a);
                i++;
                dlina--;
            }
            res = a_i.Last();
            return res;
        }//степень по модулю

        public void Sh_text(TextBox tx_RSA_text, BigInteger rsa_e, BigInteger n)//шифровака
        {
            string test = tx_RSA_text.Text;
            List<int> index = new List<int>();

            foreach (var i in test)
                index.Add(i);

            foreach(var j in index)
                tx_RSA_text_sh.Text += Convert.ToString(stepen_po_mod(j, rsa_e, n))+ ' ';
        }

        private void Sh_Click(object sender, RoutedEventArgs e)//кнопка шифровки
        {
            tx_RSA_text_sh.Clear();
            if (tx_RSA_text.Text == "")
            {
                System.Windows.MessageBox.Show("Введите сообщение");
                return;
            }
            Sh_text(tx_RSA_text, r_e, n);

        }

        public static void Desh_text(TextBox tx_RSA_text_sh, TextBox tx_RSA_text_desh, BigInteger d, BigInteger n)
        {
            string test_dh = tx_RSA_text_sh.Text;
            
            string[] test_dh_s = test_dh.Trim().Split(' ');

            for(int i = 0; i < test_dh_s.Length; i++)
            {
                BigInteger g = stepen_po_mod(BigInteger.Parse(test_dh_s[i]), d, n);

                try
                {
                    tx_RSA_text_desh.Text += (char)g;
                }
                catch
                {
                    tx_RSA_text_desh.Text += '?';
                }

            }
        }

        public void Desh_Click(object sender, RoutedEventArgs e)//кнопка расшифровка
        {
            tx_RSA_text_desh.Clear();

            Desh_text(tx_RSA_text_sh, tx_RSA_text_desh, d, n);
        }

        public List<BigInteger> gen_p_and_q (int dlina)
        {
            List<BigInteger> li_st = new List<BigInteger>();
            BigInteger p = 0;
            BigInteger q = 0;
            if (dlina < 50)
            {
                p = Random_4islo(dlina);
                q = Random_4islo(dlina);
            }
            else
            {
                p = Rd_int(dlina);
                q = Rd_int(dlina);
            }


            if (p == q)
            {
                while (p == q)
                {
                    p = Rd_int(dlina);
                    q = Rd_int(dlina);
                }
            }

            while (p % 2 == 0)
            {
                p++;
            }

            while (q % 2 == 0)
            {
                q++;
            }
            li_st.Add(p);
            li_st.Add(q);
            return li_st;
        }

        static bool MillerRabinTest(BigInteger n)
        {
            BigInteger k = (BigInteger)BigInteger.Log(n, 2);

            BigInteger t = n - 1;
            int s = 0;
            while (t % 2 == 0)
            {
                t /= 2;
                s += 1;
            }

            for (int i = 0; i < k; i++)
            {
                List<int> f = perevod(n);
                BigInteger a = Random_4islo(f.Count - 2);

                BigInteger x = stepen_po_mod(a, t, n);
                if (x == 1 || x == n - 1)// n - 1 не обязательно
                    continue;

                for (int r = 1; r < s; r++)
                {
                    x = stepen_po_mod(x, 2, n);
                    if (x == 1)
                        return false;
                    if (x == n - 1)
                        break;
                }

                if (x != n - 1)
                    return false;
            }
            return true;
        }
    }

}
