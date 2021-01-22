import Config from '../config'

class WSClient {
    
    constructor() {
        this.endpoint = Config.BackendWebSocketEndpoint;

        this.connect = this.connect.bind(this);
        this.disconnect = this.disconnect.bind(this);
    }

    onConnect(connectHandler) {
        this.connectHandler = connectHandler;
    }

    onDisconnect(disconnectHandler) {
        this.disconnectHandler = disconnectHandler;
    }

    onMessage(messageHandler) {
        this.messageHandler = messageHandler;
    }

    send(message) {
        this.webSocket.send(JSON.stringify(message));
    }

    connect() {
        const self = this;
        this.webSocket = new WebSocket(this.endpoint);
        this.webSocket.onopen = function() {
            console.log('Connected');
            if(self.connectHandler) {
                self.connectHandler();
            }
          }

        this.webSocket.onclose = function(event) {
            if (event.wasClean) {
                console.log('Clean close');
            } else {
                console.log('connection issue')
            }
            console.log('Code: ' + event.code + ' Reason: ' + event.reason);

            if(self.disconnectHandler){
                self.disconnectHandler();
            }
        };

        this.webSocket.onmessage = function(event) {
            if(self.messageHandler){
                self.messageHandler(event);
            }
        }
    }

    disconnect() {
        this.webSocket.close();
    }
}

export default WSClient;