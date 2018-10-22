import { Component } from '@angular/core';
import { Http } from '@angular/http';
import { Chart } from 'angular-highcharts';
import * as signalR from "@aspnet/signalr";
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent {
  title = 'ma domotique';
  public version: string = '';
  public Status: Status;
  public Rooms: RoomStatus[];
  public Devices: DeviceStatus[];
  public chart: Chart;
  private _hubConnection: HubConnection;
  constructor(private http: Http) {

    this.http.get('/rest/status').subscribe(
      result => {
        this.Status = result.json() as Status;
        this.Rooms = this.Status.rooms;
        this.Devices = [];

        for (let device of this.Status.devices) {
          this.Devices.push(Object.assign(new DeviceStatus(), device));
          //console.warn(device.deviceName + ":" + device.IsLight());
        }
        this.version = this.Status.version;

      },
      error => console.error(error));
    this._hubConnection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
    this._hubConnection.on('DeviceChange', (deviceID: number, status: boolean, valueDate: Date) => {
      console.log(deviceID + "/" + status + "/" + valueDate);
    });
    this._hubConnection.on('TemperatureReceived', (roomID: number, currentTemp: number, targetTemp: number, messageDate: Date) => {
      console.log(roomID + "/" + currentTemp + "/" + targetTemp + "/" + messageDate);
    });
    this._hubConnection.start();
    this.http.get('/rest/temphistory').subscribe(
      result => {
        var series = result.json() as Highcharts.SeriesOptions[];

        this.chart = new Chart({
          chart: {
            type: 'spline'
          },
          xAxis: {
            type: 'datetime',
            dateTimeLabelFormats: { // don't display the dummy year
              month: '%e. %b',
              year: '%b'
            },
            title: {
              text: 'Date'
            }
          },
          yAxis: {
            title: {
              text: 'Température (°C)'
            },
            min: 0
          },
          title: {
            text: 'Evolution des températures'
          },
          credits: {
            enabled: false
          },
          series: series
        });
      },
      error => console.error(error));
  }
  public PowerOn(deviceid) {
    this.http.post('/rest/BinarySwitchDevice/' + deviceid + '/PowerOn', '').subscribe(
      result => console.debug(result),
      error => console.error(error)
    );
  }
  public PowerOff(deviceid) {
    this.http.post('/rest/BinarySwitchDevice/' + deviceid + '/PowerOff', '').subscribe(
      result => console.debug(result),
      error => console.error(error)
    );
  }
}
export class Status {
  public rooms: RoomStatus[];
  public devices: DeviceStatus[];
  public version: string;
}
export class RoomStatus {
  public roomId: number;
  public roomName: string;
  public picture: number;
  public temperatures: DayTemperature[];
  public lastTemperatureRefresh: Date;
  public currentTemperature: number;
}
export class DayTemperature {
  public temperatureDate: Date;
  public minTemp: number;
  public maxTemp: number;
}
export class DeviceFunction {
  public name: string;
}
export class DeviceStatus {
  public device_ID: number;
  public deviceName: string;
  public value: number;
  public valueDate: Date;
  public status: boolean;
  public onImage_ID: number;
  public offImage_ID: number;
  public functions: DeviceFunction[];
  public IsLight() {
    for (let func of this.functions) {
      if (func.name == "Light")
        return true;
    }
    return false;
  }

  public IsSwitch() {
    for (let func of this.functions) {
      if (func.name == "Switch")
        return true;
    }
    return false;
  }
}
