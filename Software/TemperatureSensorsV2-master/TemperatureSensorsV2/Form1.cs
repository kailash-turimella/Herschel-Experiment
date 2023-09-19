/*
 * (C) Copyright 2021 Ryan Bouzan and others.
 * Software for Herschel IR Demo - Spring 2021
 * 
 * 
 * Contributors:
 *     Kailash Turimella
 *     Ryan Bouzan
 */


using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

using MccDaq;

namespace TemperatureSensorsV2
{
    public partial class Form1 : Form
    {
        public MccBoard DaqBoard = new MccBoard(0);
        private ErrorInfo ULStat;
        private int UsesEXPs = 0;

        public TempScale temperatureScale = TempScale.Fahrenheit;
        public Label[] lblShowData;
        Random rnd = new Random();

        private float[] DataBuffer = new float[5]; // array to hold  the temperatures
        public int LowChan = 0;
        public int HighChan = 5;
        public int minScaled = 60;
        public int maxScaled = 90;
        public int readDuration;
        public int chartXValueCounter = 0;
        public int resizeCounter = 0;
        public int numberOfTabs = 5;
        public int mainCounter = 0;
        public int initCounter = 0;
        


        public string tempScaleString = "°F";
        
        public bool errorOverride = false;
        public bool isReading = false;
        public bool isChecked = false;
        
        #region Calibration Variables

        public string calInfo = "";

        public bool calibrating;

        public int calCounter = 0;

        public float tempCal;
        public float sample = 0;


        private float[] calibrationBuffer = new float[5];
        public float[] offsetArray = new float[5];

        public float[] speedArray = new float[25];
        public float[] calArrayColor0 = new float[5];
        public float[] calArrayColor1 = new float[5];
        public float[] calArrayColor2 = new float[5];
        public float[] calArrayColor3 = new float[5];
        public float[] calArrayColor4 = new float[5];
        #endregion

        public Form1()
        {
            //Custom components include the thermometers and the "LED" textbox. They are components from the BERGtools.dll file, instructions to add them to the toolbox are in the documentation for this project
            InitializeComponent();
            ChartLoad();
            tabControl1.Location = new Point(-7, -27);
            UsesEXPs = 0;

        }

        private void Form1_Load(object sender, EventArgs e)
        {

            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            // WindowState = FormWindowState.Maximized;

            prevButton.Hide();
            demoButton.Hide();
            returnButton.Hide();
            resetButton.Hide();
            targetCalConf.Hide();
            //rotates next arrow image 180 degrees
            nextButton.BackgroundImage.RotateFlip((RotateFlipType.Rotate90FlipXY));
            nextButton.BackgroundImage.RotateFlip((RotateFlipType.Rotate90FlipXY));

            //creates array of labels for each temperature
            lblShowData = (new Label[] { label1, label2, label3, label4, label5 });
            timeDDL.SelectedIndex = 0;

        }


        public void initializeDAQ(int boardNum)
        {
            MccBoard DaqBoard = new MccBoard(boardNum);
            ULStat = DaqBoard.BoardConfig.GetUsesExps(out UsesEXPs);

        }



        #region Buttons code

        private void noErrorBox_CheckedChanged(object sender, EventArgs e)
        {
            errorOverride =! errorOverride;
        }

        
        private void optionsButton_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(optionsTab);
            nextButton.Hide();
            returnButton.Show();
            demoButton.Show();
        }

        private void sampleButton_Click(object sender, EventArgs e)
        {
            //gets a temperature value from a random sensor
            sample = 0;
            MccDaq.ThermocoupleOptions Options = 0;
            DaqBoard.TIn(rnd.Next(0, 4), temperatureScale, out sample, Options);
            targetCalText.Text = Convert.ToString(sample);

        }


        private void targetCalText_Click(object sender, EventArgs e)
        {
            if (targetCalText.Text != "")
            {
                targetCalConf.Show();
                sample = Convert.ToSingle(targetCalText.Text);
                oneSecond.Enabled = true;
            }
           
        }

