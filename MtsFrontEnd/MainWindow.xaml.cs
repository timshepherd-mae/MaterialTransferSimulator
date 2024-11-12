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

namespace MtsFrontEnd
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            flowRegion.Children.Add(TransferLink(cenBayA, cenBayB, "T1"));
            flowRegion.Children.Add(TransferLink(cenBayA, cenBayD, "T2"));
            flowRegion.Children.Add(TransferLink(cenBayE, cenBayD, "T3"));
            flowRegion.Children.Add(TransferLink(cenBayB, cenBayF, "T4"));
            flowRegion.Children.Add(TransferLink(cenBayF, cenBayH, "T5"));
            flowRegion.Children.Add(TransferLink(cenBayC, cenBayB, "T6"));
            flowRegion.Children.Add(TransferLink(cenBayC, cenBayG, "T7"));
            flowRegion.Children.Add(TransferLink(cenBayH, cenBayE, "T8"));

            //TransferViewGeometry(cenBayA, cenBayB, flowRegion, "t1");
            //TransferViewGeometry(cenBayE, cenBayF, flowRegion, "t2");
            //TransferViewGeometry(cenBayA, cenBayD, flowRegion, "t3");
            //TransferViewGeometry(cenBayB, cenBayF, flowRegion, "t4");
            //TransferViewGeometry(cenBayF, cenBayH, flowRegion, "t5");
            //TransferViewGeometry(cenBayC, cenBayB, flowRegion, "t6");

        }




        public static Point cenBayA = new Point(90, 70);
        public static Point cenBayB = new Point(220, 160);
        public static Point cenBayC = new Point(350, 70);
        public static Point cenBayD = new Point(480, 160);
        public static Point cenBayE = new Point(90, 250);
        public static Point cenBayF = new Point(220, 340);
        public static Point cenBayG = new Point(350, 250);
        public static Point cenBayH = new Point(480, 340);


 
    }


}
