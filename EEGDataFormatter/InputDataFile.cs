using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;

namespace EEGDataFormatter
{
    // Class to read EEG data file
    class InputDataFile
    {
        public const String DataPath = @"C:\Users\cmcdo\OneDrive\Documents\BA Project documents\1\eeg\custom\decrypt\";
        public String FileName;
        public long StartTime { get; set; }
        public long EndTime { get; set; }
        public List<InputDataModel> DataList { get; set; }

        public InputDataFile(String FileName)
        {
            this.FileName = FileName;
            LoadFile(FileName);
        }

        // TODO: validation
        public void LoadFile(String FileName)
        {
            // Getting start and end times for data file
            String[] digits = Regex.Split(FileName, @"\D+");
            StartTime = long.Parse(digits[0]);
            EndTime = long.Parse(digits[1]);

            // Loading JSON string from file
            String escapedJsonString = File.ReadAllText(DataPath + FileName);
            // First deserialise: removing escape characters. Each string is from single time
            List<String> unescapedJsonString = JsonConvert.DeserializeObject<List<String>>(escapedJsonString);
            // Second deserialise: Making list of input points
            DataList = unescapedJsonString.Select(Str => JsonConvert.DeserializeObject<InputDataModel>(Str)).ToList();
        }
    }
}
