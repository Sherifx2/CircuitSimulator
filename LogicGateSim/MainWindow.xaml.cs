using LogicGateSim.AbstractClasses;
using LogicGateSim.Gates;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LogicGateSim
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Rectangle andGateGUI;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AndGate andGate = new AndGate(2, 1);

            andGateGUI = new Rectangle
            {
                Width = 48,
                Height = 24,
                Fill = Brushes.Blue,
                Stroke = Brushes.Black,
            };
            andGateGUI.MouseMove += electricalComponent_MouseMove;

            placementMat.Children.Add(andGateGUI);
        }
        private void electricalComponent_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(this.andGateGUI, new DataObject(this.andGateGUI), DragDropEffects.Move);
            }
        }
        private void placementMat_DragOver(object sender, DragEventArgs e)
        {
            double cellWidth = 12;
            double cellHeight = 12;

            Point dropPosition = e.GetPosition(placementMat);

            double snappedX = Math.Floor(dropPosition.X / cellWidth) * cellWidth;
            double snappedY = Math.Floor(dropPosition.Y / cellHeight) * cellHeight;

            Canvas.SetLeft(andGateGUI, snappedX);
            Canvas.SetTop(andGateGUI, snappedY);

        }
        private void placementMat_Drop(object sender, DragEventArgs e)
        {

        }
    }
}