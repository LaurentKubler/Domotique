import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core'; 
import { HttpModule } from '@angular/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { RoomStatusComponent } from './room-status/room-status.component';
import { ChartModule } from 'angular-highcharts';
import { DeviceComponent } from './device/device.component';

@NgModule({
  declarations: [
    AppComponent,
    RoomStatusComponent,
    DeviceComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpModule,
    ChartModule 
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
