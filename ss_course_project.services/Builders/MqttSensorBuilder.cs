﻿/*****************************************************************************/

/* FIXME List:
 Consider Change:
 * CC6 - Add Subscription Manager
 * CC7 - Unsubscribe on disposal
 */

/*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*****************************************************************************/

using System.Net.Mqtt;

/*****************************************************************************/

using ss_course_project.services.Model;

/*****************************************************************************/

namespace ss_course_project.services.Settings
{
    public class MqttSensorBuilder
    {
        public MqttSensorBuilder(Repositories.ConnectionRepo connections)
        {
            m_connections = connections;
        }

        public async Task<MqttDoubleSensor> Build(MqttSensorSetting settings)
        {
            IMqttClient client = m_connections.GetClient(settings.ConnectionId);
            MqttDoubleSensor sensor = new MqttDoubleSensor(settings.Id, settings.Topic, settings.Units);
            sensor.FriendlyName = settings.FriendlyName;

            // FIXME: CC6
            await client.SubscribeAsync(settings.Topic, settings.QosLevel);

            // FIXME: CC7
            client.MessageStream.Subscribe(sensor);

            return sensor;
        }

        Repositories.ConnectionRepo m_connections;
    }
}

/*****************************************************************************/
