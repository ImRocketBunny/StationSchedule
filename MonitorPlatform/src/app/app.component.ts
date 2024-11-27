import { Component,OnInit } from '@angular/core';
import {
  IMqttMessage,
  IMqttServiceOptions,
  MqttService,
  IPublishOptions,
} from 'ngx-mqtt';
//import { MatSnackBar } from '@angular/material/snack-bar';
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

  


  //subscription: Subscription | undefined;
  /*constructor(private mqttService: MqttService) {
    this.serial = new NgxSerial(this.dataHandler);
    console.log(mqttService)
    this.subscription = this.mqttService.observe('my/topic').subscribe((message: IMqttMessage) => {
      this.message = message.payload.toString();
      console.log('Received message:', message.payload.toString());
    });
  }*/


  constructor(private _mqttService: MqttService/*, private _snackBar: MatSnackBar*/) {
    this.client = this._mqttService;
  }
  private curSubscription: Subscription | undefined;
  connection = {
    hostname: '127.0.0.1',
    port: 1883,
    path: '/mqtt',
    clean: true, 
    connectTimeout: 4000, 
    reconnectPeriod: 4000, 
    protocol: 'ws',
  }
  subscription = {
    topic: 'station/main/arrivals',
    qos: 0,
  };
  publish = {
    topic: 'topictest/browser',
    qos: 0,
    payload: '{ "msg": "Hello, I am browser." }',
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
      this.receiveNews = this.receiveNews.concat([packet.payload.toString(), '\n'].join())
      console.log(`Received message ${packet.payload.toString()} from topic ${packet.topic}`)
    })
  }

  // Subscribe to topic
  doSubscribe() {
    const { topic, qos } = this.subscription
    if (!this.client) {
      //this._snackBar.open('There is no mqtt client available...', 'close');
      return;
    }
    this.curSubscription = this.client.observe(topic, { qos } as IClientSubscribeOptions)
      .subscribe((message: IMqttMessage) => {
        this.subscribeSuccess = true
        const msg = ['Received message: ', message.payload.toString()].join(' ');
        //this._snackBar.open(msg, 'close');
        this.messages.push(message.payload.toString())
        console.log(message.payload.toString())
      });
  }


  doUnSubscribe() {
    this.curSubscription?.unsubscribe()
    this.subscribeSuccess = false
  }

  doPublish() {
    const { topic, qos, payload } = this.publish
    console.log(this.publish)
    this.client?.unsafePublish(topic, payload, { qos } as IPublishOptions)
  }

  destroyConnection() {
    try {
      this.client?.disconnect(true)
      this.isConnection = false
      console.log('Successfully disconnected!')
    } catch (error: any) {
      console.log('Disconnect failed', error.toString())
    }
  }

  
  ngOnInit() {
    this.createConnection();
    this.doSubscribe();
  }

}
