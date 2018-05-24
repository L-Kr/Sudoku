using System;
using System.Drawing;
using System.Windows.Forms;

namespace 数独
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        TextBox[,] Tb = new TextBox[9,9];
        int[,] Num = new int[9, 9];

        private void Form1_Load(object sender, EventArgs e)
        {
            Input();
        }

        /*点击查看答案按钮*/
        private void button1_Click(object sender, EventArgs e)
        {
            if (Anwser(0, 0))
                for (int i = 0; i < 9; i++)
                    for (int j = 0; j < 9; j++)
                        Tb[i, j].Text = Num[i,j].ToString();   //将正确结果显示到文本框中
            else
                MessageBox.Show("无解？");
            button1.Enabled = false;   //都得出答案了还按什么按钮，所以直接取消按钮效应
            button2.Enabled = false;   //如上所说一样
        }

        /*点击提交答案按钮*/
        private void button2_Click(object sender, EventArgs e)
        {
            foreach (TextBox t in Tb)
                if (t.Text == "")
                {
                    MessageBox.Show("你做错了！");
                    return;
                }
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    Num[i, j] = Convert.ToInt32(Tb[i, j].Text);
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    if (!Nengyong(i, j, Num[i, j]))
                    {
                        MessageBox.Show("你做错了！");
                        return;
                    }
            MessageBox.Show("恭喜你做对了！");
        }

        public void Input()
        {
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = true;
            /*实例化文本框*/
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                    Tb[i, j] = new TextBox();
            }
            /*设置文本框属性*/
            foreach (TextBox t in Tb)
            {
                t.Size = new Size(38, 38);  //文本框大小为38*38
                t.Font = new Font("宋体", 20F);  //文本框文字
                t.MaxLength = 1;   //只允许输入一位
                Controls.Add(t);  //添加文本框控件到窗体
            }
            Tb[0, 0].Location = new Point(12, 12);  //设置第一个文本框的坐标方便之后的文本框坐标的确定
            Tb[0, 0].Name = "0,0";  //设置第一个文本框的控件名字
            for (int i = 1; i < 9; i++)   //先确定第一行文本框坐标，便于之后文本框以此为基准确定坐标
            {
                Tb[i, 0].Name = i.ToString() + ",0";
                Tb[i, 0].Location = new Point(Tb[i - 1, 0].Location.X + 40, 12);
            }
            for (int i = 0; i < 9; i++)  //确定剩下的所有文本框的位置及控件名字
            {
                for (int j = 1; j < 9; j++)
                {
                    Tb[i, j].Name = i.ToString() + "," + j.ToString();
                    Tb[i, j].Location = new Point(Tb[i, 0].Location.X, Tb[i, j - 1].Location.Y + 40);  //该文本框的位置为横坐标与第一行对应，纵坐标为上一行+40（因为文本框边长是38再留2的空隙）
                }
            }
        }

        /*画两条横线两条竖线用于区分九宫格*/
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics grp = CreateGraphics();    //创建一个Graphics对象画两条横线两条竖线，区分出9个九宫格
            grp.DrawLine(new Pen(Color.Black), new Point(131, 12), new Point(131, 370));   //从坐标(131,12)到(131,370)画一根竖线
            grp.DrawLine(new Pen(Color.Black), new Point(251, 12), new Point(251, 370));   //从坐标(251, 12)到(251, 370)画一根竖线
            grp.DrawLine(new Pen(Color.Black), new Point(12, 131), new Point(370, 131));   //从坐标(12, 131)到(370, 131)画一根横线
            grp.DrawLine(new Pen(Color.Black), new Point(12, 251), new Point(370, 251));   //从坐标(12, 251)到(370, 251)画一根横线
            grp.Dispose();    //释放资源
        }

        /*使用回溯法递归求出正确结果*/
        public bool Anwser(int a, int b)
        {
            if (Num[a,b]!=0)   //如果递归到的位置已给出数字则跳过该位置（由于是按照一定顺序递归所以当其值不为0时则可判断其为已知数）
            {
                if (a == 8)   //如果遍历到矩阵最右边（某一行最后一个）
                {
                    if (b == 8)  //如果遍历到矩阵最右边且最下边则可判断已经遍历结束（最后一行最后一个）
                        return true;
                    else
                        return Anwser(0, b + 1);  //如果遍历到矩阵最右边且不是最下边则遍历下一行第一个
                }
                else
                    return Anwser(a + 1, b);  //如果没有遍历到最右边则遍历该行的右边那个位置
            }
            else
            {
                do
                {
                    Num[a, b]++;  //先对该位置自身+1
                    if (Nengyong(a, b, Num[a, b]))  //如果坐标(a,b)的该值满足条件则判断（下列代码与141行到151行原理相同）
                    {
                        if (a == 8)
                        {
                            if (b == 8)
                                return true;
                            else if (Anwser(0, b + 1))
                                return true;                                
                        }
                        else if (Anwser(a + 1, b))
                            return true;
                    }
                } while (Num[a,b]<9);  //该位置的数不能大于9，由于是先+1再判断所以是<9时继续循环
            }
            Num[a, b] = 0;   //如果没有满足条件的值需要回溯，则将该位置的值重置为0
            return false ;
        }

        /*判断坐标为(a,b)的值c是否满足数独条件*/
        public bool Nengyong(int a, int b, int c)
        {
            for (int i = 0; i < 9; i++)  //判断在坐标(a,b)填入c后同一横行是否存在重复
                if (Num[a, i] == c && i != b)
                    return false;
            for (int i = 0; i < 9; i++)    //判断在坐标(a,b)填入c后同一纵行是否存在重复
                if (Num[i, b] == c && i != a)
                    return false;
            for (int i = 0; i < 9; i++)     //判断在坐标(a,b)填入c后同一九宫格内是否存在重复
                for (int j = 0; j < 9; j++)
                    if (i / 3 == a / 3 && j / 3 == b / 3)
                        if (i != a && j != b)
                            if (Num[i, j] == c)
                                return false;
            return true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = false;
            button4.Enabled = false;
            for(int i=0;i<9;i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (Tb[i, j].Text == "")
                        Num[i, j] = 0;
                    else
                    {
                        Num[i, j] = Convert.ToInt32(Tb[i, j].Text);
                        Tb[i, j].Enabled = false;
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Random r = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                {
                    Num[i, j] = 0;
                    Tb[i, j].Enabled = false;
                }
            Num[0, 0] = r.Next(10);
            Num[1, 3] = r.Next(10);
            Num[2, 6] = r.Next(10);
            Num[3, 1] = r.Next(10);
            Num[4, 4] = r.Next(10);
            Num[5, 7] = r.Next(10);
            Num[6, 2] = r.Next(10);
            Num[7, 5] = r.Next(10);
            Num[8, 8] = r.Next(10);
            if (Anwser(0, 0))
            {
                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = false;
                button4.Enabled = false;
                comboBox1.Enabled = false;
                for (int i = 0; i < Convert.ToInt32(comboBox1.Text); i++)
                {
                    int a, b;
                    do
                    {
                        a = r.Next(9);
                        b = r.Next(9);
                    } while (Num[a, b] == 0);
                    Num[a, b] = 0;
                    Tb[a, b].Enabled = true;
                }
                for (int i = 0; i < 9; i++)
                    for (int j = 0; j < 9; j++)
                        if (Num[i, j] != 0)
                            Tb[i, j].Text = Num[i, j].ToString();
            }
            else
                MessageBox.Show("题目生成错误?");
            Anwser(0, 0);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int a, b;
            Random r = new Random();
            bool flag = false;
            foreach (TextBox t in Tb)
                if (t.Enabled == true)
                    flag = true;
            if(!flag)
            {
                MessageBox.Show("全都告诉你了你还想怎样？");
                return;
            }
            do
            {
                a = r.Next(9);
                b = r.Next(9);
            } while (Tb[a, b].Enabled == false);
            Tb[a, b].Enabled = false;
            Tb[a, b].Text = Num[a, b].ToString();
        }
    }
}
