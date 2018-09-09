import { Component } from '@angular/core';
import { Http } from '@angular/http';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent {
  title = 'ClientApp';
  public Rooms: RoomStatus[];
  constructor(private http: Http) {
    this.http.get('/rest/status').subscribe(result => {
      this.Rooms =  result.json() as RoomStatus[];
      console.warn(this.Rooms[0].roomName);
    }, error => console.error(error));
  }
}
export class Status {
  public Rooms: RoomStatus[];
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
