import { Component } from '@angular/core';
import { MqttService, IMqttMessage } from 'ngx-mqtt';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'MonitorPlatform';
  messages: string[] = [];

  constructor(private mqttService: MqttService) { }

  ngOnInit(): void {
    this.mqttService.observe('station/IX/1/lcd').subscribe((message: IMqttMessage) => {
      const msg = message.payload.toString();
      this.messages.push(msg);
      console.log('Received message:', msg);
    });
  }
  sendMessage(): void {
    const payload = 'Hello MQTT from Angular!';
    this.mqttService.unsafePublish('test/topic', payload, { qos: 1 });
    console.log('Message sent:', payload);
  }
}
