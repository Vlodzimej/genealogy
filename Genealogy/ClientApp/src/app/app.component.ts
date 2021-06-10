import { Component, Inject } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  title = 'Генеалогия';
  message = '';

  constructor(@Inject('MESSAGE') message: string) {
    this.message = message;
  }

}