        private void startcalibration_Click(object sender, EventArgs e)
        {

            if (!calibrating)
            {
                if (targetCalText.Text == "")
                {
                    throwError(2, 0);
                }
                else
                {
                    
                    calIndicator.Text = "*";
                    calibrating = true;

                    if (startcalibration.Text != "Show Offset")
                    {

                        if (quickCal.Checked)
                        {
                            calibrateLED.OnText = "Time remaining: " + 4 + " second(s)";
                            calibrateArrayTimer.Interval = 1000;
                        }
                        else
                        {
                            calibrateLED.OnText = "Time remaining: " + 12  + " second(s)";

                            calibrateArrayTimer.Interval = 3000;
                        }

                        MessageBox.Show("Insure sensors have consistent conditions \n" + "Press OK to start", "IMPORTANT", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        startcalibration.Text = "Abort";
                        calibrateLED.Value = true;
                        calibrateArrayTimer.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show(calInfo, "Calibration Success! Fast calibration: " + quickCal.Checked , MessageBoxButtons.OK, MessageBoxIcon.Information);
                        calibrating = false;
                    }

                }
            }
            else if(calibrating && calCounter != 0)
            {
                resetcalibrationButton.PerformClick();
                MessageBox.Show("calibration was aborted!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

               
            }
           

            //creates an array for each sensor (shortens calibration time from 25 seconds to 5 seconds





        }

        //handles all user related actions (clicking buttons)
        private void showTabs_CheckedChanged(object sender, EventArgs e)
        {

            if (isChecked)
            {
                tabControl1.Location = new System.Drawing.Point(-9, -31);
                Debug.WriteLine("showing tabset!");
            }
            else
            {
                tabControl1.Location = new System.Drawing.Point(0, 0);
                Debug.WriteLine("not showing tabset");
            }
            isChecked = !isChecked;
        }


        #region Temperature Scale Buttons

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            temperatureScale = TempScale.Fahrenheit;
            tempScaleString = "°F";
            tempTableTitle.Text = "Temperature (°F)";
            minScaled = 60;
            maxScaled = 95;
            thermometer1.YMinimum = 60D;
            thermometer1.YMaximum = 90D;
            thermometer2.YMinimum = 60D;
            thermometer2.YMaximum = 90D;
            thermometer3.YMinimum = 60D;
            thermometer3.YMaximum = 90D;
            thermometer4.YMinimum = 60D;
            thermometer4.YMaximum = 90D;
            thermometer5.YMinimum = 60D;
            thermometer5.YMaximum = 90D;

            chart1.ChartAreas[0].AxisY.Title = "Temperature (°F)";

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            temperatureScale = TempScale.Celsius;
            tempScaleString = "°C";
            tempTableTitle.Text = "Temperature (°C)";
            minScaled = 10;
            maxScaled = 40;
            thermometer1.YMinimum = 15D;
            thermometer1.YMaximum = 32D;
            thermometer2.YMinimum = 15D;
            thermometer2.YMaximum = 32D;
            thermometer3.YMinimum = 15D;
            thermometer3.YMaximum = 32D;
            thermometer4.YMinimum = 15D;
            thermometer4.YMaximum = 32D;
            thermometer5.YMinimum = 15D;
            thermometer5.YMaximum = 32D;

            chart1.ChartAreas[0].AxisY.Title = "Temperature (°C)";

        }


        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            temperatureScale = TempScale.Kelvin;
            tempScaleString = "°K";
            tempTableTitle.Text = "Temperature (°K)";
            minScaled = 260;
            maxScaled = 290;
            thermometer1.YMinimum = 270D;
            thermometer1.YMaximum = 300D;
            thermometer2.YMinimum = 270D;
            thermometer2.YMaximum = 300D;
            thermometer3.YMinimum = 270D;
            thermometer3.YMaximum = 300D;
            thermometer4.YMinimum = 270D;
            thermometer4.YMaximum = 300D;
            thermometer5.YMinimum = 270D;
            thermometer5.YMaximum = 300D;

            chart1.ChartAreas[0].AxisY.Title = "Temperature (°K)";

        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            temperatureScale = TempScale.NoScale;
            tempScaleString = "NS";
            tempTableTitle.Text = "Temperature (NS)";
            minScaled = 0;
            maxScaled = 1000;
            thermometer1.YMinimum = 270D;
            thermometer1.YMaximum = 300D;
            thermometer2.YMinimum = 270D;
            thermometer2.YMaximum = 300D;
            thermometer3.YMinimum = 270D;
            thermometer3.YMaximum = 300D;
            thermometer4.YMinimum = 270D;
            thermometer4.YMaximum = 300D;
            thermometer5.YMinimum = 270D;
            thermometer5.YMaximum = 300D;

            chart1.ChartAreas[0].AxisY.Title = "Temperature (?)";

        }

