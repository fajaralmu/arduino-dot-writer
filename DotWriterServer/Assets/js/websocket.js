let websocket = null;

//Websocket Status List
// 0 	CONNECTING 	Socket has been created. The connection is not yet open.
// 1 	OPEN 	The connection is open and ready to communicate.
// 2 	CLOSING 	The connection is in the process of closing.
// 3 	CLOSED 	The connection is closed or couldn't be opened.
function getWebSocketStatusString() {
    if (null == websocket) return "N/A";
    switch (websocket.readyState) {
        case WebSocket.CONNECTING: return 'CONNECTING'
        case WebSocket.OPEN: return 'OPEN'
        case WebSocket.CLOSING: return 'CLOSING'
        case WebSocket.CLOSING: return 'CLOSING'
    
        default:
            return "N/A";
    }
}

function connectWebsocket(url, onmessage, onopen = null, onclose = null, onerror = null){
    if (null != websocket) {
        if (websocket.readyState == WebSocket.OPEN) {
            console.warn("Websocket is already opened");
            return;
        }
    }
    websocket = new WebSocket(url);
    websocket.onopen = (e) => {
        console.info("websocket opened");
        if (onopen) onopen(e);
    }
    websocket.onmessage = (e) => {
        if (onmessage) onmessage(e);
    }
    websocket.onclose = (e) => {
        console.info("Websocket closed: ", e);
        websocket = null;
        if (onclose) onclose(e);
    }
    websocket.onerror = (e) => {
        console.info("Websocket error: ", e);
        if (onerror) onerror(e);
    }
}

function disconnectWebsocket() {
    if (websocket) {
        websocket.close();
    }
}

function printWsStatusInfo() {
    const wsStatusInfo = document.getElementById("ws-status-info");
    if (!wsStatusInfo) return;
    wsStatusInfo.innerHTML =  getWebSocketStatusString();
}