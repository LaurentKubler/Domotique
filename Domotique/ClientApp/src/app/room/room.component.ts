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

}
