using System;
using System.Collections.Generic;

namespace HireZ.DTOs
{
    public class PipelineStageCount
    {
        public string Stage { get; set; } = "";
        public int Count { get; set; }
    }

    public class AnalyticsOverviewDto
    {
        public int TotalUsers { get; set; }
        public int ResumesUploaded { get; set; }
        public int ResumesWithTextExtracted { get; set; }
        public int ResumesAnalyzed { get; set; } // feedback exists
        public int InterviewSessions { get; set; }
        public double AvgTimeToAnalysisDays { get; set; } // days
        public List<PipelineStageCount> Pipeline { get; set; } = new();
    }
}
