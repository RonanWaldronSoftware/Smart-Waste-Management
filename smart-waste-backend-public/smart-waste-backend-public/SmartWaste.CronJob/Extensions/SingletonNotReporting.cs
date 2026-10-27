namespace SmartWaste.CronJob.Extensions
{
    public sealed class SingletonNotReporting
    {
        // List of reported units
        private List<(int unitId, DateTime sendDate)> reportedUnits = new List<(int, DateTime)>();

        public SingletonNotReporting() { }

        /// <summary>
        /// Replace reported units list with a new one
        /// </summary>
        /// <param name="newReportedUnits"></param>
        public void ReplaceReportedUnits(List<(int unitId, DateTime sendDate)> newReportedUnits)
        {
            reportedUnits = newReportedUnits;
        }

        /// <summary>
        /// Get a list of reported units
        /// </summary>
        /// <returns></returns>
        public List<(int unitId, DateTime sendDate)> GetReportedUnits()
        {
            return new List<(int unitId, DateTime sendDate)>(reportedUnits);
        }
    }
}
