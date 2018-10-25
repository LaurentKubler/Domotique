import { Component, OnInit, Input } from '@angular/core';
import { Http } from '@angular/http';
import { RoomStatus } from '../app.component';

@Component({
  selector: 'app-room',
  templateUrl: './room.component.html',
  styleUrls: ['./room.component.css']
})
export class RoomComponent implements OnInit {

  @Input() room: RoomStatus;
  constructor(private http: Http) { };

  ngOnInit() {
  }
  public SetRegulationOn() {
    this.http.get('/rest/room/' + this.room.roomId + '/SetRegulationOn').subscribe(
      result => {
        console.log("Regulation set Auto on roomId")
      },
      error => console.error(error));
  }
  public SetRegulationOff() {
    this.http.get('/rest/room/' + this.room.roomId + '/SetRegulationOff').subscribe(
      result => {
        console.log("Regulation set Off on oom roomId")
      },
      error => console.error(error));
  }
}
