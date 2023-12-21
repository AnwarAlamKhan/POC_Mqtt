using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Configuration;

namespace Application2
{
    public partial class RabbitQueueForm : Form
    {
        private IConnection _rabbitMqConnection;
        private IModel _emailChannel;

        public RabbitQueueForm()
        {
            InitializeComponent();
        }

        private void RabbitQueueForm_Load(object sender, EventArgs e)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["RabbitMqConnection"].ConnectionString;
            var connectionFactory = new ConnectionFactory();
            connectionFactory.Uri = new Uri(connectionString);
            // connectionFactory.AutomaticRecoveryEnabled = true;
            //connectionFactory.DispatchConsumersAsync = false;
            _rabbitMqConnection = connectionFactory.CreateConnection();

            _emailChannel = _rabbitMqConnection.CreateModel();

            _emailChannel.ExchangeDeclare(exchange: "topic", type: ExchangeType.Topic, true);

            var queueName = "QueueApplication2";//_emailChannel.QueueDeclare().QueueName;

            _emailChannel.QueueBind(queue: queueName, exchange: "amq.topic", routingKey: "Application1.Message");


            var consumer = new EventingBasicConsumer(_emailChannel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                lstEmail.Invoke((MethodInvoker)(() =>
                {
                    lstEmail.Items.Add($"Topic1 - Recieved new message: {message}");
                    lstEmail.SelectedIndex = lstEmail.Items.Count - 1;
                }
                ));


            };
            this.lstEmail.SelectedIndex = this.lstEmail.Items.Count - 1;
            _emailChannel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

        }
    }
}
