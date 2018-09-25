﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using ZWave.Channel;
using ZWave.CommandClasses;

namespace ZWave
{
    public class Program
    {
        public static void Mains(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
            static void Main1(string[] args)
            {
                // use first serial port
                var portName = System.IO.Ports.SerialPort.GetPortNames().First();

                // create a channel
                var channel = new ZWaveChannel("COM5");

                // uncommment to see detailed logging
                // channel.Log = Console.Out;

                // subcribe to node events
                channel.NodeEventReceived += (sender, e) => Console.WriteLine($"Event: NodeID:{e.NodeID:D3} Command:[{e.Command}]");

                // open channel
                channel.Open();
                try
                {
                    Run(channel).Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex}");
                }
                finally
                {
                    Console.ReadLine();
                    channel.Close();
                }
            }

        static private async Task Run(ZWaveChannel channel)
        {
            // ZWave function: GetVersion
            var response = await channel.Send(Function.GetVersion);
            var data = response.TakeWhile(element => element != 0).ToArray();
            var version = Encoding.UTF8.GetString(data, 0, data.Length);
            Console.WriteLine($"Version: {version}");

            // ZWave function: MemoryGetId
            response = await channel.Send(Function.MemoryGetId);
            var homeID = BitConverter.ToUInt32(response.Take(4).Reverse().ToArray(), 0);
            Console.WriteLine($"HomeID: {homeID:X}");
            Console.WriteLine($"ControllerID: {response[4]:D3}");

            // NodeID of the fibaro wall plug
            byte wallPlugID = 5;
            // turn wallplug on
            Console.WriteLine($"Set wallplug on.");
            await channel.Send(wallPlugID, new Command(CommandClass.SwitchBinary, 0x02, 255));

            await channel.Send(wallPlugID, new Command(CommandClass.MultiChannel, 0x07, 255)); 
            //await channel.Send(wallPlugID, new Command(CommandClass.SwitchBinary, 0x01, 255));
            await Task.Delay(1000);

            // turn wallplug off
            Console.WriteLine($"Set wallplug off.");
            await channel.Send(wallPlugID, new Command(CommandClass.SwitchBinary, 0x02, 0));
            //await channel.Send(wallPlugID, new Command(CommandClass.SwitchBinary, 0x01, 0));
        }
        static void Main(string[] args)
        {
            var portName = System.IO.Ports.SerialPort.GetPortNames().Where(element => element != "COM1").First();

            var controller = new ZWaveController("COM3");
            
            // netsh http add urlacl url=http://+:80/ user=Everyone
            // http://localhost:80/api/v1.0/controller/nodes/19/basic/get/
            
            //controller.Channel.Log = Console.Out;

            controller.Open();            
            try
            {
                Run(controller).Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var inner in ex.InnerExceptions)
                {
                    LogMessage($"{inner}");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"{ex}");
            }
            finally
            {
                Console.ReadLine();
                
                controller.Close();
            }
        }

        private static void LogMessage(string message)
        {
            var text = $"{DateTime.Now.TimeOfDay} {message}";

            Console.WriteLine(text);
            lock (typeof(File))
            {
                if (Directory.Exists(@"D:\Temp"))
                {
                    File.AppendAllText(@"D:\Temp\ZWave.log", text + Environment.NewLine);
                }
            }
        }

