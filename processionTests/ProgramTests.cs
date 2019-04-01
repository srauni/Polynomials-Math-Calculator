using Microsoft.VisualStudio.TestTools.UnitTesting;
using procession;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace procession.Tests
{
    [TestClass()]
    public class ProgramTests
    {
        [TestMethod()]
        public void nodeAnalyzeTest()
        {
            Node test = Program.nodeAnalyze("-12.3x^-6");
            Assert.IsTrue(test.next.num == -12.3);
            Assert.IsTrue(test.next.pow == -6.0);
            Node test2 = Program.nodeAnalyze("13x");
            Assert.IsTrue(test2.next.num == 13.0);
            Assert.IsTrue(test2.next.pow == 1.0);
            Node test3 = Program.nodeAnalyze("-20");
            Assert.IsTrue(test3.next.num == -20.0);
            Assert.IsTrue(test3.next.pow == 0.0);
            Node test4 = Program.nodeAnalyze("+17.6");
            Assert.IsTrue(test4.next.num == 17.6);
            Assert.IsTrue(test4.next.pow == 0.0);
            Node test5 = Program.nodeAnalyze("0.2x^0.003");
            Assert.IsTrue(test5.next.num == 0.2);
            Assert.IsTrue(test5.next.pow == 0.003);
            Node test6 = Program.nodeAnalyze("x");
            Assert.IsTrue(test6.next.num == 1.0);
            Assert.IsTrue(test6.next.pow == 1.0);
        }

        [TestMethod()]
        public void expressionAnalyzeTest()
        {
            Node test1 = Program.expressionAnalyze("-(x+x)+x-(+x)");
            Assert.IsTrue(test1.next.num == -2.0);
            Assert.IsTrue(test1.next.pow == 1);
            Node test2 = Program.expressionAnalyze("2x+5x^8-3.1x^11");
            Assert.IsTrue(Program.linkToString(test2) == "-3.1x^11+5x^8+2x");
            Node test3 = Program.expressionAnalyze("7-5x^8+11x^9");
            Assert.IsTrue(Program.linkToString(test3) == "11x^9-5x^8+7");
        }

        [TestMethod()]
        public void linkToStringTest()
        {
            string test1 = Program.linkToString(Program.expressionAnalyze("(2x+5x^8-3.1x^11)+(7-5x^8+11x^9)"));
            Assert.IsTrue(test1.Equals("-3.1x^11+11x^9+2x+7"));
        }

        [TestMethod()]
        public void linkToStringTest1()
        {
            string test2 = Program.linkToString(Program.expressionAnalyze("(6x^-3-x+4.4x^2-1.2x^9)-(-6x^-3+5.4x^2+7.8x^15)"));
            Assert.IsTrue(test2.Equals("-7.8x^15-1.2x^9-x^2-x+12x^-3"));
        }

        [TestMethod()]
        public void linkToStringTest2()
        {
            string test3 = Program.linkToString(Program.expressionAnalyze("(x+x^2+x^3)+0"));
            Assert.IsTrue(test3.Equals("x^3+x^2+x"));
        }

        [TestMethod()]
        public void linkToStringTest3()
        {
            string test4 = Program.linkToString(Program.expressionAnalyze("(x+x^3)-(-x-x^-3)"));
            Assert.IsTrue(test4.Equals("x^3+2x+x^-3"));
        }

        [TestMethod()]
        public void linkToStringTest4()
        {
            string test = Program.linkToString(Program.expressionAnalyze("(2x^2+1.2x^4)*(16+2.2x^3)"));
            Assert.IsTrue(test.Equals("2.64x^7+4.4x^5+19.2x^4+32x^2"));
        }
    }
}