        #endregion


        //close button
        private void exitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        
        //next and previous button names must start with "n" or "p" and have + number of tab


        private void Switch_Tab(object sender, EventArgs e)
        {

            var button = (Button)sender;

            int currentTab = tabControl1.SelectedIndex;
            currentTab++;
            string directionLetter = Convert.ToString(button.Name.Substring(0, 1));

            if (directionLetter == "n")
            {
                if (currentTab < numberOfTabs)
                {
                    //checks if button is on the last tab, current tab variable must be manually updated
                    currentTab++;
                }
            }
            else if (directionLetter == "p")
            {
                if (currentTab > 1)
                {
                    currentTab--;
                }
            }
            else
            {
                Debug.WriteLine("Incorrect naming or invalid tab selection");
            }
            string tabPage;
            tabPage = "tabPage" + currentTab;
            tabControl1.SelectTab(tabPage);


            //logic for showing various buttons 
            if (Convert.ToInt32(tabControl1.SelectedIndex) == 0)
            {
                prevButton.Hide();

            }
            else if (Convert.ToInt32(tabControl1.SelectedIndex) == numberOfTabs - 1)
            {
                nextButton.Hide();
                demoButton.Show();
            }
            else if (Convert.ToInt32(tabControl1.SelectedIndex) == 1)
            {
                this.nextButton.Text = "Next";
                prevButton.Show();
                prevButton.Text = "Return";
            }
            else
            {
                nextButton.Show();
                prevButton.Show();
                demoButton.Hide();
                prevButton.Text = "Previous";
            }
        }

            private void returnButton_Click(object sender, EventArgs e)
            {
                tabControl1.SelectTab(tabPage1);
                nextButton.Text = "Start";
                returnButton.Hide();
                nextButton.Show();
                prevButton.Hide();
            }



            private void demoButton_Click(object sender, EventArgs e)
            {
                tabControl1.SelectTab(tablePage);
                demoButton.Hide();
                returnButton.Show();
            }


            //called start in GUI handles initiating data collection
            public void testButton_Click(object sender, EventArgs e)
            {
                string readDurationChars;
                var chart = chart1.ChartAreas[0];

                if (isReading)
                {
                    testButton.Text = "Start";
                    readArrayTimer.Enabled = false;
                    readLED.Value = false;
                    readProgress.MarqueeAnimationSpeed = 300;
                    isReading = false;
                }
                else
                {

                    readDurationChars = Convert.ToString(timeDDL.SelectedItem);

                    if (readDurationChars.Substring(0, 1) == "S")
                    {
                        //prompts user to select a time if the Drop down list (DDL) has no integer for the readDuration
                        MessageBox.Show("Please select a time duration from the list");
                    }
                    else
                    {
                        testButton.Text = "Pause";

                        //TODO: optimize this for users who want to record temperatures over 99 seconds since its a substring of 2 characters
                        timeDDL.Hide();
                    chart.AxisY.Minimum = minScaled;
                    chart.AxisY.Maximum = maxScaled;
                        
                    readDuration = Convert.ToInt32(readDurationChars.Substring(0, 2));
                        readProgress.Maximum = readDuration * 100;
                        readLED.OnText = "Waiting for data...";
                        readProgress.MarqueeAnimationSpeed = 0;
                        readArrayTimer.Enabled = true;
                        readLED.Value = true;
                        isReading = true;
                    chart1.ChartAreas[0].AxisY.Minimum = minScaled;
                    chart1.ChartAreas[0].AxisY.Maximum = maxScaled;
                    } //end else
                } //end else
            }

        private void resetButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {
                chart1.Series[i].Points.Clear();
                lblShowData[i].Text = "";
            }

            thermometer1.TempValue = 0;
            thermometer2.TempValue = 0;
            thermometer3.TempValue = 0;
            thermometer4.TempValue = 0;
            thermometer5.TempValue = 0;

