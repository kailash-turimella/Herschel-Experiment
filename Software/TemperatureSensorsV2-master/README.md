# TemperatureSensorsV2



TemperatureSensorsV1 software
 - GUI code written by Andric Li.
 - Driver code written by Timothy Bodrov.
 - Internship for DRS Technologies, 2019-2020.

TemperatureSensorsV2 software
 - Implemented GUI and Driver code from TemperatureSensorsV1
 - New GUI, Driver, and Calibration code written by Kailash Turimella and Ryan Bouzan 
 - Internship for DRS Technologies, 2020-2021.

Notes:
- The official manual for the MccDaq recommends that we let it sit and warm up for at least 30 minutes before starting the temperature reading process.
- Make sure to run InstaCal and set up the DAQ configuration file before running our program. It will not work without it!
- We are using RTD thermosensors. Make sure to set this in InstaCal. The current configuration has channels 0-4 on [2 wire 2 sensor RTD] , and channel 5 has [1 wire 1 sensor RTD]
- Select  *Show Connections* in InstaCal to see wiring diagram
- See trello board for official documentation of this program and guides for software neccesary software installation


