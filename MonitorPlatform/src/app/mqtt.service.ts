import { Injectable } from '@angular/core';
import { MqttClient } from 'mqtt';
import mqtt from 'mqtt';


@Injectable({
  providedIn: 'root',
})
export class MqttService {
  private client: MqttClient;

  constructor() {
    this.client = {} as MqttClient;
    this.connectToBroker();
  }

  


  private connectToBroker(): void {
    const brokerUrl = 'mqtt://127.0.0.1:1883/' // Pełny adres URL brokera MQTT
    const options = {
      clientId: `mqttjs_${Math.random().toString(16).slice(2)}`, // Unikalny clientId
      clean: true, // Utrzymuje sesję czystą po rozłączeniu
      connectTimeout: 4000 // Timeout w ms
    };
    this.client = mqtt.connect(brokerUrl,options);

    this.client.on('connect', () => {
      console.log('Połączono z MQTT!');
    });

    this.client.on('error', (error) => {
      console.error('Błąd połączenia z MQTT:', error);
    });

    this.client.on('message', (topic, message) => {
      console.log(`Wiadomość na temacie ${topic}: ${message.toString()}`);
    });
  }

  public subscribe(topic: string): void {
    if (this.client) {
      this.client.subscribe(topic, (err) => {
        if (err) {
          console.error('Błąd subskrypcji:', err);
        } else {
          console.log(`Subskrybowano temat: ${topic}`);
        }
      });
    }
  }

  public publish(topic: string, message: string): void {
    if (this.client) {
      this.client.publish(topic, message, (err) => {
        if (err) {
          console.error('Błąd publikacji:', err);
        } else {
          console.log(`Opublikowano na temacie ${topic}: ${message}`);
        }
      });
    }
  }
}
