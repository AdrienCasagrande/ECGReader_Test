using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Collections;
using System.Windows;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;

namespace Test_novacor
{

    public class MyViewModel
    {
        private ArrayList sig1;
        private ArrayList sig2;

        public ICommand loadFile { get; private set; }

        public PlotModel Model1 { get; private set; }
        public PlotModel Model2 { get; private set; }
        public PlotController Controller { get; private set; }

        public MyViewModel()
        {
            var customController = new PlotController();
            customController.UnbindMouseWheel();
            
            this.Controller = customController;

            loadFile = new RelayCommand(() => datRead(), () => true);

            Model1 = new PlotModel();
            Model2 = new PlotModel();
            var tmp1 = new PlotModel { Title = "First series" };
            var tmp2 = new PlotModel { Title = "Second series" };
            var series1 = new LineSeries { MarkerType = MarkerType.None };
            var series2 = new LineSeries { MarkerType = MarkerType.None };
            for (int i = 0; i < 650000; i++)
            {
                series1.Points.Add(new DataPoint(i, 0));
                series2.Points.Add(new DataPoint(i, 0));
            }

            tmp1.Series.Add(series1);
            tmp2.Series.Add(series2);

            tmp1.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                AbsoluteMinimum = 0,
                Minimum = 0,
                AbsoluteMaximum = 650000,
                Maximum = 650000,
               
            });
            tmp2.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                AbsoluteMinimum = 0,
                Minimum = 0,
                AbsoluteMaximum = 650000,
                Maximum = 650000,
            });

            this.Model1 = tmp1;
            this.Model2 = tmp2;
        }

        private void createPlot()
        {
            var series1 = new LineSeries { MarkerType = MarkerType.None };
            var series2 = new LineSeries { MarkerType = MarkerType.None };

            for (int i = 0; i < sig1.Count; i++)
            {
                series1.Points.Add(new DataPoint(i, (int)sig1[i]));
                series2.Points.Add(new DataPoint(i, (int)sig2[i]));
            }
            this.Model1.Series.Clear();
            this.Model1.Series.Add(series1);
            this.Model2.Series.Clear();
            this.Model2.Series.Add(series2);
            this.Model1.InvalidatePlot(true);
            this.Model2.InvalidatePlot(true);
        }

        private void datRead()
        {
            Stream s = null;
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string path = dlg.FileName;

                try
                {
                    if ((s = dlg.OpenFile()) != null)
                    {
                        using (BinaryReader reader = new BinaryReader(s))
                        {
                            Byte[] data = reader.ReadBytes((int)reader.BaseStream.Length);
                            sig1 = new ArrayList();
                            sig2 = new ArrayList();

                            for (int i = 0; i < data.Length - 2; i += 3)
                            {
                                sig1.Add(((data[i + 1] & 0x0F) << 8) | data[i]);
                                sig2.Add(((data[i + 1] >> 4) << 8) | data[i + 2]);
                            }
                        }
                        createPlot();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not read file from disk: " + ex.Message);
                }
            }
        }

        //private void printByte(Byte b) {
        //    Console.WriteLine(Convert.ToString(b, 2).PadLeft(8, '0'));
        //}
    }
}
