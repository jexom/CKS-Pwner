using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pwner
{
    public partial class Form1 : Form
    {
        Dictionary<char, byte> dumb = new Dictionary<char, byte>();
        byte[] lr = new byte[] { 0x01, 0xCA, 0x45, 0x77, 0x63 };
        String group = "A-  -  ";
        String name = "              ";
        byte[] cpGroup = new byte[7];
        byte[] cpName = new byte[14];

        byte[] allTasks = new byte[220];

        public Form1()
        {
            InitializeComponent();
            initArrays();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int labs = 1;
            if (radioButton2.Checked || radioButton3.Checked || radioButton4.Checked || radioButton5.Checked)
            {
                labs = 2;
            }
            if (radioButton3.Checked || radioButton4.Checked || radioButton5.Checked)
            {
                labs = 3;
            }
            if (radioButton4.Checked || radioButton5.Checked)
            {
                labs = 4;
            }
            if (radioButton5.Checked)
            {
                labs = 5;
            }

            pwn(labs);
            MessageBox.Show("Pwned!","Pwned!", MessageBoxButtons.OK);
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for(int i = 1; i < 6; i++)
            {
                pwn(i);
            }
            MessageBox.Show("Pwned!", "Pwned!", MessageBoxButtons.OK);
            Application.Exit();
        }

        private void pwn(int labs)
        {
            name = textBox1.Text.PadRight(14);
            group = maskedTextBox1.Text.PadRight(7);
            string filename = name.Substring(0, name.IndexOf(' ')) + group.Split('-')[1];

            Encoding utf16 = Encoding.GetEncoding("UTF-16");
            Encoding win1251 = Encoding.GetEncoding("Windows-1251");

            byte[] utf16Bytes = utf16.GetBytes(name);
            byte[] cpName = Encoding.Convert(utf16, win1251, utf16Bytes);

            utf16Bytes = utf16.GetBytes(group);
            byte[] cpGroup = Encoding.Convert(utf16, win1251, utf16Bytes);


            byte[] task = new byte[20];
            task[0] = dumb[name[4]];
            task[1] = dumb[name[1]];
            task[2] = dumb[name[0]];
            task[3] = dumb[name[10]];
            task[4] = dumb[group[6]];
            task[5] = dumb[name[12]];
            task[6] = dumb[group[3]];
            task[7] = dumb[name[9]];
            task[8] = dumb[name[2]];
            task[9] = dumb[name[8]];
            task[10] = dumb[name[3]];
            task[11] = 0x19;
            task[12] = lr[0];
            task[13] = dumb[name[5]];
            task[14] = dumb[group[5]];
            task[15] = dumb[name[13]];
            task[16] = dumb[name[11]];
            task[17] = dumb[name[6]];
            task[18] = dumb[name[7]];
            task[19] = dumb[group[2]];

            string state = "";
            switch (labs)
            {
                case 1:
                    state = state != "" ? state : "(1)";
                    task[12] = lr[0];
                    Array.Copy(task, 0, allTasks, 200, 20);
                    break;
                case 2:
                    state = state != "" ? state : "(1-2)";
                    task[12] = lr[1];
                    Array.Copy(task, 0, allTasks, 180, 20);
                    goto case 1;
                case 3:
                    state = state != "" ? state : "(1-3)";
                    task[12] = lr[2];
                    Array.Copy(task, 0, allTasks, 160, 20);
                    goto case 2;
                case 4:
                    state = state != "" ? state : "(1-4)";
                    task[12] = lr[3];
                    Array.Copy(task, 0, allTasks, 140, 20);
                    goto case 3;
                case 5:
                    state = "(1-Зачет)";
                    task[12] = lr[4];
                    Array.Copy(task, 0, allTasks, 120, 20);
                    goto case 4;
            }

            int checksum = 0;
            for (int i = 0; i < 220; i++)
            {
                checksum += allTasks[i];
            }

            checksum = checksum % 0xFFFF;
            string chksm = checksum.ToString();

            Encoding utf8 = Encoding.GetEncoding("UTF-8");
            utf16Bytes = utf8.GetBytes(chksm);
            byte[] cpChecksum = Encoding.Convert(utf8, win1251, utf16Bytes);


            File.WriteAllBytes("./" + filename + state + ".QRT", Properties.Resources.BaseFile);
            FileStream fStream = File.OpenWrite("./" + filename + state + ".QRT");
            fStream.Seek(0x73B5, SeekOrigin.Begin);
            fStream.Write(cpGroup, 0, 7);
            fStream.Seek(0x73BD, SeekOrigin.Begin);
            fStream.Write(cpName, 0, 14);
            fStream.Seek(0x73DB, SeekOrigin.Begin);
            fStream.Write(cpChecksum, 0, cpChecksum.Length);
            fStream.Seek(0x73E2, SeekOrigin.Begin);
            fStream.Write(allTasks, 0, 220);
            fStream.Close();
        }

        private void initArrays()
        {
            for(int i = 0; i < 14; i++)
            {
                cpName[i] = 0x20;
            }

            for (int i = 0; i < 7; i++)
            {
                cpGroup[i] = 0x20;
            }

            for(int i=0; i<220; i++)
            {
                allTasks[i] = 0x20;
            }

            dumb.Add(' ', 0x8D);
            dumb.Add('А', 0x54);
            dumb.Add('Б', 0x67);
            dumb.Add('В', 0x98);
            dumb.Add('Г', 0x4B);
            dumb.Add('Д', 0x1B);
            dumb.Add('Е', 0xCC);
            dumb.Add('Ж', 0x66);
            dumb.Add('З', 0xA3);
            dumb.Add('И', 0x92);
            dumb.Add('Й', 0xC8);
            dumb.Add('К', 0x86);
            dumb.Add('Л', 0x49);
            dumb.Add('М', 0x4F);
            dumb.Add('Н', 0x4E);
            dumb.Add('О', 0xCE);
            dumb.Add('П', 0x65);
            dumb.Add('Р', 0xEE);
            dumb.Add('С', 0xD8);
            dumb.Add('Т', 0xC1);
            dumb.Add('У', 0x84);
            dumb.Add('Ф', 0x87);
            dumb.Add('Х', 0x60);
            dumb.Add('Ц', 0x80);
            dumb.Add('Ч', 0xE2);
            dumb.Add('Ш', 0xB5);
            dumb.Add('Щ', 0x7D);
            dumb.Add('Э', 0x78);
            dumb.Add('Ю', 0xC0);
            dumb.Add('Я', 0x70);
            dumb.Add('а', 0x17);
            dumb.Add('б', 0x24);
            dumb.Add('в', 0x7B);
            dumb.Add('г', 0x58);
            dumb.Add('д', 0x46);
            dumb.Add('е', 0x32);
            dumb.Add('ж', 0xFF);
            dumb.Add('з', 0xF0);
            dumb.Add('и', 0x1D);
            dumb.Add('й', 0xB7);
            dumb.Add('к', 0xAE);
            dumb.Add('л', 0x25);
            dumb.Add('м', 0xD5);
            dumb.Add('н', 0xA9);
            dumb.Add('о', 0x81);
            dumb.Add('п', 0xC5);
            dumb.Add('р', 0xB1);
            dumb.Add('с', 0xDE);
            dumb.Add('т', 0xCD);
            dumb.Add('у', 0x52);
            dumb.Add('ф', 0x34);
            dumb.Add('х', 0x05);
            dumb.Add('ц', 0xF8);
            dumb.Add('ч', 0x5D);
            dumb.Add('ш', 0x0D);
            dumb.Add('щ', 0xE7);
            dumb.Add('ъ', 0x29);
            dumb.Add('ы', 0x94);
            dumb.Add('ь', 0x71);
            dumb.Add('э', 0x3C);
            dumb.Add('ю', 0x0C);
            dumb.Add('я', 0x5F);
            dumb.Add('0', 0xA0);
            dumb.Add('1', 0x01);
            dumb.Add('2', 0xCA);
            dumb.Add('3', 0x45);
            dumb.Add('4', 0x77);
            dumb.Add('5', 0x63);
            dumb.Add('6', 0x61);
            dumb.Add('7', 0x95);
            dumb.Add('8', 0x48);
            dumb.Add('9', 0x2C);
        }
    }
}
