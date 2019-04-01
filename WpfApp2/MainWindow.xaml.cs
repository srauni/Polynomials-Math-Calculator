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
using procession;

namespace WpfApp2
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // 清除掉默认的文字
            lab.Content = "";
            text.Text = "";
            errorLab.Content = "";
            // 默认勾选指数位处理
            check.IsChecked = true;
        }

        // 将普通的指数转化为上升的指数
        public string powToUp(string expression)
        {
            string powChar = "⁰¹²³⁴⁵⁶⁷⁸⁹˙⁺⁻";
            string res = "";
            bool powMode = false;       // 标记当前是否为上标模式
            for (int i = 0; i < expression.Length; i++)
            {
                if (expression[i] >= '0' && expression[i] <= '9')
                {
                    if (powMode == true)
                    {
                        res += powChar[expression[i] - '0'];
                    }
                    else
                    {
                        res += expression[i];
                    }
                }
                else if (expression[i] == '.')
                {
                    if (powMode == true)
                    {
                        res += "˙";
                    }
                    else
                    {
                        res += ".";
                    }
                }
                else if (expression[i] == '^')
                {
                    powMode = true;
                }
                else if (expression[i] == '+' || expression[i] == '-' || expression[i] == '*')
                {
                    if (i != 0 && expression[i - 1] != '^')
                    {
                        powMode = false;
                        res += expression[i];
                    }
                    else if (powMode == true)
                    {
                        switch (expression[i])
                        {
                            case '+':
                                res += '⁺';
                                break;
                            case '-':
                                res += '⁻';
                                break;
                        }
                    }
                    else
                    {
                        res += expression[i];
                    }
                }
                else
                {
                    res += expression[i];
                    powMode = false;
                }
            }
            return res;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string res;
            errorLab.Content = "";
            try
            {
                res = Program.linkToString(Program.expressionAnalyze(text.Text));
            }
            catch (ExpressionErrorException e1)
            {
                // 打印出错提示
                errorLab.Content = "";
                // 前面加入N-1个空格
                for (int i = 0; i < e1.index - 1; i++)
                {
                    errorLab.Content += " ";
                }
                errorLab.Content += "^";
                lab.Content = "";
                lab.Content += e1.message;
                return;
            }
            lab.Content = "";
            if (check.IsChecked == true)
            {
                //text.Text = powToUp(text.Text);
                lab.Content = powToUp(res);
            }
            else
            {
                lab.Content = res;
            }
            if ((string)lab.Content == "")
            {
                lab.Content = "0";
            }
        }
    }
}
