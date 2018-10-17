import { Component, OnInit, Input } from '@angular/core';
import { DeviceStatus } from '../app.component';

@Component({
  selector: 'device',
  templateUrl: './device.component.html',
  styleUrls: ['./device.component.css']
})
export class DeviceComponent implements OnInit {

  @Input() device: DeviceStatus;

  constructor() { };


  ngOnInit() {
  }

}
