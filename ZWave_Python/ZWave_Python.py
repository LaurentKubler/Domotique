#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""

This file is part of **python-openzwave** project https://github.com/OpenZWave/python-openzwave.
    :platform: Unix, Windows, MacOS X
    :sinopsis: openzwave wrapper

.. moduleauthor:: bibi21000 aka Sébastien GALLET <bibi21000@gmail.com>

License : GPL(v3)

**python-openzwave** is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

**python-openzwave** is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with python-openzwave. If not, see http://www.gnu.org/licenses.

"""

import logging
import sys
import os
import resource
import pika
import json
import threading

#logging.getLogger('openzwave').addHandler(logging.NullHandler())
#logging.basicConfig(level=logging.DEBUG)
logging.basicConfig(level=logging.INFO)

logger = logging.getLogger('openzwave')
import openzwave
from openzwave.node import ZWaveNode
from openzwave.value import ZWaveValue
from openzwave.scene import ZWaveScene
from openzwave.controller import ZWaveController
from openzwave.network import ZWaveNetwork
from openzwave.option import ZWaveOption
import time
from datetime import datetime
import six
if six.PY3:
    from pydispatch import dispatcher
else:
    from louie import dispatcher

with open('config.json') as json_data_file:
    cfg = json.load(json_data_file)

device = "/dev/zwave"
log = "Info"


def publish(message):
    publish_connection = pika.BlockingConnection(pika.ConnectionParameters(cfg["Queue"]["server"]))
    publish_channel = publish_connection.channel()
    publish_channel.basic_publish(exchange="InboundMessages",
                          routing_key="device.plcbus.status",
                          body=json.dumps(message))

def louie_network_started(network):
    print('//////////// ZWave network is started ////////////')
    print('Louie signal : OpenZWave network is started : homeid {:08x} - {} nodes were found.'.format(network.home_id, network.nodes_count))
    dispatcher.connect(louie_node_update, ZWaveNetwork.SIGNAL_NODE)
    dispatcher.connect(louie_value_update, ZWaveNetwork.SIGNAL_VALUE_CHANGED)
    dispatcher.connect(louie_ctrl_message, ZWaveController.SIGNAL_CONTROLLER)
    #dispatcher.connect(louie_node_event, ZWaveController.SIGNAL_NODE_EVENT)
def louie_node_event(network, node, value):
    print('Louie signal : louie_node_event.')

def louie_network_resetted(network):
    print('Louie signal : OpenZWave network is resetted.')

def louie_network_ready(network):
    print('//////////// ZWave network is ready ////////////')
    print('Louie signal : ZWave network is ready : {} nodes were found.'.format(network.nodes_count))
    print('Louie signal : Controller : {}'.format(network.controller))

def louie_node_update(network, node):
    print('Louie signal : Node update : {}.'.format(node))

def louie_value_update(network, node, value):
    print('Louie signal : Value update : {}.'.format(value))
    print('Louie signal : index : {}'.format(value.index))
    print('Louie signal : data : {}'.format(value.data))
    print('Louie signal : node : {}'.format(value.node))
    print('Louie signal : instance : {}'.format(value.instance))
    print('Louie signal : value dict: {}'.format(value.to_dict()))
    if (value.command_class == 37):
        statusMessage = {}
        statusMessage["deviceAddress"] = "{0}/{1}".format(value.to_dict()["node_id"], value.instance)
        statusMessage["deviceAdapter"] = "plcbus"
        statusMessage["value"] = value.data 
        statusMessage["messageDate"] = datetime.utcnow().strftime('%Y-%m-%d %H:%M:%S.%f')[:-3]
        publish(statusMessage)

    
def louie_ctrl_message(state, message, network, controller):
    print('Louie signal : Controller message : {}.'.format(message))


for arg in sys.argv:
    if arg.startswith("--device"):
        temp,device = arg.split("=")
    elif arg.startswith("--log"):
        temp,log = arg.split("=")
    elif arg.startswith("--help"):
        print("help : ")
        print("  --device=/dev/yourdevice ")
        print("  --log=Info|Debug")

#Define some manager options
options = ZWaveOption(device, \
  #config_path="../openzwave/config", \
  user_path=".", cmd_line="")

options.set_log_file("OZW_Log.log")
options.set_append_log_file(False)
options.set_console_output(True)
options.set_save_log_level(log)
#options.set_save_log_level('Info')
options.set_logging(False)
options.lock()

print("Memory use : {} Mo".format((resource.getrusage(resource.RUSAGE_SELF).ru_maxrss / 1024.0)))

#Create a network object
def start_zwave():
    network = ZWaveNetwork(options, log=None)
    print("Starting event registration:")
    dispatcher.connect(louie_network_started,ZWaveNetwork.SIGNAL_NETWORK_STARTED)
    print("First evenbt registered")
    dispatcher.connect(louie_network_resetted, ZWaveNetwork.SIGNAL_NETWORK_RESETTED)
    print("Second evenbt registered")
    dispatcher.connect(louie_network_ready, ZWaveNetwork.SIGNAL_NETWORK_READY)
    print("Event registration done")

    time_started = 0
    print("------------------------------------------------------------")
    print("Waiting for network awaked : ")
    print("------------------------------------------------------------")
    for i in range(0,300):
        if network.state >= network.STATE_AWAKED:
            print("Network initialization done")
            print("Memory use : {} Mo".format((resource.getrusage(resource.RUSAGE_SELF).ru_maxrss / 1024.0)))
            break
        else:
            sys.stdout.write(".")
            sys.stdout.flush()
            time_started += 1
            time.sleep(1.0)
            print("Network is not awake " + str(i))
    if network.state < network.STATE_AWAKED:
        print(".")
        print("Network is not awake but continue anyway")
    print("End of start")
    return network

def print_details():
    print("------------------------------------------------------------")
    print("Use openzwave library : {}".format(network.controller.ozw_library_version))
    print("Use python library : {}".format(network.controller.python_library_version))
    print("Use ZWave library : {}".format(network.controller.library_description))
    print("Network home id : {}".format(network.home_id_str))
    print("Controller node id : {}".format(network.controller.node.node_id))
    print("Controller node version : {}".format(network.controller.node.version))
    print("Nodes in network : {}".format(network.nodes_count))
    print("------------------------------------------------------------")
    print("Waiting for network ready : ")
    print("------------------------------------------------------------")
    for i in range(0,300):
        if network.state >= network.STATE_READY:
            print(" done in {} seconds".format(time_started))
            break
        else:
            sys.stdout.write(".")
            time_started += 1
            #sys.stdout.write(network.state_str)
            #sys.stdout.write("(")
            #sys.stdout.write(str(network.nodes_count))
            #sys.stdout.write(")")
            #sys.stdout.write(".")
            sys.stdout.flush()
            time.sleep(1.0)


    print("Memory use : {} Mo".format((resource.getrusage(resource.RUSAGE_SELF).ru_maxrss / 1024.0)))
    if not network.is_ready:
        print(".")
        print("Network is not ready but continue anyway")

    print("------------------------------------------------------------")
    print("Controller capabilities : {}".format(network.controller.capabilities))
    print("Controller node capabilities : {}".format(network.controller.node.capabilities))
    print("Nodes in network : {}".format(network.nodes_count))
    print("Driver statistics : {}".format(network.controller.stats))
    print("------------------------------------------------------------")
    for node in network.nodes:

        print("------------------------------------------------------------")
        print("{} - Name : {}".format(network.nodes[node].node_id,network.nodes[node].name))
        print("{} - Manufacturer name / id : {} / {}".format(network.nodes[node].node_id,network.nodes[node].manufacturer_name, network.nodes[node].manufacturer_id))
        print("{} - Product name / id / type : {} / {} / {}".format(network.nodes[node].node_id,network.nodes[node].product_name, network.nodes[node].product_id, network.nodes[node].product_type))
        print("{} - Version : {}".format(network.nodes[node].node_id, network.nodes[node].version))
        print("{} - Command classes : {}".format(network.nodes[node].node_id,network.nodes[node].command_classes_as_string))
        print("{} - Capabilities : {}".format(network.nodes[node].node_id,network.nodes[node].capabilities))
        print("{} - Neigbors : {}".format(network.nodes[node].node_id,network.nodes[node].neighbors))
        print("{} - Can sleep : {}".format(network.nodes[node].node_id,network.nodes[node].can_wake_up()))
        groups = {}
        for grp in network.nodes[node].groups :
            groups[network.nodes[node].groups[grp].index] = {'label':network.nodes[node].groups[grp].label, 'associations':network.nodes[node].groups[grp].associations}
        print("{} - Groups : {}".format(network.nodes[node].node_id, groups))
        values = {}
        for val in network.nodes[node].values :
            values[network.nodes[node].values[val].object_id] = {
                'label':network.nodes[node].values[val].label,
                'help':network.nodes[node].values[val].help,
                'command_class':network.nodes[node].values[val].command_class,
                'max':network.nodes[node].values[val].max,
                'min':network.nodes[node].values[val].min,
                'units':network.nodes[node].values[val].units,
                'data':network.nodes[node].values[val].data_as_string,
                'ispolled':network.nodes[node].values[val].is_polled
                }
        #print("{} - Values : {}".format(network.nodes[node].node_id, values))
        #print("------------------------------------------------------------")
        for cmd in network.nodes[node].command_classes:
            print("   ---------   ")
            #print("cmd = {}".format(cmd))
            values = {}
            for val in network.nodes[node].get_values_for_command_class(cmd) :
                values[network.nodes[node].values[val].object_id] = {
                    'label':network.nodes[node].values[val].label,
                    'help':network.nodes[node].values[val].help,
                    'max':network.nodes[node].values[val].max,
                    'min':network.nodes[node].values[val].min,
                    'units':network.nodes[node].values[val].units,
                    'data':network.nodes[node].values[val].data,
                    'data_str':network.nodes[node].values[val].data_as_string,
                    'genre':network.nodes[node].values[val].genre,
                    'type':network.nodes[node].values[val].type,
                    'ispolled':network.nodes[node].values[val].is_polled,
                    'readonly':network.nodes[node].values[val].is_read_only,
                    'writeonly':network.nodes[node].values[val].is_write_only,
                    }
            print("{} - Values for command class : {} : {}".format(network.nodes[node].node_id,
                                        network.nodes[node].get_command_class_as_string(cmd),
                                        values))
        print("------------------------------------------------------------")

    print("------------------------------------------------------------")
    print("Driver statistics : {}".format(network.controller.stats))
    print("------------------------------------------------------------")

    print("------------------------------------------------------------")
    print("Try to autodetect nodes on the network")
    print("------------------------------------------------------------")
    print("Nodes in network : {}".format(network.nodes_count))
    print("------------------------------------------------------------")
    print("Retrieve switches on the network")
    print("------------------------------------------------------------")
    values = {}
    for node in network.nodes:
        for val in network.nodes[node].get_switches() :
            print("node/name/index/instance : {}/{}/{}/{}".format(node,network.nodes[node].name,network.nodes[node].values[val].index,network.nodes[node].values[val].instance))
            print("  label/help : {}/{}".format(network.nodes[node].values[val].label,network.nodes[node].values[val].help))
            print("  id on the network : {}".format(network.nodes[node].values[val].id_on_network))
            print("  state: {}".format(network.nodes[node].get_switch_state(val)))
    print("------------------------------------------------------------")
    print("Retrieve dimmers on the network")
    print("------------------------------------------------------------")
    values = {}
    for node in network.nodes:
        for val in network.nodes[node].get_dimmers() :
            print("node/name/index/instance : {}/{}/{}/{}".format(node,network.nodes[node].name,network.nodes[node].values[val].index,network.nodes[node].values[val].instance))
            print("  label/help : {}/{}".format(network.nodes[node].values[val].label,network.nodes[node].values[val].help))
            print("  id on the network : {}".format(network.nodes[node].values[val].id_on_network))
            print("  level: {}".format(network.nodes[node].get_dimmer_level(val)))
    print("------------------------------------------------------------")
    print("Retrieve RGB Bulbs on the network")
    print("------------------------------------------------------------")
    values = {}
    for node in network.nodes:
        for val in network.nodes[node].get_rgbbulbs() :
            print("node/name/index/instance : {}/{}/{}/{}".format(node,network.nodes[node].name,network.nodes[node].values[val].index,network.nodes[node].values[val].instance))
            print("  label/help : {}/{}".format(network.nodes[node].values[val].label,network.nodes[node].values[val].help))
            print("  id on the network : {}".format(network.nodes[node].values[val].id_on_network))
            print("  level: {}".format(network.nodes[node].get_dimmer_level(val)))
    print("------------------------------------------------------------")
    print("Retrieve sensors on the network")
    print("------------------------------------------------------------")
    values = {}
    for node in network.nodes:
        for val in network.nodes[node].get_sensors() :
            print("node/name/index/instance : {}/{}/{}/{}".format(node,network.nodes[node].name,network.nodes[node].values[val].index,network.nodes[node].values[val].instance))
            print("  label/help : {}/{}".format(network.nodes[node].values[val].label,network.nodes[node].values[val].help))
            print("  id on the network : {}".format(network.nodes[node].values[val].id_on_network))
            print("  value: {} {}".format(network.nodes[node].get_sensor_value(val), network.nodes[node].values[val].units))
    print("------------------------------------------------------------")
    print("Retrieve thermostats on the network")
    print("------------------------------------------------------------")
    values = {}
    for node in network.nodes:
        for val in network.nodes[node].get_thermostats() :
            print("node/name/index/instance : {}/{}/{}/{}".format(node,network.nodes[node].name,network.nodes[node].values[val].index,network.nodes[node].values[val].instance))
            print("  label/help : {}/{}".format(network.nodes[node].values[val].label,network.nodes[node].values[val].help))
            print("  id on the network : {}".format(network.nodes[node].values[val].id_on_network))
            print("  value: {} {}".format(network.nodes[node].get_thermostat_value(val), network.nodes[node].values[val].units))
    print("------------------------------------------------------------")
    print("Retrieve switches all compatibles devices on the network    ")
    print("------------------------------------------------------------")
    values = {}
    for node in network.nodes:
        for val in network.nodes[node].get_switches_all() :
            print("node/name/index/instance : {}/{}/{}/{}".format(node,network.nodes[node].name,network.nodes[node].values[val].index,network.nodes[node].values[val].instance))
            print("  label/help : {}/{}".format(network.nodes[node].values[val].label,network.nodes[node].values[val].help))
            print("  id on the network : {}".format(network.nodes[node].values[val].id_on_network))
            print("  value / items:  / {}".format(network.nodes[node].get_switch_all_item(val), network.nodes[node].get_switch_all_items(val)))
            print("  state: {}".format(network.nodes[node].get_switch_all_state(val)))
    print("------------------------------------------------------------")
    print("------------------------------------------------------------")
    print("Retrieve protection compatibles devices on the network    ")
    print("------------------------------------------------------------")
    values = {}
    for node in network.nodes:
        for val in network.nodes[node].get_protections() :
            print("node/name/index/instance : {}/{}/{}/{}".format(node,network.nodes[node].name,network.nodes[node].values[val].index,network.nodes[node].values[val].instance))
            print("  label/help : {}/{}".format(network.nodes[node].values[val].label,network.nodes[node].values[val].help))
            print("  id on the network : ".format(network.nodes[node].values[val].id_on_network))
            print("  value / items: {} / {}".format(network.nodes[node].get_protection_item(val), network.nodes[node].get_protection_items(val)))
    print("------------------------------------------------------------")

    print("------------------------------------------------------------")
    print("Retrieve battery compatibles devices on the network         ")
    print("------------------------------------------------------------")
    values = {}
    for node in network.nodes:
        for val in network.nodes[node].get_battery_levels() :
            print("node/name/index/instance : {}/{}/{}/{}".format(node,network.nodes[node].name,network.nodes[node].values[val].index,network.nodes[node].values[val].instance))
            print("  label/help : {}/{}".format(network.nodes[node].values[val].label,network.nodes[node].values[val].help))
            print("  id on the network : {}".format(network.nodes[node].values[val].id_on_network))
            print("  value : {}".format(network.nodes[node].get_battery_level(val)))
    print("------------------------------------------------------------")

    print("------------------------------------------------------------")
    print("Retrieve power level compatibles devices on the network         ")
    print("------------------------------------------------------------")
    values = {}
    for node in network.nodes:
        for val in network.nodes[node].get_power_levels() :
            print("node/name/index/instance : {}/{}/{}/{}".format(node,network.nodes[node].name,network.nodes[node].values[val].index,network.nodes[node].values[val].instance))
            print("  label/help : {}/{}".format(network.nodes[node].values[val].label,network.nodes[node].values[val].help))
            print("  id on the network : {}".format(network.nodes[node].values[val].id_on_network))
            print("  value : {}".format(network.nodes[node].get_power_level(val)))
    print("------------------------------------------------------------")
    #print
    #print("------------------------------------------------------------")
    #print "Activate the switches on the network"
    #print "Nodes in network : {}".format network.nodes_count
    #print("------------------------------------------------------------")
    #for node in network.nodes:
    #    for val in network.nodes[node].get_switches() :
    #        print("Activate switch {} on node {}".format \
    #                (network.nodes[node].values[val].label,node))
    #        network.nodes[node].set_switch(val,True)
    #        print("Sleep 10 seconds")
    #        time.sleep(10.0)
    #        print("Dectivate switch {} on node {}".format \
    #                (network.nodes[node].values[val].label,node))
    #        network.nodes[node].set_switch(val,False)
    #print("Done"))
    #print("------------------------------------------------------------")
    print("------------------------------------------------------------")
    print("Driver statistics : {}".format(network.controller.stats))
    print("Driver label : {}".format(network.controller.get_stats_label('retries')))
    print("------------------------------------------------------------")


def bind_mq(callback):
    rabbit_channel = connection.channel()
    #rabbit_channel.exchange_declare(exchange=cfg["Queue"]["exchangeName"],
    #exchange_type='fanout')
    rabbit_channel.exchange_declare(exchange="OutboundMessages", exchange_type='fanout')
    result = rabbit_channel.queue_declare(exclusive=True)
    queue_name = result.method.queue
    #rabbit_channel.queue_bind(exchange=cfg["Queue"]["exchangeName"],
    #queue=queue_name, routing_key=cfg["Queue"]["InboundroutingKey"])
    rabbit_channel.queue_bind(exchange="OutboundMessages", queue=queue_name, routing_key=cfg["Queue"]["InboundroutingKey"])

    print(' [*] Waiting for logs. To exit press CTRL+C')

    rabbit_channel.basic_consume(callback,
                          queue=queue_name,
                          no_ack=True)

    mq_receive_thread = threading.Thread(target=rabbit_channel.start_consuming)
    mq_receive_thread.start()

def callback(ch, method, properties, body):
    print("Received " + body)
    command = json.loads(body) 
    address = command["targetAddress"]
    command = command["command"]
    nodeindex,instance = address.split('/')
    nodes = network.nodes
    print("Activate switch {} on node {}".format(instance,nodeindex))
    try:
        for node in network.nodes:
            if (str(network.nodes[node].node_id) == str(nodeindex)):
                print("node found {}".format(node))
                for val in network.nodes[node].get_switches() :
                    print("val  {}:{}".format(network.nodes[node].values[val].instance,instance))
                    if str(network.nodes[node].values[val].instance) == str(instance):
                        print("val found {}".format(node))
                        if Command == "PowerOn":
                            network.nodes[node].set_switch(val,True)
                        if Command == "PowerOff":
                            network.nodes[node].set_switch(val,False)
    except:
        print("Unexpected error:{}".format(sys.exc_info()[0]))



connection = pika.BlockingConnection(pika.ConnectionParameters(cfg["Queue"]["server"]))


print("starting zwave")
network = start_zwave()
bind_mq(callback)

for node in network.nodes:
    for val in network.nodes[node].get_switches() :
        #print("Activate switch {} on node
        #{}".format(network.nodes[node].values[val].label,node))
        print("Activate switch {} on node {}".format(network.nodes[node].values[val].index,node))
        print("Val: {}".format(network.nodes[node].values[val].index,node))
        network.nodes[node].set_switch(val,True)
        time.sleep(1)
        network.nodes[node].set_switch(val,False)
        time.sleep(5)
print("zwave started")
while True:    
    time.sleep(1)

def stop_zwave():
    print("------------------------------------------------------------")
    print("Stop network")
    print("------------------------------------------------------------")
    network.stop()
    print("Memory use : {} Mo".format((resource.getrusage(resource.RUSAGE_SELF).ru_maxrss / 1024.0)))
