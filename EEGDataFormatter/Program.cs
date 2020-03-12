using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace EEGDataFormatter
{
    class Program
    {
        public const String DataFolder = @"C:\Users\cmcdo\Documents\BA Project documents\";
        public const String WriteFolder = @"C:\Users\cmcdo\Documents\MATLAB\BA project\Data\";

        static void Main(string[] args)
        {
            // TODO: folder select dialog for data and write folders
            int opt = 0;

            while (opt != 3)
            {
                Console.WriteLine("Enter the experiment number to read: ");
                int expNo = int.Parse(Console.ReadLine());
                
                Console.WriteLine("Select: 1 - convert snippets to .mat; 2 - convert all data to .mat, 3 - exit");
                opt = int.Parse(Console.ReadLine());

                if (opt == 1)
                {
                    WriteSnippets(expNo);
                }
                else if (opt == 2)
                {
                    WriteFullExp(expNo);
                }
                else if (opt == 3)
                {
                    Console.WriteLine("Exiting...");
                }
            }
        }

        public static void WriteFullExp(int expNo)
        {
            // Creating folder to write data
            String dataPath = DataFolder + expNo + @"\";
            String writePath = WriteFolder + expNo + @"_unproc\";
            System.IO.Directory.CreateDirectory(writePath);

            // start and end times of experiment
            long[] startEnd = GetExpStartEnd(dataPath);

            // new EEG input/output object
            EEGDataIO dataIO = new EEGDataIO(dataPath, startEnd[0]);

            // loading data
            dataIO.LoadData(0, startEnd[1] - startEnd[0]);

            // writing to file
            dataIO.WriteToMat(writePath + "full");
        }

        // Writing snippets
        public static void WriteSnippets(int expNo)
        {

            String dataPath = DataFolder + expNo + @"\";
            String writePath = WriteFolder + expNo + @"_unproc\";

            // Creating folder to write
            System.IO.Directory.CreateDirectory(writePath);

            // Loading CV file
            CVFile cvf = LoadCVFile(dataPath);

            // Getting list of questions (3 snippets probably)
            List<CVBasedQuestionList> cvList = cvf.CVBasedQuestionList;

            // For each CV
            for (int i = 0; i < cvList.Count; i++)
            {
                List<CVBasedQuestion> snippetList = cvList[i].CVBasedQuestions; // get list of snippets (4)

                // For each snippet in CV
                for (int j = 0; j < snippetList.Count; j++)
                {
                    // start and end times
                    long start = snippetList[j].TimeStamp; long end = snippetList[j].CompletedTimeStamp;

                    // new IO object
                    EEGDataIO dataIO = new EEGDataIO(dataPath, GetExpStartEnd(dataPath)[0]);

                    dataIO.LoadData(start, end);

                    // Writing all data to .mat
                    dataIO.WriteToMat(writePath + "CV_" + (i + 1) + "_snippet_" + (j + 1));
                }
            }

        }

        public static long[] GetExpStartEnd(String dataPath)
        {
            // Loading JSON string from file
            String fPath = dataPath + @"experiment\GeneralAppInfo.json";
            String jsonString = File.ReadAllText(fPath);
            AppInfoFile appInfoFile = JsonConvert.DeserializeObject<AppInfoFile>(jsonString);

            return new long[] { appInfoFile.SystemStartTime, appInfoFile.SystemEndTime };
        }

        public static CVFile LoadCVFile(String dataPath)
        {
            // Loading JSON string from file
            String fPath = dataPath + @"experiment\CVB\CVBTest.json";
            String jsonString = File.ReadAllText(fPath);
            return JsonConvert.DeserializeObject<CVFile>(jsonString);
        }

    }
}
