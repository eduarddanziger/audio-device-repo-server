# Audio Device REST API Documentation

This API provides endpoints for managing audio devices.

---

## Get All Audio Devices

Retrieves a list of all available audio devices.

**GET /api/AudioDevices**

**Response:**  
- (succ) 200 OK  
```json
[
  {
    "pnpId": "8A7FB8B8-37CC-4053-9BC7-F526E3B64892",
    "hostName": "e0c9035898dd52fc65c41454cec9c4d2611bfb37",  // Hashed
    "name": "Bluetooth Headset",
    "flowType": "Render",
    "renderVolume": 80,
    "captureVolume": 0,
    "updateDate": "2023-12-01T14:30:00Z",
    "deviceMessageType": "Confirmed"
  }
]
```

---

## Get Audio Device by Key

Retrieves a single audio device by its `pnpId` and `hostName` (hostName will be hashed internally).

**GET /api/AudioDevices/{pnpId}/{hostName}**

**Responses:**
- (succ) 200 OK  
```json
{
  "pnpId": "8A7FB8B8-37CC-4053-9BC7-F526E3B64892",
  "hostName": "e0c9035898dd52fc65c41454cec9c4d2611bfb37",
  "name": "Bluetooth Headset",
  "flowType": "Render",
  "renderVolume": 80,
  "captureVolume": 0,
  "updateDate": "2023-12-01T14:30:00Z",
  "deviceMessageType": "Confirmed"
}
```
- or (fail) 404 Not Found

---

## Add Audio Device

Adds a new audio device to the system.

**POST /api/AudioDevices**

**Request body:**  
```json
{
  "pnpId": "8A7FB8B8-37CC-4053-9BC7-F526E3B64892",
  "hostName": "9c4d2611bfb37",
  "name": "Bluetooth Headset",
  "flowType": "Render",
  "renderVolume": 80,
  "captureVolume": 0,
  "updateDate": "2023-12-01T14:30:00Z",
  "deviceMessageType": "Confirmed"
}
```

**Responses:**
- (succ) 201 Created  
```json
{
  "pnpId": "8A7FB8B8-37CC-4053-9BC7-F526E3B64892",
  "hostName": "host4",
  "name": "Bluetooth Headset",
  "flowType": "Render",
  "renderVolume": 80,
  "captureVolume": 0,
  "updateDate": "2023-12-01T14:30:00Z",
  "deviceMessageType": "Confirmed"
}
```
  Location header will point to `/api/AudioDevices/{pnpId}/{hashedHostName}`.

- or (fail) 400 Bad Request

---

## Remove Audio Device

Removes an audio device from the system.

**DELETE /api/AudioDevices/{pnpId}/{hostName}**

**Responses:**
- (succ) 204 No Content (removal reports success, even if device did not exist)

---

## Update Device Volume

Updates the volume of a specific audio device.

**PUT /api/AudioDevices/{pnpId}/{hostName}**

**Request body:**  
```json
{
  "updateDate": "2023-12-01T15:00:00Z",
  "deviceMessageType": "VolumeRenderChanged",
  "volume": 100
}
```
- The request body must match the `VolumeChangeMessage` model with valid enum values:
  - `deviceMessageType`: "VolumeRenderChanged" or "VolumeCaptureChanged"

**Responses:**
- (succ) 204 No Content

- or (fail) 400 Bad Request

---

## Search Audio Devices

Search for audio devices by a query string (host name or its hash).

**GET /api/AudioDevices/search?query={query}**

**Query parameters:**
- `query`: The search string (substring of device descriotion)

**Response:**
- (succ) 200 OK  
```json
[
  {
    "pnpId": "8A7FB8B8-37CC-4053-9BC7-F526E3B64892",
    "hostName": "e0c9035898dd52fc65c41454cec9c4d2611bfb37",
    "name": "Bluetooth Headset",
    "flowType": "Render",
    "renderVolume": 80,
    "captureVolume": 0,
    "updateDate": "2023-12-01T14:30:00Z",
    "deviceMessageType": "Discovered"
  }
]
```

---

## Messages

### EntireDeviceMessage

| Property           | Type              | Description                                    |
|--------------------|-------------------|------------------------------------------------|
| pnpId              | string | Unique identifier for the audio device         |
| hostName           | string | Name of the host computer (saved in a hashed form)   |
| name               | string | Display name of the audio device               |
| flowType           | enum   | "Render", "Capture", "RenderAndCapture" |
| renderVolume       | int (0-1000)      | Output volume level of the audio device        |
| captureVolume      | int (0-1000)      | Input volume level of the audio device         |
| updateDate         | ISO 8601 datetime | timestamp of the last change       |
| deviceMessageType  | enum   | "Confirmed", "Discovered"                     |

### VolumeChangeMessage

| Property           | Type              | Description                                    |
|--------------------|-------------------|------------------------------------------------|
| updateDate         | ISO 8601 datetime | Update timestamp                    |
| deviceMessageType  | enum   | "VolumeRenderChanged", "VolumeCaptureChanged"  |
| volume             | int    | New volume value                               |

---

