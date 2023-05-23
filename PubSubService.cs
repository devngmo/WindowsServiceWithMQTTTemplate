using NLog;
using System;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace WindowsServiceWithMQTTTemplate
{
    public class PubSubService
    {
        MqttClient client;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        bool _isConnected = false;
        internal bool isConnected => _isConnected;

        internal async Task initAsync(String mqttBrokerIP, int mqttBrokerPort)
        {
            logger.Info($"PubSubService::initAsync connect to MQTT host={mqttBrokerIP} port={mqttBrokerPort}...");
            client = new MqttClient(mqttBrokerIP);
            client.Connect("TheAwesomeService");
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            client.Subscribe(new string[] { "topic1" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            client.Subscribe(new string[] { "topic2" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            client.Subscribe(new string[] { "ping" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });


            logger.Info($"PubSubService::initAsync MQTT connected");
            _isConnected = true;
        }

        private void Client_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            string message = Encoding.UTF8.GetString(e.Message);
            logger.Info($"PubSubService::onReceived: topic={e.Topic} msg={message}");
        }

        internal async Task pingAsync()
        {
            string topic = "ping";
            string message = DateTime.Now.ToString();
            logger.Info($"PubSubService::ping to MQTT: message={message}");
            try
            {
                client.Publish(topic, Encoding.UTF8.GetBytes(message), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);

                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        internal async Task StopAsync()
        {
            client.Disconnect();
        }
    }
}