        static private async Task Run(ZWaveController controller)
        {
            LogMessage($"Version: {await controller.GetVersion()}");
            LogMessage($"HomeID: {await controller.GetHomeID():X}");

            var controllerNodeID = await controller.GetNodeID();
            LogMessage($"ControllerID: {controllerNodeID:D3}");

            
            await controller.DiscoverNodes();
            var nodes = await controller.GetNodes();
            await Task.Delay(2000);
            foreach (var node in nodes)
            {
                var protocolInfo = await node.GetProtocolInfo();
                LogMessage($"Node: {node}, Generic = {protocolInfo.GenericType}, Basic = {protocolInfo.BasicType}, Listening = {protocolInfo.IsListening} ");

                var neighbours = await node.GetNeighbours();
                LogMessage($"Node: {node}, Neighbours = {string.Join(", ", neighbours.Cast<object>().ToArray())}");

                // subcribe to changes
                Subscribe(node);
            }
            //var command =nodes[2].GetCommandClass<SwitchBinary>();
            var command = nodes[5].GetCommandClass<MultiChannel>();
            //await Task.Delay(2000);

            await Task.Delay(1000);
            await command.BinarySwitchSet(1, true);
            await Task.Delay(1000);
            await command.BinarySwitchSet(2, true);
            await Task.Delay(1000);
            await command.BinarySwitchSet(1,false);
            await Task.Delay(1000);
            await command.BinarySwitchSet(2, false);

            await Task.Delay(1000);
            await command.BinarySwitchSet(2, true);
            await Task.Delay(1000);
            await command.BinarySwitchSet(1, false);
            await Task.Delay(1000);
            await command.BinarySwitchSet(2, false);

            //await InitializeWallPlug(nodes[2]);
            //await InitializeWallPlug(nodes[3]);
            //await InitializeShockSensor(nodes[4]);
            //await InitializeGarageDoorSensor(nodes[5]);
            //await InitializeThermostat(nodes[6]);
            //await InitializeMultiSensor(nodes[18]);
            //await InitializeDoorSensor(nodes[10]);
            //await InitializePowerSwitch(nodes[19]);
            //await InitializePowerSwitch(nodes[20]);
            //await InitializePowerSwitch(nodes[24]);
            //await InitializePowerSwitch(nodes[25]);
            //await InitializePowerSwitch(nodes[2]);
            //var neighborUpdateStatus = await nodes[19].RequestNeighborUpdate((status) => LogMessage($"RequestNeighborUpdate: {status} "));

            Console.ReadLine();
        }

