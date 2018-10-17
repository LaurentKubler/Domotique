import { Component, OnInit, Input } from '@angular/core';
import { DeviceStatus } from '../app.component';

@Component({
  selector: 'device',
  templateUrl: './device.component.html',
  styleUrls: ['./device.component.css']
})
export class DeviceComponent implements OnInit {

  @Input() device: DeviceStatus;
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
  constructor() { };


  ngOnInit() {
  }

}
