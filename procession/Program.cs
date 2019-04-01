using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace procession
{
    // 自定义的异常类，用于定位运算出错点
    public class ExpressionErrorException : Exception
    {
        public int index;       // 出错位置
        public string message;      // 出错信息
        public ExpressionErrorException(string message, int index)
        {
            this.message = message;
            this.index = index;
        }
    }
    // 带头单链表的节点
    public class Node
    {
        public double num;    // 储存底数
        public double pow;    // 储存指数
        public Node next;  // 指向下一个节点
        public Node()
        {
            num = 0;
            pow = 0;
            next = null;
        }
        // 将两个带头单链表相加并返回新的链表
        public static Node operator +(Node a, Node b)
        {
            // 将a链表完整复制到新链表中
            Node res = new Node();
            Node indexRes = res;
            Node indexA = a.next;
            while (indexA != null)
            {
                // 拷贝节点
                Node temp = new Node
                {
                    num = indexA.num,
                    pow = indexA.pow
                };
                // 在res尾端插入新节点
                indexRes.next = temp;
                indexRes = indexRes.next;

                indexA = indexA.next;
            }
            // 遍历b链表，并与res融合，新节点则插入至尾端
            Node indexB = b.next;
            while (indexB != null)
            {
                indexRes = res;
                bool inserted = false;
                while (indexRes.next != null)   // 可能涉及到插入尾部操作，必须使用next跳转
                {
                    // 找到相同项则合并并标记
                    if (indexB.pow == indexRes.next.pow)
                    {
                        indexRes.next.num += indexB.num;
                        inserted = true;
                        break;
                    }
                    indexRes = indexRes.next;
                }
                // 没相同项则直接复制到尾端新节点
                if (inserted == false)
                {
                    Node temp = new Node
                    {
                        num = indexB.num,
                        pow = indexB.pow
                    };
                    indexRes.next = temp;
                }
                indexB = indexB.next;
            }
            return res;
        }
        // 将两个带头单链表相乘并返回新的链表
        public static Node operator *(Node a, Node b)
        {
            Node res = new Node();
            // 任何一方为空，都将算式变为+并返回原式
            if (a.next == null || b.next == null)
            {
                res += a;
                res += b;
                return res;
            }
            Node indexA = a.next;
            while (indexA != null)
            {
                // 不断遍历相乘，结果再相加
                Node indexB = b.next;
                while (indexB != null)
                {
                    // 底数相乘，指数相加
                    Node temp = new Node()
                    {
                        num = indexA.num * indexB.num,
                        pow = indexA.pow + indexB.pow
                    };
                    // 给节点带个头
                    Node head = new Node
                    {
                        next = temp
                    };
                    // 插入到主链表
                    res += head;

                    indexB = indexB.next;
                }
                indexA = indexA.next;
            }
            return res;
        }
        // 将两个带头链表相减，并返回新的链表
        public static Node operator -(Node a, Node b)
        {
            Node res = new Node();
            // 将A链表原封不动复制到结果区块
            res += a;
            // 将B链表符号反转，并与结果区块相加
            Node indexB = b.next;
            while (indexB != null)
            {
                Node temp = new Node()
                {
                    num = -indexB.num,
                    pow = indexB.pow
                };
                // 给节点带个头
                Node head = new Node();
                head.next = temp;
                res += head;
                indexB = indexB.next;
            }
            return res;
        }
    }
    public class Program
    {
        // 语义分析器，根据传入的表达式，返回相应的节点
        public static Node nodeAnalyze(string expression)
        {
            // 初始化节点
            Node res = new Node();
            // 判断传入的表达式是否为空
            if (expression == "")
            {
                return res;
            }
            // 开始分析语义
            char sign = '+';      // 表达式为正为负
            bool powMode = false;       // 是否已开始读取指数部分
            bool smallMode = false;     // 是否已开始读取小数部分
            double smallNum = 0.1;       // 读取小数用的倍数
            bool numStart = false;      // 是否已开始读取数字
            bool noneNum = false;       // 是否已读取到x未知数
            double num = 0;      // 当前读取到的数字
            string exp = expression;     // 单词太长了简化一下
            for (int i = 0; i < expression.Length; i++)
            {
                // 识别表达式的正负
                if (exp[i] == '-')
                {
                    sign = '-';
                    continue;
                }
                else if (exp[i] == '+')
                {
                    sign = '+';
                    continue;
                }
                // 处理表达式的特殊符号
                if (exp[i] == '.')
                {
                    // 数字没开始就遇到点
                    if (numStart == false)
                    {
                        throw new ExpressionErrorException("整数部分缺失", i);
                    }
                    // 已经进入了小数读取模式还遇到点
                    if (smallMode == true)
                    {
                        throw new ExpressionErrorException("小数点重复", i);
                    }
                    smallMode = true;
                    numStart = false;
                    continue;
                }
                if (exp[i] == '^')
                {
                    // 没有遇到x就遇到^，则表达式错误
                    if (noneNum == false)
                    {
                        throw new ExpressionErrorException("未知数缺失", i);
                    }
                    // 表达式正常则开启指数读取模式
                    powMode = true;
                    continue;
                }
                if (exp[i] == 'x')
                {
                    // 没有数字只有x，则底数为1
                    if (numStart == false)
                    {
                        if (sign == '-')
                        {
                            sign = '+';
                            res.num = -1;
                        }
                        else
                        {
                            res.num = 1;
                        }
                    }
                    else
                    {
                        res.num = num;
                        if (sign == '-')
                        {
                            sign = '+';
                            res.num = -res.num;
                        }
                    }
                    num = 0;
                    numStart = false;
                    smallMode = false;
                    powMode = false;
                    noneNum = true;
                    continue;
                }
                // 非特殊符号，正常读取数字
                if (smallMode == false) // 处于整数读取状态
                {
                    if (numStart == false)
                    {
                        num = exp[i] - '0';
                        numStart = true;
                    }
                    else
                    {
                        num *= 10;
                        num += exp[i] - '0';
                    }
                }
                else // 处于小数读取状态
                {
                    if (numStart == false)
                    {
                        num += (exp[i] - '0') / 10.0;
                        smallNum = 0.01;
                        numStart = true;
                    }
                    else
                    {
                        num += (exp[i] - '0') * smallNum;
                        smallNum /= 10;
                    }
                }
            }
            if (powMode == true)
            {
                res.pow = num;
                if (sign == '-')
                {
                    res.pow = -res.pow;
                }
            }
            else  // 没有读到指数部分则默认指数为1
            {
                res.pow = 1;
            }
            // 没有读取到x，则默认指数为0，且将数字拷贝上去
            if (noneNum == false)
            {
                res.pow = 0;
                res.num = num;
                if (sign == '-')
                {
                    res.num = -res.num;
                }
            }
            // 带上头再返回
            Node head = new Node();
            head.next = res;
            return head;
        }
        // 表达式分割器，用递归的方式解析表达式
        public static Node expressionAnalyze(string expression)
        {
            Node res = new Node();      // 本次递归区块的运算式
            char sign = '+';            // 运算区块左侧的运算符
            int leftIndex = 0;      // 扫描所需的左右光标
            int rightIndex = 0;
            while (rightIndex < expression.Length)
            {
                // 分析当前的字符，做出不同的反应
                switch (expression[rightIndex])
                {
                    case '(':
                        // 将左光标挪至左括号，并将右光标挪至配对右括号
                        leftIndex = rightIndex;
                        int count = 0;
                        while (true)
                        {
                            if (rightIndex >= expression.Length)
                            {
                                throw new ExpressionErrorException("没有能与之配对的右括号", leftIndex + 1);
                            }
                            if (expression[rightIndex] == '(')
                                count++;
                            else if (expression[rightIndex] == ')')
                                count--;
                            if (count == 0)
                                break;
                            else
                                rightIndex++;
                        }
                        // 将括号间内容传入下一个递归并运算
                        string transExpression = expression.Substring(leftIndex + 1, rightIndex - leftIndex - 1);
                        try
                        {
                            switch (sign)
                            {
                                case '+':
                                    res += Program.expressionAnalyze(transExpression);
                                    break;
                                case '-':
                                    res -= Program.expressionAnalyze(transExpression);
                                    break;
                                case '*':
                                    res *= Program.expressionAnalyze(transExpression);
                                    break;
                                default:
                                    throw new Exception("程序内部错误");
                            }
                        }
                        catch(ExpressionErrorException e)
                        {
                            throw new ExpressionErrorException(e.message, e.index + leftIndex + 1);
                        }
                        // 左光标挪动到括号右边，右光标会自增不用挪动
                        leftIndex = rightIndex + 1;
                        break;
                    case ')':
                        // 正常情况下表达式不会遇到右括号
                        throw new ExpressionErrorException("多余的右括号", rightIndex + 1);
                    case '+':
                    case '-':
                    case '*':
                        // 遇见符号，先将光标间的表达式传入进行分析，再与主区块运算
                        if (rightIndex != 0 && expression[rightIndex - 1] == '^')
                        {
                            if (expression[rightIndex] == '*')
                            {
                                // 指数位的符号位不能为*号
                                throw new ExpressionErrorException("指数部分的符号位不能为*", rightIndex + 1);
                            }
                            // ^右边的符号当作指数符号位处理，直接忽略
                            break;
                        }
                        string nodeTrans = expression.Substring(leftIndex, rightIndex - leftIndex);
                        try
                        {
                            switch (sign)
                            {
                                case '+':
                                    res += Program.nodeAnalyze(nodeTrans);
                                    break;
                                case '-':
                                    res -= Program.nodeAnalyze(nodeTrans);
                                    break;
                                case '*':
                                    res *= Program.nodeAnalyze(nodeTrans);
                                    break;
                                default:
                                    throw new Exception("程序内部错误");
                            }
                        }
                        catch (ExpressionErrorException e)
                        {
                            throw new ExpressionErrorException(e.message, e.index + leftIndex + 1);
                        }
                        sign = expression[rightIndex];
                        // 如果当前为负，判断前一位是否也为负号，连续两负号应当反转
                        if (expression[rightIndex] == '-')
                        {
                            if (rightIndex != 0 && expression[rightIndex - 1] == '-')
                            {
                                sign = '+';
                            }
                        }
                        // 如果当前为正，判断前一位是否为负，前负则不变号
                        if (expression[rightIndex] == '+')
                        {
                            if (rightIndex != 0 && expression[rightIndex - 1] == '+')
                            {
                                sign = '-';
                            }
                        }
                        leftIndex = rightIndex + 1;
                        break;
                    default:
                        // 检测当前位是否为非法字符
                        string legalChar = "0123456789^+-*xX.";
                        if (legalChar.Contains(expression[rightIndex]) == false)
                        {
                            throw new ExpressionErrorException("无法识别此字符", rightIndex + 1);
                        }
                        break;
                }
                rightIndex++;
            }
            // 结束时，最后再将区块内的表达式分析并与主区块运算
            string transNode = expression.Substring(leftIndex, rightIndex - leftIndex);
            try
            {
                switch (sign)
                {
                    case '+':
                        res += Program.nodeAnalyze(transNode);
                        break;
                    case '-':
                        res -= Program.nodeAnalyze(transNode);
                        break;
                    case '*':
                        res *= Program.nodeAnalyze(transNode);
                        break;
                    default:
                        throw new Exception("程序内部错误");
                }
            }
            catch(ExpressionErrorException e)
            {
                throw new ExpressionErrorException(e.message, e.index + leftIndex + 1);
            }
            // 返回该区块的运算结果
            return res;
        }
        // 表达式输出器，根据输入的区块链表，自动排序并返回字符串表达式
        public static string linkToString(Node head)
        {
            // 判断区块是否为空，为空则返回空字符串
            if (head.next == null)
            {
                return "";
            }
            // 对链表进行排序，冒泡排序法
            bool flag = true;
            Node index;
            while (flag == true)
            {
                flag = false;
                Node lastIndex = head;
                index = head.next;
                while (index.next != null)
                {
                    if (index.pow < index.next.pow)
                    {
                        lastIndex.next = index.next;
                        index.next = index.next.next;
                        lastIndex.next.next = index;
                        flag = true;
                    }
                    lastIndex = lastIndex.next;
                    index = lastIndex.next;
                }
            }
            // 对链表进行字符串化
            string res = "";
            index = head.next;
            bool first = true;      // 标记是否为首项，首项不需要打+号
            for (; index != null; index = index.next)
            {
                // 底数为0没必要打印
                if (index.num == 0)
                {
                    continue;
                }
                // 打印每项前面的正负号
                if (first == false && index.num > 0)
                {
                    res += "+";
                }
                if (index.num < 0)
                {
                    res += "-";
                    index.num = -index.num;
                }
                // 底数为1且指数不为0则隐藏1
                if (index.num == 1 && index.pow != 0)
                {
                    res += "x";
                }
                // 指数为0则隐藏x
                else if (index.pow == 0)
                {
                    res += index.num + "";
                }
                else
                {
                    res += index.num + "x";
                }
                // 指数为1或0则隐藏^
                if (index.pow == 1 || index.pow == 0)
                {

                }
                else
                {
                    res += "^" + index.pow;
                }
                first = false;
            }
            return res;
        }
        static void Main(string[] args)
        {
            /******************************
             * 警告：
             * 本程序无符号优先级运算能力
             * 所有表达式均为左至右运算
             ******************************/
            // 读入表达式
            string expression = Console.ReadLine();
            // 送入表达式分析器
            try
            {
                Node res = Program.expressionAnalyze(expression);
                Console.WriteLine(Program.linkToString(res));
            }
            catch (ExpressionErrorException e)
            {
                // 前面加入n-1个空格，再加入^
                for (int i = 0; i < e.index - 1; i++)
                {
                    Console.Write(" ");
                }
                Console.WriteLine("^");
                Console.WriteLine(e.message);
            }
            // 打印结果
            Console.WriteLine();
        }
    }
}
