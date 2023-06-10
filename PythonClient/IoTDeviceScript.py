import logging
import time
import sys
import json
import threading
import requests

import requests
from signalrcore.hub_connection_builder import HubConnectionBuilder

authUrl = "https://authservice.greenflower-4193fb10.northeurope.azurecontainerapps.io/api/auth/device"
devicesUrl = "https://devicesservice.greenflower-4193fb10.northeurope.azurecontainerapps.io"
#devicesUrl = "http://localhost:8082"

def send(data):
    try:
        code = data[0]['code']
        exec(code)
    except Exception as e:
        import json
        s = str(e)
        print(s)
        id = data[0]['id']
        error = {
        "error": s,
        "id": id,
        }
        jerror = json.dumps(error)
        hub_connection.send("SendError", [jerror])
    time.sleep(1)

def change(data):
    print("Fields Changed")
    global isChanged
    isChanged = 1

def signalr_core_login(connectionString):
    d={'connectionString': connectionString}
    response = requests.post(authUrl,params={"connectionString" : connectionString}, verify=False)
    return response.json()["token"]


argConnStr = sys.argv[1]
connection_string = argConnStr
bearer = signalr_core_login(connection_string)

def getDeviceFields(connectionString):
    print(bearer)
    r =requests.get( devicesUrl + '/api/devices/GetConnDeviceFields/',params={"connectionString" : connectionString}, headers={"Authorization" : "Bearer " + bearer})
    print(r.reason)
    fields = r.json()
    return fields


def execFieldCode(field):
    import json
    try:
        print(field['code'])
        exec(field['code'], globals())
        js = {
        field['name']: globals()[field['name']]
        }
        y = json.dumps(js)
        hub_connection.send("SendMessage", [y])
    except Exception as e:
        s = str(e)
        print(s)
        id = field['id']
        error = {
        "error": s,
        "id": id,
        }
        jerror = json.dumps(error)
        hub_connection.send("SendFieldError", [jerror])
        return field

hub_connection = HubConnectionBuilder()\
    .with_url(devicesUrl + "/hubs/message",
	options={
	"headers": {
	    "Authorization" : "Bearer " + bearer
	}
	})\
    .with_automatic_reconnect({
        "type": "raw",
        "keep_alive_interval": 60,
        "reconnect_interval": 1,
        "max_attempts": 100
    }).build()
	
hub_connection2 = HubConnectionBuilder()\
    .with_url(devicesUrl + "/hubs/presence",
	options={
	"headers": {
	    "Authorization" : "Bearer " + bearer
	}
	})\
    .with_automatic_reconnect({
        "type": "raw",
        "keep_alive_interval": 60,
        "reconnect_interval": 1,
        "max_attempts": 100
    }).build()


hub_connection.on_open(lambda: print("connection opened and handshake received ready to send messages"))
hub_connection.on_close(lambda: print("connection closed"))

hub_connection2.on_open(lambda: print("connection opened and handshake received ready to send messages"))
hub_connection2.on_close(lambda: print("connection closed"))

hub_connection.start()
hub_connection2.start()
isChanged = 0
fields = getDeviceFields(connection_string)
hub_connection.on("InvokedAction", send) 
hub_connection.on("FieldChanged", change) 

while isChanged == 0:
    for x in fields:
        thread = threading.Thread(target=execFieldCode, args=(x,), daemon=True)
        thread.start()
    time.sleep(10)
    if isChanged == 1:        
        fields = getDeviceFields(connection_string)
        print("one")
        isChanged = 0
print("Finished")

time.sleep(10000)


