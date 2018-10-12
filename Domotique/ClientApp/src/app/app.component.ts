import { Component } from '@angular/core';
import { Http } from '@angular/http';
import { Chart } from 'angular-highcharts';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent {
  title = 'ma domotique';
  public Status: Status;
  public Rooms: RoomStatus[];
  public Devices: DeviceStatus[];
  public chart: Chart;

  constructor(private http: Http) {

    this.http.get('/rest/status').subscribe(
      result => {
        this.Status = result.json() as Status;
        this.Rooms = this.Status.rooms;
        this.Devices = this.Status.devices;
        console.warn(this.Rooms[0].roomName);
      },
      error => console.error(error));

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
}
export class Status {
  public rooms: RoomStatus[];
  public devices: DeviceStatus[];
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
  public name: number;
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
}
