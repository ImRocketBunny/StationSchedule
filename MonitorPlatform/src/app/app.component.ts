import { Component } from '@angular/core';
import { MqttService, IMqttMessage } from 'ngx-mqtt';
import { Subscription } from 'rxjs';

@Component({
    selector: 'app-root',
    template: `
    <h1>MQTT Test</h1>
    <p>Otrzymana wiadomość: {{ message }}</p>
  `
})
export class AppComponent {
    private subscription!: Subscription;
    public message: string = '';

    constructor(private mqttService: MqttService) {
        // Subskrybuj temat
        this.subscription = this.mqttService.observe('test/topic').subscribe((message: IMqttMessage) => {
            this.message = message.payload.toString();
            console.log(`Otrzymano wiadomość z tematu ${message.topic}: ${this.message}`);
        });
    }

    ngOnDestroy() {
        // Usuń subskrypcję przy zamykaniu komponentu
        this.subscription.unsubscribe();
    }

    publishMessage(): void {
        const topic = 'test/topic';
        const message = 'Hello MQTT!';
        this.mqttService.unsafePublish(topic, message, { qos: 1 });
        console.log(`Wysłano wiadomość: ${message} na temat ${topic}`);
    }
}
