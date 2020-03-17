using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using csmatio.io;
using csmatio.types;
using Newtonsoft.Json;

namespace EEGDataFormatter
{
    class EEGDataIO
    {
        
        public long RefTime { get; set; } // "Time zero" in milliseconds
        public String DataDir { get; set; } // File directory of data directory
        public String[] FileNames { get; set; } // List of filenames in time order

        List<InputDataModel> EEGDataList { get; set; } // List of input data

        // Reads and writes data from specific directory, over certain specified time
        public EEGDataIO(String dataDir, long expStartTime)
        {
            // Setting directory
            DataDir = dataDir;

            // List of filenames in time order
            FileNames = Directory.GetFiles(DataDir, "*")
                         .Select(Path.GetFileName)
                         .OrderBy(f => f)
                         .ToArray();

            // Getting time zero
            RefTime = expStartTime;
        }

        public void LoadData(long startTime, long endTime)
        {
            // TODO: make this load only data between given startTime, endTime

            // File start and end times
            long fStartTime = startTime + RefTime; long fEndTime = endTime + RefTime;

            // Corresponding index range
            int startIndex = 0; int endIndex = 1;
            bool InRange = false;
            for (int i = 0; i < FileNames.Length; i++)
            {
                String[] digits = Regex.Split(FileNames[i], @"\D+"); 
                if (!InRange && long.Parse(digits[0]) >= fStartTime)
                {
                    startIndex = Math.Max(0, i-1) ; // start on file containing start time
                    InRange = true;
                }
                if (InRange && long.Parse(digits[1]) >= fEndTime)
                {
                    endIndex = i; // end on file containing end time
                    break;
                }

            }

            int nFiles = endIndex - startIndex + 1;

            Console.WriteLine("Reading " + nFiles.ToString() + " files");

            // Getting list of files
            List<InputDataModel> fileDataList = new List<InputDataModel>();
            for (int i = 0; i < nFiles; i++)
            {
                fileDataList.AddRange(DataListFromFile(FileNames[i + startIndex]));
            }

            // Extracting list of data
            EEGDataList = new List<InputDataModel>(
                fileDataList.FindAll(t => 
                (double.Parse(t.SystemMillisecond) > fStartTime && double.Parse(t.SystemMillisecond) < fEndTime)
                ));
        }

        public void WriteToMat(String writePath)
        {
            // Columns: data for each channel at given time
            // Rows: EEG amplitudes for given channel across all times

            if (EEGDataList.Count == 0)
            {
                return;
            }

            // Generating list of data to be written
            double[][] matData = new double[EEGDataList[0].Sensors.Count()][];
            double[][] times = new double[1][];
            times[0] = EEGDataList.Select(x=>double.Parse(x.SystemMillisecond)).ToArray();

            // for each sensor
            for (int i = 0; i < EEGDataList[0].Sensors.Count(); i++)
            {
                // write amplitude for all times into double array
                double[] amplitudes = new double[EEGDataList.Count()];
                for (int j = 0; j < EEGDataList.Count(); j++)
                {
                    Sensor sensor = EEGDataList[j].Sensors[i];

                    amplitudes[j] = sensor.Value;
                }
                matData[i] = amplitudes;
            }

            MLDouble mlAmplitudes = new MLDouble("amplitudes", matData);
            MLDouble mlTimes = new MLDouble("times", times);
            List<MLArray> mlList = new List<MLArray>();
            mlList.Add(mlAmplitudes);
            mlList.Add(mlTimes);
            MatFileWriter mfw = new MatFileWriter(writePath, mlList, false);
        }

        public List<InputDataModel> DataListFromFile(String fileName)
        {
            // Getting start and end times for data file
            String[] digits = Regex.Split(fileName, @"\D+");
            long fStartTime = long.Parse(digits[0]);
            long fEndTime = long.Parse(digits[1]);

            // Loading JSON string from file
            String escapedJsonString = File.ReadAllText(DataDir + fileName);
            // First deserialise: removing escape characters. Each string is from single time
            List<String> unescapedJsonString = JsonConvert.DeserializeObject<List<String>>(escapedJsonString);
            
            // Second deserialise: Making list of input points
            List<InputDataModel> DataList = unescapedJsonString.Select(Str => JsonConvert.DeserializeObject<InputDataModel>(Str)).ToList();

            return DataList;
        }

    }
}
