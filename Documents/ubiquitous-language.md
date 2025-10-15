# Ubiquitous Language
This document defines the core domain concepts used across the system.

---
A user can: 

## Device
- A user can connect a new device
- A user can disconnect a device
- A user can rename a device
- A user can perform Device Actions

## Group
- A user can create a group
- A user can delete a group
- A user can add a device to a group
- A user can remove a device from a group
- A user can rename a group
- A user can activate all devices that can turn on within the group
- A user can deactivate all devices that can turn off within the group

## Mood
- A user can create a mood
- A user can delete a mood
- A user can add a device to a mood
- A user can remove a device from a mood
- A user can add a group of devices to the mood
- A user can remove a group of devices from the mood
- A user can activate a mood
- A user can deactivate a mood
- A user can set a mood trigger
- A user can edit a mood trigger
- A user can delete a mood trigger
- A user can set mood duration
- A user can rename mood

## Trigger
- A user can create a trigger
- A user can delete a trigger
- A trigger can be of type: schedule, device status, toggle

A Device can have multiple Actions