            timeDDL.Show();
            testButton.Show();
            resetButton.Hide();
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chartXValueCounter = 0;
            readProgress.Value = 0;
            readArrayTimer.Enabled = false;
            readLED.Value = false;
            isReading = !isReading;
            chart1.ChartAreas[0].AxisY.MajorGrid.LineWidth = 1;
            chart1.ChartAreas[0].AxisX.MajorGrid.LineWidth = 1;
            
        }

        private void resetcalibrationButton_Click(object sender, EventArgs e)
        {
            if(!calibrating)
                MessageBox.Show("Calibration cleared");


            calibrateArrayTimer.Enabled = false;
            startcalibration.Text = "Calibrate Sensors";
            calibrating = false;
            calibrateLED.Value = false;
            calIndicator.Text = "";
            calCounter = 0;
            Array.Clear(offsetArray, 0, offsetArray.Length);
            Array.Clear(calibrationBuffer, 0, calibrationBuffer.Length);

        }


        #endregion

        #region Chart code


        //loads the chart, defines the chart area and series for each of the temperature colors
        void ChartLoad()
        {
            //assign the chart in the designer to a variable
            var chart = chart1.ChartAreas[0];

            //clearing the axis labels
            chart.AxisX.LabelStyle.Format = "";
            chart.AxisY.LabelStyle.Format = "";
            chart.AxisX.LabelStyle.IsEndLabelVisible = true;

            chart.AxisX.Minimum = 0;
            chart.AxisY.Minimum = minScaled;
            chart.AxisY.Maximum = maxScaled;

            chart.AxisX.Interval = 1;
            chart.AxisY.Interval = 1;

            chart.AxisX.Title = "Time (s)";
            chart.AxisY.Title = "Temperature (°F)";

            chart1.Series[0].Name = "Infrared";
            chart1.Series[0].Color = Color.FromArgb(128, 0, 0); //maroon
            chart1.Series[0].BorderWidth = 3;

            chart1.Series.Add("Blue");
            chart1.Series[1].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series[1].Color = Color.Blue;
            chart1.Series[1].BorderWidth = 3;

            chart1.Series.Add("Green");
            chart1.Series[2].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series[2].Color = Color.Green;
            chart1.Series[2].BorderWidth = 3;

            chart1.Series.Add("Red");
            chart1.Series[3].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series[3].Color = Color.Red;
            chart1.Series[3].BorderWidth = 3;

            chart1.Series.Add("Room Temperature");
            chart1.Series[4].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series[4].BorderWidth = 3;
            chart1.Series[4].Color = Color.Black;


        }


        //resizes chart every 4 seconds based on highest and lowest values +/- 5 degrees
        private void resizeChart()
        {
            var chart = chart1.ChartAreas[0];


            float tempLow = DataBuffer[0];
            float tempHigh = DataBuffer[0];

            for (var i = 0; i < DataBuffer.Length; i++)
            {
                if (DataBuffer[i] >= tempHigh)
                {
                    tempHigh = DataBuffer[i];
                }
                if (DataBuffer[i] <= tempLow)
                {
                    tempLow = DataBuffer[i];
                }
            }

           tempLow = (float)Math.Round(Convert.ToDouble(tempLow));
           tempHigh = (float)Math.Round(Convert.ToDouble(tempHigh));

            if((tempHigh-tempLow) < 2  )
            {
                chart.AxisY.Minimum = Convert.ToInt32(tempLow) - 3;
                chart.AxisY.Maximum = Convert.ToInt32(tempHigh) + 3;
            }
            else
            {
                chart.AxisY.Minimum = Convert.ToInt32(tempLow) - 5;
                chart.AxisY.Maximum = Convert.ToInt32(tempHigh) + 5;
            }

            





        }
        #endregion

        #region Data collection code

        private bool calibrateArray(int channelSwitcher)
        {


            bool blnSuccess = false;
            MccDaq.ThermocoupleOptions Options = 0;

            for (var i = 0; i < HighChan; i++)
            {
                ErrorInfo calStat = DaqBoard.TIn(i, temperatureScale, out tempCal, Options);

                if (calStat.Value == ErrorInfo.ErrorCode.NoErrors)
                {
                    blnSuccess = true;
                }
                else
                {
                    blnSuccess = false;
                    startcalibration.PerformClick();
                }
              //  Debug.WriteLine("I: " + i + "channelSwitcher: "+ channelSwitcher );
                switch(i)
                {
                    case 0:
                        calArrayColor0[i + channelSwitcher] = tempCal;    
                        break;
                    case 1:
                        calArrayColor1[i + (channelSwitcher-1)] = tempCal;
                        break;

                    case 2:
                        calArrayColor2[i + (channelSwitcher-2)] = tempCal;
                        break;

                    case 3:
                        calArrayColor3[i + (channelSwitcher - 3)] = tempCal;
                        break;

                    case 4:
                        calArrayColor4[i + (channelSwitcher - 4)] = tempCal;
                        break;


                    default:
                        break;
                }
               // Debug.WriteLine(calArrayColor0[i]);

            }

            



            return blnSuccess;
        }


        private void calibrateArrayTimer_Tick(object sender, EventArgs e)
        {
            calInfo = "";
            // Debug.WriteLine("calcounter: " + calCounter);


            calibrateArrayTimer.Enabled = false;


            if (quickCal.Checked)
                 calibrateLED.OnText = "Time remaining: " + (3 - calCounter) + " second(s)";
            else
                calibrateLED.OnText = "Time remaining: " + (12 - calCounter * 3) + " second(s)";



            if (calibrateArray(calCounter))
            {
                calibrateArrayTimer.Enabled = true;
            }


            if (calCounter == HighChan-1)
            {
                calibrateArrayTimer.Enabled = false;
                //couldnt figure out how to do this in a loop 
                calibrationBuffer[0] = getAverage(calArrayColor0);
                calibrationBuffer[1] = getAverage(calArrayColor1);
                calibrationBuffer[2] = getAverage(calArrayColor2);
                calibrationBuffer[3] = getAverage(calArrayColor3);
                calibrationBuffer[4] = getAverage(calArrayColor4);

                for (var i = 0; i < calibrationBuffer.Length; i++)
                {

                    if (useCoefficient.Checked)
                    {

                        offsetArray[i] = sample / calibrationBuffer[i];

                        calInfo += "Color ID: " + i + " has coefficent: x" + offsetArray[i] + "\n";
                    }


                    else
                    {

                        if (calibrationBuffer[i] > sample)
                        {
                            offsetArray[i] = -1 * (calibrationBuffer[i] - sample);

                        }
                        else
                        {
                            offsetArray[i] = Math.Abs(calibrationBuffer[i] - sample);

                        }
                        calInfo += "Color ID: " + i + " has offset: " + offsetArray[i] + " degrees \n";

                    }

                }

               
                //  calibrating = false;
                startcalibration.Text = "Show Offset";
                calibrateArrayTimer.Enabled = false;
                calibrateLED.Value = false;
                calCounter = -1;
                calibrating = false;
                MessageBox.Show(calInfo, "Calibration Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
           
            calCounter++;
               

            




        }

    


        //gets data from the DAQ and stores it in the DataBuffer, channel numbers are manually set 
        //also updates the labels in the table
        private bool ReadTemperatureArray()
        {
            //  Collect the data with MccDaq.MccBoard.TInScan()
            //   Input values will be collected from a range of thermocouples.
            //   The data value will be updated and displayed until a key is pressed.
            //   Parameters:
            //     LowChan    (0)   :the starting channel of the scan
            //     HighChan   (i made it 4 because at the time only 5 RTDs were soldered on to the DAQ)   :the ending channel of the scan
            //     MccScale      :temperature scale (Celsius, Fahrenheit, Kelvin)
            //     DataBuffer()  :the array where the temperature values are collected
            Boolean blnSuccess = false;

            MccDaq.ThermocoupleOptions Options = 0;


            MccDaq.ErrorInfo ULStat = DaqBoard.TInScan(0, 4, temperatureScale, DataBuffer, Options);


            //if a calibration has been performed, offset each databuffer value by amount x
            if (calIndicator.Text == "*")
            {

                for (int h = 0; h < DataBuffer.Length; h++)
                {

                    if (useCoefficient.Checked)
                    {
                        DataBuffer[h] *= offsetArray[h];
                    }
                    else
                    {
                        DataBuffer[h] += offsetArray[h];

                    }

                }

            }


            for (int i = 0; i < lblShowData.Length; i++)
            {
                lblShowData[i].Text = DataBuffer[i].ToString("0.00 ") + tempScaleString; //  print the value


                if (i != 0)
                {
                    if (DataBuffer[i] == 0.00)
                    {
                        throwError(0, 0);
                    }
                }
               if (DataBuffer[i] <= -999)
                {
                    throwError(1, i);

                }
            }

            if (ULStat.Value == MccDaq.ErrorInfo.ErrorCode.NoErrors)
            {
                blnSuccess = true;
            }




            return blnSuccess;


        }
        




        //time interval of 100ms that updates the temperature to a databuffer every 1 second (counts to 10 then resets)

        private void readArrayTimer_Tick(object sender, EventArgs e)
        {

            var chart = chart1.ChartAreas[0];


            readProgress.PerformStep();


            if (mainCounter == 10)
            {



                readArrayTimer.Stop();

                // Read the Temps and only restart the timer if there are no errors
                if (ReadTemperatureArray())
                {
                    readArrayTimer.Start();
                }
                else
                {
                    throwError(0 , 0);
                }
                //detects when selected read time is reached, updates GUI, and sets the graph to show all data and have no internal lines for easier visibility
                if (chartXValueCounter > readDuration)
                {
                    chart.AxisX.Minimum = 0;
                    chart.AxisX.Maximum = readDuration;
                    readLED.Value = false;
                    chart.AxisY.Minimum = minScaled;
                    chart.AxisY.Maximum = maxScaled;
                    readArrayTimer.Enabled = false;


                    chart1.ChartAreas[0].AxisY.MajorGrid.LineWidth = 0;
                    chart1.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
                    resetButton.Show();
                    testButton.Hide();
                    testButton.Text = "Start";


                }
                else
                {
                    //Prevents overload by only showing the latest 20 seconds of data
                    if(chartXValueCounter >=20)
                    {
                        chart.AxisX.Minimum = chartXValueCounter - 20;
                    }
                    else
                    {
                        chart.AxisX.Minimum = 0;
                    }

                    //sets values for thermometers and chart based on temperatures in data buffer
                    chart.AxisX.Maximum = chartXValueCounter;


                    readLED.OnText = "Reading for " + (readDuration - chartXValueCounter) + " seconds";

                    chart1.Series[0].Points.AddXY(chartXValueCounter, DataBuffer[0]); //infrared

                    chart1.Series[1].Points.AddXY(chartXValueCounter, DataBuffer[1]); //red

                    chart1.Series[2].Points.AddXY(chartXValueCounter, DataBuffer[2]); //green

                    chart1.Series[3].Points.AddXY(chartXValueCounter, DataBuffer[3]); //blue

                    chart1.Series[4].Points.AddXY(chartXValueCounter, DataBuffer[4]); //room temp
                  
                    thermometer1.TempValue = DataBuffer[0];
                    thermometer2.TempValue = DataBuffer[1];
                    thermometer3.TempValue = DataBuffer[2];
                    thermometer4.TempValue = DataBuffer[3];
                    thermometer5.TempValue = DataBuffer[4];


                    chartXValueCounter++;
                    resizeCounter++;
                }


                //resizes graph every 4 seconds
                if (resizeCounter == 4)
                {
                    resizeCounter = 0;
                    resizeChart();
                }




                mainCounter = 0;
            }

            mainCounter++;
        }


        private void oneSecond_Tick(object sender, EventArgs e)
        {
            oneSecond.Enabled = false;
            targetCalConf.Hide();

        }

        #endregion

        private float getAverage(float[] arrayToAverage)
        {
            float average = 0;
            for (var i = 0; i < arrayToAverage.Length; i++)
            {
                average += arrayToAverage[i];
            }
            average = average / arrayToAverage.Length;

            return average;

        }

        private void throwError(int errorCode, int colorID)
        {


            if (!errorOverride)
            {

                bool exit = false;
                bool init = false;

                string errorMessage;

                switch (errorCode)
                {

                    case 0:
                        errorMessage = "Board/Sensor Failure - Ensure the board has been properly configured, is plugged in, sensors are wired properly." + "\n" + "Press OK to attempt reinitialization of DAQ.";
                        init = true;
                        initCounter++;
                        if (initCounter == 4)
                            errorMessage = "Too many failed attempts. Application will now exit ";

                        break;
                    case 1:
                        errorMessage = "Sensor with Color ID: " + colorID + " has a short circuit, check the sensor leads. Program will now exit.";
                        exit = true;
                        break;
                    case 2:
                        errorMessage = "Calibration value not set";
                        break;
                    default:
                        errorMessage = "Unknown error";
                        break;

                }

                readLED.Value = false;
                MessageBox.Show(errorMessage, "ERROR (" + errorCode + ")", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (exit || (initCounter == 4))
                    Application.Exit();

                if (init)
                    initializeDAQ(0);

            }
        }
        
        private void reInitBoard_Click(object sender, EventArgs e)
        {
            initializeDAQ(0);
        }
    }



}