        private static void Subscribe(Node node)
        {
            var basic = node.GetCommandClass<Basic>();
            basic.Changed += (_, e) => LogMessage($"Basic report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var sensorMultiLevel = node.GetCommandClass<SensorMultiLevel>();
            sensorMultiLevel.Changed += (_, e) => LogMessage($"SensorMultiLevel report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var meter = node.GetCommandClass<Meter>();
            meter.Changed += (_, e) => LogMessage($"Meter report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var alarm = node.GetCommandClass<Alarm>();
            alarm.Changed += (_, e) => LogMessage($"Alarm report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var sensorBinary = node.GetCommandClass<SensorBinary>();
            sensorBinary.Changed += (_, e) => LogMessage($"SensorBinary report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var sensorAlarm = node.GetCommandClass<SensorAlarm>();
            sensorAlarm.Changed += (_, e) => LogMessage($"SensorAlarm report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var wakeUp = node.GetCommandClass<WakeUp>();
            wakeUp.Changed += (_, e) => { LogMessage($"WakeUp report of Node {e.Report.Node:D3} changed to [{e.Report}]"); };

            var switchBinary = node.GetCommandClass<SwitchBinary>();
            switchBinary.Changed += (_, e) => LogMessage($"SwitchBinary report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var thermostatSetpoint = node.GetCommandClass<ThermostatSetpoint>();
            thermostatSetpoint.Changed += (_, e) => LogMessage($"thermostatSetpoint report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var sceneActivation = node.GetCommandClass<SceneActivation>();
            sceneActivation.Changed += (_, e) => LogMessage($"sceneActivation report of Node {e.Report.Node:D3} changed to [{e.Report}]");

            var multiChannel = node.GetCommandClass<MultiChannel>();
            multiChannel.Changed += (_, e) => LogMessage($"multichannel report of Node {e.Report.Node:D3} changed to [{e.Report}]");
            var multiLvel = node.GetCommandClass<SwitchMultiLevel>();
            multiLvel.Changed += (_, e) => LogMessage($"multilevel report of Node {e.Report.Node:D3} changed to [{e.Report}]");
        }

        private static async Task InitializeWallPlug(Node node)
        {
            //var association = node.GetCommandClass<Association>();
            //await association.Add(1, 1);
            //await association.Add(2, 1);
            //await association.Add(3, 1);

            var supportedCommandClasses = await node.GetSupportedCommandClasses();
            LogMessage($"Supported commandclasses:\n{string.Join("\n", supportedCommandClasses.Cast<object>())}");

            var manufacturerSpecific = node.GetCommandClass<ManufacturerSpecific>();
            var manufacturerSpecificReport = await manufacturerSpecific.Get();
            LogMessage($"Manufacturer specific report of Node {manufacturerSpecificReport.Node:D3} is [{manufacturerSpecificReport}]");

            var configuration = node.GetCommandClass<Configuration>();
            await configuration.Set(61, (byte)2); // on => White
            await configuration.Set(62, (byte)8); // off => None
            await configuration.Set(47, (ushort)900); // measure interval 15 minutes

            var switchBinary = node.GetCommandClass<SwitchBinary>();
            await switchBinary.Set(true);

            Console.ReadLine();
        }

        private static async Task InitializeShockSensor(Node node)
        {
            LogMessage("Please wakeup the shock sensor.");
            Console.ReadLine();

            var association = node.GetCommandClass<Association>();
            await association.Add(1, 1);
            await association.Add(2, 1);
            await association.Add(3, 1);

            var supportedCommandClasses = await node.GetSupportedCommandClasses();
            LogMessage($"Supported commandclasses:\n{string.Join("\n", supportedCommandClasses.Cast<object>())}");

            var manufacturerSpecific = node.GetCommandClass<ManufacturerSpecific>();
            var manufacturerSpecificReport = await manufacturerSpecific.Get();
            LogMessage($"Manufacturer specific report of Node {manufacturerSpecificReport.Node:D3} is [{manufacturerSpecificReport}]");

            var battery = node.GetCommandClass<Battery>();
            var batteryReport = await battery.Get();
            LogMessage($"Battery report of Node {batteryReport.Node:D3} is [{batteryReport}]");

            var wakeUp = node.GetCommandClass<WakeUp>();
            await wakeUp.SetInterval(TimeSpan.FromMinutes(15), 1);
            var wakeUpReport = await wakeUp.GetInterval();
            LogMessage($"WakeUp report of Node {wakeUpReport.Node:D3} is [{wakeUpReport}]");

            Console.ReadLine();
        }

        private static async Task InitializeGarageDoorSensor(Node node)
        {
            LogMessage("Please wakeup the garagedoor sensor.");
            Console.ReadLine();

            var association = node.GetCommandClass<Association>();
            await association.Add(1, 1);
            await association.Add(2, 1);
            await association.Add(3, 1);

            var supportedCommandClasses = await node.GetSupportedCommandClasses();
            LogMessage($"Supported commandclasses:\n{string.Join("\n", supportedCommandClasses.Cast<object>())}");

            var manufacturerSpecific = node.GetCommandClass<ManufacturerSpecific>();
            var manufacturerSpecificReport = await manufacturerSpecific.Get();
            LogMessage($"Manufacturer specific report of Node {manufacturerSpecificReport.Node:D3} is [{manufacturerSpecificReport}]");

            var battery = node.GetCommandClass<Battery>();
            var batteryReport = await battery.Get();
            LogMessage($"Battery report of Node {batteryReport.Node:D3} is [{batteryReport}]");

            var wakeUp = node.GetCommandClass<WakeUp>();
            await wakeUp.SetInterval(TimeSpan.FromMinutes(15), 1);
            var wakeUpReport = await wakeUp.GetInterval();
            LogMessage($"WakeUp report of Node {wakeUpReport.Node:D3} is [{wakeUpReport}]");

            Console.ReadLine();
        }

        private static async Task InitializeMultiSensor(Node motionSensor)
        {
            LogMessage("Please wakeup the multisensor.");
            Console.ReadLine();

            var association = motionSensor.GetCommandClass<Association>();
            await association.Add(1, 1);
            await association.Add(2, 1);
            await association.Add(3, 1);

            var supportedCommandClasses = await motionSensor.GetSupportedCommandClasses();
            LogMessage($"Supported commandclasses:\n{string.Join("\n", supportedCommandClasses.Cast<object>())}");

            var manufacturerSpecific = motionSensor.GetCommandClass<ManufacturerSpecific>();
            var manufacturerSpecificReport = await manufacturerSpecific.Get();
            LogMessage($"Manufacturer specific report of Node {manufacturerSpecificReport.Node:D3} is [{manufacturerSpecificReport}]");

            var battery = motionSensor.GetCommandClass<Battery>();
            var batteryReport = await battery.Get();
            LogMessage($"Battery report of Node {batteryReport.Node:D3} is [{batteryReport}]");

            var wakeUp = motionSensor.GetCommandClass<WakeUp>();
            await wakeUp.SetInterval(TimeSpan.FromMinutes(15), 0x01);
            var wakeUpReport = await wakeUp.GetInterval();
            LogMessage($"WakeUp report of Node {wakeUpReport.Node:D3} is [{wakeUpReport}]");

            var configuration = motionSensor.GetCommandClass<Configuration>();
            await configuration.Set(111, (uint)240); // minimum interval 240 seconds
            await configuration.Set(3, (UInt16)300); // set motion timeout to 5 minutes
            var motionTimeout = await configuration.Get(3);

            Console.ReadLine();
        }

        public static async Task InitializeThermostat(Node node)
        {
            LogMessage("Please wakeup the thermostat.");
            Console.ReadLine();

            var battery = node.GetCommandClass<Battery>();
            var batteryReport = await battery.Get();
            LogMessage($"Battery report of Node {batteryReport.Node:D3} is [{batteryReport}]");

            var wakeUp = node.GetCommandClass<WakeUp>();
            var wakeUpReport = await wakeUp.GetInterval();
            await wakeUp.SetInterval(TimeSpan.FromMinutes(15), 1);
            LogMessage($"WakeUp report of Node {wakeUpReport.Node:D3} is [{wakeUpReport}]");

            var thermostatSetpoint = node.GetCommandClass<ThermostatSetpoint>();
            var thermostatSetpointReport = await thermostatSetpoint.Get(ThermostatSetpointType.Heating);
            LogMessage($"SetpointReport report of Node {thermostatSetpointReport.Node:D3} is [{thermostatSetpointReport}]");
            //await thermostatSetpoint.Set(ThermostatSetpointType.Heating, 18.0F);

            var clock = node.GetCommandClass<Clock>();
            var clockReport = await clock.Get();
            LogMessage($"clockReport report of Node {clockReport.Node:D3} is [{clockReport}]");
        }

        private static async Task InitializeDoorSensor(Node node)
        {
            LogMessage("Please wakeup the door sensor.");
            Console.ReadLine();

            var association = node.GetCommandClass<Association>();
            await association.Add(1, 1);
            await association.Add(2, 1);
            await association.Add(3, 1);

            var supportedCommandClasses = await node.GetSupportedCommandClasses();
            LogMessage($"Supported commandclasses:\n{string.Join("\n", supportedCommandClasses.Cast<object>())}");

            var manufacturerSpecific = node.GetCommandClass<ManufacturerSpecific>();
            var manufacturerSpecificReport = await manufacturerSpecific.Get();
            LogMessage($"Manufacturer specific report of Node {manufacturerSpecificReport.Node:D3} is [{manufacturerSpecificReport}]");

            var battery = node.GetCommandClass<Battery>();
            var batteryReport = await battery.Get();
            LogMessage($"Battery report of Node {batteryReport.Node:D3} is [{batteryReport}]");

            var wakeUp = node.GetCommandClass<WakeUp>();
            await wakeUp.SetInterval(TimeSpan.FromMinutes(15), 1);
            var wakeUpReport = await wakeUp.GetInterval();
            LogMessage($"WakeUp report of Node {wakeUpReport.Node:D3} is [{wakeUpReport}]");

            var basic = node.GetCommandClass<Basic>();
            var basicReport = await basic.Get();
            LogMessage($"BasicReport report of Node {basicReport.Node:D3} is [{basicReport}]");

            var alarm = node.GetCommandClass<Alarm>();
            var alarmReport = await basic.Get();
            LogMessage($"AlarmReport report of Node {alarmReport.Node:D3} is [{alarmReport}]");

            Console.ReadLine();
        }

        private static async Task InitializePowerSwitch(Node node)
        {
            var association = node.GetCommandClass<Association>();
            //var groups = await association.GetGroups();

            //await association.Remove(1);
            //await association.Add(1, 19, 20, 21, 22, 23);

            var supportedCommandClasses = await node.GetSupportedCommandClasses();
            LogMessage($"Supported commandclasses:\n{string.Join("\n", supportedCommandClasses.Cast<object>())}");

            var manufacturerSpecific = node.GetCommandClass<ManufacturerSpecific>();
            var manufacturerSpecificReport = await manufacturerSpecific.Get();
            LogMessage($"Manufacturer specific report of Node {manufacturerSpecificReport.Node:D3} is [{manufacturerSpecificReport}]");

            var switchBinary = node.GetCommandClass<SwitchBinary>();
            await switchBinary.Set(true);
            await Task.Delay(1000);
            await switchBinary.Set(false);

            Console.ReadLine();
        }
    }
}
