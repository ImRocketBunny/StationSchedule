import { Component, OnInit } from '@angular/core';
import {
  IMqttMessage,
  IMqttServiceOptions,
  MqttService,
  IPublishOptions,
} from 'ngx-mqtt';
import { IClientSubscribeOptions } from 'mqtt-browser';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'MonitorPlatform';
  messages: string[] = [];


  subscription: Subscription | undefined;
  /*constructor(private mqttService: MqttService) {
    this.serial = new NgxSerial(this.dataHandler);
    console.log(mqttService)
    this.subscription = this.mqttService.observe('my/topic').subscribe((message: IMqttMessage) => {
      this.message = message.payload.toString();
      console.log('Received message:', message.payload.toString());
    });
  }*/


  constructor(private mqttService: MqttService) {

    /*this.client = this.mqttService;
    this.createConnection()
    this.doSubscribe()*/
  }
  /*private curSubscription: Subscription | undefined;
  connection = {
    hostname: '127.0.0.1',
    port: 1883,
    path: '',
    clean: true, // Retain session
    connectTimeout: 4000, // Timeout period
    reconnectPeriod: 4000, // Reconnect period
    clientId: 'mqttx_597046f4',
    protocol: 'ws',
  }
  subscription = {
    topic: 'station/main/arrivals',
    qos: 0,
  };
  receiveNews = '';
  qosList = [
    { label: 0, value: 0 },
    { label: 1, value: 1 },
    { label: 2, value: 2 },
  ];
  client: MqttService | undefined;
  isConnection = false;
  subscribeSuccess = false;
  createConnection() {
    // Connection string, which allows the protocol to specify the connection method to be used
    // ws Unencrypted WebSocket connection
    // wss Encrypted WebSocket connection
    // mqtt Unencrypted TCP connection
    // mqtts Encrypted TCP connection
    try {
      this.client?.connect(this.connection as IMqttServiceOptions)
    } catch (error) {
      console.log('mqtt.connect error', error);
    }
    this.client?.onConnect.subscribe(() => {
      this.isConnection = true
      console.log('Connection succeeded!');
    });
    this.client?.onError.subscribe((error: any) => {
      this.isConnection = false
      console.log('Connection failed', error);
    });
    this.client?.onMessage.subscribe((packet: any) => {
      this.receiveNews = this.receiveNews.concat(packet.payload.toString())
      this.messages.push(packet.payload.toString());
      console.log(`Received message ${packet.payload.toString()} from topic ${packet.topic}`)
    })
  }
  doSubscribe() {
    const { topic, qos } = this.subscription
    this.curSubscription = this.client?.observe(topic, { qos } as IClientSubscribeOptions).subscribe((message: IMqttMessage) => {
      this.subscribeSuccess = true
      this.messages.push(message.payload.toString());
      console.log('Subscribe to topics res', message.payload.toString())
    })
  }*/
  ngOnInit() {
    //this.createConnection()
    //this.doSubscribe();
  }
  /*ngOnInit(): void {
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
  }*/
}
