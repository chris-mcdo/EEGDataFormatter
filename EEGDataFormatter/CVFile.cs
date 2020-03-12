using System;
using System.Collections.Generic;
using System.Text;

namespace EEGDataFormatter
{
    class CVFile
    {
        public string Name { get; set; }
        public int CurrentIntroQuestion { get; set; }
        public int CurrentQuestion { get; set; }
        public double DocumentHeight { get; set; }
        public double DocumentWidth { get; set; }
        public List<CVBasedQuestionList> CVBasedQuestionList { get; set; }

    }

    class CVBasedQuestionList
    {
        public CVBasedIntroQuestion CVBasedIntroQuestion { get; set; }
        public List<CVBasedQuestion> CVBasedQuestions { get; set; }
    }

    class CVBasedIntroQuestion
    {
        public int TimeStamp { get; set; }
        public int CompletedTimeStamp { get; set; }
        public double Rating { get; set; }
        public string CenterTitle { get; set; }
    }

    class CVBasedQuestion
    {
        public int TimeStamp { get; set; }
        public int CompletedTimeStamp { get; set; }
        public bool Completed { get; set; }
    }
}
