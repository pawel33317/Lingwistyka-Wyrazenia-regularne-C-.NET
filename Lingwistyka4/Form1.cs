using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lingwistyka4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string input = "";
        string[] tablicaZdan;
        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            int size = 0;
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                try
                {
                    input = File.ReadAllText(file);
                    size = input.Length;
                }
                catch (IOException)
                {
                    MessageBox.Show("Error");
                }
                finally
                {
                    label1.Text = openFileDialog1.FileName.ToString();
                }
            }

            textBox1.Text = "";
            tablicaZdan = input.Split(new string[] { "\n" }, StringSplitOptions.None);
            foreach (string s in tablicaZdan)
            {
                textBox1.Text += s;
                textBox1.AppendText(Environment.NewLine);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox3.Text = "";
            tablicaZdan = textBox1.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (string s in tablicaZdan)
            {
                textBox3.Text += s;
                textBox3.AppendText(Environment.NewLine);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (tablicaZdan == null)
                return;
            textBox3.Text = "";
            foreach (string s in tablicaZdan)
            {
                Regex regex;
                Match match;
                bool poprawne = false;

                if (radioButton1.Checked) //IP
                {
                    //^     --> już od początku ciągu musi się zgadzać
                    //()    --> grupuje
                    //[0-9] --> zakres znaków
                    //{1,3} --> musi wystąpić od 1 razu do 3
                    //[\.]  --> kropka   jeżeli jest w klamrach [] nie trzema poprzedzać \
                    //{3}   --> całą grupa czyli () musi nastąpić 3 razy
                    //[ ]   --> spacja
                    //$     --> po tym ciągu już nic nie może być
                    regex = new Regex(@"^([0-9]{1,3}[\.]){3}([0-9]){1,3}([ ])([0-9]{1,3}[\.]){3}([0-9]){1,3}$");
                    match = regex.Match(s);

                    if (match.Success)
                    {
                        string[] SplittedBySpace = match.ToString().Split(new string[] { " " }, StringSplitOptions.None);
                        string[] IPSplittedByDOT = SplittedBySpace[0].Split(new string[] { "." }, StringSplitOptions.None);
                        string[] MaskSplittedByDOT = SplittedBySpace[1].Split(new string[] { "." }, StringSplitOptions.None);
                        //sprawdza IP czy ok JAK OK ustawia na true
                        if (int.Parse(IPSplittedByDOT[0]) < 256 && int.Parse(IPSplittedByDOT[1]) < 256 && int.Parse(IPSplittedByDOT[2]) < 256 && int.Parse(IPSplittedByDOT[3]) < 256)
                        {
                            poprawne = true;
                        }

                        //sprawdza maskę jak znajdzie błąd przestawia znów na false
                        //czy poprzednie człony maski są równe 255 jeżeli kolejny za nimi nie jest 0 
                        if (int.Parse(MaskSplittedByDOT[3]) > 0 )
                            if (int.Parse(MaskSplittedByDOT[2]) != 255)
                                poprawne = false;
                        if (int.Parse(MaskSplittedByDOT[2]) > 0)
                            if (int.Parse(MaskSplittedByDOT[1]) != 255)
                                poprawne = false;
                        if (int.Parse(MaskSplittedByDOT[1]) > 0)
                            if (int.Parse(MaskSplittedByDOT[0]) != 255)
                                poprawne = false;


                        //czy mają możliwe wartości maski: 0, 128, 192, 224, 240, 248, 252, 254, 255
                        foreach (string m in MaskSplittedByDOT)
                        {
                            int n = int.Parse(m);
                            if (n != 0 && n != 128 && n != 192 && n != 224 && n != 240 && n != 248 && n != 252 && n != 254 && n != 255)
                                poprawne = false;
                        }


                        
                    }
                }
                if (radioButton2.Checked) //MAIL
                {
                    //[a-zA-Z0-9_.+-] może wystąpić1 lub więcej razy przez "+"
                    //@  musi wystąpić znak @
                    //\. musi być kropka
                    //[a-zA-Z0-9-.]+ wielokrotnie końcówka domeny
                    regex = new Regex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");
                    match = regex.Match(s);
                    if (match.Success)
                        poprawne = true;
                }
                if (radioButton3.Checked) //ADD INTEGER
                {
                    //0-9 z + dowolną ilość razy
                    //na końcu cyfra
                    regex = new Regex(@"^([0-9]+\+)+[0-9]+$");
                    match = regex.Match(s);
                    if (match.Success)
                        poprawne = true;
                }
                if (radioButton4.Checked) //ADD COMPLEX
                {
                    //? raz lub w ogóle
                    //| lub lewa lub prawa
                    regex = new Regex(@"^(([0-9]+[i]?|[i])\+)+([0-9]+[i]?|i)$");
                    match = regex.Match(s);
                    if (match.Success)
                        poprawne = true;
                }
                if (radioButton5.Checked) //HTML TABLE
                {
                    //[^<]*  wszystko tylko nie < dowolną iloość razy lub w ogóle
                    //(.*) wszystko ile razy się chce
                    //<table --> ten tekst 
                    //([ ]([^<])*>)|(>)   spacja i jakiś tekst lub tylko spacja i znak ">" | znak ">"
                    regex = new Regex(@"^<table(([ ]([^<])*>)|(>))(.*)</table>$");
                    match = regex.Match(s);
                    if (match.Success)
                        poprawne = true;
                }
                if (radioButton6.Checked) //HTML A, P B SRTONG H1 EM I 
                {
                    regex = new Regex(@"^<a(([ ]([^<])*>)|(>))(.*)</a>$");
                    match = regex.Match(s);
                    if (match.Success)
                        poprawne = true;
                    regex = new Regex(@"^<p(([ ]([^<])*>)|(>))(.*)</p>$");
                    match = regex.Match(s);
                    if (match.Success)
                        poprawne = true;
                    regex = new Regex(@"^<b(([ ]([^<])*>)|(>))(.*)</b>$");
                    match = regex.Match(s);
                    if (match.Success)
                        poprawne = true;
                    regex = new Regex(@"^<strong(([ ]([^<])*>)|(>))(.*)</strong>$");
                    match = regex.Match(s);
                    if (match.Success)
                        poprawne = true;
                    regex = new Regex(@"^<em(([ ]([^<])*>)|(>))(.*)</em>$");
                    match = regex.Match(s);
                    if (match.Success)
                        poprawne = true;
                    regex = new Regex(@"^<i(([ ]([^<])*>)|(>))(.*)</i>$");
                    match = regex.Match(s);
                    if (match.Success)
                        poprawne = true;
                    regex = new Regex(@"^<h1(([ ]([^<])*>)|(>))(.*)</h1>$");
                    match = regex.Match(s);
                    if (match.Success)
                        poprawne = true;
                }


                //jak sprawdzenie ok wypisz jak nie też
                if (poprawne)
                {
                    textBox3.Text += "POPRAWNY----->: " + s;
                }
                else
                {
                    textBox3.Text += "NIEPOPRAWNY-->: " + s;
                }
                textBox3.AppendText(Environment.NewLine);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            tablicaZdan = new string[] { 
                "10.0.0.1 255.255.255.0",
                "22.22.22.22 22.22.22.22",
                "123.132.289.22 255.255.255.255",
                "1.1.1..1 3.3.3.3",
                "1.1.1.1 3.3.s.3",
                "266.0.0.1 255.255.255.0",
                "26.0.0.1 255.255.255.133",
                "26.0.0.1 255.255.255.128",
                "266.0.0.1 255.255.128.128"
            };
            foreach (string s in tablicaZdan)
            {
                textBox1.Text += s;
                textBox1.AppendText(Environment.NewLine);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            tablicaZdan = new string[] { 
                "pawel33317@gmail.com",
                "pawel@pawel@.wp.pl",
                "pawel@pl",
                "@wp.pl",
                "pawe#$%@wp.pl",
                "pawel@o2.com$"
            };
            foreach (string s in tablicaZdan)
            {
                textBox1.Text += s;
                textBox1.AppendText(Environment.NewLine);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            tablicaZdan = new string[] { 
                "223+1232+324",
                "223+1+1+",
                "1+0",
                "+1+0",
                "2i+7+6+3+i+22",
                "i+i",
                "4i+3i+1",
                "i+ii",
                "i+3+i+i+i",
                "33+i+33+2",
                "3*i+5"
            };
            foreach (string s in tablicaZdan)
            {
                textBox1.Text += s;
                textBox1.AppendText(Environment.NewLine);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            tablicaZdan = new string[] { 
                "<table>dfdsaf<>\"\"#$AS.m,bJNV     xcv  </table>",
                "<table>dfdsaf<>\"\"#$AS.m,bJNV     xcv  <table>",
                "<\table>dfdsaf<>\"\"#$AS.m,bJNV     xcv  </table>",
                "<table>dfdsaf<>\"\"#$AS.m,bJNV     xcv  </tablfe>",
                "<table id=\"adsf asd\">dfdsaf<>\"\"#$AS.m,bJNV     xcv  </table>",
                "<table weight='100px;'>dfdsaf<>\"\"#$AS.m,bJNV     xcv  </table>",
                "<table asd das >dfdsaf<>\"\"#$AS.m,bJNV     xcv  </table>",
                "<tableasdf sdf >dfdsaf<>\"\"#$AS.m,bJNV     xcv  </table>",
                "<table df  fd fdsa fsd gfsfd=''>dfdsaf<>\"\"#$AS.m,bJNV     xcv  </table>",
                "<tableasdf <a></a> >dfdsaf<>\"\"#$AS.m,bJNV     xcv  </table>"
            };
            foreach (string s in tablicaZdan)
            {
                textBox1.Text += s;
                textBox1.AppendText(Environment.NewLine);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            tablicaZdan = new string[] { 
                "<a href='#'>Link</a>",
                "<p class='klasa'>Link</p>",
                "<b>Link</b>",
                "<b>Link</a>",
                "<ccc>Link</ccc>",
                "<h1>Link</h1>",
                "<a>Link<a>",
                "<strong>Link</strong>",
                "<table>dfdsaf<>\"\"#$AS.m,bJNV     xcv  </table>"
            };
            foreach (string s in tablicaZdan)
            {
                textBox1.Text += s;
                textBox1.AppendText(Environment.NewLine);
            }
        }
    }
}
