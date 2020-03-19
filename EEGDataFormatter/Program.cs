using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace EEGDataFormatter
{
    class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            
            int opt = 0;

            while (opt != 3)
            {
                Console.WriteLine("Select: 1 - convert snippets to .mat; 2 - convert all data to .mat, 3 - exit");
                opt = int.Parse(Console.ReadLine());

                if (opt == 1)
                {
                    WriteSnippets();
                }
                else if (opt == 2)
                {
                    WriteFullExp();
                }
                else if (opt == 3)
                {
                    Console.WriteLine("Exiting...");
                }
            }
            
        }

        public static void WriteFullExp()
        {
            // Get directories to read and write from
            String[] dirReadWrite = GetReadWriteDirectories();

            String dataDir = dirReadWrite[0];
            String writeDir = dirReadWrite[1];

            // start and end times of experiment
            long[] startEnd = GetExpStartEnd(dataDir);

            // new EEG input/output object
            EEGDataIO dataIO = new EEGDataIO(dataDir, startEnd[0]);

            // loading data
            dataIO.LoadData(0, startEnd[1] - startEnd[0]);

            // writing to file
            dataIO.WriteToMat(writeDir + "full.mat");
        }

        public static void WriteSnippets()
        {
            // Get directories to read and write from
            String[] dirReadWrite = GetReadWriteDirectories();

            String dataDir = dirReadWrite[0];
            String writeDir = dirReadWrite[1]; 

            // Loading CV file
            CVFile cvf = LoadCVFile(dataDir);

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

                    EEGDataIO dataIO = new EEGDataIO(dataDir, GetExpStartEnd(dataDir)[0]);

                    dataIO.LoadData(start, end);

                    dataIO.WriteToMat(writeDir + "CV_" + (i + 1) + "_snippet_" + (j + 1) + ".mat");
                }
            }

        }

        public static long[] GetExpStartEnd(String dataPath)
        {
            // Loading JSON string from file
            String fPath = dataPath + @"..\experiment\GeneralAppInfo.json";
            String jsonString = File.ReadAllText(fPath);
            AppInfoFile appInfoFile = JsonConvert.DeserializeObject<AppInfoFile>(jsonString);

            return new long[] { appInfoFile.SystemStartTime, appInfoFile.SystemEndTime };
        }

        public static CVFile LoadCVFile(String dataPath)
        {
            // Loading JSON string from file
            String fPath = dataPath + @"..\experiment\CVB\CVBTest.json";
            String jsonString = File.ReadAllText(fPath);
            return JsonConvert.DeserializeObject<CVFile>(jsonString);
        }

        public static String[] GetReadWriteDirectories()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            // Data directory
            do
            {
                Console.WriteLine("Navigate to the folder containing the EEG data (press enter to show dialog)...");
                Console.ReadLine();
            } while (!fbd.ShowDialog().Equals(DialogResult.OK));
            String dataDir = fbd.SelectedPath + @"\";

            // Get experiment number from data directory
            int expNo = int.Parse(new DirectoryInfo(dataDir + @"..\").Name);

            do
            {
                Console.WriteLine("Navigate to the folder to write the data to, " +
                    "i.e. containing MATLAB resampling/pre-processing scripts " +
                    "(press enter to show dialog)...");
                Console.ReadLine();
            } while (!fbd.ShowDialog().Equals(DialogResult.OK));
            String writeDir = fbd.SelectedPath + @"\"+expNo+@"_unproc\";

            // Creating folder to write
            System.IO.Directory.CreateDirectory(writeDir);

            return new string[]{ dataDir, writeDir };
        }
    }
}
