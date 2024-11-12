using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace MtsFrontEnd
{
    public partial class MainWindow : Window
    {

        Polygon TransferLink(Point S, Point E, string name)
        {
            // define link dimensions
            double linkWid = 3; // width of link line
            double arrowPos = 0.55; // position of arrow base as a ratio of full length
            double arrowLen = 15; // arrow length
            double arrowWid = 8; // arrow width at widest part
            
            // define the directional and perpendicular unit vectors
            Vector V = new Vector(E.X - S.X, E.Y - S.Y);
            Vector P = new Vector(-V.Y, V.X);

            double L = V.Length; // assign length before normalising
            V.Normalize();
            P.Normalize();

            // define the polygon points
            // sL/sR are link start points Left and Right
            Point sL = S + (P * linkWid * 0.5);
            Point sR = S - (P * linkWid * 0.5);

            // bL/bR are arrow base points on link line
            Point bL = S + (V * L * arrowPos) + (P * linkWid * 0.5);
            Point bR = S + (V * L * arrowPos) - (P * linkWid * 0.5);

            // cL/cR are arrow corners out from base
            Point cL = S + (V * L * arrowPos) + (P * arrowWid);
            Point cR = S + (V * L * arrowPos) - (P * arrowWid);

            // hL/hR are arrow head points (rejoining link line
            Point hL = S + (V * ((L * arrowPos) + arrowLen)) + (P * linkWid * 0.5);
            Point hR = S + (V * ((L * arrowPos) + arrowLen)) - (P * linkWid * 0.5);

            // eL/eR are link end points
            Point eL =E + (P * linkWid * 0.5);
            Point eR = E - (P * linkWid * 0.5);

            // define polygon points collection
            PointCollection pColl = new PointCollection();

            pColl.Add(sL); pColl.Add(bL); pColl.Add(cL); pColl.Add(hL); pColl.Add(eL);
            pColl.Add(eR); pColl.Add(hR); pColl.Add(cR); pColl.Add(bR); pColl.Add(sR);

            Polygon pReturn = new Polygon()
            {
                Points = pColl,
                Fill = System.Windows.Media.Brushes.White,
                Stroke = System.Windows.Media.Brushes.Black,
                StrokeThickness = 1,
                Name = name,
                ToolTip = name + "\n40>50*>50"

            };

            pReturn.MouseUp += new MouseButtonEventHandler(PGon_MouseUp);


            return pReturn;

        }

        void TransferViewGeometry(Point S, Point E, Canvas canvas, String name)
        {
            Line line;

            // define arrowhead size and location along line
            int sizeArrowWidth = 5;
            int sizeArrowLengthRatio = 3;
            int arrowBuffer = 5;
            int arrowSetback = 50;

            // draw the linking line
            line = DrawLine(S, E);
            canvas.Children.Add(line);

            // get vector, perp vector and length of link line
            Vector V = new Vector(E.X - S.X, E.Y - S.Y);
            Vector P = new Vector(-V.Y, V.X);

            double L = V.Length;
            V.Normalize();
            P.Normalize();

            // get the arrow tip point of link line
            Point M = new Point();
            M = E - (V * arrowSetback);

            // calculate the arrow points
            Point A = new Point();
            Point B = new Point();

            A = M - (V * sizeArrowLengthRatio * sizeArrowWidth) + (P * sizeArrowWidth);
            B = M - (V * sizeArrowLengthRatio * sizeArrowWidth) - (P * sizeArrowWidth);

            /*
                        line = DrawLine(A, M);
                        flowRegion.Children.Add(line);

                        line = DrawLine(B, M);
                        flowRegion.Children.Add(line);
            */

            // visible arrow
            PointCollection pCol = new PointCollection { A, B, M };
            Polygon pGon = new Polygon()
            {
                Points = pCol,
                Fill = System.Windows.Media.Brushes.Black
            };

            // pGon.MouseUp += new MouseButtonEventHandler(pGon_MouseUp);


            canvas.Children.Add(pGon);

            // selectable arrow
            M = E - (V * (arrowSetback - arrowBuffer));
            A = M - (V * ((sizeArrowLengthRatio * sizeArrowWidth) + (2 * arrowBuffer)) + (P * (sizeArrowWidth + arrowBuffer)));
            B = M - (V * ((sizeArrowLengthRatio * sizeArrowWidth) + (2 * arrowBuffer)) - (P * (sizeArrowWidth + arrowBuffer)));


            pCol = new PointCollection { A, B, M };
            pGon = new Polygon()
            {
                Points = pCol,
                Fill = System.Windows.Media.Brushes.Transparent,
                Stroke = System.Windows.Media.Brushes.Transparent,
                StrokeThickness = 1,
                Name = name,
                ToolTip = name + "\n40>50*>50"
            };

            pGon.MouseUp += new MouseButtonEventHandler(PGon_MouseUp);


            canvas.Children.Add(pGon);



        }

        private void PGon_MouseEnter(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void PGon_MouseUp(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        Line DrawLine(Point S, Point E)
        {
            Line line = new Line
            {
                Stroke = System.Windows.Media.Brushes.Black,
                X1 = S.X,
                Y1 = S.Y,
                X2 = E.X,
                Y2 = E.Y,
                StrokeThickness = 1
            };

            return line;

        }

        void PGon_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Polygon p)
            {
                Console.WriteLine(p.Name);
            }

        }



    }
}
