import pika
from apscheduler.schedulers.blocking import BlockingScheduler
import pyownet
import json
import logging
import datetime
import time
from pygelf import GelfTcpHandler, GelfUdpHandler, GelfTlsHandler, GelfHttpHandler


QUEUE = 'jms.queue.TemperatureInput'

with open('config.json') as json_data_file:
    cfg = json.load(json_data_file)

def refresh():
    #connection =
    #pika.BlockingConnection(pika.ConnectionParameters('172.17.0.1$
        logger.info(cfg["Queue"]["server"])
        credentials = pika.PlainCredentials('guest','guest')
        connection = pika.BlockingConnection(pika.ConnectionParameters('rabbitmq'))
        channel = connection.channel()
        #channel.queue_declare(queue = cfg["Queue"]["queueName"])
        owproxy = pyownet.protocol.proxy(host = "192.168.1.42", port = 4304)
        #owproxy = pyownet.protocol.proxy(host="192.168.1.15", port=4304)
        logger.info("beggining device listing")
        devices = owproxy.dir()
        for device in devices:
                if device.find('/28') > -1:
                        try:
                                dev = {'MessageType':'ProbeTemperatureMessage'}
                                dev['ProbeAddress'] = device
                                dev['MessageDate'] = datetime.datetime.now().isoformat()
                                dev['TemperatureValue'] = owproxy.read(device + 'temperature')
                                logger.info(datetime.datetime.now().isoformat() + ":Temp:" + dev['TemperatureValue'] + "@" + dev['ProbeAddress'])
                                channel.basic_publish(exchange="InboundMessages",routing_key="device.temperature",body=json.dumps(dev))
                        except :
                            logger.debug("Oops!  That was no valid number.  Try again...")
                else:
                    logger.debug("Device type not recogniuzed :" + device)
        logger.info("Ending listing")
        connection.close()

logging.basicConfig()
logger = logging.getLogger()
logger.setLevel(level=logging.INFO)
logger.addHandler(GelfUdpHandler(host='graylog', _app_name='temperature', port=12201))

logger.info("Starting temperature ")
time.sleep(15)

logger.debug("First sleep finished")
refresh()
scheduler = BlockingScheduler()
scheduler.add_job(refresh, 'interval', seconds=cfg["Timeout"])
scheduler.start()

