import pika
from apscheduler.schedulers.blocking import BlockingScheduler
import pyownet
import json
import logging
import datetime
import time

QUEUE = 'jms.queue.TemperatureInput'

with open('config.json') as json_data_file:
    cfg = json.load(json_data_file)

def refresh():
        logging.basicConfig()
        logging.getLogger().setLevel(logging.WARNING)
    #connection =
    #pika.BlockingConnection(pika.ConnectionParameters('172.17.0.1$
        print cfg["Queue"]["server"]
        credentials = pika.PlainCredentials('guest','guest')
        connection = pika.BlockingConnection('rabbitmq')
        channel = connection.channel()
        #channel.queue_declare(queue = cfg["Queue"]["queueName"])
        owproxy = pyownet.protocol.proxy(host = "192.168.1.42", port = 4304)
        #owproxy = pyownet.protocol.proxy(host="192.168.1.15", port=4304)
        print("beggining listing")
        devices = owproxy.dir()
        for device in devices:
                if device.find('/28') > -1:
                        try:
                                dev = {'MessageType':'ProbeTemperatureMessage'}
                                dev['ProbeAddress'] = device
                                dev['MessageDate'] = datetime.datetime.now().isoformat()
                                dev['TemperatureValue'] = owproxy.read(device + 'temperature')
                                print(datetime.datetime.now().isoformat() + ":Temp:" + dev['TemperatureValue'] + "@" + dev['ProbeAddress'])
                                channel.basic_publish(exchange="InboundMessages",routing_key="device.plcbus.status",body=json.dumps(dev))
                        except :
                            print("Oops!  That was no valid number.  Try again...")
                else:
                    print("Device :" + device)
        print("Ending listing")
        connection.close()
print("Starting ")
time.sleep(15)

print("First sleep finished")
refresh()
scheduler = BlockingScheduler()
scheduler.add_job(refresh, 'interval', seconds=cfg["Timeout"])
scheduler.start()

