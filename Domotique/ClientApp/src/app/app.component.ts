import { Component } from '@angular/core';
import { Http } from '@angular/http';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'ClientApp';
  public values: string;
  constructor(private http: Http) {
    this.http.get('/rest/status').subscribe(result => {
      this.values = result.json() as string[];
      console.warn(result);
    }, error => console.error(error));
  }
}
