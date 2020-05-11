using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Automation.Text;
using System.Threading;
using CommonUtil;

//https://www.cnblogs.com/baihuitestsoftware/articles/9047705.html
//UIAUTOMATION参考文档
namespace FindFirefoxAuthenticationRequired
{
    class Program
    {
        static void Main(string[] args)
        {

            string mainmodulepath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            FileInfo currentFileInfo = new FileInfo(mainmodulepath);
            string currenpath = currentFileInfo.DirectoryName;

            string logfile = currenpath + @"\log\FindFirefoxAuthenticationRequired.log";
            StreamWriter log = new StreamWriter(logfile, false);
            log.WriteLine("mainmodulepath path is {0}", mainmodulepath);
            log.WriteLine("current path is {0}", currenpath);
            string inipath = currenpath+ @"\config.ini";
            log.WriteLine("ini path is {0}", inipath);
            IniFile ini = new IniFile(inipath);  
           
            string username=ini.IniReadValue("setting ", "username");
            string password = ini.IniReadValue("setting ", "password");


            AutomationElement desktop = AutomationElement.RootElement;
            // MessageBox.Show(ae.Current.NativeWindowHandle.ToString());
            StringBuilder sb = new StringBuilder();
            // AutomationElementCollection topWindows = desktop.FindAll(TreeScope.Children, Condition.TrueCondition); //查询所有子元素
            //AutomationElementCollection topWindows = desktop.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "CalcFrame"));//查找计算器
            Condition conditions = new AndCondition(
            new PropertyCondition(AutomationElement.IsEnabledProperty, true),
            new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window),
            new PropertyCondition(AutomationElement.NameProperty, "Mozilla Firefox"),
            new PropertyCondition(AutomationElement.ClassNameProperty, "MozillaWindowClass")
            );

            // Find all children that match the specified conditions.
            AutomationElementCollection topWindows = desktop.FindAll(TreeScope.Children, conditions);
            


            for (int i = 0; i < topWindows.Count; i++)
            {
                AutomationElement topWindow = topWindows[i];
                sb.AppendLine("Name:" + topWindow.Current.Name + ";ClassName=" + topWindow.Current.ClassName);
                Condition firefoxAuthenticationConditions = new AndCondition(         
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window),
                new PropertyCondition(AutomationElement.NameProperty, "Authentication Required - Mozilla Firefox"),
                new PropertyCondition(AutomationElement.ClassNameProperty, "MozillaDialogClass")
                );
                AutomationElement firefoxAuthenticationWindow = topWindow.FindFirst(TreeScope.Children, firefoxAuthenticationConditions);


                Condition firefoxAuthenticationUserNameConditions = new AndCondition(
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit),
                new PropertyCondition(AutomationElement.NameProperty, "User Name:"),
                new PropertyCondition(AutomationElement.ClassNameProperty, "")
                );
                AutomationElement firefoxAuthenticationUserNameEdit = firefoxAuthenticationWindow.FindFirst(TreeScope.Subtree, firefoxAuthenticationUserNameConditions);
                ValuePattern valuePatternirefoxAuthenticationUserNameEdit = (ValuePattern)firefoxAuthenticationUserNameEdit.GetCurrentPattern(ValuePattern.Pattern);
                valuePatternirefoxAuthenticationUserNameEdit.SetValue(username);

                Thread.Sleep(100);
                Condition firefoxAuthenticationPasswordConditions = new AndCondition(
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit),
                new PropertyCondition(AutomationElement.NameProperty, "Password:"),
                new PropertyCondition(AutomationElement.ClassNameProperty, "")
                );
                AutomationElement firefoxAuthenticationPasswordEdit = firefoxAuthenticationWindow.FindFirst(TreeScope.Subtree, firefoxAuthenticationPasswordConditions);
                ValuePattern valuePatternfirefoxAuthenticationPasswordEdit = (ValuePattern)firefoxAuthenticationPasswordEdit.GetCurrentPattern(ValuePattern.Pattern);
                valuePatternfirefoxAuthenticationPasswordEdit.SetValue(password);
                log.WriteLine("password set finish");
                log.Flush();
                var passwordtest=valuePatternfirefoxAuthenticationPasswordEdit.Current.Value;

                Thread.Sleep(100);
                Condition firefoxAuthenticationOKBTNConditions = new AndCondition(
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button),
                new PropertyCondition(AutomationElement.NameProperty, "OK"),
                new PropertyCondition(AutomationElement.ClassNameProperty, "")
                );
                AutomationElement firefoxAuthenticationOKBTN = firefoxAuthenticationWindow.FindFirst(TreeScope.Subtree, firefoxAuthenticationOKBTNConditions);
                log.WriteLine(@"OK button Name is {0}", firefoxAuthenticationOKBTN.Current.Name);
                var invokePatternfirefoxAuthenticationOKBTN = (InvokePattern)firefoxAuthenticationOKBTN.GetCurrentPattern(InvokePattern.Pattern);           
                log.WriteLine(@"Invoke OK button");
                invokePatternfirefoxAuthenticationOKBTN.Invoke();
               

                log.Close();
      


            }
        }
    }
}
