# Audio Device REST API Documentation

This API provides endpoints for managing audio devices.

## Get All Audio Devices

Retrieves a list of all available audio devices.

**GET /api/AudioDevices**

**Response**: 200 OK with an array of audio device objects.

Example response:
```json
[
	{
		"id": 1,
		"name": "Speakers",
		"type": "Output",
		"isDefault": true
	},
	{
		"id": 2,
		"name": "Microphone",
		"type": "Input",
		"isDefault": false
	}
]
```
## Add Audio Device

Adds a new audio device to the system.

**POST /api/AudioDevices**

**Body**: Audio device object

Example request:

```json
{
  "pnpId": "{8A7FB8B8-37CC-4053-9BC7-F526E3B64892}",
  "name": "Bluetooth Headset",
  "volume": 80,
  "lastSeen": "2023-12-01T14:30:00",
  "hostName": "Host4"
}
```

**Response**: 201 Created with the newly created audio device.

## Remove Audio Device

Removes an audio device from the system.

**DELETE /api/AudioDevices/\{pnpId\/\{hostName\}**


**Parameters**:
- `pnpId`: The unique identifier of the audio device

**Response**: 204 No Content

## Update Device Volume

Updates the volume of a specific audio device.

**PUT /api/AudioDevices/{pnpId}/volume**

**Parameters**:
- `pnpId`: The unique identifier of the audio device

**Body**: Integer value representing the volume level

Example request:
100

**Response**: 204 No Content

## Object Model

### EntireDeviceMessage

| Property | Type | Description |
|----------|------|-------------|
| pnpId | string | Unique identifier for the audio device |
| name | string | Display name of the audio device |
| volume | number | Volume level of the audio device |
| lastSeen | string (ISO 8601 date) | Timestamp when the device was last detected |
| hostName | string | Name of the host computer |












