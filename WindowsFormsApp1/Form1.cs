using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Discovery;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Create a ServiceHost for the CalculatorService type.  
            using (ServiceHost serviceHost = new ServiceHost(typeof(CalculatorService)))
            {
                serviceHost.Description.Behaviors.Add(new ServiceDiscoveryBehavior());
                serviceHost.AddServiceEndpoint(new UdpDiscoveryEndpoint());
                serviceHost.AddServiceEndpoint(typeof(ICalculator), new BasicHttpBinding(), new Uri("http://localhost:8090/CalculatorService"));
                serviceHost.Open();
                Console.WriteLine("Discoverable Calculator Service is running ...");
                //Console.ReadKey();
            }
        }
    }

    public class CalculatorService : ICalculator
    {
        int ICalculator.Add(int a, int b) => Add(a, b);

        protected int Add(int a, int b) => a + b;
    }

    [ServiceContract]
    public interface ICalculator
    {
        [OperationContract]
        int Add(int a, int b);
    }

}
